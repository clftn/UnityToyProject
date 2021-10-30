using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerNode : MonoBehaviour
{
    public int m_UnitNumber = 0;
    //public Text m_NameText = null;

    Button m_Btn = null;
    // Start is called before the first frame update
    void Start()
    {
        m_Btn = this.gameObject.GetComponent<Button>();
        if (m_Btn != null)
            m_Btn.onClick.AddListener(() =>
            {
                TowerInfoMgr a_TowerInfo = TowerInfoMgr.Init;
                a_TowerInfo.UserInfoBtnClick(m_UnitNumber);
            });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
