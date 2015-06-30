using System.Collections.Generic;
using Assets.Code.Ui.CanvasControllers;

namespace Assets.Code.Ui
{
    public class UiManager
    {
        private readonly List<BaseCanvasController> _controllers;

        public UiManager()
        {
            _controllers = new List<BaseCanvasController>();
        }

        public void RegisterUi(BaseCanvasController controller)
        {
            _controllers.Add(controller);
        }

        public void Update()
        {
            foreach (var controller in _controllers)
                controller.Update();
        }

        public void TearDown()
        {
            foreach (var controller in _controllers)
                controller.TearDown();

            _controllers.Clear();
        }
    }
}
