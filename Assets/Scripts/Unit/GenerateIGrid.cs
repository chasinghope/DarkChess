using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateIGrid : MonoBehaviour
{

    [Header("数据")]
    public float GridWidth = 30f;
    public float GridHeigth = 30f;

    [Header("引用")]
    public GameObject ChessBoard;
    public GameObject iGrid;

    // Start is called before the first frame update
    void Start()
    {
        CreateChessiGrids();
    }


    /// <summary>
    /// 生成棋盘网格
    /// </summary>
    void CreateChessiGrids()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject go = Instantiate(iGrid);
                go.transform.SetParent(ChessBoard.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(j * GridWidth, i * GridHeigth, 0);
                go.transform.SetAsLastSibling();
                go.GetComponent<iGrid>().SetPosID(j, i);
                if (i < 5)
                    go.GetComponent<iGrid>().team = Team.red;
                else
                    go.GetComponent<iGrid>().team = Team.black;

                go.name = "[ " + j.ToString() + " , " + i.ToString() + " ]";
            }
        }
        DestroyImmediate(this);
    }

}
