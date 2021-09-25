using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiPlayerInfoCtrl : MonoBehaviourPunCallbacks
{
    // 플레이어의 HP
    public int MaxHp = 200;
    public int CurHp = 200;
    MultiPlayerController PlayerConRef;
    MutiGameMgr gameMgrRef;
    CapsuleCollider PlayerCol = null;
    Rigidbody PlayerRg = null;

    internal PhotonView pv = null;

    void Awake() 
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerConRef = GetComponent<MultiPlayerController>();
        gameMgrRef = GameObject.Find("MultiGameMgr").GetComponent<MutiGameMgr>();
        PlayerCol = GetComponent<CapsuleCollider>();
        PlayerRg = GetComponent<Rigidbody>();

        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Punch")
        {
            CurHp -= other.gameObject.GetComponentInParent<MultiMonsterCtrl>().MonAtt;
            if (CurHp <= 0)
            {
                CurHp = 0;
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        if (PlayerCol != null) 
        {
            PlayerCol.enabled = false;  // 계속 충돌하는 것을 방지한다.
        }

        if (PlayerRg != null) 
        {
            PlayerRg.useGravity = false; // 시체가 바닥으로 꺼지는 걸 방지
        }
        PlayerConRef.PlayerDie();
        if (gameMgrRef != null) 
        {
            gameMgrRef.PlayerDieSend();
        }
    }//void PlayerDie()

    [PunRPC]
    public void GetMineral(int value) 
    {
        if (pv.IsMine == false)
            return;

        gameMgrRef.CurrentGetmineral += value;
        gameMgrRef.GetMineral();
    }
}