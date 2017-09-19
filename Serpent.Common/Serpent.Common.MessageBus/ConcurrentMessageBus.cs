﻿namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ConcurrentMessageBus<TMessageType> : IMessageBus<TMessageType>
    {
        private readonly ConcurrentMessageBusOptions<TMessageType> options = ConcurrentMessageBusOptions<TMessageType>.Default;

        private readonly ExclusiveAccess<int> currentSubscriptionId = new ExclusiveAccess<int>(0);

        private readonly Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishAsyncFunc;

        private readonly ConcurrentQueue<int> recycledSubscriptionIds = new ConcurrentQueue<int>();

        private readonly ConcurrentDictionary<int, ISubscription<TMessageType>> subscribers = new ConcurrentDictionary<int, ISubscription<TMessageType>>();

        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options)
        {
            this.options = options;
            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        public ConcurrentMessageBus(Action<ConcurrentMessageBusOptions<TMessageType>> optionsFunc)
        {
            var options = new ConcurrentMessageBusOptions<TMessageType>();
            optionsFunc(options);
            this.options = options;
            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        public ConcurrentMessageBus()
        {
            if (this.options.BusPublisher == null)
            {
                throw new Exception("No BusPublisher must not be null");
            }

            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options, Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishFunction)
        {
            this.options = options;
            this.publishAsyncFunc = publishFunction;
        }

        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options, BusPublisher<TMessageType> publisher)
        {
            this.options = options;
            this.publishAsyncFunc = publisher.PublishAsync;
        }

        public int SubscriberCount => this.subscribers.Count;

        public Task PublishAsync(TMessageType message) => this.publishAsyncFunc(this.subscribers.Values, message);

        public IMessageBusSubscription Subscribe(Func<TMessageType, Task> invocationFunc)
        {
            var subscription = this.CreateSubscription(invocationFunc);

            var newSubscriptionId = this.GetNewSubscriptionId();

            this.subscribers.TryAdd(newSubscriptionId, subscription);

            return this.CreateMessageBusSubscription(newSubscriptionId);
        }

        private ConcurrentMessageBusSubscription CreateMessageBusSubscription(int newSubscriptionId)
        {
            return new ConcurrentMessageBusSubscription(() => this.Unsubscribe(newSubscriptionId));
        }

        private ISubscription<TMessageType> CreateSubscription(Func<TMessageType, Task> subscriptionHandlerFunc)
        {
            return this.options.SubscriptionReferenceType != SubscriptionReferenceTypeType.WeakReferences
                       ? new StrongReferenceSubscription(subscriptionHandlerFunc)
                       : (ISubscription<TMessageType>)new WeakReferenceSubscription(subscriptionHandlerFunc);
        }

        private int GetNewSubscriptionId()
        {
            if (!this.recycledSubscriptionIds.TryDequeue(out var result))
            {
                result = this.currentSubscriptionId.Increment();
            }

            return result;
        }

        private void Unsubscribe(int subscriptionId)
        {
            this.currentSubscriptionId.Update(
                v =>
                    {
                        if (this.subscribers.TryRemove(subscriptionId, out _))
                        {
                            if (subscriptionId == v)
                            {
                                --v;
                            }
                            else
                            {
                                this.recycledSubscriptionIds.Enqueue(subscriptionId);
                            }
                        }

                        return v;
                    });
        }

        private struct ConcurrentMessageBusSubscription : IMessageBusSubscription
        {
            private static readonly Action DoNothing = () => { };

            private Action unsubscribeAction;

            public ConcurrentMessageBusSubscription(Action unsubscribeAction)
            {
                this.unsubscribeAction = unsubscribeAction;
            }

            public void Unsubscribe()
            {
                this.unsubscribeAction.Invoke();
                this.unsubscribeAction = DoNothing;
            }

            public void Dispose()
            {
                this.Unsubscribe();
            }
        }

        private struct StrongReferenceSubscription : ISubscription<TMessageType>
        {
            public StrongReferenceSubscription(Func<TMessageType, Task> subscriptionHandlerFunc)
            {
                this.SubscriptionHandlerFunc = subscriptionHandlerFunc;
            }

            public Func<TMessageType, Task> SubscriptionHandlerFunc { get; set; }
        }

        private struct WeakReferenceSubscription : ISubscription<TMessageType>
        {
            private readonly WeakReference<Func<TMessageType, Task>> subscriptionHandlerFunc;

            public WeakReferenceSubscription(Func<TMessageType, Task> subscriptionHandlerFunc)
            {
                this.subscriptionHandlerFunc = new WeakReference<Func<TMessageType, Task>>(subscriptionHandlerFunc);
            }

            public Func<TMessageType, Task> SubscriptionHandlerFunc
            {
                get
                {
                    if (this.subscriptionHandlerFunc.TryGetTarget(out var target))
                    {
                        return target;
                    }

                    return null;
                }
            }
        }
    }
}