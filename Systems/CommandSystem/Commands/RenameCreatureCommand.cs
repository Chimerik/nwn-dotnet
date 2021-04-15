using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class RenameSummon
  {
    public RenameSummon(NwPlayer oPC)
    {
      if (!oPC.KnowsFeat(Feat.SpellFocusConjuration))
      {
        oPC.SendServerMessage("Le don de spécialisation en invocation est nécessaire pour pouvoir renommer une invocation.", Color.ORANGE);
        return;
      }

      oPC.SendServerMessage("Veuillez sélectionnner la créature dont vous souhaitez modifier le nom.", Color.ROSE);

      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, SummonRenameTarget, ObjectTypes.Creature, MouseCursor.Create);
    }
    private static void SummonRenameTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      if (selection.TargetObject != null && ((NwCreature)selection.TargetObject).Master == selection.Player)
      {
        player.menu.Clear();

        player.menu.titleLines = new List<string>() {
        "Quel nom souhaitez-vous donner à votre invocation ?"
        };

        Task playerInput = NwTask.Run(async () =>
        {
          player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
          player.setString = "";
          await NwTask.WaitUntil(() => player.setString != "");

          player.oid.SendServerMessage($"{selection.TargetObject.Name.ColorString(Color.WHITE)} a été renommé {player.setString.ColorString(Color.WHITE)}.", Color.GREEN);
          player.setString = "";
          player.menu.Close();
        });

        player.menu.choices.Add(("Retour", () => CommandSystem.DrawCommandList(player)));
        player.menu.choices.Add(("Quitter", () => player.menu.Close()));
        player.menu.Draw();
      }
      else
      {
        selection.Player.SendServerMessage("Veuillez sélectionner une cible valide.", Color.RED);
        PlayerSystem.cursorTargetService.EnterTargetMode(selection.Player, SummonRenameTarget, ObjectTypes.Creature, MouseCursor.Create);
      }
    }
  }
}
