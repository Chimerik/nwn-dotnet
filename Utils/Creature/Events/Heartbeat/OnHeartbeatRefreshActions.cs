﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems.Arena;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnHeartbeatRefreshActions(ModuleEvents.OnHeartbeat onHB)
    {
      foreach (var creature in NwObject.FindObjectsOfType<NwCreature>())
      {
        creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value = 1;

        if (creature.KnowsFeat((Feat)CustomSkill.MainLeste))
          creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value += 1;

        creature.GetObjectVariable<LocalVariableInt>(HastMasterCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(SneakAttackCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(ParadeDeProjectileCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(EmpaleurCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(OpportunisteVariable).Delete();

        if(creature.KnowsFeat((Feat)CustomSkill.BersekerRepresailles))
          creature.GetObjectVariable<LocalVariableInt>(BersekerRepresaillesVariable).Value = 1;

        if (creature.IsPlayerControlled)
        {
          foreach (var feat in creature.Feats.Where(f => f.TalentMaxCR.ToBool() && creature.GetFeatRemainingUses(f) < 1))
          {
            switch(Feats2da.featTable[feat.Id].skillCategory)
            {
              case SkillSystem.Category.Manoeuvre:

                var manoeuvre = creature.Feats.FirstOrDefault(f => !f.TalentMaxCR.ToBool() && Feats2da.featTable[f.Id].skillCategory == SkillSystem.Category.Manoeuvre);

                if (manoeuvre != null)
                {
                  creature.SetFeatRemainingUses(feat, creature.GetFeatRemainingUses(manoeuvre));
                  creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
                  continue;
                }

                break;

              case SkillSystem.Category.Ki:

                var ki = creature.Feats.FirstOrDefault(f => !f.TalentMaxCR.ToBool() && Feats2da.featTable[f.Id].skillCategory == SkillSystem.Category.Ki);

                if (ki != null)
                {
                  creature.SetFeatRemainingUses(feat, creature.GetFeatRemainingUses(ki));
                  creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
                  continue;
                }

                break;
            }

            creature.SetFeatRemainingUses(feat, (byte)creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Value);
            creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
          }
        }

        if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
          continue;

        creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value = 1;
      }

      if(NwModule.Instance.PlayerCount > 0)
        LogUtils.LogMessage($"Round global - Actions et réactions récupérées", LogUtils.LogType.Combat);
    }
  }
}
