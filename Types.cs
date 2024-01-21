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
            internal readonly int limit;

            internal Weapon(long def, string name, string designer_name, int price)
            {
                this.def = def;
                this.name = name;
                this.designer_name = designer_name;
                this.price = price;
                limit = -1;
            }

            internal Weapon(Weapon w, int limit)
            {
                def = w.def;
                name = w.name;
                designer_name = w.designer_name;
                price = w.price;
                this.limit = limit;
            }
        }

        internal readonly struct PickableResult
        {
            internal readonly int limit;
            internal readonly int count;
            internal readonly bool pickable;

            internal PickableResult(int limit, int count)
            {
                this.limit = limit;
                this.count = count;
                pickable = limit >= count;
            }
        }
    }
}