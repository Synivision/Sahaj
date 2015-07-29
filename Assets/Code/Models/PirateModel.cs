using UnityEngine;
using System.Collections;
using Assets.Code.Models;
public class PirateModel : IGameDataModel{
	
	public enum Nature{ Player=0, Enemy=1};
	public enum Range{
		
		Milee=20,
		Gunner1=30,
		Gunner2=40,
		Gunner3=50
	};
	
	public int PirateRange{ get; set;}
	public Color PirateColor{ get; set;}
	public int PirateNature{ get; set;}
	public int Health{ get; set;}
	public int AttackDamage{ get; set;}
	public string PirateName{ get; set;}
	public int Courage{ get; set;}
	public string Descipriton{ get; set;}
	public string Weapon{ get; set;}
	public int Level{get; set;}
	public int MovementSpeed{get; set;}
	public int TrainingCost{get; set;}
	public float TrainingTime{get; set;}
	public string Name {get; set;}
}
