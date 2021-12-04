using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    internal string roomName;
    internal int connectPlayer;
    internal int maxPlayer;
    public Text RoomNameText;
    public Text JoinNumCount;
    public Image btnColorImg;
    TitleMgr titleRef = null;

    // Start is called before the first frame update
    void Start()
    {
        titleRef = FindObjectOfType<TitleMgr>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (titleRef != null)
            {
                titleRef.OnClickRoomItem(roomName);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayRoom(bool IsOpen)
    {        
        if (IsOpen == true)
        {
            btnColorImg.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            btnColorImg.color = new Color32(255, 0, 0, 255);
        }

        RoomNameText.text = roomName;
        JoinNumCount.text = $"({connectPlayer}/{maxPlayer})";
    }
}