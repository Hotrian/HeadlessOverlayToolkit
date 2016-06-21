using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(TwitchIRC), typeof(TextMesh))]
public class TwitchChatTester : MonoBehaviour
{
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

    private Dictionary<string, string> _userColors = new Dictionary<string, string>();

    private List<string> _userChat = new List<string>();

    private bool connected = false;

    public void ToggleConnect()
    {
        if (!connected)
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

                        connected = true;
                        AddMsg("Twitch", "ff00ff", string.Format("Connecting to #{0}!", ChannelBox.text));
                        IRC.nickName = UsernameBox.text;
                        IRC.oauth = OAuthBox.text;
                        IRC.channelName = ChannelBox.text;

                        IRC.StartIRC();
                    }
                    else
                    {
                        Debug.LogWarning("Unable to Connect: Enter a Valid Channel Name!");
                    }
                }
                else
                {
                    Debug.LogWarning("Unable to Connect: Enter a Valid OAuth Key! http://www.twitchapps.com/tmi/");
                }
            }
            else
            {
                Debug.LogWarning("Unable to Connect: Enter a Valid Username!");
            }
        }
        else
        {
            UsernameBox.interactable = true;
            OAuthBox.interactable = true;
            ChannelBox.interactable = true;
            ConnectButtonText.text = "Press to Connect";

            connected = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsg);
    }

    // Update is called once per frame
    void Update () {
	
	}

    private void OnChatMsg(string msg)
    {
        var cmd = msg.Split(' ');
        var name = cmd[0].Split('!')[0].Substring(1);
        var mode = cmd[1];
        var channel = cmd[2];
        var len = cmd[0].Length + cmd[1].Length + cmd[2].Length + 4;
        var chat = msg.Substring(len);

        if (!_userColors.ContainsKey(name))
        {
            var r = Mathf.Max(0.5f, Random.value);
            var g = Mathf.Max(0.5f, Random.value);
            var b = Mathf.Max(0.5f, Random.value);
            _userColors.Add(name, colorToHex(new Color(r, g, b)));
        }

        string hex;
        if (_userColors.TryGetValue(name, out hex))
        {
            AddMsg(name, hex, chat);
        }
    }

    private void AddMsg(string name, string color, string chat)
    {
        _userChat.Add(string.Format("<color=#00{0}>{1}</color>: {2}", color, name, chat));

        while (_userChat.Count > 12)
        {
            _userChat.RemoveAt(0);
        }
        var text = _userChat.Aggregate("", (current, t) => current + t + "\n");
        
        WordWrapText(text);
    }

    private void WordWrapText(string text)
    {
        var builder = "";
        TextMesh.text = "";
        var ren = TextMesh.GetComponent<Renderer>();
        var rowLimit = 1.02f; //find the sweet spot
        var parts = text.Split(' ');
        foreach (var t in parts)
        {
            TextMesh.text += t + " ";
            if (ren.bounds.extents.x > rowLimit)
            {
                TextMesh.text = builder.TrimEnd() + System.Environment.NewLine + t + " ";
            }
            builder = TextMesh.text;
        }
    }

    public static string colorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}
