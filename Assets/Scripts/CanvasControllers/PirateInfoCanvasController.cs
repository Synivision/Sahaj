using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.DataPipeline;
using UnityEngine.UI;
using Assets.Code.Messaging;


namespace Assets.Code.Ui.CanvasControllers
{
		public class PirateInfoCanvasController : BaseCanvasController {

				private Messager _message;
				private static Canvas _canvas;
				private GameObject panel;

				/*Display Info*/
				private static Text _healthText;
				private static Text _attackDamageText;
				private static Text _nameText;
				private static Text _descriptionText;
				

		       // private static PirateModel data;

				public PirateInfoCanvasController(IoCResolver resolver, Canvas canvasView) : base(canvasView){
					
					ResolveElement(out _healthText, "health");
					ResolveElement(out _attackDamageText, "damage");
					ResolveElement(out _nameText, "name");
					ResolveElement(out _descriptionText, "description");
					panel =_canvasView.transform.FindChild("Info_Panel").gameObject;


					_healthText.transform.SetParent(panel.transform);
					

			/*
					_healthText.transform.position = new Vector3(panel.transform.position.x,panel.transform.position.y+30,panel.transform.position.z);
					_attackDamageText.transform.position = new Vector3(panel.transform.position.x,panel.transform.position.y+17,panel.transform.position.z);
					_nameText.transform.position = new Vector3(panel.transform.position.x,panel.transform.position.y+4,panel.transform.position.z);
					_descriptionText.transform.position = new Vector3(panel.transform.position.x,panel.transform.position.y-9,panel.transform.position.z);
					*/



					_canvas = _canvasView;
					ToggleCanvas(false);
					
				}

				public static void setDisplayInfo(PirateModel model){

					UnityEngine.Debug.Log(model.Health.ToString()+"in Canvas Controller");
					_healthText.text = "Health : "+model.Health.ToString();
					_attackDamageText.text = "Attack Damage : "+model.AttackDamage.ToString();
					_nameText.text = "Name : "+model.Name;
					_descriptionText.text =  model.Descipriton;

				}

				public static void ToggleCanvas(bool value){
					_canvas.enabled =value;
								
				}

			
		}

}
