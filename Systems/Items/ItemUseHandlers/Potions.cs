using System;
using System.Linq;
using System.Text.Json;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

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
      public static void CoreInflux(Player player, NwItem potion)
      {
        // TODO : Que se passe-t-il si le personnage a déjà un CORE actif quand il prend la potion ?
        // TODO : Que se passe-t-il si le perso reco et qu'il avait un CORE actif mais que celui-ci s'est dissipé pendant la déco ?

        Utils.LogMessageToConsole("removing previous core potion", Config.Env.Chim);

        foreach (Effect eff in player.oid.LoginCreature.ActiveEffects)
          if (eff.Tag == "_CORE_EFFECT")
            player.oid.LoginCreature.RemoveEffect(eff);

        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CORE_MAX_HP").Value = potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_HP").Value;
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CORE_MAX_MANA").Value = potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_MANA").Value;
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CORE_REMAINING_HP").Value = potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_HP").Value;
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CORE_REMAINING_MANA").Value = potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_MANA").Value;

        int improvedHealth = player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = player.learnableSkills.ContainsKey(CustomSkill.Toughness) ? player.learnableSkills[CustomSkill.Toughness].currentLevel : 0;

        player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(potion.GetObjectVariable<PersistentVariableInt>("_CORE_MAX_HP").Value
          + improvedHealth * (toughness + (player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10)));

        Effect runAction = Effect.RunAction(null, removeCoreHandle);
        runAction.Tag = "_CORE_EFFECT";
        runAction.SubType = EffectSubType.Supernatural;
        runAction = Effect.LinkEffects(runAction, Effect.Icon(EffectIcon.ImmunityMind));

        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, runAction, TimeSpan.FromSeconds(potion.GetObjectVariable<LocalVariableInt>("_CORE_DURATION").Value));
        Utils.LogMessageToConsole("Applying core effect", Config.Env.Chim);
      }
      public static ScriptHandleResult RemoveCore(CallInfo _)
      {
        EffectRunScriptEvent eventData = new EffectRunScriptEvent();

        if (!Players.TryGetValue(eventData.EffectTarget, out Player player))
          return ScriptHandleResult.Handled;

        Utils.LogMessageToConsole("removing core effect", Config.Env.Chim);

        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CORE_MAX_HP").Delete();
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CORE_MAX_MANA").Delete();

        int improvedHealth = player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = player.learnableSkills.ContainsKey(CustomSkill.Toughness) ? player.learnableSkills[CustomSkill.Toughness].currentLevel : 0;

        player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
          + improvedHealth * (toughness + ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)));

        return ScriptHandleResult.Handled;
      }
    }
  }
}
