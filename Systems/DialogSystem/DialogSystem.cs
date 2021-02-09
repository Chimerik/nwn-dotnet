using System;
using System.Collections.Generic;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DialogSystem))]
  public class DialogSystem
  {
    private readonly NativeEventService nativeEventService;
    public DialogSystem(NativeEventService eventService)
    {
      nativeEventService = eventService;
    }

    [ScriptHandler("diag_root")]
    private void HandleDialogStart(CallInfo callInfo)
    {
      uint oPC = NWScript.GetLastSpeaker();

      if (!Convert.ToBoolean(NWScript.GetIsObjectValid(oPC)))
        oPC = NWScript.GetLastUsedBy();

      Player player;
      if (Players.TryGetValue(oPC, out player))
      {
        string tag = NWScript.GetTag(callInfo.ObjectSelf);

        switch (tag)
        {
          case "bank_npc":
            new Bank(player);
            break;
          case "bank_gold":
            new BankGold(player);
            break;
          case "storage_npc":
            new Storage(player);
            break;
          case "intro_mirror":
            new IntroMirror(player);
            break;
          case "refinery":
            new Refinery(player);
            break;
          case "decoupe":
            new Scierie(player);
            break;
          case "tannerie_peau":
            new Tannerie(player);
            break;
          case "bal_system":
            new Messenger(player);
            break;
          case "hventes":
            new HotelDesVentes(player);
            break;
          case "jukebox":
            new Jukebox(player, callInfo.ObjectSelf);
            break;
          case "blacksmith":
            new Blacksmith(player, (NwCreature)callInfo.ObjectSelf);
            break;
          case "woodworker":
            new Woodworker(player, (NwCreature)callInfo.ObjectSelf);
            break;
          case "tanneur":
            new Tanneur(player, (NwCreature)callInfo.ObjectSelf);
            break;
          case "le_bibliothecaire":
            new Bibliothecaire(player, (NwCreature)callInfo.ObjectSelf);
            break;
          case "tribunal_hotesse":
            new TribunalHotesse(player, (NwCreature)callInfo.ObjectSelf);
            break;
        }
      }
    }

    [ScriptHandler("intro_validation")]
    private void HandleIntroValidation(CallInfo callInfo)
    {
      uint oPC = NWScript.GetLastSpeaker();

      if (ObjectPlugin.GetInt(oPC, "_STARTING_SKILL_POINTS") > 0)
        NWScript.SendMessageToPC(oPC, $"Il vous reste encore {ObjectPlugin.GetInt(oPC, "_STARTING_SKILL_POINTS")} points de compétences à dépenser auprès du reflet avant de pouvoir débarquer !");
      else
        NWScript.SetLocalInt(callInfo.ObjectSelf, "_GO", 1);
    }
  }
}
