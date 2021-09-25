using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinUserDataCtrl : MonoBehaviour
{
    public Text UserNickText;
    public Text IsReadyText;

    internal string UserNick;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DispData(bool isReady) 
    {
        if (isReady == true)
        {            
            IsReadyText.gameObject.SetActive(true);
        }
        else 
        {
            IsReadyText.gameObject.SetActive(false);
        }

        UserNickText.text = $"Nick : {UserNick}";
    }

    public void IsMineChangeColor()
    {
        UserNickText.color = new Color(0.0f, 1.0f, 0.0f);
    }
}
