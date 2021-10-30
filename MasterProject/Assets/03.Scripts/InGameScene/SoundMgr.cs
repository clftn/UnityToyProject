using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    // 메인 BGM
    public AudioSource mainBGM;

    private AudioSource[] SFXBuffer;
    private List<AudioSource> SFXSources = new List<AudioSource>();
    private List<float> SFXOriginVolumes = new List<float>();

    private float saveSFXValue = 0.0f;
    private void Start()
    {
        saveSFXValue = GlobalValue.SoundEffect_Value * (GlobalValue.MuteBool == true ? 0 : 1);
    }

    private void Update()
    {
        // 메인 BGM 업데이트
        mainBGM.volume = GlobalValue.Bgm_Value * (GlobalValue.MuteBool == true ? 0 : 1);

        // 사운드 이펙트 임의 조절
        SFXBuffer = FindObjectsOfType<AudioSource>();

        if (saveSFXValue != GlobalValue.SoundEffect_Value * (GlobalValue.MuteBool == true ? 0 : 1) || SFXBuffer[0] != SFXBuffer[1])
        {
            for (int i = 0; i < SFXBuffer.Length; i++)
            {
                if (SFXBuffer[i].name == "Main Camera")
                    continue;

                SFXBuffer[i].volume = GlobalValue.SoundEffect_Value * (GlobalValue.MuteBool == true ? 0 : 1);
            }

            saveSFXValue = GlobalValue.SoundEffect_Value * (GlobalValue.MuteBool == true ? 0 : 1);
        }
    }


    private void LateUpdate()
    {
        //MointorSoundSources();
    }
}