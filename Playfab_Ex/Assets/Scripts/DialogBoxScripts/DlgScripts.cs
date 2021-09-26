using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgScripts : MonoBehaviour
{
    public delegate void DLT_Response();
    DLT_Response DltMethod;

    public Button m_Ok_Btn = null;
    public Button m_Close_Btn = null;
    public Button m_Cancel_Btn = null;
    public Text m_Contents_Text = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Ok_Btn != null)
            m_Ok_Btn.onClick.AddListener(()=> 
            {
                if (DltMethod != null)
                    DltMethod();

                Destroy(this.gameObject);
            });

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
            });

        if (m_Cancel_Btn != null)
            m_Cancel_Btn.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
            });
    }

    public void SetMessage(string a_Mess, DLT_Response a_DltMtd = null) 
    {
        m_Contents_Text.text = a_Mess;
        DltMethod = a_DltMtd;
    }
}
