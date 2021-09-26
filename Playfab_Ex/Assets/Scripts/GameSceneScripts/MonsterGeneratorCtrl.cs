using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGeneratorCtrl : MonoBehaviour
{
    public GameObject m_Monster;

    // 생성되는 숫자
    int MaxCount = 8;
    public int Count = 0;    
    private float MaxX;     // 생성될 x위치
    private float MinX;     // 생성될 x위치    
    private float AxisY;    // y위치
    private float MonsterGenTime = 2.0f;   // 몬스터 생성 시간
    private float MonsterGenTimeUse = 2.0f;   // 몬스터 생성 시간
    private float AddSpeedTime = 0.3f;       // 몬스터 속도가 점점 빨라지게

    Transform Parent; // 부모 오브젝트
    // Start is called before the first frame update
    void Start()
    {
        MaxX = 8.0f;
        MinX = -8.0f;
        AxisY = -4.12f;

        Parent = GameObject.Find("MonsterGroup").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.g_GameState == GameState.GameEnd)
            return;

        MonsterGenTimeUse -= Time.deltaTime;
        if (MonsterGenTimeUse <= 0.0f) 
        {
            GenerateMonster();
            MonsterGenTimeUse = MonsterGenTime;
        }
    }

    void GenerateMonster() 
    {
        if (Count >= MaxCount)
            return;

        GameObject newObj = (GameObject)Instantiate(m_Monster);
        // 몬스터 위치 잡기
        Vector3 MonPos = Vector3.zero;
        MonPos.x = Random.Range(MinX, MaxX);
        MonPos.y = AxisY;
        MonPos.z = 0.0f;
        newObj.transform.position = MonPos;

        //몬스터 방향 설정
        int dirtemp = Random.Range(0, 2);
        if (dirtemp == 0)
        {
            newObj.GetComponent<MonsterCtrl>().firDir = -1.0f;
        }
        else
        {
            newObj.GetComponent<MonsterCtrl>().firDir = 1.0f;
        }
        newObj.transform.parent = Parent;

        Count++;
    }

    public void ResetGame() 
    {
        Count = 0;
        MonsterGenTimeUse = MonsterGenTime;
        if (Parent != null) 
        {
            MonsterCtrl[] tempMons = Parent.GetComponentsInChildren<MonsterCtrl>();
            foreach (var tempMon in tempMons)
            {
                Destroy(tempMon.gameObject);
            }
        }        
    }
}
