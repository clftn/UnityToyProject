using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyRoomSpawnMgr : MonoBehaviour
{
    [HideInInspector] public Button[] m_SpawnBtn = null;
    [HideInInspector] public bool[] m_CheckPoint;
    [HideInInspector] public TowerType[] m_TowerTypeNode;
    public Button[] m_UserSellTower;
    
    //---------------------2021_10_25 추가
    public Image[] m_UserSellTowerCheckImg;
    SpawnBtnMgr m_SpawnBtnMgr = null;

    [Header("===== Tower_Overwarning_Panel =====")]
    public GameObject m_Tower_Overwarning_Panel = null;
    public Button m_Overwarning_Pan_Ok_Btn = null;
    
    [Header("===== Tower_NotSetting_Panel =====")]
    public GameObject m_Tower_NotSetting_Panel = null;
    public Button m_NotSetting_Pan_Ok_Btn = null;
    //---------------------2021_10_25 추가

    public GameObject m_SellTower_Panel = null;
    public Button m_Done_Btn = null;
    public Button m_ReSet_Btn = null;

    int m_MaxSpawn = 0;
    int m_SpawnNum = 0;

    int m_BtnNum = 0;
    Image[] a_CheckImg = null;
    Image m_CheckImg = null;

    public int m_SpawnBtnLenth = 0;
    UserMap m_UserMap = UserMap.MAP1;
    [HideInInspector] public bool OpenMapSet = false;
    [HideInInspector] public string m_strJon = "";

    int m_SpawnPointIdex = 0;
    public Button m_SellTowerCloseBtn = null;

    string UserMapSaveUrl = "";
    [HideInInspector]public string g_Message = "";
    // Start is called before the first frame update
    void Start()
    {
        m_SpawnBtn = this.transform.GetComponentsInChildren<Button>();
        m_SpawnBtnLenth = m_SpawnBtn.Length;
        CheckMapNumber(m_SpawnBtnLenth);
        m_CheckPoint = new bool[m_SpawnBtnLenth];
        m_TowerTypeNode = new TowerType[m_SpawnBtnLenth];

        for (int i = 0; i < m_SpawnBtnLenth; i++)
        {
            m_TowerTypeNode[i] = TowerType.None;
        }

        m_MaxSpawn = m_SpawnBtnLenth / 2;

        for(int i = 0; i < m_SpawnBtnLenth; i++)
        {
            m_CheckPoint[i] = false;
        }

        for (int i = 0; i < m_SpawnBtn.Length; i++)
        {
            int index = i;
            m_SpawnBtn[i].onClick.AddListener(() => 
            {
                BtnClick(index);
            });

        }

        //---------------------2021_10_25 추가
        for (int i = 0; i < m_UserSellTower.Length; i++)
        {
            int index = i;

            m_UserSellTowerCheckImg[index].gameObject.SetActive(true);
            m_UserSellTower[index].enabled = false;

            for (int ii = 0; ii < GlobarValue.g_UserTowerList.Count; ii++)
            {
                int iidex = ii;
                if(GlobarValue.g_UserTowerList[iidex].m_TowerType == (TowerType)index)
                {
                    m_UserSellTowerCheckImg[index].gameObject.SetActive(false);
                    m_UserSellTower[index].enabled = true;
                    break;
                }
            }
        }//---------------------2021_10_25 추가

        for (int i = 0; i < m_UserSellTower.Length; i++)
        {
            int index = i;
            m_UserSellTower[i].onClick.AddListener(() => 
            {
                SellTowerType((TowerType)index);
            });

        }

        if (m_Done_Btn != null)
            m_Done_Btn.onClick.AddListener(DoneBtn);

        if (m_ReSet_Btn != null)
            m_ReSet_Btn.onClick.AddListener(ReSetBtn);

        //---------------------2021_10_25 추가
        if (m_Overwarning_Pan_Ok_Btn != null)
            m_Overwarning_Pan_Ok_Btn.onClick.AddListener(() => 
            {
                m_Tower_Overwarning_Panel.SetActive(false);
            });
        
        if (m_NotSetting_Pan_Ok_Btn != null)
            m_NotSetting_Pan_Ok_Btn.onClick.AddListener(() => 
            {
                m_Tower_NotSetting_Panel.SetActive(false);
            });
        //---------------------2021_10_25 추가

        if (m_SellTowerCloseBtn != null)
            m_SellTowerCloseBtn.onClick.AddListener(() =>
            {
                a_CheckImg = m_SpawnBtn[m_BtnNum].gameObject.transform.GetComponentsInChildren<Image>();
                m_CheckImg = a_CheckImg[1];
                m_SpawnBtnMgr = m_SpawnBtn[m_BtnNum].transform.GetComponent<SpawnBtnMgr>();

                m_SellTower_Panel.SetActive(false);
                m_SpawnBtn[m_BtnNum].GetComponentInChildren<Text>().text = "T";
                m_CheckImg.enabled = false;
                m_CheckPoint[m_BtnNum] = false;
            });

        UserMapSaveUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MapSettingSave.php";
        LoadMapData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BtnClick(int _num)
    {
        m_SpawnBtnMgr = null;
        m_SpawnPointIdex = _num;
        Debug.Log(m_CheckPoint[_num]);
        if (m_SpawnNum == m_MaxSpawn && m_CheckPoint[_num] == false)
        {
            if(m_Tower_Overwarning_Panel != null)
                m_Tower_Overwarning_Panel.SetActive(true);
            return;
        }

        if (OpenMapSet == false)
            return;

        m_BtnNum = _num;
        a_CheckImg = m_SpawnBtn[m_BtnNum].gameObject.transform.GetComponentsInChildren<Image>();
        m_CheckImg = a_CheckImg[1];
        m_SpawnBtnMgr = m_SpawnBtn[m_BtnNum].transform.GetComponent<SpawnBtnMgr>();

        if (m_CheckPoint[m_BtnNum] == false)
        {
            m_SellTower_Panel.SetActive(true);
        }

        else
        {
            //----------------수정
            m_SpawnBtn[m_BtnNum].GetComponentInChildren<Text>().text = "T";
            //----------------수정
            m_CheckImg.enabled = false;
            m_CheckPoint[m_BtnNum] = false;
            m_TowerTypeNode[m_BtnNum] = TowerType.None;
            m_SpawnNum--;
        }
    }

    public void DoneBtn()
    {
        if (OpenMapSet == false)
            return;

        GlobarValue.g_UserMap = m_UserMap;

        if (m_SpawnNum == 0)
        {
            if (m_Tower_NotSetting_Panel != null)
                m_Tower_NotSetting_Panel.SetActive(true);
            return;
        }

        for (int i = 0; i < m_CheckPoint.Length; i++)
        {
            GlobarValue.g_MapList[(int)m_UserMap].m_SpawnPoint[i] = m_CheckPoint[i];
            GlobarValue.g_MapList[(int)m_UserMap].m_TowerType[i] = m_TowerTypeNode[i];
        }

        GlobarValue.g_MapList[(int)m_UserMap].SaveMapInfo();
        Debug.Log("호출");
        UserMapListSaveSend();
    }

    void ReSetBtn()
    {
        if (OpenMapSet == false)
            return;

        Debug.Log("작동");

        if (m_SpawnNum == 0)
        {
            Debug.Log("세팅된 맵이 없습니다.");
            return;
        }

        m_SpawnNum = 0;

        for (int i = 0; i < m_SpawnBtn.Length; i++)
        {
            a_CheckImg = m_SpawnBtn[i].gameObject.transform.GetComponentsInChildren<Image>();
            m_CheckImg = a_CheckImg[1];
            //----------------수정
            m_SpawnBtn[i].GetComponentInChildren<Text>().text = "T";
            //----------------수정
            m_CheckImg.enabled = false;
        }

        Debug.Log((int)m_UserMap);
        for (int i = 0; i < m_CheckPoint.Length; i++)
        {
            m_CheckPoint[i] = false;
            GlobarValue.g_MapList[(int)m_UserMap].m_SpawnPoint[i] = m_CheckPoint[i];
            GlobarValue.g_MapList[(int)m_UserMap].m_TowerType[i] = TowerType.None;
        }

        GlobarValue.g_MapList[(int)m_UserMap].m_SetMapCheck = false;
        GlobarValue.g_MapList[(int)m_UserMap].SaveMapInfo();
        UserMapListSaveSend();
    }

    void SellTowerType(TowerType _Type) 
    {
        Debug.Log(_Type);
        if (m_SellTower_Panel == null && m_SellTower_Panel.activeSelf == false)
            return;

        if (m_CheckImg == null)
            return;

        if (OpenMapSet == false)
            return;

        m_SpawnNum++;
        m_CheckImg.enabled = true;
        m_CheckPoint[m_BtnNum] = true;
        m_TowerTypeNode[m_BtnNum] = _Type;
        //GlobarValue.g_MapList[(int)m_UserMap].m_TowerType[m_BtnNum] = _Type; ////GlobarValue.g_TowerType[m_SpawnNum] = _Type;
        m_SellTower_Panel.SetActive(false);
        m_SpawnBtnMgr.m_TowerName.text = ((int)_Type + 1).ToString();
    }

    void CheckMapNumber(int _index)
    {
        if (OpenMapSet == false)
            return;

        if (_index == 50)
        {
            m_UserMap = UserMap.MAP1;
        }
        else if(_index == 68)
        {
            m_UserMap = UserMap.MAP2;
        }
    }

    //--------------------DB데이터 베이스 저장
    public void UserMapListSaveSend() //새롭게 저장된 유저의 맵정보를 DB에 보낸다.
    {
        JSONObject a_MkJSON = new JSONObject();
        for (int i = 0; i < 3; i++)
        {
            a_MkJSON["MapType"][i] = GlobarValue.g_MapList[i].m_UserMap.ToString();
            a_MkJSON["UserSellSpawn"][i] = GlobarValue.g_MapList[i].m_SaveSpawnPointList;
            a_MkJSON["USerSellTower"][i] = GlobarValue.g_MapList[i].m_SaveTowerList;
            a_MkJSON["USerMapPower"][i] = GlobarValue.g_MapList[i].m_MapPower;
        }
        m_strJon = a_MkJSON.ToString();
        Debug.Log(a_MkJSON.ToString());
        StartCoroutine(SaveMapInfoCo());
    }

    IEnumerator SaveMapInfoCo()
    {
        if (string.IsNullOrEmpty(m_strJon) == true)
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_usernumber", GlobarValue.UserNumber);
        form.AddField("map_list", m_strJon, System.Text.Encoding.UTF8);
        UnityWebRequest a_www = UnityWebRequest.Post(UserMapSaveUrl, form);
        g_Message = a_www.ToString();
        yield return a_www.SendWebRequest();
        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(a_www.downloadHandler.data);
            Debug.Log(a_ReStr);
        }
        else
        {
            Debug.Log(a_www.error);
        }

        GlobarValue.g_MapList[(int)m_UserMap].m_SetMapCheck = true;
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
    //--------------------DB데이터 베이스 저장

    public void LoadMapData()
    {
        Debug.Log("호출");
        ClosePanel();

        for (int i = 0; i < GlobarValue.g_MapList[(int)m_UserMap].m_SpawnPoint.Length; i++)
        {
            if (GlobarValue.g_MapList[(int)m_UserMap].m_SpawnPoint[i] == true)
            {
                m_SpawnNum++;
                a_CheckImg = m_SpawnBtn[i].gameObject.transform.GetComponentsInChildren<Image>();
                m_CheckImg = a_CheckImg[1];
                m_CheckPoint[i] = true;
                m_TowerTypeNode[i] = GlobarValue.g_MapList[(int)m_UserMap].m_TowerType[i];
                m_CheckImg.enabled = true;
                //-----------------------------2021_10_25 추가사항
                m_SpawnBtnMgr = m_SpawnBtn[i].transform.GetComponent<SpawnBtnMgr>();
                if(m_SpawnBtnMgr.m_TowerName != null)
                    m_SpawnBtnMgr.m_TowerName.text = ((int)GlobarValue.g_MapList[(int)m_UserMap].m_TowerType[i] + 1).ToString();
                //-----------------------------2021_10_25 추가사항
            }
        }
    }

    public void ClosePanel()
    {
        for(int i = 0; i < m_SpawnBtnLenth; i++)
        {
            m_CheckPoint[i] = false;
            m_TowerTypeNode[i] = TowerType.None;
            a_CheckImg = m_SpawnBtn[i].gameObject.transform.GetComponentsInChildren<Image>();
            m_CheckImg = a_CheckImg[1];
            m_CheckImg.enabled = false;
            m_SpawnBtnMgr = m_SpawnBtn[i].transform.GetComponent<SpawnBtnMgr>();
            m_SpawnBtnMgr.m_TowerName.text = "T";
        }
    }
}
