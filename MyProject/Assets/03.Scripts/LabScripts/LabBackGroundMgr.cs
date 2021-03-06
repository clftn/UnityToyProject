using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LabBackGroundMgr : MonoBehaviour
{    
    enum LabVideoState
    {
        first,
        Second
    }
    LabVideoState labVideoState;

    public RawImage m_BackImg = null;
    public VideoPlayer mVideoPlayer = null;

    float rootStartTime = 0.0f;
    bool isSecondStart = false;
    float PlayTime = 30.0f;
    float VideoTime = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        labVideoState = LabVideoState.first;

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
        if (PlayTime <= 0.0f && labVideoState == LabVideoState.first)
        {
            labVideoState = LabVideoState.Second;
            PlayTime = VideoTime - rootStartTime;
        }
        else if (PlayTime <= 0.0f && labVideoState == LabVideoState.Second)
        {
            isSecondStart = false;
            PlayTime = VideoTime - rootStartTime;
        }
    }

    IEnumerator PrepareVideo()
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
            if (labVideoState == LabVideoState.Second)
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
