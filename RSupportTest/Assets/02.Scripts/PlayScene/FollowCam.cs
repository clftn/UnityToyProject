using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    internal GameObject Target;
    Vector3 calcVec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target != null) 
        {
            calcVec.x = Target.transform.position.x;
            calcVec.y = 30.0f;
            calcVec.z = Target.transform.position.z;
            transform.position = calcVec;
        }
    }
}
