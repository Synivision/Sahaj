using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.States;




public class LevelManager {
	private readonly PrefabProvider _prefabProvider;
	private readonly Messager _messager;

	private readonly PoolingBehaviour _poolingbehviour;
	PoolingObjectManager poolingObjectManager;

	public LevelManager (){


		_prefabProvider = PlayState.Prefabs;
		poolingObjectManager = new PoolingObjectManager(_prefabProvider);
		//_poolingObjectManager = poolingObjectManager; PoolingObjectManager poolingObjectManager

	}

	public void CreatePirate(){

		poolingObjectManager.Instantiate("Sphere");

	}

	public void TearDownPirates(){
		
		UnityEngine.Debug.Log("level manager");
		poolingObjectManager.TearDown();

	}

}


