using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class TwitchIRC : MonoBehaviour
{
    public string Oauth;
    public string NickName;
    public string ChannelName;
    private const string Server = "irc.twitch.tv";
    private const int Port = 6667;

    //event(buffer).
    public class MsgEvent : UnityEngine.Events.UnityEvent<string> { }
    public MsgEvent MessageRecievedEvent = new MsgEvent();

    private string _buffer = string.Empty;
    private bool _stopThreads;
    private readonly Queue<string> _commandQueue = new Queue<string>();
    private readonly List<string> _recievedMsgs = new List<string>();
    private System.Threading.Thread _inProc, _outProc;
    
    private bool _connected;
    private bool _loggedin;

    public void StartIRC()
    {
        _stopThreads = false;
        var sock = new System.Net.Sockets.TcpClient();
        sock.Connect(Server, Port);
        if (!sock.Connected)
        {
            Debug.Log("Failed to connect!");
            return;
        }
        var networkStream = sock.GetStream();
        var input = new System.IO.StreamReader(networkStream);
        var output = new System.IO.StreamWriter(networkStream);
        
        _loggedin = false;
        _connected = false;
        //Send PASS & NICK.
        output.WriteLine("PASS " + Oauth);
        output.WriteLine("NICK " + NickName.ToLower());
        output.Flush();

        //output proc
        _outProc = new System.Threading.Thread(() => IRCOutputProcedure(output));
        _outProc.Start();
        //input proc
        _inProc = new System.Threading.Thread(() => IRCInputProcedure(input, networkStream));
        _inProc.Start();

        CancelInvoke("CheckConnection");
        Invoke("CheckConnection", 5f);
    }

    private void CheckConnection()
    {
        if (_stopThreads) return;
        lock (_recievedMsgs)
        {
            if (!_loggedin)
            {
                _recievedMsgs.Add(ToNotice("System", "Should be logged in by now.. are the username and oauth correct?", NoticeColor.Yellow));
            }
            else if (!_connected)
            {
                _recievedMsgs.Add(ToNotice("System", "Should be connected by now.. is the channel name correct?", NoticeColor.Yellow));
            }
        }
    }

    private void IRCInputProcedure(System.IO.TextReader input, System.Net.Sockets.NetworkStream networkStream)
    {
        while (!_stopThreads)
        {
            if (!networkStream.DataAvailable)
            {
                Thread.Sleep(20);
                continue;
            }

            _buffer = input.ReadLine();
            if (_buffer == null) continue;
            var tokens = _buffer.Split(' ');
            switch (tokens[1])
            {
                case "PRIVMSG":
                case "NOTICE":
                    lock (_recievedMsgs)
                    {
                        _recievedMsgs.Add(_buffer);
                    }
                    break;
                case "001":
                    lock (_recievedMsgs)
                    {
                        _recievedMsgs.Add(ToTwitchNotice("Logged in! Connecting to chat.."));
                        _loggedin = true;
                    }
                    SendCommand("JOIN #" + ChannelName);
                    break;
                case "JOIN":
                    lock (_recievedMsgs)
                    {
                        _recievedMsgs.Add(ToTwitchNotice(string.Format("Connected to {0}!", tokens[2])));
                        _connected = true;
                    }
                    break;
                default:
                    if (_buffer.StartsWith("PING "))
                    {
                        SendCommand(_buffer.Replace("PING", "PONG"));
                    }
                    break;
            }
        }
    }
    private void IRCOutputProcedure(System.IO.TextWriter output)
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        while (!_stopThreads)
        {
            lock (_commandQueue)
            {
                if (_commandQueue.Count <= 0)
                {
                    Thread.Sleep(20);
                    continue;
                }
                // https://github.com/justintv/Twitch-API/blob/master/IRC.md#command--message-limit 
                //has enough time passed since we last sent a message/command?
                if (stopWatch.ElapsedMilliseconds <= 1750)
                {
                    Thread.Sleep(20);
                    continue;
                }
                //send msg.
                output.WriteLine(_commandQueue.Peek());
                output.Flush();
                //remove msg from queue.
                _commandQueue.Dequeue();
                //restart stopwatch.
                stopWatch.Reset();
                stopWatch.Start();
            }
        }
    }

    public void SendCommand(string cmd)
    {
        lock (_commandQueue)
        {
            _commandQueue.Enqueue(cmd);
        }
    }
    public void SendMsg(string msg)
    {
        lock (_commandQueue)
        {
            _commandQueue.Enqueue("PRIVMSG #" + ChannelName + " :" + msg);
        }
    }
    public void OnEnable()
    {
        _stopThreads = false;
    }
    public void OnDisable()
    {
        _stopThreads = true;
        CancelInvoke("CheckConnection");
    }
    public void OnDestroy()
    {
        _stopThreads = true;
        CancelInvoke("CheckConnection");
    }
    public void Update()
    {
        lock (_recievedMsgs)
        {
            if (_recievedMsgs.Count <= 0) return;
            foreach (var t in _recievedMsgs)
            {
                MessageRecievedEvent.Invoke(t);
            }
            _recievedMsgs.Clear();
        }
    }

    public static string ToTwitchNotice(string msgIn, NoticeColor colorEnum = NoticeColor.Green)
    {
        return ToNotice("Twitch", msgIn, colorEnum);
    }

    public static string ToNotice(string nickname, string msgIn, NoticeColor colorEnum = NoticeColor.Green)
    {
        return string.Format(":{0} NOTICE {1} :{2}", nickname, NoticeColorToString(colorEnum), msgIn);
    }

    public static string NoticeColorToString(NoticeColor colorEnum)
    {
        switch (colorEnum)
        {
            case NoticeColor.Green:
                return "*System-Green";
            case NoticeColor.Red:
                return "*System-Red";
            case NoticeColor.Blue:
                return "*System-Blue";
            case NoticeColor.Yellow:
                return "*System-Yellow";
            case NoticeColor.Purple:
                return "*System-Purple";
            case NoticeColor.White:
                return "*System-White";
            default:
                throw new ArgumentOutOfRangeException("colorEnum", colorEnum, null);
        }
    }

    public enum NoticeColor
    {
        Green,
        Red,
        Blue,
        Yellow,
        Purple,
        White
    }
}
