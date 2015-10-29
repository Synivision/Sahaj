using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;

public class BombBehaviour : PoolingBehaviour{

    BombModel model;
    private float startTime;
    private float journeyLength;
    private float speed = 5.0F;
    private Vector3 _startPos;
    private Vector3 _target;
    private PoolingParticleManager _poolingParticleManager;

    public void Initialize(IoCResolver resolver, BombModel model)
    {
        this.model = model;
        resolver.Resolve(out _poolingParticleManager);

        gameObject.GetComponent<Renderer>().material.color = model.color;
        _startPos = model.startPos;
        _target = model.endPos;

        journeyLength = Vector3.Distance(_startPos, _target);

        _poolingParticleManager.Emit("blood_prefab", transform.position, Color.green, 200);
    }

    void Update() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = (distCovered / journeyLength) * 100;
  
        transform.position = Vector3.Lerp(_startPos, _target, fracJourney);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube") {
            //Damage Building
            Destroy(this.gameObject);

        }
    }


}
  

