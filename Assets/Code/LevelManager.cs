using System.Linq;
using Assets.Code.Models;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;

public delegate void OnPirateCreatedEventHandler(PirateController newPirate);
public delegate void OnPirateKilledEventHandler(PirateController killedPirate);
public delegate void OnBuildingCreatedEventHandler(BuildingController newBuilding);
public delegate void OnBuildingDestroyedEventHandler(BuildingController destroyedBuilding);

public class LevelManager 
{
    /* REFENCES */
    readonly GameDataProvider _gameDataProvider;
	private readonly PrefabProvider _prefabProvider;

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

	public LevelManager (IoCResolver resolver)
	{
		_resolver = resolver;

		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);

		_piratesParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
        _buildingsParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));

		_knownPirates = new List<PirateController> ();
        _knownBuildings = new List<BuildingController>();
		GenerateLevelMap();
	}

	public void GenerateLevelMap(){
		Object.Instantiate (_prefabProvider.GetPrefab("AStarPlane"));

	    CreateBuilding("building_a", new Vector3(50, 15, -20));
        CreateBuilding("building_b", new Vector3(-85, 15, -85));
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

        if (OnBuildingCreatedEvent != null)
            OnBuildingCreatedEvent(buildingController);

        buildingController.Stats.OnKilledEvent += () => OnBuildingKilled(buildingController);

        _knownBuildings.Add(buildingController);
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
	}

}


