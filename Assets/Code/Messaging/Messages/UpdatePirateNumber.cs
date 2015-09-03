using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.Messaging{

	public class UpdatePirateNumber : IMessage {

		public string PirateName;
		public int PirateNumber;
	}
}