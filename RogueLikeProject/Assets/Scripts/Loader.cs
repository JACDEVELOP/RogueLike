using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    // Start is called before the first frame update
    //Vamos a verificar si el GameManager.Instances es igual a nulo
    void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }
}
