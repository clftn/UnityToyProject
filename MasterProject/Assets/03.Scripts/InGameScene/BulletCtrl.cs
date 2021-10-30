using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [HideInInspector] public GameObject target_Obj = null;
    [HideInInspector] public float bullet_Damage = 0.0f;
    public GameObject explo_Obj = null;
    float speed = 20.0f;
    Vector3 target_Pos = Vector3.zero;
    Vector3 dir = Vector3.zero;
    

    void Start()
    {
    }

    void Update()
    {
        if (target_Obj == null)
        {
            Destroy(this.gameObject);
            return;
        }
            

        target_Pos = target_Obj.transform.position;
        dir = target_Pos - this.transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * 10.0f).eulerAngles;
        this.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        Destroy(gameObject, 2.0f);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag.Contains("TOWER") == true)
        {
            Vector3 pos = this.transform.position;
            pos.x -= 0.25f;
            pos.z -= 0.25f;
            Instantiate(explo_Obj, pos, Quaternion.identity);
            if(coll.name.Contains("CommandTower") != true)
            {
                TowerCtrl_Team towerCtrl = coll.transform.parent.GetComponent<TowerCtrl_Team>();
                towerCtrl.TakeDamage((int)bullet_Damage);
            }
            else
            {
                CommandTowerMgr command = coll.transform.GetComponent<CommandTowerMgr>();
                command.TakeDamage(bullet_Damage);

            }
                
            Destroy(this.gameObject);
        }
            
    }
}
