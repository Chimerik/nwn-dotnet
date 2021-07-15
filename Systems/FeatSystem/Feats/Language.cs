using Anvil.API;
using System;

namespace NWN.Systems
{
  class Language
  {
    public Language(NwPlayer player, int feat)
    {
      if (player.ControlledCreature.GetObjectVariable<LocalVariableInt>("_ACTIVE_LANGUAGE").Value == feat)
      {
        player.ControlledCreature.GetObjectVariable<LocalVariableInt>("_ACTIVE_LANGUAGE").Value = (int)CustomFeats.Invalid;
        player.SendServerMessage("Vous vous exprimez désormais en commun.");
        //NWScript.SetTextureOverride("icon_elf", "", oidSelf); // TODO : chopper l'icône correspondante dynamiquement via feat.2da
        //RefreshQBS(oidSelf, (int)feat);
      }
      else
      {
        player.ControlledCreature.GetObjectVariable<LocalVariableInt>("_ACTIVE_LANGUAGE").Value = feat;
        player.SendServerMessage($"Vous vous exprimez désormais en {Enum.GetName(typeof(CustomFeats), feat)}.");
        //NWScript.SetTextureOverride("icon_elf", "icon_elf_active", oidSelf
        //RefreshQBS(oidSelf, (int)feat);
      }
    }
  }
}
