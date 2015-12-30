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
        public GameObject AStarPlane;

        /* PROPERTIES */
        private List<DelayedAction> _delayedActions;

        public static double CurrentStaticSinValue;
        public double CurrentSinValue { get { return CurrentStaticSinValue; } }

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
            AStarPlane.gameObject.SetActive(false);
        }

        public void PauseGame()
        {
            IsPaused = true;

            if (OnGamePausedChangedEvent != null)
                OnGamePausedChangedEvent(IsPaused);
        }
        public void ResumeGame()
        {
            IsPaused = false;

            if (OnGamePausedChangedEvent != null)
                OnGamePausedChangedEvent(IsPaused);
        }

        public void VibrateDevice()
        {
#if UNITY_ANDROID
                Handheld.Vibrate();
#endif
        }

        public void Delay(Action action, float delayTime = 0f, bool ignorePaused = false)
        {
            _delayedActions.Add(new DelayedAction
            {
                Payload = action,
                RemainingTime = delayTime,
                IgnorePaused = ignorePaused
            });
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
            CurrentStaticSinValue = Math.Sin(Time.timeSinceLevelLoad / 3f);

            for (var i = 0; i < _delayedActions.Count; i++)
            {
                if (IsPaused && !_delayedActions[i].IgnorePaused) return;
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
            if (_delayedActions != null)
                _delayedActions.Clear();
        }

#if UNITY_EDITOR_WIN
        /// DOES NOT COMPILE OUTSIDE OF EDITOR - FOR DEBUGGING USE ONLY
        public void EDITOR_Time(Action function, string label = "unlabelled function")
        {
            var start = DateTime.UtcNow;
            function();
            var end = DateTime.UtcNow;

            Debug.Log(string.Format("timed function '{0}' took {1} time", label, (end - start)));
        }

        /// DOES NOT COMPILE OUTSIDE OF EDITOR - FOR DEBUGGING USE ONLY
        public T EDITOR_Time<T>(Func<T> function, string label = "unlabelled function")
        {
            var start = DateTime.UtcNow;
            var result = function();
            var end = DateTime.UtcNow;

            Debug.Log(string.Format("timed function '{0}' took {1} time", label, (end - start)));
            return result;
        }
#endif
    }
}
