using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    public GameObject HitArea;    

    bool isDead = false;
    [HideInInspector] public float MoveSpeed;
    [HideInInspector] public float firDir;
    private float MaxMove;
    private float MinMove;
    SpriteRenderer Renderer;
    PlayerCtrl playerRef;
    MonsterGeneratorCtrl MGCtrlRef;
    GameMgr MgrRef;

    // Start is called before the first frame update
    void Start()
    {
        MoveSpeed = 1.0f;
        
        MaxMove = 8.08f;
        MinMove = -8.08f;

        Renderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        playerRef = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        MGCtrlRef = GameObject.Find("MonsterGenerator").GetComponent<MonsterGeneratorCtrl>();
        MgrRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.g_GameState == GameState.GameEnd)
            return;

        if (playerRef.isfloor == true) 
        {
            HitArea.SetActive(false);
        }
        else if (playerRef.isfloor == false)
        {
            HitArea.SetActive(true);
        }

        if (transform.position.x < MinMove)    // 왼쪽으로 이동 
        {
            firDir = 1;
        }
        else if(transform.position.x >= MaxMove)
        {
            firDir = -1;
        }

        //이동 방향에 따라 이미지의 방향을 바꿔주는 코드
        if (firDir == -1)
            Renderer.flipX = false; //스프라이트 좌우 반전 시키기...
        else
            Renderer.flipX = true;

        transform.position += (firDir * Vector3.right * MoveSpeed * Time.deltaTime);
    }

    public void MonsterDestroy() 
    {
        MgrRef.Score += 50;
        MGCtrlRef.Count--;
        Destroy(this.gameObject);
    }

    public void MonsterHitPlayer() 
    {
        // 스킬 효과 부여
        if (GameMgr.g_SkillStep != SkillStep.none)
        {
            playerRef.Shield -= 1;
            if (playerRef.Shield <= -1) // -1이여야 0일때가 아닌 다음 값에서 피가 깍인다.
            {
                playerRef.Shield = -1;
                playerRef.life -= 1;
            }
        }
        else 
        {
            playerRef.life -= 1;
        }        
        
        if (playerRef.life <= 0) 
        {
            playerRef.life = 0;
            MgrRef.ServerInfoChange();
        }
    }
}
