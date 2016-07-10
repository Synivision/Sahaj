using UnityEngine;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;

public class TrapController : PoolingBehaviour {


	private  UnityReferenceMaster _unityReference;
	IoCResolver _resolver;
	private PoolingParticleManager _poolingParticleManager;

	public void Initialize (IoCResolver resolver, Vector3 pos)
	{
		_resolver = resolver;
		_resolver.Resolve(out _poolingParticleManager);

		this.transform.position = pos;
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log("trigger");
		if(other.gameObject.tag == "Player"){

			_poolingParticleManager.Emit("explosion_05", transform.position, Color.red, 1000);
			int pirateCurrentHealth  = (int)other.gameObject.GetComponent<PirateController>().Stats.CurrentHealth; 
			other.gameObject.GetComponent<PirateController>().Stats.CurrentHealth = pirateCurrentHealth - 70;
			other.gameObject.GetComponent<PirateController>()._healthBar.value = pirateCurrentHealth - 70;
			Destroy(this.gameObject);
		}

	}
}
