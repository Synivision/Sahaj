using System.Linq;
using Assets.Code.Models;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;

public delegate void OnPirateCreatedEventHandler(PirateController newPirate);
public delegate void OnPirateKilledEventHandler(PirateController killedPirate);
public delegate void OnBuildingCreatedEventHandler(BuildingController newBuilding);
public delegate void OnBuildingDestroyedEventHandler(BuildingController destroyedBuilding);

public class LevelManager 
{
    /* REFENCES */
    readonly GameDataProvider _gameDataProvider;
	private readonly PrefabProvider _prefabProvider;
	private readonly PoolingObjectManager _poolingObjectmanager;

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
	private int AreaToCover = 100;
	private  List<GameObject> _groundCoversList;
	//private int MaxGroundCover=5;
	//private int MinGroundCover=0;
	public float GridSize = 10;
	private float _centreAdjustments;

	public LevelManager (IoCResolver resolver)
	{
		_resolver = resolver;

		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);
		_resolver.Resolve (out _poolingObjectmanager);

		_piratesParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_piratesParent.name = "Pirates";
        
		_buildingsParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_buildingsParent.name = "Buildings";
		_knownPirates = new List<PirateController> ();
        _knownBuildings = new List<BuildingController>();
		GenerateLevelMap();
	}

	public void GenerateLevelMap(){
		var building = Object.Instantiate (_prefabProvider.GetPrefab("AStarPlane"));

	    CreateBuilding("building_a", new Vector3(50, 15, -20));
        CreateBuilding("building_b", new Vector3(-85, 15, -85));

		_groundCoverParent  = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_groundCoverParent.name = "GroundCovers";
		for(int x = -5; x <5; x++ ){
			for(int y = -5; y <5 ;y++){
				int random = Random.Range(-20,20);
				int random2 = Random.Range(Random.Range(-100,100),Random.Range(-100,100));
				CreateGroundCovers((x*15)+random2+random,(y*15)+random2+random);
			}

		}

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

        var fab = Object.Instantiate(_prefabProvider.GetPrefab("Building"));
        var buildingController = fab.GetComponent<BuildingController>();

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
		groundCover.transform.localPosition+=new Vector3(Random.Range(x ,y),0,Random.Range(x ,y));

	}
	
    private void OnBuildingKilled(BuildingController building)
    {
        _knownBuildings.Remove(building);
        if (OnBuildingDestroyedEvent != null)
            OnBuildingDestroyedEvent(building);

        if (building != null)
            building.Delete();
    }

	public void CreatePirate (string pirateName, Vector3 spawnposition)
	{
        // NOTE: we might also use this model to define the pirate's prefab
        // this way we could have special pirates with scripts alternate to the default
        // (same applies to buildings above)
	    var model = _gameDataProvider.GetData<PirateModel>(pirateName);

	    var fab = Object.Instantiate(_prefabProvider.GetPrefab("Sphere"));
	    var pirateController = fab.GetComponent<PirateController>();

        pirateController.Initialize(_resolver, model, this);

	    fab.name = pirateName;
		fab.transform.position = spawnposition;
		fab.transform.SetParent(_piratesParent.transform);

		if(OnPirateCreatedEvent != null)
			OnPirateCreatedEvent(pirateController);
        
        pirateController.Stats.OnKilledEvent += () => OnPirateKilled(pirateController);

        _knownPirates.Add(pirateController);
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

}


