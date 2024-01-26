# Build
You can either choose to compile the plugin yourself or find the latest build **[here](https://github.com/CS2Plugins/WeaponRestricter/tree/main/bin/Release/net7.0)**

# Config
The config is in JSON format. You can specify rules in a key-value pair style.

The `key` is the weapon name which can be found **[here](https://github.com/CS2Plugins/WeaponRestricter/blob/main/Utils.cs#L30)**
It can either be `weapon_name` or the normal name.  
For Example: `weapon_awp` & `AWP`

The `value` is the number of players present on the server to allow **1** of the weapon for each team.
So, if the `value` is set to **4** for every **4** players **1** weapon will be allowed for each team.
If there are let's say **9** players on the server, **2** weapons will be allowed for each team.

`value` of **-1** completely restricts the weapon.  
`value` of **0** or **1** completely un-restricts the weapon.  

**Why this way? Because JON is a cunt.**

# Example config
```
{
  "weapon_awp": 8
}
```

The above config restricts AWP to `0` until there are at least `8` players on the server. If there are `8` or more players on the server, `1` AWP is allowed for each team until the player count reaches `16` or more, in that case, `2` AWP will be allowed for each team, and so on.

**[Better Version of this](https://github.com/CS2Plugins/WeaponRestrict)**
