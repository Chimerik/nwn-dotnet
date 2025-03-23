using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static async void DelayImplacableRage(NwCreature creature, byte barbarianLevel)
    {
      await NwTask.NextFrame();
      creature.Immortal = false;

      creature.HP = barbarianLevel * 2;
      creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
    }
  }
}
