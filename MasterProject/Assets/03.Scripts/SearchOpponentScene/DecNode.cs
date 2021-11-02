using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecNode : MonoBehaviour
{
    public Toggle DeckToggle;
    public DeckInfo m_DecInfo;
    public Text DeckNumTxt;
    bool SelectBool = false;

    //Start is called before the first frame update
    void Start()
    {

    }

    public void Init(DeckInfo DecNd)
    {
        m_DecInfo = DecNd;
    }

    // Update is called once per frame
    void Update()
    {
        if (DeckToggle != null)
            SelectBool = DeckToggle.isOn;

        if (SelectBool)
            DeckNumTxt.color = new Color32(0, 111, 255, 255);
        else
            DeckNumTxt.color = new Color32(222, 222, 222, 255);

        DeckNumTxt.text = (m_DecInfo.UserDecCount + 1).ToString() + "번 덱";      //UI상

        if (DeckToggle.isOn)
        {
            GlobalValue.My_DeckIdx = m_DecInfo.UserDecCount;
            GlobalValue.My_DeckInfo = m_DecInfo;
            SO_Mgr.Inst.Deck_Img_Update();
        }
    }
}
