using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ZeepkistBlacklist;

[HarmonyPatch(typeof(GameMaster), "Update")]
public class GameMasterUpdatePatch
{
    static void Postfix(GameMaster __instance)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) Main.IsKick = false;
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) Main.IsKick = true;

        if (__instance.manager.hostControls_gameObject.activeSelf && Main.IsKick != Main.IsKickDebounce) Main.UpdateButtons(__instance.manager.hostControls);

        Main.IsKickDebounce = Main.IsKick;
    }
}

[HarmonyPatch(typeof(HostControlsMenu), "GoFromHomeToPlayerManager")]
public class HostControlsMenuGoFromHomeToPlayerManagerPatch
{
    static void Postfix(HostControlsMenu __instance)
    {
        Main.UpdateButtons(__instance);
    }
}

[HarmonyPatch(typeof(KickPlayerListItem), "KickPlayer")]
public class KickPlayerListItemKickPlayerPatch
{
    static void Postfix(KickPlayerListItem __instance)
    {
        if (!Main.IsKick && __instance.hasPlayer) Main.AddToBanList(ulong.Parse(__instance.thePlayer.UserId), __instance.thePlayer.NickName);
    }
}

[HarmonyPatch(typeof(PhotonZeepkist), "OnPlayerEnteredRoom")]
public class PhotonZeepkistOnPlayerEnteredRoomPatch
{
    static bool Prefix(PhotonZeepkist __instance, Player other)
    {
        if (Main.BannedPlayers.Contains(ulong.Parse(other.UserId)))
        {
            PhotonNetwork.CloseConnection(other);
            return false;
        }
        
        return true;
    }
}