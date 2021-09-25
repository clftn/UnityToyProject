using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoController : MonoBehaviour
{
    // 플레이어의 HP
    public int MaxHp = 200;
    public int CurHp = 200;

    GameMgr GmRef;
    PlayerController PlayerConRef;

    // Start is called before the first frame update
    void Start()
    {
        GmRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        PlayerConRef = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Punch")
        {
            CurHp -= other.gameObject.GetComponentInParent<MonsterController>().MonAtt;
            GmRef.HeroHpView(CurHp, MaxHp);
            if (CurHp <= 0)
            {
                Debug.Log($"PlayerHP : {CurHp}");
                CurHp = 0;
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        PlayerConRef.PlayerDie();

        GameMgr.gameState = GameMgr.GameState.GameOver;
        GmRef.GameOverFunc();
    }
}
