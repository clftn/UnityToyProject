using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class Title_Mgr : MonoBehaviour
{
    [Header("LoginPanel")]              //이렇게 쓰면 편집창에 태그들이 나온다. 
    public GameObject m_LoginPanelObj;
    public InputField IDInputField;     //Email 로 받을 것임
    public InputField PassInputField;
    public Button m_LoginBtn = null;
    public Button m_CreateAccOpenBtn = null;

    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanelObj;
    public InputField New_IDInputField;  //Email 로 받을 것임
    public InputField New_PassInputField;
    public InputField New_NickInputField;
    public Button m_CABtn = null;
    public Button m_CancelBtn = null;

    string LoginUrl;
    string CreateUrl;
    string LoginUserAttDataUrl;
    string LoginUserDefDataUrl;
    string CreateDataInsertUrl;

    // 로딩 화면 띄우기
    GameObject SR_Obj;
    bool isNetwork = false;

    // 회원 가입 시 네트워크 막기
    bool isCreate = false;

    // 다이얼로그 박스 띄우기
    public Transform DlgParent;

    //로그인패널, 회원가입패널 InputField Focus이동 판단 변수
    //전용 다이얼로그
    InputField[] LogIn_IF = new InputField[1];
    int LogIn_IF_Idx = 0;
    bool Login_Enter = true;    //다이얼로그박스 중복생성 방지

    InputField[] CA_IF = new InputField[2];
    int CA_IF_Idx = 0;
    bool CA_Enter = true;    //다이얼로그박스 중복생성 방지
    //로그인패널, 회원가입패널 InputField Focus이동 판단 변수

    // Start is called before the first frame update
    void Start()
    {
        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(CreateCancelBtn);

        if (m_CABtn != null)
            m_CABtn.onClick.AddListener(CreateAccountBtn);

        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);

        LogIn_IF = m_LoginPanelObj.GetComponentsInChildren<InputField>();
        CA_IF = m_CreateAccPanelObj.GetComponentsInChildren<InputField>();

        LoginUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/Login.php";
        CreateUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/CreateAccount.php";
        LoginUserAttDataUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/LoginUserAttData.php";
        LoginUserDefDataUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/LoginUserDefData.php";
        CreateDataInsertUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/CreateDataInsert.php";
        Login_Enter = true;
        CA_Enter = true;
    }

    // Update is called once per frame
    void Update()
    {
        Focus();
    }

    void Focus()
    {
        if (LogIn_IF != null && m_LoginPanelObj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                for (int i = 0; i < LogIn_IF.Length; i++)
                {
                    if (LogIn_IF[i].isFocused)
                    {
                        LogIn_IF_Idx = i;
                    }
                }
                if (LogIn_IF_Idx + 1 >= LogIn_IF.Length)
                    LogIn_IF[0].Select();
                else
                    LogIn_IF[LogIn_IF_Idx + 1].Select();
            }
            else if (Input.GetKeyDown(KeyCode.Return) && Login_Enter)
            {
                LoginBtn();
            }
        }
        else if (CA_IF != null && m_CreateAccPanelObj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                for (int i = 0; i < CA_IF.Length; i++)
                {
                    if (CA_IF[i].isFocused)
                    {
                        CA_IF_Idx = i;
                    }
                }
                if (CA_IF_Idx + 1 >= CA_IF.Length)
                    CA_IF[0].Select();
                else
                    CA_IF[CA_IF_Idx + 1].Select();
            }
            else if (Input.GetKeyDown(KeyCode.Return) && CA_Enter)
            {
                CreateAccountBtn();
            }
        }
    }

    public void OpenCreateAccBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(false);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(true);
    }

    public void CreateCancelBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(true);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(false);
    }

    public void CreateAccountBtn() //계정 생성 요청 함수
    {
        string a_IdStr = New_IDInputField.text;
        string a_PwStr = New_PassInputField.text;
        string a_NickStr = New_NickInputField.text;

        if (string.IsNullOrEmpty(a_IdStr.Trim()))
        {
            // 메세지 띄우기
            CA_Enter = false;
            GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "아이디 값 비어있음", () => CA_Enter = true);
            return;
        }

        if (string.IsNullOrEmpty(a_PwStr.Trim()))
        {
            // 메세지 띄우기
            CA_Enter = false;
            GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "비밀번호 값 비어있음", () => CA_Enter = true);
            return;
        }

        if (string.IsNullOrEmpty(a_NickStr.Trim()))
        {
            // 메세지 띄우기
            CA_Enter = false;
            GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "닉네임 값 비어있음", () => CA_Enter = true);
            return;
        }

        if (isCreate == false)
        {
            isCreate = true;
            StartCoroutine(CreateCo(a_IdStr, a_PwStr, a_NickStr));
        }
        else
        {
            // 메세지 띄우기
            CA_Enter = false;
            GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "아이디 생성 통신중", () => CA_Enter = true);
        }
    }

    IEnumerator CreateCo(string a_IdStr, string a_PwStr, string a_NickStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", a_PwStr);
        form.AddField("Input_nick", a_NickStr, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            // 각종 상황 예외처리
            if (sz.Contains("Could not Connect"))
            {
                isCreate = false;
                CA_Enter = false;
                GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "DB 접속 문제 발생", () => CA_Enter = true);
            }
            else if (sz.Contains("ID does exist."))
            {
                isCreate = false;
                CA_Enter = false;
                GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "중복된 아이디 생성", () => CA_Enter = true);
            }
            else if (sz.Contains("Nickname does exist."))
            {
                isCreate = false;
                CA_Enter = false;
                GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경   고", "중복된 닉네임 생성", () => CA_Enter = true) ;
            }

            if (sz.Contains("Create Success"))
            {
                // 계정이 생성되면, 아이템 넣어주는 작업을 시작한다.              
                var N = JSON.Parse(sz);
                string CreateId = "";
                if (N["UserId"] != null)
                    CreateId = N["UserId"];

                StartCoroutine(CreateFirstUserDataIn(CreateId));
            }
        }//if (a_www.error == null)
        else
        {
            isCreate = false;
            Debug.Log("통신 에러");
        }
    }

    public void LoginBtn()
    {
        string a_IdStr = IDInputField.text;
        string a_PwStr = PassInputField.text;

        if (string.IsNullOrEmpty(a_IdStr.Trim()))
        {
            // 메세지 띄우기
            Login_Enter = false;
            GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경    고", "아이디 값 비어있음", () => Login_Enter = true);
            return;
        }

        if (string.IsNullOrEmpty(a_PwStr.Trim()))
        {
            // 메세지 띄우기
            Login_Enter = false;
            GameObject dlg = Instantiate(Resources.Load("ServerRequest_DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경    고", "비밀번호 값 비어있음", () => Login_Enter = true);
            return;
        }

        StartCoroutine(LoginCo(a_IdStr, a_PwStr));
    }//public void LoginBtn()

    IEnumerator LoginCo(string a_IdStr, string a_PwStr)
    {
        if (isNetwork == false)
        {
            isNetwork = true;
            SR_Obj = (GameObject)Instantiate(Resources.Load("ServerRequest_Canvas"));
            SR_Obj.GetComponent<ServerRequest>().TipStr = "로그인 중입니다......";
            WWWForm form = new WWWForm();
            form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
            form.AddField("Input_pass", a_PwStr); // 나중에 암호화 필요

            UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);
            yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

            if (a_www.error == null) //에러가 나지 않았을 때 동작
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string sz = enc.GetString(a_www.downloadHandler.data);

                GetUserData(sz, a_IdStr);
            }//if (a_www.error == null)
            else
            {
                Debug.Log("통신 에러");
                Destroy(SR_Obj);
                isNetwork = false;
            }
        }//if (isNetwork == false) 
    }//IEnumerator LoginCo(string a_IdStr, string a_PwStr)

    void GetUserData(string LoginData, string a_IdStr)
    {
        if (LoginData.Contains("Login-Success!!") == false)
        {
            // 에러 띄우기
            if (LoginData.Contains("Pass does not Match"))
            {
                // 에러 띄우기
                Debug.Log("로그인 패스워드 에러");
                Destroy(SR_Obj);
                isNetwork = false;
                GameObject dlg = Instantiate(Resources.Load("DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경    고", "비밀번호가 다릅니다");
            }
            else
            {
                Debug.Log($"PHP 에러 : {LoginData}");
                Destroy(SR_Obj);
                isNetwork = false;
                GameObject dlg = Instantiate(Resources.Load("DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경    고", "서버쪽 장애입니다");
            }
            return;
        }
        Debug.Log($"{LoginData}");
        MyInfo.m_ID = a_IdStr;

        //JSON 파싱
        if (LoginData.Contains("UserNick") == false)
        {
            // 에러 띄우기
            Debug.Log("닉네임이 없음, 서버관리자 문의");
            Destroy(SR_Obj);
            isNetwork = false;
            return;
        }

        GetLoginUserInfo(LoginData);

        // 유저 정보를 통해 유닛 정보 받는 부분
        StartCoroutine(UserUnitAttGetData(MyInfo.m_No));
    }

    // 로그인 시 유저 정보 가져오는 부분
    void GetLoginUserInfo(string LoginData)
    {
        var N = JSON.Parse(LoginData);

        if (N["UserNo"] != null)
            MyInfo.m_No = N["UserNo"];

        if (N["UserNick"] != null)
            MyInfo.m_Nick = N["UserNick"];

        if (N["UserWin"] != null)
            MyInfo.m_Win = N["UserWin"].AsInt;

        if (N["UserDefeat"] != null)
            MyInfo.m_Defeat = N["UserDefeat"].AsInt;

        if (N["UserGold"] != null)
            MyInfo.m_Gold = N["UserGold"].AsInt;

    }

    // 아이디 생성 이후 패널 넘어가는 부분
    void CreateIDComplete()
    {
        New_IDInputField.text = "";
        New_PassInputField.text = "";
        New_NickInputField.text = "";
        m_CreateAccPanelObj.SetActive(false);
        m_LoginPanelObj.SetActive(true);
    }

    IEnumerator UserUnitAttGetData(int UserNo)
    {
        if (UserNo <= 0)
        {
            Debug.Log("유저 No 파싱 안됨");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_No", UserNo.ToString(), System.Text.Encoding.UTF8);
        UnityWebRequest a_www = UnityWebRequest.Post(LoginUserAttDataUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            UserGetAttUnitData(sz); // 공격용 가지고 오는 부분
            StartCoroutine(UserUnitDefGetData(MyInfo.m_No));   // 방어 부분 아이템도 여기서 가져오자         
        }//if (a_www.error == null)
        else
        {
            Debug.Log("통신 에러");
            Destroy(SR_Obj);
            isNetwork = false;
        }
    }

    // 유저가 가지고 있는 모든 유닛 정보 가져오기
    void UserGetAttUnitData(string LoginData)
    {
        // 파싱된 결과를 바탕으로 아이템 초기화
        AttUnit a_AttUt;
        var N = JSON.Parse(LoginData);
        GlobalValue.m_AttUnitUserItem.Clear();
        // 아이템을 전체적으로 초기화한다.
        // 먼저 JSON에 저장되어 있던 정보 초기화
        if (N == null) // 아이템이 없어도 진행가능하도록
        {
            return;
        }

        if (N.ToString().Contains("ItemListdoesnotexist"))
        {
            return;
        }

        for (int i = 0; i < N["UnitList"].Count; i++)
        {
            int itemNo = N["UnitList"][i]["ItemNo"].AsInt;
            string itemName = N["UnitList"][i]["ItemName"];
            int Level = N["UnitList"][i]["Level"].AsInt;
            int isBuy = N["UnitList"][i]["isBuy"].AsInt;
            int KindOfItem = N["UnitList"][i]["KindOfItem"].AsInt - 1;
            int ItemUsable = N["UnitList"][i]["ItemUsable"].AsInt;
            int isAttack = N["UnitList"][i]["isAttack"].AsInt;

            int UnitAtt = N["UnitList"][i]["UnitAttack"].AsInt;
            int UnitDef = N["UnitList"][i]["UnitDefence"].AsInt;
            int UnitHP = N["UnitList"][i]["UnitHP"].AsInt;
            float UnitAttSpeed = N["UnitList"][i]["UnitAttSpeed"].AsFloat;
            float UnitMoveSpeed = N["UnitList"][i]["UnitMoveSpeed"].AsFloat;
            int Unitprice = N["UnitList"][i]["UnitPrice"].AsInt;
            int UnitUprice = N["UnitList"][i]["UnitUpPrice"].AsInt;
            int UnitRange = N["UnitList"][i]["UnitRange"].AsInt;
            int UnitSkill = N["UnitList"][i]["UnitSkill"].AsInt;

            a_AttUt = new AttUnit();
            a_AttUt.m_UnitNo = itemNo;
            a_AttUt.m_Name = itemName;
            a_AttUt.m_Level = Level;
            a_AttUt.m_isBuy = isBuy;
            a_AttUt.m_unitkind = (AttUnitkind)KindOfItem;
            a_AttUt.ItemUsable = ItemUsable;

            a_AttUt.m_Att = UnitAtt + Level * GlobalValue.UnitIncreValue;
            a_AttUt.m_Def = UnitDef + Level * GlobalValue.UnitIncreValue;
            a_AttUt.m_Hp = UnitHP + Level * GlobalValue.UnitIncreValue;
            a_AttUt.m_AttSpeed = UnitAttSpeed;
            a_AttUt.m_Speed = UnitMoveSpeed;
            a_AttUt.m_Price = Unitprice;
            a_AttUt.m_UpPrice = UnitUprice;
            a_AttUt.m_Range = UnitRange;
            a_AttUt.m_SkillTime = UnitSkill;

            GlobalValue.m_AttUnitUserItem.Add(a_AttUt);
        }//for (int i = 0; i < N["UnitList"].Count; i++)          
    }//void UserGetAttUnitData(string LoginData) 

    IEnumerator UserUnitDefGetData(int UserNo)
    {
        if (UserNo <= 0)
        {
            Debug.Log("유저 No 파싱 안됨");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_No", UserNo.ToString(), System.Text.Encoding.UTF8);
        UnityWebRequest a_www = UnityWebRequest.Post(LoginUserDefDataUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            UserGetDefUnitData(sz); // 방어용 가지고 오는 부분
            Destroy(SR_Obj);
            isNetwork = false;
            // 데이터 가지고 와서 마지막에 씬 넘김
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }//if (a_www.error == null)
        else
        {
            Debug.Log("통신 에러");
            Destroy(SR_Obj);
            isNetwork = false;
        }
    }

    void UserGetDefUnitData(string LoginData)
    {
        // 파싱된 결과를 바탕으로 아이템 초기화        
        DefUnit a_DefUt;
        var N = JSON.Parse(LoginData);
        GlobalValue.m_DefUnitItem.Clear();
        if (N == null) // 아이템이 없어도 진행가능하도록
        {
            return;
        }

        if (N.ToString().Contains("ItemListdoesnotexist"))
        {
            return;
        }

        // 아이템을 전체적으로 초기화한다.
        // 먼저 JSON에 저장되어 있던 정보 초기화
        for (int i = 0; i < N["UnitList"].Count; i++)
        {
            int itemNo = N["UnitList"][i]["ItemNo"].AsInt;
            string itemName = N["UnitList"][i]["ItemName"];
            int Level = N["UnitList"][i]["Level"].AsInt;
            int isBuy = N["UnitList"][i]["isBuy"].AsInt;
            int KindOfItem = N["UnitList"][i]["KindOfItem"].AsInt - 1;

            int UnitAtt = N["UnitList"][i]["UnitAttack"].AsInt;
            int UnitDef = N["UnitList"][i]["UnitDefence"].AsInt;
            int UnitHP = N["UnitList"][i]["UnitHP"].AsInt;
            float UnitAttSpeed = N["UnitList"][i]["UnitAttSpeed"].AsFloat;
            int Unitprice = N["UnitList"][i]["UnitPrice"].AsInt;
            int UnitUprice = N["UnitList"][i]["UnitUpPrice"].AsInt;
            int UnitRange = N["UnitList"][i]["UnitRange"].AsInt;

            a_DefUt = new DefUnit();
            a_DefUt.m_UnitNo = itemNo;
            a_DefUt.m_Name = itemName;
            a_DefUt.m_Level = Level;
            a_DefUt.m_isBuy = isBuy;
            a_DefUt.m_unitkind = (DefUnitkind)KindOfItem;

            a_DefUt.m_Att = UnitAtt + Level * GlobalValue.UnitIncreValue;
            a_DefUt.m_Def = UnitDef + Level * GlobalValue.UnitIncreValue;
            a_DefUt.m_Hp = UnitHP + Level * GlobalValue.UnitIncreValue;
            a_DefUt.m_AttSpeed = UnitAttSpeed;
            a_DefUt.m_Price = Unitprice;
            a_DefUt.m_UpPrice = UnitUprice;
            a_DefUt.m_Range = UnitRange;

            GlobalValue.m_DefUnitItem.Add(a_DefUt);
        }//for (int i = 0; i < N["UnitList"].Count; i++)          
    }//void UserGetUnitData(string LoginData) 

    IEnumerator CreateFirstUserDataIn(string CreateId)
    {
        // 유저가 처음 가입할 시 기본 공격타워, 기본 탱크 10개를 넣어준다.
        WWWForm form = new WWWForm();
        form.AddField("Input_ID", CreateId, System.Text.Encoding.UTF8);
        UnityWebRequest a_www = UnityWebRequest.Post(CreateDataInsertUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...        

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Insert_Success~"))
            {
                // 성공 시 로그인 안내하기
                isCreate = false;
                GameObject dlg = Instantiate(Resources.Load("DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("알    림", "아이디 생성 완료!", CreateIDComplete);
            }
            else
            {
                isCreate = false;
                GameObject dlg = Instantiate(Resources.Load("DlgBox"), DlgParent) as GameObject;
                dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경    고", "아이템 넣기 실패, 문의주세요", CreateIDComplete);
            }
        }
        else
        {
            isCreate = false;
            GameObject dlg = Instantiate(Resources.Load("DlgBox"), DlgParent) as GameObject;
            dlg.GetComponent<DialogBoxCtrl>().TitleMsgDlg("경    고", "아이템 넣기 실패, 문의주세요", CreateIDComplete);
        }
    }
}