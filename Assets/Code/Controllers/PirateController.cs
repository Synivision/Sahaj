using UnityEngine;
using Assets.Code.Ui.CanvasControllers;
using System.Collections.Generic;
using System.Linq;

using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

[RequireComponent(typeof(MoveBehaviour))]

public class PirateController : PoolingBehaviour
{
	private  MoveBehaviour _moveBehaviour;
	private  StatsBehaviour _statsBehaviour;
	private  GameObject target;
	private  PirateModel _pirateModel;
	private  List<PirateController> _knownPirates;
	private  PirateController nearestPlayer;
	private  float time;

	//Refences
	private  IoCResolver _resolver;
	private  Messager _messager;
	private  PoolingObjectManager _poolingObjectManager;
	private  PoolingParticleManager _poolingParticleManager;
	private  float timeStay = 0;
	private  float MinPirateDistance = 100;
	private  MessagingToken _onPirateCreated;
	private  PoolingAudioPlayer _poolingAudioPlayer;
	private  SoundProvider _soundProvider;
	private  float projectileVelocity = 0;
	private  float minShootTime = 0;
	private  LevelManager _levelManager;

	private  UnityReferenceMaster _unityReference;

	private  GameObject _spawnPoint;
	private  PoolingBehaviour fabBullet;
	private  List<PirateController> nearbyPlayers;
	private  List<PirateController> nearbyEnemyPirates;

	private string _pirateNature;

	public void Initialize(IoCResolver resolver, PirateModel data,LevelManager levelManager){

		_levelManager = levelManager;
		_knownPirates = _levelManager.GetKnownPirates();
		nearbyPlayers = new List<PirateController>();

		_pirateNature = this.gameObject.tag;
		print(_pirateNature);

		UpdatePirateInfo();
		_levelManager.OnPirateGeneratedEvent += UpdatePirateInfo;

		_spawnPoint = transform.FindChild("BulletSpawnPoint").gameObject;
		_moveBehaviour = GetComponent<MoveBehaviour> ();

		OnDeadEvent += () => _levelManager.OnPirateDead(this);

		
		ResetLerp ();
		_moveBehaviour.OnLerpEndEvent += ResetLerp;
		
		_pirateModel = data;
		
		//Initialize stats behaviour
		_statsBehaviour = new StatsBehaviour (_pirateModel);
		
		//Get Resolver
		_resolver = resolver;
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingParticleManager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve (out _poolingAudioPlayer);
		_resolver.Resolve (out _soundProvider);
		_resolver.Resolve (out _unityReference);
	
	}

	private void ResetLerp ()
	{
		_moveBehaviour.LerpToTarget (new Vector3 (Random.Range (-150f, 150f), 2, Random.Range (-100f, 100f)));
    
	}

	public void Shoot ()
	{
		//hit target
		if (5 > Random.Range(0, 10)){
			Debug.Log("Hit");

			InstantiateBullet(true);
			PerformHit();
		} else {
			Debug.Log("Miss");
			//miss target 
			InstantiateBullet(false);
		}
	}

	void InstantiateBullet(bool hit){

		fabBullet = _poolingObjectManager.Instantiate ("bullet2_prefab");
		Vector3 randomPos = new Vector3(Random.Range(1,3),Random.Range(1,3),Random.Range(1,3));
		fabBullet.gameObject.GetComponent<BulletController> ().Initialize (_resolver, _spawnPoint.transform.position+randomPos, hit, Color.green, nearestPlayer);

	}

	void PerformHit(){
		System.Action action = null;
		
		action += () => nearestPlayer.ApplyHit (Random.Range (10, 20));
		//after some delay 
		_unityReference.FireDelayed(action, 3f);
	}

	public void ApplyHit (float damage)
	{
		_poolingParticleManager.Emit ("blood_prefab", this.transform.position, Color.red, 100);
		if (_statsBehaviour.CurrentHealth > 0) {
			_statsBehaviour.ApplyDamage (damage);
		} 
	}

	void Update ()
	{
		if (_knownPirates.Count > 1 && nearestPlayer != null) {
		
			if (Vector3.Distance (nearestPlayer.transform.position, transform.position) < 100) {
				minShootTime += Time.deltaTime;

				if (minShootTime >= 3) {

					Shoot ();
					minShootTime = 0;

				}
			}
		}

		if(_statsBehaviour.CurrentHealth < 0){

			Delete();
			if(_levelManager.OnPirateGeneratedEvent!=null){
				_levelManager.OnPirateGeneratedEvent();
			}
		}

	}

	void UpdatePirateInfo ()
	{


		if(_pirateNature == "Player"){
			//nearbyPlayers = _knownPirates.Where (pirate => (Vector3.Distance (pirate.transform.position, transform.position) < MinPirateDistance)).ToList();
			_knownPirates = _knownPirates.Where (pirate => pirate.gameObject.tag == "Enemy").ToList();


		}else if(_pirateNature == "Enemy"){

			//nearbyPlayers = _knownPirates.Where (pirate => (Vector3.Distance (pirate.transform.position, transform.position) < MinPirateDistance)).ToList();
			_knownPirates = _knownPirates.Where (pirate => pirate.gameObject.tag == "Player").ToList();
		}

			if (_knownPirates.Count > 1) {
				nearestPlayer = _knownPirates.OrderBy (player => Vector3.Distance (player.transform.position, transform.position)).ToList () [1];
	
		}
	}

	public void UpdateUiPanel ()
	{
		_messager.Publish (new PirateMessage{

			model = _pirateModel
		});
	}

	public PirateModel DataModel {
		get {
			return _pirateModel;
		}
		set {
			_pirateModel = value;
		}
	}

}

