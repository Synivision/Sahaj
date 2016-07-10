using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.DataPipeline;
using UnityEngine.UI;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

namespace Assets.Code.Ui.CanvasControllers
{
	public class PirateInfoCanvasController : BaseCanvasController
	{

		private Messager _messager;
		private readonly SpriteProvider _spriteProvider;
		private readonly PrefabProvider _prefabProvider;
		private  Canvas _canvas;
		private GameObject panel;

		/*Display Info*/
		private  Text _healthText;
		private  Text _attackDamageText;
		private  Text _nameText;
		private  Text _descriptionText;
				

		//token

		private readonly MessagingToken _onPirateInspectionSelected;
		private readonly MessagingToken _onToggleCanvas;

        public PirateInfoCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{

			resolver.Resolve (out _messager);
			resolver.Resolve (out _spriteProvider);
			resolver.Resolve (out _prefabProvider);
							
			ResolveElement (out _healthText, "health");
			ResolveElement (out _attackDamageText, "damage");
			ResolveElement (out _nameText, "name");
			ResolveElement (out _descriptionText, "description");
			panel = _canvasView.transform.FindChild ("Info_Panel").gameObject;

			_canvas = _canvasView;
			_canvas.enabled = false;
						
			//token
			_onToggleCanvas = _messager.Subscribe<PirateInfoCanvasMessage> (OnToggleCanvas);
			_onPirateInspectionSelected = _messager.Subscribe<PirateMessage> (OnPirateInspectionSelected);

		}

		public void OnPirateInspectionSelected (PirateMessage message)
		{

			_healthText.text = "Health : " + message.model.Stats.MaximumHealth;
			_attackDamageText.text = "Attack Damage : " + message.model.Stats.MaximumDamage;
			_nameText.text = "Name : " + message.model.PirateName;
			_descriptionText.text = message.model.Descipriton;

		}


		private  void OnToggleCanvas (PirateInfoCanvasMessage message)
		{
			_canvas.enabled = message.toggleValue;
								
		}

		public override void TearDown()
		{	
			_messager.CancelSubscription (_onToggleCanvas,_onPirateInspectionSelected);
			base.TearDown();
		}
			
	}

}
