using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  class CreateSkillbook
  {
    public CreateSkillbook(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string>() {
        "Quel don ?"
        };

      player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      WaitPlayerInput(player);
    }
    private async void WaitPlayerInput(PlayerSystem.Player player)
    {
      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleCreateSkillbook(int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value), player.oid);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
        player.menu.Close();
      }
    }
    private void HandleCreateSkillbook(int skillId, NwPlayer oPC)
    {
      NwItem skillBook = NwItem.Create("skillbookgeneriq", oPC.ControlledCreature.Location);
      skillBook.Tag = "skillbook";
      ItemUtils.CreateShopSkillBook(skillBook, skillId);
      oPC.ControlledCreature.AcquireItem(skillBook);
    }
  }
}
