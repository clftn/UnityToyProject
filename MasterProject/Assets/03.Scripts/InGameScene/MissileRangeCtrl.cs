using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileRangeCtrl : MonoBehaviour
{
    Material material;
    Color myColor;
    float alpha = 0.0f;
    float fadeSpeed = 50.0f;
    bool isFadeOut = true;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        alpha = 50.0f / 255.0f;
        Destroy(this.gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Fade();

        myColor = new Color( 255.0f/255.0f , 0.0f/255.0f, 0.0f/255.0f, alpha);
        material.SetColor("_TintColor", myColor);
    }

    void Fade()
    {
        if(isFadeOut == true) // 투명해지기
        {
            alpha -= (fadeSpeed * Time.deltaTime) / 255.0f;

            if(alpha <= 10.0f/255.0f)
            {
                isFadeOut = false;
            }
        }
        else // 진해지기
        {
            alpha += (fadeSpeed * Time.deltaTime) / 255.0f;

            if(alpha >= 50.0f/255.0f)
            {
                isFadeOut = true;
            }
        }
    }
}