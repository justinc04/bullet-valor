using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameObject killGraphic;

    private void Awake()
    {
        Instance = this;
    }
}
