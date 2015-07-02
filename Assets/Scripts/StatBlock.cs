using UnityEngine;
using System.Collections;

public delegate void OnMaximumHealthChangedEventHandler(float oldValue, float newValue, float delta);
public delegate void OnMaximumCourageChangedEventHandler(float oldValue, float newValue, float delta);
public delegate void OnMaximumDamageChangedEventHandler(float oldValue, float newValue, float delta);

public class StatBlock
{
	
	private float _maximumHealth;
	public OnMaximumHealthChangedEventHandler OnMaximumHealthChangedEvent;
	
	public float MaximumHealth { 
		get{
			return _maximumHealth;
		} 
		set{
			
			if (OnMaximumHealthChangedEvent != null)
				OnMaximumHealthChangedEvent(_maximumHealth, value, value - _maximumHealth);
			
			_maximumHealth = value;
			//print("stat block health" +_maximumHealth.ToString());
			
		}
	}
	
	private float _maximumCourage;
	public OnMaximumCourageChangedEventHandler OnMaximumCourageChangedEvent;
	
	public float MaximumCourage{
		get{
			
			return _maximumCourage;
		}
		set{
			
			if(OnMaximumCourageChangedEvent != null){
				
				OnMaximumCourageChangedEvent(_maximumCourage, value, value - _maximumCourage);
				
			}
		}
		
		
	}
	
	
	
	private float _maximumDamage;
	public OnMaximumDamageChangedEventHandler OnMaximumDamageChangedEvent;
	
	public float MaximumDamage { 
		get{return _maximumDamage;} 
		set{
			if(OnMaximumDamageChangedEvent!=null){
				
				OnMaximumDamageChangedEvent(_maximumDamage,value,value - _maximumDamage);
				
			}
		} 
	}
	
}