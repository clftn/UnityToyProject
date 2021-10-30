using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbleZone : MonoBehaviour
{
    UnitPlacing unitPlacing = null;

    private void Awake()
    {
        unitPlacing = GameObject.FindObjectOfType<UnitPlacing>();

        //if(unitPlacing != null)
        //    StateUpdate();
        // SetActive == false 이므로 인보크로 대기시킨다.
        if (unitPlacing != null)
            InvokeRepeating("StateUpdate", 0.2f, 0.2f);
    }

    void StateUpdate()
    {
        if (unitPlacing != null && unitPlacing.placingState == UnitPlacingState.INSTANCE)
        {
            this.gameObject.SetActive(true);
        }
        else
            this.gameObject.SetActive(false);
    }


}
