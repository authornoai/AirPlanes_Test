using UnityEngine;
using System.Collections;
using AirP.UI;

namespace AirP.Control
{
    public class AIController : MonoBehaviour, IController
    {
        private const float c_SpawnRange_Min = 0.25f;
        private const float c_SpawnRange_Max = 1;

        private const int c_MoveRest = 0;
        private const int c_MoveUp = -1;
        private const int c_MoveDown = 1;
        private const int c_MoveLeft = -1;
        private const int c_MoveRight = 1;

        private const int c_AngleCircle_360 = 360;
        private const int c_AngleCircle_0 = 0;

        #region Variables

        [Header("Base")]
        [SerializeField] private Transform m_targetToDestroy;
        [SerializeField] private float m_distanceClose = 30;
        [SerializeField] private float m_distanceLong = 90;
        [SerializeField] private float m_attackDistance = 45;
        [SerializeField] private int m_attackingMax = 3;
        [SerializeField] private float m_reachDistanceThreshold = 2;

        [Header("Angles")]
        [SerializeField, Range(0,1)] private float m_differenceInAngleToShoot = 0.15f;
        [SerializeField, Range(0, 1)] private float m_differenceInAngleToMove = 0.1f;
        [SerializeField] private float m_limitInPitch = 70;

        public static ObjectPoolerUIPosition PoolUIPositions;

        private Vector3 m_targetPosition;
        private int m_attackingCurrent = 0;

        private IControllableMain[] m_mobs;
        private AIStatus[] m_mobStatuses;
        private Vector3[] m_mobDesiredPositions;

        #endregion

        #region Built-In Methods

        private void Awake()
        {
            m_mobs = GetComponentsInChildren<IControllableMain>();

            for (int i = 0; i < m_mobs.Length; i++)
            {
                m_mobs[i].SetController(this);
                m_mobs[i].Index = i;

                float minus = Random.Range(0, 1) == 1 ? -1 : 1;
                Vector3 startPos = m_targetToDestroy.transform.position + 
                    (new Vector3(Random.Range(c_SpawnRange_Min, c_SpawnRange_Max), Random.Range(c_SpawnRange_Min, c_SpawnRange_Max), Random.Range(c_SpawnRange_Min, c_SpawnRange_Max)) * m_distanceLong * minus);
                m_mobs[i].Transform.position = startPos;
                m_mobs[i].Transform.LookAt(m_targetToDestroy);
            }

            m_mobStatuses = new AIStatus[m_mobs.Length];
            m_mobDesiredPositions = new Vector3[m_mobs.Length];
        }
        private void Start()
        {
            for (int i = 0; i < m_mobs.Length; i++)
            {
                var obj = PoolUIPositions.GetPooledObject(out int index);
                obj.SetTarget(m_mobs[i].Transform);
                obj.SetFromPosition(m_targetToDestroy);
                obj.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            m_targetPosition = m_targetToDestroy.position;

            HandleActions();
        }

        #endregion

        #region Public Methods

        public bool GetBoostDesire(int index)
        {
            return false;
        }

        public bool GetFireDesire(int index)
        {
            Transform mobTransform = m_mobs[index].Transform;

            Vector3 mobPos = mobTransform.position;
            Vector3 mobForward = mobTransform.forward;
            Vector3 dirToTarget = (mobPos - m_targetPosition).normalized;
            float dotForward = Vector3.Dot(mobForward, dirToTarget);

            return
            (m_targetPosition - m_mobs[index].Transform.position).sqrMagnitude < m_attackDistance * m_attackDistance //In attack distance
            && dotForward < m_differenceInAngleToShoot && dotForward > -m_differenceInAngleToShoot;
        }

        public Vector2 GetMovementAxises(int index)
        {
            var mobTransform = m_mobs[index].Transform;
            var mobAngles = mobTransform.eulerAngles;

            Vector3 mobPos = mobTransform.position;
            Vector3 targetPos = m_mobDesiredPositions[index];

            Vector3 mobForward = mobTransform.forward;
            Vector3 mobRight = mobTransform.right;
            Vector3 dirToTarget = (mobPos - targetPos).normalized;


            //If position is above target -> go down
            float targY = mobAngles.x < m_limitInPitch && mobAngles.x > c_AngleCircle_0 ? c_MoveDown : mobAngles.x > c_AngleCircle_360 - m_limitInPitch && mobAngles.x < c_AngleCircle_360 ? c_MoveUp : c_MoveRest;
            float dotY = Vector3.Dot(mobForward, dirToTarget);
            if (dotY > m_differenceInAngleToMove)
            {
                targY = c_MoveUp;
            }
            else if (dotY < -m_differenceInAngleToMove)
            {
                targY = c_MoveDown;
            }

            float targX = c_MoveRest;
            float dotX = Vector3.Dot(mobRight, dirToTarget);
            if (dotX > m_differenceInAngleToMove)
            {
                targX = c_MoveLeft;
            }
            else if (dotX < -m_differenceInAngleToMove)
            {
                targX = c_MoveRight;
            }

            return new Vector2
            {
                x = targX,
                y = targY
            };

        }

        #endregion

        #region Private Methods

        private void HandleActions()
        {
            for (int i = 0; i < m_mobs.Length; i++)
            {
                AIStatus statusRead = m_mobStatuses[i];

                if (statusRead == AIStatus.Died) continue;

                switch(statusRead)
                {
                    case AIStatus.Free:
                        HandleStatus_Free(i);
                        break;
                    case AIStatus.Attacking:
                        HandleStatus_Attacking(i);
                        break;
                    case AIStatus.Moving:
                        HandleStatus_Moving(i);
                        break;
                }
            }
        }

        private void HandleStatus_Free(int i)
        {
            if (m_attackingCurrent < m_attackingMax)
            {
                m_mobStatuses[i] = AIStatus.Attacking;
                ++m_attackingCurrent;
            } else
            {
                m_mobStatuses[i] = AIStatus.Moving;

                Vector3 movePositionRelative = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized * m_distanceLong;
                m_mobDesiredPositions[i] = m_targetPosition + movePositionRelative;
            }
        }

        private void HandleStatus_Attacking(int i)
        {
            Vector3 attackPositionRelative = (m_mobs[i].Transform.position - m_targetPosition).normalized * m_distanceClose;
            m_mobDesiredPositions[i] = m_targetPosition + attackPositionRelative;

            if((m_mobDesiredPositions[i] - m_mobs[i].Transform.position).sqrMagnitude < m_reachDistanceThreshold * m_reachDistanceThreshold)
            {
                m_mobStatuses[i] = AIStatus.Free;
                --m_attackingCurrent;
            }
        }

        private void HandleStatus_Moving(int i)
        {
            if ((m_mobDesiredPositions[i] - m_mobs[i].Transform.position).sqrMagnitude < m_reachDistanceThreshold * m_reachDistanceThreshold)
            {
                m_mobStatuses[i] = AIStatus.Free;
            }
        }

#endregion
    }
}
