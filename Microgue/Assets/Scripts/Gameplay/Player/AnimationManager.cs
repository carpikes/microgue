using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class AnimationManager : MonoBehaviour {

    Animator animator;
    GameObject mainChar;

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
        string d = null;
        if( args.TryGetValue(InputManager.IS_FACING_RIGHT, out d) )
            isRight = bool.Parse(d);

        Vector3 flip = mainChar.transform.localScale;
        flip.x *= -1f;
        mainChar.transform.localScale = flip;
    }

    private void OnMainCharAttack(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharSecondAttack(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharMove(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharIdle(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharHit(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharDeath(Bundle args)
    {
        throw new NotImplementedException();
    }

    private void OnMainCharDash(Bundle args)
    {
        throw new NotImplementedException();
    }

}
