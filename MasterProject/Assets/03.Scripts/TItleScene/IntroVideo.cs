using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;
public class IntroVideo : MonoBehaviour
{
    enum MerchantVideoState
    {
        first,
        Second
    }
    public Image loginimg = null;
    public Text FadeText = null;
    public Image FadeImg = null;
    float Fades = 1.5f;
    float fadetime = 0;
    float texts = 1.5f;
    float texttime = 0;
    float logins = 1.2f;
    float logintime = 0;
    bool quddlf = false;

    public AudioSource StartBgm = null;
    public AudioSource LoginBgm = null;

    public RawImage m_BackImg = null;
    public VideoPlayer mVideoPlayer = null;
    MerchantVideoState merchantVideoState = MerchantVideoState.first;

    public RawImage StartImg = null;
    public VideoPlayer StartVideoPlayer = null;
    float videotime = 0.0f;
    bool isSecondStart = false;

    float m_StartVideoTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_StartVideoTime = (int)StartVideoPlayer.length - 0.0f;
        if (StartImg != null && StartVideoPlayer != null)
        {
            StartCoroutine(startVideo());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (StartImg.gameObject.activeSelf)
            {
                m_StartVideoTime = 0.0f;
            }
        }
        GameStartFade();
        if(quddlf == true)
        {
            LoginFade();
        }
        //LoginFade();
    }

    IEnumerator PrepareVideo()
    {
        mVideoPlayer.gameObject.SetActive(true);
        // 비디오 준비
        mVideoPlayer.Prepare();
        //GameObject.Find("LoginVideo");
        //GameObject.Find("Canvas").transform.FindChild("LoginVideo").gameObject.SetActive(true);

        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.0f);
        }
        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        mVideoPlayer.time = videotime;
        m_BackImg.texture = mVideoPlayer.texture;
        LoginBgm.gameObject.SetActive(true);
        LoginBgm.Play();
        yield return null;
    }

    IEnumerator startVideo()
    {
        StartVideoPlayer.Prepare();
        StartImg.gameObject.SetActive(true);

        while (!StartVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.2f);
        }

        StartImg.texture = StartVideoPlayer.texture;

        while (StartVideoPlayer.isPlaying)
        {
        
            if (merchantVideoState == MerchantVideoState.first)
            {
                m_StartVideoTime -= Time.deltaTime;
                logintime += Time.deltaTime;
                if (isSecondStart == false && m_StartVideoTime <= 0.05f)
                {
                    merchantVideoState = MerchantVideoState.Second;
                    //m_StartVideoTime = (int)StartVideoPlayer.length - 1.5f;     //비디오 재생 길이 초기화
                    StartVideoPlayer.Stop();
                    isSecondStart = true;
                    StartImg.gameObject.SetActive(false);
                    StartBgm.gameObject.SetActive(false);
                    StartCoroutine(PrepareVideo());
                    quddlf = true;
                }
            }
            yield return null;
        }
    }

    void GameStartFade()
    {
        fadetime += Time.deltaTime;
        if (Fades > 0.0f && fadetime >= 0.1f || texts > 0.0f && texttime >= 0.1f)
        {
            Fades -= 0.1f;
            FadeImg.color = new Color(0, 0, 0, Fades);
            fadetime = 0.0f;

            texts -= 0.1f;
            FadeText.color = new Color(255, 255, 255, texts);
            texttime = 0.0f;

        }
        else if (Fades <= 0.0f || texts <= 0.0f)
        {
            FadeText.gameObject.SetActive(false);
            FadeImg.gameObject.SetActive(false);
            //Fades = 0.0f;
            //texts = 0.0f;
        }
    }

    void LoginFade()
    {
        logintime += Time.deltaTime;
        if (logins > 0.0f && logintime >= 0.1f)
        {
            logins -= 0.1f;
            loginimg.color = new Color(0, 0, 0, logins);
            logintime = 0.0f;
        }
        else if (logins <= 0.0f)
        {
            loginimg.gameObject.SetActive(false);
        }

    }
}