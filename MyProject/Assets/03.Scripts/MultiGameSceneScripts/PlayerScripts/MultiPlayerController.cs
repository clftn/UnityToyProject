using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSpeed = 4.0f;
    private Vector3 moveDir = Vector3.zero;
    public float rotSpeed = 200.0f;
    // 재장전 모션 시 다른 애니메이션 멈춤
    public float ReloadTime = 2.2f;
    public float ReloadTimeUse = 2.2f;
    [HideInInspector] public bool isReload = false;

    [HideInInspector] public Animator playerAnim = null;
    internal MultiFireCtrl fireConRef;

    bool isDie = false;

    // 포톤 네트워크 관련 
    public Camera MyCam;
    internal PhotonView pv = null;
    internal Rigidbody rbody;
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;
    // 포톤 네트워크 관련 변수들

    MutiGameMgr gameMgrRef = null;

    void Awake()
    {
        tr = GetComponent<Transform>();
        playerAnim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        pv.ObservedComponents[0] = this;
        pv.ObservedComponents[1] = GetComponent<PhotonAnimatorView>();

        currPos = tr.position;
        currRot = tr.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        isDie = false;
        fireConRef = GameObject.Find("firePos").GetComponent<MultiFireCtrl>();
        gameMgrRef = GameObject.Find("MultiGameMgr").GetComponent<MutiGameMgr>();
        // 내가 아닐 경우 카메라들 끄기
        if (!pv.IsMine)
        {
            MyCam.gameObject.SetActive(false);
        }

        // 무게 중심을 낮춰 충돌이 발생할 때 날아가는 것을 방지
        rbody.centerOfMass = new Vector3(0.0f, -2.5f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            #region  플레이어 움직이는 부분
            if (MutiGameMgr.g_GameState != MutiGameMgr.GameState.Ing)
                return;

            if (isDie == true)
                return;

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            // 애니메이션 처리 이후 움직임 처리
            moveDir = (Vector3.forward * v) + (Vector3.right * h);
            tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

            // 애니메이션 분기처리
            if (Input.GetKey(KeyCode.LeftShift) == true && v != 0.0f)
            {
                playerAnim.SetBool("IsRun", true);
                moveSpeed = 8.0f;

                // 다른 애니메이션 전환 시 재장전 모션 초기화
                isReload = false;
                ReloadTimeUse = ReloadTime;
            } // if (Input.GetKey(KeyCode.LeftShift) == true && v != 0.0f)
            else if (Input.GetMouseButtonDown(0) == true && isReload == false) // 미사일, 기본총 시
            {
                if (fireConRef.m_gunKind == MultiFireCtrl.GunKind.Basic)
                {
                    playerAnim.SetBool("IsSingleShoot", true);
                }
            }                                    
            else if (Input.GetMouseButtonUp(0) == true && isReload == false) // 연발 종료 시
            {
                if (fireConRef.m_gunKind == MultiFireCtrl.GunKind.Continue)
                {
                    playerAnim.SetBool("IsAutoShoot", false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.R) && isReload == false) // 장전
            {
                playerAnim.SetTrigger("IsReload");
                isReload = true;
            }
            else
            {
                playerAnim.SetBool("IsRun", false);
                playerAnim.SetBool("IsSingleShoot", false);

                moveSpeed = 4.0f;
                if (v >= 0.1f)
                {
                    playerAnim.SetBool("IsWalkFront", true);
                    playerAnim.SetBool("IsWalkBack", false);
                    playerAnim.SetBool("IsWalkRight", false);
                    playerAnim.SetBool("IsWalkLeft", false);
                }
                else if (v <= -0.1f)
                {
                    playerAnim.SetBool("IsWalkBack", true);
                    playerAnim.SetBool("IsWalkFront", false);
                    playerAnim.SetBool("IsWalkRight", false);
                    playerAnim.SetBool("IsWalkLeft", false);
                }
                else if (h >= 0.1f)
                {
                    playerAnim.SetBool("IsWalkRight", true);
                    playerAnim.SetBool("IsWalkFront", false);
                    playerAnim.SetBool("IsWalkBack", false);
                    playerAnim.SetBool("IsWalkLeft", false);
                }
                else if (h <= -0.1f)
                {
                    playerAnim.SetBool("IsWalkLeft", true);
                    playerAnim.SetBool("IsWalkFront", false);
                    playerAnim.SetBool("IsWalkBack", false);
                    playerAnim.SetBool("IsWalkRight", false);
                }
                else
                {
                    playerAnim.SetBool("IsWalkFront", false);
                    playerAnim.SetBool("IsWalkBack", false);
                    playerAnim.SetBool("IsWalkRight", false);
                    playerAnim.SetBool("IsWalkLeft", false);
                }
            }//else

            if (isReload == true)
            {
                ReloadTimeUse -= Time.deltaTime;
                if (ReloadTimeUse <= 0)
                {
                    ReloadTimeUse = ReloadTime;
                    gameMgrRef.ReloadGun();
                    isReload = false;
                }
            }//if (isReload == true)

            #endregion
        }
        else
        {
            #region 타 플레이어 움직임 동기화

            if ((tr.position - currPos).magnitude > 10.0f)
            {
                tr.position = currPos;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);

            #endregion            
        }
    }//void Update()

    public void PlayerDie()
    {
        isDie = true;
        playerAnim.SetTrigger("IsDie");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 플레이어의 위치 정보 송신
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }// 로컬 플레이어의 위치 정보 송신
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
