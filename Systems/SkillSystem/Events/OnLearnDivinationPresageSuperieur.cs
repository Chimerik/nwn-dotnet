﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDivinationPresageSuperieur(PlayerSystem.Player player, int customSkillId)
    {
      NwFeat presage = NwFeat.FromFeatId(CustomSkill.DivinationPresageSuperieur);
      int random = NwRandom.Roll(Utils.random, 20);

      if (!player.oid.LoginCreature.KnowsFeat(presage))
        player.oid.LoginCreature.AddFeat(presage);

      presage.Name.SetPlayerOverride(player.oid, $"Présage : {random}");
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage3Variable).Value = random;
      player.oid.SendServerMessage($"Vous voyez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);

      return true;
    }
  }
}
