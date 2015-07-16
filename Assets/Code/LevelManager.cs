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
		_poolingObjectManager.Instantiate ("Plane");
		_knownPirates = new List<PirateController> ();
	}

	public List<PirateController> GetKnownPirates(){

		return _knownPirates;	

	}

	public void CreatePirate ()
	{
	    pirateObject = (PirateController)_poolingObjectManager.Instantiate ("Sphere");

		_knownPirates.Add(pirateObject);


		pirateObject.Initialize(_resolver,GeneratePirateModel(),this);


		pirateObject.transform.position = new Vector3(Random.Range(-100,100),10,Random.Range(-100,100)); 

		if(OnPirateGeneratedEvent!=null){
			OnPirateGeneratedEvent();
		}

		Debug.Log ("Known Pirates : in create " + _knownPirates.Count.ToString ());

	}

	public PirateModel GeneratePirateModel(){

		PirateModel pirateModel = new PirateModel ();
		pirateModel.Health = 100;
		pirateModel.Descipriton = "pirate discription ";
		pirateModel.AttackDamage = Random.Range (10, 20);
		pirateModel.Name = "Pirate No." + Random.Range (0, 100).ToString ();
		pirateModel.Courage = Random.Range (1, 5);
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


