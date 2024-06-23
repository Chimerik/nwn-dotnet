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
        player.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage1Variable).Value = random;
        player.SendServerMessage($"Vous sentez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);
      }

      presage = NwFeat.FromFeatId(CustomSkill.DivinationPresage2);

      if (player.LoginCreature.KnowsFeat(presage))
      {
        int random = NwRandom.Roll(Utils.random, 20);

        presage.Name.SetPlayerOverride(player, $"Présage : {random}");
        player.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage2Variable).Value = random;
        player.SendServerMessage($"Vous sentez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);
      }

      presage = NwFeat.FromFeatId(CustomSkill.DivinationPresageSuperieur);

      if (player.LoginCreature.KnowsFeat(presage))
      {
        int random = NwRandom.Roll(Utils.random, 20);

        presage.Name.SetPlayerOverride(player, $"Présage : {random}");
        player.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage3Variable).Value = random;
        player.SendServerMessage($"Vous sentez un {StringUtils.ToWhitecolor(random)} se dessiner dans le futur", ColorConstants.Orange);
      }
    }
  }
}
