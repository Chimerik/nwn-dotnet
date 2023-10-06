using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private partial class Potion
    {
      public static void CureFrog(NwPlayer oPC)
      {
        foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects)
          if(arenaMalus.Tag == "CUSTOM_EFFECT_FROG")
            oPC.ControlledCreature.RemoveEffect(arenaMalus);

        oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
      }
    }
  }
}
