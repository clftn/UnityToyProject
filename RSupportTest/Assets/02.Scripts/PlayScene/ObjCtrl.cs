using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 오브젝트들은 주어진 곳에서 z축으로 50으로 랜덤하게 움직일 수 있도록 구현
/// </summary>
public class ObjCtrl : ObjBase
{    
    internal Vector3 CalcVec;

    // 포톤뷰 관련 변수들
    PhotonView pv = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 룸이 생성되어 있지 않으면 리턴한다.
        if (PhotonNetwork.CurrentRoom == null)
            return;

        // 방장인 플레이어가 오브젝트를 통제한다.
        if (pv.IsMine == true)
        {
            // 움직이는 량을 조금씩 다르게 만든다.
            RandomValue = Random.Range(1.0f, 5.0f);

            // z축으로 움직이도록 동작
            if (ConvertMove == false)
            {
                CalcVec = transform.position;
                CalcVec.z += Time.deltaTime * MoveSpeed * RandomValue;
                transform.position = CalcVec;
                if (transform.position.z >= 25)
                {
                    ConvertMove = true;
                }
            }
            else
            {
                CalcVec = transform.position;
                CalcVec.z -= Time.deltaTime * MoveSpeed * RandomValue;
                transform.position = CalcVec;
                if (transform.position.z <= -25)
                {
                    ConvertMove = false;
                }
            }
        }// if (pv.IsMine == true)
    }
}
