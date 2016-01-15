using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;

using System;

[Serializable]
public class MapItemSpawn : IGameDataModel {

	public string Name {get; set;}
	public int xGridCoord;
	public int zGridCoord;

    public MapItemSpawn(string name, int xGridCoord, int zGridCoord) {
        this.Name = name;
        this.xGridCoord = xGridCoord;
        this.zGridCoord = zGridCoord;

    }

    public MapItemSpawn() {

    }
}
