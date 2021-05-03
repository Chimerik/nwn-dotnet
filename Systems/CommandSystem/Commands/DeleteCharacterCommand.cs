using System.Threading.Tasks;
using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class DeleteCharacter
  {
    public DeleteCharacter(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add($"Souhaitez-vous réellement supprimer {player.oid.Name.ColorString(Color.RED)} ?");
      player.menu.titleLines.Add($"Pour confirmer, veuillez écrire par chat le nom de votre personnage.");
      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();

      WaitPlayerInput(player);
    }

    private async void WaitPlayerInput(PlayerSystem.Player player)
    {
      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
        AdminPlugin.DeletePlayerCharacter(player.oid, 1, $"Le personnage {player.oid.Name} a été supprimé.");
      else
      {
        player.oid.SendServerMessage($"Le nom saisit ne correspond pas. Annulation de la suppression.", Color.ORANGE);
        CommandSystem.DrawCommandList(player);
      }
    }
  }
}
