using UnityEngine;

namespace Assets.Code.Messaging.Messages
{
	public class CreatePirateMessage : IMessage
	{
		public string PirateName;
		public Vector3 SpawnPosition;
	}
	
}