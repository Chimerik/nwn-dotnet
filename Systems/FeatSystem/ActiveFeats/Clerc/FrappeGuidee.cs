﻿using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeGuidee(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1 || !caster.IsPlayerControlled)
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeGuideeVariable).Value = 1;
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Guidée", StringUtils.gold, true, true);
        ClercUtils.ConsumeConduitDivin(caster);
      }
      else
      {
        caster.ControllingPlayer.EnterTargetMode(SelectFrappeGuideeTarget, Config.selectCreatureTargetMode);
        caster.ControllingPlayer.SendServerMessage("Veuillez choisir une cible", ColorConstants.Orange);
      }
    }
    private static void SelectFrappeGuideeTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      NwCreature caster = selection.Player.LoginCreature;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeGuideeVariable).Value = 1;
      StringUtils.DisplayStringToAllPlayersNearTarget(selection.Player.LoginCreature, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Guidée - {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
