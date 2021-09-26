using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGeneratorCtrl : MonoBehaviour
{
    public GameObject m_Gold;

    // 생성되는 숫자
    int MaxCount = 3;
    [HideInInspector] public int Count = 0;

    private float MaxX;     // 생성될 x위치
    private float MinX;     // 생성될 x위치    
    private float AxisY;    // y위치
    private float GenTime = 3.0f;   // 몬스터 생성 시간
    private float GenTimeUse = 3.0f;   // 몬스터 생성 시간

    Transform Parent; // 부모 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        MaxX = 8.0f;
        MinX = -8.0f;
        AxisY = -4.12f;

        Parent = GameObject.Find("GoldGroup").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.g_GameState == GameState.GameEnd)
            return;

        GenTimeUse -= Time.deltaTime;
        if (GenTimeUse <= 0.0f) 
        {
            GoldGen();
            GenTimeUse = GenTime;
        }
    }

    void GoldGen() 
    {
        if (Count >= MaxCount)
            return;

        GameObject newObj = (GameObject)Instantiate(m_Gold);
        // 골드 위치 잡기
        Vector3 GoldPos = Vector3.zero;
        GoldPos.x = Random.Range(MinX, MaxX);
        GoldPos.y = AxisY;
        GoldPos.z = 0.0f;
        newObj.transform.position = GoldPos;

        newObj.transform.parent = Parent;

        Count++;
    }

    public void ResetGame() 
    {
        Count = 0;
        GenTimeUse = GenTime;
        if (Parent != null) 
        {
            GoldCtrl[] tempGolds = Parent.GetComponentsInChildren<GoldCtrl>();
            foreach (var tempGold in tempGolds)
            {
                Destroy(tempGold.gameObject);
            }
        }        
    }
}
