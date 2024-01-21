using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using static WeaponRestricter.Types;

namespace WeaponRestricter
{
    internal class Functions
    {
        // Get weapon by designer_name or name
        internal static Weapon? FindWeaponByName(string name, List<Weapon> list)
        {
            return list.FirstOrDefault(w => w.designer_name == name) ?? list.FirstOrDefault(w => w.name == name);
        }

        // Get weapon by index
        internal static Weapon? FindWeaponByIndex(long index, List<Weapon> list)
        {
            return list.FirstOrDefault(w => w.def == index);
        }

        // Check if a restricted weapon is pickable
        internal static PickableResult IsPickable(Weapon wep, CsTeam team)
        {
            // Get all the players
            List<CCSPlayerController> _players = Utilities.GetPlayers();

            int count = 0;
            int limit = wep.limit > 0 ? (_players.Count / wep.limit) : 0;

            foreach (CCSPlayerController player in _players)
            {
                // Is not on the team we're checking for
                if (player.Team != team) continue;

                // Get all weapons
                foreach (var weapon in player.PlayerPawn.Value!.WeaponServices!.MyWeapons)
                {
                    if (weapon.Value!.DesignerName != wep.designer_name) continue;
                    // Increment count if weapon is found
                    count++;
                }
            }

            return new(limit, count);
        }
    }
}