using NWN.API;
using System;

namespace NWN.Systems
{
    class Language
    {
        public Language(NwPlayer player, int feat)
        {
            if (player.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value == feat)
            {
                player.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value = (int)Feat.Invalid;
                player.SendServerMessage("Vous vous exprimez désormais en commun.");
                //NWScript.SetTextureOverride("icon_elf", "", oidSelf); // TODO : chopper l'icône correspondante dynamiquement via feat.2da
                //RefreshQBS(oidSelf, (int)feat);
            }
            else
            {
                player.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value = feat; 
                player.SendServerMessage($"Vous vous exprimez désormais en {Enum.GetName(typeof(Feat), feat)}.");
                //NWScript.SetTextureOverride("icon_elf", "icon_elf_active", oidSelf
                //RefreshQBS(oidSelf, (int)feat);
            }
        }
    }
}
