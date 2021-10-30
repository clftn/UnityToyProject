using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public InputField m_Count_InputField = null;
    [SerializeField] Button m_Ok_Btn = null;
    [SerializeField] Button m_Cancel_Btn = null;
    [HideInInspector] public int a_Capecity = 0;

    event Action m_Ok;
    event Action m_Count;

    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
    public void SetAction(Action a_Callback = null) => m_Ok = a_Callback;
    public void SetAction2(Action a_Callback = null) => m_Count = a_Callback;

    void Awake()
    {
        m_Ok_Btn.onClick.AddListener(Hide);
        m_Ok_Btn.onClick.AddListener(() => m_Count?.Invoke());
        m_Ok_Btn.onClick.AddListener(() => m_Ok?.Invoke());
        m_Ok_Btn.onClick.AddListener(() =>
        {
            m_Count_InputField.text = "";
        });

        m_Cancel_Btn.onClick.AddListener(Hide);
        m_Cancel_Btn.onClick.AddListener(() =>
        {
            m_Count_InputField.text = "";
        });
    }

    public void OpenPopupp()
    {
        Show();
    }

    public void SetCount(UnitAttackSetting_Node a_UaStNode)
    {
        if (m_Count_InputField.text == "") return;

        a_Capecity = int.Parse(m_Count_InputField.text);        
    }
}
