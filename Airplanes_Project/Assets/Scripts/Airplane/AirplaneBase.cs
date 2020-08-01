using AirP.Control;
using AirP.PhysicsForms;
using AirP.Planes.Settings;
using AirP.Weapon;
using UnityEngine;

namespace AirP.Planes
{
    [RequireComponent(typeof(PhysicsBody))]
    public class AirplaneBase : MonoBehaviour, IAttackable, IControllableMain
    {
        #region Consts

        private const int c_PitchToFailing = 1;
        private const int c_AngleCircle_360 = 360;
        private const int c_AngleCircle_180 = 180;
        private const int c_AngleCircle_5 = 5;

        #endregion

        #region Variables

        [SerializeField] private AirplaneSettings _settings;
        [SerializeField] private GameObject _enableOnDeath;

        private PhysicsBody m_physicsBody;
        private float m_pitch;
        private float m_roll;

        private bool m_isBurst;
        private float m_burst;

        private bool m_isHit = false;
        private float m_deathTimeCurrent;

        public int Index { get; set; } = -1;

        public IController Controller { get; private set; }
        public void SetController(IController control) => Controller = control;
        public Transform Transform { get; private set; }

        private IControllable[] m_controllableParts;

        #endregion

        #region Built-In Methods

        private void Awake()
        {
            m_physicsBody = GetComponent<PhysicsBody>();
            Transform = GetComponent<Transform>();

            m_physicsBody.SetMass(_settings.Mass);

            m_deathTimeCurrent = _settings.DeathTime;
        }

        private void Start()
        {
            // Get other controllers after index has been set (in Awake of IController)
            m_controllableParts = GetComponents<IControllable>();
            for (int i = 0; i < m_controllableParts.Length; i++)
            {
                m_controllableParts[i].Index = Index;
                m_controllableParts[i].SetController(Controller);
            }
        }

        private void Update()
        {
            if (!m_isHit)
            {
                HandleInput();
            }
            else
            {
                HandleDeath();
            }
        }
        private void FixedUpdate()
        {
            FillForces();
            FillRotations();

            m_physicsBody.MakeUpdate();
        }

        #endregion

        #region IAttackable Methods

        public void GetHit()
        {
            m_isHit = true;
        }

        private void HandleDeath()
        {
            m_pitch = c_PitchToFailing;

            _enableOnDeath.SetActive(true);

            m_deathTimeCurrent -= Time.deltaTime;
            if (m_deathTimeCurrent < 0) gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        private void HandleInput()
        {
            if (Controller == null)
            {
#if UNITY_EDITOR
                Debug.LogError(gameObject.name + ": AirplaneBase has no IController");
#endif

                return;
            }

            Vector2 movementAxises = Controller.GetMovementAxises(Index);
            m_pitch = movementAxises.y;
            m_roll = movementAxises.x;

            m_isBurst = Controller.GetBoostDesire(Index);
            m_burst = Mathf.Clamp01(m_isBurst ? m_burst + Time.deltaTime : m_burst - Time.deltaTime);
        }

        private void FillForces()
        {
            Vector3 forward = Transform.forward;

            //THRUST
            float burstMultiplier = Mathf.Lerp(_settings.BoostMultiplierDefault, _settings.BoostMultiplierMax,
                _settings.BoostCurve.Evaluate(m_burst));
            Vector3 thrust = _settings.ThrustSpeed * burstMultiplier * forward;

            //DRAG
            Vector3 drag = -thrust * _settings.Drag;

            //LIFT
            float angleOfAttack = Vector3.Dot(Vector3.forward, forward);
            angleOfAttack *= angleOfAttack;
            Vector3 lift = Transform.up * (_settings.Lift * angleOfAttack);

            //APPLY
            m_physicsBody.AddForce(thrust);
            m_physicsBody.AddForce(drag);
            m_physicsBody.AddForce(lift);
        }
        private void FillRotations()
        {
            Vector3 angles = Transform.rotation.eulerAngles;

            //PITCH
            float pitchAngle = _settings.PitchStep * m_pitch * Time.fixedDeltaTime;

            //ROLL
            float rollAngle = _settings.RollStep * -m_roll * Time.fixedDeltaTime;
            rollAngle = angles.z + rollAngle < _settings.RollLimit || angles.z + rollAngle > c_AngleCircle_360 - _settings.RollLimit ? rollAngle : 0;

            //YAW (Based on current roll)
            float rollAttitude = 0;
            if (angles.z > c_AngleCircle_180 && angles.z < c_AngleCircle_360 - c_AngleCircle_5)
            {
                rollAttitude = c_AngleCircle_180 - angles.z % c_AngleCircle_180;
            }
            else if (angles.z < c_AngleCircle_180 && angles.z > c_AngleCircle_5)
            {
                rollAttitude = -1 * angles.z;
            }

            float yawAngle = _settings.YawStep * Mathf.Clamp(rollAttitude, -_settings.YawLimit, _settings.YawLimit) * Time.fixedDeltaTime;

            //APPLY
            m_physicsBody.AddRotation(yawAngle, Vector3.up);
            m_physicsBody.AddRotation(pitchAngle, Transform.right);
            m_physicsBody.AddRotation(rollAngle, Transform.forward);
        }

        #endregion

    }
}