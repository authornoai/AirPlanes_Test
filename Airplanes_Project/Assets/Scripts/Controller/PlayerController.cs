using UnityEngine;

namespace AirP.Control
{
    public class PlayerController : MonoBehaviour, IController
    {
        private const int c_PlayerDoesntNeedIndex = -1;

        private Vector2 m_lastDir;
        private bool m_lastBurst;
        private bool m_lastFire;

        private void Awake()
        {
            GetComponentInChildren<IControllableMain>().SetController(this);
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            m_lastDir.y = Input.GetAxis("Vertical");    
            m_lastDir.x = Input.GetAxis("Horizontal");
            m_lastBurst = Input.GetKey(KeyCode.LeftShift);
            m_lastFire = Input.GetKey(KeyCode.LeftControl);
        }

        public Vector2 GetMovementAxises(int index = c_PlayerDoesntNeedIndex) => m_lastDir;
        public bool GetFireDesire(int index = c_PlayerDoesntNeedIndex) => m_lastFire;
        public bool GetBoostDesire(int index = c_PlayerDoesntNeedIndex) => m_lastBurst;
    }
}

