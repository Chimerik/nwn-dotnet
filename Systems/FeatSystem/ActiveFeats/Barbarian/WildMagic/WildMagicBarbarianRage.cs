using Anvil.API;
using NWN.Core;

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
        case 7: NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.WildMagicCroissanceVegetaleAura, NwTimeSpan.FromRounds(10))); break;
        case 8:

          caster.LoginPlayer?.DisplayFloatingTextStringOnCreature(caster, "Magie Sauvage - Téléportation".ColorString(StringUtils.gold));
          caster.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 1);
          caster.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").Value = 1;

          break;
      }
    }
  }
}
