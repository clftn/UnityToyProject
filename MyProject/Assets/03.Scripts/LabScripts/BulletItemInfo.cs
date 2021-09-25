using System.Collections;
using System.Collections.Generic;

public enum BulletType
{
    HeavyMachinGun,
    Missile,
    BulletCount
}

public class BulletItemInfo
{
    public string m_Name = "";
    public BulletType bulletType = BulletType.HeavyMachinGun;
    public int BuyGold = 0;                                     // 구매를 위해 필요한 골드
    public int BuyMineral = 0;                                  // 구매를 위해 필요한 미네랄
    public int AddBullet = 0;                                   // 구매당 채워지는 양

    public void SetType(BulletType a_bulletType, bool a_isBuy = false)
    {
        if (a_bulletType == BulletType.HeavyMachinGun)
        {
            m_Name = "연발총\n탄약";
            bulletType = a_bulletType;
            BuyGold = 20;
            BuyMineral = 10;
            AddBullet = 100;
        }
        else if (a_bulletType == BulletType.Missile)
        {
            m_Name = "미사일\n탄약";
            bulletType = a_bulletType;
            BuyGold = 30;
            BuyMineral = 20;
            AddBullet = 10;
        }
    }
}
