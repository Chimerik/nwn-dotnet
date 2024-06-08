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
      var eff = onHB.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == searingSmiteBurnEffectTag);

      if (eff is not null)
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.SearingSmite];
        NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
        NwCreature caster = (NwCreature)eff.Creator;

        byte spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
        int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, NwClass.FromClassType(ClassType.Paladin).SpellCastingAbility);
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(onHB.Creature, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster, spellLevel: spellLevel);
        int totalSave = SpellUtils.GetSavingThrowRoll(onHB.Creature, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(caster, onHB.Creature, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed)
          SpellUtils.DealSpellDamage(onHB.Creature, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spellLevel);
        else
        {
          EffectUtils.RemoveTaggedEffect(onHB.Creature, caster, searingSmiteBurnEffectTag);
          onHB.Creature.OnHeartbeat -= OnSearingSmiteBurn;
        }
      }
      else
        onHB.Creature.OnHeartbeat -= OnSearingSmiteBurn;
    }
  }
}
