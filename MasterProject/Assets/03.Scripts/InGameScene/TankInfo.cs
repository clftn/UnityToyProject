using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankType
{
    Normal, // 일반차량
    Speed,  // 가볍고 빠른차량
    Repair, // 수리차량
    Solid,  // 튼튼한차량
    Cannon, // 장거리 미사일
    Count,
}

public class TankInfo
{
    public TankType m_Type;
    
    public int   level = 1;      // 레벨
    public int   itemUsable = 10; // 유닛 사용 제한 수량
    public float maxHp = 20;      // 최대체력
    public float atk = 10;        // 공격력
    public float def = 10;        // 방어력
    public float speed = 10;      // 이동속도
    public float attRate = 3;    // 공격속도
    public float skillCool = 5.0f;  // 스킬 쿨타임
    public float attRange = 20;   // 공격거리

    public void TankInit() // 탱크의 기본정보 세팅
    {
        List<AttUnit> m_UserTankList = GameMgr.m_UserTankList;

        for(int i =0; i< m_UserTankList.Count; i++)
        {
            AttUnit a_Node = m_UserTankList[i];

            if (m_Type == (TankType)a_Node.m_unitkind)
            {
                level       = a_Node.m_Level;
                itemUsable  = a_Node.ItemUsable;
                maxHp       = a_Node.m_Hp;
                atk         = a_Node.m_Att;
                def         = a_Node.m_Def;
                speed       = a_Node.m_Speed;
                attRate     = a_Node.m_AttSpeed;
                skillCool   = a_Node.m_SkillTime;
                attRange    = a_Node.m_Range;
            }
        }
    }
}
