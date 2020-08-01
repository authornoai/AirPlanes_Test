using UnityEngine;

namespace AirP.Control
{
    public interface IController
    {
        Vector2 GetMovementAxises(int index);
        bool GetFireDesire(int index);
        bool GetBoostDesire(int index);
    }
}