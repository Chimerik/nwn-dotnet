using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnDamageHellishRebuke(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature damager)
        return;

      var rebukeEffect = damager.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.HellishRebukeTargetTag && e.Creator == onDamage.Target);

      if (rebukeEffect is null || rebukeEffect.Creator is not NwCreature rebukeSource)
        return;

      await NwTask.NextFrame();

      if (rebukeSource.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value < 1
        || rebukeSource.DistanceSquared(damager) > 324)
        return;

      if (rebukeSource.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
      {
        rebukeSource.LoginPlayer?.SendServerMessage("Vous ne pouvez pas lancer de sort tant que vous êtes équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas.", ColorConstants.Red);
        return;
      }

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.HellishRebuke);

      StringUtils.ForceBroadcastSpellCasting(rebukeSource, spell, damager);

      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(rebukeSource, Ability.Charisma);
      int advantage = GetCreatureAbilityAdvantage(damager, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid);
      int totalSave = SpellUtils.GetSavingThrowRoll(rebukeSource, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(rebukeSource, damager, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      SpellUtils.DealSpellDamage(damager, rebukeSource.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(rebukeSource, spell), rebukeSource, 1);

      rebukeSource.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value -= 1;
      rebukeSource.DecrementRemainingFeatUses((Feat)CustomSkill.HellishRebuke);

      EffectUtils.RemoveTaggedEffect(rebukeSource, EffectSystem.HellishRebukeSourceEffectTag);
      EffectUtils.RemoveTaggedEffect(damager, EffectSystem.HellishRebukeTargetTag, rebukeSource);

      damager.OnCreatureDamage -= OnDamageHellishRebuke;
      
      // TODO : en mode sort, il faudra consommer un emplacement de sort
    }
    /*public static async void OnDamageHellishRebuke(CreatureEvents.OnDamaged onDamage)
    {   
      //if (onDamage.Damager is not NwCreature damager)
        //return;

      NwCreature target = onDamage.Creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_HELLISH_REBUKE_TARGET").Value;

      ModuleSystem.Log.Info($"target : {target.Name} - {target}");
      ModuleSystem.Log.Info($"damager : {onDamage.Damager}");
      ModuleSystem.Log.Info($"damager name : {onDamage.Damager.Name}");
      ModuleSystem.Log.Info($"target != onDamage.Damager {target != onDamage.Damager}");

      if (target != onDamage.Damager)
        return;

      await NwTask.NextFrame();

      if (target is null || !target.IsValid)
      {
        onDamage.Creature.OnDamaged -= OnDamageHellishRebuke;
        onDamage.Creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_HELLISH_REBUKE_TARGET").Delete();
        return;
      }

      ModuleSystem.Log.Info($"onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value < 1 : {onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value < 1}");
      ModuleSystem.Log.Info($"onDamage.Creature.DistanceSquared(target) > 324 : {onDamage.Creature.DistanceSquared(target) > 324}");

      if (onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value < 1
        || onDamage.Creature.DistanceSquared(target) > 324)
        return;

      if (onDamage.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
      {
        onDamage.Creature?.LoginPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort tant que vous êtes équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas.", ColorConstants.Red);
        return;
      }

      ModuleSystem.Log.Info($"spell effect");

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.HellishRebuke);

      StringUtils.ForceBroadcastSpellCasting(onDamage.Creature, spell, target);

      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(onDamage.Creature, Ability.Charisma);
      int advantage = GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid);
      int totalSave = SpellUtils.GetSavingThrowRoll(onDamage.Creature, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Creature, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      SpellUtils.DealSpellDamage(target, onDamage.Creature.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(onDamage.Creature, spell), onDamage.Creature, spell.GetSpellLevelForClass((ClassType)CustomClass.Warlock));

      onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value -= 1;
      onDamage.Creature.DecrementRemainingFeatUses((Feat)CustomSkill.HellishRebuke);

      if (onDamage.Creature.GetFeatRemainingUses((Feat)CustomSkill.HellishRebuke) < 1)
        foreach (var eff in onDamage.Creature.ActiveEffects)
          if (eff.Tag == EffectSystem.HellishRebukeEffectTag)
            onDamage.Creature.RemoveEffect(eff);

      onDamage.Creature.OnDamaged -= OnDamageHellishRebuke;
      onDamage.Creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_HELLISH_REBUKE_TARGET").Delete();

      // TODO : en mode sort, il faudra consommer un emplacement de sort
    }*/
  }
}
