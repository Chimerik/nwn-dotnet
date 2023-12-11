using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private partial class Potion
    {
      public static void CureFrog(NwPlayer oPC)
      {
        EffectUtils.RemoveTaggedEffect(oPC.ControlledCreature, "CUSTOM_EFFECT_FROG");
        oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
      }
    }
  }
}
