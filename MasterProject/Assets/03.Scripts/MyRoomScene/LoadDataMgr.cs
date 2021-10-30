using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class LoadDataMgr : MonoBehaviour
{
    public Button m_MyRoomBtn = null;

    string LoadUnitInfoUrl = "";
    string LoadTowerInfoUrl = "";
    string UpdateTeamNodeUrl = "";

    private void Awake()
    {
        GlobarValue.UserNumber = MyInfo.m_No;
        LoadUnitInfoUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MyRoomLoadUnitInfo.php";
        LoadTowerInfoUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/MyRoomLoadTowerInfo.php";
        UpdateTeamNodeUrl = "http://pmaker.dothome.co.kr//TeamProject/MyRoomScene/UpdateTeamNode.php";

        UnitInfoLoad();
        TowerInfoLoad();
        UpdateTeamNode();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_MyRoomBtn != null)
            m_MyRoomBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MyRoom");
            });
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void UpdateTeamNode()
    {
        StartCoroutine(UpdateTeamNodeCo());
    }

    IEnumerator UpdateTeamNodeCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_num", GlobarValue.UserNumber);
        UnityWebRequest a_www = UnityWebRequest.Post(UpdateTeamNodeUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            if (sz.Contains("ID Does Exist.") == true)
                yield break;
            AttackSetting.a_TeamIndex = int.Parse(sz);
        }
        else Debug.Log("error");
    }

    public void TowerInfoLoad()
    {
        StartCoroutine(TowerInfoLoadCo());
    }


    IEnumerator TowerInfoLoadCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("U_ID", GlobarValue.UserNumber);

        //------- 신버전     // using UnityEngine.Networking;
        UnityWebRequest a_www = UnityWebRequest.Post(LoadTowerInfoUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...
        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Load-Success") == false)
                yield break;

            var N = JSON.Parse(sz);

            //if (N == null)
            //    yield break;

            GlobarValue.g_UserTowerList.Clear();
            UserTower a_Tower;
            int a_UnitCount = N["UnitList"][0]["Count"].AsInt;

            for (int i = 0; i < a_UnitCount; i++)
            {
                a_Tower = new UserTower();
                GlobarValue.g_UserTowerList.Add(a_Tower);
            }

            for (int i = 0; i < a_UnitCount; i++)
            {
                if (N["UnitList"][0]["UnitName"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerName = N["UnitList"][0]["UnitName"][i];

                if (N["UnitList"][0]["UnitAttack"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerAttack = N["UnitList"][0]["UnitAttack"][i].AsInt;

                if (N["UnitList"][0]["UnitDefence"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerDefence = N["UnitList"][0]["UnitDefence"][i].AsInt;

                if (N["UnitList"][0]["UnitHP"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerHP = N["UnitList"][0]["UnitHP"][i].AsInt;

                if (N["UnitList"][0]["UnitAttSpeed"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerAttSpeed = N["UnitList"][0]["UnitAttSpeed"][i].AsFloat;

                if (N["UnitList"][0]["UnitPrice"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerPrice = N["UnitList"][0]["UnitPrice"][i].AsInt;

                if (N["UnitList"][0]["UnitUpPrice"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerUpPrice = N["UnitList"][0]["UnitUpPrice"][i].AsInt;

                if (N["UnitList"][0]["UnitKind"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerKind = N["UnitList"][0]["UnitKind"][i].AsInt;

                if (N["UnitList"][0]["UnitRange"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_TowerRange = N["UnitList"][0]["UnitRange"][i].AsInt;

                if (N["UnitList"][0]["UnitLevel"][i] != null)
                    GlobarValue.g_UserTowerList[i].m_UnitLevel = N["UnitList"][0]["UnitLevel"][i].AsInt;

                Debug.Log("타워(" + i +") 레벨은 : " + GlobarValue.g_UserTowerList[i].m_UnitLevel);

                GlobarValue.g_UserTowerList[i].m_TowerType = GlobarValue.g_UserTowerList[i].SetTowerType(GlobarValue.g_UserTowerList[i].m_TowerName);

                Debug.Log(GlobarValue.g_UserTowerList[i].m_TowerName + " : " + GlobarValue.g_UserTowerList[i].m_TowerType);
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }
    //---------------------------------------유저 타워 정보 가져오기

    //------------------------------------------유저 유닛 정보 가져오기(김태형)
    public void UnitInfoLoad()
    {
        StartCoroutine(UnitInfoLoadCo());
    }//public void LoginBtn() 

    IEnumerator UnitInfoLoadCo()//에러가 나지 않았을 때 동작
    {
        WWWForm form = new WWWForm();
        form.AddField("U_ID", GlobarValue.UserNumber);

        UnityWebRequest a_www = UnityWebRequest.Post(LoadUnitInfoUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...
        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            if (sz.Contains("Load-Success") == false)
                yield break;

            var N = JSON.Parse(sz);
            //if (N == null)
            //    yield break;

            GlobarValue.g_UnitListInfo.Clear();
            UnitTank a_Unit;
            int a_UnitCount = N["UnitList"][0]["Count"].AsInt;

            for (int i = 0; i < a_UnitCount; i++)
            {
                a_Unit = new UnitTank();
                GlobarValue.g_UnitListInfo.Add(a_Unit);
            }

            for (int i = 0; i < a_UnitCount; i++)
            {
                if (N["UnitList"][0]["UnitName"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitName = N["UnitList"][0]["UnitName"][i];

                if (N["UnitList"][0]["UnitAttack"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitAttack = N["UnitList"][0]["UnitAttack"][i].AsInt;

                if (N["UnitList"][0]["UnitDefence"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitDefence = N["UnitList"][0]["UnitDefence"][i].AsInt;

                if (N["UnitList"][0]["UnitHP"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitHP = N["UnitList"][0]["UnitHP"][i].AsInt;

                if (N["UnitList"][0]["UnitAttSpeed"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitAttSpd = N["UnitList"][0]["UnitAttSpeed"][i].AsFloat;

                if (N["UnitList"][0]["UnitMoveSpeed"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitMoveSpd = N["UnitList"][0]["UnitMoveSpeed"][i].AsFloat;

                if (N["UnitList"][0]["UnitPrice"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitPrice = N["UnitList"][0]["UnitPrice"][i].AsInt;

                if (N["UnitList"][0]["UnitUpPrice"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitUpPrice = N["UnitList"][0]["UnitUpPrice"][i].AsInt;

                if (N["UnitList"][0]["UnitUseable"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitUseable = N["UnitList"][0]["UnitUseable"][i].AsInt;

                if (N["UnitList"][0]["UnitKind"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitKind = N["UnitList"][0]["UnitKind"][i].AsInt;

                if (N["UnitList"][0]["UnitRange"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitRange = N["UnitList"][0]["UnitRange"][i].AsInt;

                if (N["UnitList"][0]["UnitLevel"][i] != null)
                    GlobarValue.g_UnitListInfo[i].m_UnitLevel = N["UnitList"][0]["UnitLevel"][i].AsInt;

                Debug.Log("유닛(" + i + ") 레벨은 : " + GlobarValue.g_UnitListInfo[i].m_UnitLevel);

                GlobarValue.g_UnitListInfo[i].m_UnitType = GlobarValue.g_UnitListInfo[i].SetUnitType(GlobarValue.g_UnitListInfo[i].m_UnitName);
                GlobarValue.g_UnitListInfo[i].m_UniqueID = i;
                UnitType a_TyIdex = GlobarValue.g_UnitListInfo[i].m_UnitType;
                GlobarValue.g_UnitListInfo[i].m_UnitSpr = GlobarValue.g_UnitListInfo[i].SetSprite(a_TyIdex);
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }

        Debug.Log("로드 완료");
    }
}
