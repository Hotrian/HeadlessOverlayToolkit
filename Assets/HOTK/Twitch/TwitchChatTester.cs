using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(TwitchIRC), typeof(TextMesh))]
public class TwitchChatTester : MonoBehaviour
{
    public static TwitchChatTester Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<TwitchChatTester>()); }
    }

    private static TwitchChatTester _instance;

    public struct TwitchChat
    {
        public readonly string Name;
        public readonly string Color;
        public readonly string Message;

        public TwitchChat(string name, string color, string message)
        {
            Name = name;
            Color = color;
            Message = message;
        }
    }

    public InputField UsernameBox;
    public InputField OAuthBox;
    public InputField ChannelBox;
    public Button ConnectButton;
    public Text ConnectButtonText;

    public TextMesh TextMesh
    {
        get { return _textMesh ?? (_textMesh = GetComponent<TextMesh>()); }
    }
    private TextMesh _textMesh;

    public TwitchIRC IRC
    {
        get { return _irc ?? (_irc = GetComponent<TwitchIRC>()); }
    }
    private TwitchIRC _irc;

    private readonly Dictionary<string, string> _userColors = new Dictionary<string, string>();

    private readonly List<TwitchChat> _userChat = new List<TwitchChat>();

    private bool _connected;

    public void Awake()
    {
        _instance = this;
    }

    public void ToggleConnect()
    {
        if (!_connected)
        {
            if (UsernameBox != null && UsernameBox.text != "")
            {
                if (OAuthBox != null && OAuthBox.text != "")
                {
                    if (ChannelBox != null && ChannelBox.text != "")
                    {
                        UsernameBox.interactable = false;
                        OAuthBox.interactable = false;
                        ChannelBox.interactable = false;
                        ConnectButtonText.text = "Press to Disconnect";

                        _connected = true;
                        OnChatMsg(ToTwitchNotice(string.Format("Logging into #{0} as {1}!", ChannelBox.text, UsernameBox.text)));
                        IRC.NickName = UsernameBox.text;
                        IRC.Oauth = OAuthBox.text;
                        IRC.ChannelName = ChannelBox.text.ToLower();

                        IRC.enabled = true;
                        IRC.MessageRecievedEvent.AddListener(OnChatMsg);
                        IRC.StartIRC();
                    }
                    else OnChatMsg(ToTwitchNotice("Unable to Connect: Enter a Valid Channel Name!", true));
                }
                else OnChatMsg(ToTwitchNotice("Unable to Connect: Enter a Valid OAuth Key! http://www.twitchapps.com/tmi/", true));
            }
            else OnChatMsg(ToTwitchNotice("Unable to Connect: Enter a Valid Username!", true));
        }
        else
        {
            UsernameBox.interactable = true;
            OAuthBox.interactable = true;
            ChannelBox.interactable = true;
            ConnectButtonText.text = "Press to Connect";

            _connected = false;
            IRC.MessageRecievedEvent.RemoveListener(OnChatMsg);
            IRC.enabled = false;
            OnChatMsg(ToTwitchNotice("Disconnected!", true));
        }
    }

    private void OnChatMsg(string msg)
    {
        var cmd = msg.Split(' ');
        var nickname = cmd[0].Split('!')[0].Substring(1);
        var mode = cmd[1];
        var channel = cmd[2].Substring(1);
        var len = cmd[0].Length + cmd[1].Length + cmd[2].Length + 4;
        var chat = msg.Substring(len);

        switch (mode)
        {
            case "NOTICE":
                if (nickname == "tmi.twitch.tv")
                {
                    nickname = "Twitch";
                    if (chat.StartsWith("Error"))
                        channel = "System-Red";
                }
                switch (channel)
                {
                    case "System-Green":
                        AddMsg(nickname, ColorToHex(new Color(0f, 1f, 0f)), chat);
                        break;
                    case "System-Red":
                        AddMsg(nickname, ColorToHex(new Color(1f, 0f, 0f)), chat);
                        break;
                    case "System-Blue":
                        AddMsg(nickname, ColorToHex(new Color(0.2f, 0.4f, 1f)), chat);
                        break;
                    case "System-Yellow":
                        AddMsg(nickname, ColorToHex(new Color(1f, 1f, 0f)), chat);
                        break;
                    case "System-Purple":
                        AddMsg(nickname, ColorToHex(new Color(1f, 0f, 1f)), chat);
                        break;
                    default:
                        AddMsg(nickname, ColorToHex(new Color(1f, 1f, 1f)), chat);
                        break;
                }
                break;
            case "PRIVMSG":
                nickname = FirstLetterToUpper(nickname);
                if (!_userColors.ContainsKey(nickname))
                {
                    var r = Mathf.Max(0.25f, Random.value);
                    var g = Mathf.Max(0.25f, Random.value);
                    var b = Mathf.Max(0.25f, Random.value);
                    _userColors.Add(nickname, ColorToHex(new Color(r, g, b)));
                }

                string hex;
                if (_userColors.TryGetValue(nickname, out hex))
                    AddMsg(nickname, hex, chat);
                break;
        }
    }

    public void AddSystemNotice(string msgIn, bool warning = false)
    {
        OnChatMsg(string.Format(warning ? ":System NOTICE *System-Yellow :{0}" : ":System NOTICE *System-Blue :{0}", msgIn));
    }

    public static string ToTwitchNotice(string msgIn, bool error = false)
    {
        return string.Format(error ? ":Twitch NOTICE *System-Red :{0}" : ":Twitch NOTICE *System-Green :{0}", msgIn);
    }

    private void AddMsg(string nickname, string color, string chat)
    {
        _userChat.Add(new TwitchChat(nickname, color, chat));

        while (_userChat.Count > 27)
            _userChat.RemoveAt(0);
        
        WordWrapText(_userChat);
    }

    private void WordWrapText(List<TwitchChat> messages)
    {
        var lines = new List<string>();
        TextMesh.text = "";
        var ren = TextMesh.GetComponent<Renderer>();
        var rowLimit = 0.9f; //find the sweet spot
        foreach (var m in messages)
        {
            TextMesh.text = string.Format("{0}: ", m.Name);
            var builder = string.Format("<color=#{0}FF>{1}</color>: ", m.Color, m.Name);
            var parts = m.Message.Split(' ');
            foreach (var t in parts)
            {
                TextMesh.text += t + " ";
                if (ren.bounds.extents.x > rowLimit)
                {
                    lines.Add(builder.TrimEnd() + System.Environment.NewLine);
                    TextMesh.text = "";
                    builder = "";
                }
                builder += t + " ";
            }

            if (builder != "")
                lines.Add(builder.TrimEnd() + System.Environment.NewLine);
        }
        
        TextMesh.text = lines.Aggregate("", (current, t) => current + t);
    }

    public static string ColorToHex(Color32 color)
    {
        return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, a);
    }

    public static string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}
