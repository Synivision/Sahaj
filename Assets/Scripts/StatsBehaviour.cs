using UnityEngine;
using System.Collections;

public class StatsBehaviour : MonoBehaviour {
	StatBlock stats;
	
	private float _currentDamage;
	private float _currentHealth;
	private float _currentCourage;
	
	void Start(){
		
//		_currentHealth = stats.MaximumHealth;
		//_currentCourage = stats.MaximumCourage;
		//_currentDamage = 0;
		_currentHealth = 100;
		_currentDamage = 10;
		_currentCourage = 1;
		
	}
	
	public void ApplyDamage(){
		
		if (_currentDamage > _currentHealth) {
			Debug.Log ("Dead");
		} else {
			_currentHealth -= _currentDamage;
			Debug.Log("Current Health : "+ _currentHealth.ToString());
		}
	}
	
	public void Regenerate(){}
	
	public void Courage(){}
	
	public void UpgradeHealth(float newHeallth){
		
		stats.MaximumHealth = newHeallth;
		
	}
	
	
}
