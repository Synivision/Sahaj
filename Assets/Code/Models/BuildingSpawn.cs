using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;

using System;

[Serializable]
public class BuildingSpawn : IGameDataModel {

	//name = Spritename
	public string Name {get; set;}
	public int xGridCoord;
	public int zGridCoord;


    public BuildingSpawn()
    {


    }
    public BuildingSpawn(string name, int x, int z) {

        Name = name;
        xGridCoord = x;
        zGridCoord = z;
    }


}
