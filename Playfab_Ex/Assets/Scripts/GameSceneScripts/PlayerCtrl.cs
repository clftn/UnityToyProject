using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour
{
    public GameObject StartPos;

    Rigidbody2D rigid2D;
    Animator animator;
    float jumpForce = 350.0f;
    float walkForce = 30.0f;    
    float maxWalkSpeed = 2.0f;
    int key = 0;
    float speedx = 0.0f;    

    // 주인공 상태값
    [HideInInspector] public int Maxlife = 3;
    [HideInInspector] public int life = 3;
    [HideInInspector] public int MaxShield = 1;
    [HideInInspector] public int Shield = 1;
    int shotDir = 1;    // 처음 시작이 오른쪽을 본다.
    private bool isInfinite = false;    // 무적 시간 동작
    private float infiniteTime = 2.0f; // 무적 시간(부딪쳤을 시)
    private float infiniteTimeUse = 2.0f;
    float infiniteBlinkTime = 0.09f;     // 0.3초마다 깜박이게
    float infiniteBlinkTimeUse = 0.09f;
    bool isAlphaZero = false;    
    Color OriginColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    Color AlphaZeroColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);    

    // 중복 충돌 막기
    GameObject m_CollSvObj = null;
    // 주인공이 바닦에 있을 경우
    [HideInInspector] public bool isfloor = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        
        if (GameMgr.g_SkillStep == SkillStep.Step1 || GameMgr.g_SkillStep == SkillStep.Step2)
        {
            MaxShield = 1;
            Shield = MaxShield;
        }
        else if (GameMgr.g_SkillStep == SkillStep.Step3)
        {
            MaxShield = 1;
            Shield = MaxShield;
            Maxlife = 4;
            life = Maxlife;
        }
        else if (GameMgr.g_SkillStep == SkillStep.Step31)
        {
            MaxShield = 1;
            Shield = MaxShield;
            Maxlife = 5;
            life = Maxlife;
        }
        else if (GameMgr.g_SkillStep == SkillStep.Step4 ||
            GameMgr.g_SkillStep == SkillStep.Step5      ||
            GameMgr.g_SkillStep == SkillStep.Step6) 
        {
            MaxShield = 1;
            Shield = MaxShield;
            Maxlife = 5;
            life = Maxlife;
        }
        else if (GameMgr.g_SkillStep == SkillStep.none)
        {
            MaxShield = 0;
            Shield = MaxShield;
            Maxlife = 3;
            life = Maxlife;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.g_GameState == GameState.GameEnd)
            return;

        if (isInfinite == true) 
        {
            infiniteLogic();
        }        

        // 점프한다.
        if (Input.GetKeyDown(KeyCode.Space) && this.rigid2D.velocity.y == 0)
        {
            this.animator.SetTrigger("JumpTrigger");
            this.rigid2D.AddForce(transform.up * this.jumpForce);
        }

        // 좌우 이동
        key = 0;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) key = 1;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) key = -1;

        // 플레이어 속도
        speedx = Mathf.Abs(this.rigid2D.velocity.x);

        // 스피드 제한
        if (speedx < this.maxWalkSpeed)
        {
            this.rigid2D.AddForce(transform.right * key * this.walkForce);
        }

        // 움직이는 방향에 따라 이미지 반전
        if (key != 0)
        {
            transform.localScale = new Vector3(key, 1, 1);
            shotDir = key;
        }

        // 플레이어 속도에 맞춰 애니메이션 속도를 바꾼다.
        if (this.rigid2D.velocity.y == 0)
        {
            this.animator.speed = speedx / 2.0f;
        }
        else
        {
            this.animator.speed = 1.0f;
        }

        if (transform.position.y < -10)
        {
            SceneManager.LoadScene("GameScene");
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    public void ResetGame() 
    {
        // 피 초기화
        life = Maxlife;

        // 무적 시간 초기화
        isInfinite = false;
        infiniteTimeUse = infiniteTime;
        infiniteBlinkTimeUse = infiniteBlinkTime;
        GetComponent<SpriteRenderer>().color = OriginColor;

        // 위치 초기화
        isfloor = false;
        transform.position = StartPos.transform.position;

        // 스킬 초기화
        if (GameMgr.g_SkillStep == SkillStep.Step1 || GameMgr.g_SkillStep == SkillStep.Step2)
        {
            MaxShield = 1;
            Shield = MaxShield;
        }
        else if (GameMgr.g_SkillStep == SkillStep.Step3)
        {
            MaxShield = 1;
            Shield = MaxShield;
            Maxlife = 4;
            life = Maxlife;
        }
        else if (GameMgr.g_SkillStep == SkillStep.Step31)
        {
            MaxShield = 1;
            Shield = MaxShield;
            Maxlife = 5;
            life = Maxlife;
        }
        else if (GameMgr.g_SkillStep == SkillStep.Step4 ||
            GameMgr.g_SkillStep == SkillStep.Step5 ||
            GameMgr.g_SkillStep == SkillStep.Step6)
        {
            MaxShield = 1;
            Shield = MaxShield;
            Maxlife = 5;
            life = Maxlife;
        }
        else if (GameMgr.g_SkillStep == SkillStep.none)
        {
            MaxShield = 0;
            Shield = MaxShield;
            Maxlife = 3;
            life = Maxlife;
        }
    }

    void OnCollisionEnter2D(Collision2D Obj)
    {
        if (isInfinite == true)
            return;

        if (Obj.gameObject.name == "HitArea") 
        {
            if (m_CollSvObj != Obj.gameObject)  // 중복 제거
            {
                m_CollSvObj = Obj.gameObject;
                Obj.gameObject.GetComponentInParent<MonsterCtrl>().MonsterDestroy();
            }
        }

        if (Obj.gameObject.name == "AttackArea")
        {
            if (m_CollSvObj != Obj.gameObject) 
            {
                m_CollSvObj = Obj.gameObject;
                // 밀려나는 부분
                Vector3 pushDir = this.transform.position - Obj.transform.position;
                pushDir.y += 1.4f;
                pushDir.Normalize();
                this.rigid2D.AddForce(pushDir * 300.0f);

                MonsterCtrl tempMCtrl = Obj.gameObject.GetComponentInParent<MonsterCtrl>();
                if (tempMCtrl != null) 
                {
                    tempMCtrl.MonsterHitPlayer();                    
                }                
                // 무적 시간 부여
                isInfinite = true;    
            }            
        }

        if (Obj.gameObject.tag == "Cloud") 
        {
            isfloor = true;
        }
    }    

    private void infiniteLogic()
    {
        infiniteTimeUse -= Time.deltaTime;
        if (infiniteTimeUse <= 0.0f) // 무적 시간 종료
        {
            m_CollSvObj = null;
            GetComponent<SpriteRenderer>().color = OriginColor;
            isInfinite = false;
            infiniteTimeUse = infiniteTime;
        }
        else 
        {
            infiniteBlinkTimeUse -= Time.deltaTime;
            if (infiniteBlinkTimeUse <= 0.0f)
            {                
                if (isAlphaZero == true)
                {                    
                    GetComponent<SpriteRenderer>().color = AlphaZeroColor;
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = OriginColor;
                }
                isAlphaZero = !isAlphaZero;
                infiniteBlinkTimeUse = infiniteBlinkTime;
            }
        }// else
    }
}
