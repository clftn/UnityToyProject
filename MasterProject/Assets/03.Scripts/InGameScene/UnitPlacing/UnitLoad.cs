using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

/*
 ====================================================================================================
 서버에서 정보를 받아오는 것도 방법이지만
 글로벌 변수에 unit Usable(==unitCount) 가 있는 경우 그것을 받아와도 된다.

 아직 UserItem 테이블의 정보를 받아오는 스크립트가 없으므로 부득이하게 만들었다.
 만약 테이블의 정보를 받아온다면 그것을 UnitPool의 int[]인 UnitCountLimit 변수에 테이블 컬럼인 KindOfItem과 
 인덱스를 일치시켜주면 된다. (만약 KindOfItem 이 1이라면 int[0]에 할당한다.) 유닛 종류는 최대 5종이기 때문
 ====================================================================================================
 */


public class UnitLoad : MonoBehaviour
{
    // 덱의 아이템 정보를 받아올 변수
    [HideInInspector] public int[] myDeckTypeArray = new int[5];        // 덱의 유닛 타입 배열
    private int[] myDeckNumArray = new int[5];                          // 덱의 유닛 숫자 배열

    // !!!! 테스트용
    DeckInfo deckInfo = new DeckInfo();

    void Start()
    {
        //LoadAttackUnit();

        // 테스트용 
        deckInfo = GlobalValue.My_DeckInfo;
        LoadAttackUnit2();
    }


    // UserUnit 클래스에서 enum UnitKind 를 enum TankType과 일치시켜주세요....!
    // AttackUnit을 리스트로 받아서 가지고 있는게 매우 현명할 듯...
    // 공격 유닛을 글로벌에서 받아오는 함수
    private void LoadAttackUnit()
    {
        for (int i = 0; i < GlobalValue.m_AttUnitUserItem.Count; i++)
        {
            // 내가 구매했고 공격 아이템인지 확인
            if (GlobalValue.m_AttUnitUserItem[i].m_isBuy == 1)
            {
                // ↓↓↓↓↓↓ 이거 꼭 확인해야함
                // UnitKind 의 Unit_0 == NormalTank 라면....
                if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == AttUnitkind.Unit_0)
                    UnitObjPool.Inst.tankCountLimit[0] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == AttUnitkind.Unit_1)
                    UnitObjPool.Inst.tankCountLimit[1] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == AttUnitkind.Unit_2)
                    UnitObjPool.Inst.tankCountLimit[2] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == AttUnitkind.Unit_3)
                    UnitObjPool.Inst.tankCountLimit[3] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == AttUnitkind.Unit_4)
                    UnitObjPool.Inst.tankCountLimit[4] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;
            }
        }
    }


        // MyDeckInfo 라는 글로벌 변수가 새로 생겨서 추가
        // 해당 변수는 덱의 정보를 들고 있는 부분이다.
    private void LoadAttackUnit2()
    {
        //// 클라이언트의 유저 아이디와 서버에서 가져온 유저의 아이디와 다르다면 ... return
        //if (GlobalValue.g_VsUserNumber != GlobalValue.My_DeckInfo.UserN)
        //    return;

        // 테스트용 할당

        for (int ii = 0; ii < deckInfo.UserDec.Length; ii++)
        {
            myDeckTypeArray[ii] = deckInfo.UserDec[ii] - 1;
            myDeckNumArray[ii] = deckInfo.UserDec_Num[ii];
        }

        // 배열 내에서 일치하는 타입에 해당 정보를 전달한다.
        int normal = 0, speed = 1, repair = 2, solid = 3, cannon = 4;       // 인덱스를 위한 변수 선언

        for (int i = 0; i < myDeckTypeArray.Length; i++)
        {
            // 덱의 넘버가 어떤 탱크 타입과 일치하는지 확인한다. 일치하면 해당 갯수를 타입에 맞는 탱크에 삽입해준다.
            if (normal == myDeckTypeArray[i])
                UnitObjPool.Inst.tankCountLimit[normal] = myDeckNumArray[i];

            if (speed == myDeckTypeArray[i])
                UnitObjPool.Inst.tankCountLimit[speed] = myDeckNumArray[i];

            if (repair == myDeckTypeArray[i])
                UnitObjPool.Inst.tankCountLimit[repair] = myDeckNumArray[i];

            if (solid == myDeckTypeArray[i])
                UnitObjPool.Inst.tankCountLimit[solid] = myDeckNumArray[i];

            if (cannon == myDeckTypeArray[i])
                UnitObjPool.Inst.tankCountLimit[cannon] = myDeckNumArray[i];
        }
    }
}