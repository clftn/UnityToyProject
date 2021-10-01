using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DBPhpConnectScript : MonoBehaviour
{
    public static bool QueryOK = false;
    string LoginDataInsertUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/LoginPart.php";
    string LoginInfoUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/LoginInfo.php";
    string LabWeaponDataInsertUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/LabInsertWeaponData.php";
    string LabBulletDataInsertUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/LabInsertBulletData.php";
    string GetUserGunDataUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/GetUserGunInfo.php";
    string GetUserBulletDataUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/GetUserBulletInfo.php";
    string SinglePlayGetBulletUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/SinglePlayGetBullet.php";
    string SinglePlayGetGunUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/SinglePlayGetWeapon.php";
    string SinglePlayGameEndUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/SinglePlayEnd.php";
    string MultiPlayGameEndUrl = "http://qoaudtn2008.dothome.co.kr/UnityGame/SinglePlayEnd.php";

    private static DBPhpConnectScript DBinstance;
    public static DBPhpConnectScript GetInstance()
    {
        if (!DBinstance)
        {
            DBinstance = GameObject.FindObjectOfType(typeof(DBPhpConnectScript)) as DBPhpConnectScript;
            if (!DBinstance)
            {
                GameObject container = new GameObject();
                container.name = "DBconnector";
                DBinstance = container.AddComponent(typeof(DBPhpConnectScript)) as DBPhpConnectScript;
            }
        }
        return DBinstance;
    }

    #region 로그인 DB 모음
    public void LoginDatafunc(string uid, string nickname, string ConDate)
    {
        StartCoroutine(LoginDataCo(uid, nickname, ConDate));
    }

    IEnumerator LoginDataCo(string uid, string nickname, string ConDate)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_uid", uid, System.Text.Encoding.UTF8);
        form.AddField("Input_nick", nickname, System.Text.Encoding.UTF8);
        form.AddField("Input_conDate", ConDate, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(LoginDataInsertUrl, form);
        yield return request.SendWebRequest();
    }

    public void LoginUserDataSelect(string uid)
    {
        StartCoroutine(LoginUserDataSelectCo(uid));
    }

    IEnumerator LoginUserDataSelectCo(string uid)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_uid", uid, System.Text.Encoding.UTF8);
        UnityWebRequest request = UnityWebRequest.Post(LoginInfoUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(request.downloadHandler.data);

            if (sz.Contains("No Result") == true) 
            {
                // 처음 로그인할 경우
                UserInfo.UserGold = 0;
                UserInfo.UserMineral = 0;
                QueryOK = true;
            }

            if (sz.Contains("Fail") == true)
            {
                Debug.Log("쿼리 실패");
                yield break;
            }

            if (sz.Contains("Gold") == true)
            {
                var N = JSON.Parse(sz);
                if (N["Gold"] != null)
                    UserInfo.UserGold = N["Gold"];

                if (N["Mineral"] != null)
                    UserInfo.UserMineral = N["Mineral"];

                QueryOK = true;
            }
        }
        else
        {
            Debug.Log("통신 에러");
        }
    }

    #endregion

    #region 싱글 게임 부분

    public void InitSinglePlaybulletData(int[] Curbullet, int[] Totbullet)
    {
        StartCoroutine(InitSinglePlaybulletDataCo(Curbullet, Totbullet));
    }

    IEnumerator InitSinglePlaybulletDataCo(int[] Curbullet, int[] Totbullet)
    {
        if (UserInfo.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_uid", UserInfo.g_Unique_ID, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(SinglePlayGetBulletUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(request.downloadHandler.data);

            if (sz.Contains("Fail") == true)
            {
                Debug.Log("쿼리 실패");
                yield break;
            }

            if (sz.Contains("ContinueBullet") == true && sz.Contains("MissileBullet") == true)
            {
                var N = JSON.Parse(sz);
                if (N["ContinueBullet"] != null)
                {
                    // 위험한 방법이긴 한데 다른 방법이 있을까?
                    Totbullet[0] = N["ContinueBullet"];
                    UserInfo.UserCBullet = Totbullet[0];                    
                    Curbullet[1] = 100;
                }

                if (N["MissileBullet"] != null)
                {
                    Totbullet[1] = N["MissileBullet"];
                    UserInfo.UserMBullet = Totbullet[1];                    
                    Curbullet[2] = 20;
                }
            }
        }//if (request.error == null)
        else
        {
            Debug.Log("통신 에러");
            yield break;
        }
    }//IEnumerator InitSinglePlaybulletDataCo(int[] Curbullet, int[] Totbullet)

    public void InitSinglePlayGunData(bool[] PlayerGuns)
    {
        StartCoroutine(InitSinglePlayGunDataCo(PlayerGuns));
    }

    IEnumerator InitSinglePlayGunDataCo(bool[] PlayerGuns)
    {
        if (UserInfo.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_uid", UserInfo.g_Unique_ID, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(SinglePlayGetGunUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(request.downloadHandler.data);
            if (sz.Contains("Fail") == true)
            {
                Debug.Log("쿼리 실패");
                yield break;
            }

            if (sz.Contains("machinGun") == true && sz.Contains("Missile") == true)
            {
                var N = JSON.Parse(sz);
                if (N["machinGun"] != null)
                {
                    // 위험한 방법이긴 한데 다른 방법이 있을까?
                    PlayerGuns[0] = N["machinGun"] > 0 ? true : false;
                }

                if (N["Missile"] != null)
                {
                    PlayerGuns[1] = N["Missile"] > 0 ? true : false;
                }
            }
        }//if (request.error == null)
        else
        {
            Debug.Log("통신 에러");
            yield break;
        }
    }//IEnumerator InitSinglePlayGunDataCo(int[] Curbullet, int[] Totbullet)

    public void InsertSingleGameEnd() 
    {
        StartCoroutine(InsertSingleGameEndCo());
    }

    IEnumerator InsertSingleGameEndCo() 
    {
        if (UserInfo.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_uid", UserInfo.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_ugold", UserInfo.UserGold.ToString(), System.Text.Encoding.UTF8);
        form.AddField("Input_umineral", UserInfo.UserMineral.ToString(), System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(SinglePlayGameEndUrl, form);
        yield return request.SendWebRequest();

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        if (request.error == null)
        {
            Debug.Log($"접속 성공 인서트 결과 : {enc.GetString(request.downloadHandler.data)}");
        }
        else
        {
            Debug.Log($"접속 실패");
        }

        GameMgr.isLock = false;
        GameMgr.isDBprocess = true;
    }

    #endregion

    #region 연구소 부분

    public void LabWeaponDataInsertfunc(string uid, string ItemKind, int gold, int mineral)
    {
        StartCoroutine(LabWeaponInsertfuncCo(uid, ItemKind, gold, mineral));
    }

    IEnumerator LabWeaponInsertfuncCo(string uid, string ItemKind, int gold, int mineral)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_uid", uid, System.Text.Encoding.UTF8);
        form.AddField("Input_Item", ItemKind, System.Text.Encoding.UTF8);
        form.AddField("Input_gold", gold.ToString(), System.Text.Encoding.UTF8);
        form.AddField("Input_mineral", mineral.ToString(), System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(LabWeaponDataInsertUrl, form);
        yield return request.SendWebRequest();

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        if (request.error == null)
        {
            Debug.Log($"접속 성공 인서트 결과 : {enc.GetString(request.downloadHandler.data)}");
        }
        else
        {
            Debug.Log($"접속 실패");
        }
    }

    public void LabBulletDataInsertfunc(string uid, string ItemKind, string bullet, int gold, int mineral)
    {
        StartCoroutine(LabBulletInsertfuncCo(uid, ItemKind, bullet, gold, mineral));
    }

    IEnumerator LabBulletInsertfuncCo(string uid, string ItemKind, string bullet, int gold, int mineral)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_uid", uid, System.Text.Encoding.UTF8);
        form.AddField("Input_Item", ItemKind, System.Text.Encoding.UTF8);
        form.AddField("Input_bullet", bullet, System.Text.Encoding.UTF8);
        form.AddField("Input_gold", gold.ToString(), System.Text.Encoding.UTF8);
        form.AddField("Input_mineral", mineral.ToString(), System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(LabBulletDataInsertUrl, form);
        yield return request.SendWebRequest();

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        if (request.error == null)
        {
            Debug.Log($"접속 성공 인서트 결과 : {enc.GetString(request.downloadHandler.data)}");
        }
        else
        {
            Debug.Log($"접속 실패");
        }
    }

    #endregion

    #region 멀티 게임 부분

    public void InsertMultiGameEnd()
    {
        StartCoroutine(InsertMultiGameEndCo());
    }

    IEnumerator InsertMultiGameEndCo()
    {
        if (UserInfo.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_uid", UserInfo.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_ugold", UserInfo.UserGold.ToString(), System.Text.Encoding.UTF8);
        form.AddField("Input_umineral", UserInfo.UserMineral.ToString(), System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(MultiPlayGameEndUrl, form);
        yield return request.SendWebRequest();

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        if (request.error == null)
        {
            Debug.Log($"접속 성공 인서트 결과 : {enc.GetString(request.downloadHandler.data)}");
        }
        else
        {
            Debug.Log($"접속 실패");
        }
    }

    #endregion

    #region 내정보 가져오기 부분

    public void InitGunData()
    {
        StartCoroutine(InitGunDataCo());
    }

    IEnumerator InitGunDataCo()
    {
        if (UserInfo.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_uid", UserInfo.g_Unique_ID, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(GetUserGunDataUrl, form);
        yield return request.SendWebRequest();

        // DB에서 추가된 값을 토대로 아이템 숫자만큼의 배열을 만들어 먼저 넣어준다.
        bool[] isbuyArr = new bool[(int)GunType.GunCount];
        
        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(request.downloadHandler.data);
            
            if (sz.Contains("Fail") == true)
            {
                Debug.Log("쿼리 실패");
                yield break;
            }

            if (sz.Contains("machinGun") == true && sz.Contains("Missile") == true)
            {
                var N = JSON.Parse(sz);
                if (N["machinGun"] != null)
                    isbuyArr[0] = N["machinGun"] > 0 ? true : false;

                if (N["Missile"] != null)
                    isbuyArr[1] = N["Missile"] > 0 ? true : false;
            }
        }
        else
        {
            Debug.Log("통신 에러");
            yield break;
        }

        UserInfo.m_GunItems.Clear();    // 새로 받기 위해

        GunItemInfo gunItemInfo;
        for (int i = 0; i < (int)GunType.GunCount; i++)
        {
            gunItemInfo = new GunItemInfo();
            gunItemInfo.SetType((GunType)i, isbuyArr[i]);
            UserInfo.m_GunItems.Add(gunItemInfo);
        }

        LabMgr.isGunInit = true;
    }

    public void InitBulletData()
    {
        StartCoroutine(InitBulletDataCo());
    }

    IEnumerator InitBulletDataCo()
    {
        if (UserInfo.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_uid", UserInfo.g_Unique_ID, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(GetUserBulletDataUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(request.downloadHandler.data);

            if (sz.Contains("Fail") == true)
            {
                Debug.Log("쿼리 실패");
                yield break;
            }
            if (sz.Contains("ContinueBullet") == true && sz.Contains("MissileBullet") == true)
            {
                var N = JSON.Parse(sz);
                if (N["ContinueBullet"] != null)
                    UserInfo.UserCBullet = N["ContinueBullet"];

                if (N["MissileBullet"] != null)
                    UserInfo.UserMBullet = N["MissileBullet"];
            }
        }
        else
        {
            Debug.Log("통신 에러");
            yield break;
        }

        UserInfo.m_BulletItems.Clear();

        BulletItemInfo bulletItemInfo;
        for (int i = 0; i < (int)BulletType.BulletCount; i++)
        {
            bulletItemInfo = new BulletItemInfo();
            bulletItemInfo.SetType((BulletType)i);
            UserInfo.m_BulletItems.Add(bulletItemInfo);
        }

        LabMgr.isBulletInit = true;
    }
    #endregion
}
