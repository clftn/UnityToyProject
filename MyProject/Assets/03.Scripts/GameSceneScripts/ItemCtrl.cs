using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    float LifeTime = 6.0f;
    GameMgr gameMgrRef = null;

    // Start is called before the first frame update
    void Start()
    {
        gameMgrRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0.0f) 
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision colobj)
    {
        if (colobj.gameObject.tag == "Player") 
        {
            // 아이템 획득 부분
            gameMgrRef.GetMineral();

            Destroy(gameObject);
        }
    }
}
