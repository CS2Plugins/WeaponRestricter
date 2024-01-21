using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using static WeaponRestricter.Types;
using static WeaponRestricter.Utils;

namespace WeaponRestricter
{
    public class WeaponRestrictPlugin : BasePlugin
    {
        public override string ModuleName => "WeaponRestrictPlugin";

        public override string ModuleVersion => "1.0.0";

        public override string ModuleAuthor => "FireBird";

        public override void Load(bool hotReload)
        {
            LoadConfig(ModuleDirectory);
            RegisterEventHandler<EventItemPurchase>(OnEventItemPurchase);
            RegisterEventHandler<EventItemPickup>(OnEventItemPickup);
        }

        private HookResult OnEventItemPurchase(EventItemPurchase e, GameEventInfo info)
        {
            Weapon? wep = FindWeaponByName(e.Weapon, _data_restricted);
            if (wep == null) goto Skip;

            PickableResult res = IsPickable(wep, e.Userid.Team);
            if (res.pickable) goto Skip;

            e.Userid.InGameMoneyServices!.Account += wep.price;
            e.Userid.PrintToChat(ChatMessage($"{ChatColors.Purple} Refunded {ChatColors.Lime}${wep.price}"));

        Skip:
            return HookResult.Continue;
        }

        private HookResult OnEventItemPickup(EventItemPickup e, GameEventInfo info)
        {
            Weapon? wep = FindWeaponByIndex(e.Defindex, _data_restricted);
            if (wep == null) return HookResult.Continue;

            PickableResult res = IsPickable(wep, e.Userid.Team);
            if (res.pickable) goto Skip;

            foreach (var weapon in e.Userid.PlayerPawn.Value!.WeaponServices!.MyWeapons)
            {
                // Is not the weapon we're looking for
                if (weapon.Value!.DesignerName != wep.designer_name) continue;
                weapon.Value.Remove();
                e.Userid.ExecuteClientCommand("lastinv");
                e.Userid.PrintToChat(ChatMessage($"{ChatColors.Red}Weapon {ChatColors.Magenta}{wep.name} {ChatColors.Red}is restricted to {ChatColors.Magenta}{res.limit} {ChatColors.Red}per team."));
            }

        Skip:
            return HookResult.Continue;
        }

        private static PickableResult IsPickable(Weapon wep, CsTeam team)
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
                    // Is not the weapon we're looking for
                    if (weapon.Value!.DesignerName != wep.designer_name) continue;
                    count++;
                }
            }

            return new(limit, count);
        }
    }
}