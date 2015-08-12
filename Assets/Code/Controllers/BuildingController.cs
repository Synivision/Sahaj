using Assets.Code.Models;
using UnityEngine;
using Assets.Code.Messaging;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

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

    private LevelManager _levelManager;

    // other compenents
    public StatsBehaviour Stats;
    private GameObject _bulletOrigin;
	private GameObject _pirateSpawnPoint;

    // data
	public BuildingModel Model;

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

        //gameObject.GetComponent<Renderer>().material.color = Model.BuildingColor;
        Stats = GetComponent<StatsBehaviour>();
        _bulletOrigin = transform.FindChild("BulletSpawnPoint").gameObject;
		_pirateSpawnPoint = transform.FindChild("PirateSpawnPoint").gameObject;
        // initialize properties
        _levelManager.OnPirateCreatedEvent += OnPirateCreated;

        Stats.Initialize(model.Stats);
	    Stats.OnCurrentHealthChangedEvent += OnCurrentHealthChanged;

        MarkAsInitialized();
    }

    private void OnCurrentHealthChanged(float oldHealth, float newHealth, float delta)
    {
        // you might here want to implement a tiered system, or a gradial one
        // where the amount of damage is related to the amount of blood/horrible screaming (Y)
        if (delta < 0)
            // this bleeding building brought to you by steven king
            _poolingParticleManager.Emit("blood_prefab", transform.position, Color.red, 100);
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
        _timeTillNextShot -= Time.deltaTime;
        _timeTillNextSearch -= Time.deltaTime;

        if (_currentTarget == null && _timeTillNextSearch <= 0f)
        {
            _currentTarget = EvaluateTargets();
            _timeTillNextSearch = 2f;
        }
        else if (_currentTarget != null && _timeTillNextShot <= 0f &&
                 Vector3.Distance(transform.position, _currentTarget.transform.position) <= Model.Range)
        {
            Shoot(_currentTarget);
            _timeTillNextShot = 1f;
        }

		if(_currentTarget != null && Vector3.Distance(transform.position, _currentTarget.transform.position) <= Model.Range && Model.Type == BuildingModel.BuildingType.Defence_Platoons && maxenemyPirateCount > 0){


			_createPirateTime += Time.deltaTime;
			if(_createPirateTime >3){
				_levelManager.CreatePirate("EnemyPirate3",_pirateSpawnPoint.transform.position);
				maxenemyPirateCount--;
				_createPirateTime = 0;
			}
			//_levelManager.CreatePirate("EnemyPirate1",new Vector3(transform.position.x + 20,transform.position.y,transform.position.z +20));
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
                                                        didHit, Color.green, bulletTarget);
    }

    private StatsBehaviour EvaluateTargets()
    {
        // TODO: blah blah blah building natures blah
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