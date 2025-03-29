using System.Linq;
using System.Security.Cryptography;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLeconDesAnciens(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat((Feat)CustomSkill.OeilDeSorciere);

      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FEAT_SELECTION").Value = 2;

      if (!player.windows.TryGetValue("featSelection", out var value)) player.windows.Add("featSelection", new FeatSelectionWindow(player, true));
      else ((FeatSelectionWindow)value).CreateWindow(true);

      return true;
    }
  }
}
