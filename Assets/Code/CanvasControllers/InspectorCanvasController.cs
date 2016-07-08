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

    public class InspectorCanvasController : BaseCanvasController
    {
        private Canvas _canvasView;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
        private IoCResolver _resolver;
        private Button _move;
        private Button _info;
        private Button _action;
        private Messager _messager;
        private Canvas _canvas;
        private Dictionary<string, GameObject> _elements;

        private BuildingModel.BuildingType _type;

        private BuildingModel _buildingModel;

        public InspectorCanvasController(IoCResolver resolver, Canvas canvasView,BuildingModel buildingModel,Vector3 position
        )
            : base(resolver, canvasView){


            _resolver = resolver;
            _resolver.Resolve(out _messager);
            _resolver.Resolve (out _canvasProvider);
            _canvas = canvasView;
            _uiManager = new UiManager ();
            _buildingModel = buildingModel;
            _type = buildingModel.Type;
            _canvasView = canvasView;
            ResolveElement (out _move, "Panel/Move");
            ResolveElement (out _info, "Panel/Info");
            ResolveElement (out _action,"Panel/Action");
    
            _move.onClick.AddListener (OnMoveClicked);
            _info.onClick.AddListener (OnInfoClicked);
            _action.onClick.AddListener (OnActionClicked);

//			canvasView.transform.GetChild (0).position = position;

        }
        
        private void OnMoveClicked(){
            
            Debug.Log ("On Move Clicked");

            _messager.Publish(new MoveBuildingmessage {
                BuildingName = _buildingModel.Name
            });
            disableCanvas ();
        }
        
        private void OnInfoClicked(){

            Debug.Log ("On Info Clicked");

            _messager.Publish(new OpenBuildingInfoCanvas {
                
                buildingName = _buildingModel.Name

            });

            disableCanvas ();
        }

        public void enableCanvas() {
            _canvas.enabled = true;
        }

        public void disableCanvas() {
            _canvas.enabled = false;
        }

        private void OnActionClicked(){
            
            //ToDo Open Respective Action Canvas depending on the BuildingModel.BuildingType i.e. _type
            Debug.Log ("On Action Clicked");
            //_messager.Publish(new OpenInventory{});
            _messager.Publish (new OpenCreatePirateCanvasMessage{BuildingModel = this._buildingModel});
            disableCanvas();
        }

    }
}
