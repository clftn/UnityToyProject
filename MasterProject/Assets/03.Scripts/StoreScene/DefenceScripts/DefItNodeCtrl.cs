using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefItNodeCtrl : MonoBehaviour
{
    internal DefUnitkind m_Unitkind = DefUnitkind.Unit_0;
    internal AttUnitState m_DefUnitState = AttUnitState.BeforeBuy;

    public Button m_DefBtn;            // 자기 자신의 버튼
    public Text m_NameText;            // 이름 텍스트
    public Image m_UnitIconImg;        // 아이콘
    public Text m_UnitLevelText;       // 유닛 레벨(아이템 구매 체크)
    public Text m_UnitAttText;         // 유닛 공격력
    public Text m_UnitHPText;          // 유닛 피통
    public Text m_UnitPriceText;       // 유닛 가격

    // 해당 노드에 정보를 담을 맴버변수들
    int m_ItemNo = 0;
    string m_Name = "";
    Sprite m_DefSpt = null;
    int m_Price = 0;
    int m_UpPrice = 0;
    int m_Level = 0;
    int m_Hp = 0;
    int m_Att = 0;
    int m_Def = 0;
    float m_AttSpeed = 0;
    float m_Speed = 0;
    int m_Moveable = 0;

    // 게임 자세히 보기 시 사용할 GameObject
    GameObject ParentsObj;
    GameObject DefSelNode;

    // Start is called before the first frame update
    void Start()
    {
        ParentsObj = GameObject.Find("SelItemViewPoint");   //선택한 아이템

        if (ParentsObj != null)
            DefSelNode = ParentsObj.transform.Find("DefSelItem").gameObject;

            if (m_DefBtn != null)
            m_DefBtn.onClick.AddListener(() =>
            {
                if (DefSelNode == null)
                    return;

                DefSelNode.GetComponent<DefSelNodeCtrl>().ItemSel(m_Name, m_DefSpt, m_Level,
                                    m_Hp, m_Att, m_Def, m_AttSpeed, m_Speed, m_Moveable.ToString(), m_DefUnitState, m_Price,
                                    m_UpPrice, (int)m_Unitkind + 1, 0, m_Moveable, m_ItemNo); // 유닛 ID는 Enum이 0부터 시작하기 때문에 +1을 해준다.

                DefSelNode.SetActive(true);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitData(DefUnitkind a_UnitType)
    {
        if (a_UnitType < DefUnitkind.Unit_0 || DefUnitkind.UnitCount <= a_UnitType)
            return;

        m_Unitkind = a_UnitType;
        //m_UnitIconImg.sprite = GlobalValue.m_ItDataList[(int)a_ItType].m_IconImg; //<- 이미지 넣는 곳, 나중에 리소스 받으면 넣을 것
        //m_ItIconImg.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalValue.m_ItDataList[(int)a_ItType].m_IconSize.x * 135.0f, 135.0f);

        // 맴버 변수 초기화
        m_Name = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Name;
        if (GlobalValue.m_DefUnitItem[(int)a_UnitType].m_IconImg != null)
            m_DefSpt = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_IconImg;
        m_Price = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Price;
        m_UpPrice = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_UpPrice;
        m_Level = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Level;
        m_Att = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Att;
        m_Hp = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Hp;
        m_Def = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Def;
        m_AttSpeed = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_AttSpeed;        
        m_ItemNo = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_UnitNo;

        // UI 관련 제어
        m_NameText.text = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Name;
        if (GlobalValue.m_DefUnitItem[(int)a_UnitType].m_IconImg != null)
            m_UnitIconImg.sprite = GlobalValue.m_DefUnitItem[(int)a_UnitType].m_IconImg;
        m_UnitLevelText.text = $"Level : {GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Level}";
        m_UnitPriceText.text = $"{GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Price}";
        m_UnitAttText.text = $"유닛 공격력 : {GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Att + (GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Att * (GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Level - 1)) / GlobalValue.UnitIncreValue}";
        m_UnitHPText.text = $"유닛 HP : {GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Hp + (GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Hp * (GlobalValue.m_DefUnitItem[(int)a_UnitType].m_Level - 1)) / GlobalValue.UnitIncreValue}";
    }

    public void SetState(AttUnitState a_UnitState, int a_Level = 1)
    {
        m_DefUnitState = a_UnitState;
        m_Level = a_Level;

        if (m_DefUnitState == AttUnitState.BeforeBuy) // 처음 구매 상태
        {
            m_UnitPriceText.text = m_Price.ToString();
            m_UnitIconImg.color = new Color32(255, 255, 255, 120); //new Color32(110, 110, 110, 255);
            m_UnitLevelText.text = "Buy!!"; //여기서는 그냥 기본 가격            
        }
        else if (m_DefUnitState == AttUnitState.Active) // 구매를 한 상태
        {
            m_UnitPriceText.text = m_UpPrice.ToString();
            m_UnitIconImg.color = new Color32(255, 255, 255, 255); //new Color32(110, 110, 110, 255);
            m_UnitLevelText.text = $"Level : {m_Level}";
            m_UnitAttText.text = $"유닛 공격력 : {m_Att + (m_Att * (m_Level - 1) / GlobalValue.UnitIncreValue)}";
            m_UnitHPText.text = $"유닛 HP : {m_Hp + (m_Hp * (m_Level - 1) / GlobalValue.UnitIncreValue)}";
        }
    }//public void SetState(CrState a_CrState, int a_Price, int a_Lv = 0)
}
