using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanelMgr : MonoBehaviour
{
    public Button m_UserUnitBtn = null;
    public Button m_UserTowerBtn = null;

    public GameObject m_UserUnitObj = null;
    public GameObject m_UserTowerObj = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_UserUnitBtn != null)
            m_UserUnitBtn.onClick.AddListener(() => 
            {
                m_UserUnitObj.SetActive(true);
                m_UserTowerObj.SetActive(false);
                TowerInfoMgr a_InMgr = m_UserTowerObj.GetComponent<TowerInfoMgr>();
                a_InMgr.ResetInfo();
            });
        
        if (m_UserTowerBtn != null)
            m_UserTowerBtn.onClick.AddListener(() => 
            {
                m_UserUnitObj.SetActive(false);
                m_UserTowerObj.SetActive(true);
                UnitInfoMgr a_InMgr = m_UserUnitObj.GetComponent<UnitInfoMgr>();
                a_InMgr.ResetInfo();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
