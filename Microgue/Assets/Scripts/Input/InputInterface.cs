using UnityEngine;
using System.Collections;

public interface InputInterface {
    Vector2 GetScreenPointerCoordinates();
    Vector2 GetVelocityDelta();
    bool IsShootingButtonPressed();
    bool IsItemButtonPressed();
    bool IsDashButtonPressed();
    bool IsSecondaryAttackButtonPressed();

    // Usata dal joystick per centrare
    // il mirino anche quando il player
    // non e` centrato
    void FeedPlayerPosition(Vector2 pos);

    bool IsSkipToBossPressed();
}
