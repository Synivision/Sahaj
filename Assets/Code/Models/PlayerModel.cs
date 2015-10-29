using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerModel : IGameDataModel {
	
	public string Name{get;set;}
	public string Email{get;set;}
	public int Gold{get;set;}
	public int MaxGoldCapacity{get;set;}
	public Image UserIcon{get;set;}
	public int ExperiencePoints{get;set;}
	public int UserLevel{get;set;}
	public int UserRank{get;set;}
	public Dictionary<string, bool> UnlockedPirates{get;set;}
	public Dictionary<string, bool> UnlockedShipAttacks { get; set; }
	public Dictionary<string, int> ShipAttacksCountDict { get; set; }
    public Dictionary<string, int> ShipAttackCostDict { get; set; }
	public int LevelUnLocked{get;set;}
	public int Wins{get;set;}
	public int Gems{get;set;}
	public Dictionary<string,int> PirateCountDict{ get; set;}
	public int ShipBulletsAvailable { get; set; }
}
