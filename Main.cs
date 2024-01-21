using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using static WeaponRestricter.Functions;
using static WeaponRestricter.Types;
using static WeaponRestricter.Utils;

namespace WeaponRestricter
{
    public class WeaponRestrictPlugin : BasePlugin
    {
        public override string ModuleName => "WeaponRestrictPlugin";

        public override string ModuleVersion => "1.0.2";

        public override string ModuleAuthor => "FireBird";

        public override void Load(bool hotReload)
        {
            LoadConfig(ModuleDirectory);
            RegisterEventHandler<EventItemPurchase>(OnEventItemPurchase);
            RegisterEventHandler<EventItemPickup>(OnEventItemPickup);
        }

        private HookResult OnEventItemPurchase(EventItemPurchase e, GameEventInfo info)
        {
            // Weapon is not restricted
            Weapon? wep = FindWeaponByName(e.Weapon, _data_restricted);
            if (wep == null) goto Skip;

            // Weapon is pickable
            PickableResult res = IsPickable(wep, e.Userid.Team);
            if (res.pickable) goto Skip;

            e.Userid.InGameMoneyServices!.Account += wep.price;
            e.Userid.PrintToChat(ChatMessage($"{ChatColors.DarkBlue} Refunded {ChatColors.Lime}${wep.price}"));
        Skip:
            return HookResult.Continue;
        }

        private HookResult OnEventItemPickup(EventItemPickup e, GameEventInfo info)
        {
            // Weapon is not restricted
            Weapon? wep = FindWeaponByIndex(e.Defindex, _data_restricted);
            if (wep == null) goto Skip;

            // Weapon is pickable
            PickableResult res = IsPickable(wep, e.Userid.Team);
            //e.Userid.PrintToChat($"Wep_Limit: {wep.limit} | Res_Limit: {res.limit} | Res_Count: {res.count} | Res_Pickable: {res.pickable}");
            if (res.pickable) goto Skip;

            foreach (var weapon in e.Userid.PlayerPawn.Value!.WeaponServices!.MyWeapons)
            {
                if (weapon.Value!.DesignerName != wep.designer_name) continue;
                // Remove weapon & switch to previous weapon
                weapon.Value.Remove();
                e.Userid.ExecuteClientCommand("lastinv");
                e.Userid.PrintToChat(ChatMessage($"{ChatColors.Red}Weapon {ChatColors.Magenta}{wep.name} {ChatColors.Red}is restricted to {ChatColors.Magenta}{res.limit} {ChatColors.Red}per team."));
            }

        Skip:
            return HookResult.Continue;
        }
    }
}