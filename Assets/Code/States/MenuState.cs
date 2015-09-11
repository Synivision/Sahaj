using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Ui.CanvasControllers;
using Assets.Code.States;
using Assets.Code.Messaging.Messages;

public class MenuState : BaseState{

	public PrefabProvider _prefabProvider;
	/* REFERENCES */
	private readonly Messager _messager;
	private CanvasProvider _canvasProvider;
	private UiManager _uiManager;
	private MessagingToken _onStartGame;
	private PoolingObjectManager _poolingObjectManager;
	private MessagingToken _onOpenShipBaseMessage;
	
	/* PROPERTIES */
	
	public MenuState (IoCResolver resolver) : base(resolver)
	{
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _canvasProvider);
		_resolver.Resolve (out _poolingObjectManager);
	}
	public override void Initialize ()
	{
		//Debug.Log ("Menu state initialized.");
		//message tokens
		_onStartGame = _messager.Subscribe<StartGameMessage>(OnStartGame);

		_onOpenShipBaseMessage = _messager.Subscribe<OpenShipBaseMessage>(OnOpenShipBaseMessage);
		_uiManager = new UiManager ();

		_uiManager.RegisterUi(new MenuCanvasController(_resolver, _canvasProvider.GetCanvas("MenuCanvas")));


	}

	private void OnStartGame(StartGameMessage message)
	{
		SwitchState(new PlayState(_resolver, message.MapLayout));
	}

	private void OnOpenShipBaseMessage(OpenShipBaseMessage message){
		
		MapLayout map = new MapLayout();
		
		for (var x=0; x<25; x++){
			for (var z=0; z<25; z++){
				MapItemSpawn mapItem = new MapItemSpawn();
				mapItem.xGridCoord = x;
				mapItem.Name = "empty";
				mapItem.zGridCoord = z;
				map.mapItemSpawnList.Add(mapItem);
			}
		}
		
		BuildingSpawn goldbuilding = new BuildingSpawn();
		goldbuilding.Name = "gold_storage";
		goldbuilding.xGridCoord = 10;
		goldbuilding.zGridCoord = 10;
		
		BuildingSpawn goldbuilding2 = new BuildingSpawn();
		goldbuilding2.Name = "gold_storage";
		goldbuilding2.xGridCoord = 5;
		goldbuilding2.zGridCoord = 5;
		
		BuildingSpawn gunner_tower = new BuildingSpawn();
		gunner_tower.Name = "gunner_tower";
		gunner_tower.xGridCoord = 10;
		gunner_tower.zGridCoord = 5;
		
		BuildingSpawn gunner_tower2 = new BuildingSpawn();
		gunner_tower2.Name = "gunner_tower";
		gunner_tower2.xGridCoord = 5;
		gunner_tower2.zGridCoord = 10;
		
		BuildingSpawn platoons = new BuildingSpawn();
		platoons.Name = "platoons";
		platoons.xGridCoord = 15;
		platoons.zGridCoord = 5;
		
		BuildingSpawn platoons2 = new BuildingSpawn();
		platoons2.Name = "platoons";
		platoons2.xGridCoord = 20;
		platoons2.zGridCoord = 15;
		
		BuildingSpawn water_cannon = new BuildingSpawn();
		water_cannon.Name = "water_cannon";
		water_cannon.xGridCoord = 20;
		water_cannon.zGridCoord = 20;
		
		
		map.buildingSpawnList.Add(goldbuilding);
		map.buildingSpawnList.Add(goldbuilding2);
		map.buildingSpawnList.Add(gunner_tower);
		map.buildingSpawnList.Add(gunner_tower2);
		map.buildingSpawnList.Add(platoons);
		map.buildingSpawnList.Add(platoons2);
		map.buildingSpawnList.Add(water_cannon);
		
		//add river
		
		for(int z=0; z<25; z++){
			MapItemSpawn mapItem = new MapItemSpawn();
			mapItem.xGridCoord = 0;
			mapItem.Name = "river";
			mapItem.zGridCoord = z;
			map.mapItemSpawnList.Add(mapItem);
		}
		for(int z=0; z<25; z++){
			MapItemSpawn mapItem = new MapItemSpawn();
			mapItem.xGridCoord = 1;
			mapItem.Name = "river";
			mapItem.zGridCoord = z;
			map.mapItemSpawnList.Add(mapItem);
		}
		for(int z=0; z<25; z++){
			MapItemSpawn mapItem = new MapItemSpawn();
			mapItem.xGridCoord = z;
			mapItem.Name = "river";
			mapItem.zGridCoord = 0;
			map.mapItemSpawnList.Add(mapItem);
		}
		SwitchState(new ShipBaseState(_resolver,map));
	}


	public override void Update ()
	{
		_uiManager.Update ();
		
		// super general input goes here
	}
	
	public override void HandleInput ()
	{

	}
	public override void TearDown ()
	{
		_messager.CancelSubscription (_onStartGame);
		_uiManager.TearDown ();
		_poolingObjectManager.TearDown();

	}

}
