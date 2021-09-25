using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class UserInfo
{
    public static string g_Unique_ID = "";
    public static string g_NickName = "";
    public static string g_UserResentLoginDate = "";

    // 초기 유저 골드 및 미네랄    
    public static int UserGold = 0;
    public static int UserMineral = 0;

    public static int UserCBullet = 0;
    public static int UserMBullet = 0;

    public static List<BulletItemInfo> m_BulletItems = new List<BulletItemInfo>();
    public static List<GunItemInfo> m_GunItems = new List<GunItemInfo>();

    public static void InitGunData() 
    {
        if (m_GunItems.Count > 0)
            return;

        // DB에서 값을 가져와서 구매여부 확인하기
        string query = "";

        // DB에서 추가된 값을 토대로 아이템 숫자만큼의 배열을 만들어 먼저 넣어준다.
        bool[] isbuyArr = new bool[(int)GunType.GunCount];

        if (UserInfo.g_Unique_ID != "") 
        {
            query = $"select * from User_Weapon where uno = '{UserInfo.g_Unique_ID}'";
            MySQLConnect sqlcon = new MySQLConnect();
            DataTable dt = sqlcon.selsql(query);
            if (dt.Rows.Count > 0) 
            {
                for (int i = 0; i < (int)GunType.GunCount; i++) 
                {
                    int temp = 0;
                    int.TryParse(dt.Rows[0][1 + i].ToString(), out temp); // 컬럼 값이 0이 uno, 1이 연발총, 2가 미사일이다.
                    isbuyArr[i] = Convert.ToBoolean(temp);
                }
            }
        }

        GunItemInfo gunItemInfo;
        for (int i = 0; i < (int)GunType.GunCount; i++)
        {
            gunItemInfo = new GunItemInfo();
            gunItemInfo.SetType((GunType)i, isbuyArr[i]);
            m_GunItems.Add(gunItemInfo);
        }
    }

    public static void InitBulletData()
    {
        if (m_BulletItems.Count > 0)
            return;

        string query = "";                
        if (UserInfo.g_Unique_ID != "")
        {
            query = $"select * from User_Bullet where uno = '{UserInfo.g_Unique_ID}'";
            MySQLConnect sqlcon = new MySQLConnect();
            DataTable dt = sqlcon.selsql(query);
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0][1].ToString(), out UserCBullet);
                int.TryParse(dt.Rows[0][2].ToString(), out UserMBullet);
            }
        }

        BulletItemInfo bulletItemInfo;
        for (int i = 0; i < (int)BulletType.BulletCount; i++)
        {            
            bulletItemInfo = new BulletItemInfo();
            bulletItemInfo.SetType((BulletType)i);
            m_BulletItems.Add(bulletItemInfo);
        }
    }
}
