using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelect : MonoBehaviour
{
    public Toggle DeckToggle;
    public Text DeckNumTxt;
    bool SelectBool = false;
    [HideInInspector] public int m_DeckNum;

    [HideInInspector] public string Dec1Name;
    [HideInInspector] public int Dec1Level;
    [HideInInspector] public int Dec1Num;

    [HideInInspector] public string Dec2Name;
    [HideInInspector] public int Dec2Level;
    [HideInInspector] public int Dec2Num;

    [HideInInspector] public string Dec3Name;
    [HideInInspector] public int Dec3Level;
    [HideInInspector] public int Dec3Num;

    [HideInInspector] public string Dec4Name;
    [HideInInspector] public int Dec4Level;
    [HideInInspector] public int Dec4Num;

    bool On;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DeckToggle != null)
            SelectBool = DeckToggle.isOn;

        if(SelectBool)
            DeckNumTxt.color = new Color32(0, 111, 255, 255);
        else
            DeckNumTxt.color = new Color32(222, 222, 222, 255);

        DeckNumTxt.text = m_DeckNum.ToString() + "번 덱";

        if(DeckToggle.isOn && On)
        {
            Debug.Log(m_DeckNum+"번 덱 ("+ Dec1Name+","+ Dec1Level+","+ Dec1Num+")"
                                        +"("+ Dec2Name + "," + Dec2Level + "," + Dec2Num + ")"
                                        + "(" + Dec3Name + "," + Dec3Level + "," + Dec3Num + ")"
                                        + "(" + Dec4Name + "," + Dec4Level + "," + Dec4Num + ")");
            On = false;
        }
        else if(!DeckToggle.isOn && !On)
        {
            On = true;
        }
    }

}
