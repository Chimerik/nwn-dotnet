using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anvil.API;
using System.ComponentModel;
using static NWN.Systems.PlayerSystem;
using Anvil.Services;
using Anvil.API.Events;
using NWN.Systems;

namespace DicePoker
{
  [ServiceBinding(typeof(DicePokerService))]
  public class DicePokerService
  {
    public DicePokerService()
    {
      foreach (NwPlaceable dicePoker in NwObject.FindObjectsWithTag<NwPlaceable>("dice_poker"))
        dicePoker.OnUsed += OnUsedDicePoker;
    }
    private void OnUsedDicePoker(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").HasNothing)
        onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value = 2;

      int availableSlots = onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value;

      if (availableSlots == 0)
      {
        player.oid.SendServerMessage("Aucune place n'est disponible sur ce plateau de dés !");
        return;
      }
      else if (availableSlots == 1)
      {
        onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value = 0;
        onUsed.Placeable.GetObjectVariable<LocalVariableObject<NwCreature>>("_PLAYER_TWO").Value = player.oid.ControlledCreature;
      }
      else if (availableSlots == 2)
      {
        new DicePokerGame(player, onUsed.Placeable);
      }
    }
  }
}
