using System.Collections.Generic;
using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void TransmutationStone(OnItemUse onUse)
    {
      if (!Players.TryGetValue(onUse.UsedBy, out Player player))
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
      onUse.PreventUseItem = true;

      int stoneCharacterId = onUse.Item.GetObjectVariable<LocalVariableInt>("_CHARACTER_ID").Value;
      int stoneEffect = onUse.Item.GetObjectVariable<LocalVariableInt>("_TRANSMUTER_STONE_CHOICE").Value;

      if (onUse.UsedBy.ActiveEffects.Any(e => e.Tag == $"{EffectSystem.TransmutationStoneEffectTag}{stoneCharacterId}"))
      {
        if (player.characterId == stoneCharacterId)
        {
          if (!player.windows.TryGetValue("transmutationStoneChoice", out var value)) player.windows.Add("transmutationStoneChoice", new PlayerSystem.Player.TransmutationStoneWindow(player, onUse.Item));
          else ((Player.TransmutationStoneWindow)value).CreateWindow(onUse.Item);
        }
        else
          player.oid.SendServerMessage("Vous bénéficiez déjà de l'effet de cette pierre", ColorConstants.Orange);
      }
      else
      {
        if (player.characterId != stoneCharacterId)
        {
          var stoneCreator = Players.Values.FirstOrDefault(p => p.characterId == stoneCharacterId);
          Guid stoneId = Guid.Empty;

          if (stoneCreator is null)
          {
            var query = SqLiteUtils.SelectQuery("playerCharacters",
                new List<string>() { { "transmutationStone" } },
                new List<string[]>() { { new string[] { "rowid", stoneCharacterId.ToString() } } });

            foreach (var result in query)
              stoneId = Guid.TryParse(result[19], out Guid uuid) ? uuid : Guid.Empty;
          }
          else
            stoneId = stoneCreator.transmutationStone;

          if (stoneId != onUse.Item.UUID)
          {
            player.oid.SendServerMessage("La pierre de transmutation en votre possession a perdu son pouvoir", ColorConstants.Orange);
            onUse.Item.Tag = "inactive_stone";

            foreach (var ip in onUse.Item.ItemProperties)
              onUse.Item.RemoveItemProperty(ip);
          }
          else
            onUse.UsedBy.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetTransmutationStoneEffect(onUse.UsedBy, onUse.Item, stoneEffect));
        }
        else
          onUse.UsedBy.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetTransmutationStoneEffect(onUse.UsedBy, onUse.Item, stoneEffect));
      }
    }
  }
}
