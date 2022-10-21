using System;
using Tools;
using Tools.Liquidated;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class DamagebleObject : LiquidatedObject
    {
        public event Action<DamagebleObject> OnDead;

        [SerializeField]
        private int maxHealthPoints;

        [ReadOnly]
        [SerializeField]
        private int currentHealthPoints;

        private bool IsDead;


        protected override void OnAwake()
        {
            currentHealthPoints = maxHealthPoints;
            IsDead = false;
            base.OnAwake();
        }

        public void IncreaseHealthPoints(int hp)
        {
            currentHealthPoints = Math.Min(currentHealthPoints + hp, maxHealthPoints);
        }

        public void ReduceHealthPoints(int hp)
        {
            currentHealthPoints -= hp;
            if (!IsDead && currentHealthPoints <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            IsDead = true;
            OnDead?.Invoke(this);
            CustomEvent.Trigger(gameObject, "Die");
        }

        public override void Liquidate()
        {
            currentHealthPoints = maxHealthPoints;
            IsDead = false;
            base.Liquidate();
        }
    }
}
