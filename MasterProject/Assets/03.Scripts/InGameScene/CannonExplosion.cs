using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonExplosion : MonoBehaviour
{
    List<GameObject> targetList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TOWER")
        {
            for(int i =0; i < targetList.Count; i++)
            {
                if (targetList[i] == other.gameObject)
                    return;
            }
            targetList.Add(other.gameObject);
        }
    }

    public void Explosion(int a_Damage = 25)
    {
        if (targetList.Count < 1)
            return;

        for(int i =0; i<targetList.Count; i++)
        {
            if (targetList[i] == null)
                continue;
            TowerCtrl_Team a_EnemyNode = null;
            a_EnemyNode = targetList[i].GetComponentInParent<TowerCtrl_Team>();
            
            if(a_EnemyNode != null)
                a_EnemyNode.TakeDamage(a_Damage);
            CommandTowerMgr a_CmdNode = null;
            a_CmdNode = targetList[i].GetComponent<CommandTowerMgr>();

            if (a_CmdNode != null)
                a_CmdNode.TakeDamage(a_Damage);
        }
    }
}
