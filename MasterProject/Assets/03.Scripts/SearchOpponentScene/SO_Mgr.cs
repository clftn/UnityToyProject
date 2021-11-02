using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SO_Mgr : MonoBehaviour
{
    #region ---------변수 선언
    [Header("Opponent List")]
    public GameObject Opponents_SV_Content;         //Scroll View Content
    public GameObject OpponentPrefab;       //Opponent Info Prefab
    public GameObject TouchCover_Panel;     //TouchCover                            //Opponents_SV가 터치가 안되도록

    [System.Serializable]
    public struct OI_Panel
    {
        public GameObject OpponentInfo_Panel;   //UserInfo_Panel
        [Tooltip("Opponent Info Nick UI text / '상대 닉네임' :")]
        public Text OI_Nick_txt;
        [Tooltip("Opponent Info UserNo")]
        [HideInInspector] public int OI_UserNo;
        [Tooltip("Opponent Info Win UI text / '상대 승 수'승")]
        public Text OI_Win_txt;
        [Tooltip("Opponent Info Defeat UI text / '상대 패 수'패")]
        public Text OI_Defeat_txt;
        [Tooltip("Opponent Info UnitPower UI text / '유닛 전투력'")]
        public Text OI_UnitPower_txt;
        [Tooltip("Opponent Info UserItem.Count UI text / '상대 유닛 수'개")]
        public Text OI_UserItemCount_txt;
        public Button Fight_Btn;                //Opponent Fight Btn                    //공격 시작
        public Button Cancel_Btn;               //Opponent Cancle Btn                   //취소
        public static bool OI_OnOff = false;
    }

    public List<UserInfo> SO_List = new List<UserInfo>();
    public List<DeckInfo> Deck_List = new List<DeckInfo>();
    public UserInfo Fight_SO_Info;

    public Image BackImg;

    [Header("Opponent Info Panel")]
    [Tooltip("툴팁 : 기능 / UI 예시")]
    public OI_Panel OI;

    [Header("Other UI")]
    public Button Go_Lobby_Btn;             //Go Lobby Btn                          //로비로 돌아가기

    [Header("ConfigObj")]
    public Button Setting_Btn;              //Setting Btn                           //환경설정버튼

    [Header("Deck")]
    public GameObject Deck_Panel;
    public GameObject DeckCover_Panel;
    public GameObject Deck_SV_Content;
    public GameObject DeckObj;
    public Button DeckEdit_Btn;
    [HideInInspector] public bool DeckEditBool = false;
    public GameObject[] DecNum_Img_Obj;
    public Sprite[] DecNum_Img;

    [Header("FadeInOut Object")]
    public GameObject LeftFade;
    public GameObject RightFade;
    public GameObject PanelFade;
    public GameObject DeckFade;
    public GameObject UpFade;
    bool FadeIn_Bool = true;    //Start()시 FadeIn이 끝났는지 확인하는 변수

    [Header("Audio")]
    public AudioClip[] m_audioclip;
    new AudioSource audio;

    Vector3 LeftFadePos = new Vector3(-820, 0, 0);
    Vector3 RightFadePos = new Vector3(500, 0, 0);
    Vector3 UpFadePos = new Vector3(0, 145, 0);
    [HideInInspector] public Vector3 PanelFadePos = new Vector3(500, 0, 0);     //OI_Item.cs에서 참조하기위해 public
    [HideInInspector] public Vector3 DeckFadePos = new Vector3(0, -600, 0);      //OI_Item.cs에서 참조하기위해 public

    float FadeTime = 1.5f;

    //DB    
    [Header("DB")]
    public static SO_Mgr Inst;
    public GameObject DialogBox;
    public GameObject ServerRequest_Dlg;
    GameObject SR_Obj;
    GameObject Dlg;
    #endregion

    private void Awake()
    {
        Inst = this;
        audio = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        OI_Panel.OI_OnOff = false;

        if (OI.OpponentInfo_Panel != null)
        {
            if (Go_Lobby_Btn != null)
                Go_Lobby_Btn.onClick.AddListener(Go_LobbyBtnClick);


            if (OI.Cancel_Btn != null)
                OI.Cancel_Btn.onClick.AddListener(Cancle);
        }

        if (Deck_Panel != null)
        {
            if (DeckEdit_Btn != null)
                DeckEdit_Btn.onClick.AddListener(EditDeck);
        }

        if (Setting_Btn != null)
            Setting_Btn.onClick.AddListener(Config_Setting);

        if (OI.Fight_Btn != null)
            OI.Fight_Btn.onClick.AddListener(Fight);

        FadeIn_Bool = true;

        DBConnect();
        Deck_Img_Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (TouchCover_Panel != null)
            TouchCover_Panel.SetActive(OI_Panel.OI_OnOff);
    }

    #region =======LeftFade
    void DBConnect()
    {
        SR_Obj = (GameObject)Instantiate(Resources.LoadAsync("ServerRequest_Canvas").asset);
        SR_Obj.GetComponent<ServerRequest>().TipStr = "Tip : 유닛 전투력 = (유닛별 레벨 * 유닛별 공격력)들의 총합입니다.";
        GlobalValue.ServerRequestCount = 0;
        SR_Obj.SetActive(false);
        StartCoroutine(UserInfo_DBConnect());
    }

    IEnumerator UserInfo_DBConnect()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", MyInfo.m_ID, System.Text.Encoding.UTF8);    //유저의 아이디        
        UnityWebRequest a_www = UnityWebRequest.Post(GlobalValue.OI_Url, form);
        a_www.timeout = 1;
        yield return a_www.SendWebRequest();    //응답이 올 때까지 대기하기...

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            //Debug.Log(sz);
            if (sz.Contains("ID does not") == true)
            {
                Debug.Log("대전 상대가 없음");
                Destroy(SR_Obj);
                yield break;
            }

            if (!sz.Contains("DB Connect"))
            {
                Debug.Log("DB 접속 안됨");
                Destroy(SR_Obj);
                yield break;
            }

            //JSON 파싱
            var N = JSON.Parse(sz);
            if (N == null)
                yield break;

            if (N["RkList"].Count <= 10)   //전체 유저가 10명 이하일 때
            {
                for (int i = 0; i < N["RkList"].Count; i++)
                {
                    UserInfo m_UI_Nd = new UserInfo();
                    m_UI_Nd.m_ID = N["RkList"][i]["UserId"];
                    m_UI_Nd.m_No = N["RkList"][i]["UserNo"].AsInt;
                    m_UI_Nd.m_Nick = N["RkList"][i]["UserNick"];
                    m_UI_Nd.m_Win = N["RkList"][i]["UserWin"].AsInt;
                    m_UI_Nd.m_Defeat = N["RkList"][i]["UserDefeat"].AsInt;
                    m_UI_Nd.m_Gold = N["RkList"][i]["UserGold"].AsInt;
                    m_UI_Nd.m_UnitCount = N["RkList"][i]["UnitCount"].AsInt;
                    m_UI_Nd.m_UnitPower = 0;
                    if (m_UI_Nd.m_UnitCount > 0)
                    {
                        for (int j = 0; j < m_UI_Nd.m_UnitCount; j++)
                        {
                            int UnitLevel = N["RkList"][i]["Unit"][j]["Level"].AsInt;
                            int UnitAttack = N["RkList"][i]["Unit"][j]["UnitAttack"].AsInt;
                            m_UI_Nd.m_UnitPower += (UnitLevel + UnitAttack);    //레벨 + 유닛공격력                            
                        }
                    }
                    else
                    {
                        m_UI_Nd.m_UnitCount = 0;
                        m_UI_Nd.m_UnitPower = 0;
                    }
                    CreateList(m_UI_Nd);
                }
            }
            else if (N["RkList"].Count > 10)    //테스트완료
            {
                int[] O_List = GetRandomInt(10, 0, N["RkList"].Count);
                for (int i = 0; i < O_List.Length; i++)
                {
                    UserInfo m_UI_Nd = new UserInfo();
                    m_UI_Nd.m_ID = N["RkList"][O_List[i]]["UserId"];
                    m_UI_Nd.m_No = N["RkList"][O_List[i]]["UserNo"].AsInt;
                    m_UI_Nd.m_Nick = N["RkList"][O_List[i]]["UserNick"];
                    m_UI_Nd.m_Win = N["RkList"][O_List[i]]["UserWin"].AsInt;
                    m_UI_Nd.m_Defeat = N["RkList"][O_List[i]]["UserDefeat"].AsInt;
                    m_UI_Nd.m_Gold = N["RkList"][O_List[i]]["UserGold"].AsInt;
                    m_UI_Nd.m_UnitCount = N["RkList"][O_List[i]]["UnitCount"].AsInt;
                    m_UI_Nd.m_UnitPower = 0;
                    if (m_UI_Nd.m_UnitCount > 0)
                    {
                        for (int j = 0; j < m_UI_Nd.m_UnitCount; j++)
                        {
                            int UnitLevel = N["RkList"][O_List[i]]["Unit"][j]["Level"].AsInt;
                            int UnitAttack = N["RkList"][O_List[i]]["Unit"][j]["UnitAttack"].AsInt;
                            m_UI_Nd.m_UnitPower += (UnitLevel + UnitAttack);    //레벨 + 유닛공격력                            
                        }
                    }
                    else
                    {
                        m_UI_Nd.m_UnitCount = 0;
                        m_UI_Nd.m_UnitPower = 0;
                    }
                    CreateList(m_UI_Nd);
                }
            }

            //덱
            if (N["DeckCount"] > 0)
            {
                GlobalValue.My_DeckInfo = null;
                for (int i = 0; i < N["DeckCount"].AsInt; i++)
                {
                    DeckInfo m_DecNode = new DeckInfo();

                    m_DecNode.UserDecN = N["DeckList"][i]["UserDecN"].AsInt;

                    m_DecNode.UserDec[0] = N["DeckList"][i]["UserDec1"].AsInt;
                    m_DecNode.UserDec[1] = N["DeckList"][i]["UserDec2"].AsInt;
                    m_DecNode.UserDec[2] = N["DeckList"][i]["UserDec3"].AsInt;
                    m_DecNode.UserDec[3] = N["DeckList"][i]["UserDec4"].AsInt;
                    m_DecNode.UserDec[4] = N["DeckList"][i]["UserDec5"].AsInt;

                    m_DecNode.UserN = N["DeckList"][i]["UserN"].AsInt;

                    m_DecNode.UserDec_Num[0] = N["DeckList"][i]["UserDec1_Num"].AsInt;
                    m_DecNode.UserDec_Num[1] = N["DeckList"][i]["UserDec2_Num"].AsInt;
                    m_DecNode.UserDec_Num[2] = N["DeckList"][i]["UserDec3_Num"].AsInt;
                    m_DecNode.UserDec_Num[3] = N["DeckList"][i]["UserDec4_Num"].AsInt;
                    m_DecNode.UserDec_Num[4] = N["DeckList"][i]["UserDec5_Num"].AsInt;

                    m_DecNode.UserDecCount = N["DeckList"][i]["UserDecCount"].AsInt;

                    CreateDeckList(m_DecNode);
                    m_DecNode = null;
                }
            }

            //덱
            //GlobalValue.ServerRequestCount = 0;
            Destroy(SR_Obj);

            //FadeIn
            if (RightFade != null)
                StartCoroutine(FadeIn(RightFade, RightFadePos));

            if (UpFade != null)
                StartCoroutine(FadeIn(UpFade, UpFadePos));

            if (LeftFade != null)
                yield return StartCoroutine(FadeIn(LeftFade, LeftFadePos));

            FadeIn_Bool = false;

            //FadeIn
        }
        else
        {
            Debug.Log(a_www.error);
            SR_Obj.SetActive(true);
            GlobalValue.ServerRequestCount++;
            if (GlobalValue.ServerRequestCount > 5)
            {
                Debug.Log("접속 실패");
                if (GlobalValue.ServerRequestCount == 6)
                {
                    Instantiate(ServerRequest_Dlg, SR_Obj.transform).GetComponent<DialogBoxCtrl>().MsgDlg("서버 연결에 실패하였습니다.\n타이틀 씬으로 돌아갑니다.", () => SceneManager.LoadScene("TitleScene"));
                }
            }
            else
                StartCoroutine(UserInfo_DBConnect());
        }
    }

    int[] GetRandomInt(int length, int min, int max)
    {
        int[] randArray = new int[length];
        bool isSame;
        for (int i = 0; i < length; ++i)
        {
            while (true)
            {
                randArray[i] = Random.Range(min, max);
                isSame = false;
                for (int j = 0; j < i; ++j)
                {
                    if (randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame)
                    break;
                else
                    continue;
            }
        }
        return randArray;
    }

    void CreateList(UserInfo UI)
    {
        UserInfo m_UI = new UserInfo();
        GameObject OI_Obj = Instantiate(OpponentPrefab, Opponents_SV_Content.transform);
        m_UI.m_ID = UI.m_ID;
        m_UI.m_No = UI.m_No;
        m_UI.m_Nick = UI.m_Nick;
        m_UI.m_Win = UI.m_Win;
        m_UI.m_Defeat = UI.m_Defeat;
        m_UI.m_Gold = UI.m_Gold;
        m_UI.m_UnitCount = UI.m_UnitCount;
        m_UI.m_UnitPower = UI.m_UnitPower * 10;     //총 유닛별 전투력의 곱 10
        OI_Obj.GetComponent<OI_Item>().OI_Item_UserInfo = m_UI;
        OI_Obj.GetComponent<OI_Item>().OpponentNick_txt.text = UI.m_Nick + " :";
        OI_Obj.GetComponent<OI_Item>().U_Win.text = UI.m_Win.ToString() + "승";
        OI_Obj.GetComponent<OI_Item>().U_Defeat.text = UI.m_Defeat.ToString() + "패";
        SO_List.Add(m_UI);
        m_UI = null;
    }

    void CreateDeckList(DeckInfo m_Dec)
    {
        GameObject DO = Instantiate(DeckObj, Deck_SV_Content.transform);
        DO.GetComponent<DecNode>().m_DecInfo = m_Dec;

        if (m_Dec.UserDecCount != GlobalValue.My_DeckIdx)
            DO.GetComponent<Toggle>().isOn = false;
        else
            DO.GetComponent<Toggle>().isOn = true;

        DO.GetComponent<Toggle>().group = Deck_SV_Content.GetComponent<ToggleGroup>();  //덱 그룹 설정

        DeckInfo m_DeckNode = new DeckInfo();

        m_DeckNode.UserDecN = m_Dec.UserDecN;
        m_DeckNode.UserN = m_Dec.UserN;
        m_DeckNode.UserDecCount = m_Dec.UserDecCount;

        for (int i = 0; i < 5; i++)
        {
            m_DeckNode.UserDec[i] = m_Dec.UserDec[i];
            m_DeckNode.UserDec_Num[i] = m_Dec.UserDec_Num[i];
        }

        Deck_List.Add(m_DeckNode);


        m_DeckNode = null;
    }
    #endregion

    #region =======RightFade

    void EditDeck()
    {
        //UI씬으로 이동
        Instantiate(DialogBox, BackImg.transform).GetComponent<DialogBoxCtrl>().MsgDlg("덱을 편집하기위해 마이룸으로 이동하시겠습니까?", () => SceneManager.LoadScene("LoadingGoMyRoom"));
    }

    void Fight()
    {
        if (GlobalValue.My_DeckIdx != -1)            //선택한 덱이 있을 때
        {
            audio.PlayOneShot(m_audioclip[0]);

            GlobalValue.SO_Info = Fight_SO_Info;
            StartCoroutine(Go_InGame());
        }
        else if (GlobalValue.My_DeckIdx == -1)       //선택한 덱이 없을 때
        {
            audio.PlayOneShot(m_audioclip[0]);

            if (Deck_List != null)    //덱은 있지만 선택을 안했을 때
                Instantiate(ServerRequest_Dlg, BackImg.transform).GetComponent<DialogBoxCtrl>().MsgDlg("선택된 덱이 없습니다.\n덱을 선택해 주십시오.");
            else                                    //덱이 없을 때
                Instantiate(ServerRequest_Dlg, BackImg.transform).GetComponent<DialogBoxCtrl>().MsgDlg("보유한 덱이 없습니다.\n덱을 생성해 주십시오.");
        }
    }

    void Go_LobbyBtnClick()
    {
        if (FadeIn_Bool)
            return;

        audio.PlayOneShot(m_audioclip[0]);
        StartCoroutine(GO_Lobby());
    }

    public void Deck_Img_Update()
    {
        if (GlobalValue.My_DeckIdx != -1)
        {
            for (int i = 0; i < DecNum_Img_Obj.Length; i++)
            {
                //Debug.Log(i);
                //Debug.Log(i + "번 " + GlobalValue.My_DeckInfo.UserDec[i]);

                if (GlobalValue.My_DeckInfo.UserDec[i] == -1)       //덱에 탱크가 배치안됐을 때
                    DecNum_Img_Obj[i].GetComponent<Image>().sprite = DecNum_Img[0];
                else
                    DecNum_Img_Obj[i].GetComponent<Image>().sprite = DecNum_Img[GlobalValue.My_DeckInfo.UserDec[i]];
            }
        }
        else
        {
            for (int i = 0; i < DecNum_Img_Obj.Length; i++)
            {
                DecNum_Img_Obj[i].GetComponent<Image>().sprite = DecNum_Img[0];
            }
        }
    }

    IEnumerator GO_Lobby()
    {
        StartCoroutine(FadeOut(RightFade, RightFadePos));
        StartCoroutine(FadeOut(UpFade, UpFadePos));
        yield return StartCoroutine(FadeOut(LeftFade, LeftFadePos));
        //제일 FadeOut이 오래걸리는 LeftFadeOut이 끝나면 씬이동
        SceneManager.LoadScene("LobbyScene");
        yield break;
    }

    IEnumerator Go_InGame()
    {
        StartCoroutine(FadeOut(RightFade, RightFadePos));
        StartCoroutine(FadeOut(UpFade, UpFadePos));
        yield return StartCoroutine(FadeOut(LeftFade, LeftFadePos));

        int rand = Random.Range(0, 2);
        string map_Str = "";
        if (rand == 0)
            map_Str = "InGame_Map1";
        else
            map_Str = "InGame_Map2";

        GlobarValue.g_UserMap = (UserMap)rand;
        SceneManager.LoadScene(map_Str);
    }

    IEnumerator InfoPanel_FadeOut(GameObject FadeObj, Vector3 FadePos)
    {
        yield return StartCoroutine(FadeOut(FadeObj, FadePos)); //FadeOut(FadeObj, FadePos)코루틴이 끝나면 넘어감
        OI_Panel.OI_OnOff = false;    //DeckPanel_FadeIn가 더 오래걸리기에 주석처리
    }

    IEnumerator DeckPanel_FadeIn(GameObject FadeObj, Vector3 FadePos)
    {
        yield return StartCoroutine(FadeIn(FadeObj, FadePos)); //FadeIn(FadeObj, FadePos)코루틴이 끝나면 넘어감
        //DeckEditBool = false;
    }
    #endregion

    #region =======UpFade
    void Config_Setting()
    {
        audio.PlayOneShot(m_audioclip[0]);
        /*GameObject Config = (GameObject)*/
        Instantiate(Resources.Load("Config_Canvas"));
    }

    void Cancle()
    {
        StartCoroutine(InfoPanel_FadeOut(PanelFade, PanelFadePos));
        StartCoroutine(DeckPanel_FadeIn(DeckFade, DeckFadePos));
    }

    #endregion

    #region =======FadeIn,Out
    public IEnumerator FadeIn(GameObject FadeObj, Vector3 FadeInPos)
    {
        FadeObj.transform.localPosition = FadeInPos;
        while (FadeObj.transform.localPosition.normalized != -FadeInPos.normalized)
        {
            FadeObj.transform.Translate(-FadeInPos.normalized * 20f, Space.Self);
            yield return new WaitForSeconds(FadeTime / FadeInPos.magnitude);
        }
        FadeObj.transform.localPosition = Vector3.zero;
        yield break;
    }

    public IEnumerator FadeOut(GameObject FadeObj, Vector3 FadeOutPos)
    {
        if (FadeObj.transform.localPosition != FadeOutPos)
        {
            while (FadeObj.transform.localPosition.magnitude < FadeOutPos.magnitude)
            {
                FadeObj.transform.Translate(FadeOutPos.normalized * 20f, Space.Self);
                yield return new WaitForSeconds(FadeTime / FadeOutPos.magnitude);
            }
            FadeObj.transform.localPosition = FadeOutPos;
            yield break;
        }
    }
    #endregion

}