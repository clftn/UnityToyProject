using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerCheckObjMgr : MonoBehaviour
{
    public List<GameObject> m_TowerList = new List<GameObject>();
    public int _ListCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TOWER")
        {
            m_TowerList.Add(other.gameObject);
            _ListCount = m_TowerList.Count;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TOWER")
        {
            GameObject a_Tower = other.gameObject;
            TowerCtrl_Team m_TowerCtrl_Team = a_Tower.GetComponent<TowerCtrl_Team>();
            int a_TowerNum = m_TowerCtrl_Team.m_TowerNumber;
            for (int i = 0; i < m_TowerList.Count; i++)
            {
                m_TowerCtrl_Team = m_TowerList[i].GetComponent<TowerCtrl_Team>();
                if (a_TowerNum == m_TowerCtrl_Team.m_TowerNumber)
                {
                    m_TowerList.RemoveAt(i);
                    _ListCount = m_TowerList.Count;
                }
            }
        }
    }
}
