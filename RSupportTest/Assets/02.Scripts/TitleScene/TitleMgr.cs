using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

/// <summary>
/// 타이틀 UI 및 포톤 네트워크 관리 스크립트
/// </summary>
public class TitleMgr : MonoBehaviourPunCallbacks
{
    // 타이틀 UI 관련 변수들
    public Text NetworkStateText;
    public InputField CreateRoomNameText;
    public InputField UserNameText;
    public Button CreateBtn;
    public Button ExitBtn;
    // 타이틀 UI 관련 변수들

    // 룸 목록 갱신용 변수들
    public GameObject scrollContents;
    public GameObject roomItem;    
    // 룸 목록 갱신용 변수들

    void Awake()
    {
        if (CreateBtn != null)
            CreateBtn.onClick.AddListener(CreateRoomFunc);

        // 포톤 네트워크 초기화가 이루어지지 않으면 생성버튼 비활성화
        CreateBtn.gameObject.SetActive(false);

        if (!PhotonNetwork.IsConnected)
        {
            NetworkStateText.text = "네트워크 접속 중입니다.";
            // 포톤클라우드 접속
            PhotonNetwork.ConnectUsingSettings();
        }

        if (ExitBtn != null) 
        {
            ExitBtn.onClick.AddListener(()=> 
            {
                // 실행파일에서는 프로세스들을 정리한다.
#if !UNITY_EDITOR
    System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
                // 각 부분에 대해 종료 프로세스를 실행한다.
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
            });
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region 포톤클라우드 접속 관련 모음

    public override void OnConnectedToMaster()
    {
        NetworkStateText.text = "접속 완료 중 입니다.";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        CreateBtn.gameObject.SetActive(true);
        NetworkStateText.text = "접속할 준비를 모두 마쳤습니다.";
    }

    public void CreateRoomFunc()
    {
        string roomName = CreateRoomNameText.text;
        if (string.IsNullOrEmpty(roomName.Trim()))
        {
            // 방이름이 없으면 랜덤하게 생성
            int randNum = Random.Range(0, 500) + 1;
            roomName = $"{randNum}_Room";
        }

        string UserName = UserNameText.text;
        if (string.IsNullOrEmpty(UserName.Trim()))
        {
            // 유저이름이 없으면 랜덤하게 생성
            int randNum = Random.Range(0, 500) + 1;
            UserName = $"{randNum}_User";
        }

        // 로컬 플레이어의 이름을 저장
        PhotonNetwork.LocalPlayer.NickName = UserName;

        // 생성할 룸의 조건 설정
        RoomOptions roomOption = new RoomOptions();
        roomOption.IsOpen = true;
        roomOption.IsVisible = true;
        roomOption.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomName, roomOption, TypedLobby.Default);
    }

    // 플레이씬으로 넘어가는 곳
    IEnumerator LoadPlayerScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        Time.timeScale = 1.0f;
        AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("PlayScene");

        yield return ao;
    }

    // 방 참여를 성공해서 넘어갈 경우
    public override void OnJoinedRoom()
    {
        NetworkStateText.text = "곧 플레이 씬으로 넘어갑니다.";
        StartCoroutine(LoadPlayerScene());
    }

    // 방만들기가 실패한 경우
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        NetworkStateText.text = "방만들기에 실패했습니다.";
        Debug.Log($"원인 : {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        NetworkStateText.text = "방참여에 실패했습니다. 방을 새로 만드세요";
        Debug.Log($"원인 : {message}");
    }

    // 방 참가 여부 확인을 위한 노드들 업데이트 하는 부분
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!roomList.Contains(roomList[i])) roomList.Add(roomList[i]);
                else roomList[roomList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (roomList.IndexOf(roomList[i]) != -1)
            {
                roomList.RemoveAt(roomList.IndexOf(roomList[i]));
            }
        }

        // 룸 목록을 다시 받았을 때 갱신하기 위해 기존에 생성된 RoomItem을 삭제
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        // 스크롤 영역 초기화
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        foreach (var roomNode in roomList)
        {
            GameObject room = (GameObject)Instantiate(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            // 생성한 RoomItem에 표시 하기 위한 텍스트 정보 전달
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = roomNode.Name;
            roomData.connectPlayer = roomNode.PlayerCount;
            roomData.maxPlayer = roomNode.MaxPlayers;
            
            // 텍스트 정보를 표시
            roomData.DisplayRoom(roomNode.IsOpen);

        }
    }//public override void OnRoomListUpdate(List<RoomInfo> roomList)

    // 방 참여를 위한 이벤트 연결 함수 다른 클래스에서 사용한다.
    public void OnClickRoomItem(string roomName)
    {
        // 로컬 플레이어 이름을 설정
        string UserName = UserNameText.text;
        if (string.IsNullOrEmpty(UserName.Trim()))
        {
            // 유저이름이 없으면 랜덤하게 생성
            int randNum = Random.Range(0, 500) + 1;
            UserName = $"{randNum}_User";
        }

        PhotonNetwork.LocalPlayer.NickName = UserName;

        // 인자로 전달된 이름에 해당하는 룸으로 입장
        PhotonNetwork.JoinRoom(roomName);
    }

    #endregion
}
