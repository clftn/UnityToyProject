using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 플레이어 베이스 클래스
/// </summary>
public class PlayerBase : MonoBehaviourPunCallbacks
{
    public float Speed = 20.0f;
    public float RotateSpeed = 100.0f;
    public float MouseRotateSpeed = 500.0f;
    public float ScaleIncreSpeed = 10.0f;
}
