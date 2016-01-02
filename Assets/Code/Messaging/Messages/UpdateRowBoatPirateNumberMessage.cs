using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.Messaging
{

    public class UpdateRowBoatPirateNumberMessage : IMessage
    {

        public string BoatName;
        public int PirateNumber;
    }
}
