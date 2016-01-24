using Assets.Code.Models;
using UnityEngine;
using UnityEngine.UI;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Models.Pooling;

enum PirateState
{
    Shooting,
    Chasing,
    Idle,
    Scanning,
    Fleeing
}

[RequireComponent(typeof(StatsBehaviour))]
public class PirateController : AIPath
{
    /* REFERENCES */
    // ioc
    private IoCResolver _resolver;
    private Messager _messager;
    private PrefabProvider _prefabProvider;
    private SoundProvider _soundProvider;
    private PoolingAudioPlayer _poolingAudioPlayer;
    private PoolingObjectManager _poolingObjectManager;
    private PoolingParticleManager _poolingParticleManager;
    private UnityReferenceMaster _unityReference;

    private LevelManager _levelManager;

    // other components
    public MoveBehaviour MoveBehaviour;
    public StatsBehaviour Stats;

    private GameObject _bulletOrigin;
    public Slider _healthBar;
    public Text _stateText;
    public RectTransform Panel;

    private MessagingToken _openShipBaseMessageToken;

    // data
    public PirateModel Model { get; private set; }

    /* PROPERTIES */
    private StatsBehaviour _currentTarget;

    private float _timeTillNextShot;
    private float _timeTillNextSearch;

    private PirateState _currentState;
    private bool isDead = false;
    private GameObject sword = null;

    public void Initialize(IoCResolver resolver, PirateModel model, LevelManager levelManager)
    {
        // resolve references
        _levelManager = levelManager;
        Model = model;

        _resolver = resolver;
        _resolver.Resolve(out _messager);
        _resolver.Resolve(out _poolingParticleManager);
        _resolver.Resolve(out _poolingObjectManager);
        _resolver.Resolve(out _poolingAudioPlayer);
        _resolver.Resolve(out _soundProvider);
        _resolver.Resolve(out _prefabProvider);
        _resolver.Resolve(out _unityReference);

        MoveBehaviour = GetComponent<MoveBehaviour>();
        Stats = GetComponent<StatsBehaviour>();
        _bulletOrigin = transform.FindChild("BulletSpawnPoint").gameObject;

        // initialize properties
        _levelManager.OnPirateCreatedEvent += OnPirateCreated;
        _levelManager.OnBuildingCreatedEvent += OnBuildingCreated;

        Stats.Initialize(Model.Stats);
        Stats.OnCurrentHealthChangedEvent += OnCurrentHealthChanged;

        gameObject.GetComponent<Renderer>().material.color = Model.PirateColor;
        Panel.sizeDelta = new Vector2(Model.PirateRange, Model.PirateRange);

        ChangeState(PirateState.Scanning);
        _healthBar.maxValue = Stats.Block.MaximumHealth;
        _healthBar.value = Stats.CurrentHealth;

        _openShipBaseMessageToken = _messager.Subscribe<OpenShipBaseMessage>(OnOpenShipBaseMessage);

        // NOTE: this should be replaced with an attack speed value from the model
        _timeTillNextShot = 1f;
        _timeTillNextSearch = 2f;

        if (Model.PirateColor.Equals(Color.blue))
        {
            //instantiate sword
            var fab = _poolingObjectManager.Instantiate("captain_melee_weapon").gameObject;
            
            fab.transform.SetParent(gameObject.transform);
            fab.transform.position = new Vector3(2.7f,2.1f,6);

        }
    }

    private void OnCurrentHealthChanged (float oldHealth, float newHealth, float delta)
    {
        _healthBar.value = Stats.CurrentHealth;

        // you might here want to implement a tiered system, or a gradial one
        // where the amount of damage is related to the amount of blood/horrible screaming (Y)
        if (delta < 0)
            _poolingParticleManager.Emit ("blood_prefab", transform.position, Color.red, 100);
    }

    private void OnPirateCreated(PirateController newPirate)
    {
        if (Model.PirateNature != newPirate.Model.PirateNature)
        {
            if (_currentTarget == null)
                _currentTarget = newPirate.Stats;
        }
    }

    private void OnBuildingCreated(BuildingController newBuilding)
    {
        // TODO: natures for buildings?
        if (Model.PirateNature == PirateNature.Player)
        {
            if (_currentTarget == null)
                _currentTarget = newBuilding.Stats;
        }
    }

    void Update ()
    {
        HandleScanning();

        switch (_currentState)
        {
            case PirateState.Chasing:
                HandleChase();
                break;
            case PirateState.Fleeing:
                break;
            case PirateState.Shooting:
                {
                   
                    if (Model.PirateColor.Equals(Color.blue))
                    {
                        HandleMeleeAttack();
                    }
                    else {
                        HandleShooting();
                    }
                    
                }
                break;
            case PirateState.Scanning:
                HandleScanning();
                break;
            case PirateState.Idle:
                break;
        }
    }

    #region STATE HANDLING
    private void HandleChase()
    {
        if (_currentTarget != null)
        {
            //Calculate desired velocity
            var dir = CalculateVelocity(tr.transform.position);

            //Rotate towards targetDirection (filled in by CalculateVelocity)
            RotateTowards(targetDirection);
            dir.y = 0;

            controller.SimpleMove(dir);

            // start shooting if we're in range
            if (Vector3.Distance(transform.position, target.position) <= Model.PirateRange)
                ChangeState(PirateState.Shooting);
        }
        else
            ChangeState(PirateState.Scanning);
    }

    private void HandleMeleeAttack() {
        _timeTillNextShot -= Time.deltaTime;
        if (_currentTarget != null)
        {
            if (_timeTillNextShot <= 0f)
            {
                MeleeAttack(_currentTarget);
                _timeTillNextShot = 1f;
            }
        }
        else
            ChangeState(PirateState.Scanning);


    }
    private void HandleShooting()
    {
        _timeTillNextShot -= Time.deltaTime;

        if (_currentTarget != null)
        {
            if (_timeTillNextShot <= 0f)
            {
                Shoot(_currentTarget);
                _timeTillNextShot = 1f;
            }
        }
        else
            ChangeState(PirateState.Scanning);
    }

    private void HandleScanning()
    {
        _timeTillNextSearch -= Time.deltaTime;

        if (_timeTillNextSearch <= 0f)
        {
            _currentTarget = EvaluateTargets();
            
            // if we found a target, do we chase or shoot?
            if (_currentTarget != null)
            {
                if (Vector3.Distance(transform.position, _currentTarget.transform.position) <= Model.PirateRange)
                    ChangeState(PirateState.Shooting);
                else
                {
                    target = _currentTarget.transform;
                    ChangeState(PirateState.Chasing);
                }
            }

            _timeTillNextSearch = 2f;
        }
    }
    #endregion

    private void MeleeAttack(StatsBehaviour shootingTarget)
    {
        if (shootingTarget!=null) {
            _unityReference.Delay(() =>
            {
                shootingTarget.CurrentHealth -= Model.Stats.MaximumDamage;
            }, 0.1f);


        }

        // TODO sword animation
        if (sword != null)
        {
            var pos = sword.transform.rotation;

            sword.transform.Rotate(Vector3.up * Time.deltaTime, Space.World);

            sword.transform.rotation = pos;
        }
        _poolingAudioPlayer.PlaySound(new PooledAudioRequest
            {
                Sound = _soundProvider.GetSound("lazer_shoot1"),
                Target = transform.position,
                Volume = 0.3f
            });
    }

    private void Shoot(StatsBehaviour shootingTarget)
    {
        // hit target		
        if (8 > Random.Range(0, 10))
        {
            _unityReference.Delay(() =>
            {
                shootingTarget.CurrentHealth -= Model.Stats.MaximumDamage;
            }, 0.1f);

            CreateBullet(shootingTarget.transform.position, true);
            _unityReference.Camera.ApplyShake(1f);
            _poolingAudioPlayer.PlaySound(new PooledAudioRequest
            {
                Sound = _soundProvider.GetSound("lazer_shoot1"),
                Target = transform.position,
                Volume = 0.3f
            });
        }
        // miss
        else
        {
            CreateBullet(shootingTarget.transform.position, false);
            _unityReference.Camera.ApplyShake(1f);
            _poolingAudioPlayer.PlaySound(new PooledAudioRequest
            {
                Sound = _soundProvider.GetSound("lazer_shoot_miss"),
                Target = transform.position,
                Volume = 0.3f
            });
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
        var potentialTargets = _levelManager.GetOpposition(Model.PirateNature);
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
        // TODO: chances are we care more about just distance here
        // this is where we consider things like the opponent's strength & etc.
        return Model.PirateRange - Vector3.Distance(potentialTarget.transform.position, transform.position);
    }

    public void UpdateUiPanel ()
    {
        _messager.Publish (new PirateMessage{
            model = Model
        });
    }

    private void ChangeState (PirateState newState)
    {
        _currentState = newState;
        _stateText.text = newState.ToString ();
    }

    private void OnOpenShipBaseMessage(OpenShipBaseMessage message) {
        Delete();
    }

    public void Delete()
    {
        _levelManager.OnPirateCreatedEvent -= OnPirateCreated;
        _levelManager.OnBuildingCreatedEvent -= OnBuildingCreated;
        Stats.OnCurrentHealthChangedEvent -= OnCurrentHealthChanged;
        if (!isDead)
        {
            Destroy(gameObject);
            isDead = true;
        }
    }
}

