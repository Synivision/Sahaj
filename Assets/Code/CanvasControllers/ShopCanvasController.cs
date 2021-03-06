﻿using UnityEngine;
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
    public class ShopCanvasController : BaseCanvasController
    {
        private Canvas _canvas;
        private Button closeButton;
        private IoCResolver _resolver;
        private readonly Messager _messager;
        private UiManager _uiManager;
        private Button openBuildingPanelButton;
        private GameObject buttonPanel, buildingButtonListPanel;
        private GameObject buildingButtonListContentPanel;
        private  PrefabProvider _prefabProvider;
        private Button buildingButton;
        private SpriteProvider _spriteProvider;
        private List<Button> buildingButtonList;

        public ShopCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
            _canvas = canvasView;
            _resolver = resolver;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _spriteProvider);
            //_canvas.enabled = true;

            buildingButtonList = new List<Button>();
            ResolveElement(out closeButton, "MainPanel/CloseButton");

            buttonPanel = GetElement("MainPanel/ButtonPanel");

            buildingButtonListPanel = GetElement("MainPanel/BuildingListPanel");
            buildingButtonListContentPanel = GetElement("MainPanel/BuildingListPanel/Content");
            buildingButtonListPanel.SetActive(false);
            buttonPanel.SetActive(true);

            ResolveElement(out openBuildingPanelButton, "MainPanel/ButtonPanel/TreasureButton");
            closeButton.onClick.AddListener(onCloseClicked);
            openBuildingPanelButton.onClick.AddListener(onBuildingPanelButtonClicked);
           
        }

        public void onCloseClicked() {
            
            disableCanvas();
            
        }

        public void enableCanvas() {
            _canvas.enabled = true;
        }

        public void disableCanvas() {
            _canvas.enabled = false;
        }

        public void onBuildingPanelButtonClicked()
        {
            
            buildingButtonListPanel.SetActive(true);
            addBuildingButton("gold_storage");
            addBuildingButton("platoons");
            addBuildingButton("water_cannon");
            addBuildingButton("gunner_tower");
            buttonPanel.SetActive(false);

        }

        public void addBuildingButton(string name) {

            var fab = Object.Instantiate(_prefabProvider.GetPrefab("buildings_button").gameObject.GetComponent<Button>());
            var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
            var buttomImage = fab.GetComponent<Image>();
            buttomImage.sprite = _spriteProvider.GetSprite(name);
            fab.name = name;
            buttonLabel.text = name;

            fab.onClick.AddListener(() => onBuildingButtonclicked(name));
            fab.transform.SetParent(buildingButtonListContentPanel.transform);
            buildingButtonList.Add(fab);
        }

        public void onBuildingButtonclicked(string name) {

            //generate building in map 
            //send message to ship play state 

            _messager.Publish(new CreateBuildingMessage {
                
                BuildingName = name

            });

            _canvas.enabled = false;
        }

        public override void TearDown()
        {
            foreach (var button in buildingButtonList)
            {

                button.onClick.RemoveAllListeners();
                GameObject.Destroy(button.gameObject);
            }

            openBuildingPanelButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            buildingButtonList.Clear();
            base.TearDown();
        }
    }

}
