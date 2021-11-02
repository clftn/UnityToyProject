using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoMgr : MonoBehaviour
{
    public GameObject m_Content = null;
    public static Button[] m_UnitInfoBtn = null;
    public static UnitInfoMgr Init;
    //-----------------------------------------------------------------------------추가
    public Text m_UnitName = null;
    public Text m_UnitAttack = null;
    public Text m_UnitDefance = null;
    public Text m_UnitHP = null;
    public Text m_UnitAttSpd = null;
    public Text m_UnitMoveSpd = null;
    public Text m_UnitPrice = null;
    public Text m_UnitUpPrice = null;
    public Text m_UnitUseable = null;
    public Text m_UnitKind = null;
    public Text m_UnitRange = null;
    public Image m_UnitImg = null;
    public Sprite m_TankSt = null;
    //-----------------------------------------------------------------------------추가

    int m_index = 0;
    // Start is called before the first frame update
    void Start()
    {
        Init = this;

        m_UnitInfoBtn = m_Content.transform.GetComponentsInChildren<Button>();

        if (m_UnitInfoBtn.Length != 0)
        {
            for (int i = 0; i < m_UnitInfoBtn.Length; i++)
            {
                m_index = i;
                m_UnitInfoBtn[i].onClick.AddListener(() => 
                {
                    UserInfoBtnClick(m_index);
                });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ResetInfo()
    {
        m_UnitName.text = "이름 : ";
        m_UnitAttack.text = "공격력 : ";
        m_UnitDefance.text = "방어력 : ";
        m_UnitHP.text = "체력 : ";
        m_UnitAttSpd.text = "공격속도 : ";
        m_UnitMoveSpd.text = "이동속도 : ";
        m_UnitPrice.text = "가격 : ";
        m_UnitUpPrice.text = "업그레이드 가격 : ";
        m_UnitUseable.text = "사용 여부 : ";
        m_UnitImg.sprite = m_TankSt;
        m_UnitKind.text = "유닛 용도 : ";
        m_UnitRange.text = "유닛 레벨 : ";
    }

    //-----------------------------------------------------------------------------추가
    public void UserInfoBtnClick(int _index)
    {
        m_UnitName.text = "이름 : " + GlobarValue.g_UnitListInfo[_index].m_UnitName;
        int Attack = (int)GlobarValue.g_UnitListInfo[_index].m_UnitAttack;
        int Def = (int)GlobarValue.g_UnitListInfo[_index].m_UnitDefence;
        Attack = Attack + (Attack * (GlobarValue.g_UnitListInfo[_index].m_UnitLevel - 1) / 10);
        Def = Def + (Def * (GlobarValue.g_UnitListInfo[_index].m_UnitLevel - 1) / 10);
        m_UnitAttack.text = "공격력 : " + Attack.ToString();
        m_UnitDefance.text = "방어력 : " + Def.ToString(); m_UnitHP.text = "체력 : " + GlobarValue.g_UnitListInfo[_index].m_UnitHP.ToString();
        m_UnitAttSpd.text = "공격속도 : " + GlobarValue.g_UnitListInfo[_index].m_UnitAttSpd.ToString();
        m_UnitMoveSpd.text = "이동속도 : " + GlobarValue.g_UnitListInfo[_index].m_UnitMoveSpd.ToString();
        m_UnitPrice.text = "가격 : " + GlobarValue.g_UnitListInfo[_index].m_UnitPrice.ToString();
        m_UnitUpPrice.text = "업그레이드 가격 : " + GlobarValue.g_UnitListInfo[_index].m_UnitUpPrice.ToString();
        m_UnitUseable.text = "사용 여부 : " + GlobarValue.g_UnitListInfo[_index].m_UnitUseable.ToString();
        m_UnitRange.text = "유닛 레벨 : " + GlobarValue.g_UnitListInfo[_index].m_UnitLevel.ToString();
        m_UnitImg.sprite = GlobarValue.g_UnitListInfo[_index].m_UnitSpr;
        string a_Kind = "";
        if (GlobarValue.g_UnitListInfo[_index].m_UnitKind == 0)
        {
            a_Kind = "방어(" + GlobarValue.g_UnitListInfo[_index].m_UnitKind.ToString() + ")";
        }
        else
        {
            a_Kind = "공격(" + GlobarValue.g_UnitListInfo[_index].m_UnitKind.ToString() + ")";
        }
        m_UnitKind.text = "유닛 용도 : " + a_Kind;
    }
    //-----------------------------------------------------------------------------추가
}
