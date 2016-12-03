using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class AnimationManager : MonoBehaviour {

    Animator animator;
    GameObject mainChar;

    public static string ANIM_MAIN_ATTACK = "mainAttack";
    public static string ANIM_DEATH = "death";
    public static string ANIM_HIT = "hit";
    public static string ANIM_IS_MOVING = "isMoving";

    // to mirror animations
    bool isRight = true;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_MAIN_CHAR_CHANGE_DIR, OnMainCharChangeDir);
        EventManager.StartListening(Events.ON_MAIN_CHAR_ATTACK, OnMainCharAttack);
        EventManager.StartListening(Events.ON_MAIN_CHAR_DASH, OnMainCharDash);
        EventManager.StartListening(Events.ON_MAIN_CHAR_DEATH, OnMainCharDeath);
        EventManager.StartListening(Events.ON_MAIN_CHAR_HIT, OnMainCharHit);
        EventManager.StartListening(Events.ON_MAIN_CHAR_IDLE, OnMainCharIdle);
        EventManager.StartListening(Events.ON_MAIN_CHAR_MOVE, OnMainCharMove);
        EventManager.StartListening(Events.ON_MAIN_CHAR_SECOND_ATTACK, OnMainCharSecondAttack);
        //EventManager.StartListening(Events.ON_MAIN_CHAR_SPAWN, OnMainCharSpawn);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_CHANGE_DIR, OnMainCharChangeDir);
        EventManager.StopListening(Events.ON_MAIN_CHAR_ATTACK, OnMainCharAttack);
        EventManager.StopListening(Events.ON_MAIN_CHAR_DASH, OnMainCharDash);
        EventManager.StopListening(Events.ON_MAIN_CHAR_DEATH, OnMainCharDeath);
        EventManager.StopListening(Events.ON_MAIN_CHAR_HIT, OnMainCharHit);
        EventManager.StopListening(Events.ON_MAIN_CHAR_IDLE, OnMainCharIdle);
        EventManager.StopListening(Events.ON_MAIN_CHAR_MOVE, OnMainCharMove);
        EventManager.StopListening(Events.ON_MAIN_CHAR_SECOND_ATTACK, OnMainCharSecondAttack);
        //EventManager.StopListening(Events.ON_MAIN_CHAR_SPAWN, OnMainCharSpawn);
    }

    // Use this for initialization
    void Start()
    {
        mainChar = GameObject.FindGameObjectWithTag("Player");
        animator = mainChar.GetComponent<Animator>();
    }

    private void OnMainCharChangeDir(Bundle args)
    {
        /* NOT USING THIS INFO! */
        string d = null;
        if( args.TryGetValue(InputManager.IS_FACING_RIGHT, out d) )
            isRight = bool.Parse(d);

        Vector3 flip = mainChar.transform.localScale;
        flip.x *= -1f;
        mainChar.transform.localScale = flip;
    }

    private void OnMainCharAttack(Bundle args)
    {
        animator.SetTrigger(ANIM_MAIN_ATTACK);
    }

    private void OnMainCharSecondAttack(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharMove(Bundle args)
    {
        animator.SetBool(ANIM_IS_MOVING, true);
    }

    private void OnMainCharIdle(Bundle args)
    {
        animator.SetBool(ANIM_IS_MOVING, false);
    }

    private void OnMainCharHit(Bundle args)
    {
        // flashing effect??
        animator.SetTrigger(ANIM_HIT);
    }

    private void OnMainCharDeath(Bundle args)
    {
        animator.SetTrigger(ANIM_DEATH);
    }

    private void OnMainCharDash(Bundle args)
    {
        throw new NotImplementedException();
    }

}
