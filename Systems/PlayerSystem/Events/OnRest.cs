using Anvil.API.Events;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnRest(ModuleEvents.OnPlayerRest onRest)
    {
      if(onRest.RestEventType == RestEventType.Finished)
      {
        byte? warMasterLevel = onRest.Player.LoginCreature.GetClassInfo(NwClass.FromClassId(CustomClass.Warmaster))?.Level;

        if (warMasterLevel.HasValue)
          RestoreManoeuvres(onRest.Player.LoginCreature, warMasterLevel.Value);
      }
    }
    private static async void RestoreManoeuvres(NwCreature creature, byte level)
    {
      byte featUse = (byte)(level > 14 ? 6 : level > 6 ? 5 : 4);
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterAttaquePrecise), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterBalayage), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterRenversement), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterDesarmement), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterDiversion), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterFeinte), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterInstruction), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterJeuDeJambe), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterManoeuvreTactique), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterParade), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterProvocation), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterRalliement), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterRiposte), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterEvaluationTactique), featUse);
    }
  }
}
