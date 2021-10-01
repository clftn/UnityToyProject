using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public enum GunKind
    {
        Basic,
        Continue,
        Missile,
        GunCount
    }

    internal GunKind m_gunKind = GunKind.Basic;

    public enum FireState
    {
        StopFire,
        Fireing,
    }

    internal FireState fireState = FireState.StopFire;

    public enum AttackState
    {
        firstDelay,
        Attack,
        LastDelay
    }

    public AttackState attackState = AttackState.firstDelay;

    public GameObject bullet;
    public GameObject Missile;
    public Transform firePos;
    // 총알 발사 효과
    public MeshRenderer muzzleFlash;

    float firstShootDelayUse = 0.4f;
    float firstShootDelay = 0.4f;

    // 기본총 발사 딜레이
    float fireDelay = 0.6f;
    float fireDelayUse = 0.6f;

    // 연발총 발사 딜레이
    float fireCDelay = 0.2f;
    float fireCDelayUse = 0.2f;

    // 미사일 발사 딜레이
    float fireMDelay = 0.6f;
    float fireMDelayUse = 0.6f;

    float AfterDelay = 0.3f;
    float AfterDelayUse = 0.3f;
    private PlayerController playerConRef;
    GameMgr gameMgrRef = null;

    bool isMouseClick = false;

    // 총을 쏘고 있다고 판단하는 부분
    bool isCShot = false;
    // 총관련 부분
    bool[] HasGun;

    // 사운드 관련 변수들
    AudioClip MissileSfx = null;
    AudioClip GunSfx = null;
    AudioSource GameObjSound = null;

    // Start is called before the first frame update
    void Start()
    {
        firstShootDelayUse = firstShootDelay;
        fireDelayUse = fireDelay;
        AfterDelayUse = AfterDelay;
        playerConRef = GameObject.Find("PlayerCharactor").GetComponent<PlayerController>();
        gameMgrRef = GameObject.Find("GameMgr").GetComponent<GameMgr>();

        fireState = FireState.StopFire;
        attackState = AttackState.firstDelay;

        muzzleFlash.enabled = false;
        // 처음 기본총으로 시작
        m_gunKind = GunKind.Basic;

        #region DB 정보 가져오기 - 총 소유 여부

        HasGun = new bool[(int)GunType.GunCount];
        DBPhpConnectScript.GetInstance().InitSinglePlayGunData(HasGun);

        #endregion

        // 사운드 초기화
        MissileSfx = Resources.Load<AudioClip>("CannonFire");
        GunSfx = Resources.Load<AudioClip>("gun");
        GameObjSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeFireState();

        // 일정 상태일 때 리로드 상태일 시 총알이 나가지 않도록 하기
        // 위에서 상태값은 바꿀 수 있도록 한다.
        if (m_gunKind == GunKind.Basic)
        {
            if (gameMgrRef.isBNeedReload == true)
            {
                return;
            }
        }
        else if (m_gunKind == GunKind.Continue)
        {
            if (gameMgrRef.isCNeedReload == true)
            {
                return;
            }
        }
        else if (m_gunKind == GunKind.Missile)
        {
            if (gameMgrRef.isSNeedReload == true)
            {
                return;
            }
        }

        FireFunction();
    }

    void ChangeFireState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_gunKind = GunKind.Basic;
            gameMgrRef.SwitchGun(m_gunKind);
            isMouseClick = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (HasGun[0] == true)
            {
                m_gunKind = GunKind.Continue;
                gameMgrRef.SwitchGun(m_gunKind);
                isMouseClick = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (HasGun[1] == true)
            {
                m_gunKind = GunKind.Missile;
                gameMgrRef.SwitchGun(m_gunKind);
                isMouseClick = false;
            }
        }

        switch (m_gunKind)
        {
            case GunKind.Basic:
                {
                    BasicGunFunc();
                    break;
                }
            case GunKind.Continue:
                {
                    if (HasGun[0] == false)
                        return;

                    if (gameMgrRef.isCNeedReload == true)
                        return;

                    ContinueGunFunc();
                    break;
                }

            case GunKind.Missile:
                {
                    if (HasGun[1] == false)
                        return;

                    if (gameMgrRef.isSNeedReload == true)
                        return;

                    ShotGunFunc();
                    break;
                }
        } // switch (m_gunKind) 
    }

    void BasicGunFunc()
    {
        if (Input.GetMouseButtonDown(0) && playerConRef.isReload == false)
        {
            if (gameMgrRef.CurrentBullet > 0)
            {
                isMouseClick = true;
                attackState = AttackState.firstDelay;
            }
        }

        if (isMouseClick == true)
        {
            switch (attackState)
            {
                case AttackState.firstDelay: // 선딜레이 체크
                    {
                        firstShootDelayUse -= Time.deltaTime;
                        if (firstShootDelayUse <= 0.0f)
                        {
                            firstShootDelayUse = firstShootDelay;
                            attackState = AttackState.Attack;
                        }
                    }
                    break;

                case AttackState.Attack: // 공격 중
                    {
                        fireState = FireState.Fireing;
                        // 애니메이션 종료 여부 확인
                        if (playerConRef.playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot_SingleShot_AR") == false)
                        {
                            attackState = AttackState.LastDelay;
                        }
                    }
                    break;

                case AttackState.LastDelay: // 후 딜레이
                    {
                        fireState = FireState.StopFire;
                        AfterDelayUse -= Time.deltaTime;
                        if (AfterDelayUse <= 0.0f)
                        {
                            AfterDelayUse = AfterDelay;
                            isMouseClick = false;
                        }
                    }
                    break;
            }//switch (attackState)            
        }//if (isMouseClick == true) 
    }

    void FireFunction()
    {
        switch (m_gunKind)
        {
            case GunKind.Basic:
                {
                    fireDelayUse -= Time.deltaTime;
                    if (fireState == FireState.Fireing)
                    {
                        if (fireDelayUse <= 0.0f)
                        {
                            fireDelayUse = fireDelay;
                            Fire();
                        }
                    }

                    break;
                }
            case GunKind.Continue:
                {
                    fireCDelayUse -= Time.deltaTime;
                    if (fireState == FireState.Fireing)
                    {
                        if (fireCDelayUse <= 0.0f)
                        {
                            fireCDelayUse = fireCDelay;
                            Fire();
                        }
                    }

                    break;
                }//case GunKind.Continue:
            case GunKind.Missile:
                {
                    fireMDelayUse -= Time.deltaTime;
                    if (fireState == FireState.Fireing)
                    {
                        if (fireMDelayUse <= 0.0f)
                        {
                            fireMDelayUse = fireMDelay;
                            Fire();
                        }
                    }

                    break;
                }//case GunKind.Continue:
        }// switch (m_gunKind)
    }

    void ContinueGunFunc()
    {
        if (Input.GetMouseButton(0) && playerConRef.isReload == false && isCShot == false)
        {
            if (gameMgrRef.CurrentBullet > 0)
            {
                isMouseClick = true;
                isCShot = true;
                attackState = AttackState.firstDelay;
            }
        }

        if (Input.GetMouseButtonUp(0) && playerConRef.isReload == false && isCShot == true)
        {
            attackState = AttackState.LastDelay;
        }

        if (isMouseClick == true)
        {
            switch (attackState)
            {
                case AttackState.firstDelay: // 선딜레이 체크
                    {
                        firstShootDelayUse -= Time.deltaTime;
                        if (firstShootDelayUse <= 0.0f)
                        {
                            firstShootDelayUse = firstShootDelay;
                            attackState = AttackState.Attack;
                        }
                    }
                    break;

                case AttackState.Attack: // 공격 중
                    {
                        fireState = FireState.Fireing;
                    }
                    break;

                case AttackState.LastDelay: // 후 딜레이
                    {
                        fireState = FireState.StopFire;
                        AfterDelayUse -= Time.deltaTime;
                        if (AfterDelayUse <= 0.0f)
                        {
                            AfterDelayUse = AfterDelay;
                            isMouseClick = false;
                            isCShot = false;
                        }
                    }
                    break;
            }//switch (attackState)            
        }//if (isMouseClick == true) 
    }

    void ShotGunFunc()
    {
        if (Input.GetMouseButtonDown(0) && playerConRef.isReload == false)
        {
            if (gameMgrRef.CurrentBullet > 0)
            {
                isMouseClick = true;
                attackState = AttackState.firstDelay;
            }
        }

        if (isMouseClick == true)
        {
            switch (attackState)
            {
                case AttackState.firstDelay: // 선딜레이 체크
                    {
                        firstShootDelayUse -= Time.deltaTime;
                        if (firstShootDelayUse <= 0.0f)
                        {
                            firstShootDelayUse = firstShootDelay;
                            attackState = AttackState.Attack;
                        }
                    }
                    break;

                case AttackState.Attack: // 공격 중
                    {
                        fireState = FireState.Fireing;
                        // 애니메이션 종료 여부 확인
                        if (playerConRef.playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot_SingleShot_AR") == false)
                        {
                            attackState = AttackState.LastDelay;
                        }
                    }
                    break;

                case AttackState.LastDelay: // 후 딜레이
                    {
                        fireState = FireState.StopFire;
                        AfterDelayUse -= Time.deltaTime;
                        if (AfterDelayUse <= 0.0f)
                        {
                            AfterDelayUse = AfterDelay;
                            isMouseClick = false;
                        }
                    }
                    break;
            }//switch (attackState)            
        }//if (isMouseClick == true) 
    }

    void Fire()
    {
        CreateBullet();
        gameMgrRef.GunCount(m_gunKind);

        // 잠시 기다리는 루틴을 위해 코루틴 함수로 호출
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        // 일반 탄환
        if (m_gunKind == GunKind.Basic)
        {
            GameObjSound.PlayOneShot(GunSfx, 1.0f);
            Instantiate(bullet, firePos.position, firePos.rotation);
        }
        else if (m_gunKind == GunKind.Continue)
        {
            GameObjSound.PlayOneShot(GunSfx, 1.0f);
            Instantiate(bullet, firePos.position, firePos.rotation);
        }
        else if (m_gunKind == GunKind.Missile)
        {
            GameObjSound.PlayOneShot(MissileSfx, 1.0f);
            Instantiate(Missile, firePos.position, firePos.rotation);
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        float scale = UnityEngine.Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.01f, 0.03f));

        muzzleFlash.enabled = false;
    }
}
