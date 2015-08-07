using UnityEngine;
using System.Collections;

using Assets.Code.Models;
public class StatsBehaviour {
	StatBlock stats;
	
	private float _currentDamage;
	private float _currentCourage;
	
	private PirateModel _pirateModel;
	private BuildingModel _buildingModel;
	public  StatsBehaviour(PirateModel pirateModel){
		
		_pirateModel = pirateModel;
		CurrentHealth = _pirateModel.Health;
		_currentDamage = _pirateModel.AttackDamage;
		_currentCourage = _pirateModel.Courage;
		
	}
	
	public  StatsBehaviour(BuildingModel buildingModel){
		
		_buildingModel = buildingModel;
		CurrentHealth = _buildingModel.Health;


		
	}
	public void ApplyDamage(float damage){
		
		CurrentHealth -= damage;
	}
	
	public void Regenerate(){}
	
	public void Courage(){}
	
	public void UpgradeHealth(float newHeallth){
		
		stats.MaximumHealth = newHeallth;
		
	}
	
	public float CurrentHealth{get; set;}
}
