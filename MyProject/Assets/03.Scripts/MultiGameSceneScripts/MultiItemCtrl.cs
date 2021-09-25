using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiItemCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    float LifeTime = 6.0f;
    MutiGameMgr gameMgrRef = null;
    internal int PickupPlayer = -1;  // 몬스터를 죽인사람 ID 받기
    int isPlayerPick = -1;
    PhotonView pv = null;

    int ItemValue = 50;
    // Start is called before the first frame update
    void Start()
    {
        gameMgrRef = GameObject.Find("MultiGameMgr").GetComponent<MutiGameMgr>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0.0f)
        {
            if (pv != null && pv.IsMine == true && PickupPlayer >= 0)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;
        
        if (pv == null)
            return;
        
        if (pv.IsMine == false)
            return;
        
        if (isPlayerPick >= 0)
            return;
        
        if (other.gameObject.tag == "Player")
        {            
            MultiPlayerInfoCtrl refHero = other.gameObject.GetComponentInChildren<MultiPlayerInfoCtrl>();           
            if (refHero != null)
            {
                refHero.pv.RPC("GetMineral", refHero.pv.Owner, ItemValue);
                isPlayerPick = refHero.pv.Owner.ActorNumber;
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PickupPlayer);
        }
        else
        {
            PickupPlayer = (int)stream.ReceiveNext();
        }
    }
}
