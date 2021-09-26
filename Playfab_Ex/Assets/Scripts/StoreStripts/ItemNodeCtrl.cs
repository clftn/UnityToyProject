using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItState 
{
    Lock,
    BeforeBuy,
    Active
}

public class ItemNodeCtrl : MonoBehaviour
{
    [HideInInspector] public ItemType m_ItType = ItemType.Item_0;
    [HideInInspector] public ItState m_ItState = ItState.Lock;

    public Text m_SkillExp;     // 스킬 설명 텍스트
    public Image m_ItIconImg;
    public Text m_HelpText;
    public Text m_BuyText;    

    // Start is called before the first frame update
    void Start()
    {
        // 리스트 뷰에 있는 캐릭터 가격 버튼을 눌러 구입 시도를 한 경우
        Button m_BtnCom = this.GetComponentInChildren<Button>();
        if (m_BtnCom != null) 
        {
            m_BtnCom.onClick.AddListener(() =>
            {                
                StoreMgr a_StoreMgr = null;
                GameObject a_StoreObj = GameObject.Find("StoreMgr");

                if (a_StoreObj != null)
                    a_StoreMgr = a_StoreObj.GetComponent<StoreMgr>();
                
                if (a_StoreMgr != null)
                    a_StoreMgr.BuyItem(m_ItType);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitData(ItemType a_ItType) 
    {
        if (a_ItType < ItemType.Item_0 || ItemType.ItemCount <= a_ItType)
            return;

        m_ItType = a_ItType;
        m_ItIconImg.sprite = GlobalValue.m_ItDataList[(int)a_ItType].m_IconImg;

        m_ItIconImg.GetComponent<RectTransform>().sizeDelta
            = new Vector2(GlobalValue.m_ItDataList[(int)a_ItType].m_IconSize.x * 135.0f, 135.0f);

        m_HelpText.text = GlobalValue.m_ItDataList[(int)a_ItType].m_SkillExp;
    }

    public void SetState(ItState a_ItState, int a_Price, string SkillExp, ItemType m_ItType, int a_Lv = 0)
    {
        m_ItState = a_ItState;
        if (a_ItState == ItState.Lock) //잠긴 상태
        {
            m_HelpText.color = new Color32(50, 50, 50, 255);
            m_HelpText.text = a_Price.ToString() + " 골드";   // 버튼 안에 들어갈 골드
            m_ItIconImg.color = new Color32(0, 0, 0, 185);
            m_HelpText.gameObject.SetActive(false);
            m_BuyText.text = "구매하지 않음"; //여기서는 그냥 기본 가격
            m_SkillExp.text = "";
        }
        else if (a_ItState == ItState.BeforeBuy) //구매 가능 상태
        {
            m_HelpText.color = new Color32(50, 50, 50, 255);
            m_HelpText.text = a_Price.ToString() + " 골드";
            m_ItIconImg.color = new Color32(255, 255, 255, 120); //new Color32(110, 110, 110, 255);
            m_HelpText.gameObject.SetActive(true);
            m_BuyText.text = "구매하지 않음"; //여기서는 그냥 기본 가격
            m_SkillExp.text = SkillExp;
        }
        else if (a_ItState == ItState.Active) //활성화 상태
        {
            if (m_ItType == ItemType.Item_2) // 힐량 증가 부분과 구분
            {
                if (a_Lv < 2)
                {
                    m_HelpText.color = new Color32(0, 0, 0, 255);
                    m_HelpText.text = $"{a_Lv}/2";
                    m_ItIconImg.color = new Color32(255, 255, 255, 255);
                    m_HelpText.gameObject.SetActive(true);
                    int a_CacPrice = a_Price + (a_Price * (a_Lv - 1));
                    m_BuyText.text = "Up " + a_CacPrice.ToString() + " 포인트"; //여기서는 업데이트 가격
                }
                else 
                {
                    m_HelpText.color = new Color32(90, 90, 90, 255);
                    m_HelpText.text = $"구매 완료!";
                    m_ItIconImg.color = new Color32(255, 255, 255, 255);
                    m_HelpText.gameObject.SetActive(true);
                    int a_CacPrice = a_Price + (a_Price * (a_Lv - 1));
                    m_BuyText.text = "스킬 활성화 중"; //여기서는 업데이트 가격
                }                
            }
            else 
            {
                m_HelpText.color = new Color32(90, 90, 90, 255);
                m_HelpText.text = "구매 완료!";
                m_ItIconImg.color = new Color32(255, 255, 255, 255);
                m_HelpText.gameObject.SetActive(true);                
                int a_CacPrice = a_Price + (a_Price * (a_Lv - 1));
                m_BuyText.text = "스킬 활성화 중";
            }            
        }
    }//public void SetState(CrState a_CrState, int a_Price, int a_Lv = 0)
}
