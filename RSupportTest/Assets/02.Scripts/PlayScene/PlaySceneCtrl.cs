using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlaySceneCtrl : MonoBehaviourPunCallbacks
{
    // 뒤로가기 버튼 만들기
    public Button Backbtn;

    PhotonView pv = null;
    Vector3 PlayerPos = Vector3.zero;
    Quaternion PlayerQua = Quaternion.identity;

    void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        pv = GetComponent<PhotonView>();

        CreatePlayer();
        CreateObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Backbtn != null)
            Backbtn.onClick.AddListener(()=> 
            {
                // 현재 룸에서 나가는 함수
                PhotonNetwork.LeaveRoom();
            });
    }

    // Update is called once per frame
    void Update()
    {
        // 마스터 클라이언트일 경우
        // 여기서 CurrentRoom을 잠근다.
        if (PhotonNetwork.IsMasterClient) 
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else 
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }            
        }        
    }

    // 플레이어 생성부분
    void CreatePlayer() 
    {
        PlayerPos.x = Random.Range(-1.5f, 1.5f);
        PlayerPos.y = 0;
        PlayerPos.z = Random.Range(-1.5f, 1.5f);

        Vector3 angleVec = new Vector3(90.0f, 0.0f, 0.0f);
        PlayerQua.eulerAngles = angleVec;

        PhotonNetwork.Instantiate("Player", PlayerPos, PlayerQua, 0);
    }

    // 주기적으로 생성하는 것이 아니므로 초기에 오브젝트들을 생성한다.
    void CreateObj() 
    {
        Vector3 obj1StartPos = new Vector3(30, 0, -25);
        Quaternion obj1Quaternion = Quaternion.identity;
        obj1Quaternion.eulerAngles = new Vector3(90, 0, 0);

        PhotonNetwork.InstantiateRoomObject("Obj1", obj1StartPos, obj1Quaternion, 0);

        Vector3 obj2StartPos = new Vector3(-30, 0, -25);
        Quaternion obj2Quaternion = Quaternion.identity;
        obj2Quaternion.eulerAngles = new Vector3(90, 0, 0);
        PhotonNetwork.InstantiateRoomObject("Obj2", obj2StartPos, obj2Quaternion, 0);
    }

    // 룸에서 접속 종료됐을 때 호출되는 콜백 함수
    public override void OnLeftRoom() // PhotonNetwork.LeaveRoom() 성공했을 시
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("TitleScene");
    }
}
