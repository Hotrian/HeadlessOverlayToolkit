using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class TwitchSettings
{
    public string Username;
    public string Channel;
    public float X, Y, Z;
    public HOTK_Overlay.AttachmentDevice Device;
    public HOTK_Overlay.AttachmentPoint Point;
    public HOTK_Overlay.AnimationType Animation;

    public float BackgroundR, BackgroundG, BackgroundB;

    public float AlphaStart, AlphaEnd, AlphaSpeed;
    public float ScaleStart, ScaleEnd, ScaleSpeed;
}

public static class TwitchSettingsSaver
{
    public static string Current;
    public static Dictionary<string, TwitchSettings> SavedSettings = new Dictionary<string, TwitchSettings>();

    //it's static so we can call it from anywhere
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedSettings.gd");
        bf.Serialize(file, SavedSettings);
        file.Close();
        Debug.Log("Saved " + SavedSettings.Count + " config(s).");
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedSettings.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedSettings.gd", FileMode.Open);
            SavedSettings = (Dictionary<string, TwitchSettings>)bf.Deserialize(file);
            file.Close();
            Debug.Log("Loaded " + SavedSettings.Count + " config(s).");
        }
    }
}