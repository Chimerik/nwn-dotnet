using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class RenameSummon
  {
    public RenameSummon(NwPlayer oPC)
    {
      if (!oPC.LoginCreature.KnowsFeat(Feat.SpellFocusConjuration))
      {
        oPC.SendServerMessage("Le don de spécialisation en invocation est nécessaire pour pouvoir renommer une invocation.", ColorConstants.Orange);
        return;
      }

      oPC.SendServerMessage("Veuillez sélectionnner la créature dont vous souhaitez modifier le nom.", ColorConstants.Rose);

      oPC.EnterTargetMode(SummonRenameTarget, ObjectTypes.Creature, MouseCursor.Create);
    }
    private static async void SummonRenameTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || !PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player))
        return;

      if (selection.TargetObject != null && ((NwCreature)selection.TargetObject).Master == selection.Player.LoginCreature)
      {
        player.menu.Clear();

        player.menu.titleLines = new List<string>() {
        "Quel nom souhaitez-vous donner à votre invocation ?"
        };

        player.menu.choices.Add(("Retour", () => CommandSystem.DrawCommandList(player)));
        player.menu.choices.Add(("Quitter", () => player.menu.Close()));
        player.menu.Draw();

        bool awaitedValue = await player.WaitForPlayerInputString();

        if (awaitedValue)
        {
          selection.TargetObject.Name = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;
          player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
          player.oid.SendServerMessage($"{selection.TargetObject.Name.ColorString(ColorConstants.White)} a été renommé {selection.TargetObject.Name.ColorString(ColorConstants.White)}.", ColorConstants.Green);
          player.menu.Close();
        }
      }
      else
      {
        selection.Player.SendServerMessage("Veuillez sélectionner une cible valide.", ColorConstants.Red);
        selection.Player.EnterTargetMode(SummonRenameTarget, ObjectTypes.Creature, MouseCursor.Create);
      }
    }
  }
}
