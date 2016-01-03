using UnityEngine;
using UnityEngine.UI;
using Assets.Code.UnityBehaviours.Pooling;

public class CameraController : MonoBehaviour {
	
	// The rate of change of the field of view in perspective mode.
	public float PerspectiveZoomSpeed = 0.5f;        
	// The rate of change of the orthographic size in orthographic mode.s
	public float OrthoZoomSpeed = 0.5f;        
	
	
	public float MoveSensitivityX = 150.0f;
	public float MoveSensitivityY = 150.0f;
	
	public bool InvertMoveX = false;
	public bool InvertMoveY = false;
	
	private float _scrollVelocity = 0.0f;
	private Vector2 _scrollDirection = Vector2.zero;
	
	private float _timeTouchPhaseEnded;
	private Camera _camera;
	
	private float _maxBoundX;
	private float _maxBoundY;
	private float _maxBoundZ;
	private float _minBoundX;
	private float _minBoundY;
	private float _minBoundZ;
	
	// How long the object should shake for.
	private float _shake = 2f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	private const float ShakeScale = 2f;
	private const float ShakeFallOff = 10.0f;
	private const float MaxShake = 1f;
	float mouseSensitivity = 1.0f;
	Vector3 lastPosition;
	
	Vector3 originalPos;
	
	public void Start(){
		
		_camera = GetComponent<Camera>();
		originalPos = _camera.transform.localPosition;
		_maxBoundX = this.transform.position.x + 80;
		_minBoundX = this.transform.position.x - 80;
		_maxBoundZ = this.transform.position.z + 80;
		_minBoundZ = this.transform.position.z - 80;
		_maxBoundY = this.transform.position.y;
		_minBoundY = this.transform.position.y;
		
	}
	
	public void ApplyShake(float shakeAmount)
	{
		_shake += shakeAmount * ShakeScale;
	}
	
	public void Update()
	{
		

		if (_shake > 0)
		{
			if (_shake > MaxShake)
				_shake = MaxShake;
			
			_camera.transform.localPosition = originalPos + Random.insideUnitSphere * ShakeScale;
			
			_shake -= Time.deltaTime * ShakeFallOff;
		}
		else
		{
			_shake = 0f;
			//_camera.transform.localPosition = originalPos;
		}
		
		Touch[] touches = Input.touches;
		
		if (touches.Length == 1)
		{
			if (touches[0].phase == TouchPhase.Began)
			{
				_scrollVelocity = 0.0f;
			}
			else if (touches[0].phase == TouchPhase.Moved)
			{
				_shake = 0f;
				Vector2 delta = touches[0].deltaPosition;
				
				float positionX = delta.x * MoveSensitivityX * Time.deltaTime;
				positionX = InvertMoveX ? positionX : positionX * -1;
				
				float positionY = delta.y * MoveSensitivityY * Time.deltaTime;
				positionY = InvertMoveY ? positionY : positionY * -1;
				
				_camera.transform.position += new Vector3 (positionX, 0, positionY);
				
				_scrollDirection = touches[0].deltaPosition.normalized;
				_scrollVelocity = (touches[0].deltaPosition.magnitude / touches[0].deltaTime)*100f;
				
				print(_scrollVelocity.ToString());
				
				if (_scrollVelocity <= 500)
					_scrollVelocity = 0;
				
			}
			else if (touches[0].phase == TouchPhase.Ended)
			{
				_timeTouchPhaseEnded = Time.time;
			}
		}
		
		// If there are two touches on the device...

		if (Input.GetMouseButtonDown(0))
		{
			lastPosition =  Input.mousePosition;
		}
		
		if (Input.GetMouseButton(0) && isButton() == false)
		{
			Vector3 delta  = Input.mousePosition - lastPosition;
			lastPosition =  Input.mousePosition;
			
			
			transform.Translate(delta.x * mouseSensitivity,delta.y * mouseSensitivity,0);
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x, _minBoundX, _maxBoundX),
				Mathf.Clamp(transform.position.y, _minBoundY, _maxBoundY),
				Mathf.Clamp(transform.position.z, _minBoundZ, _maxBoundZ));
		}

		if (Input.GetMouseButtonUp (0) && isButton() == false) {
		
			originalPos = _camera.transform.localPosition;
		
		}
		
		if (Input.touchCount == 2 && isButton() == false)
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
			if (_camera.orthographic )
			{
				// ... change the orthographic size based on the change in distance between the touches.
				_camera.orthographicSize += deltaMagnitudeDiff * OrthoZoomSpeed;
				
				// Make sure the orthographic size never drops below zero.
				_camera.orthographicSize = Mathf.Max(_camera.orthographicSize, 0.1f);
			}
			else
			{
				// Otherwise change the field of view based on the change in distance between the touches.
				_camera.fieldOfView += deltaMagnitudeDiff * PerspectiveZoomSpeed;
				
				// Clamp the field of view to make sure it's between 0 and 180.
				_camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 40f, 140f);
			}
			
		}


	}

	private bool isButton()
	{
		bool result = true;
		UnityEngine.EventSystems.EventSystem ct
			= UnityEngine.EventSystems.EventSystem.current;
		
		if (! ct.IsPointerOverGameObject() ) result = false;
		if (! ct.currentSelectedGameObject ) result = false;

		
		return result;
	}
	
}