using UnityEngine;
using System.Collections;

namespace Assets.Code.Messaging.Messages
{
	public class NewPirateGeneratedMessage : IMessage
	{
		public PirateModel PirateModel;
	}
}

