using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string searingSmiteAttackEffectTag = "_SEARING_SMITE_ATTACK_EFFECT";
    public static Effect searingSmiteAttack
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = searingSmiteAttackEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.SearingSmite);
        return eff;
      }
    }
    public const string searingSmiteBurnEffectTag = "_SEARING_SMITE_BURN_EFFECT";
    public static Effect searingSmiteBurn
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = searingSmiteBurnEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.SearingSmite);
        return eff;
      }
    }
    public static void OnSearingSmiteBurn(Anvil.API.Events.CreatureEvents.OnHeartbeat onHB)
    {
      if (onHB.Creature.ActiveEffects.Any(e => e.Tag == searingSmiteBurnEffectTag))
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.SearingSmite];
        NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
        NwCreature caster = (NwCreature)onHB.Creature.ActiveEffects.First(e => e.Tag == searingSmiteBurnEffectTag).Creator;

        int spellDC = SpellUtils.GetCasterSpellDC(caster, NwClass.FromClassType(ClassType.Paladin).SpellCastingAbility);
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(onHB.Creature, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid);
        int totalSave = SpellUtils.GetSavingThrowRoll(onHB.Creature, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(caster, onHB.Creature, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed)
          SpellUtils.DealSpellDamage(onHB.Creature, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster);
        else
        {
          foreach (var eff in onHB.Creature.ActiveEffects)
            if (eff.Tag == searingSmiteBurnEffectTag)
              onHB.Creature.RemoveEffect(eff);

          onHB.Creature.OnHeartbeat -= OnSearingSmiteBurn;
        }
      }
      else
        onHB.Creature.OnHeartbeat -= OnSearingSmiteBurn;
    }
  }
}
