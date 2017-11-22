﻿namespace Serpent.MessageHandlerChain.Decorators.Retry
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class RetryDecoratorBuilder<TMessageType> : IRetryDecoratorBuilder<TMessageType>
    {
        public int MaximumNumberOfAttempts { get; set; }

        public TimeSpan RetryDelay { get; set; }

        public Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> HandlerFailedFunc { get; set; }

        public Func<TMessageType, int, int, TimeSpan, Task> HandlerSucceededFunc { get; set; }
    }
}