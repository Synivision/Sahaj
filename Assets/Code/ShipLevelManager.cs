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
using Assets.Code.UnityBehaviours.Pooling;

public class ShipLevelManager {

	/* REFENCES */
	readonly GameDataProvider _gameDataProvider;
	private readonly PrefabProvider _prefabProvider;
	private SpriteProvider _spriteProvider;
	private readonly PoolingObjectManager _poolingObjectmanager;
	private Messager _messager;
	private readonly PoolingObjectManager _poolingObjectManager;
	private readonly IoCResolver _resolver;
	private readonly UnityReferenceMaster _unityReferenceMaster;

	private GameObject _buildingsParent;
	private readonly List<BuildingController> _knownBuildings;
	//groundcovers
	private  GameObject _groundCoverParent;
	private  List<GameObject> _groundCoversList;

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

	public ShipLevelManager (IoCResolver resolver, MapLayout map)
	{
		_resolver = resolver;
		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);
		_resolver.Resolve (out _poolingObjectmanager);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _spriteProvider);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve (out _unityReferenceMaster);

		_mapLayout = map;

		//_messager.Subscribe<OpenShopMessage>(EnableShopCanvas);
		GenerateLevelMap();
        bluePrintToMapLayout();

    }

	public void GenerateLevelMap(){
		InitializeStringList();
		_unityReferenceMaster.AStarPlane.SetActive(true);
		GenerateGrid();
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

	public void GenerateGrid(){

	

		_buildingsParent = Object.Instantiate(_prefabProvider.GetPrefab("empty2"));
		_buildingsParent.transform.position = new Vector3(0,0,0);
		_buildingsParent.gameObject.name = "Ship Base Grid";

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
				FillBlueprint (_buildingsParent.gameObject,blueprint[x,y],x,y);
			}
		}
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

	public GameObject CreateBuilding(string buildingName, Vector3 spawnPosition)
	{
		var model = _gameDataProvider.GetData<BuildingModel>(buildingName);

        if (model.piratesContained == null) {
            model.piratesContained = new List<PirateModel>();
        }
        Debug.Log(model.Type + " : Level " + model.Level);
		GameObject fab;
		BuildingController buildingController;
		fab = Object.Instantiate(_prefabProvider.GetPrefab("Building"));
		
		fab.GetComponent<SpriteRenderer>().sprite = _spriteProvider.GetSprite(buildingName);
		
		buildingController = fab.GetComponent<BuildingController>();
		buildingController.Initialize(_resolver, model, this);
		fab.name = buildingName;
		fab.transform.position = spawnPosition;
		fab.transform.SetParent (_buildingsParent.transform);
		//if (OnBuildingCreatedEvent != null){
		//	OnBuildingCreatedEvent(buildingController);
		//}
		
		//buildingController.Stats.OnKilledEvent += () => OnBuildingKilled(buildingController);
		//_knownBuildings.Add(buildingController);
		
		return fab;
		
	}

	private void SetObjectToCorrectTransform(GameObject parentGameObject,GameObject subject,int x,int y){
		subject.transform.localPosition = new Vector3 ((x * GridSize)+GridSize / 2-_centreAdjustments, subject.transform.localScale.y, (y * GridSize)+GridSize / 2-_centreAdjustments);
		//subject.transform.localScale*=GridSize;
		subject.transform.parent = parentGameObject.transform;
		subject.transform.localScale = new Vector3(GridSize, subject.transform.localScale.y ,GridSize);
		
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

    public string[,] GetBluePrint()
    {
        return blueprint;
    }

    public void AddBuildingToBlueprint(string buildingName, Vector3 position)
    {

        var newx = (int)(position.x / GridSize);
        var newz = (int)(position.z / GridSize);

        if ((newx >= 0 && newx < 25) && (newz >= 0 && newz < 25))
        {
           
            var temp = blueprint[newx, newz];
            if (temp.Equals("empty"))
            {
                blueprint[newx, newz] = buildingName;
                Debug.Log(blueprint[newx, newz]);
            }
        }

    }

    public void GenerateGroundCovers(){

		_groundCoverParent  = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
		_groundCoverParent.name = "GroundCovers";
		for(int x = -5; x <5; x++ ){
			for(int y = -5; y <5 ;y++){
				int random = Random.Range(-120,120);
				int random2 = Random.Range(Random.Range(-120,120),Random.Range(-120,120));
				CreateGroundCovers(random,random2);
			}
		}

	}

	public void CreateGroundCovers(int x,int y){
		
		_groundCoversList = new List<GameObject> ();
		var groundCover = Object.Instantiate(_prefabProvider.GetPrefab("groundcover"));
		_groundCoversList.Add (groundCover);
		groundCover.transform.SetParent(_groundCoverParent.transform);
		groundCover.transform.localPosition+=new Vector3(x,0,y);
	}
	


	public void GenerateTraps(){
		var fab = Object.Instantiate(_prefabProvider.GetPrefab("trap"));
		var trapController = fab.GetComponent<TrapController>();
		trapController.Initialize(_resolver, new Vector3(50,0,40));
		
	}

    public MapLayout bluePrintToMapLayout()
    {

        MapLayout layout = new MapLayout();

        MapItemSpawn itemSpawn = new MapItemSpawn();
        BuildingSpawn buildingSpawn = new BuildingSpawn();
        //itemSpawn.Name = blueprintItem;

        int w = blueprint.GetLength(0); // width
        int h = blueprint.GetLength(1); // height

        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y)
            {
                if (blueprint[x, y].Equals("empty"))
                {
                    itemSpawn = new MapItemSpawn(blueprint[x, y], x, y);
                    layout.mapItemSpawnList.Add(itemSpawn);
                }

                if (blueprint[x, y].Equals("river"))
                {
                    itemSpawn = new MapItemSpawn(blueprint[x, y], x, y);
                    layout.mapItemSpawnList.Add(itemSpawn);
                }

                if (blueprint[x, y].Equals("gold_storage"))
                {
                    buildingSpawn = new BuildingSpawn(blueprint[x, y], x, y);
                    layout.buildingSpawnList.Add(buildingSpawn);
                }

                if (blueprint[x, y].Equals("gunner_tower"))
                {
                    buildingSpawn = new BuildingSpawn(blueprint[x, y], x, y);
                    layout.buildingSpawnList.Add(buildingSpawn);
                }

                if (blueprint[x, y].Equals("platoons"))
                {
                    buildingSpawn = new BuildingSpawn(blueprint[x, y], x, y);
                    layout.buildingSpawnList.Add(buildingSpawn);
                }

                if (blueprint[x, y].Equals("water_cannon"))
                {
                    buildingSpawn = new BuildingSpawn(blueprint[x, y], x, y);
                    layout.buildingSpawnList.Add(buildingSpawn);
                }
            }
        }
        return layout;
    }

    public void TearDown(){

		_unityReferenceMaster.AStarPlane.SetActive(false);
		Object.Destroy (_buildingsParent);
		//destroy ground covers
		//Object.Destroy(_buildingsParent);
	}
}
