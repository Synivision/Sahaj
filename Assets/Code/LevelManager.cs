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

public class LevelManager
{

	private readonly PrefabProvider _prefabProvider;
	private readonly Messager _messager;
	private readonly PoolingBehaviour _poolingbehviour;
	PoolingObjectManager _poolingObjectManager;
	private List<GameObject> _knownPirates;
	
	public LevelManager (IoCResolver resolver)
	{

		//pass iresolver in constructor
		//_prefabProvider = PlayState.Prefabs;
		//poolingObjectManager = new PoolingObjectManager(_prefabProvider);
		
		resolver.Resolve (out _messager);
		resolver.Resolve (out _poolingObjectManager);
		resolver.Resolve (out _prefabProvider);

		_poolingObjectManager.Instantiate ("Cube");
		_poolingObjectManager.Instantiate ("Plane");
	}

	public static List<GameObject> GetKnownPirates ()
	{
		List<GameObject> _knownPirates = GameObject.FindGameObjectsWithTag ("Player").
			Select (pirate => pirate.gameObject).
				Where (behaviour => behaviour != null).ToList ();
		
		return _knownPirates;
	}

	public void CreatePirate ()
	{
		_poolingObjectManager.Instantiate ("Sphere");
	
		_knownPirates = GetKnownPirates ();
		Debug.Log ("known pirates  : " + _knownPirates.Count.ToString ());
	}

	public void TearDownLevel()
	{
		UnityEngine.Debug.Log ("level manager");
		_poolingObjectManager.TearDown();


	}


}


