using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using HarmonyLib;
using TMPro;

namespace ZeepkistBlacklist;

[BepInAutoPlugin("dev.thebox1.zeepkistblacklist")]
[BepInProcess("Zeepkist.exe")]
public partial class Main : BaseUnityPlugin
{
    public new static BepInEx.Logging.ManualLogSource Logger;

    private Harmony Harmony { get; } = new (Id);

    private static string FilePath = Path.Combine(Paths.ConfigPath, "BlackListedPlayers.txt");

    public static bool IsKickDebounce = true;
    public static bool IsKick = true;

    public static List<ulong> BannedPlayers = new ();

    public static void UpdateButtons(HostControlsMenu menu)
    {
        foreach (KickPlayerListItem item in menu.playerManager_kickList)
        {
            item.button_kick.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = IsKick
                ? I2.Loc.LocalizationManager.GetTranslation("Online/MngPlayers/Kick_Button")
                : "Ban";
        }
    }

    public static void AddToBanList(ulong id, string name)
    {
        if (BannedPlayers.Contains(id)) return;
        
        BannedPlayers.Add(id);

        using StreamWriter sw = File.AppendText(FilePath);
        sw.WriteLine(id + " - " + name);
    }

    public void Awake()
    {
        if (!File.Exists(FilePath))
        {
            File.Create(FilePath);
        }
        else
        {
            string[] bannedList = File.ReadAllLines(FilePath);
            
            foreach (string banned in bannedList)
                if (banned != "")
                    BannedPlayers.Add(ulong.Parse(Regex.Split(banned, @" - ")[0]));
        }

        Logger = base.Logger;
        Harmony.PatchAll();
        Logger.LogMessage("Loaded ZeepkistBlacklist");
    }
}