using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System.Data;

public class TitleMgr : MonoBehaviour
{
    [Header("------ Title --------")]
    public Text TitleText;

    [Header("------ Login Part --------")]
    public GameObject LoginPanel;
    public InputField LoginIDInputField;
    public InputField LoginPWInputField;
    public Button LoginOKBtn;
    public Button LoginJoinOkBtn;
    public Button LoginExitBtn;
    public Text LoginInfoText;

    [Header("------ Join Part --------")]
    public GameObject JoinPanel;
    public InputField JoinIDInputField;
    public InputField JoinPWInputField;
    public InputField JoinNickInputField;
    public Button JoinOKBtn;
    public Button JoinBackBtn;
    public Button JoinExitBtn;
    public Text JoinInfoText;

    bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크

    string PlayfabTitleId = "408F1";

    void Start()
    {
        // 로그인 관련 버튼 초기화
        if (LoginOKBtn != null)
            LoginOKBtn.onClick.AddListener(LoginOKBtnLogic);

        if (LoginJoinOkBtn != null)
            LoginJoinOkBtn.onClick.AddListener(() =>
            {
                // 화면 전환
                LoginPanel.SetActive(false);
                JoinPanel.SetActive(true);
            });

        if (LoginExitBtn != null)
            LoginExitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
            });

        // 회원 가입 관련 초기화
        if (JoinOKBtn != null)
            JoinOKBtn.onClick.AddListener(JoinLogic);

        if (JoinBackBtn != null)
            JoinBackBtn.onClick.AddListener(() =>
            {
                // 화면 전환
                LoginPanel.SetActive(true);
                JoinPanel.SetActive(false);
            });

        if (JoinExitBtn != null)
            JoinExitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
            });
    }

    void Update()
    {

    }

    #region 로그인 부분
    void LoginOKBtnLogic()
    {
        string a_LoginId = LoginIDInputField.text;
        string a_LoginPw = LoginPWInputField.text;

        if (a_LoginId.Trim() == "" || a_LoginPw.Trim() == "")
        {
            LoginInfoText.text = "ID, PW, 별명 빈칸없이 입력해주셔야 합니다.";
            return;
        }

        if (!(a_LoginId.Length >= 3 && a_LoginId.Length < 20))
        {
            LoginInfoText.text = "ID는 3글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!(a_LoginPw.Length >= 6 && a_LoginPw.Length < 20))
        {
            LoginInfoText.text = "비밀번호는 6글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!CheckEmailAddress(a_LoginId))
        {
            LoginInfoText.text = "이메일 형식이 맞지 않습니다.";
            return;
        }

        var request = new LoginWithEmailAddressRequest
        {
            Email = LoginIDInputField.text,
            Password = LoginPWInputField.text,
            TitleId = PlayfabTitleId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                // 이 옵션으로 DisplayName, AvaterUrl을 가져올 수 있다.
                GetPlayerProfile = true,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    //ShowAvatarUrl = true
                }
            }
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        // 플레이팹 유저 정보 가져오는 부분
        UserInfo.g_Unique_ID = result.PlayFabId;
        if (result.InfoResultPayload != null)
        {
            UserInfo.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            UserInfo.g_UserResentLoginDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        }// if (result.InfoResultPayload != null) 

        // DB로 값을 넘기는 부분
        string Query = $"INSERT INTO User_Info(uno, NickName, loginTime)" +
            $" VALUES('{UserInfo.g_Unique_ID}','{UserInfo.g_NickName}','{UserInfo.g_UserResentLoginDate}')" +
            $" ON DUPLICATE KEY UPDATE loginTime = '{UserInfo.g_UserResentLoginDate}'; ";

        MySQLConnect mysqlTestRef = new MySQLConnect();
        mysqlTestRef.sqlcmdSel(Query);

        // 저장되어 있는 유저 골드값 미네랄 값 가져오기
        Query = $"Select * from User_Gold where uno = '{UserInfo.g_Unique_ID}';";
        Debug.Log(Query);
        DataTable tempdt = mysqlTestRef.selsql(Query);
        if (tempdt.Rows.Count > 0) 
        {
            int.TryParse(tempdt.Rows[0][1].ToString(), out UserInfo.UserGold);
            int.TryParse(tempdt.Rows[0][2].ToString(), out UserInfo.UserMineral);
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        SceneManager.LoadScene("LobbyScene");
    }

    void OnLoginFailure(PlayFabError error)
    {
        LoginInfoText.text = "로그인에 실패했습니다.";
        Debug.Log($"원인 : {error.ErrorMessage}");
    }

    #endregion

    #region 회원 가입 부분

    void JoinLogic()
    {
        string a_JoinId = JoinIDInputField.text;
        string a_JoinPw = JoinPWInputField.text;
        string a_JoinNick = JoinNickInputField.text;

        if (a_JoinId.Trim() == "" || a_JoinPw.Trim() == "" || a_JoinNick.Trim() == "")
        {
            JoinInfoText.text = "ID, PW, 별명 빈칸없이 입력해주셔야 합니다.";
            return;
        }

        if (!(a_JoinId.Length >= 3 && a_JoinId.Length < 20))
        {
            JoinInfoText.text = "ID는 3글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!(a_JoinPw.Length >= 6 && a_JoinPw.Length < 20))
        {
            JoinInfoText.text = "비밀번호는 6글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!(a_JoinNick.Length >= 2 && a_JoinNick.Length < 20))
        {
            JoinInfoText.text = "닉네임은 2글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!CheckEmailAddress(a_JoinId))
        {
            JoinInfoText.text = "이메일 형식이 맞지 않습니다.";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = JoinIDInputField.text,
            Password = JoinPWInputField.text,
            DisplayName = JoinNickInputField.text,
            RequireBothUsernameAndEmail = false,
            TitleId = PlayfabTitleId
            // 아이디와 이메일을 입력해야 하는 데, 아이디를 이메일로 사용하기 위해 false로 디폴트는 true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        LoginInfoText.text = "회원가입 성공!";
        LoginPanel.SetActive(true);
        JoinPanel.SetActive(false);
    }


    void RegisterFailure(PlayFabError error)
    {
        JoinInfoText.text = "회원 가입에 실패했습니다.";
        Debug.Log(error.ErrorMessage);
    }

    #endregion

    void OnApplicationQuit()
    {
#if !UNITY_EDITOR
    System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    private bool CheckEmailAddress(string EmailStr)
    {
        if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        // true 로 반환할 시, 올바른 이메일 포맷임.
        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        return isValidFormat;
    }

    string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }
}