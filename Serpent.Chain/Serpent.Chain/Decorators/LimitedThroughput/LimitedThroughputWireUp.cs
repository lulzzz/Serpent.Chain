﻿// ReSharper disable StyleCop.SA1126
namespace Serpent.Chain.Decorators.LimitedThroughput
{
    using System;

    using Serpent.Chain.WireUp;

    internal class LimitedThroughputWireUp : BaseWireUp<LimitedThroughputAttribute, LimitedThroughputConfiguration>
    {
        protected override LimitedThroughputConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (int.TryParse(text, out var maxNumberOfMessagesPerPeriod))
            {
                return new LimitedThroughputConfiguration
                {
                    MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod,
                    Period = TimeSpan.FromSeconds(1)
                };
            }

            throw new Exception("LimitedThroughput: Could not convert text to integer " + text);
        }

        protected override bool WireUpFromAttribute<TMessageType, THandlerType>(
            LimitedThroughputAttribute attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.LimitedThroughput(attribute.MaxNumberOfMessagesPerPeriod, attribute.Period);
            return true;
        }

        protected override bool WireUpFromConfiguration<TMessageType, THandlerType>(
            LimitedThroughputConfiguration configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.LimitedThroughput(configuration.MaxNumberOfMessagesPerPeriod, configuration.Period);
            return true;
        }
    }
}