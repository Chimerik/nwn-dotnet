using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RadianceDeLaube(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);
      int clercLevel = clerc is null ? 0 : clerc.Level;
      int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Wisdom);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Radiance de L'aube", StringUtils.gold, true, true);

      foreach (var oTarget in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
      {
        if (oTarget is NwCreature target)
        {
          if (target == caster || !caster.IsReactionTypeHostile(target))
            continue;

          int damage = NwRandom.Roll(Utils.random, 10, 2) + clercLevel;

          if (CreatureUtils.GetSavingThrow(caster, target, Ability.Constitution, spellDC) == SavingThrowResult.Success)
            damage /= 2;

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDivineStrikeHoly));
          target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamHoly, caster, BodyNode.Hand), TimeSpan.FromSeconds(1));
          target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Divine));
        }
      }

      foreach (var aoe in caster.Location.GetObjectsInShapeByType<NwAreaOfEffect>(Shape.Sphere, 9, true))
      {
        if (aoe.Spell is not null && aoe.Spell.SpellType == Spell.Darkness)
          aoe.Destroy();
      }

      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
