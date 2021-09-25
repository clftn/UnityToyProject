using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomMgr : MonoBehaviourPunCallbacks
{
    [Header("--- Room UI ---")]
    public Button BackBtn;
    public Button StartBtn;
    public Button ReadyBtn;
    public Text PlayerCountText;
    public Text RoomNameText;

    [Header("--- UserNode ---")]
    public GameObject UserNodeItem;
    public Transform UserNodeParent;

    [Header("--- Chatting ---")]
    public Text ChattingText;
    public InputField ChattingInput;

    // 챗팅 메모리 관리용 List
    List<string> chatList = new List<string>();
    string[] TchatList = null;
    // 포톤 컴포넌트 선언
    PhotonView pv = null;

    // CustomProperties 래디 상태 확인용
    ExitGames.Client.Photon.Hashtable m_PlayerReady = new ExitGames.Client.Photon.Hashtable();

    // CustomProperties 시작 상태 확인용
    ExitGames.Client.Photon.Hashtable m_StartCheck = new ExitGames.Client.Photon.Hashtable();

    // 계산용 유저 노드
    GameObject a_UserNode = null;

    // 레디 버튼 바꾸는 플래그 변수
    bool isReady = false;
    // 시작할 때 Lock걸기 -> 원격으로 할 경우 update에서 탐지하므로 여러번 눌리는 불상사가 발생함
    bool StartLock = false;

    void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true; // 다시 서버와 통신 시작
        pv = GetComponent<PhotonView>();
        // 룸에 입장한 뒤 기존 접속자 정보 확인
        GetConnectPlayerCount();

        // CustomProperties 초기화
        InitSelTeamProps();
        StartLock = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //RPC 함수 호출
        pv.RPC("RefreshRoomData", RpcTarget.AllViaServer);

        if (BackBtn != null)
            BackBtn.onClick.AddListener(OnClickExitRoom);

        // 방장이 아니면 누를 수 없게
        if (StartBtn != null) 
        {
            StartBtn.interactable = false;
            StartBtn.onClick.AddListener(StartUpFunc);
        }
        
        if (ReadyBtn != null)
            ReadyBtn.onClick.AddListener(() =>
            {
                isReady = !isReady;
                if (isReady)
                    SendReady(1);
                else
                    SendReady(0);

                if (pv != null)
                    pv.RPC("RefreshRoomData", RpcTarget.AllViaServer);
            });

        string msg = $"\n<color=#00ff00>[{PhotonNetwork.LocalPlayer.NickName}] Connected</color>";
        //RPC 함수 호출
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChattingInput.ActivateInputField(); // 커서를 인풋필드로 이동시켜줌
            if (ChattingInput.text != "")
            {
                EnterChat();
            }//if (TextChat.text != "") 
        }//if (Input.GetKeyDown(KeyCode.Return))

        if (PhotonNetwork.IsMasterClient == false) 
        {
            ReceiveStart();
        }
    }
    void InitSelTeamProps()
    {
        m_PlayerReady.Clear();
        m_PlayerReady.Add("IsReady", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReady);
    }

    void GetConnectPlayerCount()
    {
        // 현재 입장한 룸 정보를 받아옴
        Room currRoom = PhotonNetwork.CurrentRoom;

        RoomNameText.text = $"방이름 : {currRoom.Name}";
        // 현재 룸의 접속자수와 최대 접속자 확인
        PlayerCountText.text = $"참가한 인원들 : {currRoom.PlayerCount}/{currRoom.MaxPlayers}";
    }

    // 네트워크 플레이어가 접속했을 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GetConnectPlayerCount();
    }

    // 네트워크 플레이어가 룸을 나가거나 접속이 끊어졌을 때 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();

        if (pv != null)
            pv.RPC("RefreshRoomData", RpcTarget.AllViaServer);
    }

    // 룸 나가기 버튼 클릭 이벤트에 연결된 함수
    public void OnClickExitRoom()
    {
        string msg = $"\n<color=#00ff00>[{PhotonNetwork.LocalPlayer.NickName}] Disconnected</color>";
        //RPC 함수 호출
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

        // 현재 룸에서 나가는 함수
        PhotonNetwork.LeaveRoom();
    }

    // 룸에서 접속 종료됐을 때 호출되는 콜백 함수
    public override void OnLeftRoom() // PhotonNetwork.LeaveRoom() 성공했을 시
    {
        SceneManager.LoadSceneAsync("MultiRobbyScene");
    }

    [PunRPC]
    void RefreshRoomData()
    {
        // 전체 창 초기화
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_INNERPLAYER"))
        {
            Destroy(obj);
        }

        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
        {
            // 방장이면 Start 버튼 활성화
            if (a_RefPlayer.IsMasterClient && PhotonNetwork.LocalPlayer == a_RefPlayer)
            {
                StartBtn.interactable = true;
            }

            a_UserNode = (GameObject)Instantiate(UserNodeItem);
            a_UserNode.transform.SetParent(UserNodeParent);

            JoinUserDataCtrl joinUserData = a_UserNode.GetComponent<JoinUserDataCtrl>();
            joinUserData.UserNick = a_RefPlayer.NickName;
            joinUserData.DispData(ReceiveReady(a_RefPlayer));
            if (a_RefPlayer.IsLocal)
                joinUserData.IsMineChangeColor();
        }
    }//void RefreshRoomData()

    #region 채팅 관련 함수

    [PunRPC]
    void LogMsg(string msg)
    {
        // 로그 메시지 TextUI에 텍스트를 누적시켜서 표시
        ChattingText.text = ChattingText.text + msg;

        TchatList = ChattingText.text.Split('\n');
        if (TchatList.Length > 0)
        {
            chatList.Clear();
            string tempS = "";
            foreach (string chatT in TchatList)
            {
                tempS = chatT.Trim();
                if (!string.IsNullOrEmpty(tempS))
                {
                    chatList.Add(chatT);
                }
            }

            if (chatList.Count >= 15) // 15줄 이상 넘어가면 채팅 메모리 삭제
            {
                chatList.RemoveAt(0); // 가장 첫줄부터 날리기
            }
            ChattingText.text = "";

            foreach (var Tchat in chatList)
            {
                ChattingText.text += $"\n{Tchat}";
            }
        }//if (TchatList.Length > 0)        
    }

    void EnterChat()
    {
        string msg = $"\n[{PhotonNetwork.LocalPlayer.NickName}] {ChattingInput.text}";
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

        ChattingInput.text = "";
    }

    #endregion    

    void SendReady(int a_Ready = 1)
    {
        if (m_PlayerReady == null)
        {
            m_PlayerReady = new ExitGames.Client.Photon.Hashtable();
            m_PlayerReady.Clear();
        }

        if (m_PlayerReady.ContainsKey("IsReady") == true)
        {
            m_PlayerReady["IsReady"] = a_Ready;
        }
        else
        {
            m_PlayerReady.Add("IsReady", a_Ready);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReady);
    }//void SendReady(int a_Ready = 1) 

    bool ReceiveReady(Player a_Player)
    {
        if (a_Player == null)
            return false;

        if (a_Player.CustomProperties.ContainsKey("IsReady") == false)
            return false;

        if ((int)a_Player.CustomProperties["IsReady"] == 0)
            return false;
        else
            return true;
    }//bool ReceiveReady(Player a_Player) 

    // 시작 버튼 눌렀을 시
    void StartUpFunc()
    {
        // Ready 상태 체크
        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
        {
            if (a_RefPlayer.CustomProperties.ContainsKey("IsReady") == false)
            {
                Debug.Log("레디 상태가 동기화 안됨, 네트워크 문제");
                return;
            }

            if ((int)a_RefPlayer.CustomProperties["IsReady"] == 0)
            {
                Debug.Log("준비 상태 안됨");
                return;
            }           
        }
        SendStart();
        // 씬 전환
        StartCoroutine(LoadGameScene());        
    }

    IEnumerator LoadGameScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        Time.timeScale = 1.0f;

        AsyncOperation ao = SceneManager.LoadSceneAsync("MultiGameScene");

        yield return ao;
    }

    #region 시작 동기화 부분
    void SendStart()
    {
        if (m_StartCheck.ContainsKey("IsStart") == false)
        {
            m_StartCheck.Clear();
            m_StartCheck.Add("IsStart", 1);
        }
        else 
        {
            m_StartCheck["IsStart"] = 1;
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_StartCheck);
    }

    void ReceiveStart()
    {
        if (PhotonNetwork.CurrentRoom == null) // 방을 나간 경우
            return;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsStart") == false)
            return;

        if (StartLock == false) 
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["IsStart"] == 1)
            {
                StartLock = true;
                // 씬 전환
                StartCoroutine(LoadGameScene());
            }//if ((int)PhotonNetwork.CurrentRoom.CustomProperties["IsStart"] == 1) 
            else
            {
                Debug.Log("동기화 실패");
            }
        }        
    }

    #endregion
}
