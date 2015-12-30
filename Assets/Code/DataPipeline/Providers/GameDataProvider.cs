using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Extensions;
using Assets.Code.Logic.Logging;
using Assets.Code.Models;

namespace Assets.Code.DataPipeline.Providers
{
    public class GameDataProvider : IResolvableItem
    {
        private readonly Logger _logger;
        private readonly Dictionary<Type, Dictionary<string, IGameDataModel>> _data;

        public GameDataProvider(Logger logger)
        {
            _logger = logger;
            _data = new Dictionary<Type, Dictionary<string, IGameDataModel>>();
        }

        public void AddData<T>(T data) where T : class, IGameDataModel
        {
            var targetType = typeof(T);

            if (data == null)
            {
                _logger.Log("WARNING! attempted to add null object of type " + targetType + " to GameDataProvider!", true);
                return;
            }

            if (!_data.ContainsKey(targetType))
                _data.Add(targetType, new Dictionary<string, IGameDataModel>());

            if (_data[targetType].ContainsKey(data.Name))
            {
                _logger.Log(
                    "WARNING! duplicate name (" + data.Name + ") of type " + targetType + " added to GameDataProvider!",
                    true);
                _data[targetType][data.Name] = data;
            }
            else
                _data[targetType].Add(data.Name, data);
        }

        public T GetData<T>(string name, bool expectingToFindItem = true) where T : class, IGameDataModel
        {
            if (string.IsNullOrEmpty(name)) return null;

            var requestedType = typeof(T);

            if (!_data.ContainsKey(requestedType))
            {
                _logger.Log("WARNING! no models of type " + requestedType + " exist", expectingToFindItem);
                return null;
            }

            if (!_data[requestedType].ContainsKey(name))
            {
                _logger.Log("WARNING! model of type " + requestedType + " with name " + name + " does not exist", expectingToFindItem);
                return null;
            }

            return _data[requestedType][name] as T;
        }

        public T GetRandomData<T>() where T : class, IGameDataModel
        {
            var requestedType = typeof(T);

            if (!_data.ContainsKey(requestedType))
            {
                _logger.Log("WARNING! no models of type " + requestedType + " does not exist", true);
                return null;
            }

            return _data[requestedType].ToList().GetRandomItem().Value as T;
        }

        public List<T> GetAllData<T>() where T : class
        {
            var requestedType = typeof(T);

            if (!_data.ContainsKey(requestedType))
            {
                _logger.Log("WARNING! no models of type " + requestedType + " does not exist", true);
                return new List<T>();
            }

            return _data[requestedType].Select(item => item.Value as T).ToList();
        }
    }
}
