using Assets.Code.Models;
using UnityEngine;
using Assets.Code.Messaging;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

public class ShipBehaviour : MonoBehaviour {

	private  IoCResolver _resolver;
	private Messager _messager;
	private UnityReferenceMaster _unityReference;
	private LevelManager _levelManager;

	private GameObject _pirateSpawnPoint;
	private bool enablePirate;
	private float _createPirateTime;
	int maxenemyPirateCount = 5;
	
	public void Initialize (IoCResolver resolver,LevelManager levelmanager,Vector3 pos)
	{
	
		_levelManager = levelmanager;
		_resolver = resolver;
		enablePirate = true;

		_resolver.Resolve(out _unityReference);

		this.transform.position = pos;
		//get pirate spawn point
		_pirateSpawnPoint = this.gameObject;
	}


	void Update(){

		transform.LookAt(_unityReference.Sun.transform,-Vector3.down);

		//move ship


		_createPirateTime += Time.deltaTime;
		if(_createPirateTime >.5 && enablePirate == true && maxenemyPirateCount > 0){
			//_levelManager.CreatePirate("EnemyPirate3",_pirateSpawnPoint.transform.position + new Vector3(Random.Range(0,15),0,Random.Range(0,15)));
			_levelManager.CreatePirate("EnemyPirate3",new Vector3(-120,5,-25)  + new Vector3(Random.Range(0,10),0,Random.Range(0,10)) );
			maxenemyPirateCount--;
			_createPirateTime = 0;
		}
	}
}
