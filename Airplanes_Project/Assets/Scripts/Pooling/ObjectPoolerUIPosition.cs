
using AirP.Control;

namespace AirP.UI
{
    public class ObjectPoolerUIPosition : ObjectPooler<ViewEnemyPosition>
    {
        private void Start()
        {
            AIController.PoolUIPositions = this;
        }
    }
}
