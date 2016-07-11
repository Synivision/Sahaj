

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Code.Messaging
{

    public class RowBoatSelectedMessage : IMessage
    {
        public string BoatName;
        public Action onCancelled; 

    }
}
