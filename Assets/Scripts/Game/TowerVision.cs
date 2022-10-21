using System;
using System.Collections.Generic;
using Tools.Liquidated;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class TowerVision : MonoBehaviour
    {
        public event Action<Enemy> OnEnemyEnter;
        public event Action<Enemy> OnEnemyLeave;

        private CapsuleCollider Area
        {
            get
            {
                if (area == null)
                {
                    area = GetComponent<CapsuleCollider>();
                    area.isTrigger = true;
                }
                return area;
            }
        }

        private HashSet<Enemy> targets;
        private CapsuleCollider area;

        private void Awake()
        {
            targets = new HashSet<Enemy>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                targets.Add(enemy);
                enemy.OnLiquidation += TargetLiquidate;
                enemy.OnDead += TargetLiquidate;
                OnEnemyEnter?.Invoke(enemy);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                targets.Remove(enemy);
                enemy.OnLiquidation -= TargetLiquidate;
                enemy.OnDead -= TargetLiquidate;
                OnEnemyLeave?.Invoke(enemy);
            }
        }

        private void TargetLiquidate(LiquidatedObject obj)
        {
            var enemy = obj as Enemy;
            if (enemy)
            {
                targets.Remove(enemy);
                enemy.OnLiquidation -= TargetLiquidate;
                enemy.OnDead -= TargetLiquidate;
                OnEnemyLeave?.Invoke(enemy);
            }
        }

        public bool TryToGetTarget(out Enemy target)
        {
            target = null;
            if (targets.Count == 0)
            {
                return false;
            }

            foreach(var enemy in targets)
            {
                if (target == null || enemy.Range > target.Range)
                {
                    target = enemy;
                }
            }

            return target != null;
        }

        public void SetAttackRange(float attackRange)
        {
            Area.radius = attackRange;
        }
    }
}
