using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class CanvasProvider : IResolvableItem
    {
        private readonly Dictionary<string, Canvas> _canvases;

        public CanvasProvider()
        {
            _canvases = new Dictionary<string, Canvas>();
        }

        public void AddCanvas(Canvas canvas)
        {
            _canvases.Add(canvas.name, canvas);
        }

        public Canvas GetCanvas(string name)
        {
            if (!_canvases.ContainsKey(name))
            {
                Debug.Log("WARNING! canvas " + name + " does not exist");
                return null;
            }

            return _canvases[name];
        }
    }
}
