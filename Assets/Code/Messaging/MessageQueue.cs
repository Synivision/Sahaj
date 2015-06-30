using System;
using System.Collections.Generic;

namespace Assets.Code.Messaging
{
    class MessageQueue
    {
        private readonly Dictionary<Guid, Action<IMessage>> _actionList;

        public MessageQueue()
        {
            _actionList = new Dictionary<Guid, Action<IMessage>>();
        }

        public Guid AddToFireList(Action<IMessage> action)
        {
            var id = Guid.NewGuid();

            _actionList.Add(id, action);

            return id;
        }

        public void Fire(IMessage message)
        {
            foreach (var action in _actionList)
                action.Value(message);
        }

        public void RemoveFromFireList(Guid id)
        {
            _actionList.Remove(id);
        }
    }
}
