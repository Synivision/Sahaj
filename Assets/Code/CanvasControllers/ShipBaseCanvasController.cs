using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;
using Assets.Code.States;

namespace Assets.Code.Ui.CanvasControllers
{
	public class ShipBaseCanvasController : BaseCanvasController {
		
		private Button _attackButton;
		private Button _shopButton;
		private Text _fps;
		
		private Canvas _canvasView;
		private IoCResolver _resolver;
		private readonly Messager _messager;
		private UiManager _uiManager;
		private CanvasProvider _canvasProvider;
		
		public ShipBaseCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{
			_canvasView = canvasView;
			_resolver = resolver;
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _canvasProvider);
			_uiManager = new UiManager ();

			ResolveElement (out _attackButton, "AttackButton");
			ResolveElement (out _shopButton, "ShopButton");
			ResolveElement (out _fps,"FpsText");

			_attackButton.onClick.AddListener (OnAttackClicked);
			_shopButton.onClick.AddListener(OnShopButtonClicked);
		}


		public void OnShopButtonClicked(){

			_messager.Publish(new OpenShopMessage{});
		}

		public void OnAttackClicked(){
			//send message to switch state
			//generate map layout
			/*
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
			
			_messager.Publish(new ShipBaseToAttackStateMessage{
				LevelName = "level1",
				MapLayout = map
			});
			*/

			_uiManager.RegisterUi(new LevelSelectCanvasController(_resolver, _canvasProvider.GetCanvas("LevelSelectCanvas")));
			TearDown();

		}

	}
	
}
