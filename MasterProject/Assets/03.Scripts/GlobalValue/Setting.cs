using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class Setting : MonoBehaviour
{
    [Header("Config Btn")]
    public Button SaveSetting;

    //Sound Setting
    [Header("Sound")]
    public Button SoundBtn;
    public GameObject SoundSettingObj;
    public Slider BGM_Slider;
    public Slider SoundEffect_Slider;
    public Toggle Mute_Toggle;
    public Button Sound_Reset;
    bool SoundSetActive = false;

    float CurBgm_Value = 0.0f;
    float CurSoundEffect_Value = 0.0f;
    bool CurMuteBool = false;
    //Sound Setting

    //Social Setting
    [Header("Social")]
    public Button SocialBtn;
    public GameObject SocialSettingObj;
    public InputField NickChange_IF;
    public Text NickChange_IF_Placeholder;
    public Button NickChange_Btn;
    public GameObject Dialog;
    bool SocialSetActive = false;
    GameObject SR_Obj;
    //Social Setting

    // Start is called before the first frame update
    void Start()
    {        
        SoundSetActive = false;
        if (SoundSettingObj != null)
            SoundSettingObj.SetActive(SoundSetActive);

        SocialSetActive = false;
        if (SocialSettingObj != null)
            SocialSettingObj.SetActive(SocialSetActive);

        GlobalValue.deltaTime = 0.0f;   //일시정지 효과

        if (SoundBtn != null)
            SoundBtn.onClick.AddListener(SoundBtnClick);

        if (Sound_Reset != null)
            Sound_Reset.onClick.AddListener(SoundReset);

        if (SocialBtn != null)
            SocialBtn.onClick.AddListener(SocialBtnClick);

        if (NickChange_Btn != null)
            NickChange_Btn.onClick.AddListener(NickChange);

        if (SaveSetting != null)
            SaveSetting.onClick.AddListener(SaveSet);

        //설정값 가져오기
        BGM_Slider.value = PlayerPrefs.GetFloat("BGM", 1.0f);
        SoundEffect_Slider.value = PlayerPrefs.GetFloat("SoundEffect", 1.0f);
        Mute_Toggle.isOn = PlayerPrefs.GetInt("Mute", 0) == 1 ? true : false;   //PlayerPrefs.GetInt("Mute", 0)가 1이면 true 0이면 false

        NickChange_IF.text = "";
        //설정값 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        if (SoundSettingObj != null)
            SoundSettingObj.SetActive(SoundSetActive);

        if (SocialSettingObj != null)
            SocialSettingObj.SetActive(SocialSetActive);

        SoundSet();
        SocialSet();
    }

    void ClearActive()
    {
        SoundSetActive = false;
        //GraphicSetActive = false;
        SocialSetActive = false;
    }

    #region =========Sound Setting
    void SoundBtnClick()
    {
        ClearActive();
        SoundSetActive = true;
    }
    void SoundSet()
    {
        if (!SoundSetActive)
            return;

        //저장하지않아도 사운드를 조절하여 원하는 값으로 맞추기 위해
        CurBgm_Value = BGM_Slider.value;
        GlobalValue.Bgm_Value = CurBgm_Value;

        CurSoundEffect_Value = SoundEffect_Slider.value;
        GlobalValue.SoundEffect_Value = CurSoundEffect_Value;

        CurMuteBool = Mute_Toggle.isOn;
        GlobalValue.MuteBool = CurMuteBool;
        //저장하지않아도 사운드를 조절하여 원하는 값으로 맞추기 위해
    }

    void SoundReset()
    {
        //소리설정 리셋
        BGM_Slider.value = 1.0f;
        SoundEffect_Slider.value = 1.0f;
        Mute_Toggle.isOn = false;
        //소리설정 리셋
    }
    #endregion

    //#region =========Graphic Setting
    //void GraphicBtnClick()
    //{
    //    ClearActive();
    //    GraphicSetActive = true;
    //}

    //void GraphicSet()
    //{
    //    GlobalValue.FPS60_Bool = FPS60_Toggle.isOn;
    //    FPSDisplayBool = FPSDisplay_Toggle.isOn;
    //    GlobalValue.FPSDisplay_Bool = FPSDisplayBool;
    //}

    //void GraphicReset()
    //{
    //    //ToggleGroup으로 인해 FPS60_Toggle.isOn = true이면 자동으로 FPS30_Toggle.isOn = false
    //    FPS60_Toggle.isOn = true;
    //    FPSDisplay_Toggle.isOn = false;
    //    //FPS30_Toggle.isOn = false;
    //}
    //#endregion

    #region =========Social Setting
    void SocialBtnClick()
    {
        ClearActive();
        SocialSetActive = true;
    }

    void SocialSet()
    {
        NickChange_IF_Placeholder.text = MyInfo.m_Nick;
    }

    void NickChange()
    {
        if (NickChange_IF.text == "")
            return;                

        SR_Obj = (GameObject)Instantiate(Resources.Load("ServerRequest_Canvas"));
        SR_Obj.GetComponent<ServerRequest>().TipStr = "Tip : 유닛 전투력 = (유닛별 레벨 * 유닛별 공격력)들의 총합입니다.";
        SR_Obj.SetActive(false);
        StartCoroutine(NickChagne_DB(NickChange_IF.text));
    }

    IEnumerator NickChagne_DB(string NickStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", MyInfo.m_ID, System.Text.Encoding.UTF8);       //유저의 아이디
        form.AddField("ChangeNick", NickStr, System.Text.Encoding.UTF8);                            //바꾸려고 하는 닉네임                                                                                                 
        UnityWebRequest a_www = UnityWebRequest.Post(GlobalValue.NickChange_php, form);
        yield return a_www.SendWebRequest();    //응답이 올 때까지 대기하기...

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if(sz.Contains("Duplicate nicknames."))
            {
                Debug.Log("중복된 아이디입니다.");
                Instantiate(Dialog, SocialSettingObj.transform).GetComponent<DialogBoxCtrl>().MsgDlg("중복된 아이디입니다.");
                NickChange_IF.text = "";
                Destroy(SR_Obj);
                yield break;
            }
            else if(sz.Contains("ID does not exist."))
            {
                Debug.Log("ID가 존재하지 않습니다.");
                Instantiate(Dialog, SocialSettingObj.transform).GetComponent<DialogBoxCtrl>().MsgDlg("존재하지 않는 아이디입니다.");
                NickChange_IF.text = "";
                Destroy(SR_Obj);
                yield break;
            }
            else if (!sz.Contains("ChangeNickSuccess"))
            {
                yield break;
            }
            
            MyInfo.m_Nick = NickStr;
            NickChange_IF.text = "";
            Destroy(SR_Obj);
        }
        else
        {
            Debug.Log(a_www.error);
            SR_Obj.SetActive(true);
            StartCoroutine(NickChagne_DB(NickStr));
        }
    }
    #endregion

    void SaveSet()
    {
        //소리설정 저장
        if(GlobalValue.Bgm_Value != PlayerPrefs.GetFloat("BGM", 1.0f))
        {
            PlayerPrefs.SetFloat("BGM", GlobalValue.Bgm_Value);
            GlobalValue.Bgm_Value = PlayerPrefs.GetFloat("BGM", 1.0f);
        }
            
        if (GlobalValue.SoundEffect_Value != PlayerPrefs.GetFloat("SoundEffect", 1.0f))
        {
            PlayerPrefs.SetFloat("SoundEffect", GlobalValue.SoundEffect_Value);
            GlobalValue.SoundEffect_Value = PlayerPrefs.GetFloat("SoundEffect", 1.0f);
        }
            
        if (GlobalValue.MuteBool != (PlayerPrefs.GetInt("Mute", 0) == 1 ? true : false))
        {
            PlayerPrefs.SetInt("Mute", GlobalValue.MuteBool == true ? 1 : 0);    //CurMuteBool == true 면 1 false면 0 저장
            GlobalValue.MuteBool = PlayerPrefs.GetInt("Mute", 0) == 1 ? true : false;
        }
        //소리설정 저장

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        GlobalValue.deltaTime = 0.02f;     //일시정지 해제 효과
        
        //저장하지않고 나갈 시에 이전 저장값으로 설정  //저장 시에도 발생하나 같은 값이기에 변화 x
        //Sound
        GlobalValue.Bgm_Value = PlayerPrefs.GetFloat("BGM", 1.0f);
        GlobalValue.SoundEffect_Value = PlayerPrefs.GetFloat("SoundEffect", 1.0f);
        GlobalValue.MuteBool = PlayerPrefs.GetInt("Mute", 0) == 1 ? true : false;
        //Sound

        //저장하지않고 나갈 시에 이전 저장값으로 설정
    }
}
