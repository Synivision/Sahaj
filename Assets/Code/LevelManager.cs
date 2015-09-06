using System.Linq;
using Assets.Code.Models;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;

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
	private readonly UnityReferenceMaster _unityReferenceMaster;
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
	private int AreaToCover = 25;
	public float GridSize = 10;
	private float _centreAdjustments;
	private string[,] blueprint;
	
	private MapLayout _mapLayout;
	private List<string> BuildingList;
	private List<string> MapItemsList;

	public enum PassabilityType {
		Impassible,
		Passible
	};
	
	public LevelManager (IoCResolver resolver, MapLayout map)
	{
		_resolver = resolver;
		_mapLayout = map;
		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);
		_resolver.Resolve (out _poolingObjectmanager);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _spriteProvider);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve(out _unityReferenceMaster);

		_piratesParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_piratesParent.name = "Pirates";
		
		_buildingsParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_buildingsParent.name = "Buildings";
		_knownPirates = new List<PirateController> ();
		_knownBuildings = new List<BuildingController>();
		
		GenerateLevelMap();
		// Debug.Log(GetTileAt(new Vector3(12,0,13)));
	}
	
	public void GenerateLevelMap(){
		
		// aStarPlane = Object.Instantiate (_prefabProvider.GetPrefab("a_star_plane"));
		_unityReferenceMaster.AStarPlane.SetActive(true);
		InitializeStringList();
		
		GenerateGrid ();
		
		/*
		GenerateTraps();
		GenerateShip ();
		*/
	}
	
	public void UpdateBlueprint(Vector3 oldPos, Vector3 newPos){

		var oldx = (int)(oldPos.x/GridSize);
		var oldz = (int)(oldPos.z/GridSize);
		var newx = (int)(newPos.x/GridSize);
		var newz = (int)(newPos.z/GridSize);

		if( (oldx >= 0 && oldx < 25) && (oldz >= 0 && oldz < 25) && (newx >= 0 && newx < 25) && (newz >= 0 && newz < 25) ){
			
			var temp = blueprint[oldx,oldz];
			blueprint[oldx,oldz] = "empty";
			blueprint[(int)(newPos.x/GridSize),(int)(newPos.z/GridSize)] = temp;

		}
	}

	public PassabilityType GetCoordinatePassability(Vector3 point)
	{
		
		string tile = GetTileAt(point);
		if(tile == "empty"){
			return PassabilityType.Passible;
		}
		else {
			return PassabilityType.Impassible;
		}
	}
	public string GetTileAt(Vector3 point){
		
		string tileName = "outOfBounds";
		
		int x = (int)(point.x/GridSize);
		int z = (int)(point.z/GridSize);

		if( (x >= 0 && x < 25) && (z >= 0 && z < 25) ){
			
			tileName = blueprint[x,z];
		}
		//Debug.Log("Tilename = " +tileName);
		return tileName;
	}
	public void GenerateGrid(){
		
		var parentGameObject = _poolingObjectManager.Instantiate("empty2");
		parentGameObject.transform.position = new Vector3(0,0,0);
		parentGameObject.gameObject.name = "Grid";
		
		AreaToCover = 25;
		GridSize = 10;
		
		_centreAdjustments = AreaToCover * GridSize / 2;
		blueprint = new string[AreaToCover,AreaToCover];

		foreach (var mapItem in _mapLayout.mapItemSpawnList){
			
			blueprint[mapItem.xGridCoord,mapItem.zGridCoord] = mapItem.Name;
		}
		
		foreach (var building in _mapLayout.buildingSpawnList){
			
			blueprint[building.xGridCoord,building.zGridCoord] = building.Name;
		}
		//instantiate onjects as per the blueprint

		for (var x=0; x<AreaToCover; x++){
			for (var y=0; y<AreaToCover; y++){
				FillBlueprint (parentGameObject.gameObject,blueprint[x,y],x,y);
			}
		}
	}
	
	public void InitializeStringList(){
		BuildingList = new List<string>();
		BuildingList.Add("gold_storage");
		BuildingList.Add("gunner_tower");
		BuildingList.Add("platoons");
		BuildingList.Add("water_cannon");
		
		MapItemsList = new List<string>();
		MapItemsList.Add("river");
		MapItemsList.Add("wall");
		//MapItemsList.Add("empty");
		
	}
	private void FillBlueprint(GameObject parentGameObject,string type,int x ,int y){
		
		
		if(BuildingList.Contains(type)){
			
			var fab2 = CreateBuilding(type,new Vector3(0,0,0));
			SetObjectToCorrectTransform(parentGameObject,fab2.gameObject,x,y);
		}
		if (MapItemsList.Contains(type)){
			
			var fab = _poolingObjectManager.Instantiate(type);
			SetObjectToCorrectTransform(parentGameObject,fab.gameObject,x,y);
		}
	}
	
	
	private void SetObjectToCorrectTransform(GameObject parentGameObject,GameObject subject,int x,int y){
		subject.transform.localPosition = new Vector3 ((x * GridSize)+GridSize / 2-_centreAdjustments, subject.transform.localScale.y, (y * GridSize)+GridSize / 2-_centreAdjustments);
		//subject.transform.localScale*=GridSize;
		subject.transform.parent = parentGameObject.transform;
		subject.transform.localScale = new Vector3(GridSize, subject.transform.localScale.y ,GridSize);
		
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
	
	public GameObject CreateBuilding(string buildingName, Vector3 spawnPosition)
	{
		var model = _gameDataProvider.GetData<BuildingModel>(buildingName);
		GameObject fab;
		BuildingController buildingController;
		fab = Object.Instantiate(_prefabProvider.GetPrefab("Building"));
		
		fab.GetComponent<SpriteRenderer>().sprite = _spriteProvider.GetSprite(buildingName);
		
		buildingController = fab.GetComponent<BuildingController>();
		buildingController.Initialize(_resolver, model, this);
		fab.name = buildingName;
		fab.transform.position = spawnPosition;
		fab.transform.SetParent (_buildingsParent.transform);
		
		if (OnBuildingCreatedEvent != null){
			OnBuildingCreatedEvent(buildingController);
		}
		
		buildingController.Stats.OnKilledEvent += () => OnBuildingKilled(buildingController);
		_knownBuildings.Add(buildingController);
		
		return fab;
		
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
				//send message to canvas to update number of pirate
				if(pirateName!="EnemyPirate3"){
				_messager.Publish(new UpdatePirateNumber{
					PirateName = pirateName,
					PirateNumber = PirateCountDict [pirateName]
				});
				}
				var model = _gameDataProvider.GetData<PirateModel> (pirateName);
				
				var fab = Object.Instantiate (_prefabProvider.GetPrefab ("Sphere"));
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

	public void GenerateShip(){
		var fab = Object.Instantiate(_prefabProvider.GetPrefab("ship"));
		var shipBehaviour = fab.GetComponent<ShipBehaviour>();
		shipBehaviour.Initialize(_resolver, this, new Vector3(-180,40,-25));
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
		_unityReferenceMaster.AStarPlane.SetActive(false);
		Object.Destroy(_piratesParent);
		Object.Destroy(_buildingsParent);
		//destroy ground covers
		Object.Destroy(_groundCoverParent);
	}
}