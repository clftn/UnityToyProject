using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;

public class AttackSetting : MonoBehaviour
{
    [Header("Save Team UI")]
    [SerializeField] GameObject m_Atk_Panel = null;
    [SerializeField] Button m_Add_Btn = null;
    [SerializeField] Button m_Save_Btn = null;
    [SerializeField] Button m_Clear_Btn = null;
    [SerializeField] GameObject m_TeamNode = null;
    [SerializeField] Transform m_TeamContent = null;
    [HideInInspector] public List<TeamNode> m_TNodeList = new List<TeamNode>();
    public UnitAttackSetting_Node[] m_SelectionNode = new UnitAttackSetting_Node[5];
    [HideInInspector] public static int m_TeamNodeNumber = 0;

    [Header("Drag and Drop")]
    GraphicRaycaster m_Gr;
    PointerEventData m_Data;
    List<RaycastResult> m_RayList;

    UnitAttackSetting_Node m_BeginDragSlot;                                     // Drag를 시작한 슬롯
    Transform m_BeginDragIconTransform;                                         // 슬롯의 아이콘 트랜스폼

    Vector3 m_BeginDragIconPoint;                                               // 드래그 시작 시 슬롯의 위치
    Vector3 m_BeginDragCursorPoint;                                             // 드래그 시작 시 커서의 위치
    int m_BeginDragSlotIndex;

    UnitAttackSetting_Node m_PointerOverSlot;                                    // 현재 포인터가 위치한 슬롯
    public UnitAttackSetting_Node[] m_SlotGroup;

    [SerializeField] ScrollRect m_ScrollRect = null;
    [SerializeField] PopUp m_PopUI = null;
    string MyUpdateUrl = "";
    string UpdateTeamNodeUrl = "";
    string SaveNodeUrl = "";
    bool m_Saveing = false;

    public GameObject m_SaveDonePanel = null;
    public Button m_SaveDonePanelOkBtn = null;

    public bool IsValidIndex(int a_Index) => a_Index >= 0 && a_Index < m_SlotGroup.Length;                       // 인덱스가 수용 범위에 있는지 확인
    public void SetItemIcon(int a_Index, Sprite a_Icon) => m_SlotGroup[a_Index].SetItem(a_Icon);                 // 아이템 아이콘 등록
    public void RemoveItem(int a_Index) => m_SlotGroup[a_Index].RemoveItem();                                    // 아이템 아이콘 삭제

    public static int m_UserSellDecIndex = -1;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if(m_Atk_Panel != null)
            m_Gr = m_Atk_Panel.gameObject.GetComponent<GraphicRaycaster>();

        m_Data = new PointerEventData(EventSystem.current);
        m_RayList = new List<RaycastResult>(10);
        MyUpdateUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/UpdateDec.php";
        SaveNodeUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/SaveNode.php";
    }
   
    void InitSlot()
    {
        Debug.Log(m_SlotGroup);
        for (int i = 0; i < m_SlotGroup.Length; i++)
        {
            m_SlotGroup[i].SetSlotIndex(i);
        }
    }

    void Start()
    {
        if (m_Add_Btn != null)
            m_Add_Btn.onClick.AddListener(SaveTeam);

        if (m_Save_Btn != null)
            m_Save_Btn.onClick.AddListener(ReviseTeam);

        if (m_Clear_Btn != null)
            m_Clear_Btn.onClick.AddListener(ClearTeam);

        if (m_SaveDonePanelOkBtn != null)
        {
            m_SaveDonePanelOkBtn.onClick.AddListener(() =>
            {
                if (m_SaveDonePanel.activeSelf == true)
                    m_SaveDonePanel.SetActive(false);
            });
        }

        LoadTeamNode();

        m_SlotGroup = m_ScrollRect.gameObject.GetComponentsInChildren<UnitAttackSetting_Node>();
        UpdateAllSlot();
        InitSlot();
    }

    void Update()
    {
        if (m_Data == null) return;

        m_Data.position = Input.mousePosition;

        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
    }

    // 첫번째 슬롯의 정보를 가져온다.
    T RaycastAndGetFristComponet<T>() where T : Component
    {
        // 리스트 초기화
        m_RayList.Clear();

        // 포인터 위치로부터 Raycast 발생, 결과는 m_RayList 담긴다
        m_Gr.Raycast(m_Data, m_RayList);

        // 아무것도 없다면 리턴으로 null을 반환
        if (m_RayList.Count == 0)
            return null;

        // 있다면 SlotUI를 반환
        return m_RayList[0].gameObject.GetComponent<T>();
    }

    void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_BeginDragSlot = RaycastAndGetFristComponet<UnitAttackSetting_Node>();

            // 아이템을 갖고있는 슬롯만
            if (m_BeginDragSlot != null && m_BeginDragSlot.HasItem)
            {
                // 위치 기억, 참조 등록
                m_BeginDragIconTransform = m_BeginDragSlot.IconRect.transform;
                m_BeginDragIconPoint = m_BeginDragIconTransform.position;
                m_BeginDragCursorPoint = Input.mousePosition;

                // 맨 위에 보이기
                m_BeginDragSlotIndex = m_BeginDragSlot.transform.GetSiblingIndex();
                m_BeginDragSlot.transform.SetAsLastSibling();
                m_BeginDragSlot.Maskable(false);
                m_ScrollRect.horizontal = false;
                m_BeginDragSlot.IconRectSetting();
                m_BeginDragSlot.HideText();
            }
            else
                m_BeginDragSlot = null;
        }
    }

    void OnPointerDrag()
    {
        if (m_BeginDragSlot == null) return;
        if (Input.GetMouseButton(0)) m_BeginDragIconTransform.position = m_BeginDragIconPoint + (Input.mousePosition - m_BeginDragCursorPoint);
    }

    void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (m_BeginDragSlot == null)
                return;

            // 위치 복원
            m_BeginDragIconTransform.position = m_BeginDragIconPoint;

            // UI 순서 복원
            m_BeginDragSlot.transform.SetSiblingIndex(m_BeginDragSlotIndex);
            m_BeginDragSlot.Maskable(true);
            m_ScrollRect.horizontal = true;
            m_BeginDragSlot.IconRectSetting();
            m_BeginDragSlot.ShowText();

            // Drag 완료
            EndDrag();

            // 참조 제거
            m_BeginDragSlot = null;
            m_BeginDragIconTransform = null;
        }
    }

    // 함수 종료 처리
    void EndDrag()
    {
        UnitAttackSetting_Node a_EndDragSlot = RaycastAndGetFristComponet<UnitAttackSetting_Node>();

        if (a_EndDragSlot != null)
        {
            if (a_EndDragSlot.name.Contains("SelectionSlot_") == true)
            {
                UnitAttackSetting_Node a_RefNode = m_BeginDragSlot;
                m_PopUI.OpenPopupp();
                m_PopUI.SetAction2(() => m_PopUI.SetCount(a_EndDragSlot));
                m_PopUI.SetAction(() => TrySwapItems(a_RefNode, a_EndDragSlot));
            }
            else TrySwapItems(m_BeginDragSlot, a_EndDragSlot);
        }        
    }

    // 장비 아이템 교환
    public void TryLoadSwapItems(UnitAttackSetting_Node a_TempSlot_A, UnitAttackSetting_Node a_TempSlot_B)
    {
        if (a_TempSlot_A == a_TempSlot_B)
            return;

        a_TempSlot_A.SwapOrMove(a_TempSlot_B);
    }

    public void TrySwapItems(UnitAttackSetting_Node a_TempSlot_A, UnitAttackSetting_Node a_TempSlot_B)
    {
        //if (a_TempSlot_B.m_Capacity <= 0 || a_TempSlot_B.m_Capacity > 10) return;
        if (a_TempSlot_A == a_TempSlot_B)
            return;

        a_TempSlot_A.SwapOrMove(a_TempSlot_B);
        //a_TempSlot_B.SetCapecity();
    }

    // 해당 슬롯의 정보 갱신
    public void UpdateSlot(int a_Index)
    {
        if (!IsValidIndex(a_Index)) return;             // 잘못된 인덱스

        UnitAttackSetting_Node a_Item = m_SlotGroup[a_Index];

        if (a_Item.m_UniqueNum == -1) RemoveItem(a_Index);
        if (a_Item != null) SetItemIcon(a_Index, a_Item.m_Icon_Img.sprite);               // 아이템이 슬롯에 존재하는 경우
        else RemoveItem(a_Index);                                                         // 빈 슬롯
    }

    public void UpdateAllSlot()
    {
        for (int i = 0; i < m_SlotGroup.Length; i++)
        {
            UpdateSlot(i);
        }
    }

    public static int a_TeamIndex = 0;
    IEnumerator UpdateTeamNodeCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_num", GlobarValue.UserNumber);
        UnityWebRequest a_www = UnityWebRequest.Post(UpdateTeamNodeUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            a_TeamIndex = int.Parse(sz);
        }
        else Debug.Log("error");
    }

    public void LoadTeamNode()
    {
        GameObject a_SaveNode;
        for (int i = 0; i < a_TeamIndex; i++)
        {
            a_SaveNode = Instantiate(m_TeamNode);

            if (a_SaveNode.TryGetComponent<TeamNode>(out var a_RefSaveNode))
            {
                a_RefSaveNode.transform.SetParent(m_TeamContent, false);
                a_RefSaveNode.m_NodeName.text = $"{i + 1} 번 덱";
                a_RefSaveNode.name = "TeamNode_" + i;
                a_RefSaveNode.m_TeamNumber = i;
                m_TNodeList.Add(a_RefSaveNode);
            }
        }
    }

    // 팀 노드 저장
    void SaveTeam()
    {
        if (m_Saveing == true) return;

        m_Saveing = true;
        GameObject a_SaveNode = Instantiate(m_TeamNode);

        if (a_SaveNode.TryGetComponent<TeamNode>(out var a_RefSaveNode))
        {
            StartCoroutine(SaveNode(a_TeamIndex));
            a_RefSaveNode.transform.SetParent(m_TeamContent, false);
            a_RefSaveNode.m_NodeName.text = $"{a_TeamIndex + 1} 번 덱";
            a_RefSaveNode.name = "TeamNode_" + a_TeamIndex;
            a_RefSaveNode.m_TeamNumber = a_TeamIndex;
            m_TNodeList.Add(a_RefSaveNode);
            a_TeamIndex++;
        }
    }

    // 팀 노드 수정
    void ReviseTeam()
    {
        StartCoroutine(UpdateMyCo(m_TeamNodeNumber));
        m_SaveDonePanel.SetActive(true);
    }

    int a_FindIndex = -1;
    // 팀 노드 초기화
    void ClearTeam()
    {
        for (int i = 0; i < m_SelectionNode.Length; i++)
        {
            a_FindIndex = FindEmptySlotIndex(i);
            if (a_FindIndex == -1) continue;
            TryLoadSwapItems(m_SelectionNode[i], m_SlotGroup[a_FindIndex]);
        }
        StartCoroutine(UpdateMyCo(m_TeamNodeNumber));
    }

    IEnumerator UpdateMyCo(int a_Index = 0)
    { 
        WWWForm form = new WWWForm();
        form.AddField("user_dec", a_Index);
        Debug.Log(a_Index);
        form.AddField("user_num", GlobarValue.UserNumber);
        Debug.Log(GlobarValue.UserNumber);

        //==========================여기 수정할것
        form.AddField("user_item_1", m_SelectionNode[0].m_ItemTypeNumber);
        form.AddField("user_item_1_num", m_SelectionNode[0].m_UnitUseableCount);

        form.AddField("user_item_2", m_SelectionNode[1].m_ItemTypeNumber);
        form.AddField("user_item_2_num", m_SelectionNode[1].m_UnitUseableCount);

        form.AddField("user_item_3", m_SelectionNode[2].m_ItemTypeNumber);
        form.AddField("user_item_3_num", m_SelectionNode[2].m_UnitUseableCount);

        form.AddField("user_item_4", m_SelectionNode[3].m_ItemTypeNumber);
        form.AddField("user_item_4_num", m_SelectionNode[3].m_UnitUseableCount);
        
        form.AddField("user_item_5", m_SelectionNode[4].m_ItemTypeNumber);
        form.AddField("user_item_5_num", m_SelectionNode[4].m_UnitUseableCount);
        //==========================여기 수정할것

        UnityWebRequest a_www = UnityWebRequest.Post(MyUpdateUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null) 
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            Debug.Log(sz); 
        }
        else Debug.Log(a_www.error);
    }

    public int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < m_SlotGroup.Length; i++)
            if (m_SlotGroup[i].m_Icon_Img.gameObject.activeSelf == false) return m_SlotGroup[i].m_UnitNumber;
        return -1;
    }

    IEnumerator SaveNode(int a_Index = 0)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_dec", a_Index);
        form.AddField("user_num", GlobarValue.UserNumber);

        UnityWebRequest a_www = UnityWebRequest.Post(SaveNodeUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null) Debug.Log("UpDateSuccess");
        else Debug.Log(a_www.error);

        m_Saveing = false;
    }
}
