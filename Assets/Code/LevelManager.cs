using System.Linq;
using Assets.Code.Models;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

public delegate void OnPirateCreatedEventHandler(PirateController newPirate);
public delegate void OnPirateKilledEventHandler(PirateController killedPirate);
public delegate void OnBuildingCreatedEventHandler(BuildingController newBuilding);
public delegate void OnBuildingDestroyedEventHandler(BuildingController destroyedBuilding);

public class LevelManager 
{
    /* REFENCES */
    readonly GameDataProvider _gameDataProvider;
	private readonly PrefabProvider _prefabProvider;
	private SpriteProvider _spriteProvider;
	private readonly PoolingObjectManager _poolingObjectmanager;
	private Messager _messager;
	private readonly PoolingObjectManager _poolingObjectManager;

    /* PROPERTIES */
    private readonly List<PirateController> _knownPirates;
    private readonly List<BuildingController> _knownBuildings;
	private readonly IoCResolver _resolver;

	private readonly GameObject _piratesParent;
    private readonly GameObject _buildingsParent;

    public OnPirateCreatedEventHandler OnPirateCreatedEvent;
    public OnPirateKilledEventHandler OnPirateKilledEvent;
    public OnBuildingCreatedEventHandler OnBuildingCreatedEvent;
    public OnBuildingDestroyedEventHandler OnBuildingDestroyedEvent;

	//groundcovers
	private  GameObject _groundCoverParent;
	private  List<GameObject> _groundCoversList;

	public Dictionary<string,int> PirateCountDict{ get; set;}

	//Grid Map generator
	private int AreaToCover;
	public float GridSize;
	private float _centreAdjustments;

	public LevelManager (IoCResolver resolver)
	{
		_resolver = resolver;

		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);
		_resolver.Resolve (out _poolingObjectmanager);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _spriteProvider);
		_resolver.Resolve (out _poolingObjectManager);
		_piratesParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_piratesParent.name = "Pirates";
        
		_buildingsParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_buildingsParent.name = "Buildings";
		_knownPirates = new List<PirateController> ();
        _knownBuildings = new List<BuildingController>();
		GenerateLevelMap();
		GenerateGrid ();
	}

	public void GenerateLevelMap(){

		var building = Object.Instantiate (_prefabProvider.GetPrefab("a_star_plane"));
		
		CreateBuilding("gunner_tower", new Vector3(91, 15, 81));
		CreateBuilding("gunner_tower", new Vector3(-85, 15, -88));
		CreateBuilding("gold_storage", new Vector3(40, 15, 0));
		CreateBuilding("gold_storage", new Vector3(-63, 15, 0));
		CreateBuilding("platoons", new Vector3(-100, 15, 85));
		CreateBuilding("platoons", new Vector3(85, 15, -85));
		CreateBuilding("water_cannon", new Vector3(-10, 15, 0));
		
		_groundCoverParent  = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_groundCoverParent.name = "GroundCovers";
		for(int x = -5; x <5; x++ ){
			for(int y = -5; y <5 ;y++){
				int random = Random.Range(-120,120);
				int random2 = Random.Range(Random.Range(-120,120),Random.Range(-120,120));
				CreateGroundCovers(random,random2);
			}
		}

		GenerateTraps();
		GenerateShip ();
	}

	public void GenerateTraps(){
		var fab = Object.Instantiate(_prefabProvider.GetPrefab("trap"));
		var trapController = fab.GetComponent<TrapController>();
		trapController.Initialize(_resolver, new Vector3(50,0,40));

	}
    public List<StatsBehaviour> GetOpposition(PirateNature context)
    {
        var opposition = _knownPirates.Where(pirate => pirate.Model.PirateNature != context)
                                      .Select(pirate => pirate.Stats)
                                      .ToList();
        
        // TODO: add nature to buildings?
        if(context == PirateNature.Player)
            opposition.AddRange(_knownBuildings.Select(building => building.Stats));

        return opposition;
    } 

	public List<PirateController> GetOpposingPirates(PirateNature context){
		return _knownPirates.Where(pirate => pirate.Model.PirateNature != context).ToList();
	}

    public List<BuildingController> GetOpposingBuildings(PirateNature context)
    {
        return context == PirateNature.Player ? _knownBuildings : new List<BuildingController>();
    } 

    public void CreateBuilding(string buildingName, Vector3 spawnPosition)
    {
		var model = _gameDataProvider.GetData<BuildingModel>(buildingName);
		GameObject fab;
		BuildingController buildingController;
		fab = Object.Instantiate(_prefabProvider.GetPrefab("building"));

		fab.GetComponent<SpriteRenderer>().sprite = _spriteProvider.GetSprite(buildingName);

		buildingController = fab.GetComponent<BuildingController>();
		buildingController.Initialize(_resolver, model, this);
		fab.name = buildingName;
		fab.transform.position = spawnPosition;
		fab.transform.SetParent (_buildingsParent.transform);

		if (OnBuildingCreatedEvent != null)
			OnBuildingCreatedEvent(buildingController);

		buildingController.Stats.OnKilledEvent += () => OnBuildingKilled(buildingController);
		_knownBuildings.Add(buildingController);


    }

	public void CreateGroundCovers(int x,int y){

		_groundCoversList = new List<GameObject> ();
		var groundCover = Object.Instantiate(_prefabProvider.GetPrefab("groundcover"));
		_groundCoversList.Add (groundCover);
		groundCover.transform.SetParent(_groundCoverParent.transform);
		groundCover.transform.localPosition+=new Vector3(x,0,y);
	}

	public void GenerateShip(){
		var fab = Object.Instantiate(_prefabProvider.GetPrefab("ship"));
		var shipBehaviour = fab.GetComponent<ShipBehaviour>();
		shipBehaviour.Initialize(_resolver, this, new Vector3(-180,40,-25));
	}

    private void OnBuildingKilled(BuildingController building)
    {
        _knownBuildings.Remove(building);
        if (OnBuildingDestroyedEvent != null)
            OnBuildingDestroyedEvent(building);

        if (building != null)
            building.Delete();

		if(_knownBuildings.Count == 0){

			_messager.Publish(new WinMessage{});
		}
    }

	public void CreatePirate (string pirateName, Vector3 spawnposition)
	{
        // NOTE: we might also use this model to define the pirate's prefab
        // this way we could have special pirates with scripts alternate to the default
        // (same applies to buildings above)
		if (PirateCountDict.ContainsKey(pirateName)){
			int val = PirateCountDict [pirateName];
			if (val > 0) {
				PirateCountDict [pirateName] = val - 1;
				var model = _gameDataProvider.GetData<PirateModel> (pirateName);
				
				var fab = Object.Instantiate (_prefabProvider.GetPrefab ("sphere"));
				var pirateController = fab.GetComponent<PirateController> ();
				
				pirateController.Initialize (_resolver, model, this);
				
				fab.name = pirateName;
				fab.transform.position = spawnposition;
				fab.transform.SetParent (_piratesParent.transform);
				
				if (OnPirateCreatedEvent != null)
					OnPirateCreatedEvent (pirateController);
				
				pirateController.Stats.OnKilledEvent += () => OnPirateKilled (pirateController);
				
				_knownPirates.Add (pirateController);
			}
		}
	}

    private void OnPirateKilled(PirateController pirate)
    {
        _knownPirates.Remove(pirate);
        if (OnPirateKilledEvent != null)
            OnPirateKilledEvent(pirate);

        if (pirate != null)
            pirate.Delete();
    }

	public void TearDownLevel ()
	{
        Object.Destroy(_piratesParent);
        Object.Destroy(_buildingsParent);
		//destroy ground covers
		Object.Destroy(_groundCoverParent);
	}

	public void GenerateGrid(){
		
		var parentGameObject = _poolingObjectManager.Instantiate("empty2");
		parentGameObject.transform.position = new Vector3(0,0,0);
		parentGameObject.gameObject.name = "Grid";
		
		AreaToCover = 25;
		GridSize = 10;
		
		_centreAdjustments = AreaToCover * GridSize / 2;
		var blueprint = new string[AreaToCover,AreaToCover];
		
		//fill up blueprint with walls
		for (var x=0; x<AreaToCover;x++){
			for(var y=0; y<AreaToCover; y++){
				blueprint[x,y]="wall";
			}
		}
		//River
		for (var x=0; x<5;x++){
			for(var y=0; y<5; y++){
				blueprint[x,y]="river";
			}
		}
		
		//instantiate onjects as per the blueprint
		for (var x=0; x<AreaToCover; x++){
			for (var y=0; y<AreaToCover; y++){
				FillBlueprint (parentGameObject.gameObject,blueprint[x,y],x,y);
			}
		}
	}

	private void FillBlueprint(GameObject parentGameObject,string type,int x ,int y){
		
		
		if (type == "wall") {
			
			var fab = _poolingObjectManager.Instantiate("empty");
			SetObjectToCorrectTransform(parentGameObject,fab.gameObject,x,y);
			
		}
		else if(type=="empty"){
			
		}
		
		else if (type == "river") {
			var fab = _poolingObjectManager.Instantiate("empty2");
			SetObjectToCorrectTransform(parentGameObject,fab.gameObject,x,y);
		}
		
	}
	
	
	private void SetObjectToCorrectTransform(GameObject parentGameObject,GameObject subject,int x,int y){
		subject.transform.localPosition = new Vector3 ((x * GridSize)+GridSize / 2-_centreAdjustments, 0, (y * GridSize)+GridSize / 2-_centreAdjustments);
		//subject.transform.localScale*=GridSize;
		subject.transform.parent = parentGameObject.transform;
		
		
	}
}