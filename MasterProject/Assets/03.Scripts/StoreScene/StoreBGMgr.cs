using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StoreBGMgr : MonoBehaviour
{
    enum MerchantVideoState
    {
        first,
        Second
    }

    public RawImage m_BackImg = null;
    public VideoPlayer mVideoPlayer = null;
    MerchantVideoState merchantVideoState = MerchantVideoState.first;
    float rootStartTime = 0.0f;
    bool isSecondStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (m_BackImg != null && mVideoPlayer != null)
        {
            // 비디오 준비 코루틴 호출
            StartCoroutine(PrepareVideo());
        }
    }

    // Update is called once per frame
    void Update()
    {

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
            if (merchantVideoState == MerchantVideoState.Second)
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
