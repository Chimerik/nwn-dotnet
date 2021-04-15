using NWN.Core;
using NWN.API;

namespace NWN.Systems
{
  class LoadPCDescription
  {
    public LoadPCDescription(PlayerSystem.Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT descriptionName, description from playerDescriptions where characterId = @characterId");

      while (NWScript.SqlStep(query) > 0)
      {
        string descriptionName = $"- {NWScript.SqlGetString(query, 0)}".ColorString(Color.CYAN);
        string description = NWScript.SqlGetString(query, 1);

        player.menu.choices.Add((
          descriptionName,
          () => ApplySelectedDescription(player.oid, descriptionName, description)
        ));
      }

      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }

    private void ApplySelectedDescription(NwPlayer oPC, string descriptionName, string description)
    {
      oPC.Description = description;
      oPC.SendServerMessage($"La description {descriptionName.ColorString(Color.WHITE)} a bien été appliquée à votre personnage.", Color.BLUE);
    }
  }
}
