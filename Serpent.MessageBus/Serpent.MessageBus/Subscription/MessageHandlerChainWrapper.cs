﻿//// ReSharper disable CheckNamespace

namespace Serpent.MessageBus
{
    /// <summary>
    ///     The subscription wrapper type. Unsubscribes when disposed or runs out of scope
    /// </summary>
    public struct MessageHandlerChainWrapper : IMessageHandlerChain
    {
        private IMessageHandlerChain messageHandlerChain;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageHandlerChainWrapper" /> class.
        /// </summary>
        /// <param name="messageHandlerChain">
        ///     The subscription
        /// </param>
        public MessageHandlerChainWrapper(IMessageHandlerChain messageHandlerChain)
        {
            this.messageHandlerChain = messageHandlerChain;
        }

        /// <summary>
        ///     Creates a new subscription wrapper
        /// </summary>
        /// <param name="messageHandlerChain">The message handler chain</param>
        /// <returns>A subscription wrapper</returns>
        public static MessageHandlerChainWrapper Create(IMessageHandlerChain messageHandlerChain)
        {
            return new MessageHandlerChainWrapper(messageHandlerChain);
        }

        /// <summary>
        ///     Disposes the object
        /// </summary>
        public void Dispose()
        {
            this.Unsubscribe();
        }

        /// <summary>
        ///     Unsubscribes to the message bus
        /// </summary>
        public void Unsubscribe()
        {
            this.messageHandlerChain?.Dispose();
            this.messageHandlerChain = null;
        }
    }
}