using UnityEngine;
using Assets.Code.Ui.CanvasControllers;
using System.Collections.Generic;
using System.Linq;

using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

public class BulletController : PoolingBehaviour {

	private Vector3 _startPos, _endPos;
	private Color _color;
	IoCResolver _resolver;

	private float startTime;
	private float journeyLength;
	private float speed = 5.0F;
	private Light muzzleFlash;
	private float muzzleFlashTime = 0.2f;
	public float time; 

	public void Initialize(IoCResolver resolver, Vector3 startPos, Vector3 endPos, Color color){
		_color    = color;
		_resolver = resolver;
		_startPos = startPos;
		_endPos   = endPos;

		muzzleFlash = this.gameObject.GetComponent<Light>();
		muzzleFlash.enabled = true;
		
		startTime = Time.time;
		journeyLength = Vector3.Distance(_startPos, _endPos);


		//DestroyObject(this.gameObject,3.5f);

	}

	void Update () {
		time += Time.deltaTime;
		if(time > muzzleFlashTime){
			muzzleFlash.enabled = false;

		}

		if(time>3.5f){
			DestroyObject(this.gameObject);
		}
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = (distCovered / journeyLength)*10;
		transform.position = Vector3.Lerp(_startPos, _endPos,fracJourney );


	}


}
