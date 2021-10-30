using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VirtualObjMove : MonoBehaviour
{
    //======================================================================================== ↓ 변수 선언부
    // 리얼 유닛 오브젝트
    public GameObject realObj = null;                  // 임시 오브젝트 (나중에 진짜 유닛 할당 필요)

    // 오브젝트의 관련 변수
    private int objKind = -1;        // 어떤 탱크인지 알려주는 인덱스
    private Vector3 targetObjPos = Vector3.zero;       // 생성할 오브젝트의 위치 변수    
    [HideInInspector] public bool isSequencePlacement = false;    // 연속 설치를 위한 bool

    // 메테리얼 관련 변수   
    public Material correctMtrl = null;     // 설치가 가능하면 보여줄 메테리얼
    public Material denyMtrl = null;        // 설치가 안되면 보여줄 메테리얼
    private new MeshRenderer[] renderer;   // 메테리얼을 바꿔주기 위한 매쉬랜더러    

    // 위치 조정 및 상태 변화를 위한 변수
    Ray ray = new Ray();                    // 레이
    RaycastHit hit = new RaycastHit();      // 레이 히트
    bool isOccupied = false;                // 다른 물체가 있는지 확인을 위한 bool
    int layMask = 0;
    // UnitPlacing 의 상태 변화를 위한 변수
    UnitPlacing unitPlacing = null;

    // 진짜로 뽑을 Obj의 TankCtrl 스크립트 변수
    TankCtrl tankCtrl = null;
    //======================================================================================== ↑ 변수 선언부


    //======================================================================================== ↓ 유니티 함수 부분
    //---------------------------------------------------------------------------- Start()
    private void Start()
    {        
        // 캐싱 부분
        renderer = this.GetComponentsInChildren<MeshRenderer>();       // 매쉬 랜더러 캐싱
        unitPlacing = GameObject.FindObjectOfType<UnitPlacing>();      // unitPlacing 캐싱
        tankCtrl = realObj.GetComponent<TankCtrl>();                   // tankCtrl 캐싱
        objKind = (int)tankCtrl.m_Type;                                // ObjKind 캐싱
        layMask = (1 << LayerMask.NameToLayer("AbleZone")) + (1 << LayerMask.NameToLayer("Ground"));
        // !!!!! 테스트용 캐싱 !!!!!!!
        isSequencePlacement = true;     // 연속 생산 bool true!
    }
    //---------------------------------------------------------------------------- Start()

    //---------------------------------------------------------------------------- Update()
    private void Update()
    {
        // 오브젝트가 마우스를 따라가도록 함
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layMask))
        {            
            targetObjPos = hit.point;
            targetObjPos.y = 1.55f;
            this.transform.position = targetObjPos;

            // 배치 가능 구역으로 들어간다면 메테리얼을 초록색으로
            if (hit.collider.gameObject.CompareTag("AbleZone") == true 
                && isOccupied == false)
            {
                for(int ii = 0; ii < renderer.Length; ii++)
                    renderer[ii].material = correctMtrl;


                bool isLeft = false;
                if (Input.GetMouseButtonDown(0))
                {
                    // 연속 설치 옵션이 꺼져 있는 경우
                    if (isSequencePlacement == false)
                    {
                        if (hit.collider.gameObject.name == "AbleZoneLeft")
                        {
                            isLeft = true;
                            MakeRealObj(isLeft);
                        }
                        else if (hit.collider.gameObject.name == "AbleZoneRight")
                        {
                            isLeft = false;
                            MakeRealObj(isLeft);
                        }
                        unitPlacing.placingState = UnitPlacingState.PRIMARY;        // 상태를 다시 원래대로
                    }

                    else
                    {
                        // 인스턴스 단계 유지
                        unitPlacing.placingState = UnitPlacingState.INSTANCE;

                        if (hit.collider.gameObject.name == "AbleZoneLeft")
                        {
                            isLeft = true;
                            MakeRealObj(isLeft);
                        }
                        else if (hit.collider.gameObject.name == "AbleZoneRight")
                        {
                            isLeft = false;
                            MakeRealObj(isLeft);
                        }

                        // 탱크 카운트 모니터링하고 숫자가 다 차면 더이상 생산 불가하게 바꾸는 함수
                        MonitorUnitCount();
                    }
                }
            }            

            else
            {
                for (int ii = 0; ii < renderer.Length; ii++)
                    renderer[ii].material = denyMtrl;
            }
        }
    }
    //---------------------------------------------------------------------------- Update()

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag.Contains("TANK") == true)
            isOccupied = true;

    }

    private void OnTriggerExit(Collider col)
    {
        isOccupied = false;
    }

    //======================================================================================== ↓ 사용자 정의 함수 부분
    //---------------------------------------------------------------------------- MakeRealObj()
    //--------- 클릭시 진짜 오브젝트를 생성해주고 이 오브젝트는 파괴한다.
    private void MakeRealObj(bool isLeft)
    {
        if (isSequencePlacement == false)
            Destroy(this.gameObject.GetComponent<Rigidbody>());     // 원래꺼가 밀어내는 거 방지를 위해 가상 오브젝트의 리지드 바디 삭제        
        
        Vector3 pos = this.transform.position;                  // 가상 오브젝트의 위치에 생성
        pos.y = 1.0f;                                           // 맵과 위치 맞춰줌

        UnitObjPool.Inst.GetObj(objKind, pos, isLeft);             // 진짜 오브젝트 생산 (풀에서 꺼내옴)

        if(isSequencePlacement == false)
            Destroy(this.gameObject, 0.08f);            // 약간 딜레이 주고 배치용 오브젝트 삭제        
    }
    //---------------------------------------------------------------------------- MakeRealObj()

    //---------------------------------------------------------------------------- MonitorUnitCount()
    //--------- 유닛 카운트를 체크하고 상태를 변화시키는 함수
    private void MonitorUnitCount()
    {
        if(UnitObjPool.Inst.activeTankCount[objKind] >= UnitObjPool.Inst.tankCountLimit[objKind])
        {
            Destroy(this.gameObject, 0.08f);
            unitPlacing.placingState = UnitPlacingState.PRIMARY;
        }
    }
}
