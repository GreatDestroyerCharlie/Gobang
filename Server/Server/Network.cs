using System;
using Multiplay;

public class Network
{
    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="ip">IPv4地址</param>
    public Network(string ip)
    {
        //注册
        Server.Register(MessageType.HeartBeat, _HeartBeat);
        Server.Register(MessageType.Enroll, _Enroll);
        Server.Register(MessageType.CreatRoom, _CreatRoom);
        Server.Register(MessageType.EnterRoom, _EnterRoom);
        Server.Register(MessageType.ExitRoom, _ExitRoom);
        Server.Register(MessageType.StartGame, _StartGame);
        Server.Register(MessageType.PlayChess, _PlayChess);
        //启动服务器
        Server.Start(ip);
    }

    private void _HeartBeat(Player player, byte[] data)
    {
        //仅做回应
        player.Send(MessageType.HeartBeat);
    }

    private void _Enroll(Player player, byte[] data)
    {
        Enroll result = new Enroll();

        Enroll receive = NetworkUtils.Deserialize<Enroll>(data);

        Console.WriteLine($"玩家{player.Name}改名为{receive.Name}");
        //设置玩家名字
        player.Name = receive.Name;

        //向玩家发送成功操作结果
        result.Suc = true;
        result.Name = receive.Name;
        data = NetworkUtils.Serialize(result);
        player.Send(MessageType.Enroll, data);
    }

    private void _CreatRoom(Player player, byte[] data)
    {
        //结果
        CreatRoom result = new CreatRoom();

        CreatRoom receive = NetworkUtils.Deserialize<CreatRoom>(data);

        //逻辑检测(玩家不在任何房间中 并且 不存在该房间)
        if (!player.InRoom && !Server.Rooms.ContainsKey(receive.RoomId))
        {
            //新增房间
            Room room = new Room(receive.RoomId);
            Server.Rooms.Add(room.RoomId, room);
            //添加玩家
            room.Players.Add(player);
            player.EnterRoom(receive.RoomId);

            Console.WriteLine($"玩家:{player.Name}创建房间成功");

            //向客户端发送操作结果
            result.Suc = true;
            result.RoomId = receive.RoomId;
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.CreatRoom, data);
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}创建房间失败");
            //向客户端发送操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.CreatRoom, data);
        }
    }

    private void _EnterRoom(Player player, byte[] data)
    {
        //结果
        EnterRoom result = new EnterRoom();

        EnterRoom receive = NetworkUtils.Deserialize<EnterRoom>(data);

        //逻辑检测(玩家不在任何房间中 并且 存在该房间)
        if (!player.InRoom && Server.Rooms.ContainsKey(receive.RoomId))
        {
            Room room = Server.Rooms[receive.RoomId];
            //加入玩家
            if (room.Players.Count < Room.MAX_PLAYER_AMOUNT && !room.Players.Contains(player))
            {
                room.Players.Add(player);
                player.EnterRoom(receive.RoomId);

                Console.WriteLine($"玩家:{player.Name}成为了房间:{receive.RoomId}的玩家");

                //向玩家发送成功操作结果
                result.RoomId = receive.RoomId;
                result.result = EnterRoom.Result.Player;
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.EnterRoom, data);
            }
            //加入观察者
            else if (room.OBs.Count < Room.MAX_OBSERVER_AMOUNT && !room.OBs.Contains(player))
            {
                room.OBs.Add(player);
                player.EnterRoom(receive.RoomId);

                Console.WriteLine($"玩家:{player.Name}成为了房间:{receive.RoomId}的观察者");

                //向玩家发送成功操作结果
                result.RoomId = receive.RoomId;
                result.result = EnterRoom.Result.Observer;
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.EnterRoom, data);
            }
            //加入房间失败
            else
            {
                Console.WriteLine($"玩家:{player.Name}加入房间失败");

                result.result = EnterRoom.Result.None;
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.EnterRoom, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}进入房间失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.EnterRoom, data);
        }
    }

    private void _ExitRoom(Player player, byte[] data)
    {
        //结果
        ExitRoom result = new ExitRoom();

        ExitRoom receive = NetworkUtils.Deserialize<ExitRoom>(data);

        //逻辑检测(有该房间)
        if (Server.Rooms.ContainsKey(receive.RoomId))
        {
            //确保有该房间并且玩家在该房间内
            if (Server.Rooms[receive.RoomId].Players.Contains(player) ||
                Server.Rooms[receive.RoomId].OBs.Contains(player))
            {
                result.Suc = true;
                //移除该玩家
                if (Server.Rooms[receive.RoomId].Players.Contains(player))
                {
                    Server.Rooms[receive.RoomId].Players.Remove(player);
                }
                else if (Server.Rooms[receive.RoomId].OBs.Contains(player))
                {
                    Server.Rooms[receive.RoomId].OBs.Remove(player);
                }

                if (Server.Rooms[receive.RoomId].Players.Count == 0)
                {
                    Server.Rooms.Remove(receive.RoomId); //如果该房间没有玩家则移除该房间
                }

                Console.WriteLine($"玩家:{player.Name}退出房间成功");

                player.ExitRoom();
                //向玩家发送成功操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.ExitRoom, data);
            }
            else
            {
                Console.WriteLine($"玩家:{player.Name}退出房间失败");
                //向玩家发送失败操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.ExitRoom, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}退出房间失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.ExitRoom, data);
        }
    }

    private void _StartGame(Player player, byte[] data)
    {
        //结果
        StartGame result = new StartGame();

        StartGame receive = NetworkUtils.Deserialize<StartGame>(data);

        //逻辑检测(有该房间)
        if (Server.Rooms.ContainsKey(receive.RoomId))
        {
            //玩家模式开始游戏
            if (Server.Rooms[receive.RoomId].Players.Contains(player) &&
                Server.Rooms[receive.RoomId].Players.Count == Room.MAX_PLAYER_AMOUNT)
            {
                //游戏开始
                Server.Rooms[receive.RoomId].State = Room.RoomState.Gaming;

                Console.WriteLine($"玩家:{player.Name}开始游戏成功");

                //遍历该房间玩家
                foreach (var each in Server.Rooms[receive.RoomId].Players)
                {
                    //开始游戏者先手
                    if (each == player)
                    {
                        result.Suc = true;
                        result.First = true;
                        data = NetworkUtils.Serialize(result);
                        each.Send(MessageType.StartGame, data);
                    }
                    else
                    {
                        result.Suc = true;
                        result.First = false;
                        data = NetworkUtils.Serialize(result);
                        each.Send(MessageType.StartGame, data);
                    }
                }

                //如果有观察者
                if (Server.Rooms[receive.RoomId].OBs.Count > 0)
                {
                    result.Suc = true;
                    result.Watch = true;
                    data = NetworkUtils.Serialize(result);
                    //向观战者发送信息
                    foreach (var each in Server.Rooms[receive.RoomId].OBs)
                    {
                        each.Send(MessageType.StartGame, data);
                    }
                }
            }
            else
            {
                Console.WriteLine($"玩家:{player.Name}开始游戏失败");
                //向玩家发送失败操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.StartGame, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}开始游戏失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.StartGame, data);
        }
    }

    private void _PlayChess(Player player, byte[] data)
    {
        //结果
        PlayChess result = new PlayChess();

        PlayChess receive = NetworkUtils.Deserialize<PlayChess>(data);

        //逻辑检测(有该房间)
        if (Server.Rooms.ContainsKey(receive.RoomId))
        {
            //该房间中的玩家有资格下棋
            if (Server.Rooms[receive.RoomId].Players.Contains(player) &&
                Server.Rooms[receive.RoomId].State == Room.RoomState.Gaming &&
                receive.Chess == Server.Rooms[receive.RoomId].GamePlay.Turn)
            {
                //判断结果
                Chess chess = Server.Rooms[receive.RoomId].GamePlay.Calculate(receive.X, receive.Y);
                //检测操作:如果游戏结束
                bool over = _ChessResult(chess, result);

                if (result.Suc)
                {
                    result.Chess = receive.Chess;
                    result.X = receive.X;
                    result.Y = receive.Y;

                    Console.WriteLine($"玩家:{player.Name}下棋成功");

                    //向该房间中玩家与观察者广播结果
                    data = NetworkUtils.Serialize(result);
                    foreach (var each in Server.Rooms[receive.RoomId].Players)
                    {
                        each.Send(MessageType.PlayChess, data);
                    }
                    foreach (var each in Server.Rooms[receive.RoomId].OBs)
                    {
                        each.Send(MessageType.PlayChess, data);
                    }

                    if (over)
                    {
                        Console.WriteLine("游戏结束,房间关闭");
                        Server.Rooms[receive.RoomId].Close();
                    }
                }
                else
                {
                    Console.WriteLine($"玩家:{player.Name}下棋失败");
                    //向玩家发送失败操作结果
                    data = NetworkUtils.Serialize(result);
                    player.Send(MessageType.PlayChess, data);
                }
            }
            else
            {
                Console.WriteLine($"玩家:{player.Name}下棋失败");
                //向玩家发送失败操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.PlayChess, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}下棋失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.PlayChess, data);
        }
    }

    private bool _ChessResult(Chess chess, PlayChess result)
    {
        bool over = false;
        //操作成功
        result.Suc = true;
        switch (chess)
        {
            case Chess.Null:
                //操作失败
                result.Suc = false;
                break;
            case Chess.None:
                result.Challenger = Chess.None;
                break;
            case Chess.Black:
                over = true;
                result.Challenger = Chess.Black;
                break;
            case Chess.White:
                over = true;
                result.Challenger = Chess.White;
                break;
            case Chess.Draw:
                over = true;
                result.Challenger = Chess.Draw;
                break;
        }

        return over;
    }
}