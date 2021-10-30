using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierCtrl : MonoBehaviour
{
    float duration = 5.0f;
    int durability = 5; // 내구도 ( 공격을 막아내는 횟수 )
    
    Color[] barrierColor = { new Color(255.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 0.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 0.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 70.0f/255.0f) , 
                             new Color(212.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 35.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 73.0f/255.0f * Mathf.Pow(2.0f, 2.0f)  ,70.0f/255.0f), 
                             new Color(191.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 18.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 125.0f/255.0f * Mathf.Pow(2.0f, 2.0f) ,70.0f/255.0f), 
                             new Color(219.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 30.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 180.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 70.0f/255.0f), 
                             new Color(191.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 32.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 191.0f/255.0f * Mathf.Pow(2.0f, 2.0f), 70.0f/255.0f) };
    
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        
        if(duration < 0.0f)
        {
            Destroy(this.gameObject);
        }
        if(durability <= 0) // 내구도가 모두 닳면 배리어 파괴
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BULLET")
        {
            Destroy(other.gameObject);
            durability--;
            if (durability > 0)
                material.SetColor("_Color", barrierColor[durability - 1]);
        }
    }
}
