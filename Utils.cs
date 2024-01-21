using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;
using static WeaponRestricter.Types;

namespace WeaponRestricter
{
    internal class Utils
    {
        // Base weapon data
        // Credits to Quake1011 for the data (https://github.com/Quake1011/Weapon-Restrict/blob/main/src/MetaData.cs)
        private static readonly List<Weapon> _data_base = new()
        {
            new((long)ItemDefinition.M4A4, "M4A1", "weapon_m4a1", 3100),
            new((long)ItemDefinition.M4A1_S, "M4A1-S", "weapon_m4a1_silencer", 2900),
            new((long)ItemDefinition.FAMAS, "Famas", "weapon_famas", 2050),
            new((long)ItemDefinition.AUG, "AUG", "weapon_aug", 3300),
            new((long)ItemDefinition.AK_47, "AK-47", "weapon_ak47", 2700),
            new((long)ItemDefinition.GALIL_AR, "Galil", "weapon_galilar", 1800),
            new((long)ItemDefinition.SG_553, "Sg553", "weapon_sg556", 3000),
            new((long)ItemDefinition.SCAR_20, "Scar-20", "weapon_scar20", 5000),
            new((long)ItemDefinition.AWP, "AWP", "weapon_awp", 4750),
            new((long)ItemDefinition.SSG_08, "SSG08", "weapon_ssg08", 1700),
            new((long)ItemDefinition.G3SG1, "G3SG1", "weapon_g3sg1", 5000),
            new((long)ItemDefinition.MP9, "MP9", "weapon_mp9", 1250),
            new((long)ItemDefinition.MP7, "MP7", "weapon_mp7", 1500),
            new((long)ItemDefinition.MP5_SD, "MP5-SD", "weapon_mp5sd", 1500),
            new((long)ItemDefinition.UMP_45, "UMP-45", "weapon_ump45", 1200),
            new((long)ItemDefinition.P90, "P-90", "weapon_p90", 2350),
            new((long)ItemDefinition.PP_BIZON, "PP-19 Bizon", "weapon_bizon", 1400),
            new((long)ItemDefinition.MAC_10, "MAC-10", "weapon_mac10", 1050),
            new((long)ItemDefinition.USP_S, "USP-S", "weapon_usp_silencer", 200),
            new((long)ItemDefinition.P2000, "P2000", "weapon_hkp2000", 200),
            new((long)ItemDefinition.GLOCK_18, "Glock-18", "weapon_glock", 200),
            new((long)ItemDefinition.DUAL_BERETTAS, "Dual berettas", "weapon_elite", 300),
            new((long)ItemDefinition.P250, "P250", "weapon_p250", 300),
            new((long)ItemDefinition.FIVE_SEVEN, "Five-SeveN", "weapon_fiveseven", 500),
            new((long)ItemDefinition.CZ75_AUTO, "CZ75-Auto", "weapon_cz75a", 500),
            new((long)ItemDefinition.TEC_9, "Tec-9", "weapon_tec9", 500),
            new((long)ItemDefinition.R8_REVOLVER, "Revolver R8", "weapon_revolver", 600),
            new((long)ItemDefinition.DESERT_EAGLE, "Desert Eagle", "weapon_deagle", 700),
            new((long)ItemDefinition.NOVA, "Nova", "weapon_nova", 1050),
            new((long)ItemDefinition.XM1014, "XM1014", "weapon_xm1014", 2000),
            new((long)ItemDefinition.MAG_7, "MAG-7", "weapon_mag7", 1300),
            new((long)ItemDefinition.SAWED_OFF, "Sawed-off", "weapon_sawedoff", 1100),
            new((long)ItemDefinition.M249, "M429", "weapon_m249", 5200),
            new((long)ItemDefinition.NEGEV, "Negev", "weapon_negev", 1700),
            new((long)ItemDefinition.ZEUS_X27, "Zeus x27", "weapon_taser", 200),
            new((long)ItemDefinition.HIGH_EXPLOSIVE_GRENADE, "High Explosive Grenade", "weapon_hegrenade", 300),
            new((long)ItemDefinition.MOLOTOV, "Molotov", "weapon_molotov", 400),
            new((long)ItemDefinition.INCENDIARY_GRENADE, "Incendiary Grenade", "weapon_incgrenade", 600),
            new((long)ItemDefinition.SMOKE_GRENADE, "Smoke Grenade", "weapon_smokegrenade", 300),
            new((long)ItemDefinition.FLASHBANG, "Flashbang", "weapon_flashbang", 200),
            new((long)ItemDefinition.DECOY_GRENADE, "Decoy Grenade", "weapon_decoy", 50)
        };

        // Restricted weapon data loaded from cfg
        internal static List<Weapon> _data_restricted = new();

        // Load weapon restricts from config
        internal static void LoadConfig(string moduleDir)
        {
            try
            {
                List<string> failed = new();
                JsonSerializerOptions options = new()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                // Create config file if doesnt already exist
                var configPath = Path.Join(moduleDir, "config.json");
                if (!File.Exists(configPath)) File.Create(configPath);

                // Read the config file and Deserialize it while ignoring comments
                var config = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(configPath), options) ?? throw new Exception("Failed to Read");
                foreach (var weapon in config)
                {
                    string _name = weapon.Key;
                    int _limit = weapon.Value;
                    // Get weapon data by name
                    Weapon? configWeapon = Functions.FindWeaponByName(_name, _data_base);
                    if (configWeapon == null)
                    {
                        // Weapon name was incorrect in config continue to next
                        failed.Add(_name);
                        continue;
                    }

                    // Prevent divide by 0 crashes later
                    if (_limit == 0) _limit = 1;

                    // Add the weapon to the restricted list
                    _data_restricted.Add(new(configWeapon, _limit));
                    Log($"Loaded config for {_name} with value of: {_limit}");
                }
                if (failed.Count == 0) return;

                // Log any incorrect config names/values
                LogSpam("WARN", $"Failed to load {string.Join(", ", failed)} | Weapon name incorrect", 5);
            }
            catch (Exception e)
            {
                LogSpam("ERROR", $"Failed to load Config | Reason: {e.Message}", 5);
            }
        }

        // Get Chat printable message
        internal static string ChatMessage(string message)
        {
            return $"game{ChatColors.Lime}sense {ChatColors.White}» {message}";
        }

        // Regular log
        internal static void Log(string message)
        {
            Console.WriteLine($"WeaponRestricter - {message}");
        }

        // Multiline log so its easier to spot in console
        internal static void LogSpam(string prefix, string message, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(" ");
                Console.WriteLine("#####################################");
                Console.WriteLine($"{prefix}: WeaponRestricter - {message}");
                Console.WriteLine("#####################################");
                Console.WriteLine(" ");
            }
        }
    }
}