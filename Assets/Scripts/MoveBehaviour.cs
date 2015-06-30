using UnityEngine;

public delegate void OnLerpEndEventHandler();

public class MoveBehaviour : MonoBehaviour {
	void Start () {
		

	}
	
	void Update () {

	}

	public void move(Vector3 startPos, Vector3 endPos, float timer){

		transform.position = Vector3.Lerp(startPos, endPos, (Time.time - timer)/2);

	}

    /* ALTERNATE
    private const float LerpTime = 1f;

    public OnLerpEndEventHandler OnLerpEndEvent;

    private Vector3 _oldPosition;
    private Vector3 _targetPosition;
    private float _lerpProgress;

    public void Update()
    {
        _lerpProgress += Time.deltaTime;
        var lerpPercentage = _lerpProgress / LerpTime;

        transform.position = Vector3.Lerp(_oldPosition, _targetPosition, lerpPercentage / 2);

        if (lerpPercentage > LerpTime && OnLerpEndEvent != null)
            OnLerpEndEvent();
    }

    public void LerpToTarget(Vector3 target)
    {
        _oldPosition = transform.position;
        _targetPosition = target;

        _lerpProgress = 0f;
    }
     */
}
