using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Item_0 = 0,
    Item_1,
    Item_2,
    Item_3,
    Item_4,
    Item_5,
    ItemCount
}

public class ItemInfo
{
    public string m_Name = "";                      // 아이템 이름
    public ItemType m_ItType = ItemType.Item_0;     // 아이템 타입
    public Vector2 m_IconSize = Vector2.one;        // 아이콘 크기
    public int m_Price = 500;                       // 아이템 기본 가격
    public int m_UpPrice = 250;                     // 업그레이드 가격
    public int m_Level = 0;                         // 아이템 레벨
    public int m_CurSkillCount = 1;                 // 사용할 수 있는 스킬 카운트
    public string m_SkillExp = "";                   // 아이템 설명
    public Sprite m_IconImg = null;                 // 아이템 이미지

    public void SetType(ItemType a_ItType)
    {
        m_ItType = a_ItType;
        if (a_ItType == ItemType.Item_0)
        {
            m_Name = "강아지";
            m_IconSize.x = 0.766f;   //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 500; //기본가격            

            m_SkillExp = "공격을 1회 막아줍니다";
            m_IconImg = Resources.Load("Images/m0011", typeof(Sprite)) as Sprite;
        }
        else if (a_ItType == ItemType.Item_1)
        {
            m_Name = "구미호";
            m_IconSize.x = 0.81f;    //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 1000; //기본가격            

            m_SkillExp = "골드 2배";
            m_IconImg = Resources.Load("Images/m0054", typeof(Sprite)) as Sprite;
        }
        else if (a_ItType == ItemType.Item_2)
        {
            m_Name = "구미호";
            m_IconSize.x = 0.946f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 2000; //기본가격
            m_UpPrice = 500; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "HP 증가";
            m_IconImg = Resources.Load("Images/m0367", typeof(Sprite)) as Sprite;
        }
        else if (a_ItType == ItemType.Item_3)
        {
            m_Name = "야옹이";
            m_IconSize.x = 0.93f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 2500; //기본가격            

            m_SkillExp = "스코어 2배";
            m_IconImg = Resources.Load("Images/m0423", typeof(Sprite)) as Sprite;
        }
        else if (a_ItType == ItemType.Item_4)
        {
            m_Name = "드래곤";
            m_IconSize.x = 0.93f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 3000; //기본가격            

            m_SkillExp = "골드 3배";
            m_IconImg = Resources.Load("Images/m0244", typeof(Sprite)) as Sprite;
        }
        else if (a_ItType == ItemType.Item_5)
        {
            m_Name = "팅커벨";
            m_IconSize.x = 0.93f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 15000; //기본가격            

            m_SkillExp = "골드 4배 스코어 4배";
            m_IconImg = Resources.Load("Images/m0172", typeof(Sprite)) as Sprite;
        }
    }
}

public class GlobalValue
{
    public static string g_Unique_ID = "";
    public static string g_NickName = "";
    public static int g_BestScore = 0;
    public static int g_Gold = 0;

    public static int g_UserRank = 0;

    // 골드는 g_Gold에서 따로 관리
    // 캐릭터 아이템 리스트, 서버에서는 보유 아이템을 배열로 레벨만 가지고 있도록 한다.
    // 레벨 0이면 아직 미보유 상태이다.
    public static List<ItemInfo> m_ItDataList = new List<ItemInfo>();

    public static void InitData() 
    {
        if (m_ItDataList.Count > 0)
            return;

        ItemInfo a_ItNd;
        for (int i = 0; i<(int)ItemType.ItemCount;i++) 
        {
            a_ItNd = new ItemInfo();
            a_ItNd.SetType((ItemType)i);
            m_ItDataList.Add(a_ItNd);
        }
    } // public static void InitData() 
}
