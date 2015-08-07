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

public class BulletController : PoolingBehaviour
{
	private Vector3 _startPos;
	private Color _color;
	IoCResolver _resolver;
	private float startTime;
	private float journeyLength;
	private float speed = 5.0F;
	private Light muzzleFlash;
	private float muzzleFlashTime = 0.1f;
	private PoolingBehaviour _targetPirate;
	private bool _hit;
	private  UnityReferenceMaster _unityReference;
	private LineRenderer _lineRenderer;
	Vector3 randomPosition;

	public void Initialize (IoCResolver resolver, Vector3 startPos, bool hit, Color color, PoolingBehaviour targetPirate)
	{

		CameraController.shake = 1;
		_color = color;
		_resolver = resolver;
		_startPos = startPos;
		_hit = hit;
		_targetPirate = targetPirate;

		muzzleFlash = this.gameObject.GetComponent<Light> ();
		muzzleFlash.enabled = true;
		
		startTime = Time.time;
		if (_hit) {
			journeyLength = Vector3.Distance (_startPos, _targetPirate.transform.position);

		} else {
			//TODO change journey length
			randomPosition = new Vector3 (Random.Range (1, 30), Random.Range (1, 30), Random.Range (1, 30));
			journeyLength = Vector3.Distance (_startPos, _targetPirate.transform.position + randomPosition);
		}

		_resolver.Resolve (out _unityReference);

	
		_unityReference.FireDelayed (()=>{
			Delete();
		}, .15f);


		_unityReference.FireDelayed (() => {
				muzzleFlash.enabled = false;
				}, muzzleFlashTime);

		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.SetWidth(.45f,.45f);
		_lineRenderer.SetPosition(0,startPos);
	}
	
	void Update ()
	{
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = (distCovered / journeyLength) * 100;

		if (_hit) {

			transform.position = Vector3.Lerp (_startPos, _targetPirate.transform.position, fracJourney);
		} else {  
			transform.position = Vector3.Lerp (_startPos, _targetPirate.transform.position + randomPosition, fracJourney);
		}


		_lineRenderer.SetPosition(1,this.transform.position);
	}


}
