using System;
using NWN.API;
using NWN.Core;
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

      if (Players.TryGetValue(oPC, out Player player))
      {
        switch (callInfo.ObjectSelf.Tag)
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
            new Jukebox(player, (NwCreature)callInfo.ObjectSelf);
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
          case "pve_arena_host":
            Arena.WelcomeMenu.DrawMainPage(player);
            break;
        }
      }
    }
  }
}
