using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStage
{
    None,
    Connected,
    Ready,
    LayOut,
    Wait,
    Doing,
    Over
}

public delegate void ChessAction(Grid one, Grid two);

public class GameManager : MonoBehaviour
{
    #region 单例模式
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    [Header("配置")]
    public Team team;                  // 服务器红色             客户端黑色
    public Team Round;               // 回合
    public GameStage selfStage = GameStage.None;
    public GameStage otherStage = GameStage.None;

    [Header("资源")]
    public GameObject iGrid;
    public GameObject pathTag;
    public GameObject selectedTag;
    public GameObject goneTag;


    [Header("引用")]
    public MyNetManager netManager;
    public Transform ChessBoardBG;
    public Transform ChessBorad;
    public GameUI gameUI;

    public Chess FirstChess;
    public iGrid SelectedIGrid;
    public iGrid GoneIGrid;




    [Header("测试")]
    public List<Chess> chessCluster;
    public List<iGrid> iGridCluster;

    public List<Chess> selfChessCluster;
    public List<Chess> otherChessCluster;

    public bool isSelfRound { get { return (team == Round); } }

    private NetData netDataSend;
    private NetData netDataReceive;

    private void Start()
    {
        netManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetManager>();
        GainGrids();
        netManager.ReceiveEvent += this.On_NetMessageDeal;
        gameUI = UIManager.Instance.GamePanel.GetComponent<GameUI>();


        PlayMainBGM();
    }

    private void Update()
    {
        GameStateCheck();
        #region 键入测试
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (team == Team.black)
                team = Team.red;
            else
            {
                team = Team.black;
            }
        }
        #endregion
    }

    #region Tools

    /// <summary>
    /// 获取棋盘网格
    /// </summary>
    public void GainGrids()
    {
        iGrid[] grids = ChessBorad.GetComponentsInChildren<iGrid>();
        iGridCluster = new List<iGrid>(grids);
    }



    /// <summary>
    /// 根据posID 获取网格
    /// </summary>
    public iGrid GetGrids(Vector2 posid)
    {
        foreach (iGrid item in iGridCluster)
        {
            if (item.CheckPosID(posid) == true)
                return item;
        }
        return null;
    }

    /// <summary>
    /// 判断某个棋子是否是自己的棋子
    /// </summary>
    /// <param name="tmp"></param>
    /// <returns></returns>
    public bool isMyChess(Team tmp)
    {
        return (team == tmp);
    }

    /// <summary>
    /// 该网格是否在游戏管理网格内
    /// </summary>
    /// <param name="tmp"></param>
    /// <returns></returns>
    public bool hasTheIGrid(iGrid tmp)
    {
        return iGridCluster.Contains(tmp);
    }


    /// <summary>
    /// 隐藏GoneIGrid
    /// </summary>
    /// <param name="tmp"></param>
    public void ChangeGoneIGrid(iGrid tmp)
    {
        if(GoneIGrid != null)
        {
            GoneIGrid.HideGoneTag();
        }
        tmp.ShowGoneTag();
        GoneIGrid = tmp;
    }

    /// <summary>
    /// 改变选中的棋子
    /// </summary>
    /// <param name="tmp"></param>
    public void ChangeSelectedChess(iGrid tmp)
    {
        if(SelectedIGrid != null)
        {
            SelectedIGrid.HideSelectedTag();
        }
        tmp.ShowSelectedTag();
        SelectedIGrid = tmp;
    }




    /// <summary>
    /// 在服务器和客户端生成棋子
    /// </summary>
    /// <param name="team">服务器 输入参数为  Team.red ;      客户端输入参数为     Team.black</param>
    void InitChessColor(Team team)
    {
        if (team == Team.black)
        {
            ChessBoardBG.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            Chess[] chesses = Resources.LoadAll<Chess>("Chess/Black");
            foreach (Chess item in chesses)
            {
                GameObject go = Instantiate(item.gameObject);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                selfChessCluster.Add(go.GetComponent<Chess>());
                go.GetComponent<Chess>().State = ChessState.Hide;
            }

            chesses = Resources.LoadAll<Chess>("Chess/Red");
            foreach (Chess item in chesses)
            {
                GameObject go = Instantiate(item.gameObject);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                otherChessCluster.Add(go.GetComponent<Chess>());
            }

        }

        if (team == Team.red)
        {
            ChessBoardBG.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            Chess[] chesses = Resources.LoadAll<Chess>("Chess/Red");
            foreach (Chess item in chesses)
            {
                GameObject go = Instantiate(item.gameObject);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                selfChessCluster.Add(go.GetComponent<Chess>());
                go.GetComponent<Chess>().State = ChessState.Hide;
            }

            chesses = Resources.LoadAll<Chess>("Chess/Black");
            foreach (Chess item in chesses)
            {
                GameObject go = Instantiate(item.gameObject);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                otherChessCluster.Add(go.GetComponent<Chess>());
            }
        }
    }

    void UpdateIGrids()
    {
        foreach (iGrid item in iGridCluster)
        {
            item.GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// 根据UID查找棋子
    /// </summary>
    /// <param name="u_id"></param>
    /// <param name="chessList"></param>
    /// <returns></returns>
    Chess GetChessFromUID(int u_id, List<Chess> chessList)
    {
        foreach (Chess item in chessList)
        {
            if (item.u_id == u_id)
            {

                return item;
            }
        }
        return null;
    }


    /// <summary>
    /// 恢复数据
    /// </summary>
    void ResetData()
    {
        // 恢复对数据的应用
        netDataReceive = null;
        netDataSend = null;
        
        if(chessCluster != null)
        {
            foreach (Chess item in chessCluster)
            {
                Destroy(item.gameObject);
            }
        }

        chessCluster = new List<Chess>(); ;
        selfChessCluster = new List<Chess>();
        otherChessCluster = new List<Chess>();

        selfStage = GameStage.None;
        otherStage = GameStage.None;

        FirstChess = null;

        if(SelectedIGrid != null)
        {
            SelectedIGrid.HideSelectedTag();
            SelectedIGrid = null;
        }

        if(GoneIGrid != null)
        {
            GoneIGrid.HideGoneTag();
            GoneIGrid = null;
        }

    }



    #endregion


    #region 局域网联机模块

    /// <summary>
    /// 游戏作为服务器启动
    /// </summary>
    public void StartAsServer()
    {
        // 网络启动
        netManager.SetupServer();

        team = Team.red;
        Round = Team.red;
        // InitChessColor(team);

        // 设置游戏当前状态
        selfStage = GameStage.Connected;
        // 设置提示文本内容
        gameUI.SetExplainText("等待服务器连接中...");

    }

    /// <summary>
    /// 游戏作为客户端启动
    /// </summary>
    public void StartAsClient()
    {
        // 网络启动
        netManager.SetupClient();

        team = Team.black;
        Round = Team.black;
        // InitChessColor(team);

        // 设置游戏当前状态
        selfStage = GameStage.Connected;
    }

    /// <summary>
    /// 客户端连接服务器
    /// </summary>
    public void ClientConnectToserver(string addr)
    {
        netManager.ClientConnectServer(addr);
    }

    /// <summary>
    /// 游戏作为服务器关闭
    /// </summary>
    public void EndAsServer()
    {
        netManager.DisableServer();
    }

    /// <summary>
    /// 游戏作为客户端关闭
    /// </summary>
    public void EndAsClient()
    {
        netManager.QuitConnectToServer();
    }


    /// <summary>
    /// 发送网络信息
    /// </summary>
    /// <param name="info">信息</param>
    public void SendNetMessage(string info)
    {

         if (team == Team.red)
         {
             netManager.sendMessageToClient(info);
         }
         else
         {
             netManager.sendMessageToServer(info);
         }

    }


    #endregion


    #region 游戏GameUI逻辑

    /// <summary>
    /// 点击准备按钮
    /// </summary>
    public void Ready_onClick()
    {
        if (selfStage == GameStage.Connected || selfStage == GameStage.Ready)
        {
            if (netManager.isConnecting)
            {
                selfStage = GameStage.Ready;
                netDataSend = new NetData("Ready");
                string tmp = JsonMapper.ToJson(netDataSend);
                SendNetMessage(tmp);
                // 文本提示用户
                gameUI.SetExplainText("已准备，等待对方准备");
            }


        }
    }

    /// <summary>
    /// 点击就绪按钮
    /// </summary>
    public void Ok_onClick()
    {
        if (selfStage == GameStage.LayOut && netManager.isConnecting)
        {
            selfStage = GameStage.Wait;
            netDataSend = new NetData("Wait");

            foreach (Chess item in selfChessCluster)
            {
                netDataSend.netChesses.Add(new NetChess(item.u_id, (int)item.posID.x, (int)item.posID.y));
            }

            string tmp = JsonMapper.ToJson(netDataSend);
            // Debug.Log(tmp);
            SendNetMessage(tmp);

            // 文本提示用户
            gameUI.SetExplainText("已就绪，等待对方就绪");
        }
    }

    /// <summary>
    /// 点击认输按钮
    /// </summary>
    public void Giveup_onClick()
    {
        if(selfStage == GameStage.Doing && netManager.isConnecting)
        {
            selfStage = otherStage = GameStage.Over;
            netDataSend = new NetData("Giveup");
            string tmp = JsonMapper.ToJson(netDataSend);
            SendNetMessage(tmp);
            LoseGame();
        }
    }

    /// <summary>
    /// 点击退出按钮
    /// </summary>
    public void Quit_onClick()
    {
        if(team == Team.red)
        {
            EndAsServer();
        }
        
        if(team == Team.black)
        {
            EndAsClient();
        }
        ResetData();

        // netManager.discovery.ClearIPList();
    }

    /// <summary>
    /// 改变自身游戏回合
    /// </summary>
    public void ChangeGameRound()
    {
        if (Round == Team.black)
        {
            Round = Team.red;
            gameUI.SetExplainText("红方走");
        }
        else
        {
            Round = Team.black;
            gameUI.SetExplainText("黑方走");
        }
    }

    /// <summary>
    /// 棋子下棋行为，网络发送
    /// </summary>
    /// <param name="Action">类型，Move    /    Eat</param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <param name="toX"></param>
    /// <param name="toY"></param>
    public void ChessActionBehavior(string Action, float fromX, float fromY, float toX, float toY)
    {
        netDataSend = new NetData("Doing");
        netDataSend.netChessActions = new NetChessAction(Action, (int)fromX, (int)fromY, (int)toX, (int)toY);
        string tmp = JsonMapper.ToJson(netDataSend);
        SendNetMessage(tmp);
    }

    /// <summary>
    /// 赢得比赛
    /// </summary>
    public void WinGame()
    {
        PlayWinSound();
        selfStage = otherStage = GameStage.Over;
        Debug.Log("恭喜你，赢得了比赛");
        gameUI.SetExplainText("恭喜你，赢得了比赛");
    }

    /// <summary>
    /// 输掉比赛
    /// </summary>
    public void LoseGame()
    {
        PlayLoseSound();
        selfStage = otherStage = GameStage.Over;
        Debug.Log("很抱歉，你输了比赛");
        gameUI.SetExplainText("很遗憾，你输了比赛");
    }

    /// <summary>
    /// 网络信息处理
    /// </summary>
    void On_NetMessageDeal(string info)
    {
        netDataReceive = JsonMapper.ToObject<NetData>(info);

        switch (netDataReceive.type)
        {
            case "Ready":
                otherStage = GameStage.Ready;

                if(team == Team.red )
                {
                    netManager.discovery.StopBroadcast();
                }

                break;

            case "Wait":
                otherStage = GameStage.Wait;
                break;

            case "Giveup":

                selfStage = otherStage = GameStage.Over;
                WinGame();

                break;

            case "Doing":

                switch (netDataReceive.netChessActions.Action)
                {

                    case "Move":

                        Chess chess = GetGrids(new Vector2(netDataReceive.netChessActions.fromX, netDataReceive.netChessActions.fromY)).chess;
                        iGrid igrid = GetGrids(new Vector2(netDataReceive.netChessActions.toX, netDataReceive.netChessActions.toY));
                        ChangeGoneIGrid(chess.iGrid);
                        ChangeSelectedChess(igrid);
                        chess.SetNewGrid(igrid);
                        chess.State = ChessState.Show;

                        // 播放音效
                        PlayChessMoveSound();
                        // 改变回合
                        ChangeGameRound();
                        break;

                    case "Eat":

                        Chess chessA = GetGrids(new Vector2(netDataReceive.netChessActions.fromX, netDataReceive.netChessActions.fromY)).chess;
                        Chess chessB = GetGrids(new Vector2(netDataReceive.netChessActions.toX, netDataReceive.netChessActions.toY)).chess;

                        chessA.State = ChessState.Show;
                        chessB.Dead();
                        ChangeGoneIGrid(chessA.iGrid);
                        ChangeSelectedChess(chessB.iGrid);
                        chessA.SetNewGrid(chessB.iGrid);

                        

                        // Debug.Log("网络连接吃子后消息" + chessB.iGrid.posID);

                        // 播放音效
                        PlayEatChessSound();

                        if(selfStage != GameStage.Over)
                        {
                            // 改变回合
                            ChangeGameRound();
                        }

                        break;


                    default:
                        break;
                }




                break;

            case "Over":
                otherStage = GameStage.Over;
                break;



            default:
                break;
        }
    }

    /// <summary>
    /// 游戏状态检测
    /// </summary>
    void GameStateCheck()
    {

        if (selfStage == GameStage.Ready && otherStage == GameStage.Ready)
        {
            Debug.Log("生成棋子");
            selfStage = otherStage = GameStage.LayOut;

            // 播放音效
            PlayStartSound();
            UpdateIGrids();
            InitChessColor(team);

            chessCluster.AddRange(selfChessCluster);
            chessCluster.AddRange(otherChessCluster);

            gameUI.SetExplainText("布局阶段");
        }


        if (selfStage == GameStage.Wait && otherStage == GameStage.Wait)
        {
            gameUI.SetExplainText("游戏开始");

            // 重置棋子状态提醒标志
            if (SelectedIGrid != null)
            {
                SelectedIGrid.HideSelectedTag();
                SelectedIGrid = null;
            }

            if (GoneIGrid != null)
            {
                GoneIGrid.HideGoneTag();
                GoneIGrid = null;
            }

            selfStage = otherStage = GameStage.Doing;
            // 同步棋盘数据， 把对方的棋盘设置为自己的棋盘
            PlayStartSound();
            PlayGameBGM();

            foreach (NetChess item in netDataReceive.netChesses)
            {
                Chess chess = GetChessFromUID(item.u_id, otherChessCluster);
                Vector2 t_posID = new Vector2(item.posIDx, item.posIDy);


                chess.SetNewGrid(GetGrids(t_posID));
                chess.State = ChessState.Hide;
            }



            // 初始化回合，红方先行
            Round = Team.red;
            gameUI.SetExplainText("红方走");


        }

        if (selfStage == GameStage.Over && otherStage == GameStage.Over)
        {
            // 游戏结束

        }


    }

    #endregion


    #region 音乐逻辑

    /// <summary>
    /// 播放主菜单bgm
    /// </summary>
    public void PlayMainBGM()
    {
        MusicMgr.Instance.PlayBGM("MainBGM");
    }

    /// <summary>
    /// 播放游戏时BGM
    /// </summary>
    public void PlayGameBGM()
    {
        MusicMgr.Instance.PlayBGM("GameBGM");
    }

    /// <summary>
    /// 播放按钮点击音效
    /// </summary>
    public void PlayButtonClickSound()
    {
        MusicMgr.Instance.PlaySound("Button");
    }

    /// <summary>
    /// 播放棋子移动音效
    /// </summary>
    public void PlayChessMoveSound()
    {
        MusicMgr.Instance.PlaySound("ChessMove");
    }


    /// <summary>
    /// 播放点击棋子音效
    /// </summary>
    public void PlayClickChessSound()
    {
        MusicMgr.Instance.PlaySound("ClickChess");
    }

    /// <summary>
    /// 播放吃子音效
    /// </summary>
    public void PlayEatChessSound()
    {
        MusicMgr.Instance.PlaySound("EatChess");
    }


    /// <summary>
    /// 播放开始音效
    /// </summary>
    public void PlayStartSound()
    {
        MusicMgr.Instance.PlaySound("Start");
    }

    /// <summary>
    /// 播放胜利音效
    /// </summary>
    public void PlayWinSound()
    {
        MusicMgr.Instance.PlaySound("Win");
    }

    /// <summary>
    /// 播放失败音效
    /// </summary>
    public void PlayLoseSound()
    {
        MusicMgr.Instance.PlaySound("Lose");
    }
    


    #endregion

}
