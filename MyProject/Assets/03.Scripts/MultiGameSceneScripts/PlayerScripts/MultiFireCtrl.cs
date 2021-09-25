using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiFireCtrl : MonoBehaviourPunCallbacks
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
    private MultiPlayerController playerConRef;

    bool isMouseClick = false;

    // 총을 쏘고 있다고 판단하는 부분
    bool isCShot = false;
    float ShootTime = 0.0f; // 총 쏘는 시간을 확인하는 변수

    // 총관련 부분
    bool[] HasGun;

    // 사운드 관련 변수들
    AudioClip MissileSfx = null;
    AudioClip GunSfx = null;
    AudioSource GameObjSound = null;

    // 포톤 관련 변수들
    PhotonView pv = null;
    int MyPvId = -1;

    // 리로딩 관련 변수들
    MutiGameMgr gameMgr;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 내 플레이어 찾기=> 맞는 값이 없어서 ActorNumber를 활용한다.       
        GameObject[] gPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (Player Tplayer in PhotonNetwork.PlayerList) 
        {
            foreach (GameObject player in gPlayers)
            {                
                if (Tplayer.ActorNumber == player.GetComponentInChildren<MultiPlayerController>().pv.Owner.ActorNumber)// 자기 자신의 플레이어
                {
                    playerConRef = player.GetComponentInChildren<MultiPlayerController>();
                    MyPvId = Tplayer.ActorNumber;
                }
            }
        }
        // 내 플레이어 찾기

        firstShootDelayUse = firstShootDelay;
        fireDelayUse = fireDelay;
        AfterDelayUse = AfterDelay;

        fireState = FireState.StopFire;
        attackState = AttackState.firstDelay;

        muzzleFlash.enabled = false;
        // 처음 기본총으로 시작
        m_gunKind = GunKind.Basic;

        #region DB 정보 가져오기 - 총 소유 여부

        // DB 잠시 주석 처리
        //HasGun = new bool[(int)GunType.GunCount];
        //string query = "";
        //if (UserInfo.g_Unique_ID != "")
        //{
        //    query = $"select * from User_Weapon where uno = '{UserInfo.g_Unique_ID}'";
        //    MySQLConnect sqlcon = new MySQLConnect();
        //    DataTable dt = sqlcon.selsql(query);
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < (int)GunType.GunCount; i++)
        //        {
        //            int temp = 0;
        //            int.TryParse(dt.Rows[0][1 + i].ToString(), out temp); // 컬럼 값이 0이 uno, 1이 연발총, 2가 샷건이다.
        //            HasGun[i] = Convert.ToBoolean(temp);
        //        }
        //    }
        //}

        #endregion

        // 사운드 초기화
        MissileSfx = Resources.Load<AudioClip>("CannonFire");
        GunSfx = Resources.Load<AudioClip>("gun");
        GameObjSound = GetComponent<AudioSource>();

        // 게임 초기화
        gameMgr = GameObject.Find("MultiGameMgr").GetComponent<MutiGameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MutiGameMgr.g_GameState != MutiGameMgr.GameState.Ing)
            return;

        ChangeFireState();

        //// 일정 상태일 때 리로드 상태일 시 총알이 나가지 않도록 하기
        //// 위에서 상태값은 바꿀 수 있도록 한다.
        //if (m_gunKind == GunKind.Basic)
        //{
        //    //if (gameMgrRef.isBNeedReload == true)
        //    //{
        //    //    return;
        //    //}
        //}
        //else if (m_gunKind == GunKind.Continue)
        //{
        //    //if (gameMgrRef.isCNeedReload == true)
        //    //{
        //    //    return;
        //    //}
        //}
        //else if (m_gunKind == GunKind.Missile)
        //{
        //    //if (gameMgrRef.isSNeedReload == true)
        //    //{
        //    //    return;
        //    //}
        //}

        FireFunction();
    }

    void ChangeFireState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_gunKind = GunKind.Basic;
            //gameMgrRef.SwitchGun(m_gunKind);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (HasGun[0] == true)
            {
                m_gunKind = GunKind.Continue;
                //gameMgrRef.SwitchGun(m_gunKind);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (HasGun[1] == true)
            {
                m_gunKind = GunKind.Missile;
                //gameMgrRef.SwitchGun(m_gunKind);
            }
        }

        m_gunKind = GunKind.Basic;// 우선 기본 탄환만 사용하기

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

                    //if (gameMgrRef.isCNeedReload == true)
                    //    return;

                    ContinueGunFunc();
                    break;
                }

            case GunKind.Missile:
                {
                    if (HasGun[1] == false)
                        return;

                    //if (gameMgrRef.isSNeedReload == true)
                    //    return;

                    ShotGunFunc();
                    break;
                }
        } // switch (m_gunKind) 
    }

    void BasicGunFunc()
    {
        if (pv.IsMine && Input.GetMouseButtonDown(0) && playerConRef.isReload == false)
        {
            if (gameMgr.CurrentBullet <= 0) // 숫자가 0이면 딜레이 1발 남았을 때
                return;

            // 사격중일 때 마우스클릭이 되지 않도록
            if (isMouseClick == true && ShootTime < firstShootDelay + fireDelay) 
            {
                ShootTime = 0.0f;
                return;
            }            
            isMouseClick = true;
            attackState = AttackState.firstDelay;
        }

        if (isMouseClick == true)
        {
            ShootTime += Time.deltaTime;            
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
                        if (gameMgr.CurrentBullet > 0) // 연속으로 발사할 시 모니터링이 안되서 여기서 한번더 체크
                        {
                            fireState = FireState.Fireing;
                            // 애니메이션 종료 여부 확인
                            if (playerConRef.playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot_SingleShot_AR") == false)
                            {
                                attackState = AttackState.LastDelay;
                            }
                        }
                        else 
                        {
                            fireState = FireState.StopFire;
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
        if (pv.IsMine && Input.GetMouseButton(0) && playerConRef.isReload == false && isCShot == false)
        {
            //if (gameMgrRef.CurrentBullet > 0)
            //{
            //    isMouseClick = true;
            //    isCShot = true;
            //    attackState = AttackState.firstDelay;
            //}
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
            //if (gameMgrRef.CurrentBullet > 0)
            //{
            //    isMouseClick = true;
            //    attackState = AttackState.firstDelay;
            //}
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
        if (pv.IsMine)
        {
            gameMgr.GunCount();
            CreateBullet();            
        }

        pv.RPC("CreateBullet", RpcTarget.Others, null);
    }

    [PunRPC]
    void CreateBullet()
    {
        GameObjSound.PlayOneShot(GunSfx, 1.0f);
        GameObject bulletRef = Instantiate(bullet, firePos.position, firePos.rotation);
        bulletRef.GetComponent<MultiBulletCtrl>().MakerId = MyPvId; // 쏜사람 아이디 저장

        //// 일반 탄환
        //if (m_gunKind == GunKind.Basic)
        //{
        //    GameObjSound.PlayOneShot(GunSfx, 1.0f);
        //    Instantiate(bullet, firePos.position, firePos.rotation);
        //}
        //else if (m_gunKind == GunKind.Continue)
        //{
        //    GameObjSound.PlayOneShot(GunSfx, 1.0f);
        //    Instantiate(bullet, firePos.position, firePos.rotation);
        //}
        //else if (m_gunKind == GunKind.Missile)
        //{
        //    GameObjSound.PlayOneShot(MissileSfx, 1.0f);
        //    Instantiate(Missile, firePos.position, firePos.rotation);
        //}

        // 잠시 기다리는 루틴을 위해 코루틴 함수로 호출
        StartCoroutine(this.ShowMuzzleFlash());
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