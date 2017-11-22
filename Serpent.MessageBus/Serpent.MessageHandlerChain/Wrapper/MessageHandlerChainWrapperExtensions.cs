﻿// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    /// <summary>
    /// Provides extensions for subscription wrapper.
    /// </summary>
    public static class MessageHandlerChainWrapperExtensions 
    {
        /// <summary>
        /// Creates a new subscription wrapper for this subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        /// <returns>A new subscription wrapper</returns>
        public static MessageHandlerChainWrapper Wrapper(this IMessageHandlerChain subscription)
        {
            return new MessageHandlerChainWrapper(subscription);
        }
    }
}