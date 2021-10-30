/*
=========================================================================================================================
Title : <유닛 배치 시스템>

Ver : 1.0
Date : 2021/10/08 ~ 2021/10/18 완성

BluePrint: 

 1) UI의 유닛 배치 버튼 클릭 →
 2) 유닛 배치 상태로 넘어감 →
 3) 배치 가능한 공간에 유닛을 배치(가능하면 초록, 불가능하면 붉은색)

Content :

 - 레이 캐스트 활용한 유닛 배치
 - 유닛끼리는 콜리전을 통해 배치 가능한 공간과 그렇지 않은 공간 둠
 - 그리드로 구현할지 미지수!

Relations :

 - UnitObjPool.cs = 유닛의 오브젝트 풀
 - VirtualObjMove.cs = 배치용 오브젝트 스크립트
 - UnitLoad.cs = 유닛 정보를 불러오는 스크립트
=========================================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitPlacingState
{
    PRIMARY = 0,   // 초기 단계
    INSTANCE,      // 유닛 생성 단계
}

public class UnitPlacing : MonoBehaviour
{
    //======================================================================================== ↓ 변수 선언부
    // 배치 상태 변수
    [HideInInspector] public UnitPlacingState placingState = UnitPlacingState.PRIMARY;

    // 버튼 변수
    public GameObject unitButtonRoot = null;
    public Button[] unitButtonPrefab = new Button[5];       // 5종류의 유닛을 위한 버튼 ... 각 버튼의 인덱스와 탱크의 타입을 일치해줘야함
    private Button[] unitButton = new Button[5];            // 실제 사용될 버튼
    private Vector2[] buttonPosArray = new Vector2[5];      // 버튼의 좌표값

    // 유닛 드래그 앤 드롭 관련 변수
    private GameObject virtualUnitObj = null;          // 아직 배치되지 않은 상태의 유닛 오브젝트

    // !!!! 테스트용
    DeckInfo deckInfo = new DeckInfo();

    //======================================================================================== ↑ 변수 선언부


    //======================================================================================== ↓ 유니티 함수 부분
    //---------------------------------------------------------------------------- Start()
    void Start()
    {
        // !!!!!!! 테스트용 캐싱
        deckCaching();

        // 버튼의 정렬에 맞게 버튼을 배치하는 함수
        InitButton();

        // 버튼을 모니터링 하는 함수
        MonitorButton();
    }
    //---------------------------------------------------------------------------- Start()

    //---------------------------------------------------------------------------- Update()
    void Update()
    {
        if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
            return;

        if (placingState == UnitPlacingState.INSTANCE && (Input.GetKeyDown(KeyCode.Escape) 
            || GameMgr.Inst.m_ConfigRoot.gameObject.activeInHierarchy == true))
        {
            placingState = UnitPlacingState.PRIMARY;
            Destroy(virtualUnitObj);
            return;
        }
    }
    //---------------------------------------------------------------------------- Update()
    //======================================================================================== ↑ 유니티 함수 부분

    //======================================================================================== ↓ 사용자 정의 함수 부분


    // 테스트를 위한 캐싱
    private void deckCaching()
    {
        deckInfo = GlobalValue.My_DeckInfo;
    }

    //---------------------------------------------------------------------------- InitButton()
    //--------- 버튼을 대입해주는 함수
    private void InitButton()
    {
        //// 클라이언트의 유저 아이디와 글로벌이 일치하지 않으면 .... return
        //if (GlobalValue.My_DeckInfo.UserN != GlobalValue.g_VsUserNumber)
        //    return;

        // 프리펩 할당 안되있으면 .... return
        if (unitButtonPrefab == null)
            return;

        // 덱의 타입이 어떻게 되어있는지 받아올 변수
        int[] myDeckTypeArray = new int[5];

        // 테스트용 캐싱

        for (int ii = 0; ii < deckInfo.UserDec.Length; ii++)
        {
            myDeckTypeArray[ii] = deckInfo.UserDec[ii] - 1;
        }

        // 버튼 위치 캐싱 (위의 왼쪽부터 1번)
        buttonPosArray[0] = new Vector2(460, -160);      // 버튼 1번의 위치 캐싱
        buttonPosArray[1] = new Vector2(570, -160);      // 버튼 2번의 위치 캐싱
        buttonPosArray[2] = new Vector2(350, -280);      // 버튼 3번의 위치 캐싱
        buttonPosArray[3] = new Vector2(460, -280);      // 버튼 4번의 위치 캐싱
        buttonPosArray[4] = new Vector2(570, -280);      // 버튼 5번의 위치 캐싱

        // -------------------------- 버튼을 서버에서 정렬된 순서대로 배치하는 부분
        // 인덱스를 위한 변수 선언
        int normal = 0, speed = 1, repair = 2, solid = 3, cannon = 4;
        float scale = 1.0f;     // 로컬 스케일 값 캐싱

        // 타입과의 일치여부를 확인하고 일치하는 버튼을 순서대로 배치한다.
        for (int i = 0; i < myDeckTypeArray.Length; i++)
        {
            if (normal == myDeckTypeArray[i])
            {
                Button btn = Instantiate(unitButtonPrefab[normal]);
                btn.gameObject.transform.SetParent(unitButtonRoot.transform);
                btn.GetComponent<RectTransform>().anchoredPosition = buttonPosArray[i];
                btn.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                unitButton[normal] = btn;
            }

            else if (speed == myDeckTypeArray[i])
            {
                Button btn = Instantiate(unitButtonPrefab[speed]);
                btn.gameObject.transform.SetParent(unitButtonRoot.transform);
                btn.GetComponent<RectTransform>().anchoredPosition = buttonPosArray[i];
                btn.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                unitButton[speed] = btn;
            }

            else if (repair == myDeckTypeArray[i])
            {
                Button btn = Instantiate(unitButtonPrefab[repair]);
                btn.gameObject.transform.SetParent(unitButtonRoot.transform);
                btn.GetComponent<RectTransform>().anchoredPosition = buttonPosArray[i];
                btn.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                unitButton[repair] = btn;
            }

            else if (solid == myDeckTypeArray[i])
            {
                Button btn = Instantiate(unitButtonPrefab[solid]);
                btn.gameObject.transform.SetParent(unitButtonRoot.transform);
                btn.GetComponent<RectTransform>().anchoredPosition = buttonPosArray[i];
                btn.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                unitButton[solid] = btn;
            }

            else if (cannon == myDeckTypeArray[i])
            {
                Button btn = Instantiate(unitButtonPrefab[cannon]);
                btn.gameObject.transform.SetParent(unitButtonRoot.transform);
                btn.GetComponent<RectTransform>().anchoredPosition = buttonPosArray[i];
                btn.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                unitButton[cannon] = btn;
            }

            else
                Debug.Log("서버 정보와 아이템 타입 불일치....!");
        }
        // -------------------------- 버튼을 서버에서 정렬된 순서대로 배치하는 부분
    }
    //---------------------------------------------------------------------------- InitButton()


    //---------------------------------------------------------------------------- MonitorButton()
    //--------- 버튼을 모니터 해주는 함수
    private void MonitorButton()
    {
        // 인덱스를 위한 변수 선언
        int normal = 0, speed = 1, repair = 2, solid = 3, cannon = 4;

        // 노멀 탱크 버튼 클릭 감지
        if (unitButton[normal] != null && unitButton[normal].enabled == true)
        {
            unitButton[normal].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[normal] >= UnitObjPool.Inst.tankCountLimit[normal])
                    return;

                // 이 버튼으로 전환시킨다면
                if (placingState == UnitPlacingState.INSTANCE && virtualUnitObj != null)
                    Destroy(virtualUnitObj);

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[normal].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 스피드 탱크 버튼 클릭 감지
        if (unitButton[speed] != null && unitButton[speed].enabled == true)
        {
            unitButton[speed].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[speed] >= UnitObjPool.Inst.tankCountLimit[speed])
                    return;

                // 이 버튼으로 전환시킨다면
                if (placingState == UnitPlacingState.INSTANCE && virtualUnitObj != null)
                    Destroy(virtualUnitObj);

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[speed].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 리페어 탱크 버튼 클릭 감지
        if (unitButton[repair] != null && unitButton[repair].enabled == true)
        {
            unitButton[repair].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[repair] >= UnitObjPool.Inst.tankCountLimit[repair])
                    return;

                // 이 버튼으로 전환시킨다면
                if (placingState == UnitPlacingState.INSTANCE && virtualUnitObj != null)
                    Destroy(virtualUnitObj);

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[repair].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 솔리드 탱크 버튼 클릭 감지
        if (unitButton[solid] != null && unitButton[solid].enabled == true)
        {
            unitButton[solid].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[solid] >= UnitObjPool.Inst.tankCountLimit[solid])
                    return;

                // 이 버튼으로 전환시킨다면
                if (placingState == UnitPlacingState.INSTANCE && virtualUnitObj != null)
                    Destroy(virtualUnitObj);

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[solid].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 캐논 탱크 버튼 클릭 감지
        if (unitButton[cannon] != null && unitButton[cannon].enabled == true)
        {
            unitButton[cannon].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[cannon] >= UnitObjPool.Inst.tankCountLimit[cannon])
                    return;

                // 이 버튼으로 전환시킨다면
                if (placingState == UnitPlacingState.INSTANCE && virtualUnitObj != null)
                    Destroy(virtualUnitObj);

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[cannon].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }
    }

    //---------------------------------------------------------------------------- OffAllUnitButton()
    //--------- 유닛 배치 모드 시 모든 버튼을 꺼주는 함수
    private void OffAllUnitButton()
    {
        if (placingState == UnitPlacingState.PRIMARY)
        {
            // 모든 버튼의 enable을 true로 바꿈
            for (int i = 0; i < unitButton.Length; i++)
            {
                if (unitButton[i].enabled == false)
                    unitButton[i].enabled = true;
            }

            return;
        }

        else
        {
            // 모든 버튼의 enable을 false로 바꿈
            for (int i = 0; i < unitButton.Length; i++)
            {
                unitButton[i].enabled = false;
            }
        }
    }
    //---------------------------------------------------------------------------- OffAllUnitButton()

    //======================================================================================== ↑ 사용자 정의 함수 부분
}
