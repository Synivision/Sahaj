using UnityEngine;


[RequireComponent(typeof(MoveBehaviour))]
public class PirateController : MonoBehaviour {


    private MoveBehaviour _moveBehaviour;

    void Start()
    {
        _moveBehaviour = GetComponent<MoveBehaviour>();

        ResetLerp();
        _moveBehaviour.OnLerpEndEvent += ResetLerp;
    }

    private void ResetLerp()
    {
        _moveBehaviour.LerpToTarget(new Vector3(Random.Range(-50f, 20f), 2, Random.Range(-50f, 20f)));
    }

}
