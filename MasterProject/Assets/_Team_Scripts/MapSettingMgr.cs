using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSettingMgr : MonoBehaviour
{
    [Header("======= GameObject =======")]
    public GameObject m_UserSellMap_Panel = null;
    public GameObject m_TowerSet_Panel = null;
    public GameObject[] m_MAP_OBJ = null;

    [Header("======= Button =======")]
    public Button m_MapSet_Btn = null;
    public Button m_UserSellMapCloseBtn = null;
    public Button m_MapSetCloseBtn = null;
    public Button[] m_UserSellMap_Btn;
    public Button m_MapSetDone_Btn = null;

    public static bool m_MapSetDoneCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        if (m_MapSet_Btn != null)
            m_MapSet_Btn.onClick.AddListener(() =>
            {
                m_UserSellMap_Panel.SetActive(true);
            });

        if (m_UserSellMapCloseBtn != null)
            m_UserSellMapCloseBtn.onClick.AddListener(() =>
            {
                m_UserSellMap_Panel.SetActive(false);
            });

        if (m_MapSetCloseBtn != null)
            m_MapSetCloseBtn.onClick.AddListener(() =>
            {
                m_TowerSet_Panel.SetActive(false);
            });

        for (int i = 0; i < m_UserSellMap_Btn.Length; i++)
        {
            int m_Index = i;
            m_UserSellMap_Btn[i].onClick.AddListener(() =>
            {
                UserSellMapBtnClick(m_Index);
            });
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void UserSellMapBtnClick(int index)
    {
        if (m_UserSellMap_Panel != null)
        {
            m_UserSellMap_Panel.SetActive(true);
            m_TowerSet_Panel.SetActive(true);
            m_UserSellMap_Panel.SetActive(false);
            for (int i = 0; i < m_MAP_OBJ.Length; i++)
            {
                if (i == index)
                {
                    m_MAP_OBJ[i].SetActive(true);
                    MyRoomSpawnMgr m_Mgr = m_MAP_OBJ[i].GetComponent<MyRoomSpawnMgr>();
                    if (m_Mgr.m_SpawnBtnLenth != 0)
                        m_Mgr.LoadMapData();
                    m_Mgr.OpenMapSet = true;
                }
                else
                {
                    m_MAP_OBJ[i].SetActive(false);
                    MyRoomSpawnMgr m_Mgr = m_MAP_OBJ[i].GetComponent<MyRoomSpawnMgr>();
                    m_Mgr.OpenMapSet = false;
                }

                GlobarValue.g_UserMap = (UserMap)index;
            }
        }
    }
}
