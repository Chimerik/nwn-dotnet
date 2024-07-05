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
        creature.GetObjectVariable<LocalVariableInt>(BriseurDeHordesVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(PourfendeurDeColosseVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(TueurDeGeantsCoolDownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(HunterVoleeVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(RafaleDuTraqueurVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(EsquiveDuTraqueurVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(AttaqueCoordonneCoolDownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(AttaqueCoordonneeVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(FurieBestialeCoolDownVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(OpportunisteVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(BersekerRepresaillesVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(VoeuHostileVariable).Delete();
        HandleGoadingRoarCooldown(creature);

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

        if(creature.KnowsFeat((Feat)CustomSkill.BelluaireFurieBestiale) 
          && creature.GetObjectVariable<LocalVariableObject<NwCreature>>(AnimalCompanionVariable).HasValue)
          creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 100);

        if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
          continue;

        creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value = 1;
      }

      //if(NwModule.Instance.PlayerCount > 0)
        //LogUtils.LogMessage("Round global - Actions et réactions récupérées", LogUtils.LogType.Combat);
    }
    private static void HandleGoadingRoarCooldown(NwCreature creature)
    {
      if (creature.GetObjectVariable<LocalVariableObject<NwCreature>>(AnimalCompanionVariable).HasNothing)
        return;

      if (creature.KnowsFeat((Feat)CustomSkill.BelluairePatteMielleuse))
        creature.SetFeatRemainingUses((Feat)CustomSkill.BelluairePatteMielleuse, 100);

      if (creature.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauAveuglement))
        creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauAveuglement, 100);

      if (creature.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure))
        creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure, 100);

      if (creature.KnowsFeat((Feat)CustomSkill.BelluaireLoupMorsurePlongeante))
        creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireLoupMorsurePlongeante, 100);

      if (creature.KnowsFeat((Feat)CustomSkill.BelluaireSpiderCocoon))
        creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderCocoon, 100);

      if (creature.KnowsFeat((Feat)CustomSkill.ClercFrappeDivine))
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.ClercFrappeDivine, 1);
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.FrappeDivineEffectTag);
      }

      if (creature.GetObjectVariable<LocalVariableInt>(BelluaireRugissementProvoquantCoolDownVariable).HasValue)
      {
        if(creature.GetObjectVariable<LocalVariableInt>(BelluaireRugissementProvoquantCoolDownVariable).Value < 10)
          creature.GetObjectVariable<LocalVariableInt>(BelluaireRugissementProvoquantCoolDownVariable).Value += 1;
        else
        {
          creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 100);
          creature.GetObjectVariable<LocalVariableInt>(BelluaireRugissementProvoquantCoolDownVariable).Delete();
        }
      }

      if (creature.GetObjectVariable<LocalVariableInt>(BelluaireChargeDuSanglierCoolDownVariable).HasValue)
      {
        if (creature.GetObjectVariable<LocalVariableInt>(BelluaireChargeDuSanglierCoolDownVariable).Value < 10)
          creature.GetObjectVariable<LocalVariableInt>(BelluaireChargeDuSanglierCoolDownVariable).Value += 1;
        else
        {
          creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 100);
          creature.GetObjectVariable<LocalVariableInt>(BelluaireChargeDuSanglierCoolDownVariable).Delete();
        }
      }

      if (creature.GetObjectVariable<LocalVariableInt>(BelluaireSpiderWebCoolDownVariable).HasValue)
      {
        if (creature.GetObjectVariable<LocalVariableInt>(BelluaireSpiderWebCoolDownVariable).Value < 10)
          creature.GetObjectVariable<LocalVariableInt>(BelluaireSpiderWebCoolDownVariable).Value += 1;
        else
        {
          creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderWeb, 100);
          creature.GetObjectVariable<LocalVariableInt>(BelluaireSpiderWebCoolDownVariable).Delete();
        }
      }
    }
  }
}
