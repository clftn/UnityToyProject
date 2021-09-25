using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiBulletCtrl : MonoBehaviour
{
    public int damage = 50;
    public float speed = 5000.0f;
    public float lifeTime = 4.0f;
    internal int MakerId = -1;

    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 4.0f;
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "StaticLevel") // 벽하고 부딪칠 시 파괴됨(벽이 너무 많아서 여기에)
        {
            Destroy(this.gameObject);
        }
    }
}
