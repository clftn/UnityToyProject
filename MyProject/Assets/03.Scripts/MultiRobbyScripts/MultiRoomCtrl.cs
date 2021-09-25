using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiRoomCtrl : MonoBehaviour
{
    internal string roomName = "";
    internal string makerName = "";
    internal int connectPlayer = 0;
    internal byte maxPlayer = 0;

    public Text textRoomName;
    public Text textmakerName;
    public Text textConnectInfo;

    internal string ReadyState = ""; // Ready 상태 표시

    // Start is called before the first frame update
    void Start()
    {
        PhotonInitRobby RefPtInit = FindObjectOfType<PhotonInitRobby>();
        GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    if (RefPtInit != null)
                    {
                        RefPtInit.OnClickRoomItem(roomName);
                    }
                });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DispRoomData(bool IsOpen)
    {
        if (IsOpen == true)
        {
            textRoomName.color = new Color32(0, 0, 0, 255);
            textConnectInfo.color = new Color32(0, 0, 0, 255);
        }
        else
        {
            textRoomName.color = new Color32(0, 0, 255, 255);
            textConnectInfo.color = new Color32(0, 0, 255, 255);
        }

        textRoomName.text = $"방이름 : {roomName}";
        textmakerName.text = $"만든사람 : {makerName}";        
        textConnectInfo.text = $"({connectPlayer}/{maxPlayer})";
    }
}
