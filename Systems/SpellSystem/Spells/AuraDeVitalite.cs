using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> AuraDeVitalite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAuraHoly));

        DelayEffect(oCaster, EffectSystem.AuraDeVitalite(castingClass.SpellCastingAbility, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        DelayEffect(oCaster, EffectSystem.AuraDeVitaliteHeal, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }

      return new List<NwGameObject>() { oCaster };
    }

    public static void AuraDeVitaliteHeal(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.EnterTargetMode(SelectVitaliteHealTarget, Config.CreatureTargetMode(10, new Vector2() { X = 1, Y = 1 }));
    }
    private static void SelectVitaliteHealTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      NwCreature caster = selection.Player.ControlledCreature;

      Effect eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.AuraDeVitaliteEffectTag);

      if (eff is null)
        return;

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.AuraDeVitalite];
      SpellUtils.SignalEventSpellCast(target, caster, (Spell)CustomSpell.AuraDeVitalite, false);

      int healAmount = caster.KnowsFeat((Feat)CustomSkill.ClercGuerisonSupreme) || target.ActiveEffects.Any(e => e.Tag == EffectSystem.LueurDespoirEffectTag)
        ? (spellEntry.damageDice * spellEntry.numDice) + caster.GetAbilityModifier((Ability)eff.IntParams[5])
        : Utils.Roll(spellEntry.damageDice, spellEntry.numDice) + caster.GetAbilityModifier((Ability)eff.IntParams[5]);

      if (caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie))
        healAmount += 5;

      if (caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni) && caster != target)
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(5)));

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));

      EffectUtils.RemoveTaggedEffect(caster, EffectSystem.AuraDeVitaliteHealEffectTag);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 6, CustomSpell.AuraDeVitalite));
    }
  }
}
