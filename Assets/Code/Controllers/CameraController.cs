using UnityEngine;
using System.Collections;
using Assets.Code.UnityBehaviours.Pooling;

public class CameraController : PoolingBehaviour {

	// The rate of change of the field of view in perspective mode.
	public float perspectiveZoomSpeed = 0.5f;        
	// The rate of change of the orthographic size in orthographic mode.s
	public float orthoZoomSpeed = 0.5f;        


	public float moveSensitivityX = 150.0f;
	public float moveSensitivityY = 150.0f;

	public bool invertMoveX = false;
	public bool invertMoveY = false;

	private float scrollVelocity = 0.0f;
	private Vector2 scrollDirection = Vector2.zero;

	private float timeTouchPhaseEnded;
	private Camera _camera;

	// How long the object should shake for.
	public static float shake = 2f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;

	void Start(){

		_camera =GameObject.Find("Main Camera").GetComponent<Camera>();
		originalPos = _camera.transform.localPosition;
	}
	
	void Update()
	{

		if (shake > 0)
		{
			_camera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			_camera.transform.localPosition = originalPos;
		}

		Touch[] touches = Input.touches;

		if (touches.Length == 1)
		{
			if (touches[0].phase == TouchPhase.Began)
			{
				scrollVelocity = 0.0f;
			}
			else if (touches[0].phase == TouchPhase.Moved)
			{


					Vector2 delta = touches[0].deltaPosition;
					
					float positionX = delta.x * moveSensitivityX * Time.deltaTime;
					positionX = invertMoveX ? positionX : positionX * -1;
					
					float positionY = delta.y * moveSensitivityY * Time.deltaTime;
					positionY = invertMoveY ? positionY : positionY * -1;
					
					_camera.transform.position += new Vector3 (positionX, 0, positionY);
					
					scrollDirection = touches[0].deltaPosition.normalized;
					scrollVelocity = (touches[0].deltaPosition.magnitude / touches[0].deltaTime)*100f;
					
					print(scrollVelocity.ToString());
					
					if (scrollVelocity <= 500)
						scrollVelocity = 0;

			}
			else if (touches[0].phase == TouchPhase.Ended)
			{
				timeTouchPhaseEnded = Time.time;
			}
		}

		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			
			// If the camera is orthographic...
			if (_camera.orthographic)
			{
				// ... change the orthographic size based on the change in distance between the touches.
				_camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
				
				// Make sure the orthographic size never drops below zero.
				_camera.orthographicSize = Mathf.Max(_camera.orthographicSize, 0.1f);
			}
			else
			{
				// Otherwise change the field of view based on the change in distance between the touches.
				_camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
				
				// Clamp the field of view to make sure it's between 0 and 180.
				_camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 40f, 140f);
			}
		}
	}
}
