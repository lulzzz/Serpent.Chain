﻿namespace Serpent.Chain.Tests.Decorators.Distinct
{
    internal class DistinctTestMessage
    {
        public DistinctTestMessage(string id)
        {
            this.Id = id;
        }

        public string Id { get; }
    }
}