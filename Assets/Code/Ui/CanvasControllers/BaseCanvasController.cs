using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Ui.CanvasControllers
{
    public abstract class BaseCanvasController
    {
        protected readonly Canvas _canvasView;

        private readonly Dictionary<string, GameObject> _elements;

        protected BaseCanvasController(Canvas canvasView)
        {
            _elements = new Dictionary<string, GameObject>();

            _canvasView = canvasView;

            _canvasView.gameObject.SetActive(true);

            for (var i = 0; i < canvasView.transform.childCount; i++)
            {
                var child = canvasView.transform.GetChild(i);
                if (_elements.ContainsKey(child.name))
                {
                    Debug.Log("WARNING! found duplicate child name : " + child.name + " in " + this + "!");
                    continue;
                }
                _elements.Add(child.name, child.gameObject);
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
                Debug.Log("WARNING! canvas controller (" + GetType() + ") does not have element named " + name);
                return null;
            }

            return _elements[name];
        }

        protected T GetElement<T>(string name) where T : Component
        {
            if (!_elements.ContainsKey(name))
            {
                Debug.Log("WARNING! canvas controller (" + GetType() + ") does not have element named " + name);
                return null;
            }

            return _elements[name].GetComponent<T>();
        }

        protected void ResolveElement<T>(out T element, string name) where T : Component
        {
            if (!_elements.ContainsKey(name))
            {
                Debug.Log("WARNING! canvas controller (" + GetType() + ") does not have element named " + name);
                element = null;
                return;
            }

            element = _elements[name].GetComponent<T>();
        }
    }
}
