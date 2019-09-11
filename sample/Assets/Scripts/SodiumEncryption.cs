using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sodium;
using System.Text;

public class SodiumEncryption : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string msg = "sodium test world";
        var noonce = Sodium.SecretBox.GenerateNonce();
        var key = Sodium.SecretBox.GenerateKey();

        var detachedBox = Sodium.SecretBox.CreateDetached(Encoding.UTF8.GetBytes(msg), noonce, key);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
