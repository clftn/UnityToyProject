using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleStageSelMgr : MonoBehaviour
{
    public Button Stage1btn;
    public Button Stage2btn;
    public Button Backbtn;

    // Start is called before the first frame update
    void Start()
    {
        Stage1btn.onClick.AddListener(()=> 
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        });

        Backbtn.onClick.AddListener(()=> 
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
