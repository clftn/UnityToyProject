using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GunItemState 
{    
    BeforeBuy,
    Active,
}

public enum BulletItemState
{    
    BeforeBuy,
}

public class ItemNodeCtrl : MonoBehaviour
{
    internal GunItemState m_gunState = GunItemState.BeforeBuy;
    internal GunType m_GunType = GunType.HeavyMachinGun;

    internal BulletItemState m_bulletState = BulletItemState.BeforeBuy;
    internal BulletType m_BulletType = BulletType.HeavyMachinGun;

    // UI관련 버튼들
    public Image SoldOutImg;
    public Text TitleText;
    public Text GoldText;
    public Text MineralText;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 눌렀을 시 반응하는 소스
        Button m_BtnCom = GetComponentInChildren<Button>();
        if (m_BtnCom != null) 
        {
            m_BtnCom.onClick.AddListener(()=> 
            {
                LabMgr a_LabMgr = null;
                GameObject a_Labobj = GameObject.Find("LabMgr");

                if (a_Labobj != null) 
                {
                    a_LabMgr = a_Labobj.GetComponent<LabMgr>();
                }

                if (a_LabMgr != null)
                    a_LabMgr.BuyFunc(m_GunType, m_BulletType);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitGunData(GunType a_GunType) 
    {
        if (a_GunType < GunType.HeavyMachinGun && a_GunType >= GunType.GunCount)
            return;

        // 기본 총 정보 보여주기
        m_GunType = a_GunType;
        TitleText.text = UserInfo.m_GunItems[(int)a_GunType].m_Name;
        GoldText.text = $"Gold : {UserInfo.m_GunItems[(int)a_GunType].BuyGold}";
        MineralText.text = $"Mineral : {UserInfo.m_GunItems[(int)a_GunType].BuyMineral}";
    }

    public void InitBulletData(BulletType a_BulletType)
    {
        if (a_BulletType < BulletType.HeavyMachinGun && a_BulletType >= BulletType.BulletCount)
            return;

        // 총알 정보 보여주기
        m_BulletType = a_BulletType;
        TitleText.text = UserInfo.m_BulletItems[(int)a_BulletType].m_Name;
        GoldText.text = $"Gold : {UserInfo.m_BulletItems[(int)a_BulletType].BuyGold}";
        MineralText.text = $"Mineral : {UserInfo.m_BulletItems[(int)a_BulletType].BuyMineral}";
    }

    // 아이템 노드 셋팅
    internal void SetGunNodeItem(GunItemState a_GState, GunType a_GType, string a_GunName, int a_Gold, int a_Mineral) 
    {
        m_gunState = a_GState;
        if (a_GState == GunItemState.BeforeBuy)     // 총 사기 전
        {
            TitleText.text = a_GunName;
            m_GunType = a_GType;            
            GoldText.text = $"Gold : {a_Gold}";
            MineralText.text = $"Mineral : {a_Mineral}";
            SoldOutImg.gameObject.SetActive(false);
        }
        else if (a_GState == GunItemState.Active) 
        {
            TitleText.text = a_GunName;
            m_GunType = a_GType;            
            GoldText.text = $"Gold : {a_Gold}";
            MineralText.text = $"Mineral : {a_Mineral}";
            SoldOutImg.gameObject.SetActive(true);
        }
    }

    internal void SetBulletNodeItem(BulletItemState a_BState, BulletType a_BType, string a_BulletName, int a_Gold, int a_Mineral)
    {
        m_bulletState = a_BState;
        if (a_BState == BulletItemState.BeforeBuy)     // 총알은 구매 상태가 한개
        {
            TitleText.text = a_BulletName;
            m_BulletType = a_BType;
            GoldText.text = $"Gold : {a_Gold}";
            MineralText.text = $"Mineral : {a_Mineral}";
            SoldOutImg.gameObject.SetActive(false);
        }
    }
}
