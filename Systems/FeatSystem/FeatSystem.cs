﻿using NLog;
using NWN.API;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using NWN.API.Constants;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public class FeatSystem
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    
    public FeatSystem(NWNXEventService nwnxEventService)
    {
      
    }
    public static void OnUseFeatBefore(FeatUseEvents.OnUseFeatBefore onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.FeatUser, out PlayerSystem.Player oPC))
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
          new Language(oPC.oid, (int)onUseFeat.Feat);
          break;
        case CustomFeats.BlueprintCopy:
        case CustomFeats.Research:
        case CustomFeats.Metallurgy:

          onUseFeat.Skip = true;
          Craft.Blueprint.BlueprintValidation(oPC.oid, onUseFeat.TargetGameObject, (Feat)onUseFeat.Feat);
          break;

        case CustomFeats.Recycler:
          new Recycler(oPC.oid, onUseFeat.TargetGameObject);
          break;

        case CustomFeats.Renforcement:
          new Renforcement(oPC.oid, onUseFeat.TargetGameObject);
          break;

        case CustomFeats.SurchargeArcanique:
          new SurchargeArcanique(oPC.oid, onUseFeat.TargetGameObject);
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

          onUseFeat.Skip = true;
          oPC.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat));
          break;
        case CustomFeats.WoodProspection:

          onUseFeat.Skip = true;
          Craft.Collect.System.StartCollectCycle(
              oPC,
              oPC.oid.Area,
              () => Craft.Collect.Wood.HandleCompleteProspectionCycle(oPC)
          );
          break;
        case CustomFeats.Hunting:

          onUseFeat.Skip = true;
          Craft.Collect.System.StartCollectCycle(
              oPC,
              oPC.oid.Area,
              () => Craft.Collect.Pelt.HandleCompleteProspectionCycle(oPC)
          );
          break;
      }
    }
  }
}
