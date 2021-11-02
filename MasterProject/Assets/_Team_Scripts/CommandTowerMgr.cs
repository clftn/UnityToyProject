using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandTowerMgr : MonoBehaviour
{
    //-----------커맨드타워의 체력
    float m_CurHP = 0;
    float m_MaxHP = 1000;
    //-----------커맨드타워의 체력

    public Image m_HpImg = null;

    // Start is called before the first frame update
    void Start()
    {
        m_CurHP = m_MaxHP;
        GameMgr.Inst.tower_List.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float _damage)
    {
        m_CurHP -= _damage;
        Debug.Log(m_CurHP);
        m_HpImg.fillAmount = m_CurHP / m_MaxHP;
        GameMgr.Inst.m_VsHpImg.fillAmount = m_HpImg.fillAmount;
        if (m_CurHP <= 0)
        {
            StartEndCtrl.Inst.g_GameState = GameState.GS_GameEnd;
        }
    }
}
