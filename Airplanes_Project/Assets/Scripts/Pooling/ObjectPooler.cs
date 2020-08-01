using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirP
{
    public class ObjectPooler<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject[] m_toPool = null;
        [SerializeField] private int m_amountToPoolAtStart = 1;
        [SerializeField] private bool m_shouldExpandOnLimit = true;
        private List<T> m_pooledObjects;
        private List<bool> m_pooledStatus;

        #endregion

        #region Built-in Methods

        private void Awake()
        {
            m_pooledObjects = new List<T>(m_amountToPoolAtStart);
            m_pooledStatus = new List<bool>(m_amountToPoolAtStart);

            for (int i = 0; i < m_amountToPoolAtStart; i++)
            {
                GameObject obj = Instantiate(m_toPool[UnityEngine.Random.Range(0, m_toPool.Length)], transform);
                obj.SetActive(false);
                m_pooledObjects.Add(obj.GetComponent<T>());
                m_pooledStatus.Add(false);
            }
        }

        #endregion

        #region Public Methods

        public T GetPooledObject(out int index)
        {
            index = -1;
            for (int i = 0; i < m_pooledObjects.Count; i++)
            {
                if (m_pooledStatus[i] == false)
                {

                    index = i;
                    m_pooledStatus[i] = true;
                    return m_pooledObjects[i];
                }
            }

            //Not found
            if (m_shouldExpandOnLimit)
            {
                GameObject obj = Instantiate(m_toPool[UnityEngine.Random.Range(0, m_toPool.Length)], transform);

                obj.SetActive(false);

                index = m_pooledObjects.Count;
                m_pooledObjects.Add(obj.GetComponent<T>());
                m_pooledStatus.Add(true);

                return obj.GetComponent<T>();
            }

            return null;
        }

        public void ReturnObjectToPool(int index, bool deactivate = true)
        {
            if (deactivate) m_pooledObjects[index].gameObject.SetActive(false);
            m_pooledObjects[index].transform.SetParent(transform);
            m_pooledStatus[index] = false;

        }

        #endregion
    }
}




