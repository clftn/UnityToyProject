using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoxCtrl : MonoBehaviour
{
    public delegate void DLT_Response();    //델리게이트 데이터(타입)형
    DLT_Response DltMethod = null;          //델리게이트 변수 선언

    public Text m_TitleTxt = null;          // 제목 텍스트 Default : "알림"
    public Text m_MsgTxt = null;            // 메시지 텍스트    Default : " "
    public Button m_OKBtn = null;           // 확인 버튼
    public Button m_CancelBtn = null;       // 취소 버튼

    GameObject m_DlgObj = null;             // 다이얼로그 박스를 동적 생성할 오브젝트

    //int m_MouseClick = 0;                 // 마우스 클릭 횟수

    // Start is called before the first frame update
    void Start()
    {
        //다이얼로그 활성화
        if(m_DlgObj != null)
            m_DlgObj.SetActive(true);

        //확인 버튼
        if (m_OKBtn != null)
            m_OKBtn.onClick.AddListener(OKBtnFunc);

        //취소 버튼
        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(CancelBtnFunc);

        //StartCoroutine(ClickMsgCo());
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 클릭 횟수 저장
        //if(Input.GetMouseButtonDown(0))
        //{
        //    m_MouseClick += 1;
        //}
    }

    IEnumerator ClickMsgCo()
    {
        //while (true)
        //{
        //    //여러번 클릭 시 경고 처리해줄..부분
        //    yield return null;
        //}
        yield return null;
    }

    #region 버튼 함수 처리
    //확인 버튼 함수
    void OKBtnFunc()
    {
        if (DltMethod != null)
            DltMethod();

        Destroy(this.gameObject);

        Debug.Log("다이얼로그 확인");
    }

    //취소 버튼 함수
    void CancelBtnFunc()
    {
        //다이얼로그 제거
        Destroy(this.gameObject);

        Debug.Log("다이얼로그 취소");
    }
    #endregion

    #region 다이얼로그 내용 초기화 함수

    //데이터 초기화
    public void InitData()
    {
        if (m_TitleTxt != null)
            m_TitleTxt.text = "알   림";

        if (m_MsgTxt != null)
            m_MsgTxt.text = "";
    }

    #endregion

    #region 다이얼로그 내용 변경 함수


    //제목 + 메시지 변경 함수
    public void TitleMsgDlg(string a_TitleStr, string a_MsgStr, DLT_Response a_DltMtd = null)
    {
        if (m_TitleTxt != null)
            m_TitleTxt.text = a_TitleStr;

        if (m_MsgTxt != null)
            m_MsgTxt.text = a_MsgStr;

        DltMethod = a_DltMtd;
    }

    // 메시지 변경 함수
    public void MsgDlg(string a_MsgStr, DLT_Response a_DltMtd = null)
    {
        if (m_MsgTxt != null)
            m_MsgTxt.text = a_MsgStr;

        DltMethod = a_DltMtd;
    }
    #endregion
}
