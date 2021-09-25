using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraCtrl : MonoBehaviour
{
    public Transform targetTr;
    public GameObject RenderingTarget;
    public float maxDistance = 3.0f;
    public float WheelSpeed = 50.0f;
    public float updateSpeed = 10.0f;
    [Range(0, 3)]
    public float currentDistance = 3.0f;
    private string moveAxis = "Mouse ScrollWheel";
    private GameObject ahead;    
    public float hideDistance = 0.1f;

    private SkinnedMeshRenderer[] _renderer;

    // Start is called before the first frame update
    void Start()
    {
        ahead = new GameObject("ahead");
        _renderer = RenderingTarget.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetTr != null) 
        {
            ahead.transform.position = targetTr.position + targetTr.forward * (maxDistance * 0.25f);
            currentDistance += Input.GetAxisRaw(moveAxis) * WheelSpeed * Time.deltaTime * (-1.0f);
            currentDistance = Mathf.Clamp(currentDistance, 0, maxDistance);
            transform.position =
                Vector3.MoveTowards(transform.position,
                targetTr.position + Vector3.up * currentDistance
                - targetTr.forward * (currentDistance + maxDistance * 0.5f),
                updateSpeed * Time.deltaTime);

            transform.LookAt(ahead.transform);
            if (currentDistance < hideDistance)
            {
                currentDistance = hideDistance;
            }
        }//if (ahead != null)
    }
}
