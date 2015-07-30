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


public class BuildingController : PoolingBehaviour {

	private  IoCResolver _resolver;
	private  Messager _messager;
	private  PoolingObjectManager _poolingObjectManager;
	private BuildingModel _buildingModel;
	private List<PirateController> _knownPirates;
	private  LevelManager _levelManager;
	private  PirateController nearestPlayer;
	private UnityReferenceMaster _unityReference;
	private StatsBehaviour _statsBehaviour;
	private PoolingAudioPlayer _poolingAudioPlayer;
	private float minShootTime=0;
	private SoundProvider _soundProvider;
	private PoolingBehaviour fabBullet;
	private PoolingParticleManager _poolingParticleManager;
	private GameObject _spawnPoint;

	public void Initialize (IoCResolver resolver,BuildingModel model,LevelManager levelmanager)
	{

		_resolver = resolver;
		_buildingModel = model;
		_levelManager = levelmanager;
		_resolver.Resolve (out _unityReference);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve(out _poolingAudioPlayer);
		_resolver.Resolve(out _soundProvider);
		_resolver.Resolve(out _poolingParticleManager);

		gameObject.GetComponent<Renderer> ().material.color = _buildingModel.BuildingColor;
		this.name = _buildingModel.Name;

		_knownPirates = _levelManager.GetKnownPirates();
		_spawnPoint = transform.FindChild ("BulletSpawnPoint").gameObject;
		UpdatePirateInfo ();
		_levelManager.OnPirateGeneratedEvent += UpdatePirateInfo;
	}

	void Update ()
	{
		minShootTime += Time.deltaTime;


		if (nearestPlayer != null) {
			
			//if(isChase == true){

		}
		if (_knownPirates.Count >= 1 && nearestPlayer != null) {
			
			if (Vector3.Distance (nearestPlayer.transform.position, transform.position) < _buildingModel.Range) {

				if(minShootTime>1){
					Shoot();
					minShootTime=0;
				}

			} else {

			}
			
		}
		/*
		if (_statsBehaviour.CurrentHealth < 0) {
			
			Delete ();
			if (_levelManager.OnPirateGeneratedEvent != null) {
				_levelManager.OnPirateGeneratedEvent ();
			}
		}
		
		*/
		//_healthBar.value = _statsBehaviour.CurrentHealth;
		
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

	void UpdatePirateInfo ()
	{

		_knownPirates = _levelManager.GetKnownPirates ();
			
		_knownPirates = _knownPirates.Where (pirate => pirate.DataModel.PirateNature == (int)PirateModel.Nature.Player).ToList ();


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
		} 
	}

}
