using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Apaisement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

        foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
        {
          if (target.HP < 1 || !CreatureUtils.IsHumanoid(target))
            continue;

          if (caster.Faction.GetMembers().Contains(target))
          {
            EffectUtils.RemoveEffectType(target, EffectType.Frightened, EffectType.Charmed);
            EffectUtils.RemoveTaggedEffect(target, EffectSystem.CharmEffectTag);
          }
          else
          {
            if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry))
            {
              target.ApplyEffect(EffectDuration.Temporary, Effect.Pacified(), NwTimeSpan.FromRounds(spellEntry.duration));
              concentrationList.Add(target);
            }
          }

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
        }
      }

      return concentrationList;
    }
  }
}
