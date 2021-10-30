using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitNode : MonoBehaviour
{
    public int m_UnitNumber = 0;
    public Text m_NameText = null;
    public Image m_UnitImg = null;

    Button m_Btn = null;
    // Start is called before the first frame update
    void Start()
    {
        m_Btn = this.gameObject.GetComponent<Button>();
        if (m_Btn != null)
            m_Btn.onClick.AddListener(() => 
            {
                UnitInfoMgr a_UnitInfo = UnitInfoMgr.Init;
                a_UnitInfo.UserInfoBtnClick(m_UnitNumber);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
