using Assets.Code.DataPipeline;
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
		MapLayout _map;
		private float _time = 5;
		private GameObject tile;
		int pointerId = -1;
		private bool _mouseState;
		private GameObject target;
		public Vector3 screenSpace;
		public Vector3 offset;
		
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

			//generate tile and disable it
			var tileo = _poolingObjectManager.Instantiate("tile");
			tile = tileo.gameObject;
			tile.SetActive(false);
		
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
			if (Input.GetMouseButtonDown (0)) {
				RaycastHit hitInfo;
				target = GetClickedObject (out hitInfo);
				
				
				if (target != null && (target.gameObject.tag == "Cube" )) {
					_mouseState = true;
					//get position of object selected 
					selectedgameObjectPosition = target.transform.position;		
					
				}
			}
			
			if (Input.GetMouseButtonUp (0)) {
				
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
				//Debug.Log(levelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)));
				//instantiate red or green according to grid
				tile.SetActive(true);
				tile.transform.position = curPosition+new Vector3(0,-10,0);
				if(shipLevelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)) == ShipLevelManager.PassabilityType.Passible){
					tile.GetComponent<Renderer>().material.color = Color.green;
				}else{
					tile.GetComponent<Renderer>().material.color = Color.red;
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
	}
	
}
