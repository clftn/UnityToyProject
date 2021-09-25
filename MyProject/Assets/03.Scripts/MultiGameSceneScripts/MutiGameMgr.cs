using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

public class MutiGameMgr : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum GameState
    {
        first,
        Ing,
        End,
        GameOver
    }
    public static GameState g_GameState;

    public enum GameEndState 
    {
        None,
        MultiLobby,
        Lobby,
        Lab
    }
    public static GameEndState g_GEState = GameEndState.None;

    [Header("---SpawnPos---")]
    public Transform[] SpawnPos; // 주인공들 스폰 장소 넣기

    [Header("---SpawnPos---")]
    public Camera LoadingCam;
    public GameObject LoadingPanel;

    [Header("---InGamePlayUI---")]
    public Text QuestCountText;
    public Image HpImg;
    public Text CurrentGunText;
    public Text TotalBulletCountText;
    public Text BulletText;
    public Text GoldText;
    public Text MineralText;

    [Header("---GameEndUI---")]
    public GameObject GameEndObjs;
    public Text GameEndGetGoldText;
    public Text GameEndGetMineralText;
    public Button GameEndGotoMultiLobbyBtn;
    public Button GameEndGotoLobbyBtn;
    public Button GameEndLabBtn;

    [Header("---GameOverUI---")]
    public GameObject GameOverObjs;
    public Button GameOverGotoMultiLobbyBtn;
    public Button GameOverGotoLobbyBtn;
    public Button GameOverLabBtn;

    // 주인공들 스폰 변수들
    int PlayerCount = 0;

    // 자리배치용 CustomProperties
    ExitGames.Client.Photon.Hashtable m_PlayerSitPosInxProps = new ExitGames.Client.Photon.Hashtable();
    // 플레이어들이 플레이어 준비가끝난는지 체크하는 변수
    ExitGames.Client.Photon.Hashtable m_PlayerReadyProps = new ExitGames.Client.Photon.Hashtable();
    // 죽인 몬스터 수 동기화
    ExitGames.Client.Photon.Hashtable m_QuestCountProps = new ExitGames.Client.Photon.Hashtable();
    // 몬스터를 죽이고, 씬의 상태를 동기화할 변수
    ExitGames.Client.Photon.Hashtable m_GameStateProps = new ExitGames.Client.Photon.Hashtable();
    // 주인공 사망 공유용 변수
    ExitGames.Client.Photon.Hashtable m_PlayerDieProps = new ExitGames.Client.Photon.Hashtable();

    // 게임 매니저 PhotonView
    PhotonView pv = null;

    // 로컬 플레이어 위치 변수
    Vector3 m_LocalPos = Vector3.zero;
    Quaternion m_LocalRot = Quaternion.identity;

    // 플레이어 정보 찾는 변수들
    GameObject m_LocalPlayerRef;
    int LocalPMaxHp = 0;
    int QuestCount = 0;

    // 게임 제어 관련 변수들
    int totalQuestCount = 10;    
    int Remote_gamestate = 1;
    int DiePlayerCount = 0;

    // 총알 관련 변수들
    internal string CurrentGunName = "기본총";
    internal int CurrentBullet = 25;
    internal int ReloadBullet = 25;

    // 골드와 미네랄 관련 변수들
    int CurrentGetgold = 0;
    internal int CurrentGetmineral = 0;

    void Awake()
    {
        LoadingCam.gameObject.SetActive(true);
        LoadingPanel.SetActive(true);
        g_GameState = GameState.first;
        g_GEState = GameEndState.None;

        PlayerCount = 0;
        PhotonNetwork.IsMessageQueueRunning = true; // 씬 로드 후 네트워크 재개
        PhotonNetwork.CurrentRoom.IsOpen = false;   // 게임 시작 후 방을 닫는다.        
        pv = GetComponent<PhotonView>();

        // 마스터 클라이언트는 각 유저의 자리패치를 한다.
        if (PhotonNetwork.IsMasterClient == true)
        {
            PlayerSitPosInxMasterCtrl();
            InitGameStateProp();
            InitPlayerDieCheck();
        }//if (PhotonNetwork.IsMasterClient == true)    
    }

    // Start is called before the first frame update
    void Start()
    {
        // 10마리 죽이면 됨
        totalQuestCount = 10;

        // 총알 관련 변수 초기화
        CurrentBullet = 25;
        ReloadBullet = 25;
        CurrentGunName = "기본총";
        CurrentGunText.text = $"현재총 : {CurrentGunName}";
        TotalBulletCountText.text = "무한";
        BulletText.text = $"{CurrentBullet}/{ReloadBullet}";

        #region 게임 UI 버튼 이벤트 모음

        if (GameEndGotoMultiLobbyBtn != null)
        {
            GameEndGotoMultiLobbyBtn.onClick.AddListener(GameEndGotoMultiLobbyBtnFunc);
        }

        if (GameEndGotoLobbyBtn != null)
        {
            GameEndGotoLobbyBtn.onClick.AddListener(GameEndGotoLobbyBtnFunc);
        }

        if (GameEndLabBtn != null)
        {
            GameEndLabBtn.onClick.AddListener(GameEndLabBtnFunc);
        }

        if (GameOverGotoMultiLobbyBtn != null)
        {
            GameOverGotoMultiLobbyBtn.onClick.AddListener(GameOverGotoMultiLobbyBtnFunc);
        }

        if (GameOverGotoLobbyBtn != null)
        {
            GameOverGotoLobbyBtn.onClick.AddListener(GameOverGotoLobbyBtnFunc);
        }

        if (GameOverLabBtn != null)
        {
            GameOverLabBtn.onClick.AddListener(GameOverLabBtnFunc);
        }

        #endregion

        // 마우스 감춤 처리
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined; // 게임 창 밖으로 마우스가 안나감
        Cursor.visible = false;
        //Esc키를 누르면 커서가 창 밖으로 나가게 할 수 있다.
#endif

        // 마우스 감춤 처리
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (g_GameState == GameState.first)
        {
            // 원격지에서는 아래와 같은 방법으로 스폰
            MultiPlayerSpawnUpdate();
        }

        if (g_GameState == GameState.Ing)
        {
            LoadingCam.gameObject.SetActive(false);
            LoadingPanel.SetActive(false);

            ViewCursor();   // 마우스 제어
            HeroHpView();   // HP 동기화
            ReceiveKillCount(); // 퀘스트용 카운트 동기화
            PlayerDieReceive(); // 죽었는지 살았는지 확인
        }//if (g_GameState == GameState.Ing) 

        ObserveGameState(); // 게임 상태 확인
    }

    #region 키처리 함수들

    void ViewCursor()
    {
        // 알트 누를 시 마우스 보이게 하기
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            // 마우스 뷰 처리
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            // 마우스 감춤 처리
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    #endregion

    // 자리 초기화 하기
    void PlayerSitPosInxMasterCtrl()
    {
        // 자리를 초기화 하기
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            m_PlayerSitPosInxProps.Clear();
            m_PlayerSitPosInxProps.Add("PlayerPos", PlayerCount);
            player.SetCustomProperties(m_PlayerSitPosInxProps);
            PlayerCount++;
        }

        m_PlayerReadyProps.Clear();
        m_PlayerReadyProps.Add("PlayerReady", 1);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_PlayerReadyProps);
    }

    #region 원격지 스폰 관련

    void MultiPlayerSpawnUpdate()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties == null)
            return;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("PlayerReady") == true) // 일단 들어 왔으면,
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["PlayerReady"] == 1)
            {
                MultiPlayerSpawn();
            }
        }
    }

    void MultiPlayerSpawn()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer)
            {
                int a_SitPosInx = (int)player.CustomProperties["PlayerPos"];
                if (a_SitPosInx >= 0 && a_SitPosInx < 3)
                {
                    m_LocalPos = SpawnPos[a_SitPosInx].position;
                    m_LocalRot = SpawnPos[a_SitPosInx].rotation;
                    m_LocalPlayerRef = PhotonNetwork.Instantiate("Player", m_LocalPos, m_LocalRot, 0);
                    LocalPMaxHp = m_LocalPlayerRef.GetComponentInChildren<MultiPlayerInfoCtrl>().MaxHp;
                }
            }
        }//foreach (Player player in PhotonNetwork.PlayerList)

        g_GameState = GameState.Ing;
    }//void PlayerSpawn() 

    #endregion

    #region 로컬플레이어 정보 표시 관련 함수들

    void HeroHpView()
    {
        if (m_LocalPlayerRef != null)
        {
            HpImg.fillAmount = (float)m_LocalPlayerRef.GetComponentInChildren<MultiPlayerInfoCtrl>().CurHp / (float)LocalPMaxHp;
        }
    }

    // 퀘스트 통신관련 

    public void SendKillMaster()
    {
        if (m_QuestCountProps == null)
        {
            m_QuestCountProps = new ExitGames.Client.Photon.Hashtable();
            m_QuestCountProps.Clear();
        }

        if (m_QuestCountProps.ContainsKey("QuestTarget") == false) // 키가 없을 경우
        {
            m_QuestCountProps.Clear();
            m_QuestCountProps.Add("QuestTarget", 1);
            PhotonNetwork.CurrentRoom.SetCustomProperties(m_QuestCountProps);
        }
        else // 키가 있을 경우
        {
            int tempNum = (int)PhotonNetwork.CurrentRoom.CustomProperties["QuestTarget"];
            tempNum++;
            PhotonNetwork.CurrentRoom.CustomProperties["QuestTarget"] = tempNum;
        }
    }

    public void ReceiveKillCount()
    {
        if (m_QuestCountProps == null)
        {
            m_QuestCountProps = new ExitGames.Client.Photon.Hashtable();
            m_QuestCountProps.Clear();
        }

        if (m_QuestCountProps.ContainsKey("QuestTarget") == true) // 키가 있을 경우에만 보여주기
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["QuestTarget"] != null) // 키가 있는데 null임...
            {
                QuestCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["QuestTarget"];
            }
        }
        QuestCountText.text = $"{QuestCount}/{totalQuestCount}";
        // 여기서 골드값을 정산한다.(원래는 몬스터가 죽을 때 계산하면 되지만, 골드를 기획상 몬스터가 죽은 숫자만큼 주므로
        GetGold(QuestCount);
    }

    // 퀘스트 통신관련 

    #region 총알 관련 함수 모음

    internal void GunCount()
    {
        if (CurrentBullet > 0)
        {
            CurrentBullet--;
        }
        else
        {
            CurrentBullet = 0;
        }
        BulletText.text = $"{CurrentBullet}/{ReloadBullet}";
    }

    internal void ReloadGun()
    {
        CurrentBullet = ReloadBullet;
        BulletText.text = $"{CurrentBullet}/{ReloadBullet}";
    }

    #endregion

    #region 게임 상태 동기화 관련 함수들

    void InitGameStateProp()
    {
        if (m_GameStateProps == null)
        {
            m_GameStateProps = new ExitGames.Client.Photon.Hashtable();
        }
        m_GameStateProps.Clear();
        m_GameStateProps.Add("GameState", 1);   // 1은 진행중, 2는 게임 클리어, 3은 게임 오버
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_GameStateProps);
    }

    void SendGameStateProp()
    {
        if (m_GameStateProps == null)
        {
            m_GameStateProps = new ExitGames.Client.Photon.Hashtable();
            m_GameStateProps.Clear();
        }

        if (m_GameStateProps.ContainsKey("GameState") == false) // 키가 없을 경우
        {
            m_GameStateProps.Clear();
            if (g_GameState == GameState.Ing)
            {
                m_GameStateProps.Add("GameState", 1);
            }
            if (g_GameState == GameState.End)
            {
                m_GameStateProps.Add("GameState", 2);
            }
            else if (g_GameState == GameState.GameOver)
            {
                m_GameStateProps.Add("GameState", 3);
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(m_GameStateProps);
        }// if (m_GameStateProps.ContainsKey("GameState") == false)
        else // 키가 있을 경우
        {
            if (g_GameState == GameState.End)
            {
                m_GameStateProps["GameState"] = 2;
                PhotonNetwork.CurrentRoom.SetCustomProperties(m_GameStateProps);
            }
            else if (g_GameState == GameState.GameOver)
            {
                m_GameStateProps["GameState"] = 3;
                PhotonNetwork.CurrentRoom.SetCustomProperties(m_GameStateProps);
            }
        }//else
    }//void SendGameStateProp() 

    void ReceiveGameState()
    {
        if (m_GameStateProps == null)
        {
            m_GameStateProps = new ExitGames.Client.Photon.Hashtable();
            m_GameStateProps.Clear();
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties["GameState"] != null) // 키가 있는데 null임...
        {
            Remote_gamestate = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameState"];
        }

        if (Remote_gamestate == 1)
        {
            g_GameState = GameState.Ing;
        }
        else if (Remote_gamestate == 2)
        {
            g_GameState = GameState.End;
        }
        else if (Remote_gamestate == 3)
        {
            g_GameState = GameState.GameOver;
        }
    }

    #endregion

    #region 게임플레이어가 골드 및 아이템 먹는 수치 조절 함수들

    internal void GetMineral() 
    {
        MineralText.text = $"미네랄 : {CurrentGetmineral}";        
    }

    internal void GetGold(int killCount) 
    {
        CurrentGetgold = killCount * 50;
        GoldText.text = $"골드 : {CurrentGetgold}";
    }

    #endregion

    #region 플레이어 사망 관련 체크 함수들

    void InitPlayerDieCheck()
    {
        if (m_PlayerDieProps == null)
        {
            m_PlayerDieProps = new ExitGames.Client.Photon.Hashtable();
        }
        m_PlayerDieProps.Clear();
        m_PlayerDieProps.Add("PlayerDieCount", 0);   // 플레이어 사망여부 판별
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_PlayerDieProps);
    }

    internal void PlayerDieSend()
    {
        if (m_PlayerDieProps == null)
        {
            m_PlayerDieProps = new ExitGames.Client.Photon.Hashtable();
            m_PlayerDieProps.Clear();
        }

        if (m_PlayerDieProps.ContainsKey("PlayerDieCount") == false) // 키가 없을 경우
        {
            m_PlayerDieProps.Clear();
            DiePlayerCount = 1; // 죽었을 때 만들어 졌을 경우
            m_PlayerDieProps.Add("PlayerDieCount", DiePlayerCount);
            PhotonNetwork.CurrentRoom.SetCustomProperties(m_PlayerDieProps);
        }//if (m_PlayerDieProps.ContainsKey("PlayerDieCount") == false)
        else // 키가 있다면,
        {
            DiePlayerCount++;            
            m_PlayerDieProps["PlayerDieCount"] = DiePlayerCount;
            PhotonNetwork.CurrentRoom.SetCustomProperties(m_PlayerDieProps);
        }
    }

    void PlayerDieReceive()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["PlayerDieCount"] != null) // 키가 있는데 null임...
        {
            DiePlayerCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["PlayerDieCount"];            
        }

        if (DiePlayerCount == PhotonNetwork.PlayerList.Length) // 죽은 사람의 숫자가 전체 플레이어와 같을 시
        {            
            g_GameState = GameState.GameOver;   // 게임 오버 처리
        }
    }

    #endregion    

    // 게임의 상태를 확인하고, 게임의 종료 여부를 판단
    void ObserveGameState()
    {
        if (g_GEState != GameEndState.None) // 게임끝나기를 눌렀을 시
            return;

        if (PhotonNetwork.IsMasterClient == true)
        {
            if (QuestCount >= totalQuestCount) // 게임이 승리할 경우
            {
                g_GameState = GameState.End;
                // 마우스 뷰 처리
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // UI 보이게하기
                GameEndObjs.SetActive(true);
                GameEndGetGoldText.text = $"획득한 골드 : {CurrentGetgold}";
                GameEndGetMineralText.text = $"획득한 미네랄 : {CurrentGetmineral}";
            }// if (QuestCount >= totalQuestCount) // 게임이 승리할 경우
            else if (g_GameState == GameState.GameOver) // 게임 오버일 경우
            {
                // 마우스 뷰 처리
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // UI 보이게하기
                GameOverObjs.SetActive(true);
            }
            SendGameStateProp();
        }//if (PhotonNetwork.IsMasterClient == true)
        else // 원격지의 경우
        {
            ReceiveGameState();
            if (g_GameState == GameState.End)// 게임이 승리할 경우 
            {
                // 마우스 뷰 처리
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GameEndObjs.SetActive(true);
                GameEndGetGoldText.text = $"획득한 골드 : {CurrentGetgold}";
                GameEndGetMineralText.text = $"획득한 미네랄 : {CurrentGetmineral}";
            }
            else if (g_GameState == GameState.GameOver) // 게임 오버일 경우
            {
                // 마우스 뷰 처리
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // UI 보이게하기
                GameOverObjs.SetActive(true);
            }
        }// else
    }// void ObserveGameState() 

    // 게임 상태 동기화 관련 함수

    #endregion

    #region 게임 클리어 혹은 종료 시 UI 동작 관련 함수들

    void GameEndGotoMultiLobbyBtnFunc()
    {
        // DB 쿼리 부분
        DBItemInsert();
        g_GEState = GameEndState.MultiLobby;
        
        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    void GameEndGotoLobbyBtnFunc()
    {
        // DB 쿼리 부분
        DBItemInsert();
        g_GEState = GameEndState.Lobby;
        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    void GameEndLabBtnFunc()
    {
        // DB 쿼리 부분
        DBItemInsert();
        g_GEState = GameEndState.Lab;
        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    void GameOverGotoMultiLobbyBtnFunc()
    {
        g_GEState = GameEndState.MultiLobby;

        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    void GameOverGotoLobbyBtnFunc()
    {
        g_GEState = GameEndState.Lobby;

        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    void GameOverLabBtnFunc()
    {
        g_GEState = GameEndState.Lab;

        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    // 룸에서 접속 종료됐을 때 호출되는 콜백 함수
    public override void OnLeftRoom() // PhotonNetwork.LeaveRoom() 성공했을 시
    {
        if (g_GEState == GameEndState.MultiLobby)
        {            
            SceneManager.LoadSceneAsync("MultiRobbyScene");
        }
        else if (g_GEState == GameEndState.Lobby)
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadSceneAsync("LobbyScene");
        }
        else if (g_GEState == GameEndState.Lab) 
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadSceneAsync("LabScene");
        }
    }
    void DBItemInsert()
    {
        if (UserInfo.g_Unique_ID == "" || UserInfo.g_Unique_ID == null)
            return;

        UserInfo.UserGold += CurrentGetgold;
        UserInfo.UserMineral += CurrentGetmineral;

        // DB로 값을 넘기는 부분
        string Query = $"INSERT INTO User_Gold(uno, Gold, Mineral)" +
            $" VALUES('{UserInfo.g_Unique_ID}','{UserInfo.UserGold}','{UserInfo.UserMineral}')" +
            $" ON DUPLICATE KEY UPDATE Gold='{UserInfo.UserGold}', Mineral='{UserInfo.UserMineral}'; ";

        MySQLConnect mysqlTestRef = new MySQLConnect();
        mysqlTestRef.sqlcmdSel(Query);
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
