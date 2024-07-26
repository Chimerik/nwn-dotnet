using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PreservationDeLaVie(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is not null)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Préservation de la Vie", StringUtils.gold, true, true);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));
        ClercUtils.ConsumeConduitDivin(caster);

        foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
        {
          if(caster.Faction.GetMembers().Contains(target)) 
          {
            NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(clerc.Level * 3)));
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
          }
        }
      }
    }
  }
}
