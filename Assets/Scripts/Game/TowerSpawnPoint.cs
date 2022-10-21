using System;
using Tools.Liquidated;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(BoxCollider))]
    public class TowerSpawnPoint : MonoBehaviour, IPointerClickHandler
    {
        public event Action<TowerSpawnPoint, PointerEventData> OnPointClicked;

        [SerializeField]
        private Transform towerPosition;
        [SerializeField]
        private Tower tower;
        [SerializeField]
        private TowerInfo towerInfo;

        private LiquidatedObjectPool pool;

        public TowerInfo TowerInfo
        {
            get
            {
                return towerInfo;
            }
        }



        public bool HasTower => towerInfo != null;

        private void Awake()
        {
            if (towerInfo != null && towerInfo.Prefab != null)
            {
                SetTower(towerInfo);
            }
        }

        [Inject]
        public void Construct(LiquidatedObjectPool pool)
        {
            this.pool = pool;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointClicked?.Invoke(this, eventData);
        }

        public void SetTower(TowerInfo info)
        {
            if (tower != null)
            {
                tower.Liquidate();
            }

            var newObj = pool.Get(info.Prefab);
            if (newObj is Tower newTower)
            {
                tower = newTower;
                towerInfo = info;
                tower.gameObject.SetActive(true);
                tower.AttackRange = info.AttackRange;
                tower.AttackRate = info.AttackRate;
                tower.transform.position = towerPosition.position;
                tower.transform.rotation = towerPosition.rotation;
            }
        }
    }
}


