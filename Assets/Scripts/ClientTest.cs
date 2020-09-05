using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientTest : MonoBehaviour
{
    GameClient client;
    // Start is called before the first frame update
    void Start()
    {
        client = new GameClient("127.0.0.1", 4566);
        client.StartListener();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
