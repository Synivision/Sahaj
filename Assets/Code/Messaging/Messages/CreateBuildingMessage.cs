
using System.Collections;
using UnityEngine;

namespace Assets.Code.Messaging.Messages
{
    public class CreateBuildingMessage : IMessage
    {
        public string BuildingName;
        public Vector3 SpawnPosition;

    }
}
