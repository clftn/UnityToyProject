using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    Vector3 originPos = new Vector3(0.0f, 80.0f, 0.0f); // 초기 위치값
    Vector3 originRot = new Vector3(90.0f, 0.0f, 0.0f);  // 초기 회전값
    Vector3 tarPos; // 목표 위치값
    
    // </ 마우스 이동 관련 변수
    float moveHorizontal = 0.0f;    // 마우스 좌우 회전값
    float moveVertical = 0.0f;      // 마우스 좌우 회전값
    float camSpeed = 3.0f;          // 카메라 속력

    float mapSizeH = 156.0f;
    float mapSizeV = 80.0f;
    // 카메라 이동 제한 포지션
    public float minX = -64.0f;
    public float maxX = 64.0f;
    public float minZ = -36.0f;
    public float maxZ = 36.0f;
    // 카메라 이동 제한 포지션

    // 마우스 이동 관련 변수 />

    // </ Orthographic 카메라의 경우 - 줌을 카메라 컴포넌트의 Size 옵션을 이용해 확대/축소 시킨다.
    float curSize = 40.0f;     // 현재 사이즈
    float tarSize = 40.0f;     // 목표 사이즈
    float maxSize = 40.0f;     // 최대 사이즈
    float minSize = 10.0f;     // 최소 사이즈
    // Orthographic 카메라의 경우 />

    float zoomSpeed = 3.0f; // 마우스 휠 조작에 대한 줌 인/아웃 스피드 설정값
    float damping = 10.0f;  // 카메라 Smooth 기능 댐핑값

    // Start is called before the first frame update
    void Start()
    {
        transform.position = originPos;
        transform.eulerAngles = originRot;
        tarPos = transform.position;
        curSize = maxSize;
        tarSize = maxSize;
    }

    // Update is called once per frame
    void Update()
    {
        Controller();
    }

    void Controller()
    {
        curSize = GetComponent<Camera>().orthographicSize;

        //Vector3 calcVec;
        //Vector3 originDir;
        //calcVec = originPos - transform.position;
        //originDir = calcVec.normalized;

        // curSize에 따라 제한 포지션이 줄어들어야 함.
        // 카메라 최대 사이즈는 40 최소 사이즈 10
        // 맵 가로 64, 세로 36 => 최소사이즈 일 때, 64 - (size *2) , 36 - size 가 된다.

        maxX = (mapSizeH / 2) - (curSize * 2);
        minX = -maxX;
        maxZ = (mapSizeV / 2) - curSize;
        minZ = -maxZ;

        maxX = Mathf.Clamp(maxX, 0, mapSizeH / 2);
        minX = Mathf.Clamp(minX, -(mapSizeH / 2), 0);
        maxZ = Mathf.Clamp(maxZ, 0, mapSizeV / 2);
        minZ = Mathf.Clamp(minZ, -(mapSizeV / 2), 0);

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && (tarSize < maxSize)) // 마우스 휠을 아래로 내렸을 때 (축소 [멀리보기] )
        {
            //distance = Mathf.Lerp(distance, distance + zoomSpeed, Time.deltaTime * damping);
            tarSize += zoomSpeed;
            if (tarSize > maxSize)
                tarSize = maxSize;
            //transform.position = Vector3.Lerp(transform.position, transform.position, Time.deltaTime * damping); // 카메라 이동 적용
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && (tarSize > minSize)) // 마우스 휠을 위로 올렸을 때 (확대 [가까이보기] )
        {
            //distance = Mathf.Lerp(distance, distance - zoomSpeed, Time.deltaTime * damping);
            tarSize -= zoomSpeed;
            if (tarSize < minSize)
                tarSize = minSize;
        }
        // && Mathf.Approximately(curSize,maxSize) == false
        if (Input.GetMouseButton(2))  // 마우스 휠을 클릭했을 경우 카메라 이동 가능
        {
            moveHorizontal -= Input.GetAxis("Mouse X") * camSpeed;
            moveVertical   -= Input.GetAxis("Mouse Y") * camSpeed;
            
        }

        LimitPosition(); // 카메라 이동 제한
        tarPos = new Vector3(moveHorizontal, transform.position.y, moveVertical);

        transform.position = Vector3.Lerp(transform.position, tarPos, Time.deltaTime * damping); // 카메라 이동 적용
        GetComponent<Camera>().orthographicSize = Mathf.Lerp( curSize, tarSize, Time.deltaTime * damping); // 카메라 축소/확대 적용
    }

    void LimitPosition() // 카메라 이동 제한 함수
    {
        moveHorizontal = Mathf.Clamp(moveHorizontal, minX, maxX);
        moveVertical = Mathf.Clamp(moveVertical, minZ, maxZ);
    }
}
