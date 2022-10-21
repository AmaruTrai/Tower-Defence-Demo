using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class SpawnWaveInfo
    {
        public float TimeToSpawn;
        public List<SpawnInfo> spawnList;

        public bool SpawnEnded
        {
            get
            {
                return !spawnList.Any((spawnInfo) => spawnInfo.Count > 0);
            }
        }
    }
}
