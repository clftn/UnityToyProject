using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EnemyType
{
    Enemy,
    EnemyBase,
}

public class EnemyCtrl : MonoBehaviour
{
    public Image hp_Img = null;
    float max_Hp = 100.0f;
    float now_Hp = 0.0f;

    public EnemyType m_EnemyType = EnemyType.Enemy;
    void Start()
    {
        if (m_EnemyType == EnemyType.EnemyBase)
            max_Hp = 10000.0f;

        now_Hp = max_Hp;

        GameMgr.Inst.tower_List.Add(this.gameObject);
    }

    void Update()
    {
        
    }

    public void Damage(float a_Damage)
    {
        now_Hp -= a_Damage;
        hp_Img.fillAmount = now_Hp / max_Hp;

        if (now_Hp <= 0.0f)
        {
            if (m_EnemyType == EnemyType.EnemyBase)
                StartEndCtrl.Inst.g_GameState = GameState.GS_GameEnd;

            Destroy(this.gameObject);
        }
    }
}
