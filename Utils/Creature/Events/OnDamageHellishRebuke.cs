using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnDamageHellishRebuke(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Damager is not NwCreature damager)
        return;

      await NwTask.NextFrame();

      if (onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value < 1
        || onDamage.Creature.DistanceSquared(damager) > 324)
        return;

      if (onDamage.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
      {
        onDamage.Creature?.LoginPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort tant que vous êtes équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas.", ColorConstants.Red);
        return;
      }

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.HellishRebuke);

      StringUtils.ForceBroadcastSpellCasting(onDamage.Creature, spell, damager);

      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(onDamage.Creature, Ability.Charisma);
      int advantage = GetCreatureAbilityAdvantage(damager, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid);
      int totalSave = SpellUtils.GetSavingThrowRoll(onDamage.Creature, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Creature, damager, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      SpellUtils.DealSpellDamage(damager, onDamage.Creature.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(onDamage.Creature, spell), onDamage.Creature, saveFailed);

      onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value -= 1;
      onDamage.Creature.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.HellishRebuke));

      if (onDamage.Creature.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.HellishRebuke)) < 1)
        foreach (var eff in onDamage.Creature.ActiveEffects)
          if (eff.Tag == EffectSystem.HellishRebukeEffectTag)
            onDamage.Creature.RemoveEffect(eff);

      // TODO : en mode sort, il faudra consommer un emplacement de sort
    }
  }
}
