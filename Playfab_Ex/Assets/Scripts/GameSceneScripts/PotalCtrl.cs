using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotalCtrl : MonoBehaviour
{
    public GameObject StartPos;
    PlayerCtrl PlayerRef;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRef = GameObject.Find("Player").GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D Obj)
    {
        if (Obj.gameObject.tag == "Player") 
        {
            PlayerRef.transform.position = StartPos.transform.position;
            PlayerRef.isfloor = false;
        }
    }
}
