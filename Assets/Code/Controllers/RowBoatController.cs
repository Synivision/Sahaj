using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.Logic.Pooling;
using Assets.Code.UnityBehaviours;

public class RowBoatController : MonoBehaviour
{

	public float speed = 1.0F;
	public float startTime;
	public float journeyLength;
	public GameObject rowPrefab ;
	private PoolingObjectManager _poolingObjectManager;
	private IoCResolver _resolver;
	private UnityReferenceMaster _unityReference;
	bool isInitialised = false;
	bool hasReachedDestination = false;
	public Vector3 destinationPosition;

	// Use this for initialization
	public void Initialize (IoCResolver resolver)
	{
		_resolver = resolver;
		_resolver.Resolve(out _unityReference);
		isInitialised = true;
	}

	void Start(){
		startTime = Time.time;
		//rowPrefab = this.gameObject;
		//destinationPosition = transform.position;	
	}

	// Update is called once per frame
	void Update ()
	{
		if (isInitialised) {
			transform.LookAt (_unityReference.Sun.transform, -Vector3.down);
		}

		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		rowPrefab.transform.position = Vector3.Lerp(rowPrefab.transform.position, destinationPosition, fracJourney);
	}
}

