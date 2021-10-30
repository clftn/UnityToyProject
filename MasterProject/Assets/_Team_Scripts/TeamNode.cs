using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamNode : MonoBehaviour
{
    AttackSetting m_AttackSetting = null;
    int[] m_TeamNodeNumber = new int[5];
    int[] m_TeamTankCount = new int[5];
    public Text m_NodeName = null;
    [HideInInspector] public int m_TeamNumber = -1;
    string m_MyTeamNodeUrl = "";
    Image m_ClickImg = null;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        m_AttackSetting = FindObjectOfType<AttackSetting>();
        m_MyTeamNodeUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MyTeamNode.php";
        m_ClickImg = GetComponent<Image>();
    }

    void Start()
    {
        Setting();
    }

    void Setting()
    {
        if (TryGetComponent<Button>(out var m_Btn))
        {
            m_Btn.onClick.AddListener(() =>
            {
                AttackSetting.m_TeamNodeNumber = m_TeamNumber;
                ChangeColor(m_TeamNumber, m_AttackSetting.m_TNodeList.Count);
                StartCoroutine(LoadTeamNodeCo(m_TeamNumber));
                Invoke("LoadTeam", 0.05f);
            });
        }
    }

    IEnumerator LoadTeamNodeCo(int a_Index)
    {
        WWWForm a_Form = new WWWForm();
        a_Form.AddField("user_dec", a_Index);
        a_Form.AddField("user_num", GlobarValue.UserNumber);

        UnityWebRequest a_WWW = UnityWebRequest.Post(m_MyTeamNodeUrl, a_Form);
        yield return a_WWW.SendWebRequest();

        if (a_WWW.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_WWW.downloadHandler.data);
            Debug.Log(sz);
            if (sz.Contains("Update-Success") == false)
                yield break;

            var N = JSON.Parse(sz);

            if (N["UserDec1"] != null)
                m_TeamNodeNumber[0] = N["UserDec1"].AsInt -1;

            if (N["UserDec2"] != null)
                m_TeamNodeNumber[1] = N["UserDec2"].AsInt - 1;

            if (N["UserDec3"] != null)
                m_TeamNodeNumber[2] = N["UserDec3"].AsInt - 1;

            if (N["UserDec4"] != null)
                m_TeamNodeNumber[3] = N["UserDec4"].AsInt - 1;

            if (N["UserDec5"] != null)
                m_TeamNodeNumber[4] = N["UserDec5"].AsInt - 1;

            if (N["UserDec1_Num"] != null)
                m_TeamTankCount[0] = N["UserDec1_Num"].AsInt;

            if (N["UserDec2_Num"] != null)
                m_TeamTankCount[1] = N["UserDec2_Num"].AsInt;

            if (N["UserDec3_Num"] != null)
                m_TeamTankCount[2] = N["UserDec3_Num"].AsInt;

            if (N["UserDec4_Num"] != null)
                m_TeamTankCount[3] = N["UserDec4_Num"].AsInt;

            if (N["UserDec5_Num"] != null)
                m_TeamTankCount[4] = N["UserDec5_Num"].AsInt;
        }
        else Debug.Log("error");
    }

    int a_FindIndex = -1;
    void ClearObject()
    {
        int a_Index = 0;
        for(int i = 0; i < m_AttackSetting.m_SelectionNode.Length; i++)
        {
            a_FindIndex = m_AttackSetting.FindEmptySlotIndex(a_Index);
            if (a_FindIndex == -1) return;
            m_AttackSetting.TryLoadSwapItems(m_AttackSetting.m_SelectionNode[i], m_AttackSetting.m_SlotGroup[a_FindIndex]);
        }       
    }

    void LoadTeam()
    {
        ClearObject();

        for (int i = 0; i < m_AttackSetting.m_SelectionNode.Length; i++)
        {
            if (m_TeamNodeNumber[i] == -1) continue;

            // 아이템이 존재하지 않는다면
            if (m_AttackSetting.m_SelectionNode[i].m_Icon_Img.gameObject.activeSelf == false)
            {
                for (int j = 0; j < m_AttackSetting.m_SlotGroup.Length; j++)
                {
                    if (m_AttackSetting.m_SlotGroup[j].m_UniqueNum == m_TeamNodeNumber[i])
                    {
                        //m_AttackSetting.m_SelectionNode[i].m_Capacity = m_TeamTankCount[i];
                        m_AttackSetting.TrySwapItems(m_AttackSetting.m_SlotGroup[j], m_AttackSetting.m_SelectionNode[i]);
                    }
                }
            }
        }

        for (int i = 0; i < m_AttackSetting.m_SelectionNode.Length; i++)
        {
            //m_AttackSetting.m_SelectionNode[i].m_Capacity = m_TeamTankCount[i];

            //if (m_AttackSetting.m_SelectionNode[i].m_Capacity <= 0)
            //{
            //    m_AttackSetting.m_SelectionNode[i].HideCapacity();
            //}
        }
    }

    public void ChangeColor(int a_TeamIndex, int a_TeamCount)
    {
        for (int i = 0; i < a_TeamCount; i++)
        {
            if (a_TeamIndex == m_AttackSetting.m_TNodeList[i].m_TeamNumber) m_AttackSetting.m_TNodeList[i].m_ClickImg.color = Color.yellow;
            else m_AttackSetting.m_TNodeList[i].m_ClickImg.color = Color.white;
        }
    }
}
