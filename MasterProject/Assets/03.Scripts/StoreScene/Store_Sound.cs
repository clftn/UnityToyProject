using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store_Sound : MonoBehaviour
{
    public AudioSource[] Music;
    //Music[0] = BGM;
    //Music[1] = SFX;
    public static Store_Sound Inst;

    private void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Music[0].volume = GlobalValue.Bgm_Value * (GlobalValue.MuteBool == true ? 0 : 1);
        Music[1].volume = GlobalValue.SoundEffect_Value * (GlobalValue.MuteBool == true ? 0 : 1);
    }
}
