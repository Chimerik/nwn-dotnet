

using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RadianceDelAube(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (target == caster || !caster.IsReactionTypeHostile(target))
          continue;

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 1, 
          CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry));

        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSunstrike)));
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamHoly, caster, BodyNode.Hand), TimeSpan.FromSeconds(1.7)));
      }

      foreach (var aoe in caster.Location.GetObjectsInShapeByType<NwAreaOfEffect>(Shape.Sphere, 9, true))
      {
        if (aoe.Spell is not null && aoe.Spell.SpellType == Spell.Darkness)
          aoe.Destroy();
      }

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.ClercRadianceDeLaube);
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
