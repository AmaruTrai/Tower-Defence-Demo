using Cysharp.Threading.Tasks;
using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tools.Liquidated;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private List<SpawnWaveInfo> waves;
        [SerializeField]
        private float spawnRate;
        [SerializeField]
        private List<PathCreator> path;

        private SpawnWaveInfo currentSpawnInfo;
        private LiquidatedObjectPool pool;
        private CancellationTokenSource cts;

        public bool SpawnEnded
        {
            get
            {
                return waves.All(wave => wave.SpawnEnded);
            }
        }

        [Inject]
        public void Construct(LiquidatedObjectPool pool)
        {
            this.pool = pool;
        }

        private void Start()
        {
            cts = new CancellationTokenSource();
            var spawnCoroutine = SpawnCoroutine(cts.Token);
        }

        private void OnDestroy()
        {
            cts.Cancel();
        }

        private async UniTask SpawnCoroutine(CancellationToken token)
        {
            var enumerator = waves.GetEnumerator();
            while (enumerator.MoveNext())
            {
                currentSpawnInfo = enumerator.Current;
                await UniTask.Delay(TimeSpan.FromSeconds(currentSpawnInfo.TimeToSpawn), cancellationToken: token);
                while (!currentSpawnInfo.SpawnEnded)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(spawnRate), cancellationToken: token);
                    var obj = Spawn();
                    if (obj.TryGetComponent<Enemy>(out var enemy))
                    {
                        var index = UnityEngine.Random.Range(0, path.Count);
                        enemy.SetPath(path[index].path);
                        obj.transform.position = path[index].path.GetPoint(0);
                    }
 
                    obj.gameObject.SetActive(true);
                    CustomEvent.Trigger(obj.gameObject, "Run");
                }
            }
            await UniTask.Yield();
        }

        private LiquidatedObject Spawn()
        {
            var spawnsList = currentSpawnInfo.spawnList.Where((spawn) => spawn.Count > 0).ToList();
            int index = UnityEngine.Random.Range(0, spawnsList.Count);
            var spawn = spawnsList[index];
            spawn.Count--;

            return pool.Get(spawn.prefab);
        }
    }
}
