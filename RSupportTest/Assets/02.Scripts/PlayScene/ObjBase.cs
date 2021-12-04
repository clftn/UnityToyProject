using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 오브젝트들의 기본 설정 값들, 해당 값들을 상속 받아서 사용
/// </summary>
public class ObjBase : MonoBehaviourPunCallbacks
{
    public float MoveSpeed = 5.0f;
    public float MoveLength = 50.0f;
    public bool ConvertMove = false;
    public float RandomValue = 1.0f;
}
