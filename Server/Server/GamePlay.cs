using Multiplay;

public class GamePlay
{
    /// <summary>
    /// 初始化棋盘
    /// </summary>
    public GamePlay()
    {
        ChessState = new Chess[15, 15];
        _totalChess = 0;
        Playing = true;
        Turn = Chess.Black;
    }

    public Chess[,] ChessState;                     //储存棋子状态

    private int _totalChess;                        //总棋数

    public bool Playing;                            //游戏进行中

    public Chess Turn;                              //轮流下棋

    /// <summary>
    /// 计算下棋结果
    /// </summary>
    public Chess Calculate(int x, int y)
    {
        if (!Playing)
        {
            return Chess.Null;
        }

        //逻辑判断
        if (x < 0 || x >= 15 || y < 0 || y >= 15 || ChessState[x, y] != Chess.None)
        {
            return Chess.Null;
        }

        //下棋
        _totalChess++;
        //黑棋
        if (Turn == Chess.Black)
        {
            ChessState[x, y] = Chess.Black;
        }
        //白棋
        else if (Turn == Chess.White)
        {
            ChessState[x, y] = Chess.White;
        }

        //计算结果
        bool? result = _CheckWinner();
        //要么平局要么胜利(任意一方胜利后不在交替下棋,游戏结束)
        if (result != false)
        {
            //游戏结束
            Playing = false;
            //胜利
            if (result == true)
            {
                return Turn;
            }
            //平局
            else
            {
                return Chess.Draw;
            }
        }
        //继续下棋
        else
        {
            //交替下棋
            Turn = (Turn == Chess.Black ? Chess.White : Chess.Black);
            return Chess.None;
        }
    }

    private bool? _CheckWinner()
    {
        //遍历棋盘
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                //各方向连线
                int horizontal = 1, vertical = 1, rightUp = 1, rightDown = 1;
                Chess curPos = ChessState[i, j];

                if (curPos != Turn)
                {
                    continue;
                }

                //判断5连
                for (int link = 1; link < 5; link++)
                {
                    //扫描横线
                    if (i + link < 15)
                    {
                        if (curPos == ChessState[i + link, j])
                        {
                            horizontal++;
                        }
                    }
                    //扫描竖线
                    if (j + link < 15)
                    {
                        if (curPos == ChessState[i, j + link])
                        {
                            vertical++;
                        }
                    }
                    //扫描右上斜线
                    if (i + link < 15 && j + link < 15)
                    {
                        if (curPos == ChessState[i + link, j + link])
                        {
                            rightUp++;
                        }
                    }
                    //扫描右下斜线
                    if (i + link < 15 && j - link >= 0)
                    {
                        if (curPos == ChessState[i + link, j - link])
                        {
                            rightDown++;
                        }
                    }
                }

                //胜负判断
                if (horizontal == 5 || vertical == 5 || rightUp == 5 || rightDown == 5)
                {
                    return true;
                }
            }
        }

        //棋盘下满
        if (_totalChess == ChessState.GetLength(0) * ChessState.GetLength(1))
        {
            //平局
            return null;
        }

        return false;
    }
}