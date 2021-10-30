using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMCtrl : MonoBehaviour
{
    public AudioSource m_BGM;
    public AudioSource m_SFX;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_BGM.volume = GlobalValue.Bgm_Value * (GlobalValue.MuteBool == true ? 0 : 1);
        //m_SFX.volume = GlobalValue.SoundEffect_Value * (GlobalValue.MuteBool == true ? 0 : 1);
    }
}
