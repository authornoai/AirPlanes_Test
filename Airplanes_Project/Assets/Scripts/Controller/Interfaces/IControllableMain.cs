using UnityEngine;
using System.Collections;

namespace AirP.Control
{
    public interface IControllableMain : IControllable
    {
        Transform Transform{ get; }
    }
}
