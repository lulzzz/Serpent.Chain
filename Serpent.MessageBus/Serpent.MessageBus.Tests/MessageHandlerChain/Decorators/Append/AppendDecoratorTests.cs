﻿// ReSharper disable InconsistentNaming
// ReSharper disable AccessToModifiedClosure

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Append
{
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class AppendDecoratorTests
    {
        [Fact]
        public async Task Append_Normal_Tests()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            var counter = 0;

            using (bus.Subscribe(b => b.Append(msg => msg).Handler(message => { Interlocked.Add(ref counter, message.AddValue); })).Wrapper())
            {
                for (var i = 0; i < 100; i++)
                {
                    await bus.PublishAsync(new Message1(1 + (i % 2)));
                }

                Assert.Equal(300, counter);
            }

            counter = 0;

#pragma warning disable 1998 
            using (bus.Subscribe(b => b.Append(async msg => msg).Handler(async message => { Interlocked.Add(ref counter, message.AddValue); })).Wrapper())
#pragma warning restore 1998
            {
                for (var i = 0; i < 100; i++)
                {
                    await bus.PublishAsync(new Message1(1 + (i % 2)));
                }

                Assert.Equal(300, counter);
            }
        }

        private class Message1
        {
            public Message1(int addValue)
            {
                this.AddValue = addValue;
            }

            public int AddValue { get; }
        }
    }
}