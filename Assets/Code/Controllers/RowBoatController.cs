using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.Logic.Pooling;
using Assets.Code.UnityBehaviours;

public class RowBoatController : MonoBehaviour
{

	public float speed = 1.0F;
	private float startTime;
	private float journeyLength;
	GameObject rowPrefab ;
	private PoolingObjectManager _poolingObjectManager;
	private IoCResolver _resolver;
	private UnityReferenceMaster _unityReference;
	bool isInitialised = false;

	// Use this for initialization
	public void Initialize (IoCResolver resolver)
	{
		_resolver = resolver;
		_resolver.Resolve(out _unityReference);
		isInitialised = true;
	}

	void Start(){
		startTime = Time.time;
		rowPrefab = this.gameObject;
		journeyLength = Vector3.Distance(rowPrefab.transform.position, new Vector3(-120,15,-60));	
	}

	// Update is called once per frame
	void Update ()
	{
		if (isInitialised) {
			transform.LookAt (_unityReference.Sun.transform, -Vector3.down);
		}
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		rowPrefab.transform.position = Vector3.Lerp(rowPrefab.transform.position, new Vector3(-120, 15, -60), fracJourney);
	}
}

