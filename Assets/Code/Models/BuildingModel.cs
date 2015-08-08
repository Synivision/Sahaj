using UnityEngine;
using Assets.Code.Models;

public class BuildingModel : IGameDataModel {

	public string Name {get; set;}
    public StatBlock Stats { get; set; }
	public string Descipriton{ get; set;}
	public BuildingType Type{get;set;}
	public enum BuildingType{ 
		Gold_Locker,
		Captains_Cabin,
		Carpenter_Cabin,
		Surgeons_Cabin,
		Armory,
		Cannon_Port,
		Gun_Port_Deck,
		Defence_Gunner_Towers,
		Defence_Cannons,
		Defence_Platoons,
		Defence_Tear_Gas,
		Defence_Water_Cannons
	};

	public int Level{get;set;}
	public int Storage{get;set;}
	public int Range{get;set;}
	public int TileArea{get;set;}
	public Color BuildingColor{ get; set;}
	public bool CanAttack {get;set;}


}
