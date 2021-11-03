using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TowerState
{
    Tracking,
    Attack,
    Die,
    Count
}

public class TowerCtrl_Team : MonoBehaviour
{
    [HideInInspector] public TowerState m_TowerState = TowerState.Tracking;

    //-----터렛 구현을 위한 변수
    [Header("========= Turret =========")]
    public GameObject m_Turret = null;
    public GameObject m_BulletSpwanPoint = null;

    //---타겟과의 거리 계산용

    public List<GameObject> m_TargetList = new List<GameObject>();
    Vector3 m_MoveDir = Vector3.zero;
    Vector3 m_CacVLen = Vector3.zero;

    public GameObject m_TargetObj = null;      //타겟 오브젝트
    float m_TaggetDistance = float.MaxValue;
    float m_RotSpeed = 10.0f;                    // 회전 속도
    //---타겟과의 거리 계산용

    //-----터렛 구현을 위한 변수

    //-----타워의 스텟을 담당하는 변수
    float m_MaxHP = 500.0f;              //타워의 체력
    float m_CurHP = 0.0F;
    float m_AttackDelayTime = 0.2f;            //공격 딜레이 시간
    float m_AttackRate = 0.0f;      //타워의 공격력
    float m_Attack_distance = 0.0f;

    [Header("========= UI =========")]
    public Text m_TowerNameTxt = null;
    public Image m_HPImg = null;
    //-----타워의 스텟을 담당하는 변수

    public static int m_TargetCount = 0;

    [HideInInspector] public string g_Message = "";

    // ------------------- 진진원
    [SerializeField] GameObject m_BulletObj = null;
    GameObject m_Effobj = null;
    Vector3 m_EffPos = Vector3.zero;
    Vector3 m_BulletPos = Vector3.zero;
    [SerializeField] GameObject m_EffPosObj = null;

    [HideInInspector]public int m_TowerNumber = 0;
    public TowerType m_TowerType = TowerType.Emp_Tower;

    SphereCollider m_SpCollider = null;

    AudioSource m_AttacSound = null;
    AudioClip m_AttackClicp = null;

    public bool m_TowerCommand = false;
    // Start is called before the first frame update
    void Start()
    {
        m_SpCollider = this.GetComponent<SphereCollider>();
        m_AttacSound = this.GetComponent<AudioSource>();
        SetTowerStatus();
        m_CurHP = m_MaxHP;
        
        switch (m_TowerType)
        {
            case TowerType.MachineGun_Tower:
                m_AttackClicp = Resources.Load<AudioClip>("SoundEffect/TowerSound/MachineGunShot_Snd");
                break;

            case TowerType.Missile_Tower:
                m_AttackClicp = Resources.Load<AudioClip>("SoundEffect/TowerSound/Missile_Shot_Snd");
                break;

            case TowerType.Emp_Tower:
                m_AttackClicp = Resources.Load<AudioClip>("SoundEffect/TowerSound/EMP_Shot_Snd");
                break;

            case TowerType.Super_MachineGun_Tower:
                m_AttackClicp = Resources.Load<AudioClip>("SoundEffect/TowerSound/MachineGunShot_Snd");
                break;
        }
    }

    // ------------------ 진진원
    // Update is called once per frame
    void Update()
    {
        if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
            return;
        if (m_TowerState == TowerState.Tracking) //대기 모드
        {
            Target_Tracking();
        }
        if (m_TowerState == TowerState.Count) //타겟모드
        {
            Target_Choice();
        }
        if (m_TowerState == TowerState.Attack) //공격 모드
        {
            Target_Attack();
        }
    }

    public void TakeDamage(int a_Damage)//GameObject _Unit)
    {
        m_CurHP -= a_Damage;
        if (m_HPImg != null)
            m_HPImg.fillAmount = m_CurHP / m_MaxHP;
        
        if(m_CurHP <= 0.0f)
        {
            m_CurHP = 0.0f;
            GameMgr.Inst.GoldTextSett(50);
            m_AttacSound.mute= true;
            Destroy(this.gameObject);
        }
    }

    void Attack_Eff()
    {

    }

    void Tower_Broken()
    {

    }

    void Target_Attack() //공격을 담당하는 함수
    {
        if(m_TargetObj.activeSelf == false)
        {
            m_TargetList.RemoveAt(0);
            m_TowerState = TowerState.Count;
            return;
        }

        if (m_TargetList[0] == null)
        {
            m_TowerState = TowerState.Count;    //타겟모드로 전환
            return;
        }

        // ------------------------------------- 진진원
        if (m_TargetObj == null)
            return;

        m_CacVLen = m_TargetObj.transform.position - this.transform.position;
        m_CacVLen.y = 0.0f;
        m_MoveDir = m_CacVLen.normalized;
        Quaternion a_TargetRot = Quaternion.LookRotation(m_MoveDir);
        m_Turret.transform.rotation = Quaternion.Slerp(m_Turret.transform.rotation, a_TargetRot, Time.deltaTime * m_RotSpeed);

        //  데미지를 입히는 부분을 테스트하기 위해 구현하였습니다.
        m_AttackDelayTime -= Time.deltaTime;

        if (m_AttackDelayTime <= 0.0f)
        {
            if (m_TowerType == TowerType.MachineGun_Tower || m_TowerType == TowerType.Missile_Tower)
            {// 총알 생성
                m_AttackDelayTime = 1.5f;
                GameObject a_Bullet = Instantiate(m_BulletObj);
                GameObject a_BulletGroup = GameObject.Find("BulletGroup");
                a_Bullet.GetComponent<Bullet>().m_BulletType = m_TowerType;
                a_Bullet.GetComponent<Bullet>().m_AttackRate = (int)m_AttackRate;
                a_Bullet.transform.position = m_BulletSpwanPoint.transform.position;
                a_Bullet.transform.LookAt(m_TargetObj.transform);
                a_Bullet.TryGetComponent(out Bullet a_RefBullet);
                m_Effobj = EffectPool.Inst.GetEffectObj("WFX_Explosion_Small", Vector3.zero, Quaternion.identity);
                a_RefBullet.TargetObj = m_TargetObj;
                m_EffPos = m_EffPosObj.transform.position;
                m_Effobj.transform.position = m_EffPos + (-m_CacVLen.normalized * 0.93f);
                m_Effobj.transform.LookAt(m_EffPos + (m_CacVLen.normalized * 2.0f));
            }
            else if(m_TowerType == TowerType.Super_MachineGun_Tower)
            {
                m_AttackDelayTime = 0.3f;
                
                GameObject a_Bullet = Instantiate(m_BulletObj);
                GameObject a_BulletGroup = GameObject.Find("BulletGroup");
                a_Bullet.GetComponent<Bullet>().m_BulletType = m_TowerType;
                a_Bullet.GetComponent<Bullet>().m_AttackRate = (int)m_AttackRate;
                //a_Bullet.transform.SetParent(a_BulletGroup.transform, false);
                a_Bullet.transform.position = m_BulletSpwanPoint.transform.position;
                a_Bullet.transform.LookAt(m_TargetObj.transform);
                a_Bullet.TryGetComponent(out Bullet a_RefBullet);
                a_RefBullet.TargetObj = m_TargetObj;
                EffectPool.Inst.GetEffectObj("FX_Fire_01", m_EffPosObj.transform.position, Quaternion.identity);
            }
            else if (m_TowerType == TowerType.Emp_Tower)
            {
                m_AttackDelayTime = 2.0f;

                m_Effobj = EffectPool.Inst.GetEffectObj("LaserImpactPFX", Vector3.zero, Quaternion.identity);
                m_EffPos = m_EffPosObj.transform.position;
                m_Effobj.transform.position = m_EffPos + (-m_CacVLen.normalized * 0.93f);
                m_Effobj.transform.LookAt(m_EffPos + (m_CacVLen.normalized * 2.0f));
            }
            PlaySound(m_TowerType);
        }
    }

    void Target_Tracking() //평소 타워의 상태
    {
        m_Turret.transform.Rotate(Vector3.up, 150.0f * Time.deltaTime);
    }

    void Target_Choice()
    {
        if (m_TargetList.Count == 0)
        {
            m_TowerState = TowerState.Tracking;
            return;
        }
        
        for (int i = 0; i < m_TargetList.Count; i++)
        {
            if (m_TargetList[i] == null || m_TargetList[i].activeSelf == false)
            {
                if(m_TargetList[i].activeSelf == false)
                {
                    m_TargetList.RemoveAt(i);
                    break;
                }    
                continue;
            }

            m_TargetObj = m_TargetList[i];
            m_TowerState = TowerState.Attack;            
            break;
        }

        if (m_TowerState != TowerState.Attack)
            m_TowerState = TowerState.Count;
    }

    private void OnTriggerEnter(Collider other) //공격거리안으로 적이 들어왔는지 판단하는 함수
    {
        // m_TargetList에 추가
        if (other.tag == "TANK")
        {
            if (other.gameObject.activeSelf == false)
                return;

            //---------------추가
            if (m_TowerType == TowerType.Emp_Tower) 
            {
                TankCtrl a_TankCtrl = other.GetComponent<TankCtrl>();
                a_TankCtrl.BackUpmoveVelocity = a_TankCtrl.moveVelocity;
                a_TankCtrl.moveVelocity = 5.0f;
            }
            //---------------추가

            m_TargetList.Add(other.gameObject);

            if (this.gameObject.name == "SuperTower_0_Size1")
            {
                Debug.Log("현재 공격거리내 타겟수 : " + m_TargetList.Count);
                Debug.Log("현재 공격거리내 1번째 타켓 : " + m_TargetList[0].name);
            }

            if(m_TargetList.Count == 1)
            {
                m_TargetObj = m_TargetList[0].gameObject;
                m_TowerState = TowerState.Attack;
            }
            //--------------------------KTH 추가
            //콜라이더의 충돌한 Enemy 수가 0이상일 때 타겟모드
            //m_TowerState = TowerState.Count;
            //--------------------------KTH 추가
        }

        m_TargetCount = m_TargetList.Count;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TANK")
        {
            int _index = DeletTankListCheck(other.gameObject);
            
            for(int i = 0; i < m_TargetList.Count; i++)
            {
                TankCtrl a_MoveTank = m_TargetList[i].GetComponent<TankCtrl>();

                //---------------추가
                if (m_TowerType == TowerType.Emp_Tower)
                    a_MoveTank.moveVelocity = a_MoveTank.BackUpmoveVelocity;
                //---------------추가

                if (_index == a_MoveTank.m_TankNumber)
                {
                    m_TargetList.Remove(other.gameObject);
                    if(m_TargetList.Count != 0)
                    {
                        for(int z = 0; z < m_TargetList.Count; z++)
                        {
                            if (m_TargetList[z].activeSelf == true)
                            {
                                m_TargetObj = m_TargetList[z];
                                m_TowerState = TowerState.Attack;
                                break;
                            }
                        }
                        m_TowerState = TowerState.Count;
                    }
                    else                         //리스트 삭제 후 List가 존재하지않는다면.
                    {
                        m_TowerState = TowerState.Tracking;
                    }
                    //--------------------------KTH 추가

                    break;
                }
            }

            m_TargetCount = m_TargetList.Count;
        }
    }

    int DeletTankListCheck(GameObject _obj)
    {
        TankCtrl a_MoveTank = _obj.GetComponent<TankCtrl>();
        return a_MoveTank.m_TankNumber;
    }



    //---------------DB에서 가져온 포탑 스텟 적용
    void SetTowerStatus()
    {
        int a_level = 0;
        for (int i = 0; i < GlobarValue.g_VsUserTowerList.Count; i++)
        {
            a_level = 0;
            int m_idex = i;
            int HP = 0;
            int AttackDamage = 0;
            if (GlobarValue.g_VsUserTowerList[m_idex].m_TowerType == m_TowerType)
            {
                a_level = GlobarValue.g_VsUserTowerList[m_idex].m_UnitLevel;
                HP = GlobarValue.g_VsUserTowerList[m_idex].m_TowerHP;
                m_MaxHP = HP + (HP * (a_level - 1) / 10);
                AttackDamage = GlobarValue.g_VsUserTowerList[m_idex].m_TowerAttack;
                m_AttackRate = AttackDamage + (AttackDamage * (a_level - 1) / 10);
                break;
            }
        }

        if(m_TowerCommand == true)
        {
            m_AttackRate = 10.0f;
        }
    }
    //---------------DB에서 가져온 포탑 스텟 적용

    void PlaySound(TowerType _type)
    {
        m_AttacSound.clip = m_AttackClicp;
        m_AttacSound.volume = 0.5f;
        m_AttacSound.Play();
    }
}
