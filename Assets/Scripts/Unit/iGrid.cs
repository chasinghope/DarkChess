using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class iGrid : MonoBehaviour
{
    public Vector2 posID;
    public Chess chess
    {
        get
        {
            return transform.GetComponentInChildren<Chess>();
        }
    }
    public Team team;                                                       //   改网格是哪儿一队伍的地盘

    Transform pathTrans;
    Transform selectedTag;
    Transform goneTag;
    Button btn;

    GameManager gml;
    public GameManager gm
    {
        get
        {
            if (gml == null)
                gml = GameManager.Instance;
            return gml;
        }
    }

    private void Start()
    {
        // gm = GameManager.Instance;

        // 初始化pathTag
        pathTrans = Instantiate(gm.pathTag).transform;
        pathTrans.SetParent(transform);
        pathTrans.localPosition = Vector3.zero;
        pathTrans.localScale = Vector3.one;
        // 初始化selectedTag
        selectedTag = Instantiate(gm.selectedTag).transform;
        selectedTag.SetParent(transform);
        selectedTag.localPosition = Vector3.zero;
        selectedTag.localScale = Vector3.one;

        // 初始化goneTag
        goneTag = Instantiate(gm.goneTag).transform;
        goneTag.SetParent(transform);
        goneTag.localPosition = Vector3.zero;
        goneTag.localScale = Vector3.one;


        HidePathTag();
        HideSelectedTag();
        HideGoneTag();
        

        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Click);
    }

    #region 按钮点击事件
    void Click()
    {
        // 如果不是自己的回合，点击无效
        if (!gm.isSelfRound)
            return;

        if( gm.FirstChess == null)                             // 如果第一次点击的是格子， 无操作
        {
            Debug.Log("第一次点击空格子，无操作");
            return;
        }
        else                                                             // 如果第二次点击的是格子
        {
            // 移动棋子

            if (gm.selfStage == GameStage.LayOut)                          // 如果是     【布局阶段】
            {
                if (gm.FirstChess.IGridCanBeMove(this))
                {
                    ChessMove(gm.FirstChess, this);
                    //Debug.Log("第二次点击空格子，移动棋子");
                    gm.FirstChess = null;
                    return;
                }
            }
            
            if(gm.selfStage == GameStage.Doing)                          // 如果是       【下棋阶段】
            {
                if( gm.FirstChess.IGridCanBeMove(this))
                {

                    // 发送网络消息给对端
                    gm.ChessActionBehavior("Move", gm.FirstChess.posID.x, gm.FirstChess.posID.y, posID.x, posID.y);

                    ChessMove(gm.FirstChess, this);
                    gm.FirstChess.State = ChessState.Show;

                    gm.FirstChess = null;



                    // 改变回合
                    gm.ChangeGameRound();
                    return;
                }
            }
        }
    }
    #endregion

    #region 显示与隐藏

    /// <summary>
    /// 显示该iGrid的标志点
    /// </summary>
    public void ShowPathTag()
    {
        pathTrans.gameObject.SetActive(true);
        pathTrans.SetAsLastSibling();
    }

    /// <summary>
    /// 隐藏该iGrid的标志点
    /// </summary>
    public void HidePathTag()
    {
        pathTrans.gameObject.SetActive(false);
    }


    /// <summary>
    /// 显示选中该iGrid的标志点
    /// </summary>
    public void ShowSelectedTag()
    {
        selectedTag.gameObject.SetActive(true);
        selectedTag.SetAsLastSibling();
    }

    /// <summary>
    /// 隐藏选中该iGrid的标志点
    /// </summary>
    public void HideSelectedTag()
    {
        selectedTag.gameObject.SetActive(false);
    }


    /// <summary>
    /// 显示走过该iGrid的标志点
    /// </summary>
    public void ShowGoneTag()
    {
        goneTag.gameObject.SetActive(true);
        goneTag.SetAsLastSibling();
    }

    /// <summary>
    /// 隐藏走过该iGrid的标志点
    /// </summary>
    public void HideGoneTag()
    {
        goneTag.gameObject.SetActive(false);
    }





    #endregion


    /// <summary>
    /// 设置 IGrid 的posID
    /// </summary>
    /// <param name="x">id x</param>
    /// <param name="y">id y</param>
    public void SetPosID(int x, int y)
    {
        this.posID.x = x;
        this.posID.y = y;
    }

    /// <summary>
    /// 检查目标posID是否是自己
    /// </summary>
    /// <param name="posID"></param>
    /// <returns></returns>
    public bool CheckPosID(Vector2 posID)
    {

        if (this.posID == posID)
            return true;

        return false;
    }

    /// <summary>
    /// 移动棋子到目标网格
    /// </summary>
    /// <param name="chess">要移动的棋子</param>
    /// <param name="igrid">目标网格</param>
    public void ChessMove(Chess chess, iGrid igrid)
    {
        // 走子轨迹
        gm.ChangeGoneIGrid(chess.iGrid);
        gm.ChangeSelectedChess(igrid);

        chess.HidePath();
        chess.SetNewGrid(igrid);
        gm.PlayChessMoveSound();
    }








}
