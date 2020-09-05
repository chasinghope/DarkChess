using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team
{
    black,
    red
}

public enum ChessState
{
    Hide,
    Show,
    Dead
}


public enum ChessType
{
    jiang,
    shuai,
    che,
    ma,
    xiang,
    shi,
    pao,
    bing,
    zu
}




public class Chess : MonoBehaviour
{
    public iGrid iGrid;
    public int u_id;

    ChessState state;
    public ChessState State
    {
        private get { return state; } set { state = value;  Display();  }
    }
    public ChessType chessType = ChessType.shi;
    public Team team;
    public Vector2 posID;                // posID 游戏底层数据结构

    Button btn;
    public Image maskl;
    public Image mask
    {
        get
        {
            if(maskl == null)
            {
                // Debug.Log("获取Image");
                maskl = transform.GetChild(0).GetComponent<Image>();
                // Debug.Log(transform.GetChild(0).name);
            }
            return maskl;
        }
    }

    GameManager gml;
    public GameManager gm
    {
        get
        {
            if(gml == null)
                gml = GameManager.Instance;
            return gml;
        }
    }
    Team enemyTeam;


    [Header("测试")]
    public List<iGrid> path;


    private void Awake()
    {
        // 获取游戏必要引用和事件绑定

        path = new List<iGrid>();
        btn = this.GetComponent<Button>();

        btn.onClick.AddListener(Click);
        if(team == Team.black)
        {
            enemyTeam = Team.red;
        }
        else
        {
            enemyTeam = Team.black;
        }

        // 根据posID设置新的网格点
        SetNewGrid( GameManager.Instance.GetGrids( posID ) );

    }

    #region 按钮点击事件
    void Click()
    {
        // 如果不是自己的回合，点击无效
        if (!gm.isSelfRound)
            return;


        if (  gm.FirstChess == null)                                           //  一次操作中，第一次点击的对象只能是自己的棋子
        {
            if( gm.isMyChess(team) )                                        // 如果是自己的棋子，则将其存入 FirstChess
            {

                if(gm.selfStage == GameStage.LayOut)              // 如果是     【布局阶段】
                {
                    gm.PlayClickChessSound();
                    gm.FirstChess = this;
                    // this.iGrid.ShowSelectedTag();
                    gm.ChangeSelectedChess(this.iGrid);
                }

                if(gm.selfStage == GameStage.Doing)              // 如果是       【下棋阶段】
                {
                    gm.PlayClickChessSound();
                    gm.FirstChess = this;
                    // 显示可走路径
                    ShowPath();
                    // this.iGrid.ShowSelectedTag();
                    gm.ChangeSelectedChess(this.iGrid);
                }
                return;
            }
            else
            {
                return;
            }
        }
        else                                                                           // 第二次点击， 分是自己的棋子和对方的棋子两个情况来讨论
        {
            if( gm.isMyChess(team))                                       // 是自己的棋子
            {
                if( gm.selfStage == GameStage.LayOut)         // 如果是     【布局阶段】
                {
                    // 交换两个棋子
                    ExchageSelfChess(gm.FirstChess, this);
                    gm.PlayClickChessSound();
                    Debug.Log("交换棋子");
                    // gm.FirstChess.iGrid.HideSelectedTag();
                    gm.ChangeSelectedChess(gm.FirstChess.iGrid);
                    gm.FirstChess = null;
                    return;
                }
                if (gm.selfStage == GameStage.Doing)         // 如果是       【下棋阶段】
                {
                    gm.FirstChess.HidePath();
                    gm.FirstChess = this;
                    gm.ChangeSelectedChess(this.iGrid);
                    gm.PlayClickChessSound();
                    //TODO 显示棋子可以走包含吃子的所有网格点
                    ShowPath();
                    Debug.Log("显示路径");

                    return;
                }
            }
            else                                                                      //  不是自己的棋子
            {
                if( gm.selfStage == GameStage.Doing)          // 如果是    【下棋阶段】
                {
                    // 吃子
                    gm.PlayEatChessSound();

                    EatOtherChess(gm.FirstChess, this);
                    gm.FirstChess.State = ChessState.Show;
                    gm.FirstChess = null;
                    return;
                }
            }
        }


    }
    #endregion



    /// <summary>
    /// 为棋子设置新的网格点
    /// </summary>
    /// <param name="tmpGrid">新的网格点</param>
    public void SetNewGrid(iGrid tmpGrid)
    {
        if (tmpGrid == null)
            return;
        if( iGrid != null)
        {
            iGrid.GetComponent<Button>().interactable = true;
        }
        tmpGrid.GetComponent<Button>().interactable = false;
        iGrid = tmpGrid;
        transform.SetParent(iGrid.transform);
        this.posID = iGrid.posID;
        transform.localPosition = Vector3.zero;

    }



    #region 棋子行为方法

    /// <summary>
    /// 交换两个棋子，在布局阶段使用
    /// </summary>
    /// <param name="one">第一个棋子</param>
    /// <param name="two">要交换位置的棋子</param>
    public void ExchageSelfChess(Chess one, Chess two)
    {
        iGrid tmpGrid = two.iGrid;
        two.SetNewGrid(one.iGrid);
        one.SetNewGrid(tmpGrid);
    }

    /// <summary>
    /// 棋子死亡
    /// </summary>
    public void Dead()
    {
        gm.chessCluster.Remove(this);
        Destroy(this.gameObject);

        if ( chessType == ChessType.jiang )
        {
            if(gm.team == Team.black)
            {
                gm.LoseGame();
            }
            else
            {
                gm.WinGame();
            }
        }

        if( chessType == ChessType.shuai)
        {
            if (gm.team == Team.red)
            {
                gm.LoseGame();
            }
            else
            {
                gm.WinGame();
            }
        }

    }


    /// <summary>
    /// 第一个棋子吃掉第二个棋子
    /// </summary>
    /// <param name="one">第一个棋子</param>
    /// <param name="two">要被吃掉第二个棋子</param>
    public void EatOtherChess(Chess one, Chess two)
    {
        if( one.path.Contains(two.iGrid))
        {
            // 发送网络消息给对端
            gm.ChessActionBehavior("Eat", one.posID.x, one.posID.y, two.posID.x, two.posID.y);

            //　显示吃子路径
            gm.ChangeGoneIGrid(one.iGrid);
            gm.ChangeSelectedChess(two.iGrid);

            Debug.Log("吃掉对方棋子");
            one.SetNewGrid(two.iGrid);
            one.HidePath();
            two.Dead();

            // 赢得比赛
            if(two.chessType == ChessType.jiang || two.chessType == ChessType.shuai)
            {
                gm.PlayWinSound();
                gm.selfStage = GameStage.Over;
            }

            // 改变游戏回合
            if(gm.selfStage != GameStage.Over)
            {
                gm.ChangeGameRound();
            }

        }
    }

    /// <summary>
    /// 显示可吃或者可走的网格点， 即路径
    /// </summary>
    public void ShowPath()
    {

        // 士为例子
        switch (chessType)
        {
            case ChessType.jiang:
                jiang_Behaviour();
                break;
            case ChessType.che:
                che_Behaviour();
                break;
            case ChessType.ma:
                ma_Behaviour();
                break;
            case ChessType.xiang:
                xiang_Behaviour();
                break;
            case ChessType.shi:
                shi_Behaviour();
                break;
            case ChessType.pao:
                pao_Behaviour();
                break;
            case ChessType.bing:
                bing_Behaviour();
                break;
            case ChessType.zu:
                zu_Behaviour();
                break;
            default:
                break;
        }



        foreach (iGrid item in path)
        {
            item.ShowPathTag();
        }
    }


    /// <summary>
    /// 在棋子可走路径添加网格
    /// </summary>
    /// <param name="igrid"></param>
    void PathAdd(iGrid igrid)
    {
        if( igrid != null  )
        {
            if( igrid.chess == null )
                 path.Add(igrid);
            else
            {
                if (igrid.chess.team == this.enemyTeam)
                    path.Add(igrid);
            }
        }
    }

    /// <summary>
    /// 隐藏该棋子可走路径
    /// </summary>
    public void HidePath()
    {
        foreach (iGrid item in path)
        {
            item.HidePathTag();
        }
        path.Clear();
    }

    /// <summary>
    /// 判断该玩网格是否可被棋子前往
    /// </summary>
    /// <param name="igrid"></param>
    /// <returns></returns>
    public bool IGridCanBeMove(iGrid igrid)
    {
        if(gm.selfStage == GameStage.LayOut)
        {
            if (igrid.team == this.team)
                return true;
        }

        if(gm.selfStage == GameStage.Doing)
        {
            if (path.Contains(igrid))
            {
                return true;
            }
        }



        return false;
    }



    #endregion

    #region 不同棋子走子，吃子路径方法

    /// <summary>
    /// 士的走法
    /// </summary>
    void shi_Behaviour()
    {
        iGrid igrid;
        List<Vector2> offsetList;

        offsetList = new List<Vector2>()
        {
            new Vector2(-1 ,1),
            new Vector2(-1, -1),
            new Vector2(1, 1),
            new Vector2(1, -1)
        };

        foreach (Vector2 offset in offsetList)
        {
            igrid = gm.GetGrids(this.posID + offset);
            PathAdd(igrid);
        }
    }

    /// <summary>
    /// 兵的走法
    /// </summary>
    void bing_Behaviour()
    {
        iGrid igrid;
        List<Vector2> offsetList;

        offsetList = new List<Vector2>()
        {
            new Vector2(0 ,1),
            new Vector2(-1, 0),
            new Vector2(1, 0)
        };

        foreach (Vector2 offset in offsetList)
        {
            igrid = gm.GetGrids(this.posID + offset);
            PathAdd(igrid);
        }
    }

    /// <summary>
    /// 车的走法
    /// </summary>
    void che_Behaviour()
    {
        iGrid igrid;
        List<Vector2> offsetList;

        offsetList = new List<Vector2>()
        {
            new Vector2(0 ,1),
            new Vector2(-1, 0),
            new Vector2(1, 0),
            new Vector2(0, -1)
        };

        Vector2 dir;
        foreach (Vector2 offset in offsetList)
        {

            dir = offset;
            for (int i = 1; i <= 10; i++)
            {
                igrid = gm.GetGrids(this.posID + dir);

                // 如果搜索路径没有网格
                if (igrid == null)
                    break;

                // 如果是空格子
                if (igrid.chess == null)
                {
                    PathAdd(igrid);
                    dir = offset * i;
                }

                // 如果是己方棋子
                if (igrid.chess != null && igrid.chess.team == this.team)
                {
                    break;
                }

                if (igrid.chess != null && igrid.chess.team == this.enemyTeam)
                {
                    PathAdd(igrid);
                    break;
                }
            }

        }

    }

    /// <summary>
    /// 炮的走法
    /// </summary>
    void pao_Behaviour()
    {
        iGrid igrid;
        List<Vector2> offsetList;

        offsetList = new List<Vector2>()
        {
            new Vector2(0 ,1),
            new Vector2(-1, 0),
            new Vector2(1, 0),
            new Vector2(0, -1)
        };

        Vector2 dir;

        foreach (Vector2 offset in offsetList)
        {
            dir = offset;
            for (int i = 1; i <= 10 ; i++)
            {
                igrid = gm.GetGrids(posID + dir);

                // 如果搜索路径没有网格
                if (igrid == null)
                    break;

                // 如果是空格子
                if (igrid.chess == null)
                {
                    PathAdd(igrid);
                    dir = offset * i;
                }


                // 如果是己方棋子
                if( igrid.chess != null)
                {
                    // 继续搜索该网格之后的棋子，如果有敌人的棋子则加入路径中
                    Vector2 dir2 = offset;
                    for (int j = 1; j < 9; j++)
                    {
                        igrid = gm.GetGrids(posID + dir + dir2);
                        // 如果搜索路径没有网格
                        if (igrid == null)
                            break;

                        if(igrid.chess == null)
                        {
                            dir2 = offset * j;
                            continue;
                        }
                        else
                        {
                            if (igrid.chess.team == this.enemyTeam)
                                PathAdd(igrid);
                        }


                    }

                    break;
                }

            }

        }
    }

    /// <summary>
    /// 将，帅的走法
    /// </summary>
    void jiang_Behaviour()
    {

    }

    /// <summary>
    /// 相的走法
    /// </summary>
    void xiang_Behaviour()
    {
        iGrid igrid;
        iGrid igrid2;
        List<Vector2> offsetList;

        offsetList = new List<Vector2>()
        {
            new Vector2(-1 ,1),
            new Vector2(-1, -1),
            new Vector2(1, 1),
            new Vector2(1, -1)
        };

        foreach (Vector2 offset in offsetList)
        {
            igrid = gm.GetGrids(this.posID + offset);
            igrid2 = gm.GetGrids(this.posID + offset * 2);

            if ( igrid2 != null)                 // igrid2不为空，igrid1必不为空
            {
                if (igrid.chess == null)
                {
                    if( igrid2.chess == null || igrid2.chess.team == this.enemyTeam)
                        PathAdd(igrid2);
                }

            }
        }
    }

    /// <summary>
    /// 马的走法
    /// </summary>
    void ma_Behaviour()
    {
        iGrid igrid0, igrid1, igrid2;


        List<Vector2> offsetList;
        List<Vector2> targetList1;
        List<Vector2> targetList2;



        offsetList = new List<Vector2>()
        {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        };

        targetList1 = new List<Vector2>()
        {
            new Vector2(-1, 2),
            new Vector2(2, 1),
            new Vector2(1, -2),
            new Vector2(-2, -1)
        };

        targetList2 = new List<Vector2>()
        {
            new Vector2(1, 2),
            new Vector2(2, -1),
            new Vector2(-1,- 2),
            new Vector2(-2, 1)
        };

        for (int i = 0; i < 4; i++)
        {
            igrid0 = gm.GetGrids(posID + offsetList[i]);


            // 如果该阻塞点不存在，则该方向搜索完毕
            if (igrid0 == null)
                continue;

            // 如果该阻塞点为空
            if( igrid0.chess == null)
            {
                igrid1 = gm.GetGrids(posID + targetList1[i]);
                igrid2 = gm.GetGrids(posID + targetList2[i]);

                if (igrid1 != null)
                {
                    if (igrid1.chess == null || igrid1.chess.team == this.enemyTeam)
                        PathAdd(igrid1);
                }

                if (igrid2 != null)
                {
                    if (igrid2.chess == null || igrid2.chess.team == this.enemyTeam)
                        PathAdd(igrid2);
                }
            }

        }

    }

    /// <summary>
    /// 卒的走法
    /// </summary>
    void zu_Behaviour()
    {
        iGrid igrid;
        List<Vector2> offsetList;

        offsetList = new List<Vector2>()
        {
            new Vector2(0 ,-1),
            new Vector2(-1, 0),
            new Vector2(1, 0)
        };

        foreach (Vector2 offset in offsetList)
        {
            igrid = gm.GetGrids(this.posID + offset);
            PathAdd(igrid);
        }
    }
    #endregion


    #region 棋子显示方法


    /// <summary>
    /// 展示棋子
    /// </summary>
    void Display()
    {
        if (State == ChessState.Show)
        {
            mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.0f);
        }

        if (State == ChessState.Hide)
        {
            if (this.team == gm.team)
            {
                mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.5f);
            }
            else
            {
                mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 1f);
            }
        }
    }




    #endregion



}

