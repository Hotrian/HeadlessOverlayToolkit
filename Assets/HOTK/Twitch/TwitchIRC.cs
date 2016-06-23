using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    }
    private void IRCInputProcedure(System.IO.TextReader input, System.Net.Sockets.NetworkStream networkStream)
    {
        while (!_stopThreads)
        {
            if (!networkStream.DataAvailable)
                continue;

            _buffer = input.ReadLine();
            
            if (_buffer == null) continue;
            if (_buffer.Contains("PRIVMSG #") || _buffer.Split(' ')[1] == "NOTICE")
            {
                lock (_recievedMsgs)
                {
                    _recievedMsgs.Add(_buffer);
                }
            }
            else if (_buffer.StartsWith("PING "))
            {
                SendCommand(_buffer.Replace("PING", "PONG"));
            }
            else if (_buffer.Split(' ')[1] == "001")
            {
                lock (_recievedMsgs)
                {
                    _recievedMsgs.Add(TwitchChatTester.ToTwitchNotice("Connected!"));
                }
                SendCommand("JOIN #" + ChannelName);
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
                if (_commandQueue.Count <= 0) continue;
                // https://github.com/justintv/Twitch-API/blob/master/IRC.md#command--message-limit 
                //have enough time passed since we last sent a message/command?
                if (stopWatch.ElapsedMilliseconds <= 1750) continue;
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
    }
    public void OnDestroy()
    {
        _stopThreads = true;
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
}
