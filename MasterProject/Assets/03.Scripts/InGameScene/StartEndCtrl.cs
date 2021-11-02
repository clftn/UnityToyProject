using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum GameState
{
    GS_Ready,
    GS_Playing,
    GS_GameEnd,
}

public class StartEndCtrl : MonoBehaviour
{
    public static StartEndCtrl Inst = null;

    public GameState g_GameState = GameState.GS_Ready;

    public Text m_StartCountTxt = null;
    float m_WaitTime = 4.0f;

    public Text m_PlayTimeTxt = null;
    float m_PlayTime = 120.1f;

    [Header("게임 종료 판넬")]
    public GameObject m_GameEndPanel = null;
    public Text m_UITitleTxt = null;
    public Text m_RemainingTimeTxt = null;
    public Text m_GoldTxt = null;
    public Button m_RetryBtn = null;
    public Button m_GoToLobbyBtn = null;

    string UpdateRecordUrl = "http://pmaker.dothome.co.kr/TeamProject/InGameScene/UpdateRecord.php";
    string strWinLose = "";
    bool isUpdate = false;
    bool isUpdateComplete = false;

    void Start()
    {
        //m_GoToLobbyBtn.gameObject.SetActive(false);
        Inst = this;
        m_PlayTimeTxt.gameObject.SetActive(false);
        m_GameEndPanel.SetActive(false);

        if (g_GameState == GameState.GS_Ready && m_StartCountTxt != null)
        {
            m_StartCountTxt.gameObject.SetActive(true);
            m_StartCountTxt.text = m_WaitTime.ToString();
        }

        if (m_RetryBtn != null)
            m_RetryBtn.onClick.AddListener(ReTry);

        if (m_GoToLobbyBtn != null)
            m_GoToLobbyBtn.onClick.AddListener(GotoLobby);
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 준비시간일때
        if (g_GameState == GameState.GS_Ready && m_StartCountTxt != null)
            ReadyStateFunc();
        // 게임중일때
        if (g_GameState == GameState.GS_Playing && m_PlayTimeTxt != null)
            PlayingStateFunc();
        // 게임이 끝났을 때
        if (g_GameState == GameState.GS_GameEnd)
        {
            m_PlayTimeTxt.gameObject.SetActive(false);

            if (0 < m_PlayTime)
            {
                strWinLose = "win";
                WinAndLose("승리");
            }
            else
            {
                strWinLose = "lose";
                WinAndLose("패배");
            }

            GameEndCall(strWinLose);

            m_GoToLobbyBtn.gameObject.SetActive(isUpdateComplete);

        }
        // Debug.Log(g_GameState);
    }

    string userId = "1";

    public void GameEndCall(string a_StrWL)
    {
        if (isUpdate == false)
        {
            isUpdate = true;

            if (a_StrWL.Contains("win") == true)
            {
                GameMgr.Inst.GoldTextSett(300);
                StartCoroutine(UpdateRecordCo(MyInfo.m_No, "win", GameMgr.Inst.m_GetGold));
                StartCoroutine(UpdateRecordCo(GlobalValue.SO_Info.m_No, "lose", 0));
            }
            else if (a_StrWL.Contains("lose") == true)
            {
                StartCoroutine(UpdateRecordCo(MyInfo.m_No, "lose", GameMgr.Inst.m_GetGold));
                StartCoroutine(UpdateRecordCo(GlobalValue.SO_Info.m_No, "win", 0));
            }
        }
    }

    IEnumerator UpdateRecordCo(int a_UserNo, string a_StrWL, int a_GetGold)
    {
        int a_Gold = MyInfo.m_Gold + a_GetGold;
        WWWForm form = new WWWForm();
        form.AddField("Input_user", a_UserNo);
        form.AddField("Input_Record", a_StrWL, System.Text.Encoding.UTF8);
        form.AddField("Input_Gold", a_Gold);

        UnityWebRequest a_www = UnityWebRequest.Post(UpdateRecordUrl, form);
        yield return a_www.SendWebRequest(); // 응답이 올때까지 대기

        if (a_www.error == null) // 에러가 나지 않는다면
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            if (sz.Contains("Update_Record") == true)
            {
                yield break;
            }
            isUpdateComplete = true;
        }
    }

    // 게임 준비시 카운트 함수
    void ReadyStateFunc()
    {
        if (0 < m_WaitTime)
        {
            if (m_StartCountTxt != null)
                m_StartCountTxt.text = ((int)
                        m_WaitTime).ToString();

            m_WaitTime = m_WaitTime - Time.deltaTime;
        }
        else
        {
            m_StartCountTxt.gameObject.SetActive(false);
            g_GameState = GameState.GS_Playing;
        }
    }

    // 게임중 상태 함수
    void PlayingStateFunc()
    {
        if (0 < m_PlayTime)
        {
            m_PlayTimeTxt.gameObject.SetActive(true);
            m_PlayTimeTxt.text = m_PlayTime.ToString("F1");
            m_PlayTime = m_PlayTime - Time.deltaTime;
        }
        if (m_PlayTime <= 0)  // 시간초과로 인한 패배
        {
            g_GameState = GameState.GS_GameEnd;
        }
    }

    void WinAndLose(string a_WL) // 승리시 함수
    {
        m_GameEndPanel.SetActive(true);

        if (a_WL == "승리")
        {
            m_UITitleTxt.color = Color.blue;
            m_UITitleTxt.text = a_WL;
        }
        else if (a_WL == "패배")
        {
            m_UITitleTxt.color = Color.red;
            m_UITitleTxt.text = a_WL;
        }

        m_RemainingTimeTxt.text = "남은시간 : " + m_PlayTime.ToString("F1");
        m_GoldTxt.text = "획득 골드 : + " + GameMgr.Inst.m_GetGold.ToString() + "G";
    }

    void ReTry()
    {
        string map_Str = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(map_Str);
    }
    void GotoLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}