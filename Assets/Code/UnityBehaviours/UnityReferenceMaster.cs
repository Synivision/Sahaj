using System;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Models;
using UnityEngine;

namespace Assets.Code.UnityBehaviours
{
    public delegate void OnDebugModeChangedEventHandler(bool debugModeActive);
    public delegate void OnGamePausedChangedEventHandler(bool isPaused);

    public class UnityReferenceMaster : MonoBehaviour, IResolvableItem
    {
        /* REFERENCES */
        public CameraController Camera;
		public GameObject Sun;
        /* PROPERTIES */
        private List<DelayedAction> _delayedActions;

        // use this instead of calculating sin inside of many scripts
        public double CurrentSinValue { get; private set; }

        private bool _debugModeActive;
        public bool DebugModeActive
        {
            get { return _debugModeActive; }
            set
            {
                _debugModeActive = value;

                if (OnDebugModeChangedEvent != null)
                    OnDebugModeChangedEvent(value);
            }
        }
        public OnDebugModeChangedEventHandler OnDebugModeChangedEvent;

        public bool IsPaused { get; private set; }
        public OnGamePausedChangedEventHandler OnGamePausedChangedEvent;

        public void Awake()
        {
            _delayedActions = new List<DelayedAction>();
        }

        public void PauseGame()
        {
            IsPaused = true;

            if(OnGamePausedChangedEvent != null)
                OnGamePausedChangedEvent(IsPaused);
        }
        public void ResumeGame()
        {
            IsPaused = false;

            if (OnGamePausedChangedEvent != null)
                OnGamePausedChangedEvent(IsPaused);
        }

        public void FireDelayed(Action action, float delayTime)
        {
            _delayedActions.Add(new DelayedAction
            {
                Payload = action,
                RemainingTime = delayTime
            });
        }

        public void VibrateDevice()
        {
            #if UNITY_ANDROID
                Handheld.Vibrate();
            #endif
        }

        public void LoadCanvases(CanvasProvider canvasProvider)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var childCanvas = transform.GetChild(i).GetComponent<Canvas>();

                if (childCanvas != null)
                {
                    childCanvas.gameObject.SetActive(false);
                    canvasProvider.AddCanvas(childCanvas);
                }
            }
        }

        public void FixedUpdate()
        {
            CurrentSinValue = Math.Sin(Time.timeSinceLevelLoad);

            if(!IsPaused)
                for (var i = 0; i < _delayedActions.Count; i++)
                {
                    _delayedActions[i].RemainingTime -= Time.deltaTime;

                    if (_delayedActions[i].RemainingTime <= 0)
                    {
                        _delayedActions[i].Payload();
                        _delayedActions.RemoveAt(i);

                        i--;
                    }
                }
        }

        public void ResetDelayedActions()
        {
            _delayedActions.Clear();
        }
    }
}
