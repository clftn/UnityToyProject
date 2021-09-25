using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCtrl : MonoBehaviour
{
    public float Speed = 60.0f;
    public GameObject expEffect;
    CapsuleCollider _collider;
    Rigidbody _rigidbody;
    public int Damage = 80;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        GetComponent<Rigidbody>().AddForce(transform.forward * Speed);

        StartCoroutine(ExplosionCanon(3.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ExplosionCanon(float tm)
    {
        yield return new WaitForSeconds(tm);        

        // 충돌 콜백 함수가 발생하지 않도록 Collider를 비활성화
        if (_collider != null)
            _collider.enabled = false;

        // 리지드 바디 제거
        if (_rigidbody != null)
            _rigidbody.velocity = Vector3.zero;

        // 폭발 프리팹 동적 생성
        GameObject obj = (GameObject)Instantiate(expEffect, transform.position, Quaternion.identity);        
        // 폭발 프리펩 제거
        Destroy(obj, 1.0f);        
        // 해당 프리펩 제거
        Destroy(this.gameObject, 1.0f);        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "StaticLevel") // 벽하고 부딪칠 시 파괴됨(벽이 너무 많아서 여기에)
        {
            StartCoroutine(ExplosionCanon(0.0f));
        }

        if (collision.gameObject.tag == "Monster")
        {
            StartCoroutine(ExplosionCanon(0.0f));
        }
    }    
}
