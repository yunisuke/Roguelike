using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    void Awake()
    {
        // GameManagerが存在しない時、GameManagerを作成する
        if (GameManager.instance == null) {
            Instantiate(gameManager);
        }
    }
}
