using Tools.Liquidated;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CharacterController))]
    public class Bullet : LiquidatedObject
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private int damage;

        private CharacterController controller;

        protected override void OnAwake()
        {
            controller = GetComponent<CharacterController>();
            controller.detectCollisions = false;
            base.OnAwake();
        }

        void Update()
        {
            // Move to forward;
            controller.Move(transform.TransformVector(Vector3.forward * Time.deltaTime * speed));
        }

        public void SetTarget(Vector3 target)
        {
            // Rotate to target position
            transform.rotation = Quaternion.LookRotation(target - transform.position);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Ground"))
            {
                Liquidate();
            }

            if (hit.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.ReduceHealthPoints(damage);
                Liquidate();
            }
        }
    }
}
