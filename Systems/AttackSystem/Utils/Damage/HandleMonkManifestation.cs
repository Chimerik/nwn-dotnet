using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleMonkManifestation(CNWSCreature creature, CNWSItem weapon)
    {
      if(weapon is not null && creature.m_pStats.GetClassLevel(5, 0) > 5)
      {
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ManifestationAmeEffectExoTag);
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ManifestationCorpsEffectExoTag);
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ManifestationEspritEffectExoTag);
      }
    }
  }
}
