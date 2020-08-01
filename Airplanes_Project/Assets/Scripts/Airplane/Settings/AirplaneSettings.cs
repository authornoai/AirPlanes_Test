using UnityEngine;

namespace AirP.Planes.Settings
{
    [CreateAssetMenu(fileName = "Settings_Plane_", menuName = "AirP/Create Airplane Settings", order = 0)]
    public class AirplaneSettings : ScriptableObject
    {
        #region Variables

        [Header("Base Physics")]
        [SerializeField] private float m_mass = 2.5f;
        [SerializeField] private float m_Lift = 10f;
        [SerializeField] private float m_thrustSpeed = 50;
        [SerializeField] private float m_dragCoefficient = 0.025f;
        
        [Header("Rotation Step")]
        [SerializeField] private float m_pitchStep = 30;
        [SerializeField] private float m_rollStep = 30;
        [SerializeField, Range(0, 1)] private float m_yawRollRatio = 0.05f;
        [SerializeField, HideInInspector] private float m_yawStep;
        
        [Header("Rotation Limit")]
        [SerializeField] private float m_rollLimit = 75;
        [SerializeField] private float m_yawLimit = 30;
        
        [Header("Boosting")]
        [SerializeField] private float m_boostMultiplierDefault = 1;
        [SerializeField] private float m_boostMultiplierMax = 3;
        [SerializeField] private AnimationCurve m_boostCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Death")]
        [SerializeField] private float m_deathTime = 3;

        #endregion

        #region Properties

        public float Mass => m_mass;
        public float ThrustSpeed => m_thrustSpeed;
        public float Lift => m_Lift;
        public float Drag => m_dragCoefficient;

        public float PitchStep => m_pitchStep;
        public float RollStep => m_rollStep;
        public float YawStep => m_yawStep;
        
        public float RollLimit => m_rollLimit;
        public float YawLimit => m_yawLimit;

        public float BoostMultiplierDefault => m_boostMultiplierDefault;
        public float BoostMultiplierMax => m_boostMultiplierMax;
        public AnimationCurve BoostCurve => m_boostCurve;

        public float DeathTime => m_deathTime;

        #endregion

        private void OnValidate()
        {
            m_yawStep = m_rollStep * m_yawRollRatio;
        }
    }
}