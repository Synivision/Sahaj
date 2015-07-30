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
	GameDataProvider _gameDataProvider;

	PirateController pirateObject;
	PoolingBehaviour allPiratesObject;
	private BuildingModel model,model2;
	public LevelManager (IoCResolver resolver)
	{
		_resolver = resolver;

		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);


		allPiratesObject = (PoolingBehaviour)_poolingObjectManager.Instantiate ("AllPirates");

		_knownPirates = new List<PirateController> ();

		GenerateLevelMap();
	}

	public void GenerateLevelMap(){
		_poolingObjectManager.Instantiate ("AStarPlane");


		var building1 = _poolingObjectManager.Instantiate ("Building") as BuildingController;
		model = new BuildingModel();

		model.Name = "Building1";
		model.BuildingColor = Color.gray;
		model.Range = 50;

		building1.Initialize(_resolver, model,this);
		building1.transform.position = new Vector3(50,building1.transform.position.y,-20);

		var building2 = _poolingObjectManager.Instantiate ("Building") as BuildingController;
		model2 = new BuildingModel();
		model2.Name = "Building2";
		model2.BuildingColor = Color.red;
		model2.Range = 50;

		building2.Initialize(_resolver, model2,this);
		building2.transform.position = new Vector3(-85,building1.transform.position.y,-85);


	}

	public List<PirateController> GetKnownPirates(){

		return _knownPirates;	

	}

	public void CreatePirate (string pirateName, Vector3 spawnposition)
	{
	    pirateObject = (PirateController)_poolingObjectManager.Instantiate ("Sphere");

		_knownPirates.Add(pirateObject);


		pirateObject.Initialize(_resolver,GeneratePirateModel(pirateName),this);
			
		pirateObject.transform.position = spawnposition;
		pirateObject.transform.SetParent(allPiratesObject.transform);
		if(OnPirateGeneratedEvent!=null){
			OnPirateGeneratedEvent();
		}

	}

	public PirateModel GeneratePirateModel(string pirateName){

		PirateModel pirateModel = new PirateModel ();
		switch (pirateName){
			case"Pirate1":
				pirateModel = _gameDataProvider.GetData<PirateModel>("Pirate1");
				break;
			case"Pirate2":
				pirateModel = _gameDataProvider.GetData<PirateModel>("Pirate2");
				break;
			case"Pirate3":
				pirateModel = _gameDataProvider.GetData<PirateModel>("Pirate3");
				break;
			case"EnemyPirate1":
				pirateModel = _gameDataProvider.GetData<PirateModel>("EnemyPirate1");
				break;
			case"EnemyPirate2":
				pirateModel = _gameDataProvider.GetData<PirateModel>("EnemyPirate2");
				break;
			case"EnemyPirate3":
				pirateModel = _gameDataProvider.GetData<PirateModel>("EnemyPirate3");
				break;

		}
		return pirateModel;

	}




	public void OnPirateDead(PirateController pirate){

		_knownPirates.Remove(pirate);
	//	Debug.Log ("Known Pirates : after remove " + _knownPirates.Count.ToString ());

	}

	public void TearDownLevel ()
	{
		_poolingObjectManager.TearDown ();
	}

}


