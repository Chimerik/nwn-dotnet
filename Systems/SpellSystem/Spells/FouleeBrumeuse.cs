using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FouleeBrumeuse(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwFeat feat)
    {
      if (oCaster is not NwCreature caster)
        return;
   
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);


      if(feat is not null)
      {
        switch(feat.Id)
        {
          case CustomSkill.FouleeRafraichissante:

            caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 10)));

            foreach(NwCreature target in targetLocation.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Reputation(ReputationType.Friend)))
            {
              if (caster.DistanceSquared(target) > 9)
                break;

              target.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 10)));
              StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Foulée Rafraichissante sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true , true);
              break;
            }

            break;

          case CustomSkill.FouleeProvocatrice:

            StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Foulée Provocatrice", StringUtils.gold, true, true);
            int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Charisma);

            foreach (NwCreature target in targetLocation.GetNearestCreatures(CreatureTypeFilter.Alive(true)))
            {
              if (!caster.IsReactionTypeHostile(target))
                continue;

              if (caster.DistanceSquared(target) > 15)
                break;

              if(CreatureUtils.GetSavingThrow(oCaster, target, Ability.Wisdom, spellDC, spellEntry) == SavingThrowResult.Failure)
                EffectSystem.ApplyProvocation(caster, target, NwTimeSpan.FromRounds(1));
            }

            break;

          case CustomSkill.FouleeEvanescente:

            NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, Effect.Invisibility(InvisibilityType.Normal), NwTimeSpan.FromRounds(1)));

            break;

          case CustomSkill.FouleeRedoutable:

            int dc = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Charisma);

            foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 4, false))
            {
              if (caster == target || !caster.IsReactionTypeHostile(target))
                continue;

              if (CreatureUtils.GetSavingThrow(oCaster, target, Ability.Wisdom, dc, spellEntry) == SavingThrowResult.Failure)
              {
                NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 10, 2), CustomDamageType.Psychic)));
                NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS)));
              }
            }

            foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 4, false))
            {
              if (caster == target || !caster.IsReactionTypeHostile(target))
                continue;

              if (CreatureUtils.GetSavingThrow(oCaster, target, Ability.Wisdom, dc, spellEntry) == SavingThrowResult.Failure)
              {
                NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 10, 2), CustomDamageType.Psychic)));
                NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS)));
              }
            }

            break;
        } 
      }

      OccultisteUtils.DecrementFouleeFeerique(caster, feat);

      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      _ = caster.ClearActionQueue();
      _ = caster.ActionJumpToLocation(targetLocation);
      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
    }
  }
}
