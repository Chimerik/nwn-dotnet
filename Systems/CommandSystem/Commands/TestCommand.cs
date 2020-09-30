using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        int movementRate = NWScript.GetMovementRate(player);
        float movementRateFactor = NWNX.Creature.GetMovementRateFactor(player);

        Console.WriteLine("Movement rate : " + movementRate);
        Console.WriteLine("Movement rate factor : " + movementRateFactor);

        if (movementRate == (int)MovementRate.DMFast)
          movementRate = 1;
        else if(movementRate == 6)
          movementRate += 2;
        else
          movementRate++;

        NWNX.Creature.SetMovementRate(player, (MovementRate)movementRate);
        
        /*if (movementRateFactor >= 1.5f)
          movementRateFactor = 0.0f;
        else
          movementRateFactor += 0.1f;

        NWNX.Creature.SetMovementRateFactor(player, movementRateFactor);*/

        if (NWScript.GetMovementRate(player) == (int)MovementRate.Immobile)
        {
          //NWNX.Creature.SetMovementRate(player, MovementRate.Fast);
          
        }
        else
        {
          //NWNX.Creature.SetMovementRate(player, MovementRate.Immobile);
        }
        /*if (NWNX.Creature.GetMovementRateFactor(player) == 0.0f)
        {
          NWNX.Creature.SetMovementRateFactor(player, 1.0f);

        }
        else
        {
          NWNX.Creature.SetMovementRateFactor(player, 0.0f);
        }*/
      }
    }
  }
}
