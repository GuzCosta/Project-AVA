using UnityEngine;
using AVA.Core;
using System.Collections;
using AVA.Effects;
using System.Collections.Generic;

namespace AVA.Combat
{
    /// <summary>
    /// Base class for projectiles
    /// </summary>
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField]
        protected AudioClip shootSound;

        [SerializeField]
        protected float projectileSpeed = 10f;

        [SerializeField]
        protected float destroyTimer = 5f;

        [SerializeField]
        [Range(0, 100)]
        protected int initialPierce = 0;
        protected int pierceCount = 0;

        protected List<IBaseEffectFactory> _onHitEffects;

        private void OnEnable()
        {
            pierceCount = initialPierce;
        }

        private void Start()
        {
            pierceCount = initialPierce;
        }

        public AttackInstance attackInstance;

        private Coroutine timeoutCoroutine;

        /// <summary>
        /// Shoots the projectile in the given direction
        /// </summary>
        /// <param name="direction">The Vector3 representing the direction to shoot the projectile</param>
        public void ShootProjectile(Vector3 direction, List<IBaseEffectFactory> onHitEffects = null)
        {
            _onHitEffects = onHitEffects;
            OnShootProjectile(direction);
            //TODO: Mejorar esto, maybe usar las clases de timing creadas recientemente
            timeoutCoroutine = StartCoroutine(ProjectileTimeout());
            ResetTrailRenderer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (LayerManager.IsInLayerMask(LayerManager.environmentLayer, other.gameObject.layer))
            {
                ReturnToPool();
                //Debug.Log("Projectile collided with environment");
                return;
            }
            if (LayerManager.IsInLayerMask(LayerManager.pierceLayer, other.gameObject.layer))
            {
                if (pierceCount > 0)
                {
                    //Debug.Log("Projectile pierced through object: " + other.gameObject.name + " Pierce left: " + pierceCount);
                    pierceCount--;
                    OnProjectilePierce(other);
                }
                else
                {
                    //Debug.Log("Projectile ran out of pierce ");
                    ReturnToPool();
                    StopCoroutine(timeoutCoroutine);
                }
            }
            

            if (_onHitEffects != null && other.gameObject.GetComponent<EffectService>() != null) {
                foreach(var effect in _onHitEffects)
                    other.gameObject.GetComponent<EffectService>().AddEffect(effect);
            }
        }

        private void ReturnToPool()
        {
            gameObject.SetActive(false);
            OnDestroyProjectile();
        }

        public void ResetTrailRenderer()
        {
            var particleSystem = GetComponentInChildren<TrailRenderer>();
            if(particleSystem == null) Debug.LogWarning("No particle system found in projectile");
            
            particleSystem?.Clear();
        }

        private IEnumerator ProjectileTimeout()
        {
            yield return new WaitForSeconds(destroyTimer);
            ReturnToPool();
        }


        protected abstract void OnProjectilePierce(Collider other);

        protected abstract void OnDestroyProjectile();

        protected abstract void OnShootProjectile(Vector3 direction);
    }

}