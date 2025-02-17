using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Confusion(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      List<NwGameObject> targets = new();
      int DC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);

      foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
        {
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetConfusionEffect(castingClass.SpellCastingAbility, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpConfusionS));
          targetList.Add(target);
        }
      }

      return targetList;
    }
  }
}
