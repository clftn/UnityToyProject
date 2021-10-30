using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public enum UserMap
{
    MAP1,
    MAP2,
    MAP3,
}

public enum TowerType
{
    MachineGun_Tower,
    Missile_Tower,
    Emp_Tower,
    Super_MachineGun_Tower,
    None
}

public enum UnitType
{
    NormalTank,
    SpeedTank,
    Repair,
    SolidTank,
    CannonTank,
}

public class UnitTank
{
    public UnitType m_UnitType = UnitType.NormalTank;
    public int m_UniqueID = -1;
    public Sprite m_UnitSpr = null;

    public string m_UnitName = "";
    public int m_UnitAttack = 0;
    public int m_UnitDefence = 0;
    public int m_UnitHP = 0;
    public float m_UnitAttSpd = 0.0f;
    public float m_UnitMoveSpd = 0.0f;
    public int m_UnitPrice = 0;
    public int m_UnitUpPrice = 0;
    public int m_UnitUseable = 0;
    public int m_UnitKind = 0;
    public int m_UnitRange = 0;
    public int m_UnitLevel = 0;

    public UnitType SetUnitType(string _name)
    {
        UnitType a_UnitType = UnitType.NormalTank;
        if(_name.Contains("Normal") == true)
        {
            a_UnitType = UnitType.NormalTank;
        }
        else if (_name.Contains("Speed") == true)
        {
            a_UnitType = UnitType.SpeedTank;
        }
        else if (_name.Contains("Repair") == true)
        {
            a_UnitType = UnitType.Repair;
        }
        else if (_name.Contains("Solid") == true)
        {
            a_UnitType = UnitType.SolidTank;
        }
        else if (_name.Contains("Cannon") == true)
        {
            a_UnitType = UnitType.CannonTank;
        }

        return a_UnitType;
    }

    public Sprite SetSprite(UnitType _type)
    {
        Sprite a_Spr = null;
        if (_type == UnitType.NormalTank)
            a_Spr = Resources.Load<Sprite>("StoreImg/NomalTankImg");
        else if (_type == UnitType.SpeedTank)
            a_Spr = Resources.Load<Sprite>("StoreImg/SpeedTankImg");
        else if (_type == UnitType.Repair)
            a_Spr = Resources.Load<Sprite>("StoreImg/RepairTankImg");
        else if (_type == UnitType.SolidTank)
            a_Spr = Resources.Load<Sprite>("StoreImg/ShieldTankImg");
        else if (_type == UnitType.CannonTank)
            a_Spr = Resources.Load<Sprite>("StoreImg/CannonTankImg");

        return a_Spr;
    }
}

public class UserTower
{
    public string m_TowerName = "";
    public int m_TowerAttack = 0;
    public int m_TowerDefence = 0;
    public int m_TowerHP = 0;
    public float m_TowerAttSpeed = 0;
    public int m_TowerPrice = 0;
    public int m_TowerUpPrice = 0;
    public int m_TowerKind = 0;
    public int m_TowerRange = 0;
    public TowerType m_TowerType = TowerType.None;
    public int m_UnitLevel = 0;

    public TowerType SetTowerType(string _name)
    {
        TowerType a_Type = TowerType.None;

        if (_name == "MachineGunTower" || _name.Contains("1"))
        {
            a_Type = TowerType.MachineGun_Tower;
        }
        else if (_name == "MissileTower" || _name.Contains("2"))
        {
            a_Type = TowerType.Missile_Tower;
        }
        else if (_name == "EmpTower" || _name.Contains("3"))
        {
            a_Type = TowerType.Emp_Tower;
        }
        else if (_name == "SuperMachineGunTower" || _name.Contains("4"))
        {
            a_Type = TowerType.Super_MachineGun_Tower;
        }

        return a_Type;
    }
}

public class MapSetting
{
    public bool m_SetMapCheck = false;
    public bool[] m_SpawnPoint;
    public TowerType[] m_TowerType;
    public UserMap m_UserMap = UserMap.MAP1;
    public int m_MapPower = 0;
    public string m_SaveSpawnPointList = "";
    public string m_SaveTowerList = "";
    public int m_MpaSetTower = 0;

    public void SetSpawnPoint(UserMap _map)
    {
        m_UserMap = _map;
        if (_map == UserMap.MAP1)
        {
            m_SetMapCheck = false;

            m_TowerType = new TowerType[50];
            for(int i = 0; i < m_TowerType.Length; i++)
            {
                m_TowerType[i] = TowerType.None;
            }

            m_SpawnPoint = new bool[50];
            for (int i = 0; i < m_SpawnPoint.Length; i++)
            {
                m_SpawnPoint[i] = false;
            }
        }

        else if (_map == UserMap.MAP2)
        {
            m_SetMapCheck = false;

            m_TowerType = new TowerType[68];
            for (int i = 0; i < m_TowerType.Length; i++)
            {
                m_TowerType[i] = TowerType.None;
            }

            m_SpawnPoint = new bool[68];
            for (int i = 0; i < m_SpawnPoint.Length; i++)
            {
                m_SpawnPoint[i] = false;
            }
        }
    }

    public void SaveMapInfo()
    {
        m_MapPower = 0;
        for (int i = 0; i < m_SpawnPoint.Length; i++)
        {
            if (m_SpawnPoint[i] == true)
            {
                for(int ii = 0; ii < GlobarValue.g_UserTowerList.Count; ii++)
                {
                    if(GlobarValue.g_UserTowerList[ii].m_TowerType == m_TowerType[i])
                    {
                        m_MapPower += GlobarValue.g_UserTowerList[ii].m_TowerAttack;
                        break;
                    }
                }
            }
        }
        m_MapPower = m_MapPower * 10;

        m_MapPower += 200; //기본포탑 데미지 x 10;

        m_SaveSpawnPointList = "";
        m_SaveTowerList = "";
        for (int i = 0; i < m_SpawnPoint.Length; i++)
        {
            if (m_SpawnPoint[i] == true)
                m_SaveSpawnPointList += "0 ";
            else
                m_SaveSpawnPointList += "1 ";

            m_SaveTowerList += (int)m_TowerType[i] + " ";
        }
    }
}

public class GlobarValue : MonoBehaviour
{
    public static int UserNumber = 1;
    //-------------------------맵정보
    public static UserMap g_UserMap = UserMap.MAP1;
    public static List<MapSetting> g_MapList = new List<MapSetting>();
    //-------------------------맵정보

    //-------------------------유닛정보(임시용)
    //public static List<UnitTank> g_UnitList = new List<UnitTank>();
    public static List<UnitTank> g_UnitListInfo = new List<UnitTank>();
    public static List<UserTower> g_UserTowerList = new List<UserTower>();

    public static List<UserTower> g_VsUserTowerList = new List<UserTower>();

    //-------------------------유닛정보(임시용)

    public static void MakeMapSave()
    {
        for (int i = 0; i < 3; i++)
        {
            MapSetting a_MapSetting = new MapSetting();
            a_MapSetting.SetSpawnPoint((UserMap)i);
            g_MapList.Add(a_MapSetting);
        }
    }
}