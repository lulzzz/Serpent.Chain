﻿// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.LimitedThroughput
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.LimitedThroughput;

    using Xunit;

    public class LimitedThroughputWireUpTests
    {
        private const int DelayMultiplier = 10;

        [Fact]
        public async Task LimitedThroughput_WireUp_Attribute_Test()
        {
            var handler = new LimitedThroughputMessageHandler();

            var func = Create.SimpleFunc<LimitedThroughputTestMessage>(s => s.SoftFireAndForget().WireUp(handler));

            for (var i = 0; i < 100; i++)
            {
                await func(new LimitedThroughputTestMessage("1"));
            }

            await Task.Delay(DelayMultiplier * 110);

            Assert.Equal(20, handler.Count);

            await Task.Delay(DelayMultiplier * 410);
            Assert.Equal(60, handler.Count);
        }

        [Fact]
        public async Task TestWireUpFromConfiguration()
        {
            var wireUp = new LimitedThroughputWireUp();

            var config = wireUp.CreateConfigurationFromDefaultValue("10");

            var limitedThroughputConfiguration = config as LimitedThroughputConfiguration;
            Assert.NotNull(limitedThroughputConfiguration);

            Assert.Equal(10, limitedThroughputConfiguration.MaxNumberOfMessagesPerPeriod);
            Assert.Equal(TimeSpan.FromSeconds(1), limitedThroughputConfiguration.Period);

            var handler = new LimitedThroughputMessageHandler();

            var func = Create.SimpleFunc<LimitedThroughputTestMessage>(b => b.SoftFireAndForget().WireUp(handler, new[] { config }));

            Assert.Equal(0, handler.Count);

            for (var i = 0; i < 100; i++)
            {
#pragma warning disable 4014
                func(new LimitedThroughputTestMessage("1"));
#pragma warning restore 4014
            }

            await Task.Delay(DelayMultiplier * 110);

            Assert.Equal(20, handler.Count);

            await Task.Delay(DelayMultiplier * 410);
            Assert.Equal(60, handler.Count);
        }
    }
}