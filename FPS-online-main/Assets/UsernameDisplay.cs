using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPv;
    [SerializeField] TMP_Text text;

    void Start()
    {
        if(playerPv.IsMine) 
        { 
            gameObject.SetActive(false);
        }
        text.text = playerPv.Owner.NickName;

    }
}
