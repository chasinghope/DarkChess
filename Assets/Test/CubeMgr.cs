using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CubeMgr : MonoBehaviour
{
    public List<GameObject> cubeList = new List<GameObject>();

    public List<GameObject> showCube = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in cubeList)
            {
                GameObject go = Instantiate(item.gameObject);
                go.GetComponent<Cube>().posID = new Vector2(10, 10);
                showCube.Add(go);
            }
        }
    }
}
