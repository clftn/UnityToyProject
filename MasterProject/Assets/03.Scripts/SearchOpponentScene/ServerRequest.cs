using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerRequest : MonoBehaviour
{
    [Header("RenderTextuerObj")]
    public RawImage RenderTexture_RImg;
    public Image _Img;
    public Text RepeatTxt;
    public static string RepeatStr = "서버 응답 요청 중입니다. . .";
    public Text TipTxt;
    [HideInInspector] public string TipStr = "Tip : 공격 전 유닛을 정비하세요!";
    public GameObject Obj_3D;
    float RotY = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("ServerRequest");
        if (RepeatTxt != null)
            RepeatTxt.text = "";
        if (TipTxt != null)
            TipTxt.text = TipStr;
        StartCoroutine(Repeat_Text(RepeatStr, .3f));
    }

    // Update is called once per frame
    void Update()
    {
        RotY += Time.deltaTime * 50.0f;
        if (_Img != null)
            _Img.transform.localRotation = Quaternion.Euler(0, 0, RotY);
        if (Obj_3D != null)
            Obj_3D.transform.localRotation = Quaternion.Euler(0, RotY, 0);
    }

    IEnumerator Repeat_Text(string Txt, float Deltime)
    {
        char[] chars = Txt.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (RepeatTxt != null)
                RepeatTxt.text += chars[i];
            yield return new WaitForSeconds(Deltime);
            if (i == chars.Length - 1)
            {
                i = -1;
                RepeatTxt.text = "";
                continue;
            }
        }
    }

    private void OnDestroy()
    {
        //Debug.Log("Connect Server");
    }
}
