using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject Item;

    int ItemGenRate = 0;
    int randnum = 0;

    // Start is called before the first frame update
    void Start()
    {
        ItemGenRate = 50;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ItemGen(Vector3 tr)
    {
        randnum = Random.Range(0, 100) + 1;
        if (randnum <= ItemGenRate)
        {
            tr.y = 1.65f; // y위치를 약간 높임

            GameObject item = Instantiate(Item, this.gameObject.transform);
            item.transform.position = tr;
        }
    }
}
