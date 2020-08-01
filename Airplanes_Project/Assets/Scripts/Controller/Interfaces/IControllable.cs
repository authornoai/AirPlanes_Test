using UnityEngine;
using System.Collections;

namespace AirP.Control
{
    public interface IControllable
    {
        int Index { get; set; }

        void SetController(IController control);
        IController Controller { get; }
    }
}
