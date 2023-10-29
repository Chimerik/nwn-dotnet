using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class DamageUtils
  {
    public static async void HandleHellishRebuke(NwCreature target, NwCreature damager)
    {
      await NwTask.NextFrame();

      if (target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value < 1 
        || !target.ActiveEffects.Any(e => e.Tag == EffectSystem.HellishRebukeEffectTag)
        || target.DistanceSquared(damager) > 324)
        return;

      if(target.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
      {
        target?.LoginPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort tant que vous êtes équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas.", ColorConstants.Red);
        return;
      }

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.HellishRebuke);

      StringUtils.ForceBroadcastSpellCasting(target, spell, damager);

      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(target, spell);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(damager, spellEntry, SpellConfig.SpellEffectType.Invalid);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry, advantage, feedback);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(target, damager, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      SpellUtils.DealSpellDamage(damager, target.LastSpellCasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(target, spell), saveFailed, target);

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
      target.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.HellishRebuke));

      if(target.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.HellishRebuke)) < 1)
        foreach(var eff in target.ActiveEffects)
          if(eff.Tag == EffectSystem.HellishRebukeEffectTag)
            target.RemoveEffect(eff);

      // TODO : en mode sort, il faudra consommer un emplacement de sort
    }
  }
}
