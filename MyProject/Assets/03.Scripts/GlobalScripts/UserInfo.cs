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
}
