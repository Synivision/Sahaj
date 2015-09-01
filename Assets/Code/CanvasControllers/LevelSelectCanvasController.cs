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

		public LevelSelectCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
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
			BuildingSpawn goldbuilding = new BuildingSpawn();
			goldbuilding.Name = "gold_storage";
			goldbuilding.gridXPosition = 10;
			goldbuilding.gridZPosition = 10;

			BuildingSpawn goldbuilding2 = new BuildingSpawn();
			goldbuilding2.Name = "gold_storage";
			goldbuilding2.gridXPosition = 5;
			goldbuilding2.gridZPosition = 5;

			BuildingSpawn gunner_tower = new BuildingSpawn();
			gunner_tower.Name = "gunner_tower";
			gunner_tower.gridXPosition = 10;
			gunner_tower.gridZPosition = 5;

			BuildingSpawn gunner_tower2 = new BuildingSpawn();
			gunner_tower2.Name = "gunner_tower";
			gunner_tower2.gridXPosition = 5;
			gunner_tower2.gridZPosition = 10;

			BuildingSpawn platoons = new BuildingSpawn();
			platoons.Name = "platoons";
			platoons.gridXPosition = 15;
			platoons.gridZPosition = 5;

			BuildingSpawn platoons2 = new BuildingSpawn();
			platoons2.Name = "platoons";
			platoons2.gridXPosition = 20;
			platoons2.gridZPosition = 15;

			BuildingSpawn water_cannon = new BuildingSpawn();
			water_cannon.Name = "water_cannon";
			water_cannon.gridXPosition = 20;
			water_cannon.gridZPosition = 20;


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
				mapItem.gridXPosition = 0;
				mapItem.Name = "river";
				mapItem.gridZPosition = z;
				map.mapItemSpawnList.Add(mapItem);
			}
			for(int z=0; z<25; z++){
				MapItemSpawn mapItem = new MapItemSpawn();
				mapItem.gridXPosition = 1;
				mapItem.Name = "river";
				mapItem.gridZPosition = z;
				map.mapItemSpawnList.Add(mapItem);
			}
			for(int z=0; z<25; z++){
				MapItemSpawn mapItem = new MapItemSpawn();
				mapItem.gridXPosition = 2;
				mapItem.Name = "river";
				mapItem.gridZPosition = z;
				map.mapItemSpawnList.Add(mapItem);
			}


			//add river



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
			goldbuilding.gridXPosition = 20;
			goldbuilding.gridZPosition = 20;
			
			BuildingSpawn goldbuilding2 = new BuildingSpawn();
			goldbuilding2.Name = "gold_storage";
			goldbuilding2.gridXPosition = 15;
			goldbuilding2.gridZPosition = 15;

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
			platoons.gridXPosition = 15;
			platoons.gridZPosition = 5;
			
			BuildingSpawn platoons2 = new BuildingSpawn();
			platoons2.Name = "platoons";
			platoons2.gridXPosition = 20;
			platoons2.gridZPosition = 15;
			
			BuildingSpawn water_cannon = new BuildingSpawn();
			water_cannon.Name = "water_cannon";
			water_cannon.gridXPosition = 20;
			water_cannon.gridZPosition = 20;

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