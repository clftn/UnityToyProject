using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeCtrl : MonoBehaviour
{
    List<GameObject> target_List = new List<GameObject>();
    SphereCollider range_Coll;
    float damage_Time = 0.5f;
    bool damage_Bool = false;

    void Start()
    {
        range_Coll = this.GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (damage_Time > 0.0f)
        {
            damage_Time -= Time.deltaTime;
            return;
        }

        SkillDamage();
    }

    void SkillDamage()
    {
        if (damage_Bool == true)
            return;

        range_Coll.enabled = false;
        damage_Bool = true;

        for(int ii = 0; ii < target_List.Count; ii++)
        {
            if (target_List[ii] == null)
                return;

            if (target_List[ii].tag.Contains("TOWER") == true)
                if (target_List[ii].name.Contains("CommandTower") == true)
                {
                    CommandTowerMgr command = target_List[ii].GetComponent<CommandTowerMgr>();
                    if (command != null)
                        command.TakeDamage(100);
                }
                else
                {
                    TowerCtrl_Team ctrl = target_List[ii].transform.parent.GetComponent<TowerCtrl_Team>();
                    if (ctrl != null)
                        ctrl.TakeDamage(100);
                }



        }

        Destroy(this.gameObject,3f);
    }

    #region ---------- 사정거리 충돌 체크

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.tag.Contains("TOWER") == true)
            target_List.Add(coll.gameObject);
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.tag.Contains("TOWER") == true)
            target_List.Remove(coll.gameObject);
    }

    #endregion
}
