using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject ZombiePrefab;
    public Transform GenPos1;
    public Transform GenPos2;
    public Transform GenPos3;

    // 몬스터를 발생시킬 주기
    public float createTime = 2.0f;

    // 게임 종료 시 멈추기 위한 딜레이
    public float EndTime = 10000.0f;

    // 몬스터 수
    int MaxCount = 3;

    // 위치 배열
    Transform[] points;
    bool isPlayerDie = false;

    // Start is called before the first frame update
    void Start()
    {
        createTime = 2.0f;
        MaxCount = 3;

        points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();        

        if (points.Length > 0)
        {
            StartCoroutine(CreateMonster());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CreateMonster()
    {
        while (!isPlayerDie)
        {
            if (GameMgr.gameState == GameMgr.GameState.GameEnd)
            {                
                yield return new WaitForSeconds(EndTime); ;
            }

            int monsterCount = (int)GameObject.FindGameObjectsWithTag("Monster").Length;

            if (monsterCount <= MaxCount)
            {
                yield return new WaitForSeconds(createTime);

                int idx = Random.Range(1, points.Length);
                Instantiate(ZombiePrefab, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }            
        }
    }
}
