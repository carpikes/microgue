using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class AnimationManager : MonoBehaviour {
    Animator animator;
    GameObject mainChar;
    SpriteRenderer spriteRenderer;
    StatManager statManager;

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
        spriteRenderer = mainChar.GetComponent<SpriteRenderer>();

        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
    }

    private void OnMainCharChangeDir(Bundle args)
    {
        string d = null;
        if( args.TryGetValue(InputManager.IS_FACING_RIGHT, out d) )
            isRight = bool.Parse(d);

        mainChar.transform.localScale = new Vector3(isRight ? -1 : 1, 1, 1);
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
        if (!statManager.IsInvulnerable)
        {
            animator.SetTrigger(ANIM_HIT);
            StartCoroutine(MainCharFlashing());
        }
    }

    private IEnumerator MainCharFlashing()
    {
        const int iterations = 10;

        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_INVULNERABLE_BEGIN, null);

        for (int i = 0; i < iterations; ++i)
        {
            if( spriteRenderer) spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer) spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_INVULNERABLE_END, null);
    }

    private void OnMainCharDeath(Bundle args)
    {
        animator.SetTrigger(ANIM_DEATH);
    }

    private void OnMainCharDash(Bundle args)
    {
        //throw new NotImplementedException();
    }

}
