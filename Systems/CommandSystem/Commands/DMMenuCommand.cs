using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDMMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player) && (ctx.oSender.IsDM || ctx.oSender.PlayerName == "Chim"))
      {
        DrawDMCommandList(player);
      }
      else
        ctx.oSender.SendServerMessage("Il faut être connecté en DM pour avoir accès à ce menu.", Color.ORANGE);
    }
    public static void DrawDMCommandList(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici la liste des commandes disponibles en jeu.");
      player.menu.choices.Add(("Afficher le menu d'édition des pnjs.", () => new PNJFactory(player)));
      player.menu.choices.Add(("Obtenir le tag de la zone actuelle.", () => player.oid.SendServerMessage($"Tag de {player.oid.Area.Name} : {player.oid.Area.Tag}")));
      player.menu.choices.Add(("Activer/Désactiver l'écoute globale.", () => new ListenAll(player)));
      player.menu.choices.Add(("Ajouter/Retirer un joueur de la liste d'écoute.", () => new ListenTarget(player.oid)));
      player.menu.choices.Add(("Changer le nom de la cible.", () => new DMRenameTarget(player.oid)));
      player.menu.choices.Add(("Activer/désactiver la persistance des placeables.", () => new PlaceablePersistance(player.oid)));

      if (player.oid.PlayerName == "Chim")
      {
        player.menu.choices.Add(("Activer/désactiver mode DM.", () => new DMMode(player.oid)));
        player.menu.choices.Add(("Reboot du module.", () => new Reboot()));
        player.menu.choices.Add(("Refill des ressources.", () => new Refill()));
        player.menu.choices.Add(("Changer le tag de la cible.", () => new SetTargetTag(player.oid)));
        player.menu.choices.Add(("Donner des ressources à la cible.", () => new GiveResourcesToTarget(player)));
        player.menu.choices.Add(("Créer un skillbook.", () => new CreateSkillbook(player)));
        player.menu.choices.Add(("Instant Learn.", () => new InstantLearn(player.oid)));
        player.menu.choices.Add(("Instant Craft.", () => new InstantCraft(player.oid)));
      }
     
      player.menu.Draw();
    }
  }
}
