using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingMgr : MonoBehaviour
{
    public GameObject m_TankMoveImg = null;
    public GameObject m_ImgObj = null;

    float m_MoveSpeed = 200.0f;
    float m_RotSpeed = 50.0f;
    Vector3 m_MoveDir = Vector3.zero;
    Vector3 m_Rot = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        m_MoveDir = m_TankMoveImg.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_TankMoveImg != null)
        {
            m_MoveDir.x += m_MoveSpeed * Time.deltaTime;
            m_TankMoveImg.transform.position = m_MoveDir;

            if (m_TankMoveImg.transform.position.x > 1000)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MyRoom");
                Destroy(m_TankMoveImg);
            }
        }

        if(m_ImgObj != null)
        {
            m_Rot.z = m_RotSpeed * Time.deltaTime;
            m_ImgObj.transform.Rotate(m_Rot);
        }
    }
}
