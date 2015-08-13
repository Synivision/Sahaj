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
	public class PlayState : BaseState
	{
		public PrefabProvider _prefabProvider;
		/* REFERENCES */
		private readonly Messager _messager;

		/* PROPERTIES */
		private UiManager _uiManager;

		/* UiControllers */
		private MainCanvasController _mainCanvasController;
		private CanvasProvider _canvasProvider;

		//tokens
		private MessagingToken _onQuitGame;
		private MessagingToken _onCreatePirate;
		private MessagingToken _onTearDownLevel;
		private PoolingObjectManager _poolingObjectManager;
		//level Manager
		private Dictionary<string,int> _pirateCountDict;
		private Dictionary<string,bool> _unlockedPirates;

		LevelManager levelManager;
		PlayerManager _playerManager;

		//Input Controller requirements

		private bool _mouseState;
		private GameObject target;
		public Vector3 screenSpace;
		public Vector3 offset;
		private float _time = 5;
		private InputSession _inputSession;
		private InputSessionData _inputSessionData;
		int pointerId = -1;

		public PlayState (IoCResolver resolver) : base(resolver)
		{
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _prefabProvider);
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _poolingObjectManager);
			_resolver.Resolve (out _playerManager);
			_resolver.Resolve (out _inputSession);

		}

		public override void Initialize ()
		{
			_uiManager = new UiManager ();

			//_uiManager.RegisterUi (new MainCanvasController (_resolver, _canvasProvider.GetCanvas ("MainCanvas")));
			_uiManager.RegisterUi (new PirateInfoCanvasController (_resolver, _canvasProvider.GetCanvas ("PirateInfoCanvas")));

			_unlockedPirates = new Dictionary<string, bool>();
			_unlockedPirates.Add("Captain",true);
			_unlockedPirates.Add("Quarter Master",true);
			_unlockedPirates.Add("Gunner",true);
			_unlockedPirates.Add("Bomber",true);
			_unlockedPirates.Add("Surgeon",true);
			_unlockedPirates.Add("Carpenter",false);
			_unlockedPirates.Add ("Chef",false);
			_unlockedPirates.Add ("Pirate4",false);
			_unlockedPirates.Add ("EnemyPirate5",false);

			_pirateCountDict = new Dictionary<string, int>();
			_pirateCountDict.Add("Captain",1);
			_pirateCountDict.Add("Quarter Master",2);
			_pirateCountDict.Add("Gunner",2);
			_pirateCountDict.Add("Bomber",3);
			_pirateCountDict.Add("Surgeon",1);
			_pirateCountDict.Add("Carpenter",2);
			_pirateCountDict.Add ("Chef",2);
			_pirateCountDict.Add ("Pirate4",1);
			_pirateCountDict.Add ("EnemyPirate3",10);

			//Initialize level manager
			levelManager = new LevelManager (_resolver);


			PlayerModel playerModel = new PlayerModel();
			playerModel.Name = "User";
			playerModel.Email = "user@gmail.com";
			playerModel.Gold = 0;
			playerModel.ExperiencePoints = 0;
			playerModel.UserLevel =  0;
			playerModel.UserRank = 0;
			playerModel.Wins = 0;
			playerModel.Gems = 100;
			playerModel.MaxGoldCapacity = 2000;
			playerModel.UnlockedPirates = _unlockedPirates;
			playerModel.PirateCountDict = _pirateCountDict;


			_playerManager.Initialize(_resolver,playerModel,levelManager);
			levelManager.PirateCountDict = _pirateCountDict;
			_uiManager.RegisterUi (new GamePlayCanvasController (_resolver, _canvasProvider.GetCanvas ("GamePlayCanvas"), _playerManager));
			//Debug.Log ("Play state initialized.");

			//Message tokens
			_onQuitGame = _messager.Subscribe<QuitGameMessage> (OnQuitGame);
			_onCreatePirate = _messager.Subscribe<CreatePirateMessage> (OnCreatePirate);
			_onTearDownLevel = _messager.Subscribe<TearDownLevelMessage> (OnTearDownLevel);


			_inputSessionData = new InputSessionData();
			_inputSessionData.Name = "Gunner";
			_inputSession.Initialize(_inputSessionData);

		
		}

		public void OnCreatePirate (CreatePirateMessage message)
		{

				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);

		}

		public override void Update ()
		{
			_uiManager.Update ();
            
			// input controller update
			_time += Time.deltaTime;
			if (_time > 3) {
				_messager.Publish (new PirateInfoCanvasMessage{
					toggleValue = false
				});
			}
			
			
			Touch[] touch = Input.touches;
			if (Application.platform == RuntimePlatform.Android)
				pointerId = touch[0].fingerId;
			
			//used to move cube around 
			if (Input.GetMouseButtonDown (0)) {
				
				RaycastHit hitInfo;
				target = GetClickedObject (out hitInfo);
				
				if (target != null && (target.gameObject.tag == "Cube" )) {
					_mouseState = true;
					screenSpace = Camera.main.WorldToScreenPoint (target.transform.position);
					offset = target.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
					
					//myCameraController.enabled = false;
					
				}
				//&& target.gameObject.tag == "Player"
				if (target != null) {
					
					//send message to pirate canvas controller to display pirate info
					/*PirateController playerObject = target.GetComponent<PirateController> ();
				playerObject.UpdateUiPanel ();
				_time = 0;
				
				_messager.Publish (new PirateInfoCanvasMessage{
					toggleValue = true
				});
				*/
				}
				
				Ray ray;
				
				
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId)== false){
					//&& target.gameObject.tag=="Plane"
					if(Physics.Raycast(ray,out hitInfo)&& target.gameObject.tag!="Cube" )
					{
						
						Vector3 spawnPosition = new Vector3(hitInfo.point.x,5.2f,hitInfo.point.z);
						//Debug.Log ("Spawn Point from Input Controller = " + spawnPosition.ToString());
						_messager.Publish (new CreatePirateMessage{
							PirateName = _inputSession.CurrentlySelectedPirateName,
							SpawnPosition = spawnPosition
						});
					}
					
				}
				
				
			}
			
			if (Input.GetMouseButtonUp (0)) {
				_mouseState = false;
				//myCameraController.enabled = true;
			}
			
			if (_mouseState) {
				
				var curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
				var curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
				curPosition.y = target.transform.position.y;
				target.transform.position = curPosition;
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

		private void OnQuitGame (QuitGameMessage message)
		{
			SwitchState (new MenuState (_resolver));

		}

		public override void HandleInput ()
		{

		}

		private void OnTearDownLevel (TearDownLevelMessage message)
		{

			levelManager.TearDownLevel ();
		}

		public override void TearDown ()
		{
			_messager.CancelSubscription (_onQuitGame, _onTearDownLevel, _onCreatePirate);
			levelManager.TearDownLevel ();
			_uiManager.TearDown ();

			_poolingObjectManager.TearDown ();

		}
	}
}
