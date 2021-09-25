using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerAniController : MonoBehaviour
{
    enum LobbyAnimState 
    {
        first,
        Second,
        Third
    }

    LobbyAnimState lobbyAniState = LobbyAnimState.first;
    Animator playerAnim = null;
    float firstAniTime = 3.0f;
    float firstAniTimeUse = 3.0f;    

    float SecondAniTime = 3.0f;
    float SecondAniTimeUse = 3.0f;    

    float thirdAniTime = 3.0f;
    float thirdAniTimeUse = 3.0f;    

    // Start is called before the first frame update
    void Start()
    {
        lobbyAniState = LobbyAnimState.first;
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (lobbyAniState) 
        {
            case LobbyAnimState.first:
                {
                    playerAnim.SetBool("IsRun", true);
                    playerAnim.SetBool("IsAutoShot", false);
                    playerAnim.SetBool("IsIdle", false);

                    firstAniTimeUse -= Time.deltaTime;
                    if (firstAniTimeUse <= 0.0f)
                    {
                        lobbyAniState = LobbyAnimState.Second;
                        firstAniTimeUse = firstAniTime;
                    }
                }                
                break;
            case LobbyAnimState.Second:
                {
                    playerAnim.SetBool("IsRun", false);
                    playerAnim.SetBool("IsAutoShot", true);
                    playerAnim.SetBool("IsIdle", false);

                    SecondAniTimeUse -= Time.deltaTime;
                    if (SecondAniTimeUse <= 0.0f)
                    {
                        lobbyAniState = LobbyAnimState.Third;
                        SecondAniTimeUse = SecondAniTime;
                    }
                }
                break;
            case LobbyAnimState.Third:
                {
                    playerAnim.SetBool("IsRun", false);
                    playerAnim.SetBool("IsAutoShot", false);
                    playerAnim.SetBool("IsIdle", true);

                    thirdAniTimeUse -= Time.deltaTime;
                    if (thirdAniTimeUse <= 0.0f)
                    {
                        lobbyAniState = LobbyAnimState.first;
                        thirdAniTimeUse = thirdAniTime;
                    }
                }
                break;
        }
    }
}
