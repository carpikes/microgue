﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerAudio : MonoBehaviour {

    FMODUnity.StudioEventEmitter[] emitters;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_MAIN_CHAR_ACTUALLY_HIT, OnHit);
        EventManager.StartListening(Events.ON_MAIN_CHAR_KEEP_ATTACK, OnAttack);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_ACTUALLY_HIT, OnHit);
        EventManager.StopListening(Events.ON_MAIN_CHAR_KEEP_ATTACK, OnAttack);
    }

    // Use this for initialization
    void Start () {
        emitters = GetComponents<FMODUnity.StudioEventEmitter>();

	}

    private void OnAttack(Dictionary<string, string> arg0)
    {
        emitters[0].Play();
    }

    private void OnHit(Dictionary<string, string> arg0)
    {
        emitters[1].Play();
    }

}