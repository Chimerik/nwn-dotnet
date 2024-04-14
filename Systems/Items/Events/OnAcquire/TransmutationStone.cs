using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using static NWN.Systems.PlayerSystem;
using System.Collections.Generic;
using System;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireTransmutationStone(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwItem stone = onAcquireItem.Item;

      if (stone is null || stone.Tag != "PierredeTransmutation" || onAcquireItem.AcquiredBy is not NwCreature creature
        || creature.ActiveEffects.Any(e => e.Tag == $"{EffectSystem.TransmutationStoneEffectTag}{stone.GetObjectVariable<LocalVariableInt>("_CHARACTER_ID").Value}"))
        return;

      int stoneCharacterId = stone.GetObjectVariable<LocalVariableInt>("_CHARACTER_ID").Value;

      var stoneCreator = Players.Values.FirstOrDefault(p => p.characterId == stoneCharacterId);
      Guid stoneId = Guid.Empty;

      if (stoneCreator is null)
      {
        var query = SqLiteUtils.SelectQuery("playerCharacters",
            new List<string>() { { "transmutationStone" } },
            new List<string[]>() { { new string[] { "rowid", stoneCharacterId.ToString() } } });

        foreach (var result in query)
          stoneId = Guid.TryParse(result[0], out Guid uuid) ? uuid : Guid.Empty;
      }
      else
        stoneId = stoneCreator.transmutationStone;

      if (stoneId != stone.UUID)
      {
        creature.LoginPlayer?.SendServerMessage("La pierre de transmutation en votre possession a perdu son pouvoir", ColorConstants.Orange);
        stone.Tag = "inactive_stone";

        foreach (var ip in stone.ItemProperties)
          stone.RemoveItemProperty(ip);
      }
      else if(!creature.ActiveEffects.Any(e => e.Tag == $"{EffectSystem.TransmutationStoneEffectTag}{stoneCharacterId}"))
        creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetTransmutationStoneEffect(creature, stone, stone.GetObjectVariable<LocalVariableInt>("_TRANSMUTER_STONE_CHOICE").Value));
    }
  }
}
