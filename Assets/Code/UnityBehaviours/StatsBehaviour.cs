using UnityEngine;
using System.Collections;

public class StatsBehaviour {
	StatBlock stats;
	
	private float _currentDamage;
	private float _currentCourage;
	
	private PirateModel _pirateModel;
	
	public  StatsBehaviour(PirateModel pirateModel){
		
		_pirateModel = pirateModel;
		CurrentHealth = _pirateModel.Health;
		_currentDamage = _pirateModel.AttackDamage;
		_currentCourage = _pirateModel.Courage;
		
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
