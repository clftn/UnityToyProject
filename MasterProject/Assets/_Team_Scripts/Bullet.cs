using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public TowerType m_BulletType;
    public int m_AttackRate = 0;

    // ------------------ 진진원
    float m_Speed = 30.0f;                         //불렛의 속도
    Vector3 m_TargetPos = Vector3.zero;            //타겟의 위치 계산용
    Vector3 m_MoveDir = Vector3.zero;              //이동 방향 계산용 
    float m_Time = 5.0f;                           //일정시간 지나면 자폭
    [HideInInspector] public GameObject TargetObj; //적을 추적하는 불렛을 만들기 위해

    void Update()
    {
        //프레임당 계속 적의 위치를 추적
        Fire();

        //일정 시간이 지나면 포탄을 파괴
        if (0.0f < m_Time)
        {
            m_Time -= Time.deltaTime;
            if(m_Time < 0.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    //한번 호출해서 가는 방법이 아닌 적을 추적하도록 변경
    public void Fire()
    {
        if (TargetObj == null || TargetObj.activeSelf == false)
        {
            Destroy(this.gameObject); //추적할 적이 없다면 자폭
            return;
        }

        m_TargetPos = TargetObj.transform.position;
        m_MoveDir = m_TargetPos - this.transform.position;
        transform.Translate(m_MoveDir.normalized * m_Speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other) //공격거리안으로 적이 들어왔는지 판단하는 함수
    {
        if (other.tag == "TANK")
        {
            //---- Tower 에 따른 공격 성공 이펙트 분류
            if(m_BulletType == TowerType.MachineGun_Tower || m_BulletType == TowerType.Missile_Tower)//Tower01/Tower02
            {
                EffectPool.Inst.GetEffectObj("Tower01_AttackSuccess_FX", this.gameObject.transform.position, Quaternion.identity);
            }
            else if (m_BulletType == TowerType.Super_MachineGun_Tower)//SuperTower
            {
                EffectPool.Inst.GetEffectObj("SuperTower_AttackSucess_FX", this.gameObject.transform.position, Quaternion.identity);             
            }
            //---- Tower 에 따른 공격 성공 이펙트 분류

            //Debug.Log($"{other.gameObject.name} 에게 피해를 입힘");
            TankCtrl m_MoveTank = other.gameObject.GetComponent<TankCtrl>();
            m_MoveTank.TakeDamage(m_AttackRate);
            Destroy(gameObject);
        }
    }
    // ------------------ 진진원

}
