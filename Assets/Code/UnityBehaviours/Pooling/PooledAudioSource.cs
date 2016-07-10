using Assets.Code.Models.Pooling;
using UnityEngine;

namespace Assets.Code.UnityBehaviours.Pooling
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : InitializeRequiredBehaviour
    {
        /* REFERENCES */
        private UnityReferenceMaster _unity;
        private Transform _cameraTransform;
        private AudioSource _audio;

        /* PROPERTIES */
        private PooledAudioRequest _currentRequest;
        public bool IsActive { get { return _currentRequest != null; } }

        // lerpy loo
        private bool _isTrailingOff;
        private float _trailOffTime;
        private float _trailOffProgress;

        public void Initialize(UnityReferenceMaster unity)
        {
            _cameraTransform = unity.Camera.transform;
            _audio = GetComponent<AudioSource>();
            
            _unity = unity;
            _unity.OnGamePausedChangedEvent += OnGamePausedChanged;

            MarkAsInitialized();
        }

        private void OnGamePausedChanged(bool isGamePaused)
        {
            if (IsActive && _audio.loop)
                if (isGamePaused)
                    _audio.Pause();
                else
                    _audio.Play();
        }

        public AudioToken PlaySound(PooledAudioRequest request)
        {
            _isTrailingOff = false;
            if (_currentRequest != null && _currentRequest.OnFinished != null)
                _currentRequest.OnFinished();
            _currentRequest = request;

            // bail gracefully if request is null
            if (request == null)
                return new AudioToken
                {
                    IsCurrentlyActive = false,

                    Replace = replacementRequest => PlaySound(replacementRequest),
                    End = () => { },
                    TrailOff = time => { }
                };

            _audio.loop = request.Next == null && request.IsLoop;
            _audio.transform.position = request.IsSpatial ? request.Target : _cameraTransform.position;

            _audio.clip = request.Sound;
            _audio.volume = request.Volume;
            _audio.Play();

            var token = new AudioToken
            {
                IsCurrentlyActive = true,

                Replace = replacementRequest => PlaySound(replacementRequest),
                TrailOff = time => TrailOff(time)
            };
            token.End = () =>
            {
                Stop();
                token.IsCurrentlyActive = false;
            };
            return token;
        }

        public void Stop()
        {
            _isTrailingOff = false;
            _audio.Stop();
            _currentRequest = null;
        }

        public void TrailOff(float time)
        {
            if (_isTrailingOff) return;

            _isTrailingOff = true;
            _trailOffProgress = 0f;
            _trailOffTime = time;
        }

        public void FixedUpdate()
        {
            if (_currentRequest == null || _unity.IsPaused) return;

            if (_isTrailingOff)
            {
                _audio.volume = _currentRequest.Volume * (1 - (_trailOffProgress / _trailOffTime));
                _trailOffProgress += Time.deltaTime;

                if (_trailOffProgress >= _trailOffTime)
                {
                    if (_currentRequest.OnFinished != null)
                        _currentRequest.OnFinished();
                    Stop();
                }
            }

            // once finished, we move on to the next sound
            // or it will be null (both are cool)
            else if (!_audio.isPlaying)
            {
                if (_currentRequest.Next != null)
                    PlaySound(_currentRequest.Next);
                else if (_currentRequest.OnFinished != null)
                    _currentRequest.OnFinished();
                else
                    Stop();
            }

            else if (!_currentRequest.IsSpatial)
                transform.position = _cameraTransform.position;
        }

        public void Delete()
        {
            _unity.OnGamePausedChangedEvent -= OnGamePausedChanged;
        }
    }
}
