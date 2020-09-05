using System.Collections.Generic;



public class NetData
{
    public string type;
    public List<NetChess> netChesses = new List<NetChess>();
    public NetChessAction netChessActions;

    public NetData()
    {

    }

    public NetData(string type)
    {
        this.type = type;
    }
}

public class NetChess
{
    public int u_id;
    public int posIDx;
    public int posIDy;

    public NetChess()
    {

    }

    public NetChess(int u, int x, int y)
    {
        this.u_id = u;
        this.posIDx = x;
        this.posIDy = y;
    }
}

public class NetChessAction
{
    public string Action;
    public int fromX, fromY;
    public int toX, toY;


    public NetChessAction()
    {

    }

    public NetChessAction(string Action, int fromX, int fromY, int toX, int toY)
    {
        this.Action = Action;
        this.fromX = fromX;
        this.fromY = fromY;
        this.toX = toX;
        this.toY = toY;
    }

}




