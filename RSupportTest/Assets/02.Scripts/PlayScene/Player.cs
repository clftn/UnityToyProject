using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

/// <summary>
/// 사용자기본 컨트롤 부분
/// </summary>
public class Player : PlayerBase, IPunObservable
{
    // 키보드 움직임 관련
    Vector3 DirVec;
    Vector3 CalcPos;
    Vector3 CalcRotate;
    Vector3 CalcScale;

    // 버튼
    Button TurnBtn;
    Button SizeUpBtn;
    Button SizeDownBtn;
    Button ReSizeBtn;

    // 조이스틱 처리 부분
    GameObject m_JoySBackObj = null;
    Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OriginPos = Vector3.zero;
    Vector3 m_Axis = Vector3.zero;
    Vector3 m_JsCacVec = Vector3.zero;
    float m_JsCacDist = 0.0f;

    float m_JoyMvLen = 0.0f;
    Vector3 m_JoyMvDir = Vector3.zero;
    // 조이스틱 처리 부분

    private Rigidbody rb;

    // 포톤 네트워크 부분
    PhotonView pv = null;

    void Awake()
    {
        CalcScale.x = 1.0f;
        CalcScale.y = 1.0f;
        CalcScale.z = 1.0f;

        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();

        // 포톤에서는 isMine일 때 카메라가 나를 따라 다니도록 한다.
        // 색상초기화도 같이 한다.
        if (pv.IsMine)
        {
            Camera.main.GetComponent<FollowCam>().Target = this.gameObject;            
        }
        else 
        {
            GetComponent<MeshRenderer>().material.color = new Color32(0,0,255,255);
        }
        
        CalcPos = transform.position;
        CalcRotate = transform.rotation.eulerAngles;
        CalcScale = transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 버튼들 초기화
        // 나 일때만 초기화 할 것
        #region 버튼 UI 연결 및 초기화
        if (pv.IsMine)
        {
            TurnBtn = GameObject.Find("TrunBtn").GetComponent<Button>();
            if (TurnBtn != null)
                TurnBtn.onClick.AddListener(() =>
                {
                    CalcRotate.x = 90.0f;
                    CalcRotate.y += Time.deltaTime * MouseRotateSpeed;
                    CalcRotate.z = 0;
                    transform.eulerAngles = CalcRotate;
                });

            SizeUpBtn = GameObject.Find("SizeUpBtn").GetComponent<Button>();
            if (SizeUpBtn != null)
                SizeUpBtn.onClick.AddListener(() =>
                {
                    if (CalcScale.x <= 10 || CalcScale.y <= 10 || CalcScale.z <= 10)
                    {
                        CalcScale.x += Time.deltaTime * ScaleIncreSpeed;
                        CalcScale.y += Time.deltaTime * ScaleIncreSpeed;
                        CalcScale.z += Time.deltaTime * ScaleIncreSpeed;
                        transform.localScale = CalcScale;
                    }
                });

            SizeDownBtn = GameObject.Find("SizeDownBtn").GetComponent<Button>();
            if (SizeDownBtn != null)
                SizeDownBtn.onClick.AddListener(() =>
                {
                    if (CalcScale.x >= 0.1 || CalcScale.y >= 0.1 || CalcScale.z >= 0.1)
                    {
                        CalcScale.x -= Time.deltaTime * ScaleIncreSpeed;
                        CalcScale.y -= Time.deltaTime * ScaleIncreSpeed;
                        CalcScale.z -= Time.deltaTime * ScaleIncreSpeed;
                        transform.localScale = CalcScale;
                    }
                });

            ReSizeBtn = GameObject.Find("ReSizeBtn").GetComponent<Button>();
            if (ReSizeBtn != null)
                ReSizeBtn.onClick.AddListener(() =>
                {
                    // 위치 초기화
                    CalcPos.x = 0.0f;
                    CalcPos.y = 0.0f;
                    CalcPos.z = 0.0f;
                    transform.position = CalcPos;

                    // 회전 초기화
                    CalcRotate.x = 90.0f;
                    CalcRotate.y = 0.0f;
                    CalcRotate.z = 0.0f;
                    transform.eulerAngles = CalcRotate;

                    // 스케일 초기화
                    CalcScale.x = 1.0f;
                    CalcScale.y = 1.0f;
                    CalcScale.z = 1.0f;
                    transform.localScale = CalcScale;
                });

            m_JoySBackObj = GameObject.Find("JoyStickBackImg");
            m_JoyStickImg = GameObject.Find("Stick").GetComponent<Image>();

            if (m_JoySBackObj != null && m_JoyStickImg != null
                && m_JoySBackObj.activeSelf == true)
            {
                Vector3[] v = new Vector3[4];
                m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
                //[0] : 좌측하단, [1] : 좌측상단, [2] : 우측상단, [3] : 우측 하단
                m_Radius = v[2].y - v[0].y;
                m_Radius = m_Radius / 3.0f;

                m_OriginPos = m_JoyStickImg.transform.position;

                EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Drag;
                entry.callback.AddListener((data) =>
                {
                    OnDragJoyStick((PointerEventData)data);
                });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.EndDrag;
                entry.callback.AddListener((data) =>
                {
                    OnEndDragJoyStick((PointerEventData)data);
                });
                trigger.triggers.Add(entry);
            }
        }//if (pv.IsMine)         
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            KeyBoardMove();
            JoyStickUpdate();
        } //if (pv.IsMine) 
    }

    void KeyBoardMove()
    {
        // 캐릭터 이동 부분
        DirVec.x = Input.GetAxisRaw("Horizontal");
        DirVec.y = 0;
        DirVec.z = Input.GetAxisRaw("Vertical");
        rb.velocity = DirVec * Speed;
        CalcPos = transform.position;   // 위치 동기화를 위한 위치 넣는 부분

        // Rotate 부분
        // Q키를 이용해서 플레이어 회전 - 직관성을 위해 한쪽으로만 회전
        if (Input.GetKey(KeyCode.Q))
        {
            CalcRotate.x = 90.0f;
            CalcRotate.y += Time.deltaTime * RotateSpeed;
            CalcRotate.z = 0;
            transform.eulerAngles = CalcRotate;
        }

        // Scale 부분
        // E키를 이용해 크기 증가, R키를 이용해 크기 감소, 크기는 x, y, z가 동시에 증가
        if (Input.GetKey(KeyCode.E))
        {
            if (CalcScale.x <= 10 || CalcScale.y <= 10 || CalcScale.z <= 10)
            {
                CalcScale.x += Time.deltaTime * ScaleIncreSpeed;
                CalcScale.y += Time.deltaTime * ScaleIncreSpeed;
                CalcScale.z += Time.deltaTime * ScaleIncreSpeed;
                transform.localScale = CalcScale;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (CalcScale.x >= 0.1 || CalcScale.y >= 0.1 || CalcScale.z >= 0.1)
            {
                CalcScale.x -= Time.deltaTime * ScaleIncreSpeed;
                CalcScale.y -= Time.deltaTime * ScaleIncreSpeed;
                CalcScale.z -= Time.deltaTime * ScaleIncreSpeed;
                transform.localScale = CalcScale;
            }
        }

        // T를 누르면 초기화
        if (Input.GetKey(KeyCode.T))
        {
            // 위치 초기화
            CalcPos.x = 0.0f;
            CalcPos.y = 0.0f;
            CalcPos.z = 0.0f;
            transform.position = CalcPos;

            // 회전 초기화
            CalcRotate.x = 90.0f;
            CalcRotate.y = 0.0f;
            CalcRotate.z = 0.0f;
            transform.eulerAngles = CalcRotate;

            // 스케일 초기화
            CalcScale.x = 1.0f;
            CalcScale.y = 1.0f;
            CalcScale.z = 1.0f;
            transform.localScale = CalcScale;
        }
    }//void KeyBoardMove()

    #region JoyStick 처리 부분

    void OnDragJoyStick(PointerEventData _data)
    {
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = Input.mousePosition - m_OriginPos;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        // 조이스틱 백그라운드를 벗어나지 못하게 막는 부분
        if (m_Radius < m_JsCacDist)
        {
            m_JoyStickImg.transform.position = m_OriginPos + m_Axis * m_Radius;
        }
        else
        {
            m_JoyStickImg.transform.position = m_OriginPos + m_Axis * m_JsCacDist;
        }

        if (m_JsCacDist > 1.0f)
            m_JsCacDist = 1.0f;

        SetJoyStickMv(m_JsCacDist, m_JsCacVec);
    }

    void OnEndDragJoyStick(PointerEventData _data)
    {
        if (m_JoyStickImg == null)
            return;

        m_Axis = Vector3.zero;
        m_JoyStickImg.transform.position = m_OriginPos;

        m_JsCacDist = 0.0f;
        m_JoyMvLen = 0.0f;
        rb.velocity = Vector3.zero;
    }

    // 조이스틱 이동 부분 업데이트
    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        m_JoyMvLen = a_JoyMvLen;
        if (a_JoyMvLen > 0.0f)
        {
            m_JoyMvDir.x = a_JoyMvDir.x;
            m_JoyMvDir.y = 0.0f;
            m_JoyMvDir.z = a_JoyMvDir.y;    // 마우스의 Y는 캐릭터의 이동의 Z 축이 된다.
            m_JoyMvDir.Normalize();
        }
    }//public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)

    void JoyStickUpdate()
    {
        // 키보드 입력이 들어왔을 경우
        if (DirVec.x > 0 || DirVec.z > 0)
        {
            m_JoyMvLen = 0.0f;
            m_JoyMvDir = Vector3.zero;
            return;
        }

        if (m_JoyMvLen > 0.0f)
        {
            // 일반적인 이동
            rb.velocity = m_JoyMvDir * Speed;
        }//if (m_JoyMvLen > 0.0f)
    }

    #endregion

    #region 포톤 네트워크 관련 함수들

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    #endregion

}
