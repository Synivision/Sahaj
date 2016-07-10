using UnityEngine;
using System.Collections;
using Assets.Code.UnityBehaviours;

namespace Assets.Code.Messaging.Messages
{
    public class SelectShipAttackMessage : IMessage
    {
        public Color color;
        public int scale;
        public int speed;
        public bool isProjectile;
    }
}
