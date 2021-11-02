using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemCtrl : MonoBehaviour
{
    Transform tr = null;

    // Start is called before the first frame update
    void Start()
    {
        tr = this.transform;
        tr.localScale = new Vector3(2.0f, 2.0f, 2.0f);

        if (LobbyMgr.m_isAttack == 0 && LobbyMgr.m_GetRandomItemNo == 2)
            tr.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        tr.Rotate(0, 1, 0);
    }
}