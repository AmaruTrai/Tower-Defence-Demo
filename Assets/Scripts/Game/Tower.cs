using Cysharp.Threading.Tasks;
using System;
using Tools.Liquidated;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Tower : LiquidatedObject
    {
        [SerializeField]
        private Enemy currentTarget;
        [SerializeField]
        private TowerVision vision;
        [SerializeField]
        private float rotationSpeed = 5f;
        [SerializeField]
        private Transform spawnPosition;
        [SerializeField]
        private LiquidatedObject bulletPrefab;

        private UniTask attackTask;
        private LiquidatedObjectPool pool;
        private Animator animator;

        public float AttackRate
        {
            set
            {
                attackRate = value;
            }
        }

        public float AttackRange
        {
            set
            {
                vision.SetAttackRange(value);
            }
        }

        private float attackRate;

        [Inject]
        public void Construct(LiquidatedObjectPool pool)
        {
            this.pool = pool;
        }

        protected override void OnAwake()
        {
            animator = GetComponent<Animator>();
            vision.OnEnemyEnter += OnEnemyEnterVision;
            vision.OnEnemyLeave += OnEnemyLeaveVision;
            base.OnAwake();
        }

        private void Update()
        {
            if (currentTarget != null)
            {
                var direction = currentTarget.transform.position - transform.position;
                direction.y = 0;
                var position = transform.position;
                position.y = 0;
                var rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);

                if (
                    attackTask.Status == UniTaskStatus.Succeeded &&
                    Quaternion.Angle(transform.rotation, rotation) <= 0.1f
                ) {
                    attackTask = AttackCoroutine();
                }

                Debug.DrawRay(transform.position, currentTarget.transform.position - transform.position, Color.red);
            }
        }

        private void OnEnemyEnterVision(Enemy enemy)
        {
            if (currentTarget == null)
            {
                if (vision.TryToGetTarget(out currentTarget))
                {
                    animator.SetBool("HasTarget", true);
                }
            }
        }

        private void OnEnemyLeaveVision(Enemy enemy)
        {
            if (enemy == currentTarget)
            {
                currentTarget = null;
                animator.SetBool("HasTarget", false);
                if (vision.TryToGetTarget(out currentTarget))
                {
                    animator.SetBool("HasTarget", true);
                }
            }
        }

        private async UniTask AttackCoroutine()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(attackRate));
            if (isActiveAndEnabled)
            {
                Attack();
            }
            await UniTask.Yield();
        }

        private void Attack()
        {
            if (currentTarget != null)
            {
                var newObj = pool.Get(bulletPrefab);
                newObj.transform.position = spawnPosition.position;
                newObj.gameObject.SetActive(true);
                var bullet = newObj.GetComponent<Bullet>();
                bullet.SetTarget(currentTarget.transform.position);
            }
        }
    }
}
