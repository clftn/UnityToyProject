using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleBackGroundMgr : MonoBehaviour
{
    enum TitleVideoState
    {
        first,
        Second
    }

    // 배경 다루는 함수
    public RawImage m_BackImg = null;
    public VideoPlayer mVideoPlayer = null;
    TitleVideoState titleVideoState;
    float PlayTime = 30.0f;
    float VideoTime = 30.0f;
    float rootStartTime = 4.7f;
    bool isSecondStart = false;
    float fadeOutTime = 2.0f;
    Color BackAlpha;
    float fadeOutSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (m_BackImg != null)
            BackAlpha = m_BackImg.color;

        titleVideoState = TitleVideoState.first;
        if (m_BackImg != null && mVideoPlayer != null)
        {
            // 비디오 준비 코루틴 호출
            StartCoroutine(PrepareVideo());
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayTime -= Time.deltaTime;
        if (PlayTime <= 0.0f && titleVideoState == TitleVideoState.first)
        {
            titleVideoState = TitleVideoState.Second;
            PlayTime = VideoTime - rootStartTime;
        }
        else if (PlayTime <= 0.0f && titleVideoState == TitleVideoState.Second)
        {
            isSecondStart = false;
            PlayTime = VideoTime - rootStartTime;
        }

        if (PlayTime <= fadeOutTime)
        {
            BackAlpha.a -= Time.deltaTime * fadeOutSpeed;
            if (BackAlpha.a <= 0.0f)
            {
                BackAlpha.a = 0.0f;
            }
            m_BackImg.color = BackAlpha;
        }
        else if (PlayTime >= (VideoTime - rootStartTime) - fadeOutTime
            && titleVideoState == TitleVideoState.Second)
        {
            BackAlpha.a += Time.deltaTime * fadeOutSpeed;
            if (BackAlpha.a >= 1.0f)
            {
                BackAlpha.a = 1.0f;
            }
            m_BackImg.color = BackAlpha;
        }
    }

    protected IEnumerator PrepareVideo()
    {
        // 비디오 준비
        mVideoPlayer.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        m_BackImg.texture = mVideoPlayer.texture;

        while (mVideoPlayer.isPlaying)
        {
            if (titleVideoState == TitleVideoState.Second)
            {
                if (isSecondStart == false)
                {
                    mVideoPlayer.time = rootStartTime;
                    isSecondStart = true;
                }
            }
            yield return null;
        }
    }
}
