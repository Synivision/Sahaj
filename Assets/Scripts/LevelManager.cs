using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.States;
using System.Linq;

public class LevelManager : MonoBehaviour {

	private readonly PrefabProvider _prefabProvider;
	private readonly Messager _messager;

	private readonly PoolingBehaviour _poolingbehviour;
	PoolingObjectManager poolingObjectManager;

	private List<GameObject> _knownPirates;
	
	public LevelManager (){


		_prefabProvider = PlayState.Prefabs;
		poolingObjectManager = new PoolingObjectManager(_prefabProvider);
		//_poolingObjectManager = poolingObjectManager; PoolingObjectManager poolingObjectManager

	}

	public void Start(){


	}

	public void Update(){



	}

	public static List<GameObject> GetKnownPirates(){
		List<GameObject>	_knownPirates = GameObject.FindGameObjectsWithTag("Player").
			Select(pirate => pirate.gameObject).
				Where(behaviour => behaviour != null).ToList();
		
		return _knownPirates;
	}


	public void CreatePirate(){
		poolingObjectManager.Instantiate("Sphere");
		_knownPirates = GetKnownPirates();

		Debug.Log("known pirates  : "+_knownPirates.Count.ToString());
	}

	public void TearDownLevel(){
		UnityEngine.Debug.Log("level manager");
		poolingObjectManager.TearDown();

	}

	public void TearDownPirates(){

		foreach (var pirate in _knownPirates){

			GameObject.Destroy(pirate);
		}
	}

}


