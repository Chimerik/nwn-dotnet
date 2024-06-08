using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    public static void HandleWildMagicRage(NwCreature caster)
    {
      switch (NwRandom.Roll(Utils.random, 8))
      {
        case 1: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicRayonDeLumiere, NwTimeSpan.FromRounds(10)); break;
        case 2: SpellSystem.WildMagicVrillesTenebreuses(caster); break;
        case 3: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicEspritIntangible, NwTimeSpan.FromRounds(10)); break;
        case 4: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicRepresailles, NwTimeSpan.FromRounds(10)); break;
        case 5:

          StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Magie Sauvage - Lumières Protectrices", StringUtils.gold, true);
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicLumieresProtectricesAura, NwTimeSpan.FromRounds(10)); 
          break;

        case 6: SpellSystem.WildMagicArmeInfusee(caster); break;
        case 7: caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicCroissanceVegetale, NwTimeSpan.FromRounds(10)); break;
        case 8: 
          
          caster.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 1);
          caster.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").Value = 1;

          break;
      }
    }
  }
}
