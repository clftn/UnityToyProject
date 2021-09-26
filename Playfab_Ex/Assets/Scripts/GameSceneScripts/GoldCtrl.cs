using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCtrl : MonoBehaviour
{
    public PlayerCtrl PlayerRef;
    GoldGeneratorCtrl GoldGRef;
    // 중복 충돌 막기
    GameObject m_CollSvObj = null;
    GameMgr MgrRef = null;

    // Start is called before the first frame update
    void Start()
    {
        GoldGRef = GameObject.Find("GoldGenerator").GetComponent<GoldGeneratorCtrl>();
        MgrRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D Obj)
    {
        if (Obj.gameObject.tag == "Player")
        {
            if (m_CollSvObj != Obj.gameObject) 
            {
                m_CollSvObj = Obj.gameObject;
                GoldDestroy();
            }            
        }
    }

    public void GoldDestroy() 
    {
        MgrRef.Gold += 50;
        GoldGRef.Count--;
        Destroy(this.gameObject);
    }
}
