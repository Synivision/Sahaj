using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

public class BombBehaviour : PoolingBehaviour{

    BombModel model;
    private float startTime;
    private float journeyLength;
    private float speed = 1.0F;
   
    private Vector3 _startPos;
    private Vector3 _target;
    private UnityReferenceMaster _unityReference;
    private PoolingParticleManager _poolingParticleManager;

    public void Initialize(IoCResolver resolver, BombModel model)
    {
        this.model = model;
        resolver.Resolve(out _poolingParticleManager);
        resolver.Resolve(out _unityReference);


        startTime = Time.time;
        gameObject.GetComponent<Renderer>().material.color = model.color;
        _startPos = model.startPos;
        _target = model.endPos;

        journeyLength = Vector3.Distance(_startPos, _target);
       
        _unityReference.FireDelayed(() => { Delete(); }, 2f);

    }

    void Update() {
        float distCovered = (Time.time - startTime) * 5;
        float fracJourney = (distCovered / journeyLength) * 100;
  
        transform.position = Vector3.Lerp(_startPos, _target, fracJourney);

        _poolingParticleManager.Emit("blood_prefab", _target, Color.green, 200);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube") {
            //Damage Building
            Destroy(this.gameObject);

        }
    }


}
  

