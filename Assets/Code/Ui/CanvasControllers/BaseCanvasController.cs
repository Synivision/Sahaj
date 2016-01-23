using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.Logic.Logging;
using UnityEngine;

namespace Assets.Code.Ui.CanvasControllers
{
    public abstract class BaseCanvasController
    {
        private readonly CanonLogger _logger;
        protected readonly Canvas _canvasView;

        private readonly Dictionary<string, GameObject> _elements;
        private const string PathSeperator = "/";

        protected BaseCanvasController(IoCResolver resolver, Canvas canvasView)
        {
            _elements = new Dictionary<string, GameObject>();
            resolver.Resolve(out _logger);

            _canvasView = canvasView;

            _canvasView.gameObject.SetActive(true);

            CacheObject("", canvasView.gameObject);
        }

        private void CacheObject(string parentPath, GameObject subject)
        {
            for (var i = 0; i < subject.transform.childCount; i++)
            {
                var child = subject.transform.GetChild(i);
                var transformPath = parentPath + child.name;

                if (_elements.ContainsKey(transformPath))
                {
                    _logger.Log("WARNING! found duplicate child name : " + transformPath + " in " + this + "!", true);
                    continue;
                }

                _elements.Add(transformPath, child.gameObject);
                CacheObject(transformPath + PathSeperator, child.gameObject);
            }
        }

        public virtual void Update() { }

        public virtual void TearDown()
        {
            _canvasView.gameObject.SetActive(false);
        }

        protected GameObject GetElement(string name)
        {
            if (!_elements.ContainsKey(name))
            {
                _logger.Log(string.Format("WARNING! canvas controller ({0}) does not have element named '{1}'", GetType(), name), true);
                return null;
            }

            return _elements[name];
        }

        protected T GetElement<T>(string name) where T : Component
        {
            if (!_elements.ContainsKey(name))
            {
                _logger.Log(string.Format("WARNING! canvas controller ({0}) does not have element named '{1}'", GetType(), name), true);
                return null;
            }

            return _elements[name].GetComponent<T>();
        }

        protected void ResolveElement<T>(out T element, string name) where T : Component
        {
            if (!_elements.ContainsKey(name))
            {
                _logger.Log(string.Format("WARNING! canvas controller ({0}) does not have element named '{1}'", GetType(), name), true);
                element = null;
                return;
            }

            element = _elements[name].GetComponent<T>();
        }
    }
}
