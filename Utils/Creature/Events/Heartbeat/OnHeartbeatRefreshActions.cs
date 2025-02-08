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
        creature.GetObjectVariable<LocalVariableInt>(HastMasterCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(SneakAttackCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(EmpaleurCooldownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(BriseurDeHordesVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(RafaleDuTraqueurVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(EsquiveDuTraqueurVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(AttaqueCoordonneCoolDownVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(AttaqueCoordonneeVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(EnsoApotheoseVariable).Delete();
        creature.GetObjectVariable<LocalVariableInt>(VoeuHostileVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(OpportunisteVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(BersekerRepresaillesVariable).Delete();
        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(VoeuHostileVariable).Delete();
        HandleGoadingRoarCooldown(creature);

        if (creature.KnowsFeat((Feat)CustomSkill.DruideFrappePrimordialeFroid))
        {
          creature.SetFeatRemainingUses((Feat)CustomSkill.DruideFrappePrimordialeFroid, 1);
          creature.SetFeatRemainingUses((Feat)CustomSkill.DruideFrappePrimordialeFeu, 1);
          creature.SetFeatRemainingUses((Feat)CustomSkill.DruideFrappePrimordialeElec, 1);
          creature.SetFeatRemainingUses((Feat)CustomSkill.DruideFrappePrimordialeTonnerre, 1);
        }
      }

      //if(NwModule.Instance.PlayerCount > 0)
        //LogUtils.LogMessage("Round global - Actions et réactions récupérées", LogUtils.LogType.Combat);
    }
    private static void HandleGoadingRoarCooldown(NwCreature creature)
    {
      if (creature.GetAssociate(AssociateType.AnimalCompanion) is null)
        return;

      if (creature.KnowsFeat((Feat)CustomSkill.BelluaireFurieBestiale))
        creature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 100);

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
    }
  }
}
