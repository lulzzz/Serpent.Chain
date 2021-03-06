﻿namespace Serpent.Chain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Notification;

    internal struct TakeWhileDecoratorConfiguration<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> HandlerFunc { get; set; }

        public Func<TMessageType, bool> Predicate { get; set; }

        public ChainBuilderSetupServices Services { get; set; }
    }
}