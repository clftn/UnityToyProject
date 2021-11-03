using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttSelNodeCtrl : MonoBehaviour
{
    public Image UnitImg;           // 나중에 이미지 받으면 설정
    public Text UnitNameText;       // 유닛 이름 텍스트
    public Text UnitLevelText;      // 유닛 레벨 텍스트
    public Text UnitHPText;         // 유닛 HP 텍스트
    public Text UnitAttText;        // 유닛 공격력 텍스트
    public Text UnitDefText;        // 유닛 방어력 텍스트
    public Text UnitAttSpeedText;   // 유닛 공격속도 텍스트
    public Text UnitSpeedText;      // 유닛 이동속도 텍스트
    public Text UnitMoveAbleText;   // 유닛 배치수량 텍스트
    public Button UnitBuyBtn;       // 구매 버튼
    public Text UnitBuyBtnText;     // 구매 버튼 텍스트

    // 구매관련 변수들
    public GameObject DlgParent;
    GameObject DlgObj;
    int m_ItemPrice = 0;
    int m_ItemUpPrice = 0;
    AttUnitState buyState;

    int m_ItemNo = 0;
    string m_UnitName = "";
    int m_UnitId = 0;
    int m_isAttack = 0;
    int m_Usable = 0;
    int m_Level = 0;

    // 초기화용 변수들 선언
    int m_Hp = 0;
    float m_Att = 0;
    float m_Def = 0;
    float m_Speed = 0;
    float m_AttSpeed = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (UnitBuyBtn != null)
            UnitBuyBtn.onClick.AddListener(() =>
            {
                BuyFunc();
            });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ItemSel(string UnitName, int UnitLevel, int UnitHp, float UnitAtt,
        float UnitDef, float UnitAttSpeed, float UnitSpeed, int UnitMoveable, AttUnitState isBuyState,
        int itemprice, int itemUpprice, int UnitId, int UnitisAtt, int UnitUsable, int UnitKey,AttUnitkind a_UnitType)
    {
        UnitNameText.text = UnitName;
        if (isBuyState == AttUnitState.BeforeBuy)
        {
            UnitLevelText.text = "Buy!!";
            UnitHPText.text = $"체력 : {UnitHp}";
            UnitAttText.text = $"공격력 : {UnitAtt}";
            UnitDefText.text = $"방어력 : {UnitDef}";
            UnitBuyBtnText.text = $"구매";
        }
        else if (isBuyState == AttUnitState.Active)
        {
            UnitLevelText.text = $"업단계 : {UnitLevel}";
            UnitHPText.text = $"체력 : {UnitHp + (UnitHp * (UnitLevel - 1)) / GlobalValue.UnitIncreValue}";
            UnitAttText.text = $"공격력 : {UnitAtt + (UnitAtt * (UnitLevel - 1)) / GlobalValue.UnitIncreValue}";
            UnitDefText.text = $"방어력 : {UnitDef + (UnitDef * (UnitLevel - 1)) / GlobalValue.UnitIncreValue}";
            UnitBuyBtnText.text = $"업그레이드";
        }

        UnitAttSpeedText.text = $"공격속도 : {UnitAttSpeed}";
        UnitSpeedText.text = $"이동속도 :{UnitSpeed}";
        UnitMoveAbleText.text = $"최대 배치수량 : {UnitMoveable}";

        // 사진 이미지 넣기
        if (a_UnitType == AttUnitkind.Unit_0)
        {
            // 노말 탱크
            UnitImg.sprite = Resources.Load("StoreImg/NomalTankImg", typeof(Sprite)) as Sprite;
        }
        else if (a_UnitType == AttUnitkind.Unit_1)
        {
            // 스피드 탱크
            UnitImg.sprite = Resources.Load("StoreImg/SpeedTankImg", typeof(Sprite)) as Sprite;
        }
        else if (a_UnitType == AttUnitkind.Unit_2)
        {
            // 힐링 탱크
            UnitImg.sprite = Resources.Load("StoreImg/RepairTankImg", typeof(Sprite)) as Sprite;
        }
        else if (a_UnitType == AttUnitkind.Unit_3)
        {
            // 쉴드 탱크
            UnitImg.sprite = Resources.Load("StoreImg/ShieldTankImg", typeof(Sprite)) as Sprite;
        }
        else if (a_UnitType == AttUnitkind.Unit_4)
        {
            // 캐논 탱크
            UnitImg.sprite = Resources.Load("StoreImg/CannonTankImg", typeof(Sprite)) as Sprite;
        }

        buyState = isBuyState;
        m_ItemPrice = itemprice;
        m_ItemUpPrice = itemUpprice;

        m_UnitName = UnitName;
        m_UnitId = UnitId;
        m_isAttack = UnitisAtt;
        m_Usable = UnitUsable;
        m_Level = UnitLevel;
        m_ItemNo = UnitKey;

        m_Hp = UnitHp;
        m_Att = UnitAtt;
        m_Def = UnitDef;
        m_Speed = UnitSpeed;
        m_AttSpeed = UnitAttSpeed;
    }

    void BuyFunc()
    {
        if (DlgParent != null)
        {           
            DlgObj = DlgParent.transform.Find("MessageDlg").gameObject;
            if (DlgObj != null)
            {
                DlgObj.SetActive(true);
                if (buyState == AttUnitState.BeforeBuy)
                {                    
                    DlgObj.GetComponent<MessageDlgCtrl>().price = m_ItemPrice;
                    DlgObj.GetComponent<MessageDlgCtrl>().m_AttUnitState = buyState;
                    // 구매를 위한 맴버변수들 초기화
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_ItemName = m_UnitName;
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_KindOfItem = m_UnitId;
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_isAttack = m_isAttack;
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_Level = m_Level;
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_ItemUsable = m_Usable;
                }
                else if (buyState == AttUnitState.Active)
                {                    
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_ItemNo = m_ItemNo;
                    DlgObj.GetComponent<MessageDlgCtrl>().price = m_ItemUpPrice;
                    DlgObj.GetComponent<MessageDlgCtrl>().m_AttUnitState = buyState;
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_ItemName = m_UnitName;    // 이름
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_KindOfItem = m_UnitId;    // 유저아이디
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_isAttack = m_isAttack;    // 공격 유닛인지                    
                    // 여기서부터 데이터 증가 시작
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_Level = m_Level + 1;                    
                    DlgObj.GetComponent<MessageDlgCtrl>().buy_ItemUsable = m_Usable;    // 사용 갯수
                }
            }//if (DlgObj != null) 
        }//if (DlgParent != null)
    }//void BuyFunc()

    // 업데이트 이후 UI 리프레쉬
    public void ReSetState(int Level)
    {
        if (Level > 0)
            buyState = AttUnitState.Active;

        m_Level = Level;
        UnitLevelText.text = (buyState == AttUnitState.BeforeBuy) ? $"Buy!!" : $"업단계 : {Level}";
        UnitHPText.text = $"체력 : {m_Hp + (m_Hp * (Level - 1)) / GlobalValue.UnitIncreValue}";
        UnitAttText.text = $"공격력 : {m_Att + (m_Att * (Level - 1)) / GlobalValue.UnitIncreValue}";
        UnitDefText.text = $"방어력 : {m_Def + (m_Def * (Level - 1)) / GlobalValue.UnitIncreValue}";
        UnitAttSpeedText.text = $"공격속도 : {m_AttSpeed}";
        UnitSpeedText.text = $"이동속도 :{m_Speed}";
        UnitMoveAbleText.text = $"최대 배치수량 : {m_Usable}";
        UnitBuyBtnText.text = (buyState == AttUnitState.BeforeBuy) ? $"구매" : $"업그레이드";
    }
}
