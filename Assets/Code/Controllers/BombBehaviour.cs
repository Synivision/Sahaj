using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;

public class BombBehaviour : PoolingBehaviour{

    BombModel model;
    public void Initialize(IoCResolver resolver, BombModel model)
    {
        this.model = model;


    }

    //void Update()
    //{
    //    float distCovered = (Time.time - startTime) * speed;
    //    float fracJourney = (distCovered / journeyLength) * 100;

    //    if (_hit)
    //    {

    //        transform.position = Vector3.Lerp(_startPos, _target, fracJourney);
    //    }
    //    else
    //    {
    //        transform.position = Vector3.Lerp(_startPos, _target + randomPosition, fracJourney);
    //    }
    //}
}
