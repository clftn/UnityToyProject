using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreMgr : MonoBehaviour
{
    public Button m_ReturnBtn;
    public Text m_UserInfoText;

    public GameObject m_Item_ScrollContent;
    public GameObject m_Item_NodeObj = null;

    ItemNodeCtrl[] m_ItNodeList;

    //-- 지금 뭘 구입하려고 시도한 건지?
    ItemType m_BuyItType;    
    //    string m_SvStrJson = ""; //서버에 전달하려고 하는 JSON형식이 뭔지?
    int m_SvMyGold = 0;  //서버에 전달하려고 하는 차감된 내 골드가 얼마인지?
    //-- 지금 뭘 구입하려고 시도한 건지?


    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        if (m_ReturnBtn != null)
        {
            m_ReturnBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            });
        }

        m_UserInfoText.text = $"별명({GlobalValue.g_NickName}) : 보유 골드({GlobalValue.g_Gold})";

        GameObject a_ItemObj = null;
        ItemNodeCtrl a_ItNode = null;        
        for(int ii = 0; ii < GlobalValue.m_ItDataList.Count; ii++) 
        {
            a_ItemObj = (GameObject)Instantiate(m_Item_NodeObj);
            a_ItNode = a_ItemObj.GetComponent<ItemNodeCtrl>();
            a_ItNode.InitData(GlobalValue.m_ItDataList[ii].m_ItType);
            a_ItemObj.transform.SetParent(m_Item_ScrollContent.transform, false);            
        }

        RefreshItemList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshUserInfo() 
    {
        m_UserInfoText.text = $"별명({GlobalValue.g_NickName}) : 보유 골드({GlobalValue.g_Gold})";
    }

    void RefreshItemList() // Count가 0인 아이템만 구매 가능으로 표시한다.
    {
        if (m_Item_ScrollContent != null) 
        {
            if (m_ItNodeList == null || m_ItNodeList.Length <= 0)
                m_ItNodeList = m_Item_ScrollContent.GetComponentsInChildren<ItemNodeCtrl>();
        }

        int a_FindAv = -1;
        for (int ii = 0; ii < GlobalValue.m_ItDataList.Count;ii++) 
        {
            if (m_ItNodeList[ii].m_ItType != GlobalValue.m_ItDataList[ii].m_ItType)
                continue;

            if (GlobalValue.m_ItDataList[ii].m_Level <= 0) 
            {
                if (a_FindAv < 0)
                {
                    m_ItNodeList[ii].SetState(ItState.BeforeBuy, 
                        GlobalValue.m_ItDataList[ii].m_Price,
                        GlobalValue.m_ItDataList[ii].m_SkillExp,
                        m_ItNodeList[ii].m_ItType);
                    a_FindAv = ii;
                }
                else 
                {
                    m_ItNodeList[ii].SetState(ItState.Lock, 
                        GlobalValue.m_ItDataList[ii].m_Price,
                        GlobalValue.m_ItDataList[ii].m_SkillExp,
                        m_ItNodeList[ii].m_ItType);
                }

                continue;
            }//if (GlobalValue.m_ItDataList[ii].m_Level <= 0)

            // 활성화
            m_ItNodeList[ii].SetState(ItState.Active, 
                GlobalValue.m_ItDataList[ii].m_UpPrice,
                GlobalValue.m_ItDataList[ii].m_SkillExp,
                 m_ItNodeList[ii].m_ItType,
                GlobalValue.m_ItDataList[ii].m_Level);
        }//for (int ii = 0; ii < GlobalValue.m_ItDataList.Count;ii++) 
    }//void RefreshItemList()

    public void BuyItem(ItemType a_ItType)
    {       
        m_BuyItType = a_ItType;
        BuyBeforeJobCo();
    }

    bool isDifferent = false;
    private void BuyBeforeJobCo()   // 구매 1단계 함수
    {
        if (GlobalValue.g_Unique_ID == "")
            return;

        isDifferent = false;

        var request = new GetUserDataRequest()
        {
             PlayFabId = GlobalValue.g_Unique_ID
        };

        PlayFabClientAPI.GetUserData(request,
            (result) =>
            {
                //유저 정보 확인단계
                int a_GetValue = 0;
                int Idx = 0;
                foreach (var eachData in result.Data)
                {
                    if (eachData.Key == "UserGold")
                    {
                        a_GetValue = 0;
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == false)
                            continue;

                        if (a_GetValue != GlobalValue.g_Gold)
                            isDifferent = true;

                        GlobalValue.g_Gold = a_GetValue;
                    }//if (eachData.Key == "UserGold") 
                    else if (eachData.Key.Contains("ItItem_") == true)
                    {
                        Idx = 0;
                        string[] strArr = eachData.Key.Split('_');

                        if (strArr.Length >= 2)
                        {
                            if (int.TryParse(strArr[1], out Idx) == false)
                                continue;
                        }

                        if (GlobalValue.m_ItDataList.Count <= Idx)
                            continue;

                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == false)
                            continue;

                        if (a_GetValue != GlobalValue.m_ItDataList[Idx].m_Level)
                            isDifferent = true;

                        GlobalValue.m_ItDataList[Idx].m_Level = a_GetValue;
                    }// else if (eachData.Key.Contains("ItItem_")== true) 
                }//foreach (var eachData in result.Data)

                BuyLogic();
            },
            (error) =>
            {
                Debug.Log("데이터 불러오기 실패");
            }
        );
    }// private void BuyBeforeJobCo() 

    private void BuyLogic() // 구매 로직 부분
    {
        string a_Mess = "";
        ItState a_CrState = ItState.Lock;
        bool a_NeedDelegate = false;
        ItemInfo a_ItInfo = GlobalValue.m_ItDataList[(int)m_BuyItType];
        if (m_ItNodeList != null && (int)m_BuyItType < m_ItNodeList.Length)
        {
            a_CrState = m_ItNodeList[(int)m_BuyItType].m_ItState;
        }
        if (a_CrState == ItState.Lock) //잠긴 상태
        {
            a_Mess = "이 아이템은 Lock 상태로 구입할 수 없습니다.";
        }
        else if (a_CrState == ItState.BeforeBuy) //구매 가능 상태
        {
            if (GlobalValue.g_Gold < a_ItInfo.m_Price)
            {
                a_Mess = "보유(누적) 골드가 모자랍니다.";
            }
            else
            {
                a_Mess = "정말 구입하시겠습니까?";
                a_NeedDelegate = true; //-----> 이 조건일 때 구매
            }
        }
        else if (a_CrState == ItState.Active) //활성화(업그레이드가능) 상태
        {
            if (a_ItInfo.m_ItType == ItemType.Item_2)
            {
                int a_Cost = a_ItInfo.m_UpPrice + (a_ItInfo.m_UpPrice * (a_ItInfo.m_Level - 1));
                if (a_ItInfo.m_Level >= 2)
                {
                    a_Mess = "최고 레벨입니다.";
                }
                else if (GlobalValue.g_Gold < a_Cost)
                {
                    a_Mess = "레벨업에 필요한 보유(누적) 골드가 모자랍니다.";
                }
                else
                {
                    a_Mess = "정말 업그레이드하시겠습니까?";
                    //-----> 이 조건일 때 업그레이드
                    a_NeedDelegate = true; //-----> 이 조건일 때 구매
                }
            }
            else 
            {
                a_Mess = "구입된 상태입니다.";
            }
           
        }//else if (a_CrState == CrState.Active) 
        if (isDifferent == true)
            a_Mess += "\n(서버와 다른 정보가 있어서 수정되었습니다.)";

        GameObject a_DlgRsc = Resources.Load("DlgBox") as GameObject;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false); // 
        DlgScripts a_DlgBox = a_DlgBoxObj.GetComponent<DlgScripts>();
        if (a_DlgBox != null)
        {
            if (a_NeedDelegate == true)
                a_DlgBox.SetMessage(a_Mess, TryBuyItem);
            else
                a_DlgBox.SetMessage(a_Mess);
        }
    }

    List<int> a_SetLevel = new List<int>();
    public void TryBuyItem()
    {
        Debug.Log("결제 시도");

        bool a_BuyOk = false;
        ItemInfo a_ItInfo = null;
        a_SetLevel.Clear();

        for (int ii = 0; ii < GlobalValue.m_ItDataList.Count; ii++) 
        {
            a_ItInfo = GlobalValue.m_ItDataList[ii];
            a_SetLevel.Add(a_ItInfo.m_Level);
            if (ii != (int)m_BuyItType || a_ItInfo.m_Level >= 5)
                continue;

            int a_Cost = a_ItInfo.m_Price;
            if (a_ItInfo.m_Level > 0)
                a_Cost = a_ItInfo.m_UpPrice + (a_ItInfo.m_UpPrice * (a_ItInfo.m_Level - 1));

            if (GlobalValue.g_Gold < a_Cost)
                continue;

            //1, 여기서 계산(차감)하고 서버에 결과값을 전달한다.
            //GlobalValue.g_UserGold -= a_Cost; 골드값 차감하기 
            //a_SetLevel[ii]++; 레벨증가 
            //2, 서버로부터 응답을 받은 다음에 계산(차감)해 주는 방법도 있다.
            m_SvMyGold = GlobalValue.g_Gold;
            m_SvMyGold -= a_Cost; //골드값 차감하기 백업해 놓기
            a_SetLevel[ii]++;     //레벨증가 백업해 놓기

            a_BuyOk = true; // 서버에 아이템 구매 요청

        }//for (int ii = 0; ii < GlobalValue.m_ItDataList.Count; ii++) 

        if (a_BuyOk == true)
            BuyRequestCo();
    }//public void TryBuyItem()

    private void BuyRequestCo()
    {
        if (GlobalValue.g_Unique_ID == "")
            return;            //로그인 상태가 아니면 그냥 리턴

        if (a_SetLevel.Count <= 0)
            return;            //아이템 목록이 정상적으로 만들어지지 않았으면 리턴

        Dictionary<string, string> a_ItemList = new Dictionary<string, string>();
        string a_MkKey = "";
        a_ItemList.Clear();
        a_ItemList.Add("UserGold", m_SvMyGold.ToString()); //Dictionary 노드 추가 방법
        for (int ii = 0; ii < GlobalValue.m_ItDataList.Count; ii++)
        {
            a_MkKey = "ItItem_" + ii.ToString();
            a_ItemList.Add(a_MkKey, a_SetLevel[ii].ToString());
        }

        var request = new UpdateUserDataRequest()
        {
            Data = a_ItemList

            ////KeysToRemove 특정키 값을 삭제하는 거 까지는 할 수 있다.
            ////Public 공개 설정 : 다른 유저들이 볼 수도 있게 하는 옵션
            ////Private 비공개 설정(기본설정임) : 나만 접근할 수 있는 값의 속성으로 변경
            ////유저가 언제든 Permission 만 바꿀 수도 있다.
            //강제적으로 아래 옵션을 안주면 실행 즉시 기본값 Private로 바뀐다.
            ////Permission = UserDataPermission.Public,  
            ////Permission = UserDataPermission.Private,
            //Data = new Dictionary<string, string>()
            //        //{ { "A", "AA" }, { "B", "BB" } } };
            //        {
            //             { "ItemList", GlobalValue.g_MyPoint.ToString() }
            //        }
        };

        PlayFabClientAPI.UpdateUserData(request,
                (result) => 
                {
                    RefreshMyInfoCo();
                    // 응답 완료가 되면 전체 갱신(전체 값을 받아서 갱신하는 방법이 있고,
                    // m_SvMyGold. m_BuyCrType을 가지고 갱신하는 방법이 있다.
                    // 메뉴 상태를 갱신해 줘야 한다.
                },
                (error) => 
                {
                    Debug.Log("통신 에러");
                }
            );

    }//void BuyRequestCo()

    void RefreshMyInfoCo()
    {
        if (m_BuyItType < ItemType.Item_0 || ItemType.ItemCount <= m_BuyItType)
            return;

        GlobalValue.g_Gold = m_SvMyGold;
        GlobalValue.m_ItDataList[(int)m_BuyItType].m_Level = a_SetLevel[(int)m_BuyItType];

        RefreshItemList();
        RefreshUserInfo();
    }
}//private void BuyRequestCo()
