using UnityEngine;
using System.Collections;
using Assets.Code.Models;

public class InputSessionData : IGameDataModel{

	public string Name {get; set;}
	//public string CurrentPirateData{get;set;}
	public string ShipAttackName { get; set; }
    public int ShipAttackCost { get; set; }
    public string RowBoatName { get; set; }
}
