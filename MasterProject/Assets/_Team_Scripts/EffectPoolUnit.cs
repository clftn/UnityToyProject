using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectPoolUnit : MonoBehaviour
{
    public float m_Delay = 1f;  // 풀에 환원되고 적어도 1초 지난것들 사용해야 된다.
    DateTime m_InactiveTime;    // Active 켰을 때의 시간 꺼졌을 때부터 1초 지난거에 체크를 위해 사용
    EffectPool m_ObjectPool;
    string m_EffectName;

    [SerializeField] float m_EffSize = 0.0f;
    Transform m_ChildObj = null;

    //------------------- ParticleAutoDestroy 를 위해 필요한 부분
    public enum DESTROY_TYPE
    {
        Destroy,
        Inactive,
    }

    DESTROY_TYPE m_destroy = DESTROY_TYPE.Inactive; //풀에 환원하는게 원칙이라 기본값 Inactive
    float m_LifeTime = 0.0f;
    //안꺼지고 Loop도는 파티클들때문에 안꺼지는 파티클을 제어하기위해 
    //LifeTime설정 버프 (지속시간) 같은거에서 사용
    //LifeTime이있다면 이 변수로 제어  없다면 그냥 isPlaying으로 제어
    float m_CurLifeTime;
    ParticleSystem[] m_Particles;

    // Start is called before the first frame update
    void Start()
    {
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        if (gameObject.name.Contains("LaserImpactPFX") == false)
            transform.localScale = new Vector3(m_EffSize, m_EffSize, m_EffSize);
        else if (gameObject.name.Contains("LaserImpactPFX") == true)
        {
            m_ChildObj = transform.GetChild(0);
            if (m_ChildObj == null || m_ChildObj.name != "Desktop")
                return;

            m_ChildObj.gameObject.transform.localScale = new Vector3(m_EffSize, m_EffSize, m_EffSize);
        }
        else
            return;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LifeTime > 0) //Inactive 경우인데 Pool과 연동해야됨
        {
            m_CurLifeTime += Time.deltaTime;
            if (m_CurLifeTime >= m_LifeTime)
            {
                DestroyParticles();
                m_CurLifeTime = 0f;
            }
        }
        else
        {
            bool isPlay = false;
            for (int i = 0; i < m_Particles.Length; i++)
            {
                if (m_Particles[i].isPlaying) // 파티클 재생중인지 체크가능
                {
                    isPlay = true;
                    break;
                }
            }
            if (!isPlay)
            {
                DestroyParticles();
            }
        }
    }//void Update()

    void DestroyParticles()
    {
        switch (m_destroy)
        {
            case DESTROY_TYPE.Destroy:
                Destroy(gameObject);
                break;
            case DESTROY_TYPE.Inactive:
                gameObject.SetActive(false);
                break;
        }
    }
    //------------------- ParticleAutoDestroy 를 위해 필요한 부분

    public void SetObjectPool(string effectName, EffectPool objectPool)
    {
        m_EffectName = effectName; //어떤이팩트
        m_ObjectPool = objectPool; //어떤풀에서관리하는 이팩트
        ResetParent();
    }

    public bool IsReady()
    {
        if (!gameObject.activeSelf) //꺼져있으면 풀에 들어가있음을의미
        {
            TimeSpan timeSpan = DateTime.Now - m_InactiveTime; //현재시간 - 엑티브 껏을때 시간 //timeSpan으로 값이나옴
            if (timeSpan.TotalSeconds > m_Delay)  //시간을 전체 시 / 분 / 초 로 나눠서 받을수있다. 
            {   //timeSpan.TotalSeconds 초로만 반환을 했을때 1초보다 크면

                //엑티브가 꺼진지 1초이상 지나면 이펙트 여러개터트려도 문제가 발생안되서 1초 조건 걸음
                return true;
            }
        }
        return false;
    }

    public void ResetParent()
    {
        transform.SetParent(m_ObjectPool.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        m_InactiveTime = DateTime.Now;
        m_ObjectPool.AddPoolUnit(m_EffectName, this); //액티브가 꺼질 때 메모리풀에 다시 넣어줌
    }
}
