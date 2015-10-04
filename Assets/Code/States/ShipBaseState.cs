﻿using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Ui.CanvasControllers;
using Assets.Code.Messaging.Messages;
using System.Collections;
namespace Assets.Code.States
{
	public class ShipBaseState : BaseState
	{
		
		private Canvas _shipBaseCanvas;
		
		private CanvasProvider _canvasProvider;
		private PoolingObjectManager _poolingObjectManager;
		private UiManager _uiManager;
		ShipLevelManager shipLevelManager;
		private readonly Messager _messager;
		private MessagingToken _onChangeStateToAttack;
		private MessagingToken _onInventoryOpen;
        private MessagingToken _onBuildingInfoOpen;
		MapLayout _map;
		private float _time = 5;
		private GameObject tile;
		int pointerId = -1;
		private bool _mouseState;
		private GameObject target;
		public Vector3 screenSpace;
		public Vector3 offset;

		private GameObject inventoryCanvas;

		Vector3 curPosition;
		Vector3 selectedgameObjectPosition = new Vector3(0,0,0);


		public ShipBaseState (IoCResolver resolver, MapLayout map) : base(resolver){


			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _poolingObjectManager);

			_map = map;
		}
		
		public override void Initialize (){

			_uiManager = new UiManager ();
			_uiManager.RegisterUi(new ShipBaseCanvasController(_resolver,_canvasProvider.GetCanvas("ShipBaseCanvas")));
		

			shipLevelManager = new ShipLevelManager(_resolver,_map);
			_onChangeStateToAttack = _messager.Subscribe<StartGameMessage>(OnChangeStateToAttack);	
			_onInventoryOpen = _messager.Subscribe<OpenInventory>(OpenInventoryBuilding);
            _onBuildingInfoOpen = _messager.Subscribe<OpenBuildingInfoCanvas>(OnOpenBuildingInfoCanvas);
            //generate tile and disable it
            var tileo = _poolingObjectManager.Instantiate("tile");
			tile = tileo.gameObject;
			tile.SetActive(false);
		
		}

        public void OnOpenBuildingInfoCanvas(OpenBuildingInfoCanvas message) {

            _uiManager.RegisterUi(new BuildingInfoCanvasController(_resolver, _canvasProvider.GetCanvas("BuildingInfoCanvas")));
        }

        public void OpenInventoryBuilding(OpenInventory message){
			
			_uiManager.RegisterUi (new InventoryCanvasController (_resolver, _canvasProvider.GetCanvas ("InventoryCanvas")));
			
		}

		public void OnChangeStateToAttack(StartGameMessage message){

			SwitchState (new PlayState(_resolver,message.MapLayout));

		}

		public override void Update (){

			_uiManager.Update ();
		

			Touch[] touch = Input.touches;
			if (Application.platform == RuntimePlatform.Android)
				pointerId = touch[0].fingerId;
			
			//used to move cube around 
			if (Input.GetMouseButtonDown (0)  && !isButton()) {
				RaycastHit hitInfo;
				target = GetClickedObject (out hitInfo);

				//turn off any inspector canvas visible on clicking any place other than that canvas button
				if(inventoryCanvas != null){
					inventoryCanvas.SetActive(false);
				}

				if (target != null && (target.gameObject.tag == "Cube" )) {
					_mouseState = true;
					//get position of object selected 
					selectedgameObjectPosition = target.transform.position;		
				}
			}
			
			if (Input.GetMouseButtonUp (0) ) {

				//show inspector if selected and current tilename and objectname is same espectively.
				if( _mouseState && shipLevelManager.GetTileAt(curPosition + new Vector3(125,0,125)) == target.gameObject.name){

					inventoryCanvas = target.transform.GetChild(0).gameObject;
					inventoryCanvas.SetActive(true);

				}

				//update the grid tile of moved object if grid tile is empty
				
				if(_mouseState && shipLevelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)) == ShipLevelManager.PassabilityType.Passible){
					shipLevelManager.UpdateBlueprint(selectedgameObjectPosition + new Vector3(125,0,125),curPosition + new Vector3(125,0,125));
				}
				else{
					if(target!=null && target.gameObject.tag != "Plane"){
						target.transform.position = selectedgameObjectPosition;
					}
				}

				_mouseState = false;
				tile.SetActive(false);

			}
			
			if (_mouseState) {
				screenSpace = Camera.main.WorldToScreenPoint (target.transform.position);
				var curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
				curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
				curPosition.y = target.transform.position.y;
				target.transform.position = curPosition;

				//Get grid on curposition
				//instantiate red or green according to grid
				tile.SetActive(true);
				tile.transform.position = curPosition+new Vector3(0,-10,0);


				if(shipLevelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)) == ShipLevelManager.PassabilityType.Passible){
					tile.GetComponent<Renderer>().material.color = Color.green;
				}else{

					//if the position is clicked and not moved.. then also for that object the tile is available
					if(shipLevelManager.GetTileAt(curPosition + new Vector3(125,0,125)) == target.gameObject.name){
						tile.GetComponent<Renderer>().material.color = Color.green;
					}
					else{
						tile.GetComponent<Renderer>().material.color = Color.red;
					}
				}
			}


			
		}

		public GameObject GetClickedObject (out RaycastHit hit)
		{
			GameObject target = null;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray.origin, ray.direction * 10, out hit)) {
				target = hit.collider.gameObject;
			}
			return target;
		}
		
		public override void HandleInput (){
			
		}
		
		public override void TearDown (){

			_uiManager.TearDown();
			Object.Destroy (tile.gameObject);
			shipLevelManager.TearDown();
		}

		private bool isButton()
		{
			bool result = true;
			UnityEngine.EventSystems.EventSystem ct
				= UnityEngine.EventSystems.EventSystem.current;
			
			if (! ct.IsPointerOverGameObject() ) result = false;
			if (! ct.currentSelectedGameObject ) result = false;
			
			
			return result;
		}
	}
	
}
