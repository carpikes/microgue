using UnityEngine;
using System.Collections;

class SoundList
{
    // PLAYER
    public static string PLAYER_ATTACK  = "event:/SoundFX/Player/SFX_PLayerAttack";
    public static string PLAYER_HURT = "event:/SoundFX/Player/SFX_PLayerHurt";

    // UI
    public static string UI_PAUSE = "event:/SoundFX/UI/SFX_UI_PauseGame";
    public static string UI_CONFIRM = "event:/SoundFX/UI/SFX_UI_Confirmed";
    public static string UI_MOUSE_OVER = "event:/SoundFX/UI/SFX_UI_MouseOver";

    // OTHER SOUNDS
    public static string WORLD_VIEWPORT_CHANGE = "event:/SoundFX/SFX_ViewportChange";
    public static string WORLD_KILL_ALL_ENEMIES = "event:/SoundFX/SFX_KillAllEnemiesFirst";
    public static string ITEM_PICKUP = "event:/SoundFX/SFX_PickUp";
}