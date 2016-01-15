using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;
using Assets.Code.States;

namespace Assets.Code.Ui.CanvasControllers
{
	public class CreatePirateCanvasController : BaseCanvasController
	{
		//Scroll Views
		private GameObject _parentPirateButtonPanel;
		private GameObject _parentPirateImagesScrollViewPanel;

		//Time left Label
		private Text timeLeftText;

		//Navigation Buttons
		private Button leftNavigationButton;
		private Button rightNavigationButton;

		public CreatePirateCanvasController(IoCResolver resolver, Canvas canvasView)
		: base(resolver, canvasView){



		}

	}
}