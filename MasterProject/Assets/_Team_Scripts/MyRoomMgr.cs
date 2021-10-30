using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyRoomMgr : MonoBehaviour
{
    [Header("====== Button ======")]
    public Button m_UnitBtn = null;
    public Button m_DefenseBtn = null;

    [Header("====== UnitPanel ======")]
    public GameObject m_UnitPanel = null;
    public Transform m_UnitViewContent = null;
    public Transform m_TowerViewContent = null;
    public Button m_UnitInfoClosePanelBtn = null;

    [Header("====== UserAttackSettingPanel ======")]
    public Transform m_UnitAttackContent = null;
    public GameObject m_UnitNode = null;
    public GameObject m_TowerNode = null;
    public GameObject m_UnitAttackSettingNode = null;
    public GameObject m_AttackSettingPanel = null;
    public Button m_AttackSettingPanelCloseBtn = null;

    [Header("====== GameStart ======")]
    public Button m_BackBtn = null;
    public GameObject m_StartUserSellMapPanel = null;
    public Button[] m_StartUserSellMapBtn = null;
    public Button m_StartUserSellMapPanelCloseBtn = null;

    string UserMapLoadUrl = "";
    string LoadUnitInfoUrl = "";
    string LoadTowerInfoUrl = "";

    public Sprite[] m_TowerImaSprite = null;

    void Awake()
    {
        //---------------------------------------------------------------------변경된점(2021_10_22)
        GlobarValue.UserNumber = MyInfo.m_No;
        //GlobarValue.MakeUnit();
        LoadUnitInfoUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MyRoomLoadUnitInfo.php";
        LoadTowerInfoUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MyRoomLoadTowerInfo.php";
        UserMapLoadUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MapSettingLoad.php";

        //UnitInfoLoad();
        //TowerInfoLoad();
        MakeAttackSettingNode();
        //---------------------------------------------------------------------변경된점(2021_10_22)
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobarValue.MakeMapSave();
        UserMapListLoad();
        UnitMakeNode();
        TowerMakeNode();

        if (m_UnitBtn != null)
            m_UnitBtn.onClick.AddListener(() =>
            {
                m_UnitPanel.SetActive(true);
                //ToDo Load Data
            });

        if (m_DefenseBtn != null)
            m_DefenseBtn.onClick.AddListener(() =>
            {
                m_AttackSettingPanel.SetActive(true);
                //ToDo Load Data
            });
        //------------------------임시용
        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (m_StartUserSellMapPanelCloseBtn != null)
            m_StartUserSellMapPanelCloseBtn.onClick.AddListener(() =>
            {
                if (m_StartUserSellMapPanel.activeSelf == true)
                {
                    m_StartUserSellMapPanel.SetActive(false);
                }
            });

        for (int i = 0; i < m_StartUserSellMapBtn.Length; i++)
        {
            int index = i;

            if (m_StartUserSellMapBtn[i] != null)
            {
                m_StartUserSellMapBtn[i].onClick.AddListener(() =>
                {
                    StartBtnClick(index);
                });
            }
        }
        //------------------------임시용

        if (m_UnitInfoClosePanelBtn != null)
            m_UnitInfoClosePanelBtn.onClick.AddListener(() =>
            {
                m_UnitPanel.SetActive(false);
            });

        if (m_AttackSettingPanelCloseBtn != null)
            m_AttackSettingPanelCloseBtn.onClick.AddListener(() =>
            {
                m_AttackSettingPanel.SetActive(false);
            });

    }

    // Update is called once per frame
    void Update()
    {

    }

    void UnitMakeNode()
    {
        for (int i = 0; i < GlobarValue.g_UnitListInfo.Count; i++)
        {
            GameObject a_Node = Instantiate(m_UnitNode);
            UnitNode a_UnitNode = a_Node.GetComponent<UnitNode>();
            UnitType a_Utype = GlobarValue.g_UnitListInfo[i].m_UnitType;
            a_UnitNode.m_UnitImg.sprite = GlobarValue.g_UnitListInfo[i].SetSprite(a_Utype);
            a_UnitNode.m_UnitImg.sprite = GlobarValue.g_UnitListInfo[i].m_UnitSpr;
            a_UnitNode.m_UnitNumber = i;
            a_UnitNode.m_NameText.text = GlobarValue.g_UnitListInfo[i].m_UnitName;
            a_Node.transform.SetParent(m_UnitViewContent, false);
        }
    }    
    
    void TowerMakeNode()
    {
        for (int i = 0; i < GlobarValue.g_UserTowerList.Count; i++)
        {
            GameObject a_Node = Instantiate(m_TowerNode);
            TowerNode a_TowerNode = a_Node.GetComponent<TowerNode>();
            a_TowerNode.m_UnitNumber = i;
            Debug.Log((int)GlobarValue.g_UserTowerList[i].m_TowerType);
            a_Node.GetComponent<Image>().sprite = m_TowerImaSprite[(int)GlobarValue.g_UserTowerList[i].m_TowerType];
            //a_TowerNode.m_NameText.text = GlobarValue.g_UserTowerList[i].m_TowerName;
            a_Node.transform.SetParent(m_TowerViewContent, false);
        }
    }

    void MakeAttackSettingNode() //
    {
        for (int i = 0; i < GlobarValue.g_UnitListInfo.Count; i++)
        {
            GameObject a_Node = Instantiate(m_UnitAttackSettingNode);
            UnitAttackSetting_Node a_UnitNode = a_Node.GetComponent<UnitAttackSetting_Node>();
            a_UnitNode.m_UnitNumber = i;
            a_UnitNode.m_UniqueNum = GlobarValue.g_UnitListInfo[i].m_UniqueID;
            a_UnitNode.m_ItemTypeNumber = (int)GlobarValue.g_UnitListInfo[i].m_UnitType + 1; // DB에 저장할 데이터는 여기서 + 1 해준다.
            a_UnitNode.m_UnitName.text = GlobarValue.g_UnitListInfo[i].m_UnitName;
            a_UnitNode.m_UnitUseableCount = GlobarValue.g_UnitListInfo[i].m_UnitUseable;
            a_UnitNode.SetItem(GlobarValue.g_UnitListInfo[i].m_UnitSpr);
            a_Node.transform.SetParent(m_UnitAttackContent, false);
        }
    }

    void StartBtnClick(int _index)//임시 함수
    {
        if (GlobarValue.g_MapList[_index].m_SetMapCheck == false)
        {
            Debug.Log("선택한 맵은 타워세팅이 되어 있지 않습니다.");
            return;
        }

        if (_index == (int)UserMap.MAP1)
        {
            GlobarValue.g_UserMap = (UserMap)_index;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMapSetting");
        }
        else if (_index == (int)UserMap.MAP2)
        {
            GlobarValue.g_UserMap = (UserMap)_index;
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGameMap2");
        }
    }

    bool MapSetCheck()
    {
        bool a_Check = false;

        for (int i = 0; i < GlobarValue.g_MapList.Count; i++)
        {
            if (GlobarValue.g_MapList[i].m_SetMapCheck == true)
            {
                a_Check = true;
            }
        }

        return a_Check;
    }

    //--------------------DB데이터 베이스 로드
    void UserMapListLoad()
    {
        StartCoroutine(LoadMapInfoCo());
    }

    IEnumerator LoadMapInfoCo()
    {
        if (GlobarValue.UserNumber < 0)
        {
            yield break;            //로그인 실패 상태라면 그냥 리턴
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_usernumber", GlobarValue.UserNumber);

        UnityWebRequest a_www = UnityWebRequest.Post(UserMapLoadUrl, form);
        yield return a_www.SendWebRequest();
        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(a_www.downloadHandler.data);
            if(a_ReStr.Contains("MapLoadDone") == true)
                LoadDataSetUserMap(a_ReStr);
        }
        else
        {
            Debug.Log(a_www.error);
        }

        for (int i = 0; i < 2; i++)
        {
            GlobarValue.g_MapList[i].SaveMapInfo();
        }
    }

    void LoadDataSetUserMap(string strJosnData)
    {
        if (strJosnData.Contains("MapList") == false)
            return;

        var N = JSON.Parse(strJosnData);
        int UserMapNum = 0;
        for (int i = 0; i < 3; i++)
        {
            if (N["MapList"][0]["maptype"][i] == null || N["MapList"][0]["userselltower"][i] == null || N["MapList"][0]["towertype"][i] == null)
                break;

            string _map = N["MapList"][0]["maptype"][i];
            string _userselltower = N["MapList"][0]["userselltower"][i];
            string _towertype = N["MapList"][0]["towertype"][i];
            string[] _userSetTowerList = _userselltower.Split(' ');
            string[] _TowerTyep = _towertype.Split(' ');
            int ListNum = _userSetTowerList.Length - 1;
            UserMapNum = 0;
            if (_map == UserMap.MAP1.ToString())
            {
                UserMapNum = (int)UserMap.MAP1;
            }
            else if (_map == UserMap.MAP2.ToString())
            {
                UserMapNum = (int)UserMap.MAP2;
            }
            else if (_map == UserMap.MAP3.ToString())
            {
                UserMapNum = (int)UserMap.MAP3;
            }

            GlobarValue.g_MapList[UserMapNum].m_MapPower = N["MapList"][0]["mapPower"][i].AsInt;

            if (_userSetTowerList.Length < 10 || _TowerTyep.Length < 10)
            {
                GlobarValue.g_MapList[UserMapNum].m_SetMapCheck = false;
                continue;
            }

            int a_CheckTower = 0;
            for (int ii = 0; ii < GlobarValue.g_MapList[UserMapNum].m_SpawnPoint.Length; ii++)
            {
                if (_userSetTowerList[ii] == "0")
                {
                    GlobarValue.g_MapList[UserMapNum].m_SpawnPoint[ii] = true;
                    a_CheckTower++;
                }
                else
                {
                    GlobarValue.g_MapList[UserMapNum].m_SpawnPoint[ii] = false;
                }
            }

            for (int ii = 0; ii < GlobarValue.g_MapList[UserMapNum].m_TowerType.Length; ii++)
            {
                int _TypeCint = int.Parse(_TowerTyep[ii]);

                GlobarValue.g_MapList[UserMapNum].m_TowerType[ii] = (TowerType)_TypeCint;
            }

            GlobarValue.g_MapList[UserMapNum].m_MpaSetTower = a_CheckTower;
            if (a_CheckTower == 0)
                GlobarValue.g_MapList[UserMapNum].m_SetMapCheck = false;
            else
                GlobarValue.g_MapList[UserMapNum].m_SetMapCheck = true;
        }//for (int i = 0; i < 3; i++)

    }
    //--------------------DB데이터 베이스 로드
}
