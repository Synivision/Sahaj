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

	private MoveBehaviour _moveBehaviour;
	private StatsBehaviour _statsBehaviour;
	private GameObject target;
	private PirateModel _pirateModel;
	private List<GameObject> _knownPirates;
	private GameObject nearestPlayer;
	private float time;

	//Refences
	IoCResolver _resolver;
	private  Messager _messager;
	private  PoolingObjectManager _poolingObjectManager;
	private  PoolingParticleManager _poolingParticleManager;
	float timeStay = 0;
	List<GameObject> nearbyPlayers;
	float MinPirateDistance = 2;
	private MessagingToken _onPirateCreated;
	private PoolingAudioPlayer _poolingAudioPlayer;
	private SoundProvider _soundProvider;
	float projectileVelocity = 0;
	float minShootTime = 0;

	private GameObject _spawnPoint;
	PoolingBehaviour fabBullet;
	void Start ()
	{

		_spawnPoint = transform.FindChild("BulletSpawnPoint").gameObject;
		_moveBehaviour = GetComponent<MoveBehaviour> ();

		_knownPirates = new List<GameObject> ();
		nearbyPlayers = new List<GameObject> ();

		ResetLerp ();
		_moveBehaviour.OnLerpEndEvent += ResetLerp;

		_pirateModel = new PirateModel ();
		_pirateModel.Health = 100;
		_pirateModel.Descipriton = "pirate discription ";
		_pirateModel.AttackDamage = Random.Range (10, 20);
		_pirateModel.Name = "Pirate No." + Random.Range (0, 100).ToString ();
		_pirateModel.Courage = Random.Range (1, 5);

		//Initialize stats behaviour
		_statsBehaviour = new StatsBehaviour (_pirateModel);

		//Get Resolver
		_resolver = GameObject.Find ("state_master").GetComponent<StateMaster> ().Resolver;
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingParticleManager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve (out _poolingAudioPlayer);
		_resolver.Resolve (out _soundProvider);

		//Tokens
		_onPirateCreated = _messager.Subscribe<PirateListChangeMessage> (UpdateKnownPirates);


		//_messager.Publish(new CreatePirateMessage{

		//});
		Debug.Log ("Known Pirates : " + _knownPirates.Count.ToString ());
		UpdateKnownPirates(null);
	}

	private void ResetLerp ()
	{
		_moveBehaviour.LerpToTarget (new Vector3 (Random.Range (-50f, 20f), 2, Random.Range (-50f, 20f)));
    
	}

	private void UpdateKnownPirates (PirateListChangeMessage message)
	{
		Debug.Log ("In PirateListchange message");
		_knownPirates = new List<GameObject> ();
		nearestPlayer = null;
		_knownPirates = GameObject.FindGameObjectsWithTag ("Player").
			Select (player => player.gameObject).
				Where (behaviour => behaviour != null).ToList ();


		if (_knownPirates.Count > 1) {
			nearestPlayer = _knownPirates.OrderBy (player => Vector3.Distance (player.transform.position, transform.position)).ToList () [1];
		}

	}

	public void Shoot ()
	{

		//Shoot Bullet
		fabBullet = _poolingObjectManager.Instantiate ("bullet2_prefab");

		projectileVelocity = 20 * Time.deltaTime;
	
		fabBullet.gameObject.GetComponent<BulletController> ().Initialize (_resolver, _spawnPoint.transform.position, nearestPlayer.transform.position, Color.green);
	}

	public void ApplyHit (float damage)
	{
		//emit blood on getting shot
		_poolingParticleManager.Emit ("blood_prefab", this.transform.position, Color.red, 100);

		// delete if health less than zero
		if (_statsBehaviour.CurrentHealth > 0) {
			_statsBehaviour.ApplyDamage (damage);
		} else {
			Destroy (gameObject);
			_messager.Publish (new PirateListChangeMessage{});
		}
	}

	void Update ()
	{
		if (_knownPirates.Count > 1 && nearestPlayer != null) {
		
			Debug.Log ("Known Pirates : " + _knownPirates.Count.ToString ());
			//Debug.Log ("Distance : " + Vector3.Distance (nearestPlayer.transform.position, transform.position).ToString ());
			if (Vector3.Distance (nearestPlayer.transform.position, transform.position) > 10) {

				Debug.Log ("Known Pirates : Shoot");
				minShootTime += Time.deltaTime;

				if (minShootTime >= 3) {

					Shoot ();
					minShootTime = 0;

				}
			}
		}

	}

	void UpdatePirateInfo ()
	{

		nearbyPlayers = _knownPirates.Where (pirate => Vector3.Distance (pirate.transform.position, transform.position) < MinPirateDistance).ToList ();

		if (nearbyPlayers.Any ()) {

			if (_knownPirates.Count > 1) {
				nearestPlayer = _knownPirates.OrderBy (player => Vector3.Distance (player.transform.position, transform.position)).ToList () [1];


				//	Debug.Log (Vector3.Distance (nearestPlayer.transform.position, transform.position).ToString ());
				//if (nearestPlayer !=null && nearestPlayer.GetComponent<PirateController> ().DataModel != null) {
				//	Debug.Log ("Nearest Pirate to " + this._pirateModel.Name + " is "
				//		+ nearestPlayer.GetComponent<PirateController> ().DataModel.Name);
				//}
			}
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

	void OnCollisionEnter(Collision coll) 
	{

		if (coll.gameObject.tag == "Bullet" ) {

			ApplyHit (Random.Range (10, 20));
			DestroyObject (coll.gameObject);

		}

	}
}

