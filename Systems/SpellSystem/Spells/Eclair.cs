using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Eclair(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      NwGameObject prevTarget = oCaster;

      foreach (NwCreature target in oCaster.GetNearestCreatures())
      {
        if(target == oCaster) 
          continue;

        if (oCaster.DistanceSquared(target) > 900)
          break;

        foreach(NwCreature cylinderTarget in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCylinder, 30, true, oCaster.Position))
        {
          if(target == cylinderTarget)
          {
            SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass),
              CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));

            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS), TimeSpan.FromSeconds(1));
            NWScript.AssignCommand(prevTarget, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamLightning, prevTarget, BodyNode.Chest)));
            prevTarget = target;
          }
        }
      }
    }
  }
}

