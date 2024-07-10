using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static async void HandleFrappeDivineRemoval(CNWSCreature attacker)
    {
      await NwTask.NextFrame();

      if (attacker.m_pStats.HasFeat(CustomSkill.ClercDuperieFrappeDivine).ToBool())
        EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.FrappeDivineDuperieEffectExoTag);
      else if(attacker.m_pStats.HasFeat(CustomSkill.ClercGuerreFrappeDivine).ToBool())
        EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.FrappeDivineGuerreEffectExoTag);
      else if (attacker.m_pStats.HasFeat(CustomSkill.ClercFurieElementaire).ToBool())
        EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.FurieElementaireEffectExoTag);
    }
  }
}
