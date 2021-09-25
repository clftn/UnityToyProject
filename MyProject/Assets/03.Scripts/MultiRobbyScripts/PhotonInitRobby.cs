using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonInitRobby : MonoBehaviourPunCallbacks
{
    public static bool isFocus = true;

    //플레이어 이름을 입력하는 UI 항목 연결
    [Header("----게임 룸만들기 및 보여주기 관련 UI----")]
    public InputField RoomName;
    public Button JoinRandomRoomBtn;
    public Button CreateRoomBtn;
    public Text NickNameText;
    // 룸 목록 갱신을 위한 변수들
    public GameObject scrollContents;
    public GameObject roomItem;    

    [Header("----서버 상태 확인 UI----")]
    public Text ServerStateText;

    [Header("----기타 UI----")]
    public Button BackBtn;

    // 룸 정보를 담고 있는 리스트
    List<RoomInfo> myList = new List<RoomInfo>(); // RoomInfo는 photon의 클래스

    // 룸 정보를 네트워크에 캐싱할 properties
    ExitGames.Client.Photon.Hashtable RoomMasterNameProp = new ExitGames.Client.Photon.Hashtable();

    void Awake()
    {
        // 포톤 클라우드 서버 접속 여부 확인
        if (!PhotonNetwork.IsConnected) // 처음 접속 시
        {
            //1. 포톤 클라우드에 접속한다.
            ServerStateText.text = "접속 결과 : 초기화 완료, 접속 중....";
            PhotonNetwork.ConnectUsingSettings();
            // 포톤 서버에 접속 시도(지역 서버 접속) -> 사용자 인증 -> 로비 입장 진행
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        #region UI 초기화 부분

        // 시작할 때 방만드는 버튼 비활성화
        if (JoinRandomRoomBtn != null)
        {
            JoinRandomRoomBtn.interactable = false;
            JoinRandomRoomBtn.onClick.AddListener(ClickJoinRandomRoom);
        }

        if (CreateRoomBtn != null)
        {
            CreateRoomBtn.interactable = false;
            CreateRoomBtn.onClick.AddListener(ClickCreateRoomBtn);
        }

        if (RoomName != null)
            RoomName.interactable = false;
        // 시작할 때 방만드는 버튼 비활성화

        if (BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                PhotonNetwork.Disconnect();
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                SceneManager.LoadScene("LobbyScene");
            });

        NickNameText.text = $"User NickName : {UserInfo.g_NickName}";

        #endregion

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 2. ConnectUsingSettings() 함수 호출에 대한 서버 접속이 성공하면 호출되는 콜백함수
    public override void OnConnectedToMaster()
    {
        ServerStateText.text = "접속 결과 : 서버 접속 완료";
        Debug.Log("서버 접속 완료"); // 단순 포톤 서버에 접속만 된 상태

        // 3. 규모가 작은 게임에서는 로비가 하나이므로..
        PhotonNetwork.JoinLobby();
        // 대형 게임에서는 상급자 로비, 중급자 로비, 초보자 로비 처럼
        // 로비가 여러개가 있을 수 있다.
    }

    // 4. 로비에 접속이 완료될 경우
    public override void OnJoinedLobby()
    {
        Debug.Log("로비접속 완료");
        ServerStateText.text = "접속 결과 : 게임 준비 완료";

        // 로비 접속이 완료되면 버튼 활성화
        JoinRandomRoomBtn.interactable = true;
        CreateRoomBtn.interactable = true;
        RoomName.interactable = true;

        // 무작위로 추출된 방으로 입장
        //PhotonNetwork.JoinRandomRoom();
    }

    public void ClickJoinRandomRoom() // 3번 방 입장 요청 버튼 누름
    {
        PhotonNetwork.LocalPlayer.NickName = UserInfo.g_NickName;
        PhotonNetwork.JoinRandomRoom();
    }

    void ClickCreateRoomBtn()
    {
        // 방만들기 클릭
        string _roomName = RoomName.text;

        if (string.IsNullOrEmpty(RoomName.text))
        {
            _roomName = $"ROOM_{Random.Range(0, 999).ToString("000")}";
        }

        // 로컬 플레이어 이름 설정
        PhotonNetwork.LocalPlayer.NickName = UserInfo.g_NickName;

        // 생성할 룸의 조건 설정
        RoomOptions roomOptions = new RoomOptions();       
        roomOptions.IsOpen = true;      // 입장 가능 여부
        roomOptions.IsVisible = true;   // 로비에서 룸의 노출 여부
        roomOptions.MaxPlayers = 3;    // 룸에 입장할 수 있는 최대 접속자 수
        // CustomRoomProperties 넣는 부분
        if (RoomMasterNameProp == null) 
        {
            InitCustomNameProp();
        }

        if (RoomMasterNameProp.ContainsKey("RoomMasterName") == true)
        {
            RoomMasterNameProp["RoomMasterName"] = UserInfo.g_NickName;
        }
        else 
        {
            RoomMasterNameProp.Add("RoomMasterName", UserInfo.g_NickName);
        }

        roomOptions.CustomRoomProperties = RoomMasterNameProp;
        roomOptions.CustomRoomPropertiesForLobby = new string[] {"RoomMasterName"};
        // 지정한 조건에 맞는 룸 생성 함수
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 방 참가 실패(참가할 방이 존재하지 않습니다.)");
        ServerStateText.text = "접속 결과 : 방이 존재하지 않습니다.";
        // 룸 생성
        PhotonNetwork.CreateRoom("MyRoom");
        // 랜덤 로그인 시에 서버 역할을 하게될 Client는 이쪽으로 들어오게 될 것이다.
    }

    //PhotonNetwork.CreateRoom();
    //PhotonNetwork.JoinRoom();
    //PhotonNetwork.JoinRandomRoom();
    // 위 3가지 함수의 콜백 함수
    public override void OnJoinedRoom()
    {
        // 서버 역할일 경우 5. 방입장, 클라이언트 역할 인 경우 4번 : 방입장        
        ServerStateText.text = "접속 결과 : 방참가 완료";
        StartCoroutine(LoadRoomScene());
    }

    // CreateRoom이 실패할 경우 호출되는 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 만들기 실패");
        Debug.Log(returnCode.ToString());
        Debug.Log(message);
        ServerStateText.text = $"방 만들기 실패 : 에러코드({returnCode}) Message :{message}";
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;        
        foreach (var roomData in roomList)
        {
            if (!roomData.RemovedFromList)
            {
                if (!myList.Contains(roomData)) myList.Add(roomData);
                else myList[myList.IndexOf(roomData)] = roomData;
            }
            else if (myList.IndexOf(roomData) != -1)
            {
                myList.RemoveAt(myList.IndexOf(roomData));
            }
        }//foreach (var roomData in roomList) 

        // 룸 목록을 다시 받았을 때 갱신하기 위해 기존에 생성된 RoomItem을 삭제
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        // 스크롤 영역 초기화
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        foreach (var roomNode in myList)
        {            
            GameObject room = (GameObject)Instantiate(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            // 생성한 RoomItem에 표시 하기 위한 텍스트 정보 전달
            MultiRoomCtrl roomData = room.GetComponent<MultiRoomCtrl>();
            roomData.roomName = roomNode.Name;
            roomData.connectPlayer = roomNode.PlayerCount;
            if (roomNode.CustomProperties.ContainsKey("RoomMasterName") == true)
            {
                roomData.makerName = roomNode.CustomProperties["RoomMasterName"].ToString();
            }
            else 
            {
                roomData.makerName = "미상";
            }            
            roomData.maxPlayer = roomNode.MaxPlayers;
            
            // 텍스트 정보를 표시
            roomData.DispRoomData(roomNode.IsOpen);
        }//foreach (var roomNode in myList)
    }

    // 방 참여를 위한 이벤트 연결 함수
    public void OnClickRoomItem(string roomName)
    {
        // 로컬 플레이어 이름을 설정
        PhotonNetwork.LocalPlayer.NickName = UserInfo.g_NickName;        

        // 인자로 전달된 이름에 해당하는 룸으로 입장
        PhotonNetwork.JoinRoom(roomName);
    }

    IEnumerator LoadRoomScene() 
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        Time.timeScale = 1.0f;

        AsyncOperation ao = SceneManager.LoadSceneAsync("RoomScene");

        yield return ao;
    }

    void InitCustomNameProp()
    {
        RoomMasterNameProp = new ExitGames.Client.Photon.Hashtable();
        RoomMasterNameProp.Clear();
        RoomMasterNameProp.Add("RoomMasterName", "");
    }
}
