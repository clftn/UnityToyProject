using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameTestMgr : MonoBehaviour
{
    [Header("====== GameObject ======")]
    public GameObject SpawnPointGroup = null;
    public GameObject[] m_Tower;

    MeshRenderer[] m_TowerSpawnPointList;
    List<GameObject> m_SpawnPointList = new List<GameObject>();

    [HideInInspector] public int m_TankNumbers = 0;
    [HideInInspector] public int m_TowerNumbers = 0;
    int m_UserSellMap = 0;

    static public InGameTestMgr Init = null;

    public Button m_CloseBtn = null;

    private void Awake()
    {
        Init = this;

        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_UserSellMap = (int)GlobarValue.g_UserMap;

        m_TowerSpawnPointList = SpawnPointGroup.transform.GetComponentsInChildren<MeshRenderer>();
        SetSpawnPoint();
        SpawnTower();

        if (m_CloseBtn != null)
            m_CloseBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MyRoom");
            });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetSpawnPoint()
    {
        for (int i = 0; i < m_TowerSpawnPointList.Length; i++)
        {
            m_SpawnPointList.Add(m_TowerSpawnPointList[i].gameObject);
        }
    }

    void SpawnTower()
    {
        for (int i = 0; i < GlobarValue.g_MapList[m_UserSellMap].m_SpawnPoint.Length; i++)
        {
            if (GlobarValue.g_MapList[m_UserSellMap].m_SpawnPoint[i] == true)
            {
                GameObject a_Tower = Instantiate(m_Tower[(int)GlobarValue.g_MapList[m_UserSellMap].m_TowerType[i]]);
                a_Tower.transform.position = m_TowerSpawnPointList[i].transform.position;
                TowerCtrl_Team a_WowerCtrl_Team = a_Tower.GetComponent<TowerCtrl_Team>();
                a_WowerCtrl_Team.m_TowerNumber = i;
                a_WowerCtrl_Team.m_TowerType = GlobarValue.g_MapList[m_UserSellMap].m_TowerType[i];
            }
        }
    }
}
