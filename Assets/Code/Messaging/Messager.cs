using System;
using System.Collections.Generic;
using Assets.Code.DataPipeline;

namespace Assets.Code.Messaging
{
    public class Messager : IResolvableItem
    {
        private readonly Dictionary<Type, MessageQueue> _payload;

        public Messager()
        {
            _payload = new Dictionary<Type, MessageQueue>();
        }

        public MessagingToken Subscribe<T>(Action<T> action) where T : class, IMessage
        {
            var messageType = typeof (T);
            if (!_payload.ContainsKey(messageType))
                _payload.Add(messageType, new MessageQueue());

            var tokenId = _payload[messageType].AddToFireList(message => action(message as T));

            return new MessagingToken
            {
                SubscriptionType = messageType,
                SubscriptionId = tokenId
            };
        }

        public void Publish(IMessage message)
        {
            var targetType = message.GetType();

            if (!_payload.ContainsKey(targetType))
                return;

            _payload[targetType].Fire(message);
        }

        public void CancelSubscription(params MessagingToken[] tokens)
        {
            foreach (var token in tokens)
                _payload[token.SubscriptionType].RemoveFromFireList(token.SubscriptionId);
        }
    }
}
