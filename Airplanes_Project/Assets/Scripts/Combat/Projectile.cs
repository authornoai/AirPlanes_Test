using AirP.PhysicsForms;
using UnityEngine;

namespace AirP.Weapon
{
    [RequireComponent(typeof(PhysicsBody))]
    public class Projectile : MonoBehaviour
    {
        private const float c_Gravity = 9.8f;
        #region Variables

        [SerializeField] private float m_bulletStartSpeed = 100;
        [SerializeField] private float m_bulletDragSpeed = 10;

        private Vector3 m_lastPosition;
        private Vector3 m_dirMove;
        private float m_currentSpeed;
        private PhysicsBody m_physicsBody;

        private TrailRenderer m_trail;
      
        private int m_poolIndex;
        public static ObjectPooler<Projectile> ToReturn;

        #endregion

        #region Built-in Methods

        private void Awake()
        {
            m_physicsBody = GetComponent<PhysicsBody>();
            m_trail = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            if ( m_currentSpeed < 0) EndOfLive();
        }

        private void FixedUpdate()
        {
            if (gameObject.activeInHierarchy)
            {
                m_lastPosition = transform.position;
                m_currentSpeed -= m_bulletDragSpeed * Time.fixedDeltaTime;

                m_physicsBody.AddForce(m_dirMove * m_currentSpeed);
                m_physicsBody.AddForce(Vector3.up * c_Gravity * m_currentSpeed / m_bulletStartSpeed);

                m_physicsBody.MakeUpdate();
            }
        }

        private void LateUpdate()
        {
            if(gameObject.activeInHierarchy)
            {
                if(Physics.Linecast(m_lastPosition, transform.position, out RaycastHit hit, Physics.AllLayers))
                {
                    OnHit(hit.collider);
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual void Launch(Transform owner, Vector3 dir, Quaternion look, int index)
        {
            m_dirMove = dir;
            transform.position = owner.position;
            m_lastPosition = owner.position;
            transform.rotation = look;
            m_currentSpeed = m_bulletStartSpeed;

            m_poolIndex = index;

            gameObject.SetActive(true);
        }

        #endregion

        #region Private Methods

        private void EndOfLive()
        {
            gameObject.SetActive(false);
            m_trail.Clear();
            if (m_poolIndex >= 0)
            {
                ToReturn.ReturnObjectToPool(m_poolIndex);
            }
        }

        private void OnHit(Collider collision)
        {
            EndOfLive();
            collision.gameObject.GetComponentInParent<IAttackable>()?.GetHit();
        }

        #endregion
    }
}