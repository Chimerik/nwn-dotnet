using System;
using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Enchevetrement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      List<NwGameObject> concentrationTarget = new();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
        TimeSpan duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);

        foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
        {
          if (target == oCaster)
            continue;

          if (CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
            EffectSystem.Entrave(target, caster, casterClass.SpellCastingAbility, duration, true);
        }

        targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.Enchevetrement(caster, spell), duration);
        //targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerDarkness), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        concentrationTarget.Add(UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>());
      }

      return concentrationTarget;
    }
  }
}
