using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Tools.Liquidated
{
    public class LiquidatedObjectPool
    {
        public event Action<LiquidatedObject> OnLiquidatedObjectCreated;

        private DiContainer container;
        private Dictionary<int, ObjectPool<LiquidatedObject>> pools;
        private Dictionary<int, LiquidatedObject> prefabs;

        public LiquidatedObjectPool(DiContainer container)
        {
            this.container = container;
            this.pools = new Dictionary<int, ObjectPool<LiquidatedObject>>();
            this.prefabs = new Dictionary<int, LiquidatedObject>();
        }

        public LiquidatedObject Get(LiquidatedObject prefab)
        {
            var key = prefab.GetHashCode();
            if (!pools.ContainsKey(key))
            {
                RegistrateObject(prefab);
            }
            var newObj = pools[key].Get();
            newObj.Restart();
            return newObj;
        }

        private void RegistrateObject(LiquidatedObject prefab)
        {
            var key = prefab.GetHashCode();
            if (pools.ContainsKey(key) && prefabs.ContainsKey(key))
            {
                Debug.Log($"Pool already contains a prefab: {prefab.name} with id: {key}");
            }
            {
                prefabs.Add(key, prefab);
                pools.Add(key, new ObjectPool<LiquidatedObject>(
                    createFunc: () => { return Create(key); },
                    actionOnRelease: (obj) => { obj.gameObject.SetActive(false); }
                ));
            }
        }


        private LiquidatedObject Create(int id)
        {
            if (prefabs.TryGetValue(id, out var prefab))
            {
                var newObj = container.InstantiatePrefabForComponent<LiquidatedObject>(prefab);
                newObj.gameObject.SetActive(false);
                newObj.OnLiquidation += (obj) => {
                    pools[id].Release(obj);
                };
                OnLiquidatedObjectCreated?.Invoke(newObj);
                return newObj;

            } else
            {
                Debug.Log($"Have no prefab with id: {id}");
                return null;
            }
        }

        ~LiquidatedObjectPool()
        {
            prefabs.Clear();
            pools.Clear();
        }
    }
}

