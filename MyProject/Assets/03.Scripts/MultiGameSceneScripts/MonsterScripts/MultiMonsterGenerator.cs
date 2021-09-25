using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiMonsterGenerator : MonoBehaviourPunCallbacks
{    
    public Transform GenPos1;
    public Transform GenPos2;
    public Transform GenPos3;

    // 몬스터를 발생시킬 주기
    public float createTime = 2.0f;
    public float createTimeUse = 2.0f;

    // 게임 종료 시 멈추기 위한 딜레이
    public float EndTime = 10000.0f;

    // 몬스터 수
    int MaxCount = 3;

    // 위치 배열
    Transform[] points;

    // Start is called before the first frame update
    void Start()
    {
        createTime = 2.0f;
        MaxCount = 3;

        points = GameObject.Find("MonsterReSpawnPos").GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        CreateMonsterUpdate();
    }

    void CreateMonsterUpdate() 
    {
        if (PhotonNetwork.IsMasterClient) //방장만 소환하도록
        {
            if (MutiGameMgr.g_GameState != MutiGameMgr.GameState.Ing)
                return;

            createTimeUse -= Time.deltaTime;
            if (createTimeUse <= 0.0f)
            {
                createTimeUse = createTime;
                int monsterCount = (int)GameObject.FindGameObjectsWithTag("Monster").Length;
                if (monsterCount < MaxCount)
                {
                    int idx = Random.Range(1, points.Length);
                    PhotonNetwork.InstantiateRoomObject("MultiZombie", points[idx].position, points[idx].rotation, 0);
                }//if (monsterCount <= MaxCount)
            }//if (createTimeUse <= 0.0f) 
        }//if (PhotonNetwork.IsMasterClient) 
    }
}
