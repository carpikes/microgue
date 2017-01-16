using UnityEngine;
using System.Collections;

public enum Events
{
    // game related
    ON_GAME_START,
    ON_GAME_QUIT,

    // level related
    ON_LEVEL_BEFORE_LOADING,
    ON_LEVEL_AFTER_LOADING,
    ON_LOADING_SCREEN_COMPLETE,

    ON_LEVEL_UNLOADING,
    ON_DOOR_TOUCH,

    // main character related
    ON_MAIN_CHAR_HIT,
    ON_MAIN_CHAR_ACTUALLY_HIT,
    ON_MAIN_CHAR_INVULNERABLE_BEGIN,
    ON_MAIN_CHAR_INVULNERABLE_END,
    ON_MAIN_CHAR_DEATH,
    ON_MAIN_CHAR_SPAWN,
    ON_MAIN_CHAR_START_ATTACK,
    ON_MAIN_CHAR_SECOND_ATTACK,
    ON_MAIN_CHAR_DASH,
    ON_MAIN_CHAR_MOVE,
    ON_MAIN_CHAR_CHANGE_DIR,
    ON_MAIN_CHAR_IDLE,
    ON_MAIN_CHAR_STOP_ATTACK,
    ON_MAIN_CHAR_KEEP_ATTACK,

    // enemy related
    ON_ENEMY_HIT,
    ON_ENEMY_DEATH,
    ON_ENEMY_TRIGGER,
    ON_STILL_ENEMIES_LEFT,

    // specific enemies related
    ON_STOMP_STOMP_SHADOW_TOUCH,

    // object related
    ON_ITEM_PICKUP,
    ON_ITEM_USE,
    ON_SHOW_MESSAGE,

    // time related
    ON_TICK,
    //ON_TIME_FINISHING,
    ON_TIME_ENDED,

    // stat related
    ON_STAT_CHANGED,

    // trying to access boss room
    ON_WORLD_UNEXPLORED,

    // trigger a fade out
    FADE_OUT,
    // trigger a fade in
    FADE_IN,

    ON_BOSS_KILLED,
    ON_BOSS_GOTO,

    UPDATE_SECONDARY_ATTACK,

    INCREMENT_TIME,

    ON_DOOR_UNLOCK,
    VOID_COMPLETED
}