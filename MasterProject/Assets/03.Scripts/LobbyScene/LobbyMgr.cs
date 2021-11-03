using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using SimpleJSON;
public class LobbyMgr : MonoBehaviour
{
    public Transform m_ScrollViewContent = null;
    public GameObject m_TextPrefab = null;
    public GameObject m_RandomItem = null;
    public GameObject m_ConfigPanel = null;

    public AudioSource m_BGM = null;
    public Slider m_SoundSlider = null;
    float m_BGMSound;
    public float m_SoundSliderValue = 0.5f;

    public Button m_TestTextCreateBtn;
    public Button m_GameStartBtn;
    public Button m_DefConfigBtn;
    public Button m_StoreBtn;
    public Button m_MyRoomBtn;
    public Button m_ConfigBtn;
    public Button m_LogOutBtn;
    public Button m_ConfigOKBtn;
    public Button m_ConfigCancelBtn;

    public Text m_UserNameText;
    public Text m_UserGoldText;
    public Text m_WinLoseText;
    public Text m_RandomItemNameText;

    public GameObject m_RandomItemPrefab;
    public GameObject[] m_AttRandomItemList;
    public GameObject[] m_DefRandomItemList;

    string LobbyStartUrl = "";
    string RankingSortUrl = "";
    string RandomItemGetUrl = "";

    string m_UserNick = "";
    int m_UserGold = 0;
    int m_UserWin = 0;
    int m_UserDefeat = 0;
    int m_rowsCount;

    string m_RankUserNick = "";
    int m_RankWinCount = 0;
    int m_Ranking = 0;

    string m_GetRandomItemName = "";
    public static int m_GetRandomDBIndex = 0;
    public static int m_isAttack = -1;
    int m_GetRandomItemUserId = 0;
    public static int m_GetRandomItemNo = 0;

    void Start()
    {
        Application.targetFrameRate = 120;

        if (m_TestTextCreateBtn != null)
            m_TestTextCreateBtn.onClick.AddListener(() =>
            {
                if (m_TextPrefab != null)
                {
                    GameObject obj = (GameObject)Instantiate(m_TextPrefab, m_ScrollViewContent);
                }
            });

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("SearchOpponentScene");

            });

        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("StoreScene");
            });

        if (m_LogOutBtn != null)
            m_LogOutBtn.onClick.AddListener(() =>
            {
                LogOutFunc();
                SceneManager.LoadScene("TitleScene");
            });

        if (m_ConfigBtn != null)
            m_ConfigBtn.onClick.AddListener(() =>
            {
                Instantiate(Resources.Load("Config_Canvas"));
            });

        if (m_MyRoomBtn != null)
            m_MyRoomBtn.onClick.AddListener(() => 
            {
                SceneManager.LoadScene("LoadingGoMyRoom");
            });

        LobbyStartUrl = "http://pmaker.dothome.co.kr/TeamProject/LobbyScene/LobbyStart.php";

        StartCoroutine(LobbyStartCo());  
    }

    void Update()
    {
        m_UserNameText.text = MyInfo.m_Nick + "님, 반갑습니다.";
    }

    public void LogOutFunc()
    {
        GlobalValue.m_AttUnitUserItem.Clear();
        GlobalValue.m_DefUnitItem.Clear();

        GlobarValue.UserNumber = 1;
        GlobarValue.g_MapList.Clear();
        GlobarValue.g_UnitListInfo.Clear();
        GlobarValue.g_UserTowerList.Clear();
        GlobarValue.g_VsUserTowerList.Clear();

        MyInfo.m_No = 0;
        MyInfo.m_ID = "";
        MyInfo.m_Nick = "";
        MyInfo.m_Gold = 0;
        MyInfo.m_Win = 0;
        MyInfo.m_Defeat = 0;
    }

    IEnumerator LobbyStartCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", MyInfo.m_No);
        
        UnityWebRequest a_www = UnityWebRequest.Post(LobbyStartUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            Debug.Log(sz);

            var JSONResult = JSON.Parse(sz);

            if(JSONResult["UserInfo"] != null)
            {
                m_UserNameText.text = JSONResult["UserInfo"][0]["UserNick"] + "님, 반갑습니다.";

                m_WinLoseText.text = "전적 : " + JSONResult["UserInfo"][0]["UserWin"].AsInt + "승  " 
                                               + JSONResult["UserInfo"][0]["UserDefeat"].AsInt + "패";

                m_UserGoldText.text = "보유 골드 : " + JSONResult["UserInfo"][0]["UserGold"].AsInt;
                MyInfo.m_Gold = JSONResult["UserInfo"][0]["UserGold"].AsInt;
            }

            if (JSONResult["Ranking"]["RkCount"] != null)
                    m_rowsCount = JSONResult["Ranking"]["RkCount"].AsInt;

            if (JSONResult["Ranking"]["RkList"] != null)
            {
                for (int ii = 0; ii < m_rowsCount; ii++)
                {
                    m_RankUserNick = JSONResult["Ranking"]["RkList"][ii]["UserNick"];
                    m_RankWinCount = JSONResult["Ranking"]["RkList"][ii]["UserWin"].AsInt;
                    m_Ranking = ii + 1;

                    GameObject RandomItemobj = (GameObject)Instantiate(m_TextPrefab, m_ScrollViewContent);
                    RandomItemobj.GetComponent<Text>().text = "\n 이름(" + m_RankUserNick + ")\n" + " 승리(" + m_RankWinCount + ") "
                                                  + +m_Ranking + " 위";

                    if (ii == 0)
                        RandomItemobj.GetComponent<Text>().color = new Color(255, 215, 0);

                    if (ii == 1)
                        RandomItemobj.GetComponent<Text>().color = Color.red;

                    if (ii == 2)
                        RandomItemobj.GetComponent<Text>().color = Color.blue;
                }
            }

                if (JSONResult["RandomItem"] != null)
                {
                    m_GetRandomDBIndex = Random.Range(0, JSONResult["RandomItem"].Count);
                }

            if (JSONResult["RandomItem"][m_GetRandomDBIndex] != null)
            {
                m_GetRandomItemName = JSONResult["RandomItem"][m_GetRandomDBIndex]["ItemName"];
                Debug.Log(m_GetRandomItemName);
                m_RandomItemNameText.text = m_GetRandomItemName;
            }

            if (JSONResult["RandomItem"][m_GetRandomDBIndex]["KindOfItem"] != null)
                LobbyMgr.m_GetRandomItemNo = JSONResult["RandomItem"][m_GetRandomDBIndex]["KindOfItem"].AsInt;

            if (JSONResult["RandomItem"][m_GetRandomDBIndex]["isAttack"] != null)
                LobbyMgr.m_isAttack = JSONResult["RandomItem"][m_GetRandomDBIndex]["isAttack"].AsInt;

            if(m_isAttack == 0)
            {
                m_RandomItemPrefab = m_DefRandomItemList[m_GetRandomItemNo - 1];
                GameObject obj = (GameObject)Instantiate(m_RandomItemPrefab, new Vector3(-0.2f, -2.0f, 0), Quaternion.identity);
            }

            else if (m_isAttack == 1)
            {
                m_RandomItemPrefab = m_AttRandomItemList[m_GetRandomItemNo - 1];
                GameObject obj = (GameObject)Instantiate(m_RandomItemPrefab, new Vector3(-0.2f, -2.0f, 0), Quaternion.identity);
            }

            //Debug.Log("RandomItemName : " + m_GetRandomItemName);
            //Debug.Log("KindOfItem : " + m_GetRandomItemNo);
            //Debug.Log("m_isAttack : " + m_isAttack);
        }
    } 

}

    
