using NWN.Core;
using NWN.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  class LoadPCDescription
  {
    public LoadPCDescription(PlayerSystem.Player player)
    {
      player.menu.titleLines = new List<string>() {
        "Voici la liste de vos descriptions ?",
        "Laquelle souhaitez-vous consulter ?"
        };

      var query = SqLiteUtils.SelectQuery("playerDescriptions",
        new List<string>() { { "descriptionName" }, { "description" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      foreach (var result in query.Results)
      {
        string descriptionName = $"- {result.GetString(0)}".ColorString(ColorConstants.Cyan);
        string description = result.GetString(0);

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
      oPC.ControlledCreature.Description = description;
      oPC.SendServerMessage($"La description {descriptionName.ColorString(ColorConstants.White)} a bien été appliquée à votre personnage.", ColorConstants.Blue);
    }
  }
}
