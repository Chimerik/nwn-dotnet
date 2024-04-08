using Anvil.API;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void ResetPresage(NwPlayer player)
    {
      NwFeat presage = NwFeat.FromFeatId(CustomSkill.DivinationPresage);

      if (player.LoginCreature.KnowsFeat(presage))
      {
        int random = NwRandom.Roll(Utils.random, 20);

        presage.Name.SetPlayerOverride(player, $"Présage : {random}");
        player.SendServerMessage($"Vous voyez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);
      }

      presage = NwFeat.FromFeatId(CustomSkill.DivinationPresage2);

      if (player.LoginCreature.KnowsFeat(presage))
      {
        int random = NwRandom.Roll(Utils.random, 20);

        presage.Name.SetPlayerOverride(player, $"Présage : {random}");
        player.SendServerMessage($"Vous voyez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);
      }

      presage = NwFeat.FromFeatId(CustomSkill.DivinationPresageSuperieur);

      if (player.LoginCreature.KnowsFeat(presage))
      {
        int random = NwRandom.Roll(Utils.random, 20);

        presage.Name.SetPlayerOverride(player, $"Présage : {random}");
        player.SendServerMessage($"Vous voyez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);
      }
    }
  }
}
