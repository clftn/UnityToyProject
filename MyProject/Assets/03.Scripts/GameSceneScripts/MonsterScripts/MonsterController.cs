using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum MonsterState
    {
        idle,
        patrol,
        trace,
        attack,
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
    private Transform playerTr;
    PlayerInfoController playerStateRef;
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
    float moveTime = 3.0f;
    float stopTime = 3.0f;
    bool isHit = false;

    // 아이템 드랍 클래스
    ItemGenerator itGen;

    // 골드 획득 클래스
    GameMgr gameMgrRef;

    // Start is called before the first frame update
    void Start()
    {
        CurHp = MaxHp;
        gameObject.tag = "Monster";
        traceDist = 10.0f;
        attackDist = 1.8f;

        monsterTr = this.gameObject.GetComponent<Transform>();

        // 싱글 플레이어의 경우, 멀티인 경우에는 Player를 프리펩으로 뽑아야 하기 때문에 권장하지 않는다.
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        playerStateRef = GameObject.FindWithTag("Player").GetComponent<PlayerInfoController>();

        nvAgent = this.gameObject.GetComponentInChildren<NavMeshAgent>();
        animator = this.gameObject.GetComponentInChildren<Animator>();

        gameMgrRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        //nvAgent.destination = playerTr.position;

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            if (GameMgr.gameState == GameMgr.GameState.GameEnd)
            {
                MonsterDie();
            }

            yield return new WaitForSeconds(0.1f);

            // 몬스터와 플레이어 사이의 거리 측정
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);            

            if (dist <= attackDist && playerStateRef.CurHp > 0)
            {
                monsterState = MonsterState.attack;
            }
            else if ((dist <= traceDist || isHit == true) && playerStateRef.CurHp > 0)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                //monsterState = MonsterState.idle;
                monsterState = MonsterState.patrol;
            }

            //monsterState = MonsterState.patrol;
        } // while (!isDie) 
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
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
                        moveVec.x = Random.Range(21.0f, 32.0f);
                        moveVec.z = Random.Range(-11.0f, 3.0f);
                        nvAgent.destination = moveVec;
                        moveTime = (moveVec - transform.position).magnitude / nvAgent.speed;
                        animator.SetBool("IsTrace", true);
                        nvAgent.isStopped = false;
                        monsterPS = MonsterPatrolState.Stop;
                        yield return new WaitForSeconds(moveTime);
                    }
                    else if (monsterPS == MonsterPatrolState.Stop)
                    {
                        animator.SetBool("IsTrace", false);
                        nvAgent.isStopped = true;
                        monsterPS = MonsterPatrolState.Moving;
                        yield return new WaitForSeconds(stopTime);
                    }
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsAttack", false);
                    break;
                case MonsterState.attack:
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            CurHp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            if (CurHp > 0)
            {
                animator.SetTrigger("IsHit");
                isHit = true;                
                StartCoroutine(CheckMonsterState());
                StartCoroutine(MonsterAction());
            }
            else
            {
                // 아이템 드랍(50% 확률로 아이템 드랍)
                itGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();
                itGen.ItemGen(transform.position);
                MonsterDie();                
            }

            CreateBloodEffect(collision.transform.position);
            Destroy(collision.gameObject);
        }
        
        if (collision.gameObject.tag == "Missile")
        {
            CurHp -= collision.gameObject.GetComponent<MissileCtrl>().Damage;
            if (CurHp > 0)
            {
                animator.SetTrigger("IsHit");
                isHit = true;
                StartCoroutine(CheckMonsterState());
                StartCoroutine(MonsterAction());
            }
            else
            {
                // 아이템 드랍(50% 확률로 아이템 드랍)
                itGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();
                itGen.ItemGen(transform.position);
                MonsterDie();
            }

            CreateBloodEffect(collision.transform.position);            
        }
    }

    void MonsterDie()
    {
        gameObject.tag = "Untagged";
        StopAllCoroutines();

        nvAgent.isStopped = true;
        CurHp = 0;
        animator.SetTrigger("IsDead");
        isDie = true;

        gameMgrRef.GetGold(); // 몬스터 죽일 때 골드 획득
        gameMgrRef.QuestCount(); // 퀘스트 카운트

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        Destroy(gameObject, 2.0f);
    }
}
