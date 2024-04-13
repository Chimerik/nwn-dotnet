using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageHellishRebuke(CreatureEvents.OnDamaged onDamage)
    {
      var oDamager = NWScript.GetLastDamager(onDamage.Creature).ToNwObject<NwObject>();

      if (oDamager is not NwCreature damager)
        return;

      var rebukeEffect = onDamage.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.HellishRebukeEffectTag);

      if (rebukeEffect is null || rebukeEffect.Creator is not NwCreature target || !target.IsValid)
      {
        onDamage.Creature.OnDamaged -= OnDamageHellishRebuke;
        return;
      }

      if (target != damager)
        return;

      if (onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value < 1
        || onDamage.Creature.DistanceSquared(target) > 324)
        return;

      if (onDamage.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
      {
        onDamage.Creature?.LoginPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort tant que vous êtes équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas.", ColorConstants.Red);
        return;
      }

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.HellishRebuke);

      StringUtils.ForceBroadcastSpellCasting(onDamage.Creature, spell, target);

      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(onDamage.Creature, Ability.Charisma);
      int advantage = GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid);
      int totalSave = SpellUtils.GetSavingThrowRoll(onDamage.Creature, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Creature, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      SpellUtils.DealSpellDamage(target, onDamage.Creature.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(onDamage.Creature, spell), onDamage.Creature, 1);

      onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value -= 1;
      onDamage.Creature.DecrementRemainingFeatUses((Feat)CustomSkill.HellishRebuke);
      onDamage.Creature.OnDamaged -= OnDamageHellishRebuke;
      EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.HellishRebukeEffectTag);    

      // TODO : en mode sort, il faudra consommer un emplacement de sort*/
    }
  }
}
