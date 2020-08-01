using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AirP.UI
{
    public class ViewEnemyPosition : MonoBehaviour
    {
        #region Variables

        private Transform m_from;
        private Transform m_followTarget;

        private Camera m_checkCamera;
        private Text m_text;
        private Image m_image;

        #endregion

        #region Public Methods

        public void SetTarget(Transform target) => m_followTarget = target;
        public void SetFromPosition(Transform from) => m_from = from;

        #endregion

        #region Built-In Methods

        private void Awake()
        {
            m_text = GetComponent<Text>();
            m_image = GetComponentInChildren<Image>();

            m_checkCamera = Camera.main;
        }

        private void Update()
        {
            if (m_followTarget.gameObject.activeInHierarchy)
            {

                float minX = m_text.flexibleWidth + m_text.fontSize;
                float maxX = Screen.width - minX;

                float minY = m_text.flexibleHeight + m_text.fontSize;
                float maxY = Screen.height - minY;

                Vector2 pos = m_checkCamera.WorldToScreenPoint(m_followTarget.position);

                Vector3 dirTo = (m_followTarget.position - m_from.position).normalized;

                float dotOffscreen = Vector3.Dot(dirTo, m_from.forward);
                bool isAboveYou = m_from.transform.position.y > m_followTarget.transform.position.y ? false : true;
                if (dotOffscreen < 0.5f)
                {
                    if (isAboveYou)
                    {
                        pos.y = maxY;
                    }
                    else
                    {
                        pos.y = minY;
                    }

                    if (m_image)
                    {
                        m_image.enabled = true;
                    }

                }
                else if (m_image)
                {
                    m_image.enabled = false;
                }

                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                m_text.transform.position = pos;
                m_text.text = ((int)Vector3.Distance(m_followTarget.position, m_from.position)).ToString() + "m";

            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}