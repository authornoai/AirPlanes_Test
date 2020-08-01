namespace AirP.Weapon
{
    public class ObjectPoolerProjectile : ObjectPooler<Projectile>
    {
        private void Start()
        {
            Projectile.ToReturn = this;
            WeaponBase.Bullets = this;
        }
    }
}

