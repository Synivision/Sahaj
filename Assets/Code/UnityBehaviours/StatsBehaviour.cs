using UnityEngine;
using System.Collections;

public class StatsBehaviour {
	StatBlock stats;
	
	private float _currentDamage;
	private float _currentHealth;
	private float _currentCourage;

	private PirateModel _pirateModel;
	
	public  StatsBehaviour(PirateModel pirateModel){

		_pirateModel = pirateModel;
		_currentHealth = _pirateModel.Health;
		_currentDamage = _pirateModel.AttackDamage;
		_currentCourage = _pirateModel.Courage;
		
	}
	
	public void ApplyDamage(float damage){

		_currentHealth -= damage;
		//Debug.Log("Current Health of Pirate : " + _currentHealth.ToString());
	}
	
	public void Regenerate(){}
	
	public void Courage(){}
	
	public void UpgradeHealth(float newHeallth){
		
		stats.MaximumHealth = newHeallth;
		
	}

	public float CurrentHealth{get{

			return _currentHealth;
		} set{
			_currentHealth = value;
		}}
}
