using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace DefaultNamespace
{
    public class SeekingProjectile : MonoBehaviour
    {
        public Transform target;

        public int damageAmount = 5;
        public float findTargetRange = 10f;
        private Rigidbody2D _rigidbody2D;
        [SerializeField] private float travelSpeed = 1;
        [SerializeField] LayerMask layerMask;
        [SerializeField] private ParticleSystem particleSystem;
        public float lifeTime = 5f;
        private Vector2 direction;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            Collider2D targetCollider = Physics2D.OverlapCircle(transform.position, findTargetRange, layerMask);
            if (targetCollider != null)
            {
                target = targetCollider.transform;
            }

            StartCoroutine(ProjectileTween());
        }

        IEnumerator ProjectileTween()
        {
            float timer = 0f;

            while (timer <= lifeTime)
            {
                timer += Time.deltaTime;

                yield return null;
            }

            particleSystem.transform.SetParent(null);

            Destroy(this.gameObject);
        }

        private void FixedUpdate()
        {
            if (target)
                direction = target.position - transform.position;

            _rigidbody2D.velocity = direction.normalized * travelSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                other.SendMessage("Damage", damageAmount);
                
                particleSystem.transform.SetParent(null);
                Destroy(this.gameObject);
            } else if (other.gameObject.CompareTag("Enemy"))
            {
                particleSystem.transform.SetParent(null);
                Destroy(this.gameObject);
            }

        }
    }
}