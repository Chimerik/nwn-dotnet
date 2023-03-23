using System;
using System.Linq;
using System.Text.Json;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NLog.Targets;

using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    class Potion
    {
      public static void CureMini(NwPlayer oPC)
      {
        foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_MINI"))
          oPC.ControlledCreature.RemoveEffect(arenaMalus);

        oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
      }
      public static void CureFrog(NwPlayer oPC)
      {
        foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_FROG"))
          oPC.ControlledCreature.RemoveEffect(arenaMalus);

        oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
      }
      public static void AlchemyEffect(NwItem potion, NwPlayer oPC, NwGameObject target)
      {
        string[] jsonArray = potion.GetObjectVariable<LocalVariableString>("_SERIALIZED_PROPERTIES").Value.Split("|");

        foreach (string json in jsonArray)
        {
          CustomUnpackedEffect customUnpackedEffect = JsonSerializer.Deserialize<CustomUnpackedEffect>(json);

          if (target == null)
            customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(oPC.ControlledCreature, potion);
          else
            customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(target, potion);
        }
      }
      public static async void CoreInflux(Player player, NwItem potion)
      {
        foreach (Effect eff in player.oid.LoginCreature.ActiveEffects)
          if (eff.Tag == "_CORE_EFFECT")
            player.oid.LoginCreature.RemoveEffect(eff);

        await NwTask.Delay(TimeSpan.FromSeconds(0.2));

        int maxMana = potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_MANA").Value 
          + player.oid.LoginCreature.GetAbilityModifier(Ability.Intelligence) + player.oid.LoginCreature.GetAbilityModifier(Ability.Wisdom) 
          + (player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) * 2);

        player.endurance = new Endurance(potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_HP").Value, maxMana,
          player.endurance.currentMana < maxMana ? player.endurance.maxMana : maxMana, 
          potion.GetObjectVariable<LocalVariableInt>("_CORE_REMAINING_HP").Value,
          potion.GetObjectVariable<LocalVariableInt>("_CORE_REMAINING_MANA").Value, DateTime.Now.AddSeconds(potion.GetObjectVariable<LocalVariableInt>("_CORE_DURATION").Value));

        int improvedHealth = player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = player.learnableSkills.ContainsKey(CustomSkill.Toughness) ? player.learnableSkills[CustomSkill.Toughness].currentLevel : 0;

        int conModifier = ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);
        player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(player.endurance.maxHP
          + improvedHealth * (toughness + conModifier));

        Effect runAction = Effect.RunAction(null, removeCoreHandle);
        runAction = Effect.LinkEffects(runAction, Effect.Icon(EffectIcon.TemporaryHitpoints));
        runAction.Tag = "_CORE_EFFECT";
        runAction.SubType = EffectSubType.Supernatural;

        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, runAction, TimeSpan.FromSeconds(potion.GetObjectVariable<LocalVariableInt>("_CORE_DURATION").Value));
        
        LogUtils.LogMessage($"{player.oid.LoginCreature.Name} absorbe le Mélange : HP endurance {player.endurance.maxHP}, max HP {player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier}, HP régénérable {player.endurance.regenerableHP}, mana régénérable {player.endurance.regenerableMana}, se dissipe le {player.endurance.expirationDate}", LogUtils.LogType.EnduranceSystem);
        player.oid.ExportCharacter();
      }
      public static ScriptHandleResult RemoveCore(CallInfo _)
      {
        EffectRunScriptEvent eventData = new EffectRunScriptEvent();

        if (!Players.TryGetValue(eventData.EffectTarget, out Player player))
          return ScriptHandleResult.Handled;

        player.endurance.maxHP = 10;
        player.endurance.maxMana = player.oid.LoginCreature.GetAbilityModifier(Ability.Intelligence) + player.oid.LoginCreature.GetAbilityModifier(Ability.Wisdom)
          + (player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) * 2);

        if (player.endurance.currentMana > player.endurance.maxMana)
          player.endurance.currentMana = player.endurance.maxMana;

        player.endurance.expirationDate = DateTime.Now;
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));

        int improvedHealth = player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = player.learnableSkills.ContainsKey(CustomSkill.Toughness) ? player.learnableSkills[CustomSkill.Toughness].currentLevel : 0;

        int conModifier = ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);

        player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(player.endurance.maxHP
          + improvedHealth * (toughness + conModifier));

        if (player.oid.LoginCreature.HP > player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier)
          player.oid.LoginCreature.HP = player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier;

        LogUtils.LogMessage($"{player.oid.LoginCreature.Name} perd les effets du Mélange : HP endurance {player.endurance.maxHP}, max HP {player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier}, HP régénérable {player.endurance.regenerableHP}, mana régénérable {player.endurance.regenerableMana}", LogUtils.LogType.EnduranceSystem);

        return ScriptHandleResult.Handled;
      }
    }
  }
}
