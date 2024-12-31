using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static async void DelayMirvDamageImpact(NwGameObject oCaster, NwGameObject target, NwSpell spell, SpellEntry spellEntry, NwClass casterClass, double delay, VfxType vfx = VfxType.ImpMagblue, VfxType mirv = VfxType.ImpMirv, int nbDices = 1, bool chromaticBounce = true)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(delay));

      if (target is not null && target.IsValid && oCaster is not null && oCaster.IsValid)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(vfx));
        DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDices, oCaster, spell.GetSpellLevelForClass(casterClass), casterClass: casterClass);

        if(!chromaticBounce)
        {
          oCaster.GetObjectVariable<LocalVariableInt>("_CHROMATIC_ORB_BOUNCE").Delete();
          return;
        }

        if (oCaster is NwCreature caster
          && Utils.In(spell.Id, CustomSpell.OrbeChromatiqueAcide, CustomSpell.OrbeChromatiqueFeu, CustomSpell.OrbeChromatiqueFoudre, CustomSpell.OrbeChromatiqueFoudre, CustomSpell.OrbeChromatiqueFroid, CustomSpell.OrbeChromatiquePoison, CustomSpell.OrbeChromatiqueTonnerre))
        {
          if (oCaster.GetObjectVariable<LocalVariableInt>("_CHROMATIC_ORB_BOUNCE").HasValue)
          {
            int nbDice = GetSpellDamageDiceNumber(oCaster, spell);
            double visualDelay = 0.1;

            foreach (var bounceTarget in target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
            {
              if (bounceTarget == target || bounceTarget == oCaster || !caster.IsReactionTypeHostile(bounceTarget))
                continue;

              switch (GetSpellAttackRoll(bounceTarget, oCaster, spell, casterClass.SpellCastingAbility))
              {
                case TouchAttackResult.CriticalHit: nbDice = GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
                case TouchAttackResult.Hit: break;
                default: continue;
              }

              double targetDistance = target.Distance(bounceTarget);
              double damageDelay = (targetDistance / (3.0 * Math.Log(targetDistance) + 2.0)) + visualDelay;
              DelayMirvDamageImpact(oCaster, bounceTarget, spell, spellEntry, casterClass, damageDelay, vfx, mirv, nbDice, false);
              DelayMirvVisualImpact(target, bounceTarget, visualDelay, mirv);
              visualDelay += 0.1;

              break;
            }
          }
        }
      }
    }
    public static async void DelayMirvVisualImpact(NwGameObject oCaster, NwGameObject target, double delay, VfxType vfx = VfxType.ImpMirv)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(delay));

      if (target is not null && target.IsValid)
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(vfx)));
    }
  }
}
