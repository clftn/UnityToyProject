using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitAttackSetting_Node : MonoBehaviour
{
    AttackSetting m_AttackSetting = null;
    public int m_UnitNumber = 0;
    public Text m_UnitName = null;
    public Image m_Icon_Img = null;                       // 아이템 아이콘 이미지
    [HideInInspector] public int m_UniqueNum = -1;
    //---------------------------2021_10_27
    [HideInInspector] public int m_ItemTypeNumber = -1;
    [HideInInspector] public int m_UnitUseableCount = -1;
    //---------------------------2021_10_27
    //[HideInInspector] public int m_Capacity = 0;
    [HideInInspector] public int m_MaxCapacity = 10;
    [SerializeField] GameObject m_Back = null;
    [SerializeField] Text m_Capacity_Txt = null;

    RectTransform m_SlotRect = null;
    RectTransform m_IconRect = null;

    GameObject m_IconGo = null;
    GameObject m_TextGo = null;

    Image m_Slot_Img = null;

    public int Index { get; private set; }
    public bool HasItem => m_Icon_Img.sprite != null;
    public RectTransform SlotRect => m_SlotRect;
    public RectTransform IconRect => m_IconRect;

    void ShowIcon() => m_IconGo.SetActive(true);
    public void HideIcon() => m_IconGo.SetActive(false);
    public void ShowText() => m_TextGo.SetActive(true);
    public void HideText() => m_TextGo.SetActive(false);
    public void Maskable(bool a_Maskable) => m_Icon_Img.maskable = a_Maskable;
    public void SetSlotIndex(int index) => Index = index;

    void Awake()
    {
        InitComponents();
        InitValues();
    }

    // 컴포넌트 초기화
    void InitComponents()
    {
        m_SlotRect = GetComponent<RectTransform>();
        m_Slot_Img = GetComponent<Image>();
        m_AttackSetting = FindObjectOfType<AttackSetting>();

        m_IconRect = m_Icon_Img.rectTransform;
        m_IconGo = m_IconRect.gameObject;

        m_TextGo = m_UnitName.gameObject;
    }

    float a_Padding = 1.0f;
    void InitValues()
    {
        m_IconRect.pivot = new Vector2(0.5f, 0.5f);         // 중앙
        m_IconRect.anchorMin = Vector2.zero;
        m_IconRect.anchorMax = Vector2.one;

        m_IconRect.offsetMin = Vector2.one * (a_Padding);
        m_IconRect.offsetMax = Vector2.one * (-a_Padding);

        m_Icon_Img.raycastTarget = false;
        Maskable(true);

        HideIcon();
        HideText();
    }

    void Start()
    {
        if (HasItem)
        {
            ShowIcon();
            ShowText();
        }

        if (gameObject.name.Contains("Selection") == true) m_UniqueNum = -1;
    }

    // 아이템 이동
    public void SwapOrMove(UnitAttackSetting_Node a_SlotUI)
    {
        if (a_SlotUI == null) return;
        if (a_SlotUI == this) return;

        Sprite m_TempSpr = m_Icon_Img.sprite;
        string m_TempSrg = m_UnitName.text;
        int m_TempIndex = m_UniqueNum;
        int m_TypeNumber = m_ItemTypeNumber;
        int m_UseableCount = m_UnitUseableCount;

        // 대상에 아이템이 있는 경우
        if (a_SlotUI.HasItem)
        {
            SetItem(a_SlotUI.m_Icon_Img.sprite);
            SetText(a_SlotUI.m_UnitName.text);
            SetIndex(a_SlotUI.m_UniqueNum, a_SlotUI.m_ItemTypeNumber, a_SlotUI.m_UnitUseableCount);
        }
        // 대상에 아이템이 없는 경우
        else RemoveItem();

        a_SlotUI.SetItem(m_TempSpr);
        a_SlotUI.SetText(m_TempSrg);
        a_SlotUI.SetIndex(m_TempIndex, m_TypeNumber, m_UseableCount);
        m_AttackSetting.UpdateAllSlot();
    }

    // 슬롯에 아이템 등록
    public void SetItem(Sprite a_ItemSpr)
    {
        if (a_ItemSpr != null)
        {
            m_Icon_Img.sprite = a_ItemSpr;
            ShowIcon();
            ShowText();
        }
        else RemoveItem();
    }

    //public void SetCapecity()
    //{
    //    if (m_Capacity <= 0)
    //    {
    //        HideCapacity();
    //        return;
    //    }

    //    m_Back.SetActive(true);
    //    m_Capacity_Txt.text = $"{m_Capacity} / {m_MaxCapacity}";
    //}

    //public void HideCapacity()
    //{
    //    if (m_Back != null && m_Back.activeSelf == true) m_Back.SetActive(false);
    //}


    // 슬롯에 이름 등록
    public void SetText(string a_Txt)
    {
        if (a_Txt == "" || a_Txt == null) return;

        m_UnitName.text = a_Txt;
    }

    public void SetIndex(int a_Index, int a_ItemTypenum, int a_Useable)
    {
        if (a_Index < -1 || GlobarValue.g_UnitListInfo.Count < a_Index) return;
        //if (a_Index < -1 || GlobarValue.g_UnitList.Count < a_Index) return;

        m_UniqueNum = a_Index;
        m_ItemTypeNumber = a_ItemTypenum;
        m_UnitUseableCount = a_Useable;
    }

    // 아이템 제거
    public void RemoveItem()
    {
        m_Icon_Img.sprite = null;
        m_UniqueNum = -1;
        m_ItemTypeNumber = -1;
        m_UnitUseableCount = -1;

        HideIcon();
        HideText();
        //HideCapacity();
    }

    // 아이콘 이미지 초기화
    public void IconRectSetting()
    {
        m_Icon_Img.rectTransform.offsetMin = new Vector2(1, m_Icon_Img.rectTransform.offsetMin.y);
        m_Icon_Img.rectTransform.offsetMin = new Vector2(m_Icon_Img.rectTransform.offsetMin.x,1);
        m_Icon_Img.rectTransform.offsetMax = new Vector2(1, m_Icon_Img.rectTransform.offsetMin.y);
        m_Icon_Img.rectTransform.offsetMax = new Vector2(m_Icon_Img.rectTransform.offsetMin.x, 1);
    }
}
