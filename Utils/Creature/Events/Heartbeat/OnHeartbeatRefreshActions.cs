using System.Linq;
using Anvil.API;
using Anvil.API.Events;

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
            if (Feats2da.featTable[feat.Id].skillCategory == SkillSystem.Category.Manoeuvre)
            {
              var manoeuvre = creature.Feats.FirstOrDefault(f => !f.TalentMaxCR.ToBool() && Feats2da.featTable[f.Id].skillCategory == SkillSystem.Category.Manoeuvre);

              if (manoeuvre != null) 
              {
                creature.SetFeatRemainingUses(feat, creature.GetFeatRemainingUses(manoeuvre));
                creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
                continue;
              }
            }

            creature.SetFeatRemainingUses(feat, (byte)creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Value);
            creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
          }
        }

        if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
          continue;

        creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value = 1;
      }
    }
  }
}
