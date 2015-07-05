using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;

namespace Assets.Code.Ui.CanvasControllers
{

		public class MainCanvasController : BaseCanvasController {

				private readonly Button _makePirateButton;
				private readonly Button _tearDownPirateButton;
				LevelManager levelManager;

				public MainCanvasController(IoCResolver resolver, Canvas canvasView) : base(canvasView){

					ResolveElement(out _makePirateButton, "make_pirate_button");
					ResolveElement(out _tearDownPirateButton, "teardown_pirate_button");
				
					levelManager = new LevelManager();

					_makePirateButton.onClick.AddListener(generateRandomObjects);
					_tearDownPirateButton.onClick.AddListener(tearDownObjects);
				}

				void generateRandomObjects(){
					
					
					levelManager.CreatePirate();

				}
				
				void tearDownObjects(){
					System.Console.WriteLine("teardown object");
					Debug.Log("teardown listener");
					levelManager.TearDownPirates();
					
			//PirateInfoCanvasController.ToggleCanvas(false);
				}


		}
	
}
