using UnityEngine;
using System.Collections;

public class PirateModel {

	private int _health;
	private int _attackDamage;
	private string _name;
	private int _courage;
	private string _descipriton;
	private string _weapon;
	private int _level;
	private int _movementSpeed;
	private int _trainingCost;
	private float _trainingTime;


	public int Health{
		get{
			return _health;
		}
		set{
			_health = value;
		}
	}

	
	public int AttackDamage{
		get{
			return _attackDamage;
		}
		set{
			_attackDamage = value;
		}
	}

	public string Name{
		get{
			return _name;
		}
		set{
			_name = value;
		}
	}

	public int Courage{
		get{
			return _courage;
		}
		set{
			_courage = value;
		}
	}

	public string Descipriton{
		get{
			return _descipriton;
		}
		set{
			_descipriton = value;
		}
		
	}

	public string Weapon{
		
		get{return _weapon;}
		set{_weapon = value;}
		
	}
	
	public int Level{
		
		get{return _level;}
		set{_level = value;}
		
	}
	
	public int MovementSpeed{
		
		get{return _movementSpeed;}
		set{_movementSpeed = value;}
		
	}
	
	public int TrainingCost{
		
		get{return _trainingCost;}
		set{_trainingCost = value;}
		
	}
	
	public float TrainingTime{
		
		get{return _trainingTime;}
		set{_trainingTime = value;}
		
	}



}
