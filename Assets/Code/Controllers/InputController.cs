using UnityEngine;
using System.Collections;
using Assets.Code.Ui.CanvasControllers;
using Assets.Code.Messaging;
using Assets.Code.DataPipeline;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;
using Assets.Code.UnityBehaviours.Pooling;

public class InputController : PoolingBehaviour
{
	
	private bool _mouseState;
	private GameObject target;
	public Vector3 screenSpace;
	public Vector3 offset;
	private float _time = 5;
	private CameraController myCameraController;
	private Messager _messager;
	private IoCResolver _resolver;


	public void Initialize(IoCResolver resolver){

		myCameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		_resolver = resolver;
		resolver.Resolve (out _messager);
	}

	/*
	void Start(){
		//myCameraController = GetComponent<CameraController> ();
		myCameraController = this.gameObject.GetComponent<CameraController>();
		//IoCResolver resolver = GameObject.Find("state_master").GetComponent<StateMaster>().Resolver;
		//resolver.Resolve(out _messager);
	}
	*/

	void Update ()
	{
		_time += Time.deltaTime;
		if (_time > 3) {
			_messager.Publish (new PirateInfoCanvasMessage{
				toggleValue = false
			});
		}
		//used to move cube around 
		if (Input.GetMouseButtonDown (0)) {
	
			RaycastHit hitInfo;
			target = GetClickedObject (out hitInfo);

			if (target != null && (target.gameObject.tag == "Cube" || target.gameObject.tag == "Player" )) {
				_mouseState = true;
				screenSpace = Camera.main.WorldToScreenPoint (target.transform.position);
				offset = target.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
				
				myCameraController.enabled = false;

			}

			if (target != null && target.gameObject.tag == "Player") {
				
				//send message to pirate canvas controller to display pirate info
				PirateController playerObject = target.GetComponent<PirateController> ();
				playerObject.UpdateUiPanel ();
				_time = 0;
				
				_messager.Publish (new PirateInfoCanvasMessage{
					toggleValue = true
				});
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			_mouseState = false;
			myCameraController.enabled = true;
		}

		if (_mouseState) {
			
			var curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
			var curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
			curPosition.y = target.transform.position.y;
			target.transform.position = curPosition;
		}
	}
	
	GameObject GetClickedObject (out RaycastHit hit)
	{
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray.origin, ray.direction * 10, out hit)) {
			target = hit.collider.gameObject;
		}
		return target;
	}
	
}
