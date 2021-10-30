using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMoveObjMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag == "TANK")
    //    {
    //        MoveTank a_MoveTank = other.gameObject.GetComponent<MoveTank>();
    //        //TowerCheckObjMgr _TowerCheckObjMgr = a_MoveTank.m_TowerCheckObj.GetComponent<TowerCheckObjMgr>();
    //        //int m_TowerListCount = _TowerCheckObjMgr._ListCount;

    //        Debug.Log(m_TowerListCount.ToString());

    //        if (m_TowerListCount == 0)
    //        {
    //            Destroy(other.gameObject);
    //            return;
    //        }

    //        for (int i = 0; i < m_TowerListCount; i++)
    //        {
    //            //GameObject _tower = _TowerCheckObjMgr.m_TowerList[i];
    //            TowerCtrl_Team _TowerCtrl_Team = _tower.GetComponent<TowerCtrl_Team>();
    //            for(int ii = 0; ii < _TowerCtrl_Team.m_TargetList.Count; ii++)
    //            {
    //                MoveTank _MoveTank = _TowerCtrl_Team.m_TargetList[ii].GetComponent<MoveTank>();
    //                if(_MoveTank.m_TankNumber == a_MoveTank.m_TankNumber)
    //                {
    //                    _TowerCtrl_Team.m_TargetList.RemoveAt(ii);
    //                    if (_TowerCtrl_Team.m_TargetList.Count != 0)
    //                    {
    //                        _TowerCtrl_Team.m_TargetObj = _TowerCtrl_Team.m_TargetList[0].gameObject;
    //                        _TowerCtrl_Team.m_TowerState = TowerState.Attack;
    //                    }
    //                    else //리스트 삭제 후 List가 존재하지않는다면.
    //                    {
    //                        _TowerCtrl_Team.m_TowerState = TowerState.Tracking;
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //        Destroy(other.gameObject);
    //    }
    //}
}
