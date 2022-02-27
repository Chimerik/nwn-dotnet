using NLog;
using Anvil.Services;
using Anvil.API;
using Anvil.API.Events;

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

      switch (onUseFeat.Feat.FeatType)
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
          new Language(player.oid, (int)onUseFeat.Feat.FeatType);
          break;

        case CustomFeats.Recycler:

          onUseFeat.PreventFeatUse = true;

          if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value != 0)
          {
            player.oid.SendServerMessage($"Impossible de démarrer un travail artisanal sans pouvoir accéder à un atelier.", ColorConstants.Red);
            return;
          }

          new Recycler(player.oid, onUseFeat.TargetObject);
          break;

        case CustomFeats.SurchargeArcanique:

          onUseFeat.PreventFeatUse = true;

          if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value != 0)
          {
            player.oid.SendServerMessage($"Impossible de démarrer un travail artisanal sans pouvoir accéder à un atelier.", ColorConstants.Red);
            return;
          }

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
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat.FeatType));
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
