using System.Collections;
using System.Collections.Generic;

public enum GunType
{
    HeavyMachinGun,
    Missile,
    GunCount
}

public class GunItemInfo
{
    public string m_Name = "";
    public GunType gunType = GunType.HeavyMachinGun;
    public int BuyGold = 0;                             // 구매를 위해 필요한 골드
    public int BuyMineral = 0;                          // 구매를 위해 필요한 미네랄
    public bool isBuy = false;                          // 구매 여부

    public void SetType(GunType a_gunType, bool a_isBuy = false)
    {
        if (a_gunType == GunType.HeavyMachinGun)
        {
            m_Name = "연발총";
            gunType = a_gunType;
            BuyGold = 100;
            BuyMineral = 50;
            isBuy = a_isBuy;
        }
        else if (a_gunType == GunType.Missile)
        {
            m_Name = "미사일";
            gunType = a_gunType;
            BuyGold = 500;
            BuyMineral = 250;
            isBuy = a_isBuy;
        }
    }
}
