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
        _unityReference.Delay(() => Delete(), 2f);

    }

    void Update() {
        float distCovered = (Time.time - startTime);
        float fracJourney = (distCovered / journeyLength) * 150;
  
       // transform.position = Vector3.Lerp(_startPos, _target, fracJourney);
       

        // calculate current time within our lerping time range
        float cTime = fracJourney;
        float trajectoryHeight = 30f;
        // calculate straight-line lerp position:
        Vector3 currentPos = Vector3.Lerp(_startPos, _target, cTime);
        // add a value to Y, using Sine to give a curved trajectory in the Y direction
        currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
        // finally assign the computed position to our gameObject:
        transform.position = currentPos;

         _poolingParticleManager.Emit("blood_prefab", _target, Color.green, 200);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube") {
            //Damage Building
        }
    }


}
  

