using System;
using AirP.Control;
using UnityEngine;

namespace AirP.Weapon
{
    public class WeaponBase : MonoBehaviour, IControllable
    {
        #region Variables

        [SerializeField] private Transform m_spawnPoint;
        
        [SerializeField] private int m_bulletsBeforeReload = 10;
        [SerializeField, Tooltip("Bullets Per Minute")] private int m_fireRate = 200;
        [SerializeField] private float m_reloadTime = 5f;
        
        private Transform m_owner;

        private int m_bulletsCurrent;
        private float m_bulletCooldownCurrent;
        private float m_bulletCooldown;
        
        private float m_reloadTimeCurrent;

        public static ObjectPoolerProjectile Bullets;

        public int Index { get; set; } = -1;

        public IController Controller { get; private set; }
        public void SetController(IController control) => Controller = control;

        #endregion

        #region Built-In Methods

        private void Awake()
        {
            m_owner = GetComponent<Transform>();

            m_bulletsCurrent = m_bulletsBeforeReload;
            m_bulletCooldown = 60.0f / m_fireRate;
        }

        private void Update()
        {
            if (Controller != null && Controller.GetFireDesire(Index)
            && m_bulletsCurrent > 0)
            {
                Shoot();
            }
            else if(m_bulletsCurrent == 0)
            {
                Reload();
            }
        }

        #endregion

        #region Private Methods

        private void Shoot()
        {
            m_bulletCooldownCurrent -= Time.deltaTime;
            
            if (m_bulletCooldownCurrent < 0)
            {
                m_bulletCooldownCurrent = m_bulletCooldown;
                --m_bulletsCurrent;
                LaunchBullet();
            }
            
            if (m_bulletsCurrent == 0) m_reloadTimeCurrent = m_reloadTime;
        }

        private void LaunchBullet()
        {
            Projectile bullet = Bullets.GetPooledObject(out int index);
            bullet.Launch(m_spawnPoint, m_owner.forward, m_owner.rotation, index);
        }

        private void Reload()
        {
            m_reloadTimeCurrent -= Time.deltaTime;

            if (m_reloadTimeCurrent < 0)
            {
                m_bulletsCurrent = m_bulletsBeforeReload;
            }
        }

        #endregion
    }
}


