using UnityEngine;
using System;

namespace Tools.Liquidated
{
    public class LiquidatedObject : MonoBehaviour
    {
        public event Action<LiquidatedObject> OnLiquidation;
        public event Action<LiquidatedObject> OnRestart;

        private void Awake()
        {
            OnAwake();
        }

        public virtual void Liquidate()
        {
            OnLiquidation?.Invoke(this);
        }

        public virtual void Restart()
        {
            OnRestart?.Invoke(this);
        }

        protected virtual void OnAwake()
        {

        }
    }
}

