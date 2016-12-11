using UnityEngine;
using System.Collections;

public interface InputInterface {
    Vector2 GetScreenPointerCoordinates();
    Vector2 GetVelocityDelta();
    bool IsShootingButtonReleased();
    bool IsShootingButtonKeepPressed();
    bool IsShootingButtonPressed();
    bool IsItemButtonPressed();
    bool isDashButtonPressed();
    bool isSecondaryAttackButtonPressed();
}
