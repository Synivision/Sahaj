using Assets.Code.Models;
using UnityEngine;
using Assets.Code.Messaging;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging.Messages;


public class ShipBehaviour : MonoBehaviour {

	private  IoCResolver _resolver;
	private Messager _messager;
	private MessagingToken OnAttackSelected;
	private UnityReferenceMaster _unityReference;
	private LevelManager _levelManager;
	private PoolingObjectManager _poolingObjectManager;
	private GameObject _pirateSpawnPoint;
	private bool enablePirate;
	private float _createPirateTime;
	int maxenemyPirateCount = 5;

	public enum ShipAttacktype {
		Gun,
		Bomb,
		Gas,
		Fire
	}

	public ShipAttacktype attackType;
    private PlayerManager _playerManager;

    public void Initialize(IoCResolver resolver, LevelManager levelmanager, Vector3 pos, PlayerManager playerManager)
	{
	
		_levelManager = levelmanager;
		_resolver = resolver;
		enablePirate = true;
        _playerManager = playerManager;

		//default attack type
		attackType = ShipAttacktype.Gun;

		_resolver.Resolve(out _unityReference);
		_resolver.Resolve(out _poolingObjectManager);
		_resolver.Resolve(out _messager);
		this.transform.position = pos;
		//get pirate spawn point
		_pirateSpawnPoint = this.gameObject;

		OnAttackSelected = _messager.Subscribe<SelectShipAttackMessage>(OnAttackTypeButtonClicked);
        
    }

	private void OnAttackTypeButtonClicked(SelectShipAttackMessage message)
	{
		Debug.Log("Hello" + message.ToString());
		attackType = message.attackType;

	}

	public void shoot(Vector3 firePos) {

		switch (attackType) {

			case ShipAttacktype.Gun:
				break;
			case ShipAttacktype.Bomb:
				break;
			case ShipAttacktype.Gas:
				break;
			case ShipAttacktype.Fire:
				break;
		}
		var fab = _poolingObjectManager.Instantiate("bullet2_prefab");
		var missDelta = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));

		fab.GetComponent<BulletController>().Initialize(_resolver,gameObject.transform.position + missDelta,
														true, Color.green, firePos,"Ship");
        _playerManager.Model.ShipBulletsAvailable -= 2;
    }


	void Update(){

		transform.LookAt(_unityReference.Sun.transform,-Vector3.down);

		//move ship
		_createPirateTime += Time.deltaTime;
		/*if(_createPirateTime >.5 && enablePirate == true && maxenemyPirateCount > 0){
			//_levelManager.CreatePirate("EnemyPirate3",_pirateSpawnPoint.transform.position + new Vector3(Random.Range(0,15),0,Random.Range(0,15)));
			_levelManager.CreatePirate("EnemyPirate3",new Vector3(-120,5,-25)  + new Vector3(Random.Range(0,10),0,Random.Range(0,10)) );
			maxenemyPirateCount--;
			_createPirateTime = 0;
		}*/
	}
	/*public override void TearDown ()
	{
		_messager.CancelSubscription (OnAttackSelected);

	}*/

}
