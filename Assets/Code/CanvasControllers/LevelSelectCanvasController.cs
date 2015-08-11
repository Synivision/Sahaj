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
			_messager.Publish(new StartGameMessage{
				LevelName = "level1"
			});
			TearDown();
		}

		public void StartLevel2(){
			_messager.Publish(new StartGameMessage{
				LevelName = "level2"
			});
			TearDown();
		}

		public void StartLevel3(){
			_messager.Publish(new StartGameMessage{
				LevelName = "level3"
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