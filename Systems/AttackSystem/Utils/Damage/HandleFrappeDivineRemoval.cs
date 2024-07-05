using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static async void HandleFrappeDivineRemoval(CNWSCreature attacker)
    {
      await NwTask.NextFrame();

      if (attacker.m_pStats.HasFeat(CustomSkill.ClercFrappeDivine).ToBool())
        EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.FrappeDivineEffectExoTag);
    }
  }
}
