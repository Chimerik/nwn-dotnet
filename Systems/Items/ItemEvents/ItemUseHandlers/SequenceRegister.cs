using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static async void HandleCastSequence(OnItemUse onUse)
    {
      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, onUse.UsedBy.ControllingPlayer);
      onUse.PreventUseItem = true;

      if (onUse.TargetObject is null)
      {
        onUse.UsedBy.ControllingPlayer.SendServerMessage("Il faut impérativement sélectionner une cible pour pouvoir lancer la séquence de sorts.", ColorConstants.Red);
        return;
      }
      
      string[] spellList = onUse.Item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_");

      float posX = onUse.UsedBy.Position.X;
      float posY = onUse.UsedBy.Position.Y;

      foreach (string spellId in spellList)
      {
        if (onUse.TargetObject is null || posX != onUse.UsedBy.Position.X || posY != onUse.UsedBy.Position.Y)
          return;

        int castTime = onUse.UsedBy.ActiveEffects.Any(e => e.EffectType == EffectType.Haste) ? 3 : 6;

        await onUse.UsedBy.ActionCastSpellAt((Spell)int.Parse(spellId), onUse.TargetObject);
        await NwTask.Delay(TimeSpan.FromSeconds(castTime));
      }
    }
  }
}
