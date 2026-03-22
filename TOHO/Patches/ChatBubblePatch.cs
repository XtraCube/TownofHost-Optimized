using AmongUs.GameOptions;
using HarmonyLib;
using TOHO.Roles.Core;
using UnityEngine;

namespace TOHO.Patches;

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetRight))]
class ChatBubbleSetRightPatch
{
    public static void Postfix(ChatBubble __instance)
    {
        if (Main.isChatCommand) __instance.SetLeft();
    }
}
[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
class ChatBubbleSetNamePatch
{
    public static void Postfix(ChatBubble __instance, [HarmonyArgument(1)] bool isDead, [HarmonyArgument(2)] bool voted)
    {
        switch (ThemeOptionItem.ThemeID)
        {
            case 1:
                __instance.TextArea.color = Color.black;

                if (isDead)
                    __instance.Background.color = new(0.9f, 0.9f, 0.9f, 153);
                else
                    __instance.Background.color = new(0.9f, 0.9f, 0.9f, 255);                
                break;
            case 2:
                __instance.TextArea.color = Color.white;

                if (isDead)
                    __instance.Background.color = new(0.1f, 0.1f, 0.1f, 153);
                else
                    __instance.Background.color = new(0.1f, 0.1f, 0.1f, 255);
                break;
            case 3:
                __instance.TextArea.color = Color.white;

                __instance.TextArea.color = Color.white;

                if (isDead)
                    __instance.Background.color = new Color32(112, 33, 25, 153);
                else
                    __instance.Background.color = new Color32(112, 33, 25, 255);
                break;
            case 4:
                __instance.TextArea.color = Color.white;

                if (isDead)
                    __instance.Background.color = new Color32(117, 83, 11, 153);
                else
                    __instance.Background.color = new Color32(117, 83, 11, 255);
                break;
            case 5:
                __instance.TextArea.color = Color.white;

                if (isDead)
                    __instance.Background.color = new Color32(36, 69, 25, 153);
                else
                    __instance.Background.color = new Color32(36, 69, 25, 255);
                break;
            case 6:
                __instance.TextArea.color = Color.white;

                if (isDead)
                    __instance.Background.color = new Color32(6, 13, 56, 153);
                else
                    __instance.Background.color = new Color32(6, 13, 56, 255);
                
                break;
        }

        if (!GameStates.IsInGame) return;

        var seer = PlayerControl.LocalPlayer;
        var target = __instance.playerInfo.Object;

        if (seer.PlayerId == target.PlayerId)
        {
            __instance.NameText.color = seer.GetRoleColor();
            return;
        }

        // Dog shit
        var seerRoleClass = seer.GetRoleClass();

        // if based role is Shapeshifter and is Desync Shapeshifter
        if (seerRoleClass?.ThisRoleBase.GetRoleTypes() == RoleTypes.Shapeshifter && seer.HasDesyncRole())
        {
            __instance.NameText.color = Color.white;
        }
        if (Main.PlayerStates[seer.PlayerId].IsNecromancer || Main.PlayerStates[target.PlayerId].IsNecromancer)
        {
            // When target is impostor, set name color as white
            __instance.NameText.color = Color.white;
        }
    }
}

