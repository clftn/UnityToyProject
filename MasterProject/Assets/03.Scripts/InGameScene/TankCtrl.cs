using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TankCtrl : MonoBehaviour
{
    // 기본 탱크 정보 변수
    public int level = 0;
    public TankType m_Type = TankType.Normal;      // 탱크타입
    [HideInInspector] public float moveVelocity = 10.0f;             // 이동속도\
    [HideInInspector] public float BackUpmoveVelocity = 0.0f;

    float atk = 0.0f;                       // 공격력
    float def = 0.0f;                       // 방어력
    float attRate = 0.0f;                   // 공격 속도
    float curHp = 0.0f;                     // 현재체력
    float maxHp = 0.0f;                     // 최대체력
    float skillCool = 0.0f;                 // 스킬 쿨타임
    float attRange = 0.0f;
    // 기본 탱크 정보 변수

    // 기본 탱크 정보 변수
    [HideInInspector] public GameObject target_Obj;           // 타겟 오브젝트 저장
    //[HideInInspector] public List<GameObject> target_List = new List<GameObject>();  // 타겟 목록 저장
    Vector3 tank_Pos = Vector3.zero;        // 탱크의 좌료 저장
    Vector3 target_Pos = Vector3.zero;      // 타겟의 좌표 저장
    float att_Delay = 0.0f;                 // 공격 딜레이 타이머
    float skill_Delay = 0.0f;               // 스킬 딜레이 타이머
    float turn_Speed = 10.0f;               // 포탑 회전 속도
    public GameObject turret_Obj = null;    // 포탑 오브젝트
    public GameObject fire_Pos = null;      // 발사 위치 오브젝트
    public GameObject bullet_Obj = null;    // 총알 오브젝트
    public GameObject turret_Explo = null;  // 발사 이펙트 오브젝트
    public GameObject cannon_Obj = null;    // 포대 오브젝트

    //float h, v;

    // </ 길찾기

    // </ 이동 관련 변수
    // </ Picking 관련 변수
    float rotSpeed = 7.0f;                  // 초당 회전 속도
    internal bool isMoveOn = true;                  // 이동 On/Off
    public Transform beginTarPos = null;    // 공격 탱크가 인스턴싱 될 때 지정하는 목적지
    double addTimeCount = 0.0f;             // 누적 시간 카운트
    // Picking 관련 변수 />
    // 이동 관련 변수 />

    // </ Navigation
    NavMeshAgent navAgent;
    NavMeshPath movePath;
    internal int curPathIndex = 1;
    // Navigation />

    // 길찾기 />

    // UI 관련 변수
    public Canvas tank_Canvas = null;
    public Image hp_Img = null;
    // UI 관련 변수

    TankInfo tankInfo = new TankInfo();

    // 유닛 특성 관련 변수
    int mGBullet = 3; // 기관총 특성이 발동될 때 격발할 탄환의 수
    int bulletIdx = 0; // 현재 격발한 탄환의 수
    float mGRate = 0.2f; // 탄환을 격발할 때 잠깐 사이의 텀
    float mGTimer = 0.0f; // 탄환 격발시 타이머
    [Header("차량타입에 따른 변수")]
    public Transform machineGun_Pos = null; // Speed타입차량의 기관총 트랜스폼
    public GameObject missilePrefab;
    public GameObject barrier;
    // 유닛 특성 관련 변수

    // 탱크 움직임 오디오소스
    AudioSource m_MvSource;

    [HideInInspector] public int m_TankNumber = 0;

    // 웨이포인트 관련 변수
    GameObject wayPointGroup = null;
    internal Transform[] wayPoints = null;
    internal bool isArrived = false;
    // 웨이포인트 관련 변수
    public bool isLeft = false;
    // 탱크 다시 생성시 초기화 해야할 변수들
    // 1. isArrived => false
    // 2. isMoveOn => true
    // 3. curPathIndex => 1
    // 4. oldPathIndex => 0
    // 탱크 안움직일 때 확인해봐야할 것
    // 1. 네비메쉬에이전트 꺼져있는지
    // 2. 네비메쉬에이전트 레이어
    // 3. 목표물이 제대로 들어오는지 혹은 목표물이 네비메쉬 위에 제대로 올라가 있는지
    // 4. 탱크가 네비메쉬 위에 생성됐는지

    // 적 베이스를 일반공격할 때 멈추지 않습니다. ( 끝까지 자리를 채워주기 위해서 )
    // Speed 차량이 적 베이스를 상대로 스킬공격을 할 때 멈추지 않습니다. ( 마찬가지 이유 )

    //----------공격 알고리즘 추가
    TowerCtrl_Team[] m_TowerList = null;
    GameObject m_TowerGroupObj = null;
    GameObject m_CommandTower = null;
    TowerCtrl_Team[] m_CommandTower_Turrent;

    //----------공격 알고리즘 추가

    private void Awake()
    {
        // 이 탱크의 오디오소스 할당
        m_MvSource = this.GetComponent<AudioSource>();
    }

    void Start()
    {
        //----------공격 알고리즘 추가
        m_TowerGroupObj = GameObject.Find("EnemyGroup");
        m_CommandTower = GameObject.Find("CommandTower");
        m_CommandTower_Turrent = m_CommandTower.GetComponentsInChildren<TowerCtrl_Team>();

        //----------공격 알고리즘 추가

        WayReset();
        // 탱크 기본정보 받아오기
        //Init();
        // 탱크 기본정보 받아오기
        movePath = new NavMeshPath();
        navAgent = this.gameObject.GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        beginTarPos = GameObject.Find("Begin_Tar_Pos").transform;

        Init();
        enemies = new List<GameObject>();

        //----------공격 알고리즘 추가
        m_TowerList = m_TowerGroupObj.GetComponentsInChildren<TowerCtrl_Team>();
        //----------공격 알고리즘 추가
    }

    public void WayReset()
    {
        if (isLeft == false)
            wayPointGroup = GameObject.Find("WayPointGroup_Left");
        else
            wayPointGroup = GameObject.Find("WayPointGroup_Right");

        wayPoints = wayPointGroup.GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
            return;

        tank_Pos = this.transform.position;
        tank_Pos.y = 0.0f;

        AgentOffsetControl();

        if (att_Delay > 0.0f)
            att_Delay -= Time.deltaTime;

        if (skill_Delay > 0.0f)
            skill_Delay -= Time.deltaTime;

        if (target_Obj != null)
        {
            if (m_Type != TankType.Cannon || skill_Delay > 0.0f) // 캐논형 타입의 차량은 스킬 사용중에는 터렛회전을 여기서 하지 않는다.
            {
                target_Pos = target_Obj.transform.position;
                target_Pos.y = 0.0f;
                Vector3 dir = target_Pos - tank_Pos;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(turret_Obj.transform.rotation, lookRotation, Time.deltaTime * turn_Speed).eulerAngles;
                turret_Obj.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }

        if (GameMgr.Inst.tower_List.Count <= 0)
        {
            if (m_Type != TankType.Cannon || skill_Delay > 0.0f) // 캐논형 타입의 차량은 스킬 사용중에는 터렛회전을 여기서 하지 않는다.
            {
                turret_Obj.transform.rotation = Quaternion.Slerp(turret_Obj.transform.rotation,
                this.transform.rotation, Time.deltaTime * turn_Speed);
                turret_Obj.transform.localEulerAngles = new Vector3(0.0f, turret_Obj.transform.localEulerAngles.y, 0.0f);
            }
        }

        TankUIRotate();
        NavUpdate(); // 길찾기
        Attack();
        // 유닛특성 함수들
        Repair(5); // 리페어 탱크인 경우에만 실행
        MachineGun();
        Cannon();
        Barrier();

        if (isArrived == true) // 만약 도착한 이후 다른 유닛에 의해 충돌해서 튕겨져나가 공격거리가 적 중심 건물에 닿지 않을 때
        {
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            if ((transform.position - m_CommandTower.transform.position).magnitude > attRange)
                navAgent.SetDestination(wayPoints[wayPoints.Length - 1].position);
        }
        else
        {
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
    }

    void Init()
    {
        // 탱크 기본정보 받아오기
        tankInfo.m_Type = m_Type;
        tankInfo.TankInit();

        level = tankInfo.level; // 레벨 받아오기
        atk = tankInfo.atk + (tankInfo.atk * (tankInfo.level - 1) / 10);     // 공격력 받아오기
        def = tankInfo.def + (tankInfo.def * (tankInfo.level - 1) / 10);     // 방어력 받아오기
        moveVelocity = tankInfo.speed;  // 이동속도 받아오기
        attRate = tankInfo.attRate;     // 공격속도 받아오기
        maxHp = tankInfo.maxHp + (tankInfo.maxHp * (tankInfo.level - 1) / 10);        // 최대체력 받아오기
        curHp = maxHp;                  // 현재체력 셋팅
        skillCool = tankInfo.skillCool; // 스킬쿨타임 받아오기
        attRange = tankInfo.attRange;   // 공격사거리 받아오기
        // 탱크 기본정보 받아오기
    }

    void AgentOffsetControl() // 플레이어의 발을 땅에 자연스럽게 올려 놓기 위해 에이전트의 오프셋을 조절해주는 함수
    {
        float baseOffset = 0.0f;
        float curPosY = 0.0f;
        float tarPosY = 0.0f;

        curPosY = transform.position.y;

        tarPosY = GetFootYPos();
        //Debug.Log("목표지점 : " + tarPosY);
        //Debug.Log("현재지점 : " + curPosY);
        baseOffset = tarPosY - curPosY;
        navAgent.baseOffset += baseOffset;
    }

    Ray ray;
    RaycastHit hit;

    float GetFootYPos() // 현재 밟고 있는 땅의 높이 구하기
    {
        float a_TarPosY = 0.0f;
        ray.origin = transform.position + new Vector3(0, 1.0f, 0);
        ray.direction = -Vector3.up;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            a_TarPosY = hit.point.y;
            GameObject hitObj = hit.collider.gameObject;
        }
        return a_TarPosY;
    }

    public void TakeDamage(float a_Damage)
    {
        curHp -= a_Damage;

        if (hp_Img != null)
            hp_Img.fillAmount = curHp / maxHp;

        MonitorTankDie();
    }

    #region ---------- 탱크 포탑 회전

    void TankUIRotate()
    {

        if (tank_Canvas != null)
        {
            tank_Canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
            Vector3 pos = this.transform.position;
            pos.y += 0.5f;
            pos.z -= 1;
            tank_Canvas.transform.position = pos;
        }
    }

    #endregion

    #region ---------- 탱크 공격 부분

    void Attack()
    {
        if (TankType.Speed == m_Type || TankType.Cannon == m_Type) // 스킬 사용중일 때는 일반공격 못하도록
            return;

        if (att_Delay > 0.0f)
            return;

        if (GameMgr.Inst.tower_List.Count <= 0)
        {
            target_Obj = null;
            return;
        }


        List<float> target_Dist = new List<float>();
        List<int> index_List = new List<int>();

        for (int ii = 0; ii < GameMgr.Inst.tower_List.Count;)
        {
            if (GameMgr.Inst.tower_List[ii] == null)    // 타겟 리스트의 값이 null 인지 확인
            {
                GameMgr.Inst.tower_List.Remove(GameMgr.Inst.tower_List[ii]);    // null 값이 저장되어 있으면 지우기

                if (GameMgr.Inst.tower_List.Count <= 0)
                {
                    target_Obj = null;
                    return;
                }
            }
            else
            {
                float dis = Vector3.Distance(tank_Pos, GameMgr.Inst.tower_List[ii].transform.position);

                if (dis <= attRange)
                {
                    index_List.Add(ii);
                    target_Dist.Add(dis);
                }

                ii++;
            }
        }

        if (target_Dist.Count <= 0)
        {
            target_Obj = null;
            return;
        }


        int target_Index = 0;
        GetMinCheck(target_Dist, out target_Index);
        int list_Index = index_List[target_Index];

        target_Obj = GameMgr.Inst.tower_List[list_Index];

        //if (target_Obj.name.Contains("Enemy_Base") == true)
        //    isMoveOn = false;

        target_Pos = target_Obj.transform.position;
        target_Pos.y = 0.0f;
        att_Delay = attRate;
        GameObject bullet = Instantiate(bullet_Obj, fire_Pos.transform.position, fire_Pos.transform.rotation);
        bullet.GetComponent<BulletCtrl>().target_Obj = target_Obj;
        bullet.GetComponent<BulletCtrl>().bullet_Damage = atk;
        GameObject explo_Obj = Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);
        explo_Obj.transform.SetParent(fire_Pos.transform);

    }

    #endregion

    #region ----------- 유닛 특성 구현 부분

    // 유닛 스킬 구현 부분 ------------------------------------------------------------------------------------------------------------------------------
    void Repair(int repairValue)
    {
        if (m_Type != TankType.Repair)  // 탱크 타입 검사
            return;

        if (skill_Delay > 0.0)          // 스킬 딜레이 검사
            return;

        float skillRange = 5.0f; // 스킬범위 (임시)

        GameObject[] allyObjs = GameObject.FindGameObjectsWithTag("TANK"); // 아군 탱크들을 찾음

        for (int i = 0; i < allyObjs.Length; i++)
        {
            if (allyObjs[i] == gameObject) // 자기자신은 치료하지 않음
                continue;

            if ((allyObjs[i].transform.position - transform.position).magnitude < skillRange) // 스킬 범위 내에 있는지 검사
            {
                allyObjs[i].GetComponent<TankCtrl>().curHp += repairValue; // 체력 회복
                if (allyObjs[i].GetComponent<TankCtrl>().curHp > 100)
                {
                    allyObjs[i].GetComponent<TankCtrl>().curHp = 100;
                }
                if (allyObjs[i].GetComponent<TankCtrl>().hp_Img != null)
                    allyObjs[i].GetComponent<TankCtrl>().hp_Img.fillAmount = curHp / maxHp;
                Debug.Log(allyObjs[i].name + "을 " + repairValue + "만큼 수리함");
            }
        }

        skill_Delay = skillCool;
    }
    // Solid 유닛 스킬 관련 변수
    GameObject a_Barrier = null;
    bool isBarrier = false; // 보호막이 활성화 중인지
    // Solid 유닛 스킬 관련 변수
    void Barrier() // 일정 범위에 보호막을 쳐서 아군을 보호
    {
        if (m_Type != TankType.Solid)
            return;

        if (skill_Delay > 0.0)
            return;

        if (isBarrier == false)
        {
            a_Barrier = Instantiate(barrier, transform.position, Quaternion.identity);
            a_Barrier.transform.SetParent(this.transform);
            isBarrier = true;
        }

        if (isBarrier == true && a_Barrier == null)
        {
            isBarrier = false;
            skill_Delay = skillCool;
        }

    }

    void MachineGun()
    {
        if (m_Type != TankType.Speed)
            return;

        if (att_Delay > 0.0f)
            return;

        if (mGTimer > 0.0f)
        {
            mGTimer -= Time.deltaTime;
            return;
        }

        if (GameMgr.Inst.tower_List.Count <= 0)
        {
            target_Obj = null;
            mGTimer = 0.0f;
            bulletIdx = 0;
            return;
        }


        List<float> target_Dist = new List<float>();
        List<int> index_List = new List<int>();

        for (int ii = 0; ii < GameMgr.Inst.tower_List.Count;)
        {
            if (GameMgr.Inst.tower_List[ii] == null)    // 타겟 리스트의 값이 null 인지 확인
            {
                GameMgr.Inst.tower_List.Remove(GameMgr.Inst.tower_List[ii]);    // null 값이 저장되어 있으면 지우기

                if (GameMgr.Inst.tower_List.Count <= 0)
                {
                    target_Obj = null;
                    mGTimer = 0.0f;
                    bulletIdx = 0;
                    return;
                }
            }
            else
            {
                float dis = Vector3.Distance(tank_Pos, GameMgr.Inst.tower_List[ii].transform.position);

                if (dis <= attRange)
                {
                    index_List.Add(ii);
                    target_Dist.Add(dis);
                }

                ii++;
            }
        }

        if (target_Dist.Count <= 0)
        {
            target_Obj = null;
            mGTimer = 0.0f;
            bulletIdx = 0;
            return;
        }


        int target_Index = 0;
        GetMinCheck(target_Dist, out target_Index);
        int list_Index = index_List[target_Index];

        target_Obj = GameMgr.Inst.tower_List[list_Index];

        //if (target_Obj.name.Contains("Enemy_Base") == true)
        //    isMoveOn = false;

        target_Pos = target_Obj.transform.position;
        target_Pos.y = 0.0f;


        if (mGTimer <= 0.0f)
        {
            GameObject bullet = Instantiate(bullet_Obj, machineGun_Pos.transform.position, fire_Pos.transform.rotation);
            bullet.GetComponent<BulletCtrl>().target_Obj = target_Obj;
            bullet.GetComponent<BulletCtrl>().bullet_Damage = atk;
            //bullet.GetComponent<MeshRenderer>().material.SetColor("_Color",Color.red);
            GameObject explo_Obj = Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);
            explo_Obj.transform.SetParent(fire_Pos.transform);
            mGTimer = mGRate; // 텀 충전
            bulletIdx++;
            if (bulletIdx == mGBullet) // 모든 탄환을 격발하고 나면 스킬쿨타임 돌기 시작
            {
                att_Delay = attRate; // 스킬 사용후 바로 기본공격 못하게
                mGTimer = 0.0f;
                bulletIdx = 0;
            }
        }
    }

    // -------- Cannon 유닛 스킬 관련 변수
    bool isShot = false;
    GameObject missile;
    GameObject missileRange;
    float tx;
    float ty;
    float tz;
    float v;
    public float g = 9.8f;
    float elapsed_time;
    public float max_height;
    float t;
    Vector3 start_pos;
    Vector3 end_pos;
    float dat;  //도착점 도달 시간 
    float actionTimer = 2.0f;
    int ranEnemyIdx = -1;
    List<GameObject> enemies = null;
    public GameObject missileRangePrefab = null;
    // -------- Cannon 유닛 스킬 관련 변수
    void Cannon() // 랜덤으로 선택한 적에게 포물선으로 미사일 타격
    {
        if (m_Type != TankType.Cannon)
            return;

        // 적 탐색 ---------------------------------------------------------------------------------------------------------
        GameObject[] a_Enemies = GameObject.FindGameObjectsWithTag("TOWER");

        enemies.Clear();

        for (int i = 0; i < a_Enemies.Length; i++)
        {
            if ((a_Enemies[i].transform.position - transform.position).magnitude < attRange) // 스킬 사거리를 통해 타겟측정
            {
                enemies.Add(a_Enemies[i]);
            }
        }

        if (enemies.Count > 0) // 사거리 안에 적이 있으면 안움직임
        {
            isMoveOn = false;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                    enemies.RemoveAt(i);
            }
        }

        if (enemies.Count <= 0 && isShot == false)
        {
            isMoveOn = true;   // 사거리 안에 적이 없으면 움직임
        }
        //if (enemies.Count < 1) // 탐색된 적이 없으면 리턴
        //    return;

        // 적 탐색 ---------------------------------------------------------------------------------------------------------


        if (missile == null && isShot == true) // 미사일 격추가 완료 되면
        {
            isShot = false;

            Destroy(missileRange); // 미사일 격추가 끝나면 타격범위 보여주던거 삭제

            //isMoveOn = true; // 미사일 격추가 끝나면 다시 움직임.
            skill_Delay = skillCool; // 스킬 쿨타임 재활성화
            att_Delay = attRate; // 스킬 사용후 바로 기본공격 못하게
            ranEnemyIdx = -1; // 적선택인덱스 초기화
            actionTimer = 2.0f;
            cannon_Obj.transform.localEulerAngles = new Vector3(0, cannon_Obj.transform.localEulerAngles.y, cannon_Obj.transform.localEulerAngles.z);
            return;
        }

        if (skill_Delay > 0.0f) // 아직 스킬 쿨타임이 남아있다면 리턴
            return;
        //------------------------------------------------------------------------------------------------------------------
        if (isShot == false && enemies.Count > 0)
        {
            isMoveOn = false; // 스킬 사용중에는 움직이지 않는다.

            if (actionTimer > 0.0f) // 시즈모드On 연출타임
            {
                actionTimer -= Time.deltaTime;
            }

            if (ranEnemyIdx < 0) // 적 선택
            {
                ranEnemyIdx = Random.Range(0, enemies.Count);
            }

            if (ranEnemyIdx >= 0 && ranEnemyIdx < enemies.Count)
            {
                if (enemies[ranEnemyIdx] == null) // 조준 도중에 적이 파괴됐다면
                {
                    ranEnemyIdx = -1;
                    return;
                }
            }

            if (ranEnemyIdx < 0 || ranEnemyIdx >= enemies.Count) // 조준 도중에 적이 파괴 됐다면
            {
                ranEnemyIdx = -1;
                return;
            }

            Quaternion a_TargetDir = Quaternion.LookRotation(enemies[ranEnemyIdx].transform.position - transform.position);
            turret_Obj.transform.rotation = Quaternion.Slerp(turret_Obj.transform.rotation, a_TargetDir, Time.deltaTime * 10.0f);

            Vector3 dir = new Vector3(0, 0.5f, 0.5f);
            cannon_Obj.transform.localRotation = Quaternion.LookRotation(dir);


            if (actionTimer <= 0.0f) // 시즈모드On 연출타임이 끝나면 미사일 발사
            {
                missile = Instantiate(missilePrefab, fire_Pos.transform.position, Quaternion.identity);
                missileRange = Instantiate(missileRangePrefab, enemies[ranEnemyIdx].transform.position, Quaternion.Euler(new Vector3(90, 0, 0))); // 미사일 타격범위 인스턴스
                missile.GetComponent<MissileCtrl>().target_Obj = enemies[ranEnemyIdx];
                missile.GetComponent<MissileCtrl>().damage = (int)atk; // 미사일 데미지 값
                Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);

                start_pos = missile.transform.position;
                end_pos = enemies[ranEnemyIdx].transform.position;
                max_height = 20.0f;

                var dh = end_pos.y - start_pos.y;
                var mh = max_height - start_pos.y;
                ty = Mathf.Sqrt(2 * g * mh);

                float a = g;
                float b = -2 * ty;
                float c = 2 * dh;

                dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

                tx = -(start_pos.x - end_pos.x) / dat;
                tz = -(start_pos.z - end_pos.z) / dat;

                elapsed_time = 0;

                isShot = true;
            }
        }

        if (isShot == true)
        {
            StartCoroutine(ShootImp());
        }
    }

    IEnumerator ShootImp()
    {
        while (true)
        {
            if (missile == null)
            {
                yield break;
            }

            this.elapsed_time += Time.deltaTime * 0.02f;

            var tx = start_pos.x + this.tx * elapsed_time;
            var ty = start_pos.y + this.ty * elapsed_time - 0.5f * g * elapsed_time * elapsed_time;
            var tz = start_pos.z + this.tz * elapsed_time;

            var tpos = new Vector3(tx, ty, tz);

            missile.transform.LookAt(tpos);
            missile.transform.position = tpos;

            if (this.elapsed_time >= this.dat)
                break;

            yield return null;
        }
    }
    // 유닛 스킬 구현 부분 ------------------------------------------------------------------------------------------------------------------------------
    #endregion
    #region ---------- 배열의 최소값 체크 (제일 가까운 적 체크 용)

    void GetMinCheck(List<float> a_List, out int a_Min)
    {
        float min = a_List[0];
        a_Min = 0;

        for (int ii = 0; ii < a_List.Count; ii++)
        {
            if (min > a_List[ii])
            {
                min = a_List[ii];
                a_Min = ii;
            }
        }
    }

    #endregion

    #region -------------- 길찾기 부분
    void NavUpdate()
    {
        // 마우스 피킹 이동
        if (isMoveOn == true)
        {
            // 네비게이션 메시 길찾기를 이용할 때 코드
            isMoveOn = MoveToPath(); // 도착한 경우 false 리턴
        }
    }

    // MoveToPath 관련 변수
    [HideInInspector] public bool isSuccessed = true;
    Vector3 curCPos = Vector3.zero;
    Vector3 cacDestV = Vector3.zero;
    Vector3 targetDir;
    float cacSpeed = 0.0f;
    float nowStep = 0.0f;
    Vector3 velocity = Vector3.zero;
    Vector3 vTowardNom = Vector3.zero;
    int oldPathCount = 0;

    public bool MoveToPath(float overSpeed = 1.0f)
    {
        isSuccessed = true;

        if (movePath == null)
        {
            movePath = new NavMeshPath();
        }

        oldPathCount = curPathIndex;
        if (curPathIndex < wayPoints.Length) // 최소 curPathIndex = 1 보다 큰 경우에
        {
            curCPos = this.transform.position;          // 현재 위치 업데이트
            cacDestV = wayPoints[curPathIndex].position;  // 현재 이동해야할 꼭지점의 위치

            curCPos.y = cacDestV.y;         // 높이 오차가 있어서 도착 판정을 못하는 경우가 있다. ( 도착지점의 높이를 캐릭터의 높이에 넣음 )
            targetDir = cacDestV - curCPos; // 현재 이동해야할 목표지점 - 현재 위치 ( 위에서 높이 값을 맞춰줬으므로 같은 평면으로 놓고 구한 것이 된다. ) 
            targetDir.y = 0.0f;             // 한 번 더 평면처리 (쓸데없는 듯)
            targetDir.Normalize();          // 이동해야할 방향벡터 구하기

            cacSpeed = moveVelocity;         // 속력는 버퍼에 넣어 처리
            cacSpeed = cacSpeed * overSpeed; // 현재속도 * 배속 ( 기본배속 1.0f )

            nowStep = cacSpeed * Time.deltaTime; // ( 한 프레임에 이동할 거리 ) 이번에 이동했을 때 이 안으로만 들어와도...

            velocity = cacSpeed * targetDir; // 속도 = 크기 * 방향
            velocity.y = 0.0f;               // 속도 평면처리
            navAgent.velocity = velocity;    // 이동처리

            if ((cacDestV - curCPos).magnitude <= nowStep)   // 다음 지점까지 거리가 한 프레임에 이동할 거리보다 작아지면 중간점에 도착한 것으로 본다.
            {
                curPathIndex = curPathIndex + 1; // 다음 꼭지점 업데이트
            }

            addTimeCount = addTimeCount + Time.deltaTime; // 경과 시간 증가
        }

        if (curPathIndex < wayPoints.Length) // 목적지에 아직 도착하지 않았다면
        {
            // 캐릭터 회전 / 애니메이션 방향 조정
            vTowardNom = wayPoints[curPathIndex].position - this.transform.position; // 가야할 지점까지의 거리
            vTowardNom.y = 0.0f;
            vTowardNom.Normalize(); // 단위 벡터를 만든다.

            if (0.0001f < vTowardNom.magnitude) // 로테이션에서는 모두 들어가야 한다.
            {
                Quaternion targetRot = Quaternion.LookRotation(vTowardNom);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
            }
        }
        else // 최종 목적지에 도착한 경우 매 프레임 호출
        {
            if (oldPathCount < wayPoints.Length) // 최종 목적지에 도착한 경우 한 번 발생시키기 위한 부분
            {
                ClearPath();
            }

            isSuccessed = false; // 아직 목적지에 도착하지 않았다면 다시 잡아 줄 것이기 때문에...
        }
        return isSuccessed;
    }

    void ClearPath()
    {
        Debug.Log("도착");
        isMoveOn = false;
        isArrived = true;

        if (isMoveOn == false)
            m_MvSource.mute = true;
    }
    #endregion

    #region 탱크 사망처리 부분
    // -------------- 탱크의 사망을 감지하는 함수
    private void MonitorTankDie()
    {
        // 현재 HP가 0 이하인 경우
        if (curHp <= 0)
        {
            //----------공격 알고리즘 추가
            //타워 리스트에 가지고 있는 탱크리스트를 찾아 현재 탱크가 사망시 같은 번호를 가진 리스트는 삭제
            int a_TankNume = 0;
            for (int i = 0; i < m_TowerList.Length; i++)
            {
                if (m_TowerList[i].m_TargetList.Count == 0)
                    continue;

                for (int ii = 0; ii < m_TowerList[i].m_TargetList.Count; ii++)
                {
                    a_TankNume = m_TowerList[i].m_TargetList[ii].GetComponent<TankCtrl>().m_TankNumber;
                    if (a_TankNume == m_TankNumber)
                    {
                        m_TowerList[i].m_TargetList.RemoveAt(ii);
                        m_TowerList[i].m_TowerState = TowerState.Count;
                        break;
                    }
                }
            }

            a_TankNume = 0;
            for (int i = 0; i < m_CommandTower_Turrent.Length; i++)
            {
                if (m_CommandTower_Turrent[i].m_TargetList.Count == 0)
                    continue;
                for (int ii = 0; ii < m_CommandTower_Turrent[i].m_TargetList.Count; ii++)
                {
                    a_TankNume = m_CommandTower_Turrent[i].m_TargetList[ii].GetComponent<TankCtrl>().m_TankNumber;
                    if (a_TankNume == m_TankNumber)
                    {
                        m_CommandTower_Turrent[i].m_TargetList.RemoveAt(ii);
                        m_CommandTower_Turrent[i].m_TowerState = TowerState.Count;
                        break;
                    }
                }
            }

            //----------공격 알고리즘 추가

            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            turret_Obj.transform.rotation = this.transform.rotation;
            //this.GetComponent<NavMeshAgent>().enabled = false;
            // ---- 폭발 오디오 재생하는 부분
            // 추후 경로 오류 발생시 path만 수정!
            //string resorcepath = "SoundEffect/Explosion01.ogg";
            //AudioClip audio = Resources.Load(resorcepath) as AudioClip;
            //Camera.main.GetComponent<AudioSource>().PlayOneShot(audio);

            // ----- 탱크를 오브젝트 풀로 돌리는 부분
            UnitObjPool.Inst.ReturnObj(this.gameObject, (int)m_Type);
            curHp = maxHp;
            hp_Img.fillAmount = 1.0f;
            curPathIndex = 1;
            moveVelocity = BackUpmoveVelocity;
            isMoveOn = true;
            addTimeCount = 0.0f;
            //ClearPath();
        }
    }
    #endregion
}