using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMgr : MonoBehaviour
{
    public Text m_ScoreTxt = null;
    public Button m_AddScoreBtn = null;
    private Button m_ScoreBtnObj = null;

    public Text m_UserGoldTxt = null;
    public Button m_AddGoldBtn = null;
    public Button m_SubGoldBtn = null;

    public Text m_UserInfoTxt = null;

    private Button m_GoldBtnObj = null;

    public Button m_BackBtn = null;
    
    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        if (m_AddGoldBtn != null)
            m_GoldBtnObj = m_AddGoldBtn;

        if (m_AddGoldBtn != null)
            m_AddGoldBtn.onClick.AddListener(() => 
            {
                GlobalValue.g_Gold += 1500;
                m_UserGoldTxt.text = $"보유 골드({GlobalValue.g_Gold})";
                UpdateGoldCo();
            });

        if (m_AddScoreBtn != null)
            m_ScoreBtnObj = m_AddScoreBtn;

        if (m_SubGoldBtn != null)
            m_SubGoldBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_Gold -= 1500;
                if (GlobalValue.g_Gold <= 0) 
                {
                    GlobalValue.g_Gold = 0;
                }
                m_UserGoldTxt.text = $"보유 골드({GlobalValue.g_Gold})";
                UpdateGoldCo();
            });

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() => 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            });

        if (m_AddScoreBtn != null)
            m_AddScoreBtn.onClick.AddListener(()=> 
            {
                UpdateScoreCo();
                GetMyRanking();
                RefreshRanking();
            });
        
        m_ScoreTxt.text = $"최고 점수({GlobalValue.g_BestScore})";
        m_UserGoldTxt.text = $"보유 골드({GlobalValue.g_Gold})";
        m_UserInfoTxt.text = $"내정보 : 별명({GlobalValue.g_NickName}) : 순위({GlobalValue.g_UserRank})";
    }    

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateScoreCo()
    {
        if (m_ScoreBtnObj.enabled == false)
            return;

        if (GlobalValue.g_Unique_ID == "")
            return;

        GlobalValue.g_BestScore += UnityEngine.Random.Range(5, 11);
        m_ScoreTxt.text = $"최고 점수({GlobalValue.g_BestScore})";
        m_ScoreBtnObj.enabled = false;

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "BestScore",
                    Value = GlobalValue.g_BestScore
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) =>
            {
                m_ScoreBtnObj.enabled = true;
            },
            (error) =>
            {
                m_ScoreBtnObj.enabled = true;
            }
        );
    }
        
    void UpdateGoldCo() 
    {
        if (m_GoldBtnObj.enabled == false)
            return;

        if (GlobalValue.g_Unique_ID == "")
            return;

        // 플레이어 데이터(타이틀)값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            // Permission = UserDataPermission.private 디폴트값
            // Permission = UserDataPermission.public
            // public 공개 설정 : 다른 유저들이 볼 수 있게 하는 옵션
            // private 비공개 설정 : 나만 접근할 수 있는 값의 속성으로 변경
            Data = new Dictionary<string, string>()
            {
                { "UserGold", GlobalValue.g_Gold.ToString() }
            }
        };

        m_GoldBtnObj.enabled = false;
        //PlayFabClientAPI.UpdateUserData(request, UpdateSuccess, UpdateFailure);
        PlayFabClientAPI.UpdateUserData(
                request,
                (result) => 
                {
                    m_GoldBtnObj.enabled = true;
                },
                (eroor) =>
                {
                    m_GoldBtnObj.enabled = true;
                }
            );
    }

    void GetMyRanking() // 나의 등수 불러오기
    {
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
                    GlobalValue.g_UserRank = Presult.Position + 1;
                }

            },
            (error) =>
            {
                Debug.Log("내 등수 불러오기 실패");
            }
        );
    }

    void RefreshRanking() 
    {
        m_UserInfoTxt.text = $"내정보 : 별명({GlobalValue.g_NickName}) : 순위({GlobalValue.g_UserRank})";
    }
}
