using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    /*private partial class Potion
    {
      public static async void HandleCoreInflux(NwCreature oCreature, NwItem potion)
      {
        if (!PlayerSystem.Players.TryGetValue(oCreature, out PlayerSystem.Player player))
          return;

        foreach (Effect eff in player.oid.LoginCreature.ActiveEffects)
          if (eff.Tag == "_CORE_EFFECT")
            player.oid.LoginCreature.RemoveEffect(eff);

        await NwTask.Delay(TimeSpan.FromSeconds(0.2));

        int additionalMana = player.GetAdditionalMana();
        int totalMaxMana = potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_MANA").Value + additionalMana;

        player.endurance = new Endurance(potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_HP").Value, potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_MANA").Value,
          player.endurance.currentMana < totalMaxMana ? player.endurance.maxMana : totalMaxMana,
          potion.GetObjectVariable<LocalVariableInt>("_CORE_REMAINING_HP").Value,
          potion.GetObjectVariable<LocalVariableInt>("_CORE_REMAINING_MANA").Value, DateTime.Now.AddSeconds(potion.GetObjectVariable<LocalVariableInt>("_CORE_DURATION").Value));

        player.endurance.additionnalMana = additionalMana;

        int conModifier = player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10;
        player.SetMaxHP();

        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, corePotionEffect, TimeSpan.FromSeconds(potion.GetObjectVariable<LocalVariableInt>("_CORE_DURATION").Value));

        LogUtils.LogMessage($"{player.oid.LoginCreature.Name} absorbe le Mélange : HP endurance {player.endurance.maxHP}, max HP {player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier}, HP régénérable {player.endurance.regenerableHP}, mana régénérable {(int)player.endurance.regenerableMana}, se dissipe le {player.endurance.expirationDate}", LogUtils.LogType.EnduranceSystem);
        player.oid.ExportCharacter();
      }
      public static ScriptHandleResult RemoveCore(CallInfo _)
      {
        EffectRunScriptEvent eventData = new EffectRunScriptEvent();

        if (!PlayerSystem.Players.TryGetValue(eventData.EffectTarget, out PlayerSystem.Player player))
          return ScriptHandleResult.Handled;

        player.endurance.maxHP = 10;
        player.endurance.maxCoreMana = 0;

        if (player.endurance.currentMana > player.endurance.maxMana)
          player.endurance.currentMana = player.endurance.maxMana;

        player.endurance.expirationDate = DateTime.Now;
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));

        int conModifier = ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);
        player.SetMaxHP();

        if (player.oid.LoginCreature.HP > player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier)
          player.oid.LoginCreature.HP = player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier;

        LogUtils.LogMessage($"{player.oid.LoginCreature.Name} perd les effets du Mélange : HP endurance {player.endurance.maxHP}, max HP {player.oid.LoginCreature.LevelInfo[0].HitDie + conModifier}, HP régénérable {player.endurance.regenerableHP}, mana régénérable {(int)player.endurance.regenerableMana}", LogUtils.LogType.EnduranceSystem);

        return ScriptHandleResult.Handled;
      }
    }*/
  }
}
