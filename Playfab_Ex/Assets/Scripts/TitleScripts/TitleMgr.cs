using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

public class TitleMgr : MonoBehaviour
{
    public string g_Message = "";
    [Header("LoginPanel")]
    public GameObject m_LoginPanelObj;
    public InputField IDInputField;
    public InputField PassInputField;
    public Button m_LoginBtn = null;
    public Button m_CreateAccountOpenBtn = null;

    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanelObj;
    public InputField New_IDInputField;
    public InputField New_PassInputField;
    public InputField New_NickInputField;
    public Button m_CreateAccountBtn = null;
    public Button m_CancelBtn = null;

    private bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    private bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        if (m_CreateAccountOpenBtn != null)
            m_CreateAccountOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(CreateCancelBtn);

        if (m_CreateAccountBtn != null)
            m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);

        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginBtn()
    {
        string a_IdStr = IDInputField.text;
        string a_PwStr = PassInputField.text;

        if (a_IdStr.Trim() == "" || a_PwStr.Trim() =="") 
        {
            g_Message = "ID, PW 빈칸없이 입력해주셔야 합니다.";
            return;
        }

        if (!(a_IdStr.Length >= 3 && a_IdStr.Length < 20))
        {
            g_Message = "ID는 3글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!(a_PwStr.Length >= 6 && a_PwStr.Length < 20))
        {
            g_Message = "비밀번호는 6글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            g_Message = "이메일 형식이 맞지 않습니다.";
            return;
        }

        var option = new GetPlayerCombinedInfoRequestParams()
        {
            // 이 옵션으로 DisplayName, AvaterUrl을 가져올 수 있다.
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                //ShowAvatarUrl = true
            },

            // 이 옵션으로 플레이어의 데이터를 가져오게 한다.
            GetUserData = true,

            // 통계데이터를 가져올 수 있는 옵션
            GetPlayerStatistics = true,
        };

        var request = new LoginWithEmailAddressRequest
        {
            Email = IDInputField.text,
            Password = PassInputField.text,
            InfoRequestParameters = option
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        g_Message = $"로그인 실패 : {error.GenerateErrorReport()}";
    }

    private void OnLoginSuccess(LoginResult result)
    {
        g_Message = "로그인 성공";

        GlobalValue.g_Unique_ID = result.PlayFabId;

        if (result.InfoResultPayload != null) 
        {
            GlobalValue.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;

            // 최고점수 가져오기
            foreach (var eachStat in result.InfoResultPayload.PlayerStatistics) 
            {
                if (eachStat.StatisticName == "BestScore") 
                {
                    GlobalValue.g_BestScore = eachStat.Value;
                }
            }

            // 포인트 가져오기
            int a_GetValue = 0;
            int Idx = 0;
            foreach (var eachData in result.InfoResultPayload.UserData) 
            {
                if (eachData.Key == "UserGold")
                {
                    if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                    {
                        GlobalValue.g_Gold = a_GetValue;
                    }
                    else
                    {
                        GlobalValue.g_Gold = 0;
                    }
                }//if (eachData.Key == "UserGold")
                else if (eachData.Key.Contains("ItItem_") == true)
                {
                    Idx = 0;
                    string[] strArr = eachData.Key.Split('_');
                    if (strArr.Length >= 2)
                    {
                        if (int.TryParse(strArr[1], out Idx) == false)
                            g_Message = $"string -> int : 형변환 실패";
                    }

                    if (GlobalValue.m_ItDataList.Count <= Idx)
                        continue;

                    if (int.TryParse(eachData.Value.Value, out a_GetValue) == false)
                        g_Message = $"string -> int : 형변환 실패";

                    GlobalValue.m_ItDataList[Idx].m_Level = a_GetValue;
                } // else if (eachData.Key.Contains("ItItem_") == true)
            }// foreach (var eachData in result.InfoResultPayload.UserData) 
        }// if (result.InfoResultPayload != null) 

        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
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

    void CreateAccountBtn() // 계정 생성 요청
    {
        string a_IdStr = New_IDInputField.text;
        string a_PwStr = New_PassInputField.text;
        string a_NickStr = New_NickInputField.text;

        if (a_IdStr.Trim() == "" || a_PwStr.Trim() =="" || a_NickStr.Trim() == "") 
        {
            g_Message = "ID, PW, 별명 빈칸없이 입력해주셔야 합니다.";
            return;
        }

        if (!(a_IdStr.Length >= 3 && a_IdStr.Length < 20))
        {
            g_Message = "ID는 3글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!(a_PwStr.Length >= 6 && a_PwStr.Length < 20))
        {
            g_Message = "비밀번호는 6글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!(a_NickStr.Length >= 2 && a_NickStr.Length < 20))
        {
            g_Message = "닉네임은 2글자 이상 20글자 이하로 작성해 주세요.";
            return;
        }

        if (!CheckEmailAddress(a_IdStr)) 
        {
            g_Message = "이메일 형식이 맞지 않습니다.";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = New_IDInputField.text,
            Password = New_PassInputField.text,
            DisplayName = New_NickInputField.text,
            RequireBothUsernameAndEmail = false    
            // 아이디와 이메일을 입력해야 하는 데, 아이디를 이메일로 사용하기 위해 false로 
            // 디폴트는 true
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    void RegisterFailure(PlayFabError error)
    {
        g_Message = $"가입 실패 원인 : {error.GenerateErrorReport()}";
    }

    void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        g_Message = "가입 성공!";
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

    private void OnGUI()
    {
        if (g_Message != "") 
        {
            // white, black, Blue, Cyan, Gray, color = #ff0000
            GUI.Label(new Rect(20, 15, 1500, 100), $"<color=White><size=25>{g_Message}</size></color>");
            //GUILayout.Label($"<color=White><size=25>{g_Message}</size></color>");
        }
    }
}
