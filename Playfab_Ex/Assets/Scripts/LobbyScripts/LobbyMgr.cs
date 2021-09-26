using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class LobbyMgr : MonoBehaviour
{
    public Text MyInfo_Text = null;
    public Text Ranking_Text = null;

    public Button m_StoreBtn = null;
    public Button m_GameStartBtn = null;
    public Button m_LogOutBtn = null;

    int m_My_Rank = 0;

    float RestoreTimer = 0.0f;
    public Button RestRk_Btn;
    float DelayGetLB = 3.0f; // 3초 뒤에 리더보드 불러오는 부분

    float ShowMsTimer = 0.0f;
    public Text MessageText;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        Ranking_Text.text = "";

        GetLeaderboard();
        RefreshInfo();

        if (m_LogOutBtn != null) 
        {
            m_LogOutBtn.onClick.AddListener(() => 
            {
                GlobalValue.g_Unique_ID = "";
                GlobalValue.g_NickName = "";
                GlobalValue.g_BestScore = 0;
                GlobalValue.g_Gold = 0;

                PlayFabClientAPI.ForgetAllCredentials();
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
            });
        }

        if (m_GameStartBtn != null) 
        {
            m_GameStartBtn.onClick.AddListener(() =>
            {
                //UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            });
        }

        if (m_StoreBtn != null) 
        {
            m_StoreBtn.onClick.AddListener(()=> 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("StoreScene");
            });
        }

#if AutoRestore
        RestoreTimer = 10.0f;
        if (RestRk_Btn != null)
            RestRk_Btn.gameObject.SetActive(false);    
#else
        if (RestRk_Btn != null)
            RestRk_Btn.onClick.AddListener(RestoreRank);
#endif

    }

    // Update is called once per frame
    void Update()
    {
        if (DelayGetLB > 0.0f)
        {
            DelayGetLB -= Time.deltaTime;
            if (DelayGetLB <= 0.0f)
            {
                GetLeaderboard();
            }
        }

#if AutoRestore
        RestoreTimer -= Time.deltaTime;
        if (RestoreTimer <= 0.0f)
        {
            GetLeaderboard();
            RestoreTimer = 7.0f;
        }        
#else
        // 수동일 경우
        if (RestoreTimer > 0.0f)
        {
            RestoreTimer -= Time.deltaTime;
        }
#endif

        if (ShowMsTimer > 0.0f)
        {
            ShowMsTimer -= Time.deltaTime;
            if (ShowMsTimer <= 0.0f) 
            {
                MessageOnOff(false);
            }
        }
    }

    void RestoreRank()
    {
        if (RestoreTimer > 0.0f)
        {
            MessageOnOff(true, "최소 7초 주기로만 갱신됩니다.");
            return;
        }

        DelayGetLB = 0.0f; // 딜레이 로딩 즉시 취소하고, 수동으로
        GetLeaderboard();

        RestoreTimer = 7.0f;
    }

    void RefreshInfo() 
    {
        if (MyInfo_Text != null) 
        {
            MyInfo_Text.text = $"내정보 : 별명({GlobalValue.g_NickName}) : 순위({m_My_Rank}) : 점수({GlobalValue.g_BestScore}) : 게임머니({GlobalValue.g_Gold}골드)";
        }        
    }

    void GetMyRanking() // 나의 등수 불러오기
    {
        if (GlobalValue.g_Unique_ID == "")
            return;

        // 원래 GetLeaderboardAroundPlayer는
        // 특정 PlayerFabId 주변으로 리스트를 불러오는 함수이다.
        var request = new GetLeaderboardAroundPlayerRequest
        {
            //PlayFabId = GlobalValue.g_Unique_ID, // 이 옵션을 통해 해당 ID를 통해 순위값을 가져올 수 있다.
            // 없으면 로그인한 자신의 아이디가 디폴트이다.
            StatisticName = "BestScore",
            MaxResultsCount = 1,    // 한명에 대한 정보를 가져온다.
            //ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true } 
            // 해당 옵션을 통해 이름을 가져올 수 있다.
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            request,
            (result) => 
            {
                foreach (var Presult in result.Leaderboard) 
                {
                    m_My_Rank = Presult.Position + 1;
                }

                RefreshInfo();
            },
            (error) => 
            {
                Debug.Log("내 등수 불러오기 실패");
            }
        );
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,                                          // 0번부터
            StatisticName = "BestScore",                                // 최고점수들을
            MaxResultsCount = 10,                                       // 10명까지
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true                                  // 닉네임 불러오기
            }
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            (result) =>
            {// 랭킹 리스트 받아오기 성공
                Ranking_Text.text = "";
                foreach (var boardResult in result.Leaderboard) 
                {
                    if (boardResult.PlayFabId == GlobalValue.g_Unique_ID)
                        Ranking_Text.text += "<color=#0000ff>";

                    Ranking_Text.text += $"{boardResult.Position + 1}등 : {boardResult.DisplayName} : {boardResult.StatValue}점\n";

                    GlobalValue.g_UserRank = boardResult.Position + 1;
                    // 등수안에 내가 있을 경우 색표시
                    if (boardResult.PlayFabId == GlobalValue.g_Unique_ID)
                        Ranking_Text.text += "</color>";
                }

                GetMyRanking();
            },
            (error) => 
            {
                Debug.Log("리더보드 불러오기 실패");
            }
        );
    }

    void MessageOnOff(bool isOn = true, string Mess = "") 
    {
        if (isOn)
        {
            MessageText.text = Mess;
            MessageText.gameObject.SetActive(true);
            ShowMsTimer = 5.0f;
        }
        else 
        {
            MessageText.text = "";
            MessageText.gameObject.SetActive(false);
        }
    }
}
