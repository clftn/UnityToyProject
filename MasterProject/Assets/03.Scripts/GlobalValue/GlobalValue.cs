using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 기타 static 하게 관리하게 될 변수들 클래스
/// </summary>
public class GlobalValue : MonoBehaviour
{
    // 글로벌 Value DB 인스턴스 싱글턴 선언
    private static GlobalValue DBinstance;
    public static GlobalValue GetInstance()
    {
        if (!DBinstance)
        {
            DBinstance = GameObject.FindObjectOfType(typeof(GlobalValue)) as GlobalValue;
            if (!DBinstance)
            {
                GameObject container = new GameObject();
                container.name = "DBconnector";
                DBinstance = container.AddComponent(typeof(GlobalValue)) as GlobalValue;
            }
        }
        return DBinstance;
    }

    #region 업데이트 골드
    public static bool UpGoldDataLock = false;
    string UpdateGoldUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/UpdateGold.php";

    public void UpdateGold()
    {
        if(UpGoldDataLock == false)
        {
            StartCoroutine(UpdateGoldCo());
        }
    }
    IEnumerator UpdateGoldCo()
    {
        if (MyInfo.m_ID == "")
            yield break;

        UpGoldDataLock = true;
        WWWForm form = new WWWForm();
        form.AddField("Input_ID", MyInfo.m_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_Gold", MyInfo.m_Gold);
    
        UnityWebRequest a_www = UnityWebRequest.Post(UpdateGoldUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(a_www.downloadHandler.data);
            //if (a_ReStr.Contains("UpdateGoldSuccess") == true)
            //    Debug.Log("골드업데이트 완료");
        }
        else
        {
            Debug.Log(a_www.error);
        }

        UpGoldDataLock = false;
    }

    #endregion

    #region 아이템 증가에 따른 증가 수치량 변수 모음
    // 원래는 DB에 넣고 가져와서 조율하는 것이 맞다.
    public static int UnitIncreValue = 10;  // 지금은 공통적으로 증가하게 해놓고, 추후 분리

    #endregion

    #region 공격 아이템 글로벌 변수 부분

    // 공격 아이템 가져오는 부분 URL
    string GetUserAttItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/AttGetItem.php";
    string GetAttItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/AttGetDefaultItem.php";

    public static int g_VsUserNumber = 0;

    #region 상점 부분 아이템 저장 및 초기화 부분

    public static List<AttUnit> m_AttUnitUserItem = new List<AttUnit>();     // 유저 아이템 정보를 받을 변수    


    public static bool isAttDataInit = false;                           // 아이템 데이터베이스 응답 여부 확인
    public static bool GetAttDataLock = false;

    public void InitStoreAttData()
    {
        // 여기에서 DB에 정보를 가져온다.
        if (GetAttDataLock == false) 
        {
            StartCoroutine(GetStoreAttData());
        }
    }

    IEnumerator GetStoreAttData() 
    {
        if (MyInfo.m_ID == "")
            yield break;

        GetAttDataLock = true;  // 네트워크 중복 안되는 조치
        WWWForm form = new WWWForm();
        form.AddField("Input_ID", MyInfo.m_No);        
        form.AddField("Input_itemType", "1", System.Text.Encoding.UTF8);    // 공격 아이템만 가져오기
        UnityWebRequest request = UnityWebRequest.Post(GetUserAttItemUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(request.downloadHandler.data);
            if (a_ReStr.Contains("Get_Item_Success~") == true)
            {                
                // 확인 부분
                if (a_ReStr.Contains("UnitList") == false) 
                {
                    isAttDataInit = false;   // 데이터 저장 실패
                    yield break;
                }

                // 파싱된 결과를 바탕으로 아이템 초기화
                AttUnit a_UserUt;
                var N = JSON.Parse(a_ReStr);
                m_AttUnitUserItem.Clear();
                // 아이템을 전체적으로 초기화한다.
                // 먼저 JSON에 저장되어 있던 정보 초기화
                for (int i = 0; i < N["UnitList"].Count; i++)
                {                    
                    int itemNo  = N["UnitList"][i]["ItemNo"].AsInt;
                    string itemName = N["UnitList"][i]["ItemName"];
                    int Level = N["UnitList"][i]["Level"].AsInt;
                    int isBuy = N["UnitList"][i]["isBuy"].AsInt;
                    int KindOfItem = N["UnitList"][i]["KindOfItem"].AsInt - 1;
                    int ItemUsable = N["UnitList"][i]["ItemUsable"].AsInt;

                    int UnitAtt = N["UnitList"][i]["UnitAttack"].AsInt;
                    int UnitDef = N["UnitList"][i]["UnitDefence"].AsInt;
                    int UnitHP = N["UnitList"][i]["UnitHP"].AsInt;
                    float UnitAttSpeed = N["UnitList"][i]["UnitAttSpeed"].AsFloat;
                    float UnitMoveSpeed = N["UnitList"][i]["UnitMoveSpeed"].AsFloat;
                    int Unitprice = N["UnitList"][i]["UnitPrice"].AsInt;
                    int UnitUprice = N["UnitList"][i]["UnitUpPrice"].AsInt;
                    int UnitSkill = N["UnitList"][i]["UnitSkill"].AsInt;
                    int UnitRange = N["UnitList"][i]["UnitRange"].AsInt;

                    a_UserUt = new AttUnit();
                    a_UserUt.m_UnitNo = itemNo;
                    a_UserUt.m_Name = itemName;
                    a_UserUt.m_Level = Level;
                    a_UserUt.m_isBuy = isBuy;
                    a_UserUt.m_unitkind = (AttUnitkind)KindOfItem;                                        
                    a_UserUt.ItemUsable = ItemUsable;

                    a_UserUt.m_Att = UnitAtt;
                    a_UserUt.m_Def = UnitDef;
                    a_UserUt.m_Hp = UnitHP;
                    a_UserUt.m_AttSpeed = UnitAttSpeed;
                    a_UserUt.m_Speed = UnitMoveSpeed;
                    a_UserUt.m_Price = Unitprice;
                    a_UserUt.m_UpPrice = UnitUprice;
                    a_UserUt.m_SkillTime = UnitSkill;
                    a_UserUt.m_Range = UnitRange;

                    m_AttUnitUserItem.Add(a_UserUt);
                }//for (int i = 0; i < N["UnitList"].Count; i++)

                bool isInsert = false;
                AttUnit a_UserUtNew;
                // 유저가 가지고 있지 않은 아이템은 초기 아이템 정보로 초기화한다.
                // 다시 URL 통신
                WWWForm form2 = new WWWForm();
                UnityWebRequest request2 = UnityWebRequest.Post(GetAttItemUrl, form2);
                yield return request2.SendWebRequest();

                if (request2.error == null)
                {
                    string a_ReStr2 = enc.GetString(request2.downloadHandler.data);
                    var N2 = JSON.Parse(a_ReStr2);
                    for (int i = 0; i < N2["UnitList"].Count; i++)
                    {
                        // 2중 for문이긴 한데, 여기서 중복 체크를 한번 한다.
                        foreach (var tpitem in m_AttUnitUserItem)
                        {
                            if (tpitem.m_unitkind == (AttUnitkind)i) 
                            {
                                isInsert = true;
                                break;
                            }
                            
                        }

                        if (isInsert == true)
                        {
                            isInsert = false;
                            continue;
                        }
                        else
                        {
                            isInsert = false;
                            string UnitName = N2["UnitList"][i]["UnitName"];
                            int UnitAtt = N2["UnitList"][i]["UnitAttack"].AsInt;
                            int UnitDef = N2["UnitList"][i]["UnitDefence"].AsInt;
                            int UnitHP = N2["UnitList"][i]["UnitHP"].AsInt;
                            float UnitAttSpeed = N2["UnitList"][i]["UnitAttSpeed"].AsFloat;
                            float UnitMoveSpeed = N2["UnitList"][i]["UnitMoveSpeed"].AsFloat;
                            int Unitprice = N2["UnitList"][i]["UnitPrice"].AsInt;
                            int UnitUprice = N2["UnitList"][i]["UnitUpPrice"].AsInt;                            
                            int UnitRange = N2["UnitList"][i]["UnitRange"].AsInt;
                            int UnitUseable = N2["UnitList"][i]["UnitUseable"].AsInt;
                            int UnitSkill = N2["UnitList"][i]["UnitSkill"].AsInt;

                            a_UserUtNew = new AttUnit();
                            a_UserUtNew.m_UnitNo = 0; //기본값
                            a_UserUtNew.m_Name = UnitName;
                            a_UserUtNew.m_Level = 1;    // 구매 안함
                            a_UserUtNew.m_isBuy = 0;
                            a_UserUtNew.m_unitkind = (AttUnitkind)i;                                                        
                            a_UserUtNew.m_Att = UnitAtt;
                            a_UserUtNew.m_Def = UnitDef;
                            a_UserUtNew.m_Hp = UnitHP;
                            a_UserUtNew.m_AttSpeed = UnitAttSpeed;
                            a_UserUtNew.m_Speed = UnitMoveSpeed;
                            a_UserUtNew.m_Price = Unitprice;
                            a_UserUtNew.m_UpPrice = UnitUprice;
                            a_UserUtNew.m_Range = UnitRange;
                            a_UserUtNew.ItemUsable = UnitUseable;
                            a_UserUtNew.m_SkillTime = UnitSkill;

                            m_AttUnitUserItem.Add(a_UserUtNew);
                        }
                        
                    }//for (int i = 0; i < N["UnitList"].Count; i++)
                }//if (request2.error == null) 
                else 
                {
                    isAttDataInit = false; // 데이터 불러오기 실패                    
                }

                isAttDataInit = true;   // 데이터 저장 성공

                // 성공했는지 로그 찍어보기
                for (int i = 0; i< m_AttUnitUserItem.Count;i++) 
                {
                    Debug.Log($"{m_AttUnitUserItem[i].m_Name}\n");
                }
            }//if (a_ReStr.Contains("Get_Item_Success~") == true)
            else 
            {                
                // 값이 없을 경우
                AttUnit a_UserUtNew;
                // 유저가 가지고 있지 않은 아이템은 초기 아이템 정보로 초기화한다.
                // 다시 URL 통신
                WWWForm form3 = new WWWForm();
                UnityWebRequest request3 = UnityWebRequest.Post(GetAttItemUrl, form3);
                yield return request3.SendWebRequest();

                if (request3.error == null)
                {
                    string a_ReStr2 = enc.GetString(request3.downloadHandler.data);
                    var N2 = JSON.Parse(a_ReStr2);
                    for (int i = 0; i < N2["UnitList"].Count; i++)
                    {
                        string UnitName = N2["UnitList"][i]["UnitName"];                        
                        int UnitAtt = N2["UnitList"][i]["UnitAttack"].AsInt;
                        int UnitDef = N2["UnitList"][i]["UnitDefence"].AsInt;
                        int UnitHP = N2["UnitList"][i]["UnitHP"].AsInt;
                        float UnitAttSpeed = N2["UnitList"][i]["UnitAttSpeed"].AsFloat;
                        float UnitMoveSpeed = N2["UnitList"][i]["UnitMoveSpeed"].AsFloat;
                        int Unitprice = N2["UnitList"][i]["UnitPrice"].AsInt;
                        int UnitUprice = N2["UnitList"][i]["UnitUpPrice"].AsInt;
                        int UnitRange = N2["UnitList"][i]["UnitRange"].AsInt;
                        int UnitUseable = N2["UnitList"][i]["UnitUseable"].AsInt;
                        int UnitSkill = N2["UnitList"][i]["UnitSkill"].AsInt;

                        a_UserUtNew = new AttUnit();
                        a_UserUtNew.m_UnitNo = 0; //기본값
                        a_UserUtNew.m_Name = UnitName;
                        a_UserUtNew.m_Level = 1;    // 구매 안함
                        a_UserUtNew.m_isBuy = 0;
                        a_UserUtNew.m_unitkind = (AttUnitkind)i;
                        a_UserUtNew.m_Att = UnitAtt;
                        a_UserUtNew.m_Def = UnitDef;
                        a_UserUtNew.m_Hp = UnitHP;
                        a_UserUtNew.m_AttSpeed = UnitAttSpeed;
                        a_UserUtNew.m_Speed = UnitMoveSpeed;
                        a_UserUtNew.m_Price = Unitprice;
                        a_UserUtNew.m_UpPrice = UnitUprice;
                        a_UserUtNew.m_Range = UnitRange;
                        a_UserUtNew.ItemUsable = UnitUseable;
                        a_UserUtNew.m_SkillTime = UnitSkill;

                        m_AttUnitUserItem.Add(a_UserUtNew);
                    }//for (int i = 0; i < N["UnitList"].Count; i++)
                }//if (request2.error == null) 
                isAttDataInit = true;   // 데이터 저장 실패
            }

            GetAttDataLock = false; // 성공 시 네트워크 상태 해제
        }//if (request.error == null)
        else
        {
            Debug.Log(request.error);
            isAttDataInit = false;   // 데이터 저장 실패
            GetAttDataLock = false; // 실패했을 시 네트워크 상태 해제
        }
    }

    #endregion

    #endregion

    #region 방어 아이템
    string GetUserDefItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/DefGetItem.php";
    string GetDefItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/DefGetDefaultItem.php";

    public static List<DefUnit> m_DefUnitItem = new List<DefUnit>();  // 아이템 정보를 받을 변수
    public static bool isDefDataInit = false;                                   // 아이템 데이터베이스 응답 여부 확인
    public static bool GetDefDataLock = false;

    //방어 상점 유닛 저장 및 초기화
    public void InitStoreDefData()
    {
        // 여기에서 DB에 정보를 가져온다.
        if (GetDefDataLock == false)
        {
            StartCoroutine(GetStoreDefData());
        }
    }

    //방어 상점 유닛 저장 및 초기화 코루틴
    IEnumerator GetStoreDefData()
    {
        if (MyInfo.m_ID == "")
            yield break;

        GetDefDataLock = true;  // 네트워크 중복 안되는 조치

        DefUnit a_UserUtNew;

        WWWForm form = new WWWForm();
        form.AddField("Input_ID", MyInfo.m_No);        
        form.AddField("Input_ItemType", 0);       // 방어 아이템만 가져오기

        UnityWebRequest request = UnityWebRequest.Post(GetUserDefItemUrl, form);
        yield return request.SendWebRequest();

        DefUnit a_UserUt;
        
        //에러가 안났을 경우
        if (request.error == null)
        {            
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(request.downloadHandler.data);
            if (a_ReStr.Contains("Get_Item_Success~") == true)
            {
                // 확인 부분
                if (a_ReStr.Contains("UnitList") == false)
                {
                    isDefDataInit = false;   // 데이터 저장 실패
                    Debug.Log("확인1");
                    yield break;
                }

                // 파싱된 결과를 바탕으로 아이템 초기화
                var N = JSON.Parse(a_ReStr);
                m_DefUnitItem.Clear();
                // 아이템을 전체적으로 초기화한다.
                // 먼저 JSON에 저장되어 있던 정보 초기화
                for (int i = 0; i < N["UnitList"].Count; i++)
                {
                    int itemNo = N["UnitList"][i]["ItemNo"].AsInt;
                    string itemName = N["UnitList"][i]["ItemName"];
                    int Level = N["UnitList"][i]["Level"].AsInt;
                    int isBuy = N["UnitList"][i]["isBuy"].AsInt;
                    int KindOfItem = N["UnitList"][i]["KindOfItem"].AsInt - 1;                    

                    int UnitAtt = N["UnitList"][i]["UnitAttack"].AsInt;
                    int UnitDef = N["UnitList"][i]["UnitDefence"].AsInt;
                    int UnitHP = N["UnitList"][i]["UnitHP"].AsInt;
                    float UnitDefSpeed = N["UnitList"][i]["UnitAttSpeed"].AsFloat;                    
                    int Unitprice = N["UnitList"][i]["UnitPrice"].AsInt;
                    int UnitUprice = N["UnitList"][i]["UnitUpPrice"].AsInt;
                    int UnitRange = N["UnitList"][i]["UnitRange"].AsInt;

                    a_UserUt = new DefUnit();
                    a_UserUt.m_UnitNo = itemNo;
                    a_UserUt.m_Name = itemName;
                    a_UserUt.m_Level = Level;
                    a_UserUt.m_isBuy = isBuy;
                    a_UserUt.m_unitkind = (DefUnitkind)KindOfItem;                                        

                    a_UserUt.m_Att = UnitAtt;
                    a_UserUt.m_Def = UnitDef;
                    a_UserUt.m_Hp = UnitHP;
                    a_UserUt.m_AttSpeed = UnitDefSpeed;                    
                    a_UserUt.m_Price = Unitprice;
                    a_UserUt.m_UpPrice = UnitUprice;
                    a_UserUt.m_Range = UnitRange;

                    m_DefUnitItem.Add(a_UserUt);
                }//for (int i = 0; i < N["UnitList"].Count; i++)

                bool isInsert = false;
                // 유저가 가지고 있지 않은 아이템은 초기 아이템 정보로 초기화한다.
                // 다시 URL 통신
                WWWForm form2 = new WWWForm();
                UnityWebRequest request2 = UnityWebRequest.Post(GetDefItemUrl, form2);
                yield return request2.SendWebRequest();

                if (request2.error == null)
                {
                    string a_ReStr2 = enc.GetString(request2.downloadHandler.data);
                    var N2 = JSON.Parse(a_ReStr2);

                    for (int i = 0; i < N2["UnitList"].Count; i++)
                    {
                        // 2중 for문이긴 한데, 여기서 중복 체크를 한번 한다.
                        foreach (var tpitem in m_DefUnitItem)
                        {
                            if (tpitem.m_unitkind == (DefUnitkind)i)
                                isInsert = true;
                        }

                        if (isInsert == true)
                        {
                            isInsert = false;
                            continue;
                        }
                        else
                        {
                            isInsert = false;                            

                            string UnitName = N2["UnitList"][i]["UnitName"];
                            int UnitAtt = N2["UnitList"][i]["UnitAttack"].AsInt;
                            int UnitDef = N2["UnitList"][i]["UnitDefence"].AsInt;
                            int UnitHP = N2["UnitList"][i]["UnitHP"].AsInt;
                            float UnitAttSpeed = N2["UnitList"][i]["UnitAttSpeed"].AsFloat;                            
                            int Unitprice = N2["UnitList"][i]["UnitPrice"].AsInt;
                            int UnitUprice = N2["UnitList"][i]["UnitUpPrice"].AsInt;                            
                            int UnitRange = N2["UnitList"][i]["UnitRange"].AsInt;

                            a_UserUtNew = new DefUnit();
                            a_UserUtNew.m_UnitNo = 0; //기본값
                            a_UserUtNew.m_Name = UnitName;
                            a_UserUtNew.m_Level = 1;    // 구매 안함
                            a_UserUtNew.m_isBuy = 0;
                            a_UserUtNew.m_unitkind = (DefUnitkind)i;                                                        
                            a_UserUtNew.m_Att = UnitAtt;
                            a_UserUtNew.m_Def = UnitDef;
                            a_UserUtNew.m_Hp = UnitHP;
                            a_UserUtNew.m_AttSpeed = UnitAttSpeed;                            
                            a_UserUtNew.m_Price = Unitprice;
                            a_UserUtNew.m_UpPrice = UnitUprice;
                            a_UserUtNew.m_Range = UnitRange;

                            m_DefUnitItem.Add(a_UserUtNew);
                        }

                    }//for (int i = 0; i < N["UnitList"].Count; i++)
                }//if (request2.error == null) 
                else
                {
                    isDefDataInit = false; // 데이터 불러오기 실패
                }

                isDefDataInit = true;   // 데이터 저장 성공

                //// 성공했는지 로그 찍어보기
                //for (int i = 0; i< m_DefUnitItem.Count;i++) 
                //{
                //    Debug.Log($"{m_DefUnitItem[i].m_Name}\n");
                //}
            }
            else
            {
                isDefDataInit = false;   // 데이터 저장 실패
            }

            GetDefDataLock = false; // 성공 시 네트워크 상태 해제
        }//if (request.error == null)
        else
        {
            Debug.Log(request.error);
            isDefDataInit = false;   // 데이터 저장 실패
            GetDefDataLock = false; // 실패했을 시 네트워크 상태 해제
        }

        //구매한 아이템이 하나도 존재하지 않을 경우
        if (m_DefUnitItem.Count == 0)
        {
            WWWForm form2 = new WWWForm();
            form2.AddField("Input_ItemType", 0);    // 방어 아이템만 가져오기
            UnityWebRequest request2 = UnityWebRequest.Post(GetDefItemUrl, form2);
            yield return request2.SendWebRequest();

            string a_ReStr2;            
            if (request2.error == null)
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                a_ReStr2 = enc.GetString(request2.downloadHandler.data);
                var N2 = JSON.Parse(a_ReStr2);
                
                for (int i = 0; i < N2["UnitList"].Count; i++)
                {
                    string UnitName = N2["UnitList"][i]["UnitName"];
                    int UnitAtt = N2["UnitList"][i]["UnitAttack"].AsInt;
                    int UnitDef = N2["UnitList"][i]["UnitDefence"].AsInt;
                    int UnitHP = N2["UnitList"][i]["UnitHP"].AsInt;
                    float UnitAttSpeed = N2["UnitList"][i]["UnitAttSpeed"].AsFloat;
                    int Unitprice = N2["UnitList"][i]["UnitPrice"].AsInt;
                    int UnitUprice = N2["UnitList"][i]["UnitUpPrice"].AsInt;                    
                    int UnitRange = N2["UnitList"][i]["UnitRange"].AsInt;

                    a_UserUtNew = new DefUnit();
                    a_UserUtNew.m_UnitNo = 0; //기본값
                    a_UserUtNew.m_Name = UnitName;
                    a_UserUtNew.m_Level = 1;    // 구매 안함
                    a_UserUtNew.m_isBuy = 0;
                    a_UserUtNew.m_unitkind = (DefUnitkind)i;                                        
                    a_UserUtNew.m_Att = UnitAtt;
                    a_UserUtNew.m_Def = UnitDef;
                    a_UserUtNew.m_Hp = UnitHP;
                    a_UserUtNew.m_AttSpeed = UnitAttSpeed;                    
                    a_UserUtNew.m_Price = Unitprice;
                    a_UserUtNew.m_UpPrice = UnitUprice;
                    a_UserUtNew.m_Range = UnitRange;

                    m_DefUnitItem.Add(a_UserUtNew);
                }//for (int i = 0; i < N["UnitList"].Count; i++)
            }//if (request2.error == null) 
            else
            {
                isDefDataInit = false; // 데이터 불러오기 실패
            }

            isDefDataInit = true;   // 데이터 저장 성공

            GetDefDataLock = false; // 성공 시 네트워크 상태 해제
        }
    }
    #endregion

    public static float deltaTime = 0.02f; //환경설정 시 일시정지 효과를 주기위한 변수
                                           //Time.deltaTime으로 설정한 것들을 Ctrl + H로 Time. -> GlobalValue.으로 바꾸시면 편합니다.

    //SearchOpponentScene TestUrl
    public static string OI_Url = "http://pmaker.dothome.co.kr/TeamProject/SearchOpponentScene/UserInfoConnect.php";
    //public static string OI_Url = "http://kdhhost.dothome.co.kr/TeamPortfoilo_TestDB/UserInfoConnect.php";

    public static UserInfo SO_Info = null; //싸우는 상대의 정보    
    public static DeckInfo My_DeckInfo = null;     //선택한 덱 정보
    public static int My_DeckIdx = -1;      //선택된 덱이 없을 때 -1

    public static int ServerRequestCount = 0;

    #region 셋팅 글로벌 변수 부분
    //Config_SoundSetting
    public static float Bgm_Value = PlayerPrefs.GetFloat("BGM", 1.0f);                       // BGM 게임플레이도중 환경설정을 한 번도 열지않아도 적용되도록
    public static float SoundEffect_Value = PlayerPrefs.GetFloat("SoundEffect", 1.0f);      // SFX 게임플레이도중 환경설정을 한 번도 열지않아도 적용되도록
    public static bool MuteBool = PlayerPrefs.GetInt("Mute", 0) == 1 ? true : false;        // 음소거 여부 게임플레이도중 환경설정을 한 번도 열지않아도 적용되도록

    public static bool FPS60_Bool;              // 프레임 여부
    public static bool FPSDisplay_Bool;         // 프레임 표시 여부
    public static string NickChange_php = "http://pmaker.dothome.co.kr/TeamProject/SearchOpponentScene/NickChange.php";
    #endregion
}
