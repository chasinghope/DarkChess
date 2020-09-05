using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChessTools : MonoBehaviour
{
    [MenuItem("ChessBoradTools/Clear ChessBoard")]
    static void ClearChessBoard()
    {
        Transform go = GameObject.Find("ChessBoard").transform;
        Transform[] chessItems = go.GetComponentsInChildren<Transform>();
        Debug.Log("Tools delete nums : "  + chessItems.Length);
        
        foreach (Transform item in chessItems)
        {
            if (item == go)
                continue;
            DestroyImmediate(item.gameObject);
        }
    }



}
