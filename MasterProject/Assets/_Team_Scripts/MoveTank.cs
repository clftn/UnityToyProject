using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTank : MonoBehaviour
{
    //public GameObject m_TowerCheckObj = null;
    float m_MoveVelocity = 3.0f;     //평면 초당 이동 속도...

    //---- Navigation
    protected NavMeshAgent nvAgent;    //using UnityEngine.AI;
    protected NavMeshPath movePath;

    protected Vector3 m_PathEndPos = Vector3.zero;
    [HideInInspector] public int m_CurPathIndex = 1;
    //---- Navigation

    private Vector3 m_MoveDir = Vector3.zero;   //평면 진행 방향
    private double m_MoveDurTime = 0.0;         //목표점까지 도착하는데 걸리는 시간
    private double m_AddTimeCount = 0.0;        //누적시간 카운트 
    protected float m_RotSpeed = 7.0f;          //초당 회전 속도
    Vector3 a_StartPos = Vector3.zero;
    Vector3 a_CacLenVec = Vector3.zero;
    private Vector3 m_TargetPos = Vector3.zero; //최종 목표 위치

    private GameObject m_TargetUnit = null;
    public GameObject target;

    bool m_MoveOnOff = true;
    [HideInInspector]public int m_TankNumber = 0;

    float m_HP = 0.0f;
    float m_MAXHP = 100.0f;

    private void Awake()
    {
        target = GameObject.Find("EndPost");
        movePath = new NavMeshPath();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        nvAgent.updateRotation = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_HP = m_MAXHP;
        Move(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }

    public void Move(Vector3 a_SetPickVec, GameObject a_PickMon = null)
    {
        //캐릭터들의 Hp바와 닉네임바 RaycastTarget을 모두 꺼주어야 피킹이 정상적으로 작동된다. 
        //그렇지 않으면 if (IsPointerOverUIObject() == false) 로 자꾸 막히게 된다.

        a_StartPos = this.transform.position; //출발 위치    

        a_SetPickVec.y = this.transform.position.y; // 최종 목표 위치

        a_CacLenVec = a_SetPickVec - a_StartPos;
        a_CacLenVec.y = 0.0f;

       

        if (a_CacLenVec.magnitude < 0.5f)  //너무 근거리 피킹은 스킵해 준다.
            return;

        //---네비게이션 메쉬 길찾기를 이용할 때 코드
        float a_PathLen = 0.0f;
        if (MyNavCalcPath(a_StartPos, a_SetPickVec, ref a_PathLen) == false)
            return;
        //---네비게이션 메쉬 길찾기를 이용할 때 코드

        m_TargetPos = a_SetPickVec;   // 최종 목표 위치
        m_MoveOnOff = true;       //피킹 이동 OnOff

        m_MoveDir = a_CacLenVec.normalized;
        //---네비게이션 메쉬 길찾기를 이용했을 때 거리 계산법
        m_MoveDurTime = a_PathLen / m_MoveVelocity; //도착하는데 걸리는 시간
        //---네비게이션 메쉬 길찾기를 이용했을 때 거리 계산법

        ////---일반적으로 이용했을 때 거리 계산법
        //m_MoveDurTime = a_CacLenVec.magnitude / m_MoveVelocity;
        ////도착하는데 걸리는 시간
        ////---일반적으로 이용했을 때 거리 계산법
        m_AddTimeCount = 0.0;

        m_TargetUnit = a_PickMon; //타겟 초기화 또는 무효화 

    }//public void Move()

    Vector3 a_VecLen = Vector3.zero;
    public bool MyNavCalcPath(Vector3 a_StartPos, Vector3 a_TargetPos,
                                ref float a_PathLen) //길찾기...
    { //경로 탐색 함수
        //--- 피킹이 발생된 상황이므로 초기화 하고 계산한다.
        movePath.ClearCorners();  //경로 모두 제거 
        m_CurPathIndex = 1;       //진행 인덱스 초기화 
        m_PathEndPos = transform.position;
        //--- 피킹이 발생된 상황이므로 초기화 하고 계산한다.

        if (nvAgent == null || nvAgent.enabled == false)
            return false;

        if (NavMesh.CalculatePath(a_StartPos, a_TargetPos, -1, movePath) == false)
        {   //CalculatePath() 함수 계산이 끝나고 정상적으로instance.final 
            //즉 목적지까지 계산에 도달했다는 뜻 
            //--> p.status == UnityEngine.AI.NavMeshPathStatus.PathComplete 
            //<-- 그럴때 정상적으로 타겟으로 설정해 준다.는 뜻
            // 길 찾기 실패 했을 때 점프하는 경향이 있다. 
            return false;
        }

        if (movePath.corners.Length < 2)
            return false;

        for (int i = 1; i < movePath.corners.Length; ++i)
        {
            //#if UNITY_EDITOR
            //            Debug.DrawLine(movePath.corners[i - 1], movePath.corners[i], Color.cyan, 10);
            //            //맨마지막 인자 duration 라인을 표시하는 시간
            //            Debug.DrawLine(movePath.corners[i], movePath.corners[i] + Vector3.up * i,
            //                           Color.cyan, 10);
            //#endif
            a_VecLen = movePath.corners[i] - movePath.corners[i - 1];
            //a_VecLen.y = 0.0f;
            a_PathLen = a_PathLen + a_VecLen.magnitude;
        }

        if (a_PathLen <= 0.0f)
            return false;

        //-- 주인공이 마지막 위치에 도착했을 때 정확한 방향을 
        // 바라보게 하고 싶은 경우 때문에 계산해 놓는다.
        m_PathEndPos = movePath.corners[(movePath.corners.Length - 1)];

        return true;
    }

    void MoveUpdate()
    {
        if (m_MoveOnOff == true)
        {
            m_MoveOnOff = MoveToPath();
        }
    }

    //--- MoveToPath 관련 변수들...
    private bool a_isSucessed = true;
    private Vector3 a_CurCPos = Vector3.zero;
    private Vector3 a_CacDestV = Vector3.zero;
    private Vector3 a_TargetDir;
    private float a_CacSpeed = 0.0f;
    private float a_NowStep = 0.0f;
    private Vector3 a_Velocity = Vector3.zero;
    private Vector3 a_vTowardNom = Vector3.zero;
    private int a_OldPathCount = 0;
    ////--- MoveToPath 관련 변수들...
    public bool MoveToPath(float overSpeed = 1.0f)
    {
        a_isSucessed = true;

        if (movePath == null)
        {
            movePath = new NavMeshPath();
        }

        a_OldPathCount = m_CurPathIndex;
        if (m_CurPathIndex < movePath.corners.Length) //최소 m_CurPathIndex = 1 보다 큰 경우에는 캐릭터를 이동시켜 준다.
        {
            a_CurCPos = this.transform.position;
            a_CacDestV = movePath.corners[m_CurPathIndex];
            a_CurCPos.y = a_CacDestV.y;  //높이 오차가 있어서 도착 판정을 못하는 경우가 있다. 
            a_TargetDir = a_CacDestV - a_CurCPos;
            a_TargetDir.y = 0.0f;
            a_TargetDir.Normalize();

            a_CacSpeed = m_MoveVelocity;
            a_CacSpeed = a_CacSpeed * overSpeed;

            a_NowStep = a_CacSpeed * Time.deltaTime; //이번에 이동했을 때 이 안으로만 들어와도 무조건 도착한 것으로 본다.

            a_Velocity = a_CacSpeed * a_TargetDir;
            a_Velocity.y = 0.0f;
            nvAgent.velocity = a_Velocity;          //이동 처리...

            if ((a_CacDestV - a_CurCPos).magnitude <= a_NowStep) //중간점에 도착한 것으로 본다.  여기서 a_CurCPos == Old Position의미
            {
                movePath.corners[m_CurPathIndex] = this.transform.position;
                m_CurPathIndex = m_CurPathIndex + 1;
            }//if ((a_CacDestV - a_CurCPos).magnitude <= a_NowStep) //중간점에 도착한 것으로 본다.  

            m_AddTimeCount = m_AddTimeCount + Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount) //목표점에 도착한 것으로 판정한다.
            {
                m_CurPathIndex = movePath.corners.Length;
            }

        }//if (m_CurPathIndex < movePath.corners.Length) //최소 m_CurPathIndex = 1 보다 큰 경우에는 캐릭터를 이동시켜 준다.

        if (m_CurPathIndex < movePath.corners.Length)  //목적지에 아직 도착 하지 않은 경우 매 플레임 
        {
            //-------------캐릭터 회전 / 애니메이션 방향 조정
            a_vTowardNom = movePath.corners[m_CurPathIndex] - this.transform.position;
            a_vTowardNom.y = 0.0f;
            a_vTowardNom.Normalize();        // 단위 벡터를 만든다.

            if (0.0001f < a_vTowardNom.magnitude)  //로테이션에서는 모두 들어가야 한다.
            {
                Quaternion a_TargetRot = Quaternion.LookRotation(a_vTowardNom);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                            a_TargetRot, Time.deltaTime * m_RotSpeed);
            }

            //-------------캐릭터 회전 / 애니메이션 방향 조정
        }
        else //최종 목적지에 도착한 경우 매 플레임
        {
            if (a_OldPathCount < movePath.corners.Length) //최종 목적지에 도착한 경우 한번 발생시키기 위한 부분
            {
                m_MoveOnOff = false;
                m_PathEndPos = transform.position;
                if (0 < movePath.corners.Length)
                {
                    movePath.ClearCorners();  //경로 모두 제거 
                }
                m_CurPathIndex = 1;       //진행 인덱스 초기화
            }

            a_isSucessed = false; //아직 목적지에 도착하지 않았다면 다시 잡아 줄 것이기 때문에... 
        }

        return a_isSucessed;
    }

    public void TakeDamage(float _Damage)
    {
        m_HP -= _Damage;
        if(m_HP <= 0.0f)
        {
            //TankDie();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EndTankMove")
        {
            Destroy(this.gameObject);
        }
    }

    //void TankDie()
    //{
    //    TowerCheckObjMgr _TowerCheckObjMgr = m_TowerCheckObj.GetComponent<TowerCheckObjMgr>();
    //    int m_TowerListCount = _TowerCheckObjMgr._ListCount;
    //    Debug.Log(m_TowerListCount.ToString());
    //    for (int i = 0; i < m_TowerListCount; i++)
    //    {
    //        //GameObject _tower = _TowerCheckObjMgr.m_TowerList[i];

    //        //Debug.Log(_TowerCheckObjMgr.m_TowerList[i].name);

    //        TowerCtrl_Team _TowerCtrl_Team = _tower.GetComponent<TowerCtrl_Team>();

    //        for (int ii = 0; ii < _TowerCtrl_Team.m_TargetList.Count; ii++)
    //        {
    //            MoveTank _MoveTank = _TowerCtrl_Team.m_TargetList[ii].GetComponent<MoveTank>();
    //            if (_MoveTank.m_TankNumber == m_TankNumber)
    //            {
    //                _TowerCtrl_Team.m_TargetList.RemoveAt(ii);
    //                if (_TowerCtrl_Team.m_TargetList.Count != 0)
    //                {
    //                    _TowerCtrl_Team.m_TargetObj = _TowerCtrl_Team.m_TargetList[0].gameObject;
    //                    _TowerCtrl_Team.m_TowerState = TowerState.Attack;
    //                }
    //                else //리스트 삭제 후 List가 존재하지않는다면.
    //                {
    //                    _TowerCtrl_Team.m_TowerState = TowerState.Tracking;
    //                }
    //                break;
    //            }
    //        }
    //    }
    //    Destroy(this.gameObject);
    //}
}
