using System;

namespace Multiplay
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        None,         //空类型
        HeartBeat,    //心跳包验证
        Enroll,       //注册
        CreatRoom,    //创建房间
        EnterRoom,    //进入房间
        ExitRoom,     //退出房间
        StartGame,    //开始游戏
        PlayChess,    //下棋
    }

    /// <summary>
    /// 棋子类型
    /// </summary>
    public enum Chess
    {
        //棋子类型
        None, //空棋
        Black,//黑棋
        White,//白棋

        //以下用于胜利判断结果和操作结果
        Draw, //平局
        Null, //表示无结果(用于用户操作失败情况下的返回值)
    }

    [Serializable]
    public class Enroll
    {
        public string Name;//姓名
        public bool Suc;   //是否成功
    }

    [Serializable]
    public class CreatRoom
    {
        public int RoomId; //房间号码
        public bool Suc;   //是否成功
    }

    [Serializable]
    public class EnterRoom
    {
        public int RoomId;      //房间号码
        public Result result;   //结果

        public enum Result
        {
            None,
            Player,
            Observer,
        }
    }

    [Serializable]
    public class ExitRoom
    {
        public int RoomId;  //房间号码
        public bool Suc;    //是否成功
    }

    [Serializable]
    public class StartGame
    {
        public int RoomId;            //房间号码

        public bool Suc;              //是否成功
        public bool First;            //是否先手
        public bool Watch;            //是否是观察者
    }

    [Serializable]
    public class PlayChess
    {
        public int RoomId;       //房间号码
        public Chess Chess;      //棋子类型
        public int X;            //棋子坐标
        public int Y;            //棋子坐标

        public bool Suc;         //操作结果
        public Chess Challenger; //胜利者
    }
}