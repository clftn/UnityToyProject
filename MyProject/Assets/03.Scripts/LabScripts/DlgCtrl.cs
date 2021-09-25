using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgCtrl : MonoBehaviour
{
    public delegate void DLT_Response();
    DLT_Response DltMethod;

    public Text TitleText;
    public Button OkBtn;
    public Button CancelBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (OkBtn != null)
            OkBtn.onClick.AddListener(()=> 
            {
                if (DltMethod != null)
                    DltMethod();

                Destroy(gameObject);
            });

        if (CancelBtn != null)
            CancelBtn.onClick.AddListener(()=> 
            {
                Destroy(gameObject);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(string a_Mess, DLT_Response a_DltMtd = null) 
    {
        TitleText.text = a_Mess;
        DltMethod = a_DltMtd;
    }
}
