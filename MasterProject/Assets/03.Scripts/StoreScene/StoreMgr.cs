using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoreMgr : MonoBehaviour
{
    // 공격과 방어 UI 분리용 Enum
    public enum StoreState
    {
        Attack,
        Defence
    }

    public StoreState storeState = StoreState.Attack;

    public Button m_BackBtn = null;

    [Header("Attack")]
    public RawImage m_AttackBack = null;
    public Button m_AttackStoreBtn = null;

    [Header("Shield")]
    public RawImage m_ShieldBack = null;
    public Button m_ShieldStoreBtn = null;

    [Header("Common")]
    public GameObject AttCheck;
    public GameObject DefCheck;
    public Button ConfigBtn;
    public Text GoldText;

    [Header("Audio")]
    public AudioClip m_BtnAudio = null;
    AudioSource m_Audio = null;

    #region 공격 부분 추가 맴버변수

    [Header("AttPart")]
    public GameObject m_AttItem_NodeObj;
    public GameObject m_AttItem_ScrollContent;
    public GameObject m_AttSelUnit;

    // 공격 아이템 선택 여부
    bool isAttSel = false;
    bool isAttfirst = true;

    GameObject m_AttItemObj = null;
    AttItNodeCtrl[] m_AttItemObjs = null;
    AttItNodeCtrl m_AttNode = null;

    #endregion


    #region 방어 부분 추가 맴버변수
    [Header("DefPart")]
    public Sprite[] m_DefUnitSpt = null;
    public GameObject m_DefItem_NodeObj;
    public GameObject m_DefItem_ScrollContent;
    public GameObject m_DefSelUnit;
    // 방어 아이템 선택 여부
    bool isDefSel = false;                          //노드 선택 체크 변수
    bool isDeffirst = true;

    GameObject m_DefItemObj = null;                 //아이템 오브젝트
    [HideInInspector] public DefItNodeCtrl[] m_DefItemObjs = null;           //아이템 노드 배열
    DefItNodeCtrl m_DefNode = null;                 //아이템 노드
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.GetInstance().InitStoreAttData();
        GlobalValue.GetInstance().InitStoreDefData();       //방어 상점 DB

        UpdateGold();

        //로비이동 버튼
        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                //로비로 이동
                SceneManager.LoadScene("LobbyScene");
            });

        //공격상점
        if (m_AttackStoreBtn != null)
            m_AttackStoreBtn.onClick.AddListener(() =>
            {
                isAttSel = true;
                isDefSel = false;
                storeState = StoreState.Attack;

                if (m_AttackBack != null)
                    m_AttackBack.gameObject.SetActive(true);

                if (m_ShieldBack != null)
                    m_ShieldBack.gameObject.SetActive(false);
            });

        //방어상점
        if (m_ShieldStoreBtn != null)
            m_ShieldStoreBtn.onClick.AddListener(() =>
            {
                isAttSel = false;
                isDefSel = true;
                storeState = StoreState.Defence;

                if (m_ShieldBack != null)
                    m_ShieldBack.gameObject.SetActive(true);

                if (m_AttackBack != null)
                    m_AttackBack.gameObject.SetActive(false);
            });

        // Config버튼
        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(() =>
            {
                Instantiate(Resources.Load("Config_Canvas"));
            });

        #region 공격 부분 start 함수 추가부분

        isAttSel = true;   // 초기 부분 // 테스트 용으로

        #endregion        
    }

    // Update is called once per frame
    void Update()
    {
        if (storeState == StoreState.Attack)
        {
            // 공격 선택 시 UI 액션 부분
            AttackUpdate();
        }//if (storeState == StoreState.Attack)
        else if (storeState == StoreState.Defence)
        {
            DefenceUpdate();
        }
    }

    public void UpdateGold()
    {
        GoldText.text = $"{MyInfo.m_Gold} Gold";
    }

    #region 공격 유닛 관련 함수들 모음

    void AttackUpdate()
    {
        InitAttSetting();   // 처음 아이템 노드 배치하는 부분 한번만 돌고 만다.

        if (isAttSel == true)
        {
            AttCheck.SetActive(true);
            DefCheck.SetActive(false);
            m_DefSelUnit.SetActive(false);
        }
    }//void AttackUpdate() 

    void InitAttSetting()
    {
        if (isAttfirst == true && GlobalValue.isAttDataInit == true) // DB 응답이 왔을 경우
        {
            isAttfirst = false;

            for (int i = 0; i < GlobalValue.m_AttUnitUserItem.Count; i++)
            {
                m_AttItemObj = (GameObject)Instantiate(m_AttItem_NodeObj);
                m_AttNode = m_AttItemObj.GetComponent<AttItNodeCtrl>();
                m_AttNode.InitData(i);
                m_AttNode.SetState((AttUnitState)GlobalValue.m_AttUnitUserItem[i].m_isBuy, GlobalValue.m_AttUnitUserItem[i].m_Level);
                m_AttItemObj.transform.SetParent(m_AttItem_ScrollContent.transform, false);
            }
        }//if (isAttfirst == true && GlobalValue.isAttDataInit == true)
    }//void InitAttSetting()

    public void ResetAttState()
    {
        if (m_AttItem_ScrollContent != null)
        {
            if (m_AttItemObjs == null || m_AttItemObjs.Length <= 0)
                m_AttItemObjs = m_AttItem_ScrollContent.GetComponentsInChildren<AttItNodeCtrl>();
        }//if (m_AttItem_ScrollContent != null)

        for (int i = 0; i < GlobalValue.m_AttUnitUserItem.Count; i++)
        {
            if (m_AttItemObjs[i].m_Unitkind != GlobalValue.m_AttUnitUserItem[i].m_unitkind)
                continue;

            m_AttItemObjs[i].SetState((AttUnitState)GlobalValue.m_AttUnitUserItem[i].m_isBuy, GlobalValue.m_AttUnitUserItem[i].m_Level);
        }//for (int i = 0;i< GlobalValue.m_AttUnitUserItem.Count;i++)
    }

    #endregion

    #region 방어 유닛 관련 함수들 모음

    //방어 상점 업데이트 함수
    void DefenceUpdate()
    {
        InitDefSetting();

        if (isDefSel == true)
        {
            AttCheck.SetActive(false);
            DefCheck.SetActive(true);
            m_AttSelUnit.SetActive(false);
        }
    }

    //방어 상점 노드 생성 및 초기 셋팅
    void InitDefSetting()
    {
        //초기 노드 생성 여부, 데이터 저장 성공 여부 == true 일 때
        if (isDeffirst == true && GlobalValue.isDefDataInit == true) // DB 응답이 왔을 경우
        {
            isDeffirst = false;

            for (int i = 0; i < GlobalValue.m_DefUnitItem.Count; i++)
            {
                m_DefItemObj = (GameObject)Instantiate(m_DefItem_NodeObj);          //방어템 노드 생성
                m_DefNode = m_DefItemObj.GetComponent<DefItNodeCtrl>();             //컴포넌트 할당
                if (m_DefUnitSpt[(int)GlobalValue.m_DefUnitItem[i].m_unitkind] != null)
                    GlobalValue.m_DefUnitItem[(int)GlobalValue.m_DefUnitItem[i].m_unitkind].m_IconImg = m_DefUnitSpt[(int)GlobalValue.m_DefUnitItem[i].m_unitkind];
                m_DefNode.InitData(GlobalValue.m_DefUnitItem[i].m_unitkind);                
                m_DefNode.SetState((AttUnitState)GlobalValue.m_DefUnitItem[i].m_isBuy, GlobalValue.m_DefUnitItem[i].m_Level);
                m_DefItemObj.transform.SetParent(m_DefItem_ScrollContent.transform, false);
            }
        }//if (isAttfirst == true && GlobalValue.isAttDataInit == true)
    }//void InitAttSetting()

    public void ResetDefState()
    {
        if (m_DefItem_ScrollContent != null)
        {
            //방어쪽 스크롤뷰 Content가 비어있지 않을 경우 노드의 컴포넌트 가져와서 아이템 노드 배열에 저장
            if (m_DefItemObjs == null || m_DefItemObjs.Length <= 0)
                m_DefItemObjs = m_DefItem_ScrollContent.GetComponentsInChildren<DefItNodeCtrl>();
        }//if (m_AttItem_ScrollContent != null)

        //
        for (int i = 0; i < GlobalValue.m_DefUnitItem.Count; i++)
        {
            if (m_DefItemObjs[i].m_Unitkind != GlobalValue.m_DefUnitItem[i].m_unitkind)
                continue;

            m_DefItemObjs[i].InitData(GlobalValue.m_DefUnitItem[i].m_unitkind);
            m_DefItemObjs[i].SetState((AttUnitState)GlobalValue.m_DefUnitItem[i].m_isBuy, GlobalValue.m_DefUnitItem[i].m_Level);
        }//for (int i = 0;i< GlobalValue.m_AttUnitUserItem.Count;i++)
    }

    #endregion
}
