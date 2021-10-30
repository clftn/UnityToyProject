using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoMgr : MonoBehaviour
{
    public GameObject m_Content = null;
    public static Button[] m_UnitInfoBtn = null;
    public static TowerInfoMgr Init;

    public Text m_UnitName = null;
    public Text m_UnitAttack = null;
    public Text m_UnitDefance = null;
    public Text m_UnitHP = null;
    public Text m_UnitAttSpd = null;
    public Text m_UnitPrice = null;
    public Text m_UnitUpPrice = null;
    public Text m_UnitKind = null;
    public Text m_UnitRange = null;
    public Image m_UnitImg = null;
    public Sprite[] m_TowerImgList = null;
    public Sprite m_TowerSt = null;

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
        m_UnitPrice.text = "가격 : ";
        m_UnitUpPrice.text = "업그레이드 가격 : ";
        m_UnitImg.sprite = m_TowerSt;
        m_UnitKind.text = "유닛 용도 : ";
        m_UnitRange.text = "유닛 레벨 : ";
    }

    public void UserInfoBtnClick(int _index)
    {
        m_UnitName.text = "이름 : " + GlobarValue.g_UserTowerList[_index].m_TowerName;
        m_UnitAttack.text = "공격력 : " + GlobarValue.g_UserTowerList[_index].m_TowerAttack.ToString();
        m_UnitDefance.text = "방어력 : " + GlobarValue.g_UserTowerList[_index].m_TowerDefence.ToString();
        m_UnitHP.text = "체력 : " + GlobarValue.g_UserTowerList[_index].m_TowerHP.ToString();
        m_UnitAttSpd.text = "공격속도 : " + GlobarValue.g_UserTowerList[_index].m_TowerAttSpeed.ToString();
        m_UnitPrice.text = "가격 : " + GlobarValue.g_UserTowerList[_index].m_TowerPrice.ToString();
        m_UnitUpPrice.text = "업그레이드 가격 : " + GlobarValue.g_UserTowerList[_index].m_TowerUpPrice.ToString();
        m_UnitImg.sprite = m_TowerImgList[_index];

        string a_Kind = "";
        //-----------------수정
        if (GlobarValue.g_UserTowerList[_index].m_TowerKind == 0)
        {
            a_Kind = "방어(" + GlobarValue.g_UserTowerList[_index].m_TowerKind.ToString() + ")";
        }
        else
        {
            a_Kind = "공격(" + GlobarValue.g_UserTowerList[_index].m_TowerKind.ToString() + ")";
        }
        m_UnitKind.text = "유닛 용도 : " + a_Kind;

        m_UnitRange.text = "유닛 레벨 : " + GlobarValue.g_UserTowerList[_index].m_UnitLevel.ToString();
    }
}
