using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public enum GameState
    {
        GameIng,
        GameEnd,
        GameOver,
    }

    public static GameState gameState = GameState.GameIng;

    [Header("InGameUIs")]
    public Image hpImg;
    public Text CurrentWeaponText;
    public Text TotalBulletText;
    public Text BulletText;
    public Text QuestText;
    public Text GoldText;
    public Text MineralText;

    [Header("GameEndUIs")]
    public GameObject GameEndPanel;
    public Text GameEndGoldText;
    public Text GameEndMineralText;
    public Button ReplayGameBtn;
    public Button GoLobbyBtn;
    public Button GoLabBtn;

    [Header("GameOverUIs")]
    public GameObject GameOverPanel;
    public Button GameOverReplayBtn;
    public Button GameOverGoLobbyBtn;
    public Button GameOverGoLabBtn;

    // 각종 변수들
    int questRate = 0;
    int Killnum = 0;
    int Gold = 0;
    int mineral = 0;
    string WeaponName = "";
    internal int CurrentBullet = 0;
    internal int ReloadStandardBullet = 0;

    // 각 총알 리로드 상태 확인
    internal bool isBNeedReload = false;
    internal bool isCNeedReload = false;
    internal bool isSNeedReload = false;

    int GoldIncre = 10;
    int MineralIncre = 10;

    bool isLock = false;                    // DB 데이터 통신 여부
    bool isDBProcess = false;               // DB 데이터 처리 여부

    // 총알 확인하기
    int[] HasTotBullet;
    int[] HasCurBullet;
    FireController fireRef;

    // Start is called before the first frame update
    void Start()
    {
        // 처음 시작할 때, hp는 다 차있는 상태
        hpImg.fillAmount = 1.0f;

        fireRef = GameObject.Find("Player").GetComponentInChildren<FireController>();

        #region 버튼 모음
        if (ReplayGameBtn != null)
            ReplayGameBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene");
            });

        if (GoLobbyBtn != null)
            GoLobbyBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LobbyScene");
            });

        if (GoLabBtn != null)
            GoLabBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LabScene");
            });

        if (GameOverReplayBtn != null)
            GameOverReplayBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene");
            });

        if (GameOverGoLobbyBtn != null)
            GameOverGoLobbyBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LobbyScene");
            });

        if (GameOverGoLabBtn != null)
            GameOverGoLabBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LabScene");
            });
        #endregion

        #region DB 정보 가져오기 - 총기탄약 정보

        HasCurBullet = new int[(int)GunType.GunCount + 1];
        HasTotBullet = new int[(int)GunType.GunCount];
        string query = "";
        if (UserInfo.g_Unique_ID != "")
        {
            query = $"select * from User_Bullet where uno = '{UserInfo.g_Unique_ID}'";
            MySQLConnect sqlcon = new MySQLConnect();
            DataTable dt = sqlcon.selsql(query);

            HasCurBullet[0] = 25; // 기본 탄약 넣기
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < (int)GunType.GunCount; i++)
                {
                    int temp = 0;
                    int.TryParse(dt.Rows[0][1 + i].ToString(), out temp); // 컬럼 값이 0이 uno, 1이 연발총, 2가 샷건이다.
                    HasTotBullet[i] = temp;
                    if (i == 0)
                    {
                        UserInfo.UserCBullet = HasTotBullet[i];
                        HasCurBullet[i + 1] = 100; // 연발총 기본 탄약넣기
                    }
                    else if (i == 1)
                    {
                        UserInfo.UserMBullet = HasTotBullet[i];
                        HasCurBullet[i + 1] = 20; // 미사일 기본 탄약 넣기
                    }
                }
            }
        }

        #endregion

        // 각종 변수들 초기화
        questRate = 10;
        QuestText.text = $"{Killnum}/{questRate}";
        GoldText.text = $"골드 : {Gold}";
        MineralText.text = $"미네랄 : {mineral}";
        WeaponName = "기본총";
        CurrentWeaponText.text = $"현재 총 : {WeaponName}";
        TotalBulletText.text = $"무한";
        ReloadStandardBullet = 25;
        CurrentBullet = ReloadStandardBullet;
        BulletText.text = $"{CurrentBullet}/{ReloadStandardBullet}";

        gameState = GameState.GameIng;

        // 마우스 감춤 처리
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined; // 게임 창 밖으로 마우스가 안나감
        Cursor.visible = false;
        //Esc키를 누르면 커서가 창 밖으로 나가게 할 수 있다.
#endif

        // 마우스 감춤 처리
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.GameEnd)
        {
            if (isLock == false)
            {
                if (isDBProcess == false)
                {
                    GameEndDBProcess();
                }//if (isDBProcess == false) 
            }//if (isLock == false) 
        }//if (gameState == GameState.GameEnd)
    }

    public void HeroHpView(int Curhp, int Maxhp)
    {
        hpImg.fillAmount = (float)Curhp / (float)Maxhp;
    }

    public void GetGold()
    {
        Gold += GoldIncre;
        GoldText.text = $"골드 : {Gold}";
    }

    public void GetMineral()
    {
        mineral += MineralIncre;
        MineralText.text = $"미네랄 : {mineral}";
    }

    public void GunCount(FireController.GunKind a_GunKind)
    {
        if (a_GunKind == FireController.GunKind.Basic)
        {
            HasCurBullet[0] -= 1;
            if (HasCurBullet[0] <= 0)
            {
                HasCurBullet[0] = 0;
                isBNeedReload = true;
            }
            BulletText.text = $"{HasCurBullet[0]}/{ReloadStandardBullet}";
        }
        else if (a_GunKind == FireController.GunKind.Continue)
        {
            HasCurBullet[1] -= 1;
            if (HasCurBullet[1] <= 0)
            {
                HasCurBullet[1] = 0;
                isCNeedReload = true;
            }

            HasTotBullet[0] -= 1;
            if (HasTotBullet[0] <= 0)
            {
                HasTotBullet[0] = 0;
            }
            TotalBulletText.text = $"총알 보유량 : {HasTotBullet[0]}";
            BulletText.text = $"{HasCurBullet[1]}/{ReloadStandardBullet}";
        }
        else if (a_GunKind == FireController.GunKind.Missile)
        {
            HasCurBullet[2] -= 1;
            if (HasCurBullet[2] <= 0)
            {
                HasCurBullet[2] = 0;
                isSNeedReload = true;
            }

            HasTotBullet[1] -= 1;
            if (HasTotBullet[1] <= 0)
            {
                HasTotBullet[1] = 0;
            }
            TotalBulletText.text = $"총알 보유량 : {HasTotBullet[1]}";
            BulletText.text = $"{HasCurBullet[2]}/{ReloadStandardBullet}";
        }
    }

    public void GunReload(FireController.GunKind a_GunKind)
    {
        // 리로딩 시 사격 상태를 초기화 한다.
        fireRef.fireState = FireController.FireState.StopFire;
        fireRef.attackState = FireController.AttackState.firstDelay;

        if (a_GunKind == FireController.GunKind.Basic)
        {
            isBNeedReload = false;
            HasCurBullet[0] = ReloadStandardBullet;
            BulletText.text = $"{HasCurBullet[0]}/{ReloadStandardBullet}";
        }//if (a_GunKind == FireController.GunKind.Basic) 
        else if (a_GunKind == FireController.GunKind.Continue)
        {
            isCNeedReload = false;
            if (HasCurBullet[1] < HasTotBullet[0] && HasTotBullet[0] > ReloadStandardBullet)
            {
                HasCurBullet[1] = ReloadStandardBullet;
                BulletText.text = $"{HasCurBullet[1]}/{ReloadStandardBullet}";
            }
            else
            {
                HasCurBullet[1] = HasTotBullet[0];
                BulletText.text = $"{HasCurBullet[1]}/{ReloadStandardBullet}";
            }
        }
        else if (a_GunKind == FireController.GunKind.Missile)
        {
            isSNeedReload = false;
            if (HasCurBullet[2] < HasTotBullet[1] && HasTotBullet[1] > ReloadStandardBullet)
            {
                HasCurBullet[2] = ReloadStandardBullet;
                BulletText.text = $"{HasCurBullet[2]}/{ReloadStandardBullet}";
            }
            else
            {
                HasCurBullet[2] = HasTotBullet[1];
                BulletText.text = $"{HasCurBullet[2]}/{ReloadStandardBullet}";
            }
        }//else if (a_GunKind != FireController.GunKind.ShotGun)
    }//public void GunReload(FireController.GunKind a_GunKind)

    public void QuestCount()
    {
        if (gameState == GameState.GameIng)
        {
            Killnum += 1;
            QuestText.text = $"{Killnum}/{questRate}";
            if (Killnum == questRate)
            {
                gameState = GameState.GameEnd;
                GameEndPanel.SetActive(true);

                GameEndGoldText.text = $"획득한 골드 : {Gold}";
                GameEndMineralText.text = $"획득한 미네랄 : {mineral}";

                // 마우스 뷰 처리
#if UNITY_EDITOR
                Cursor.lockState = CursorLockMode.None; // 게임 창 밖으로 마우스가 안나감
                Cursor.visible = true;
                //Esc키를 누르면 커서가 창 밖으로 나가게 할 수 있다.
#endif
                // 마우스 뷰 처리
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }//if (gameState == GameState.GameIng)
    }

    public void GameEndDBProcess()
    {
        if (UserInfo.g_Unique_ID == "")
            return;

        isLock = true;

        UserInfo.UserGold += Gold;
        UserInfo.UserMineral += mineral;

        // DB로 값을 넘기는 부분
        string Query = $"INSERT INTO User_Gold(uno, Gold, Mineral)" +
            $" VALUES('{UserInfo.g_Unique_ID}','{UserInfo.UserGold}','{UserInfo.UserMineral}')" +
            $" ON DUPLICATE KEY UPDATE Gold='{UserInfo.UserGold}', Mineral='{UserInfo.UserMineral}'; ";

        MySQLConnect mysqlTestRef = new MySQLConnect();
        mysqlTestRef.sqlcmdSel(Query);

        isLock = false;
        isDBProcess = true;
    }

    internal void GameOverFunc()
    {
        GameOverPanel.SetActive(true);

        // 마우스 뷰 처리
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined; // 게임 창 밖으로 마우스가 안나감
        Cursor.visible = true;
        //Esc키를 누르면 커서가 창 밖으로 나가게 할 수 있다.
#endif

        // 마우스 뷰 처리
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    internal void SwitchGun(FireController.GunKind a_GunKind)
    {
        if (a_GunKind == FireController.GunKind.Basic)
        {
            WeaponName = "기본총";
            ReloadStandardBullet = 25;
            CurrentWeaponText.text = $"현재 총 : {WeaponName}";
            TotalBulletText.text = $"무한";
            CurrentBullet = HasCurBullet[0];
            BulletText.text = $"{HasCurBullet[0]}/{ReloadStandardBullet}";
        }
        else if (a_GunKind == FireController.GunKind.Continue)
        {
            WeaponName = "연발총";
            ReloadStandardBullet = 100;
            CurrentWeaponText.text = $"현재 총 : {WeaponName}";
            CurrentBullet = HasCurBullet[1];
            TotalBulletText.text = $"총알 보유량 : {HasTotBullet[0]}";
            BulletText.text = $"{HasCurBullet[1]}/{ReloadStandardBullet}";
        }
        else if (a_GunKind == FireController.GunKind.Missile)
        {
            WeaponName = "미사일";
            ReloadStandardBullet = 20;
            CurrentWeaponText.text = $"현재 총 : {WeaponName}";
            TotalBulletText.text = $"총알 보유량 : {HasTotBullet[1]}";
            BulletText.text = $"{HasCurBullet[2]}/{ReloadStandardBullet}";
        }
    }
}
