using PathCreation;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    [RequireComponent(typeof(CharacterController))]
    public class Enemy : DamagebleObject
    {
        public event Action<Enemy> OnEndPointReached;

        [SerializeField]
        private int speed;
        [SerializeField]
        private VertexPath path;

        [SerializeField]
        private PathCreator creator;

        private CharacterController controller;
        private BoxCollider boxCollider;
        private float distanceTravelled;

        public float Range
        {
            get
            {
                return distanceTravelled;
            }
        }

        protected override void OnAwake()
        {
            controller = GetComponent<CharacterController>();
            boxCollider = GetComponent<BoxCollider>();
            if (creator != null)
            {
                path = creator.path;
            }
            base.OnAwake();
        }

        public bool SetPath(VertexPath path)
        {
            if (path == null)
            {
                Debug.Log("Empty path");
                return false;
            }
            this.path = path; 
            return true;
        }

        private void FixedUpdate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (distanceTravelled < path.length)
            {
               // MoveToTarget();
            } else
            {
                OnEndPointReached?.Invoke(this);
                Liquidate();
            }
        }

        public void MoveToTarget()
        {

            if (path != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                var direction = path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
                var angle = direction.eulerAngles;
                direction.Set(transform.rotation.x, direction.y, transform.rotation.z, direction.w);

                transform.rotation = direction;
            }

            controller.Move(transform.TransformDirection( Vector3.forward * Time.deltaTime * speed));
        }

        public override void Liquidate()
        {
            distanceTravelled = 0;
            base.Liquidate();
        }

        public override void Die()
        {
            boxCollider.enabled = false;
            base.Die();
        }

        public override void Restart()
        {
            boxCollider.enabled = true;
            base.Restart();
        }

        public void Dead()
        {
            CustomEvent.Trigger(gameObject, "Dead");
        }
    }
}
