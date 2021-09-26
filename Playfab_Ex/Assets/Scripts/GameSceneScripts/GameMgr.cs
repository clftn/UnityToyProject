using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    GameIng,
    GameEnd,
}

public enum SkillStep 
{
    none,
    Step1,
    Step2,
    Step3,
    Step31,
    Step4,
    Step5,
    Step6,
}

public class GameMgr : MonoBehaviour
{
    public static GameState g_GameState = GameState.GameIng;
    public static SkillStep g_SkillStep = SkillStep.none;

    [Header("---GameIng---")]
    public Button CloseBtn;
    public Text ScoreText;
    public Text GoldText;
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;
    public Image Heart4;
    public Image Heart5;
    public Image Shield;

    public Button SkillCheckBtn;
    public GameObject SkillPanel;
    public Text SkillNoneText;
    public Text Skill1Text;
    public Text Skill2Text;
    public Text Skill3Text;
    public Text Skill4Text;
    public Text Skill5Text;
    public Text Skill6Text;

    [Header("---GameEnd---")]
    public GameObject GameOverObj;
    public GameObject GameOverPanel;
    public Button LobbyBtn;
    public Button ResetGameBtn;
    public Text GameOverScoreText;
    public Text GameOverBestScoreText;
    public Text GameOverGoldText;
    public Text GameOverUserGoldText;
    Image GameOverPanelRef;
    float PanelOpenSpeed = 1.0f;

    [HideInInspector] public int Score = 0;
    [HideInInspector] public int Gold = 0;
    bool isSkillCheckbtnClick = false;

    PlayerCtrl playerRef;
    GoldGeneratorCtrl GoldGRef;
    MonsterGeneratorCtrl MonGRef;

    // Start is called before the first frame update
    void Awake()
    {
        g_GameState = GameState.GameIng;

        if (CloseBtn != null)
            CloseBtn.onClick.AddListener(()=> 
            {
                // 로비로 갈때, 스코어와 골드 갱신
                ClosingGame();                
                UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            });

        playerRef = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        MonGRef = GameObject.Find("MonsterGenerator").GetComponent<MonsterGeneratorCtrl>();
        GoldGRef = GameObject.Find("GoldGenerator").GetComponent<GoldGeneratorCtrl>();

        GameOverPanelRef = GameOverPanel.GetComponent<Image>();
        ScoreText.text = $"Score : {Score}";
        GoldText.text = $"Gold : {Gold}";

        if (LobbyBtn != null) 
        {
            LobbyBtn.onClick.AddListener(() => 
            {
                // 로비로 갈때, 스코어와 골드 갱신
                ClosingGame();
                UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            });
        }

        if (ResetGameBtn != null) 
        {
            ResetGameBtn.onClick.AddListener(()=> 
            {
                ClosingGame(); // 최신 정보 서버와 통신
                ResetGame();
            });            
        }

        // 스킬 확인 및 적용하기
        SkillCheck();

        // 스킬 버튼
        if (SkillCheckBtn != null)         
            SkillCheckBtn.onClick.AddListener(SkillCheckBtnFunc);        
    }

    void SkillCheckBtnFunc()
    {
        isSkillCheckbtnClick = !isSkillCheckbtnClick;
        if (isSkillCheckbtnClick == true)
        {
            SkillPanel.SetActive(true);
        }
        else 
        {
            SkillPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = $"Score : {Score}";
        GoldText.text = $"Gold : {Gold}";

        UserHealthCheck();

        if (playerRef.life == 0) 
        {
            g_GameState = GameState.GameEnd;
        }

        GameOverLogic();
    }

    void ResetGame() 
    {
        playerRef.ResetGame();
        MonGRef.ResetGame();
        GoldGRef.ResetGame();
        Score = 0;
        Gold = 0;
        g_GameState = GameState.GameIng;
        GameOverPanelRef.fillAmount = 0.0f;
        GameOverObj.SetActive(false);
    }

    void GameOverLogic() 
    {
        // 게임 오버 시 UI 컨트롤
        if (g_GameState == GameState.GameIng)
            return;

        GameOverObj.SetActive(true);
        GameOverPanelRef.fillAmount += Time.deltaTime * PanelOpenSpeed;
        if (GameOverPanelRef.fillAmount >= 1.0f) 
        {
            GameOverPanelRef.fillAmount = 1.0f;
        }

        // 글씨 부분 채우기
        GameOverScoreText.text = $"Your Score : {Score}";
        GameOverBestScoreText.text = $"Best Score : {GlobalValue.g_BestScore}";
        GameOverGoldText.text = $"Get Gold : {Gold}";
        GameOverUserGoldText.text = $"Total Gold : {GlobalValue.g_Gold}";
    }

    void UserHealthCheck() 
    {
        if (playerRef.life == 5)
        {            
            Heart1.gameObject.SetActive(true);
            Heart2.gameObject.SetActive(true);
            Heart3.gameObject.SetActive(true);
            Heart4.gameObject.SetActive(true);
            Heart5.gameObject.SetActive(true);
        }
        else if (playerRef.life == 4)
        {
            Heart1.gameObject.SetActive(true);
            Heart2.gameObject.SetActive(true);
            Heart3.gameObject.SetActive(true);
            Heart4.gameObject.SetActive(true);
            Heart5.gameObject.SetActive(false);
        }
        else if (playerRef.life == 3) 
        {            
            Heart1.gameObject.SetActive(true);
            Heart2.gameObject.SetActive(true);
            Heart3.gameObject.SetActive(true);
            Heart4.gameObject.SetActive(false);
            Heart5.gameObject.SetActive(false);
        }
        else if (playerRef.life == 2)
        {
            Heart1.gameObject.SetActive(true);
            Heart2.gameObject.SetActive(true);
            Heart3.gameObject.SetActive(false);
            Heart4.gameObject.SetActive(false);
            Heart5.gameObject.SetActive(false);
        }
        else if (playerRef.life == 1)
        {
            Heart1.gameObject.SetActive(true);
            Heart2.gameObject.SetActive(false);
            Heart3.gameObject.SetActive(false);
            Heart4.gameObject.SetActive(false);
            Heart5.gameObject.SetActive(false);
        }
        else if (playerRef.life == 0) 
        {
            Heart1.gameObject.SetActive(false);
            Heart2.gameObject.SetActive(false);
            Heart3.gameObject.SetActive(false);
            Heart4.gameObject.SetActive(false);
            Heart5.gameObject.SetActive(false);
        }

        if (g_SkillStep != SkillStep.none) 
        {
            if (playerRef.Shield == 1)
            {                
                Shield.gameObject.SetActive(true);
            }
            else if(playerRef.Shield <= 0)
            {                
                Shield.gameObject.SetActive(false);
            }
        }        
    }

    // 게임 오버 시 
    public void ServerInfoChange() 
    {
        // 값 정산
        if (g_SkillStep == SkillStep.Step2 || g_SkillStep == SkillStep.Step3)
        {
            Gold = Gold * 2;
            GlobalValue.g_Gold += Gold;
            if (GlobalValue.g_BestScore < Score)
            {
                GlobalValue.g_BestScore = Score;
            }
        }
        else if (g_SkillStep == SkillStep.Step4)
        {
            Gold = Gold * 2;
            GlobalValue.g_Gold += Gold;
            Score = Score * 2;
            if (GlobalValue.g_BestScore < Score)
            {
                GlobalValue.g_BestScore = Score;
            }
        }
        else if (g_SkillStep == SkillStep.Step5)
        {
            Gold = Gold * 3;
            GlobalValue.g_Gold += Gold;
            Score = Score * 2;
            if (GlobalValue.g_BestScore < Score)
            {
                GlobalValue.g_BestScore = Score;
            }
        }
        else if (g_SkillStep == SkillStep.Step6)
        {
            Gold = Gold * 4;
            GlobalValue.g_Gold += Gold;
            Score = Score * 4;
            if (GlobalValue.g_BestScore < Score)
            {
                GlobalValue.g_BestScore = Score;
            }
        }
        else if (g_SkillStep == SkillStep.none || g_SkillStep == SkillStep.Step1)
        {
            GlobalValue.g_Gold += Gold;
            if (GlobalValue.g_BestScore < Score)
            {
                GlobalValue.g_BestScore = Score;
            }
        }

        // 서버와 통신
        if (GlobalValue.g_Unique_ID == "")
            return;

        // 골드값 갱신        
        var Goldrequest = new UpdateUserDataRequest()
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

        //PlayFabClientAPI.UpdateUserData(request, UpdateSuccess, UpdateFailure);
        PlayFabClientAPI.UpdateUserData(
                Goldrequest,
                (result) =>
                {
                    Debug.Log("유저골드 갱신 성공");
                },
                (eroor) =>
                {
                    Debug.Log("유저골드 갱신 실패");
                }
            );

        var ScoreRequest = new UpdatePlayerStatisticsRequest
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
            ScoreRequest,
            (result) =>
            {
                Debug.Log("유저스코어 갱신 성공");
            },
            (error) =>
            {
                Debug.Log("유저스코어 갱신 실패");
            }
        );
    }

    void ClosingGame() 
    {        
        // 서버와 통신
        if (GlobalValue.g_Unique_ID == "")
            return;

        // 골드값 갱신        
        var Goldrequest = new UpdateUserDataRequest()
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

        //PlayFabClientAPI.UpdateUserData(request, UpdateSuccess, UpdateFailure);
        PlayFabClientAPI.UpdateUserData(
                Goldrequest,
                (result) =>
                {
                    Debug.Log("유저골드 갱신 성공");
                },
                (eroor) =>
                {
                    Debug.Log("유저골드 갱신 실패");
                }
            );

        var ScoreRequest = new UpdatePlayerStatisticsRequest
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
            ScoreRequest,
            (result) =>
            {
                Debug.Log("유저스코어 갱신 성공");
            },
            (error) =>
            {
                Debug.Log("유저스코어 갱신 실패");
            }
        );
    }

    void SkillCheck()
    {
        bool isSkill = false;
        // 아이템 효과 체크
        foreach (var tempItem in GlobalValue.m_ItDataList)
        {            
            if (tempItem.m_Level > 0) // 아이템을 구입한 상태
            {
                isSkill = true;
                switch (tempItem.m_ItType) 
                {
                    case ItemType.Item_0:
                        {
                            g_SkillStep = SkillStep.Step1;
                            Skill1Text.gameObject.SetActive(true);
                            Shield.gameObject.SetActive(true);
                        }
                        break;

                    case ItemType.Item_1:
                        {
                            g_SkillStep = SkillStep.Step2;
                            Skill2Text.gameObject.SetActive(true);
                        }
                        break;

                    case ItemType.Item_2:
                        {
                            if (tempItem.m_Level == 1)
                            {
                                g_SkillStep = SkillStep.Step3;
                                Skill3Text.text = $"3번 HP 증가 활성화 중({tempItem.m_Level}/2)";
                                Skill3Text.gameObject.SetActive(true);
                            }
                            else if (tempItem.m_Level == 2) 
                            {
                                g_SkillStep = SkillStep.Step3;
                                Skill3Text.text = $"3번 HP 증가 활성화 중({tempItem.m_Level}/2)";
                                Skill3Text.gameObject.SetActive(true);
                            }
                           
                        }
                        break;

                    case ItemType.Item_3:
                        {
                            g_SkillStep = SkillStep.Step4;
                            Skill4Text.gameObject.SetActive(true);
                        }
                        break;

                    case ItemType.Item_4:
                        {
                            g_SkillStep = SkillStep.Step5;
                            Skill5Text.gameObject.SetActive(true);
                        }
                        break;

                    case ItemType.Item_5:
                        {
                            g_SkillStep = SkillStep.Step6;
                            Skill6Text.gameObject.SetActive(true);
                        }
                        break;
                }//switch (tempItem.m_ItType) 
            }// if (tempItem.m_Level > 0) // 아이템을 구입한 상태 
        }// foreach (var tempItem in GlobalValue.m_ItDataList)
        
        if (isSkill == false) 
        {
            g_SkillStep = SkillStep.none;
            SkillNoneText.gameObject.SetActive(true);
        }
    }//void SkillCheck()
}
