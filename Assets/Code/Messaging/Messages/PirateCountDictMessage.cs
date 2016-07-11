using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.Messaging.Messages
{

	public class PirateCountDictMessage :  IMessage {
		public Dictionary<string, int> pirateDict;
	
	}
}