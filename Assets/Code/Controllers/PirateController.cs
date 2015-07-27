using UnityEngine;
using Assets.Code.Ui.CanvasControllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

public class PirateController : AIPath
{
	private  MoveBehaviour _moveBehaviour;
	private  StatsBehaviour _statsBehaviour;
	private  PirateModel _pirateModel;
	private  List<PirateController> _knownPirates;
	private  PirateController nearestPlayer;
	private  float time;

	//Refences
	private  IoCResolver _resolver;
	private  Messager _messager;
	private  PoolingObjectManager _poolingObjectManager;
	private  PrefabProvider _prefabProvider;
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
	public Slider _healthBar;
	public Text _stateText;
	public RectTransform panel;
	enum PirateState
	{
		Shooting,
		Chasing,
		Idle,
		Fleeing}
	;

	private PirateState _pirateState;
	private bool isChase = true;

	public void Initialize (IoCResolver resolver, PirateModel data, LevelManager levelManager)
	{

		_levelManager = levelManager;
		_knownPirates = _levelManager.GetKnownPirates ();
		nearbyPlayers = new List<PirateController> ();

		_spawnPoint = transform.FindChild ("BulletSpawnPoint").gameObject;
		_moveBehaviour = GetComponent<MoveBehaviour> ();

		OnDeadEvent += () => _levelManager.OnPirateDead (this);
	
		_pirateModel = data;
		gameObject.GetComponent<Renderer> ().material.color = _pirateModel.PirateColor;
		 
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
		_resolver.Resolve (out _prefabProvider);

		UpdatePirateInfo ();
		_levelManager.OnPirateGeneratedEvent += UpdatePirateInfo;
		_healthBar.maxValue = _pirateModel.Health;

		ChangeState (PirateState.Idle);
		panel.sizeDelta = new Vector2 (_pirateModel.PirateRange, _pirateModel.PirateRange);
		isChase = true;
		this.name = _pirateModel.PirateName;

	}

	public void Shoot ()
	{
		//hit target		
		if (8 > Random.Range (0, 10)) {
			InstantiateBullet (true);
			PerformHit ();
			_poolingAudioPlayer.PlaySound (transform.position, _soundProvider.GetSound ("lazer_shoot1"), 50);
			
		} else {
			
			InstantiateBullet (false);
			_poolingAudioPlayer.PlaySound (transform.position, _soundProvider.GetSound ("lazer_shoot_miss"), 50);
		}
	}

	void InstantiateBullet (bool hit)
	{

		fabBullet = _poolingObjectManager.Instantiate ("bullet2_prefab");
		Vector3 randomPos = new Vector3 (Random.Range (1, 3), Random.Range (1, 3), Random.Range (1, 3));
		fabBullet.gameObject.GetComponent<BulletController> ().Initialize (_resolver, _spawnPoint.transform.position + randomPos, hit, Color.green, nearestPlayer);

	}

	void PerformHit ()
	{
		System.Action action = null;
		_unityReference.FireDelayed (() => {
			nearestPlayer.ApplyHit (Random.Range (10, 20));
		}, .1f);
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
		minShootTime += Time.deltaTime;
		UpdateStateInfo ();
	
		switch (_pirateState) {
		case PirateState.Chasing:
				
	
			target = nearestPlayer.transform;
			HandleChase ();
			_stateText.text = "Chasing";
				
			break;

		case PirateState.Fleeing:
			break;
		case PirateState.Shooting:
			
			if (minShootTime >= 1f) {
				Shoot ();
				minShootTime = 0f;
			}
				
			//ChangeState(PirateState.Chasing);

			break;
		case PirateState.Idle:
			break;

		}
	
		if (nearestPlayer != null) {

			//if(isChase == true){
				

		}
		if (_knownPirates.Count >= 1 && nearestPlayer != null) {
		
			if (Vector3.Distance (nearestPlayer.transform.position, transform.position) < _pirateModel.PirateRange) {

				ChangeState (PirateState.Shooting);
				//Debug.Log(Vector3.Distance (nearestPlayer.transform.position, transform.position).ToString());
			} else {
				ChangeState (PirateState.Chasing);
			}

		}

		if (_statsBehaviour.CurrentHealth < 0) {

			Delete ();
			if (_levelManager.OnPirateGeneratedEvent != null) {
				_levelManager.OnPirateGeneratedEvent ();
			}
		}


		_healthBar.value = _statsBehaviour.CurrentHealth;

	}

	void UpdateStateInfo ()
	{

		if (!_knownPirates.Any ()) {
			ChangeState (PirateState.Idle);
			return;
		}

	}

	void UpdatePirateInfo ()
	{

		_knownPirates = _levelManager.GetKnownPirates ();
		if (_pirateModel.PirateNature == (int)PirateModel.Nature.Player) {
			_knownPirates = _knownPirates.Where (pirate => pirate._pirateModel.PirateNature == (int)PirateModel.Nature.Enemy).ToList ();


		} else if (_pirateModel.PirateNature == (int)PirateModel.Nature.Enemy) {

			_knownPirates = _knownPirates.Where (pirate => pirate._pirateModel.PirateNature == (int)PirateModel.Nature.Player).ToList ();
		}

		if (_knownPirates.Count >= 1) {

			nearestPlayer = _knownPirates [0];

			foreach (PirateController pirate in _knownPirates) {

				if (Vector3.Distance (pirate.transform.position, transform.position) < Vector3.Distance (nearestPlayer.transform.position, transform.position)) {

					nearestPlayer = pirate;
				
				}
								
			}

			if (nearestPlayer != null) {
			
				_unityReference.FireDelayed (() => {
					UpdatePirateInfo ();
				}, 2f);
			}
				

		} else {
			
			ChangeState (PirateState.Idle);
			
		} 

	}

	public void HandleChase ()
	{
		//Debug.Log ("Handle Chase");
		//Calculate desired velocity
		var dir = CalculateVelocity (tr.transform.position);
		
		
		//Rotate towards targetDirection (filled in by CalculateVelocity)
		RotateTowards (targetDirection);
		dir.y = 0;
		
		controller.SimpleMove (dir);
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

	private void ChangeState (PirateState newState)
	{
		_pirateState = newState;
		_stateText.text = newState.ToString ();
	}


}

