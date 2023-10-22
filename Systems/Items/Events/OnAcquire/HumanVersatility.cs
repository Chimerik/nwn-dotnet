using Anvil.API;
using System;
using static Anvil.API.Events.ModuleEvents;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireCheckHumanVersatility(OnAcquireItem onAcquireItem)
    {
      if (!PlayerSystem.Players.TryGetValue(onAcquireItem.AcquiredBy, out PlayerSystem.Player player)
        || !player.learnableSkills.TryGetValue(CustomSkill.HumanVersatility, out LearnableSkill versatility) || versatility.currentLevel < 1)
        return;

      onAcquireItem.Item.GetObjectVariable<LocalVariableString>("_ITEM_WEIGHT_PRE_VERSATILITY").Value = onAcquireItem.Item.Weight.ToString();
      onAcquireItem.Item.Weight = (int)Math.Round(((double)onAcquireItem.Item.Weight * 0.75), MidpointRounding.ToEven);
    }
    public static void OnUnAcquireCheckHumanVersatility(OnUnacquireItem onUnacqItem)
    {
      if (!PlayerSystem.Players.TryGetValue(onUnacqItem.LostBy, out PlayerSystem.Player player)
        || !player.learnableSkills.TryGetValue(CustomSkill.HumanVersatility, out LearnableSkill versatility) || versatility.currentLevel < 1)
        return;

      onUnacqItem.Item.Weight = decimal.Parse(onUnacqItem.Item.GetObjectVariable<LocalVariableString>("_ITEM_WEIGHT_PRE_VERSATILITY").Value);
    }
  }
}
