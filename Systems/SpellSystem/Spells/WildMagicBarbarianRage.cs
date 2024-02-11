using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void HandleWildMagicRage(NwCreature caster)
    {
      switch(NwRandom.Roll(Utils.random, 8))
      {
        case 1: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicRayonDeLumiere, NwTimeSpan.FromRounds(10)); break;
        case 2: WildMagicVrillesTenebreuses(caster); break;
        case 3: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicEspritIntangible, NwTimeSpan.FromRounds(10)); break;
        case 4: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicRepresailles, NwTimeSpan.FromRounds(10)); break;
        case 5: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicLumieresProtectrices, NwTimeSpan.FromRounds(10)); break;
        case 6: WildMagicArmeInfusee(caster); break;
        case 7: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicCroissanceVegetale, NwTimeSpan.FromRounds(10)); break;
        case 8: /*WildMagicTeleportation(caster);*/ break;
      }
    }
  }
}
