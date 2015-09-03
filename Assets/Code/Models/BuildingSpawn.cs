using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;


public class BuildingSpawn : IGameDataModel {

	//name = Spritename
	public string Name {get; set;}
	public int xGridCoord;
	public int zGridCoord;
}
