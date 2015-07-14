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
	
	public LevelManager (IoCResolver resolver)
	{
		resolver.Resolve (out _messager);
		resolver.Resolve (out _poolingObjectManager);
		resolver.Resolve (out _prefabProvider);

		_poolingObjectManager.Instantiate ("Cube");
		_poolingObjectManager.Instantiate ("Plane");
	}

	public void CreatePirate ()
	{
		var pirateObject = _poolingObjectManager.Instantiate ("Sphere");
		pirateObject.transform.position = new Vector3(Random.Range(-100,100),10,Random.Range(-100,100)); 
		_messager.Publish (new PirateListChangeMessage{
		});
	}

	public void TearDownLevel ()
	{
		UnityEngine.Debug.Log ("level manager");
		_poolingObjectManager.TearDown ();
	}
}


