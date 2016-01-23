using System;
using System.Collections.Generic;
using Assets.Code.Logic.Logging;

namespace Assets.Code.DataPipeline
{
    public class IoCResolver
    {
        private static IoCResolver _instance;
        private readonly CanonLogger _logger;
        private readonly Dictionary<Type, object> _items;
        private bool _isLocked;

        /// FOR ISOLATED BEHAVIOURS ONLY!
        public static IoCResolver Get() { return _instance; }
        /// FOR ISOLATED BEHAVIOURS ONLY!
        public static void QuickResolve<T>(out T subject) where T : class, IResolvableItem
        {
            _instance.Resolve(out subject);
        }

        public IoCResolver(CanonLogger CanonLogger)
        {
            _logger = CanonLogger;
            _items = new Dictionary<Type, object>();
            _isLocked = false;

            if (_instance != null)
                _logger.Log("WARNING! multiple iocResolvers exist!", true);
            _instance = this;
        }

        public void RegisterItem<T>(T item) where T : class, IResolvableItem
        {
            if (_isLocked)
            {
                _logger.Log("WARNING! RegisterItem called on a locked IoCResolver!");
                return;
            }

            var registerType = typeof(T);

            if (_items.ContainsKey(registerType))
                return;

            _items.Add(registerType, item);
        }

        public T Resolve<T>() where T : class, IResolvableItem
        {
            var targetType = typeof(T);

            if (_items.ContainsKey(targetType))
                return _items[targetType] as T;

            return null;
        }

        public void Resolve<T>(out T subject) where T : class, IResolvableItem
        {
            var targetType = typeof(T);

            if (_items.ContainsKey(targetType))
                subject = _items[targetType] as T;
            else
                subject = null;
        }

        public void Lock()
        {
            _isLocked = true;
        }
    }
}
