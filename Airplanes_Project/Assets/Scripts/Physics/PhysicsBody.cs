using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace AirP.PhysicsForms
{
    public class PhysicsBody : MonoBehaviour
    {
        #region Variables

        private Transform m_transform;
        [SerializeField] private float m_mass;

        private static readonly Vector3 m_gravityVector = Vector3.up * -9.8f;

        [SerializeField] private bool m_useGravity = true;
        [SerializeField] private int m_forcesToApply = 3;
        private List<Vector3> m_forceList;
        private Vector3 m_translate;

        [SerializeField] private int m_rotationsToApply = 3;
        private List<Quaternion> m_rotationList;
        private Quaternion m_rotation;

        #endregion

        #region Built-In Methods

        private void Awake()
        {
            m_transform = GetComponent<Transform>();
            m_forceList = new List<Vector3>(m_forcesToApply);
            m_rotationList = new List<Quaternion>(m_rotationsToApply);

            m_mass = m_mass > 0 ? m_mass : 1;
        }

        #endregion

        #region Public Methods

        public void MakeUpdate()
        {
            if (m_useGravity) ApplyGravityToVelocity();
            ApplyForcesToTranslate();
            ApplyRotation();

            UpdatePositionAndRotation();
            m_translate = Vector3.zero;
            m_rotation = Quaternion.identity;
        }
        
        public void AddForce(Vector3 force) => m_forceList.Add(force);
        public void AddRotation(Quaternion rot) => m_rotationList.Add(rot);
        public void AddRotation(float angle, Vector3 axis) => m_rotationList.Add(Quaternion.AngleAxis(angle, axis));

        public void SetMass(float value) => m_mass = value > 0 ? value : m_mass; 
        
        #endregion

        #region Private Methods

        private void ApplyGravityToVelocity()
        {
            m_translate += m_gravityVector;
        }
        private void ApplyForcesToTranslate()
        {
            for (int i = 0; i < m_forceList.Count; i++)
            {
                m_translate += m_forceList[i];
            }

            m_forceList.Clear();
        }

        private void ApplyRotation()
        {
            for (int i = 0; i < m_rotationList.Count; i++)
            {
                m_rotation *= m_rotationList[i];
            }
            
            m_rotationList.Clear();
        }

        private void UpdatePositionAndRotation()
        {
            m_transform.SetPositionAndRotation(m_transform.position + m_translate * Time.fixedDeltaTime / m_mass,
                m_rotation * m_transform.rotation);
        }

        #endregion
        
    }
}

