using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Awake()
    {
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}
