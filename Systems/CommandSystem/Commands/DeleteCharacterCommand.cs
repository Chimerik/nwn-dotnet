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

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        if(player.setString == player.oid.Name)
        {
          AdminPlugin.DeletePlayerCharacter(player.oid, 1, $"Le personnage {player.oid.Name} a été supprimé.");
        }
        else
        {
          player.oid.SendServerMessage($"Le nom saisit ne correspond pas. Annulation de la suppression.", Color.ORANGE);
          player.setString = "";
          CommandSystem.DrawCommandList(player);
        }
      });
    }
  }
}
