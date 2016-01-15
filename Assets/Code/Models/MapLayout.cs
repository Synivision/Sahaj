using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class MapLayout : IGameDataModel {

	public List<BuildingSpawn> buildingSpawnList = new List<BuildingSpawn>();
	public List<MapItemSpawn> mapItemSpawnList = new List<MapItemSpawn>();
	public string Name {get; set;}
}
