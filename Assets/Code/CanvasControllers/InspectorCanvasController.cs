using UnityEngine;
using System.Collections;
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

	public class InspectorCanvasController : MonoBehaviour
	{
		private Canvas _canvasView;
		private UiManager _uiManager;
		private CanvasProvider _canvasProvider;
		private IoCResolver _resolver;
		private Button _upgrade;
		private Button _info;
		private Button _action;
		private Messager _messager;
		
		private Dictionary<string, GameObject> _elements;

		private BuildingModel.BuildingType _type;

		private BuildingModel _buildingModel;
		
		public void Initialize (IoCResolver resolver,Canvas canvasView,BuildingModel buildingModel) 
		{
			_resolver = resolver;
			_resolver.Resolve(out _messager);
			_resolver.Resolve (out _canvasProvider);
			_canvasView = canvasView;
			_uiManager = new UiManager ();
			_buildingModel = buildingModel;
			_type = buildingModel.Type;
			
			_elements = new Dictionary<string, GameObject>();
			
			for (var i = 0; i < canvasView.transform.childCount; i++)
			{
				var child = canvasView.transform.GetChild(i);
				if (_elements.ContainsKey(child.name))
				{
					Debug.Log("WARNING! found duplicate child name : " + child.name + " in " + this + "!");
					continue;
				}
				
				_elements.Add(child.name, child.gameObject);
			}
			
			ResolveElement (out _upgrade, "Upgrade");
			ResolveElement (out _info, "Info");
			ResolveElement (out _action,"Action");
			_canvasView.gameObject.SetActive (false);
			
			_upgrade.onClick.AddListener (OnUpgradeClicked);
			_info.onClick.AddListener (OnInfoClicked);
			_action.onClick.AddListener (OnActionClicked);

		}
		
		
		private void OnUpgradeClicked(){
			
			//ToDo Open Upgrade Canvas
			
		}
		
		private void OnInfoClicked(){
			
			//ToDo Open Info Canvas
			Debug.Log ("On Info Clicked");
            var buildingNameStr = gameObject.transform.parent.name;
			_messager.Publish(new OpenBuildingInfoCanvas {
                
                buildingName = buildingNameStr

            });

            gameObject.SetActive(false);
		}
		
		private void OnActionClicked(){
			
			//ToDo Open Respective Action Canvas depending on the BuildingModel.BuildingType i.e. _type
			Debug.Log ("On Action Clicked");
			//_messager.Publish(new OpenInventory{});
			_messager.Publish (new OpenCreatePirateCanvasMessage{BuildingModel = this._buildingModel});
		}
		
		// Use this for initialization
		void Start ()
		{
			
		}
			
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		protected void ResolveElement<T>(out T element, string name) where T : Component
		{
			if (!_elements.ContainsKey(name))
			{
				Debug.Log("WARNING! canvas controller (" + GetType() + ") does not have element named " + name);
				element = null;
				return;
			}
			
			element = _elements[name].GetComponent<T>();
		}
	}
}
