using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OI_Item : MonoBehaviour
{
    [HideInInspector] public UserInfo OI_Item_UserInfo;
    //public int UserN;                       //유저 고유번호
    //[HideInInspector] public string OpponentId;                      //유저ID
    public Text OpponentNick_txt;           //'닉네임' :
    public Text U_Win;                       //'승수'승
    public Text U_Defeat;                    //'패수'패

    public AudioClip m_audioclip;

    // Start is called before the first frame update
    void Start()
    {
        if (this.GetComponent<Button>() != null)
            this.GetComponent<Button>().onClick.AddListener(Opponenet_Select);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    void Opponenet_Select()
    {
        SO_Sound.Inst.Music[1].PlayOneShot(SO_Mgr.Inst.m_audioclip[2]);
        //값 전달
        SO_Mgr.Inst.OI.OI_Nick_txt.text = OI_Item_UserInfo.m_Nick + " :"; //OpponentNick_txt.text + " :";
        SO_Mgr.Inst.OI.OI_Win_txt.text = OI_Item_UserInfo.m_Win.ToString() + "승";
        SO_Mgr.Inst.OI.OI_Defeat_txt.text = OI_Item_UserInfo.m_Defeat.ToString()+ "패";
        SO_Mgr.Inst.OI.OI_UserItemCount_txt.text = OI_Item_UserInfo.m_UnitCount.ToString() + "개";
        SO_Mgr.Inst.OI.OI_UnitPower_txt.text = OI_Item_UserInfo.m_UnitPower.ToString();
        //값 전달

        SO_Mgr.Inst.Fight_SO_Info = OI_Item_UserInfo;   //선택한 상대정보

        SO_Mgr.OI_Panel.OI_OnOff = true;
        StartCoroutine(SO_Mgr.Inst.FadeIn(SO_Mgr.Inst.PanelFade, SO_Mgr.Inst.PanelFadePos));
        SO_Mgr.Inst.DeckEditBool = true;
        StartCoroutine(SO_Mgr.Inst.FadeOut(SO_Mgr.Inst.DeckFade, SO_Mgr.Inst.DeckFadePos));
    }
}
