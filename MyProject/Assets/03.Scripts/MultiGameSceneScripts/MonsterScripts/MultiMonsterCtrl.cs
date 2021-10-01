using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class MultiMonsterCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum MonsterState
    {
        idle,
        patrol,
        trace,
        attack,
        hit,
        die
    };
    public MonsterState monsterState = MonsterState.patrol;

    enum MonsterPatrolState
    {
        Moving,
        Stop
    }
    MonsterPatrolState monsterPS = MonsterPatrolState.Moving;

    private Transform monsterTr;
    private NavMeshAgent nvAgent;

    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    public float attackDist = 2.0f;
    // 몬스터의 사망여부
    private bool isDie = false;

    // 몬스터 애니메이션
    private Animator animator;

    // 피튀기는 이펙트
    public GameObject bloodEffect;
    public GameObject bloodDecal;

    // 몬스터 상태값
    int MaxHp = 200;
    int CurHp = 200;
    [HideInInspector] public int MonAtt = 10;

    // 몬스터 이동
    Vector3 moveVec = Vector3.zero;
    float moveTime = 0.0f;
    float stopTime = 3.0f;
    float stopTimeUse = 3.0f;
    bool isHit = false;

    // 주인공 찾기 전용
    GameObject m_AggroTarget = null;
    MultiPlayerInfoCtrl AggroPlayerStateRef = null;
    // 몬스터를 죽인 ID를 기억하고, 해당 ID에게 아이템을 먹을 기회를 준다.
    int AttackFinishPlayerID = 0;
    // 몬스터가 죽었을 시간을 따로 넣고 계산해야 한다.
    float monsterDestroyTime = 2.0f;

    // 포톤 동기화 전용 변수들
    PhotonView pv = null;
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;
    int NetHp = 0;
    // 포톤 동기화 전용 변수들

    // 몬스터 처음 등장 시 관련 변수들
    Collider[] colliders;
    SkinnedMeshRenderer[] m_SMRs;
    float ViewTime = 2.0f; // 처음 등장 시 2초 뒤에    
    // 몬스터 처음 등장 시 관련 변수들

    // 게임 매니저(태그로 찾으면 더 좋지 않을까?)
    MutiGameMgr GameMgrRef = null;
    
    void Awake()
    {
        pv = GetComponent<PhotonView>();


        colliders = GetComponentsInChildren<Collider>();
        m_SMRs = GetComponentsInChildren<SkinnedMeshRenderer>();

        MonsterHide();
    }

    // Start is called before the first frame update
    void Start()
    {
        CurHp = MaxHp;
        NetHp = MaxHp;        
        gameObject.tag = "Monster";
        traceDist = 10.0f;
        attackDist = 1.8f;

        monsterTr = gameObject.GetComponent<Transform>();
        monsterTr.position = new Vector3(0, 0, -40);    // 초기 위치를 맵 밖에 준다.(위치 동기화 안될 경우, 몬스터가 다른곳에 스폰되는 것을 막는다.)        
        nvAgent = this.gameObject.GetComponentInChildren<NavMeshAgent>();
        animator = this.gameObject.GetComponentInChildren<Animator>();

        GameMgrRef = GameObject.Find("MultiGameMgr").GetComponent<MutiGameMgr>();

        monsterState = MonsterState.patrol;
    }

    // Update is called once per frame
    void Update()
    {
        ViewTime -= Time.deltaTime;
        if (ViewTime <= 0.0f)
        {
            MonsterView();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            CheckMonsterState();
            MonsterAction();
        }
        else
        {
            Remote_Move();
            Remote_AnimState();
            Remote_Hit();
        }

        MonsterObjectDestroy();
    }

    void CheckMonsterState()
    {
        if (isDie == false)
        {
            if (MutiGameMgr.g_GameState == MutiGameMgr.GameState.End)
            {
                MonsterDie();
            }

            float AggroPlayerdist = 0.0f;
            //플레이어 찾기
            if (isHit == false) // 때리는 거 없이 플레이어를 찾아가는 경우
            {
                GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in Players)
                {
                    // 몬스터와 플레이어 사이의 거리 측정
                    AggroPlayerdist = Vector3.Distance(player.transform.position, monsterTr.position);
                    if (AggroPlayerdist <= attackDist)
                    {
                        //거리에 들어온 가장 먼저 인지된 플레이어를 공격한다.(이경우에는 가장 가까운 플레이어)
                        m_AggroTarget = player;
                        break;
                    }
                }
            }
            else // 플레이어가 때렸을 경우, 플레이어와의 거리를 측정한다.
            {
                if (m_AggroTarget != null)
                {
                    // 몬스터와 플레이어 사이의 거리 측정
                    AggroPlayerdist = Vector3.Distance(m_AggroTarget.transform.position, monsterTr.position);
                }//if (m_AggroTarget != null) 
            }
            //플레이어 찾기

            if (m_AggroTarget != null)
            {
                AggroPlayerStateRef = m_AggroTarget.GetComponent<MultiPlayerInfoCtrl>();
                if (AggroPlayerdist <= attackDist && AggroPlayerStateRef.CurHp > 0)
                {
                    monsterState = MonsterState.attack;
                }
                else if ((AggroPlayerdist <= traceDist || isHit == true) && AggroPlayerStateRef.CurHp > 0)
                {
                    monsterState = MonsterState.trace;
                }
                else
                {
                    monsterState = MonsterState.patrol;
                }
            }//if (m_AggroTarget != null)
            else // 몬스터가 타겟을 못찾으면 패트롤
            {
                monsterState = MonsterState.patrol;
            }
            //monsterState = MonsterState.patrol;
        } // while (!isDie) 
    }

    void MonsterAction()
    {
        if (isDie == false)
        {
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsAttack", false);
                    break;
                case MonsterState.patrol:
                    animator.SetBool("IsAttack", false);
                    if (monsterPS == MonsterPatrolState.Moving)
                    {
                        //꼭지점 1 : (32, 0, 3)
                        //꼭지점 2 : (21, 0, 3)
                        //꼭지점 3 : (21, 0, -11)
                        //꼭지점 4 : (32, 0, -11)
                        //사각형 모양의 구역에서 임의의 지역을 움직일 수 있도록 설정
                        moveTime -= Time.deltaTime;
                        if (moveTime <= 0.0f)
                        {
                            moveVec.x = Random.Range(-11.0f, 2.0f);
                            moveVec.z = Random.Range(-32.0f, -20.0f);
                            nvAgent.destination = moveVec;
                            moveTime = (moveVec - transform.position).magnitude / nvAgent.speed;
                            animator.SetBool("IsTrace", true);
                            nvAgent.isStopped = false;
                            monsterPS = MonsterPatrolState.Stop;
                        }
                    }
                    else if (monsterPS == MonsterPatrolState.Stop)
                    {
                        stopTimeUse -= Time.deltaTime;
                        if (stopTimeUse <= 0.0f)
                        {
                            stopTimeUse = stopTime;
                            animator.SetBool("IsTrace", false);
                            nvAgent.isStopped = true;
                            monsterPS = MonsterPatrolState.Moving;
                        }
                    }
                    break;
                case MonsterState.trace:
                    if (m_AggroTarget != null)
                    {
                        nvAgent.destination = m_AggroTarget.transform.position;
                    }
                    nvAgent.isStopped = false;
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsAttack", false);
                    break;
                case MonsterState.attack:
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", true);
                    break;
            } // switch (monsterState)            
        }
    }

    void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        // 데칼 프리펩 부분
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        Destroy(blood2, 5.0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (NetHp <= 0)
            return;

        if (collision.gameObject.tag == "Bullet")
        {
            //플레이어 찾기
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in Players)
            {
                if (player.gameObject.GetComponent<MultiPlayerController>().pv.Owner.ActorNumber
                    == collision.gameObject.GetComponent<MultiBulletCtrl>().MakerId)
                {                    
                    m_AggroTarget = player;
                    break;
                }
            }
            //플레이어 찾기

            NetHp -= collision.gameObject.GetComponent<MultiBulletCtrl>().damage;            
            if (NetHp > 0)
            {
                AttackFinishPlayerID = collision.gameObject.GetComponent<MultiBulletCtrl>().MakerId;
                pv.RPC("MonsterTakeDamage", RpcTarget.All, null);                
            }
            else
            {
                pv.RPC("MonsterDie", RpcTarget.All, null);
            }

            CreateBloodEffect(collision.transform.position);
            Destroy(collision.gameObject);
        }//if (collision.gameObject.tag == "Bullet")

        if (collision.gameObject.tag == "Missile")
        {
            NetHp -= collision.gameObject.GetComponent<MissileCtrl>().Damage;
            if (NetHp > 0)
            {
                pv.RPC("MonsterTakeDamage", RpcTarget.All, null);
            }
            else
            {
                AttackFinishPlayerID = collision.gameObject.GetComponent<MultiBulletCtrl>().MakerId;
                pv.RPC("MonsterDie",RpcTarget.All, null);
            }

            CreateBloodEffect(collision.transform.position);
        }//if (collision.gameObject.tag == "Bullet")
    }

    [PunRPC]
    void MonsterDie()
    {        
        gameObject.tag = "Untagged";
        StopAllCoroutines();

        nvAgent.isStopped = true;
        CurHp = 0;
        animator.SetTrigger("IsDead");
        isDie = true;

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        monsterState = MonsterState.die;

        if (pv.IsMine == true) 
        {
            CreateItem();
        }

        // 게임 매니저에 송신
        GameMgrRef.SendKillMaster();        
    }

    // 몬스터와 관련된 게임 오브젝트들을 디스트로이한다.
    void MonsterObjectDestroy()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            if (monsterState == MonsterState.die)
            {
                monsterDestroyTime -= Time.deltaTime;
                if (monsterDestroyTime <= 0)
                {
                    monsterDestroyTime = 2.0f;
                    PhotonNetwork.Destroy(gameObject);
                }
            }//if (monsterState == MonsterState.die)
            else if (MutiGameMgr.g_GameState == MutiGameMgr.GameState.End) 
            { // 게임이 클리어될 시 사망처리
                monsterState = MonsterState.die;
                PhotonNetwork.Destroy(gameObject);
            }
        }//if (PhotonNetwork.IsMasterClient == true)         
    }

    // 포톤 네트워크 동기화 관련
    void Remote_Move()
    {
        if ((monsterTr.position - currPos).magnitude > 5.0f)
        {
            monsterTr.position = currPos;
        }
        else
        {
            monsterTr.position = Vector3.Lerp(monsterTr.position, currPos, Time.deltaTime * 10.0f);
        }
        monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, currRot, Time.deltaTime * 10.0f);
    }

    void Remote_AnimState()
    {
        switch (monsterState)
        {
            case MonsterState.idle:
                {
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsDead", false);
                }
                break;
            case MonsterState.attack:
                {
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsAttack", true);
                    animator.SetBool("IsDead", false);
                }
                break;
            case MonsterState.patrol:
                {
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsDead", false);
                }
                break;
            case MonsterState.trace:
                {
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsDead", false);
                }
                break;
            case MonsterState.hit:
                {
                    animator.SetTrigger("IsHit");
                }
                break;
            case MonsterState.die:
                {
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsDead", true);
                }
                break;
        }
    }

    void Remote_Hit() 
    {
        // 맞았는지 탐지하기
        if (isHit == true || monsterState == MonsterState.hit ) 
        {
            animator.SetTrigger("IsHit");
            CreateBloodEffect(transform.position);
            isHit = false;
        }
    }

    void MonsterHide()
    {
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }

        if (m_SMRs != null)
        {
            foreach (SkinnedMeshRenderer smr in m_SMRs)
            {
                smr.enabled = false;
            }
        }
    }

    void MonsterView() // 몬스터가 생성시 포톤 뷰의 위치 동기화 문제로 인해 껐다 켜는 로직을 작성한다.
    {
        if (monsterState == MonsterState.die)
            return;
        
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }
        }

        if (m_SMRs != null)
        {
            foreach (SkinnedMeshRenderer smr in m_SMRs)
            {
                smr.enabled = true;
            }
        }
    }

    [PunRPC]
    void MonsterTakeDamage() 
    {
        CurHp = NetHp;
        animator.SetTrigger("IsHit");
        monsterState = MonsterState.hit;
        isHit = true;        
        CheckMonsterState();
        MonsterAction();
    }

    // 몬스터 아이템 생성
    void CreateItem() 
    {
        Vector3 a_ItemPos = monsterTr.position;
        a_ItemPos.y = 1.65f; // y위치를 약간 높임

        PhotonNetwork.InstantiateRoomObject("MultiItem", a_ItemPos, Quaternion.identity, 0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(CurHp);
            stream.SendNext(monsterState);
            stream.SendNext(monsterTr.position);
            stream.SendNext(monsterTr.rotation);
        }
        else
        {
            //NetHp = (int)stream.ReceiveNext();
            monsterState = (MonsterState)stream.ReceiveNext();
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();            
        }
    }
}
