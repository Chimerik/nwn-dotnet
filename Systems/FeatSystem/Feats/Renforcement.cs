using Anvil.API;

namespace NWN.Systems
{
  class Renforcement
  {
    public Renforcement(NwPlayer oPC, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
        return;

      if (!(oTarget is NwItem))
      {
        oPC.SendServerMessage($"{oTarget.Name.ColorString(ColorConstants.White)} n'est pas un objet et ne peut donc pas être renforcé.", ColorConstants.Red);
        return;
      }

      player.craftJob.Start(Craft.Job.JobType.Renforcement, player, null, (NwItem)oTarget);
    }
  }
}
