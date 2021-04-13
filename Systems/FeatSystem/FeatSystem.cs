using NLog;
using NWN.Services;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public class FeatSystem
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.Creature, out PlayerSystem.Player player))
        return;

      //Log.Info($"{oPC.oid.Name} used feat {onUseFeat.Feat}");

      switch (onUseFeat.Feat)
      {
        case CustomFeats.Elfique:
        case CustomFeats.Abyssal:
        case CustomFeats.Céleste:
        case CustomFeats.Profond:
        case CustomFeats.Draconique:
        case CustomFeats.Druidique:
        case CustomFeats.Nain:
        case CustomFeats.Géant:
        case CustomFeats.Gobelin:
        case CustomFeats.Halfelin:
        case CustomFeats.Infernal:
        case CustomFeats.Orc:
        case CustomFeats.Primordiale:
        case CustomFeats.Sylvain:
        case CustomFeats.Voleur:
        case CustomFeats.Gnome:
          new Language(player.oid, (int)onUseFeat.Feat);
          break;
        case CustomFeats.BlueprintCopy:
        case CustomFeats.Research:
        case CustomFeats.Metallurgy:

          onUseFeat.PreventFeatUse = true;
          Craft.Blueprint.BlueprintValidation(player.oid, onUseFeat.TargetObject, (Feat)onUseFeat.Feat);
          break;

        case CustomFeats.Recycler:
          new Recycler(player.oid, onUseFeat.TargetObject);
          break;

        case CustomFeats.Renforcement:
          new Renforcement(player.oid, onUseFeat.TargetObject);
          break;

        case CustomFeats.SurchargeArcanique:
          new SurchargeArcanique(player.oid, onUseFeat.TargetObject);
          break;

        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:
        case CustomFeats.CustomPositionRight:
        case CustomFeats.CustomPositionLeft:
        case CustomFeats.CustomPositionForward:
        case CustomFeats.CustomPositionBackward:
        case CustomFeats.CustomPositionRotateRight:
        case CustomFeats.CustomPositionRotateLeft:

          onUseFeat.PreventFeatUse = true;
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat));
          break;
        case CustomFeats.WoodProspection:

          onUseFeat.PreventFeatUse = true;
          Craft.Collect.System.StartCollectCycle(
              player,
              () => Craft.Collect.Wood.HandleCompleteProspectionCycle(player)
          );
          break;
        case CustomFeats.Hunting:
          
          onUseFeat.PreventFeatUse = true;
          Craft.Collect.System.StartCollectCycle(
              player,
              () => Craft.Collect.Pelt.HandleCompleteProspectionCycle(player)
          );
          break;
        case CustomFeats.Sit:

          onUseFeat.PreventFeatUse = true;
          new Sit(player);
          break;
      }
    }
  }
}
