using UnityEngine;
using System.Collections;
using System;


namespace Assets.Code.Messaging
{

	public class OpenBuildingMenuMessage : IMessage
	{
		public string BuildingName;
		public Action onMove;
		public BuildingModel Model;
		public Vector3 Position;
	}
}
