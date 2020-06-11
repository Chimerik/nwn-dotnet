using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public const string ON_PC_KEYSTROKE_SCRIPT = "on_pc_keystroke";

    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "on_pc_connect", HandlePlayerConnect },
            { "on_pc_disconnect", HandlePlayerDisconnect },
            { ON_PC_KEYSTROKE_SCRIPT, HandlePlayerKeystroke },
        };

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    private static int HandlePlayerConnect(uint oidSelf)
    {
      var oPC = NWScript.GetEnteringObject();
      NWNX.Events.AddObjectToDispatchList(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      var player = new Player(oPC);
      Players.Add(oPC, player);

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandlePlayerDisconnect(uint oidSelf)
    {
      var oPC = NWScript.GetExitingObject();
      NWNX.Events.RemoveObjectFromDispatchList(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      Players.Remove(oPC);

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandlePlayerKeystroke(uint oidSelf)
    {
      var key = NWNX.Events.GetEventData("KEY");
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.EmitKeydown(new Player.KeydownEventArgs(key));
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
