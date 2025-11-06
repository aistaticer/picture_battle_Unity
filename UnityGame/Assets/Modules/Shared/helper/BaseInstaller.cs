using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;

namespace picture_game_view.Assets.Modules.Shared
{
    public static class InstallerHelper
    {
        public static void BindMono<T>(DiContainer container) where T : MonoBehaviour
        {
            var go = new GameObject(typeof(T).Name);
            var component = container.InstantiateComponent<T>(go);
            container.BindInstance(component).AsSingle();

            Debug.Log($"[Zenject] Bound MonoBehaviour: {typeof(T).Name}");
        }
        
        public static void BindClass<T>(DiContainer container) where T : class
        {
            var instance = container.Instantiate<T>();
            container.Bind<T>().FromInstance(instance).AsSingle();

            Debug.Log($"[Zenject] Bound Class: {typeof(T).Name}");
        }

        public static void BindSO<T>(DiContainer container, String folderPath) where T : ScriptableObject
        {
            var asset = Resources.Load<T>(folderPath + typeof(T).Name);
            if (asset != null)
                container.Bind<T>().FromInstance(asset).AsSingle();
        }
    }
}