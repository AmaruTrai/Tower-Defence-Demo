using System.Collections.Generic;
using Tools.Liquidated;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New TowerInfo", menuName = "Tower Info", order = 51)]
    public class TowerInfo : ScriptableObject
    {
        [SerializeField]
        private float attackRange;
        [SerializeField]
        private float attackRate;

        [SerializeField]
        private int cost;
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private List<TowerInfo> nextTiers;
        [SerializeField]
        private LiquidatedObject prefab;

        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public int Cost => cost;
        public Sprite Icon => icon;
        public List<TowerInfo> NextTiers => nextTiers;
        public LiquidatedObject Prefab => prefab;
    }

}
