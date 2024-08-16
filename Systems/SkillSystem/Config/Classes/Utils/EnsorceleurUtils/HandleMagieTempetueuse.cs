using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void HandleMagieTempetueuse(NwCreature creature, int spellLevel)
    {
      if (1 < spellLevel || spellLevel > 9)
        return;

      creature.ApplyEffect(EffectDuration.Temporary, EffectSystem.MagieTempetueuse, NwTimeSpan.FromRounds(1));
      creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
    }
  }
}
