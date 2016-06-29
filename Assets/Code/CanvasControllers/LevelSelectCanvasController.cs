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

		private readonly List<Button> levelSelectButtons;

		private readonly Messager _messager;

		private readonly Button attackButton;
		private readonly Button scoutButton;
        private readonly Button closeCanvasButton;

		private readonly Image mapImage1, mapImage2, mapImage3;

		private int SelectedLevel;
		private SpriteProvider _spriteProvider;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
        private IoCResolver _resolver;

        public LevelSelectCanvasController (IoCResolver resolver, Canvas canvasView) : base(resolver, canvasView)
		{
			//List of Buttons i.e. Level1, Level2, Level3 ...
			levelSelectButtons = new List<Button> ();
			int flag = 0;

            _uiManager = new UiManager();
            _resolver = resolver;
            _resolver.Resolve(out _canvasProvider);

            //Initialize Buttons + there listeners and add them to the list
            foreach (Transform buttonTransform in canvasView.transform) {

				//string buttonName = buttonTransform.GetComponent<LevelSelectButtonModel>().ButtonName;
				if(buttonTransform.GetComponent<LevelSelectButtonModel>()){
					Button currentButton;
					ResolveElement(out currentButton,buttonTransform.name);
					AddListenerToButton(ref currentButton,flag);
					levelSelectButtons.Add(currentButton);
					flag++;
				}
			}

			ResolveElement (out attackButton,"Attack");
			ResolveElement (out scoutButton,"Scout");
			ResolveElement (out mapImage1, "map_image_1");
			ResolveElement (out mapImage2,"map_image_2");
			ResolveElement (out mapImage3,"map_image_3");
            ResolveElement (out closeCanvasButton, "Close_Canvas_Button");


			attackButton.onClick.AddListener (PerformAttack);
            closeCanvasButton.onClick.AddListener(PerformCloseCanvas);


            resolver.Resolve(out _messager);
			resolver.Resolve(out _spriteProvider);
			ChangeLevelSelected (0);
			
		}

		public void AddListenerToButton(ref Button button, int level){
		
			button.onClick.AddListener(() => ChangeLevelSelected(level));
		
		}

        public void PerformCloseCanvas() {

            _uiManager.RegisterUi(new ShipBaseCanvasController(_resolver, _canvasProvider.GetCanvas("ShipBaseCanvas")));
            TearDown();

        }

        public void ChangeLevelSelected(int level){
		
			SelectedLevel = level;

			//Change Images for each button
			//mapImage1.sprite = _spriteProvider.GetSprite ("imageName");
			//mapImage2.sprite = _spriteProvider.GetSprite ("imageName");
			//mapImage3.sprite = _spriteProvider.GetSprite ("imageName");

			for (int i = 0; i < levelSelectButtons.Count; i++) {
			
				levelSelectButtons[i].GetComponent<Image>().sprite = _spriteProvider.GetSprite("dot");

			}
			levelSelectButtons[SelectedLevel].GetComponent<Image>().sprite = _spriteProvider.GetSprite("dot_selected");
		
		}

		public void PerformAttack(){

			switch (SelectedLevel) {
				
			case 0 : StartLevel1();
				break;
				
			case 1 : StartLevel2();
				break;
				
			case 2 : StartLevel3();
				break;
				
			case 3 : StartLevel4();
				break;
			}

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

		public void StartLevel4(){

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

		public override void TearDown()
		{	
			
			//Remove Listeners
			attackButton.onClick.RemoveAllListeners();
            closeCanvasButton.onClick.RemoveAllListeners();

			for (int i = 0; i < levelSelectButtons.Count; i++) {
			
				levelSelectButtons[i].onClick.RemoveAllListeners();
			
			}


			base.TearDown();
		}
	}
}