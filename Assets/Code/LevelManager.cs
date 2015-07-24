using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.States;
using System.Linq;
using Assets.Code.Messaging.Messages;
using Assets.Code.DataPipeline;

public delegate void OnPirateGeneratedEventHandler();


public class LevelManager 
{
	public OnPirateGeneratedEventHandler OnPirateGeneratedEvent;


	private readonly PrefabProvider _prefabProvider;
	private readonly Messager _messager;
	private readonly PoolingBehaviour _poolingbehviour;
	PoolingObjectManager _poolingObjectManager;
	private List<PirateController> _knownPirates;
	private readonly IoCResolver _resolver;

	PirateController pirateObject;
	
	public LevelManager (IoCResolver resolver)
	{
		_resolver = resolver;

		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve (out _prefabProvider);

		_poolingObjectManager.Instantiate ("Cube");
		_poolingObjectManager.Instantiate ("AStarPlane");
		_knownPirates = new List<PirateController> ();
	}

	public List<PirateController> GetKnownPirates(){

		return _knownPirates;	

	}

	public void CreatePirate (string pirateName)
	{
	    pirateObject = (PirateController)_poolingObjectManager.Instantiate ("Sphere");

		_knownPirates.Add(pirateObject);


		pirateObject.Initialize(_resolver,GeneratePirateModel(pirateName),this);

	
		pirateObject.transform.position = new Vector3(Random.Range(-100,0),10,Random.Range(-100,0)); 

		if(OnPirateGeneratedEvent!=null){
			OnPirateGeneratedEvent();
		}

		//Debug.Log ("Known Pirates : in create " + _knownPirates.Count.ToString ());

	}

	public PirateModel GeneratePirateModel(string pirateName){

		PirateModel pirateModel = new PirateModel ();
		switch (pirateName){
			case"Pirate1":
				pirateModel = GeneratePirate1(pirateModel);
				break;
			case"Pirate2":
				pirateModel = GeneratePirate2(pirateModel);
				break;
			case"Pirate3":
				pirateModel = GeneratePirate3(pirateModel);
				break;
			case"EnemyPirate1":
				pirateModel = GenerateEnemy1(pirateModel);
				break;
			case"EnemyPirate2":
				pirateModel = GenerateEnemy2(pirateModel);
				break;
			case"EnemyPirate3":
				pirateModel = GenerateEnemy3(pirateModel);
				break;

		}
		return pirateModel;

	}

	public PirateModel GeneratePirate1(PirateModel pirateModel){

		//Captain 
		pirateModel.Health = 200;
		pirateModel.Descipriton = "Control the crew and gives orders";
		pirateModel.AttackDamage = 25;
		pirateModel.Name = "Captain" + Random.Range (0, 100).ToString ();
		pirateModel.Courage = 5;
		pirateModel.PirateNature = (int)PirateModel.Nature.Player ;
		pirateModel.PirateRange = (int)PirateModel.Range.Gunner3;
		pirateModel.PirateColor = Color.blue;

		return pirateModel;
	}
	public PirateModel GeneratePirate2(PirateModel pirateModel){

		//Quarter Master

		pirateModel.Health = 150;
		pirateModel.Descipriton = "Second in command : swords man";
		pirateModel.AttackDamage = 20;
		pirateModel.Name = "Quarter Master" + Random.Range (0, 100).ToString();
		pirateModel.Courage = 4;
		pirateModel.PirateNature = (int)PirateModel.Nature.Player ;
		pirateModel.PirateRange = (int)PirateModel.Range.Gunner3;
		pirateModel.PirateColor = Color.green;
		
		return pirateModel;
	}

	public PirateModel GeneratePirate3(PirateModel pirateModel){

		//Gunner
	 
		pirateModel.Health = 100;
		pirateModel.Descipriton = "Attacks and defends the ship from the gun port on deck";
		pirateModel.AttackDamage = 15;
		pirateModel.Name = "Gunner" + Random.Range (0, 100).ToString ();
		pirateModel.Courage = 3;
		pirateModel.PirateRange = (int)PirateModel.Range.Gunner2;
		pirateModel.PirateNature = (int)PirateModel.Nature.Player ;
		pirateModel.PirateColor = Color.yellow;
		
		return pirateModel;
	}

	private PirateModel GenerateEnemy1(PirateModel pirateModel){
		
		pirateModel.Health = 120;
		pirateModel.Descipriton = "Milee Enemy Pirate";
		pirateModel.AttackDamage = 10;
		pirateModel.Name = "Enemy " + Random.Range (0, 100).ToString ();
		pirateModel.Courage = 3;
		pirateModel.PirateRange = (int)PirateModel.Range.Milee;
		pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
		pirateModel.PirateColor = Color.red;
		
		return pirateModel;
	}
	
	private PirateModel GenerateEnemy2(PirateModel pirateModel){
		
		pirateModel.Health = 150;
		pirateModel.Descipriton = "Gunner1 Enemy Pirate";
		pirateModel.AttackDamage = 8;
		pirateModel.Name = "Enemy " + Random.Range (0, 100).ToString ();
		pirateModel.Courage = 4;
		pirateModel.PirateRange = (int)PirateModel.Range.Gunner1;
		pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
		pirateModel.PirateColor = Color.grey;
		
		return pirateModel;
	}
	
	private PirateModel GenerateEnemy3(PirateModel pirateModel){
		
		pirateModel.Health = 100;
		pirateModel.Descipriton = "Gunner2 Enemy Pirate";
		pirateModel.AttackDamage = 10;
		pirateModel.Name = "Enemy " + Random.Range (0, 100).ToString ();
		pirateModel.Courage = 3;
		pirateModel.PirateRange = (int)PirateModel.Range.Gunner2;
		pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
		pirateModel.PirateColor = Color.black;
		
		return pirateModel;
	}


	public void OnPirateDead(PirateController pirate){

		_knownPirates.Remove(pirate);
		Debug.Log ("Known Pirates : after remove " + _knownPirates.Count.ToString ());

	}

	public void TearDownLevel ()
	{
		UnityEngine.Debug.Log ("level manager");
		_poolingObjectManager.TearDown ();
	}

}


