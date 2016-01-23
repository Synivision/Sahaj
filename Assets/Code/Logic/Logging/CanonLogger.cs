using Assets.Code.DataPipeline;
using Assets.Code.Utilities;
using UnityEngine;

namespace Assets.Code.Logic.Logging
{
    public class CanonLogger : IResolvableItem
    {
        private readonly string _logPath;
        private readonly bool _logToUnity;

        private int _backlogIndex;
        private readonly string[] _backlog;

        public CanonLogger(string logPath, bool logToUnity)
        {
            _backlogIndex = 0;
            _backlog = new string[50];

            _logPath = logPath;
            _logToUnity = logToUnity;
        }

        public void Log(string message, bool isOfConcern = false)
        {
            if (_logToUnity || isOfConcern)
                Debug.Log(message);

            _backlog[_backlogIndex] = message;
            _backlogIndex++;

            if (_backlogIndex >= _backlog.Length - 1)
                Flush();
        }

        public void Flush()
        {
            var output = "";
            for (var i = 0; i < _backlogIndex; i++)
            {
                if (_backlog[i] != null)
                    output += _backlog[i] + "\n";

                _backlog[i] = null;
            }

            FileServices.AppendToFile(_logPath, output);

            _backlogIndex = 0;
        }
    }
}
