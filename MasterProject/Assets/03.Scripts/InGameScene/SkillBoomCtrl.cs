using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBoomCtrl : MonoBehaviour
{
    public GameObject sky_Obj = null;
    public GameObject boom_Obj = null;
    public GameObject range_Obj = null;
    public GameObject sound_Obj = null;
    float sound_Delay = 0.0f;
    float boom_Delay = 0.0f;
    float target_dist = 0.0f;
    float end_dist = 0.0f;
    Vector3 target_Pos = Vector3.zero;
    Vector3 start_Pos = Vector3.zero;
    Vector3 end_Pos = Vector3.zero;

    bool range_Bool = true;


    void Start()
    {
    }

    void Update()
    {
        if (boom_Delay > 0.0f)
            boom_Delay -= Time.deltaTime;

        if (sound_Delay > 0.0f)
            sound_Delay -= Time.deltaTime;

        Vector3 pos = end_Pos - start_Pos;
        this.transform.Translate(pos * 0.2f * Time.deltaTime);

        target_dist = Vector3.Distance(target_Pos, this.transform.position);
        end_dist = Vector3.Distance(end_Pos, this.transform.position);

        if (target_dist < 3.5)
        {
            BoomCreate();
            SoundCreate();

            if (range_Bool == true)
            {
                range_Bool = false;
                Vector3 range_pos = target_Pos;
                range_pos.y = 1.0f;
                Instantiate(range_Obj, range_pos, Quaternion.identity);
            }    
        }
            

        if (end_dist < 2.0f)
            Destroy(this.gameObject);
    }

    void BoomCreate()
    {
        if (boom_Delay > 0.0f)
            return;

        float randX = Random.Range(-6.0f, 10.0f);
        float randZ = Random.Range(-6.0f, 10.0f);
        Vector3 pos = target_Pos;
        pos.x += randX - 1;
        pos.z += randZ - 1;
        pos.y = 1.0f;

        Instantiate(boom_Obj, pos, sky_Obj.transform.rotation);
        boom_Delay = 0.005f;
    }

    void SoundCreate()
    {
        if (sound_Delay > 0.0f)
            return;

        Instantiate(sound_Obj, this.transform.position, transform.rotation);
        sound_Delay = 0.1f;
    }

    public void TargetSetting(Vector3 a_Target_Pos)
    {
        start_Pos = this.transform.position;
        target_Pos = a_Target_Pos;
        target_Pos.y = start_Pos.y;
        end_Pos = target_Pos + (start_Pos - target_Pos) * -1;
        end_Pos.y = start_Pos.y;
    }
}
