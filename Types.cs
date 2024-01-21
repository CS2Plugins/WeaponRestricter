namespace WeaponRestricter
{
    internal static class Types
    {
        internal class Weapon
        {
            internal readonly long def;
            internal readonly string name;
            internal readonly string designer_name;
            internal readonly int price;

            internal Weapon(long def, string name, string designer_name, int price)
            {
                this.def = def;
                this.name = name;
                this.designer_name = designer_name;
                this.price = price;
            }
        }

        internal class RestrictedWeapon : Weapon
        {
            // -1 = fully restricted
            // 0 or Weapon not in cfg = unrestricted
            internal readonly int limit;

            internal RestrictedWeapon(Weapon w, int limit) : base(w.def, w.name, w.designer_name, w.price)
            {
                this.limit = limit;
            }
        }
    }
}