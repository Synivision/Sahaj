using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;
using UnityEngine;
using System.Collections.Generic;

public class CannonBombBehaviour : PoolingBehaviour
{
	BombModel model;
	int bombCount;
	private float speed = 1.0F;

	private Vector3 _startPos;
	private Vector3 _target;
	private UnityReferenceMaster _unityReference;
	private PoolingParticleManager _poolingParticleManager;

	private List<GameObject> _bombsList;
	private PoolingObjectManager _poolingObjectManager;
	// Use this for initialization
	void Start () {
	
	}

	public void Initialize(IoCResolver resolver, BombModel model, int bombCount)
	{
		this.model = model;
		this.bombCount = bombCount;
		resolver.Resolve(out _poolingParticleManager);
		resolver.Resolve(out _unityReference);
		resolver.Resolve(out _poolingObjectManager);

		_startPos = model.startPos;
		_target = model.endPos;

		//_unityReference.Delay(() => Delete(), 2f);
		_bombsList = new List<GameObject>();
		for (int i = 0; i<bombCount; i++) {
			var fab = _poolingObjectManager.Instantiate("cannon_bomb_prefab").gameObject;
			int randomNumber = Random.Range(0,5);
			fab.transform.position = _startPos + new Vector3(10 * randomNumber, 0, 10 * randomNumber);
			_bombsList.Add(fab);
		}

		_unityReference.Delay(() => {
			for (int i = 0; i < bombCount; i++) {
				var fab = _bombsList[i];
				GameObject.Destroy(fab);
			}

			GameObject.Destroy(this.gameObject);

		},2.5f);

	}

	// Update is called once per frame
	void Update () {
		if (bombCount>0) {

			for (int i = 0; i < bombCount; i++)
			{
				var fab = _bombsList[i];
				int randomNumber = Random.Range(0, 5);
				fab.GetComponent<Rigidbody>().AddForce(Vector3.down * 300 * randomNumber);

			}
		}

	}
}
