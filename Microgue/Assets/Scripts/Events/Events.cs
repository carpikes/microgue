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
    ON_LEVEL_UNLOADING,
    ON_DOOR_TOUCH,

    // main character related
    ON_MAIN_CHAR_HIT,
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

    // enemy related
    ON_ENEMY_HIT,
    ON_ENEMY_DEATH,
    ON_ENEMY_TRIGGER,
    ON_STILL_ENEMIES_LEFT,

    // object related
    ON_ITEM_PICKUP,
    ON_ITEM_USE,

    // time related
    ON_TICK,
    //ON_TIME_FINISHING,
    ON_TIME_ENDED,

    // stat related
    ON_STAT_CHANGED,
    ON_MAIN_CHAR_STOP_ATTACK,
    ON_MAIN_CHAR_KEEP_ATTACK,
}