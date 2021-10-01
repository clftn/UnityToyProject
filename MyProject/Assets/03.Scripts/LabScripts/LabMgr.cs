using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LabMgr : MonoBehaviour
{
    // 메인 화면 부분
    public GameObject ItemPanel;
    public Button GunCreateBtn;
    public Button BulletCreateBtn;
    public Button BackBtn;
    public Text NickText;
    public Text GoldText;
    public Text MineralText;
    public Text CbulletText;
    public Text MbulletText;

    // 버튼 아이템 생성부분
    public GameObject m_Item_NodeObj;
    public GameObject m_Item_ScrollContent;

    // 데이터 관리 부분
    ItemNodeCtrl[] itemNodes;
    bool isGunActive = false;
    bool isBulletActive = false;
    GunItemInfo m_buyGunInfo;
    BulletItemInfo m_buyBulletInfo;

    int CbulletIncre = 100;
    int MbulletIncre = 10;

    // php 통신 타이밍 확인용 변수
    public static bool isGunInit = false;   // 클래스 간 코루틴 끝난는지 확인용    
    public static bool isBulletInit = false;

    // Start is called before the first frame update
    void Start()
    {
        isGunInit = false;
        isBulletInit = false;

        // 총아이템 초기화
        DBPhpConnectScript.GetInstance().InitGunData();

        // 총알 초기화
        DBPhpConnectScript.GetInstance().InitBulletData();

        #region UI 부분
        if (GunCreateBtn != null)
        {
            GunCreateBtn.onClick.AddListener(GunCreateBtnFunc);
            GunCreateBtn.gameObject.SetActive(false);
        }

        if (BulletCreateBtn != null)
        {
            BulletCreateBtn.onClick.AddListener(BulletCreateBtnFunc);
            BulletCreateBtn.gameObject.SetActive(false);
        }

        if (BackBtn != null)
        {
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
        }

        NickText.text = $"닉네임 : {UserInfo.g_NickName}";
        GoldText.text = $"골드 : {UserInfo.UserGold}";
        MineralText.text = $"미네랄 : {UserInfo.UserMineral}";
        CbulletText.text = $"연발총 탄약 : {UserInfo.UserCBullet}";
        MbulletText.text = $"미사일 탄약 : {UserInfo.UserMBullet}";
        #endregion

        RefreshItemList();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGunInit == true)
        {
            isGunInit = false;            
            InitGunItemFunc();
            GunCreateBtn.gameObject.SetActive(true);
        }

        if (isBulletInit == true)
        {
            isBulletInit = false;            
            RefreshUserInfo();
            BulletCreateBtn.gameObject.SetActive(true);
        }
    }

    void InitGunItemFunc()
    {
        GameObject a_ItemObj = null;
        ItemNodeCtrl a_ItNode = null;

        // 총부분 초기화, 초기는 총으로 초기화
        for (int i = 0; i < UserInfo.m_GunItems.Count; i++)
        {
            a_ItemObj = (GameObject)Instantiate(m_Item_NodeObj);
            a_ItNode = a_ItemObj.GetComponent<ItemNodeCtrl>();
            a_ItNode.InitGunData(UserInfo.m_GunItems[i].gunType);
            a_ItemObj.transform.SetParent(m_Item_ScrollContent.transform, false);
        }
        RefreshItemList();
    }

    void GunCreateBtnFunc()
    {
        ItemPanel.SetActive(true);

        // 버튼 타입 Gun으로 로딩
        isGunActive = true;
        isBulletActive = false;
        RefreshItemList();
    }

    void BulletCreateBtnFunc()
    {
        ItemPanel.SetActive(true);

        isGunActive = false;
        isBulletActive = true;
        RefreshItemList();
    }

    // 아이템 리스트 관리
    void RefreshItemList()
    {
        if (itemNodes != null)
            itemNodes.Initialize();

        if (m_Item_ScrollContent != null)
        {
            if (itemNodes == null || itemNodes.Length <= 0)
                itemNodes = m_Item_ScrollContent.GetComponentsInChildren<ItemNodeCtrl>();
        }

        if (isGunActive == true)
        {
            for (int i = 0; i < UserInfo.m_GunItems.Count; i++)
            {
                if (UserInfo.m_GunItems[i].isBuy == false) // 구매를 안한 경우
                {
                    itemNodes[i].SetGunNodeItem(GunItemState.BeforeBuy,
                        UserInfo.m_GunItems[i].gunType,
                        UserInfo.m_GunItems[i].m_Name,
                        UserInfo.m_GunItems[i].BuyGold,
                        UserInfo.m_GunItems[i].BuyMineral);
                }
                else
                {
                    itemNodes[i].SetGunNodeItem(GunItemState.Active,
                        UserInfo.m_GunItems[i].gunType,
                        UserInfo.m_GunItems[i].m_Name,
                        UserInfo.m_GunItems[i].BuyGold,
                        UserInfo.m_GunItems[i].BuyMineral);
                }
            }
        }//if (isGunActive == true)        
        else if (isBulletActive == true)
        {
            for (int i = 0; i < UserInfo.m_BulletItems.Count; i++)
            {
                itemNodes[i].SetBulletNodeItem(BulletItemState.BeforeBuy,
                    UserInfo.m_BulletItems[i].bulletType,
                    UserInfo.m_BulletItems[i].m_Name,
                    UserInfo.m_BulletItems[i].BuyGold,
                    UserInfo.m_BulletItems[i].BuyGold);
            }
        }//else if (isBulletActive == true) 
    }//void RefreshItemList() 

    void RefreshUserInfo()
    {
        NickText.text = $"닉네임 : {UserInfo.g_NickName}";
        GoldText.text = $"골드 : {UserInfo.UserGold}";
        MineralText.text = $"미네랄 : {UserInfo.UserMineral}";
        CbulletText.text = $"연발총 탄약 : {UserInfo.UserCBullet}";
        MbulletText.text = $"미사일 탄약 : {UserInfo.UserMBullet}";
    }

    #region 아이템 구매 시 버튼로직들

    public void BuyFunc(GunType a_gunType, BulletType a_bulletType)
    {
        // 총알 구매시
        if (isBulletActive == true && isGunActive == false) // 총알 구매 시
        {
            BulletBuyFunc(a_bulletType);
        }
        else if (isBulletActive == false && isGunActive == true) // 총 구매시
        {
            GunBuyFunc(a_gunType);
        }
    }

    void GunBuyFunc(GunType a_gunType) // 구매하기
    {
        if (UserInfo.g_Unique_ID == "" || UserInfo.g_Unique_ID == null)
            return;

        string a_Mess = "";
        GunItemState a_GunState = GunItemState.BeforeBuy;
        bool a_NeedBuy = false;     // 구매 확정 여부
        m_buyGunInfo = UserInfo.m_GunItems[(int)a_gunType];

        if (itemNodes != null)
        {
            a_GunState = itemNodes[(int)a_gunType].m_gunState;
        }

        if (a_GunState == GunItemState.BeforeBuy)
        {
            if (UserInfo.UserGold < m_buyGunInfo.BuyGold)
            {
                a_Mess = "골드가 부족합니다.";
            }
            else if (UserInfo.UserMineral < m_buyGunInfo.BuyMineral)
            {
                a_Mess = "미네랄이 부족합니다.";
            }
            else
            {
                a_Mess = "아이템을 구입하시겠습니까?";
                a_NeedBuy = true;
            }
        }//if (a_GunState == GunItemState.BeforeBuy)
        else if (a_GunState == GunItemState.Active)
        {
            a_Mess = "이미 구매한 상품입니다.";
        }

        GameObject a_DlgRsc = Resources.Load("DlgPanel") as GameObject;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);

        DlgCtrl a_Dlgbox = a_DlgBoxObj.GetComponent<DlgCtrl>();
        if (a_Dlgbox != null)
        {
            if (a_NeedBuy == true)
            {
                a_Dlgbox.SetMessage(a_Mess, TryBuyGun);
            }
            else
            {
                a_Dlgbox.SetMessage(a_Mess);
            }
        }
    }//void GunBuyFunc(GunType a_gunType)

    void TryBuyGun()
    {
        // 위에서 사려고하는 총의 정보를 받아와야 한다.
        // 글로벌 데이터 확인 부분
        UserInfo.UserGold -= m_buyGunInfo.BuyGold;
        UserInfo.UserMineral -= m_buyGunInfo.BuyMineral;
        //UserInfo.m_GunItems[(int)m_buyGunInfo.gunType].isBuy = true;
        m_buyGunInfo.isBuy = true;

        RefreshItemList();
        RefreshUserInfo();

        // DB 접근
        DBPhpConnectScript.GetInstance().LabWeaponDataInsertfunc(UserInfo.g_Unique_ID, m_buyGunInfo.gunType.ToString(),
            UserInfo.UserGold, UserInfo.UserMineral);
    }

    void BulletBuyFunc(BulletType a_bulletType)
    {
        if (UserInfo.g_Unique_ID == "" || UserInfo.g_Unique_ID == null)
            return;

        string a_Mess = "";
        BulletItemState a_BulletState = BulletItemState.BeforeBuy;
        bool a_NeedBuy = false;     // 구매 확정 여부
        m_buyBulletInfo = UserInfo.m_BulletItems[(int)a_bulletType];

        if (itemNodes != null)
        {
            a_BulletState = itemNodes[(int)a_bulletType].m_bulletState;
        }

        if (a_BulletState == BulletItemState.BeforeBuy) // 사실상 없어도 될듯?
        {
            if (UserInfo.UserGold < m_buyBulletInfo.BuyGold)
            {
                a_Mess = "골드가 부족합니다.";
            }
            else if (UserInfo.UserMineral < m_buyBulletInfo.BuyMineral)
            {
                a_Mess = "미네랄이 부족합니다.";
            }
            else
            {
                if (a_bulletType == BulletType.HeavyMachinGun)
                {
                    a_Mess = $"연발총 탄약을 구입하시겠습니까?";
                }
                else if (a_bulletType == BulletType.Missile)
                {
                    a_Mess = $"미사일 탄약을 구입하시겠습니까?";
                }

                a_NeedBuy = true;
            }
        }//if (a_GunState == GunItemState.BeforeBuy)

        GameObject a_DlgRsc = Resources.Load("DlgPanel") as GameObject;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);

        DlgCtrl a_Dlgbox = a_DlgBoxObj.GetComponent<DlgCtrl>();
        if (a_Dlgbox != null)
        {
            if (a_NeedBuy == true)
            {
                a_Dlgbox.SetMessage(a_Mess, TryBuyBullet);
            }
            else
            {
                a_Dlgbox.SetMessage(a_Mess);
            }
        }
    }//void BulletBuyFunc(BulletType a_bulletType)

    void TryBuyBullet()
    {
        // 위에서 사려고하는 총의 정보를 받아와야 한다.
        // 글로벌 데이터 확인 부분
        UserInfo.UserGold -= m_buyBulletInfo.BuyGold;
        UserInfo.UserMineral -= m_buyBulletInfo.BuyMineral;

        if (m_buyBulletInfo.bulletType == BulletType.HeavyMachinGun)
        {
            UserInfo.UserCBullet += CbulletIncre;
        }
        else if (m_buyBulletInfo.bulletType == BulletType.Missile)
        {
            UserInfo.UserMBullet += MbulletIncre;
        }

        RefreshItemList();
        RefreshUserInfo();

        // DB 접근
        if (m_buyBulletInfo.bulletType == BulletType.HeavyMachinGun)
        {
            DBPhpConnectScript.GetInstance().LabBulletDataInsertfunc(UserInfo.g_Unique_ID, m_buyBulletInfo.bulletType.ToString(),
                UserInfo.UserCBullet.ToString(), UserInfo.UserGold, UserInfo.UserMineral); ;
        }
        else if (m_buyBulletInfo.bulletType == BulletType.Missile)
        {
            DBPhpConnectScript.GetInstance().LabBulletDataInsertfunc(UserInfo.g_Unique_ID, m_buyBulletInfo.bulletType.ToString(),
                UserInfo.UserMBullet.ToString(), UserInfo.UserGold, UserInfo.UserMineral); ;
        }
    }

    #endregion
}
