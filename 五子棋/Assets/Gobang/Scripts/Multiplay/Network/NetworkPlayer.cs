using System;
using UnityEngine;
using Multiplay;
/// <summary>
/// 一个游戏客户端只能存在一个网络玩家
/// </summary>
public class NetworkPlayer : MonoBehaviour
{
    //单例
    private NetworkPlayer() { }
    public static NetworkPlayer Instance { get; private set; }

    [HideInInspector]
    public Chess Chess;                     //棋子类型
    [HideInInspector]
    public int RoomId = 0;                  //房间号码
    [HideInInspector]
    public bool Playing = false;            //正在游戏
    [HideInInspector]
    public string Name;                     //名字

    public Action<int> OnRoomIdChange;      //房间ID改变
    public Action<bool> OnPlayingChange;    //游戏状态改变
    public Action<Chess> OnStartGame;       //开始游戏
    public Action<string> OnNameChange;     //名字改变

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        OnRoomIdChange += (roomId) => RoomId = roomId;

        OnPlayingChange += (playing) => Playing = playing;

        OnStartGame += (chess) => Chess = chess;

        OnNameChange += (name) => Name = name;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Playing)
        {
            Network.Instance.PlayChessRequest(RoomId);
        }
    }
}