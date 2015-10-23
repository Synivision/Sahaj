using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

namespace Assets.Code.Ui.CanvasControllers
{ 

public class LevelSelectCanvasController  : BaseCanvasController {

		private readonly Button _level1Button;
		private readonly Button _level2Button;
		private readonly Button _level3Button;
		private readonly Messager _messager;

		public LevelSelectCanvasController (IoCResolver resolver, Canvas canvasView) : base(resolver, canvasView)
		{
			
			ResolveElement (out _level1Button, "level1_button");
			ResolveElement (out _level2Button, "level2_button");
			ResolveElement (out _level3Button, "level3_button");

			resolver.Resolve(out _messager);
			_level1Button.onClick.AddListener (StartLevel1);
			_level2Button.onClick.AddListener (StartLevel2);
			_level3Button.onClick.AddListener (StartLevel3);
			
		}

		public void StartLevel1(){


			//generate map layout
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

			_messager.Publish(new StartGameMessage{
				LevelName = "level1",
					MapLayout = map
			});
			TearDown();
		}

		public void StartLevel2(){
			MapLayout map = new MapLayout();
			BuildingSpawn goldbuilding = new BuildingSpawn();
			goldbuilding.Name = "gold_storage";
			goldbuilding.xGridCoord = 20;
			goldbuilding.zGridCoord = 20;
			
			BuildingSpawn goldbuilding2 = new BuildingSpawn();
			goldbuilding2.Name = "gold_storage";
			goldbuilding2.xGridCoord = 15;
			goldbuilding2.zGridCoord = 15;

			map.buildingSpawnList.Add(goldbuilding);
			map.buildingSpawnList.Add(goldbuilding2);

			_messager.Publish(new StartGameMessage{
				LevelName = "level2",
				MapLayout = map
			});
			TearDown();
		}

		public void StartLevel3(){
			MapLayout map = new MapLayout();

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

			map.buildingSpawnList.Add(platoons);
			map.buildingSpawnList.Add(platoons2);
			map.buildingSpawnList.Add(water_cannon);

			_messager.Publish(new StartGameMessage{
				LevelName = "level3",
				MapLayout = map
			});
			TearDown();
		}

		public override void TearDown()
		{	
			
			//Remove Listeners
			_level1Button.onClick.RemoveAllListeners();
			_level2Button.onClick.RemoveAllListeners();
			_level3Button.onClick.RemoveAllListeners();
			base.TearDown();
		}
	}
}