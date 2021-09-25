using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMgr : MonoBehaviour
{
    public Button SingleGameStartBtn;
    public Button MultiGameStartBtn;
    public Button LabStartBtn;
    public Button LogoutBtn;
    public Text NickNameText;
    public Text GoldText;
    public Text MaterialText;

    // Start is called before the first frame update
    void Start()
    {
        NickNameText.text = $"플레이어 : {UserInfo.g_NickName}";
        GoldText.text = $"골드 : {UserInfo.UserGold}";
        MaterialText.text = $"미네럴 : {UserInfo.UserMineral}";

        if (SingleGameStartBtn != null) 
        {
            SingleGameStartBtn.onClick.AddListener(()=> 
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                UnityEngine.SceneManagement.SceneManager.LoadScene("SingleStageScene");
            });
        }
                
        if (MultiGameStartBtn != null) 
        {
            MultiGameStartBtn.onClick.AddListener(() =>
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                UnityEngine.SceneManagement.SceneManager.LoadScene("MultiRobbyScene");
            });
        }        

        if (LogoutBtn != null) 
        {
            LogoutBtn.onClick.AddListener(()=> 
            {
                UserInfo.g_Unique_ID = "";
                UserInfo.g_NickName = "";
                UserInfo.g_UserResentLoginDate = "";

                PlayFabClientAPI.ForgetAllCredentials();

                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
            });
        }

        if (LabStartBtn != null) 
        {
            LabStartBtn.onClick.AddListener(()=> 
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                UnityEngine.SceneManagement.SceneManager.LoadScene("LabScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
