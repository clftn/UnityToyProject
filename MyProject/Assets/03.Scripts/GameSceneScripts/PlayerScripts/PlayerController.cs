using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 컨트롤러 입력 클래스
/// </summary>
public class PlayerController : MonoBehaviour
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
    internal FireController fireConRef;

    bool isDie = false;
    GameMgr gameMgrRef = null;

    // Start is called before the first frame update
    void Start()
    {
        isDie = false;
        tr = GetComponent<Transform>();
        playerAnim = GetComponent<Animator>();
        fireConRef = GameObject.Find("firePos").GetComponent<FireController>();
        gameMgrRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
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
            if (fireConRef.m_gunKind == FireController.GunKind.Basic)
            {
                playerAnim.SetBool("IsSingleShoot", true);
            }
            else if (fireConRef.m_gunKind == FireController.GunKind.Missile) 
            {
                playerAnim.SetBool("IsSingleShoot", true);
            }
        }
        else if (Input.GetMouseButton(0) == true && isReload == false) // 연발 시작 시
        {
            if (fireConRef.m_gunKind == FireController.GunKind.Continue)
            {
                playerAnim.SetBool("IsAutoShoot", true);
            }
        }
        else if (Input.GetMouseButtonUp(0) == true && isReload == false) // 연발 종료 시
        {
            if (fireConRef.m_gunKind == FireController.GunKind.Continue)
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
                isReload = false;
               
                gameMgrRef.GunReload(fireConRef.m_gunKind);
            }
        }//if (isReload == true)
    }//void Update()

    public void PlayerDie()
    {
        isDie = true;
        playerAnim.SetTrigger("IsDie");
    }
}
