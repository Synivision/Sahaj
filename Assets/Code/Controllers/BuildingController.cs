using Assets.Code.Models;
using UnityEngine;
using Assets.Code.Messaging;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Ui.CanvasControllers;

[RequireComponent(typeof(StatsBehaviour))]
public class BuildingController : InitializeRequiredBehaviour {
    /* REFERENCES */
    // ioc
	private  IoCResolver _resolver;
    private Messager _messager;
    private UnityReferenceMaster _unityReference;
    private SoundProvider _soundProvider;
    private PoolingAudioPlayer _poolingAudioPlayer;
    private PoolingObjectManager _poolingObjectManager;
    private PoolingParticleManager _poolingParticleManager;
	private PlayerManager _playerManager;

    private LevelManager _levelManager;
	private ShipLevelManager _shipLevelManager;
    // other compenents
    public StatsBehaviour Stats;
    private GameObject _bulletOrigin;
	private GameObject _pirateSpawnPoint;

    // data
	private BuildingModel Model;

    /* PROPERTIES */
	private  StatsBehaviour _currentTarget;
    private float _timeTillNextShot;
    private float _timeTillNextSearch;
	private float _createPirateTime;
	int maxenemyPirateCount = 5;


	public void Initialize (IoCResolver resolver,BuildingModel model,LevelManager levelmanager)
    {
        // resolve references
        Model = model;
        _levelManager = levelmanager;

		_resolver = resolver;
		_resolver.Resolve (out _unityReference);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve(out _poolingAudioPlayer);
		_resolver.Resolve(out _soundProvider);
		_resolver.Resolve(out _poolingParticleManager);
		_resolver.Resolve(out _playerManager);

        //gameObject.GetComponent<Renderer>().material.color = Model.BuildingColor;
        Stats = GetComponent<StatsBehaviour>();
       
        // initialize properties
        _levelManager.OnPirateCreatedEvent += OnPirateCreated;

		if (Model.Type != BuildingModel.BuildingType.Gold_Locker && Model.Type != BuildingModel.BuildingType.Defence_Water_Cannons) {
			//_bulletOrigin = transform.FindChild ("BulletSpawnPoint").gameObject;
			//_pirateSpawnPoint = transform.FindChild("PirateSpawnPoint").gameObject;
			_bulletOrigin = this.gameObject;
			_pirateSpawnPoint = this.gameObject;
		}

        Stats.Initialize(model.Stats);
	    Stats.OnCurrentHealthChangedEvent += OnCurrentHealthChanged;

        MarkAsInitialized();
    }

	public void Initialize (IoCResolver resolver,BuildingModel model,ShipLevelManager shiplevelmanager)
	{
		// resolve references
		Model = model;
		_shipLevelManager = shiplevelmanager;
		
		_resolver = resolver;
		_resolver.Resolve (out _unityReference);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve(out _poolingAudioPlayer);
		_resolver.Resolve(out _soundProvider);
		_resolver.Resolve(out _poolingParticleManager);
		_resolver.Resolve(out _playerManager);
		
		//gameObject.GetComponent<Renderer>().material.color = Model.BuildingColor;
		Stats = GetComponent<StatsBehaviour>();
		
		// initialize properties

		if (Model.Type == BuildingModel.BuildingType.Gold_Locker) {

			Model.GoldAmount = 1000;

		}
		
		if (Model.Type != BuildingModel.BuildingType.Gold_Locker && Model.Type != BuildingModel.BuildingType.Defence_Water_Cannons) {
			//_bulletOrigin = transform.FindChild ("BulletSpawnPoint").gameObject;
			//_pirateSpawnPoint = transform.FindChild("PirateSpawnPoint").gameObject;
			_bulletOrigin = this.gameObject;
			_pirateSpawnPoint = this.gameObject;
		}
		
		Stats.Initialize(model.Stats);
		Stats.OnCurrentHealthChangedEvent += OnCurrentHealthChanged;
		
		MarkAsInitialized();

		var inspectorCanvasGameObject = transform.GetChild (0);
		var inspectorCanvas = inspectorCanvasGameObject.GetComponent<Canvas> ();
		inspectorCanvasGameObject.GetComponent<InspectorCanvasController>().Initialize(_resolver,inspectorCanvas,Model.Type);
	}


    private void OnCurrentHealthChanged(float oldHealth, float newHealth, float delta)
    {
        // you might here want to implement a tiered system, or a gradial one
        // where the amount of damage is related to the amount of blood/horrible screaming (Y)
	
		//TODO Calculate available gold
		//TODO check if building is of type gold
		if (Model.Type == BuildingModel.BuildingType.Gold_Locker){

			if(Model.GoldAmount<0){
				return;
			}
			_messager.Publish(new UpdateGamePlayUiMessage{
				availableGold = Model.GoldAmount ,
				Gold = 100 , 
				ExperiencePoints = 5
					
			});

			Model.GoldAmount -= 100;
			_poolingAudioPlayer.PlaySound(transform.position, _soundProvider.GetSound("coin_drop"), 0.3f);

		}

		if (delta < 0){
            // this bleeding building brought to you by steven king
			//display coin effect and sound
            _poolingParticleManager.Emit("blood_prefab", transform.position, Color.red, 100);

		}
    }

    private void OnPirateCreated(PirateController newPirate)
    {
        if (newPirate.Model.PirateNature == PirateNature.Player)
        {
            if (_currentTarget == null)
                _currentTarget = newPirate.Stats;
                // TODO: again, maybe update this if buildings get natures 
            else if (GenerateTargetEvaluation(_currentTarget) < GenerateTargetEvaluation(newPirate.Stats))
                _currentTarget = newPirate.Stats;
        }
    }

    public void Update()
    {

		transform.LookAt(_unityReference.Sun.transform,-Vector3.down);


		_timeTillNextShot -= Time.deltaTime;
		_timeTillNextSearch -= Time.deltaTime;
		
		if (Model.Type != BuildingModel.BuildingType.Gold_Locker && Model.Type != BuildingModel.BuildingType.Defence_Water_Cannons) {
			if (_currentTarget == null && _timeTillNextSearch <= 0f) {
				_currentTarget = EvaluateTargets ();
				_timeTillNextSearch = 2f;
			} else if (_currentTarget != null && _timeTillNextShot <= 0f &&
			           Vector3.Distance (transform.position, _currentTarget.transform.position) <= Model.Range) {
				Shoot (_currentTarget);
				_timeTillNextShot = 1f;
			}
		}
		if(_currentTarget != null && Vector3.Distance(transform.position, _currentTarget.transform.position) <= Model.Range && Model.Type == BuildingModel.BuildingType.Defence_Platoons && maxenemyPirateCount > 0){

			_createPirateTime += Time.deltaTime;
			if(_createPirateTime >.5){
				_levelManager.CreatePirate("EnemyPirate3",_pirateSpawnPoint.transform.position + new Vector3(Random.Range(0,15),0,Random.Range(0,15)));
				maxenemyPirateCount--;
				_createPirateTime = 0;
			}
			
		}
    }

    private void Shoot(StatsBehaviour shootingTarget)
    {
        // hit target		
        if (8 > Random.Range(0, 10))
        {
            _unityReference.FireDelayed(() =>
            {
                shootingTarget.CurrentHealth -= Model.Stats.MaximumDamage;
            }, 0.1f);

            CreateBullet(shootingTarget.transform.position, true);
            _unityReference.Camera.ApplyShake(1f);
            _poolingAudioPlayer.PlaySound(transform.position, _soundProvider.GetSound("lazer_shoot1"), 0.3f);
        }
        // miss
        else
        {
            CreateBullet(shootingTarget.transform.position, false);
            _unityReference.Camera.ApplyShake(1f);
            _poolingAudioPlayer.PlaySound(transform.position, _soundProvider.GetSound("lazer_shoot_miss"), 0.3f);
        }
    }

    void CreateBullet(Vector3 bulletTarget, bool didHit)
    {
        var fab = _poolingObjectManager.Instantiate("bullet2_prefab");
        var missDelta = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));

        fab.GetComponent<BulletController>().Initialize(_resolver, _bulletOrigin.transform.position + missDelta,
                                                        didHit, Color.green, bulletTarget,"Player");
    }

    private StatsBehaviour EvaluateTargets()
    {
        // TODO: blah blah blah building natures blah
		if(_levelManager!=null){

			var potentialTargets = _levelManager.GetOpposition(PirateNature.Enemy);
			if (potentialTargets.Count >= 1)
			{
				var bestTarget = potentialTargets[0];
				var bestScore = float.MinValue;
				foreach (var potentialTarget in potentialTargets)
				{
					var score = GenerateTargetEvaluation(potentialTarget);
					if (score > bestScore)
					{
						bestScore = score;
						bestTarget = potentialTarget;
					}
				}
				
				return bestTarget;
			}

		}
        
        return null;
    }

    private float GenerateTargetEvaluation(StatsBehaviour potentialTarget)
    {
        return Vector3.Distance(potentialTarget.transform.position, transform.position);
    }

    public void Delete()
    {
        _levelManager.OnPirateCreatedEvent -= OnPirateCreated;
        Stats.OnCurrentHealthChangedEvent -= OnCurrentHealthChanged;

        Destroy(gameObject);
    }
}