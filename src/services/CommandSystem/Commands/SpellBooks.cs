using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  class SpellBooks
  {
    PlayerSystem.Player player;
    public SpellBooks(PlayerSystem.Player player)
    {
      this.player = player;
      DrawGrimoireWelcomePage();
    }
    private void DrawGrimoireWelcomePage()
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Bienvenue dans le système de gestion des grimoires de sorts.");
      player.menu.titleLines.Add("Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Consulter mes grimoires.", () => DrawGrimoireList()));
      player.menu.choices.Add(("Enregistrer mon grimoire actuel.", () => AskGrimoireName()));
      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private async void AskGrimoireName()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quel nom souhaitez-vous donner à ce grimoire ?"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        SaveGrimoire(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private void SaveGrimoire(string grimoireName)
    {
      string spellList = "";

      for (byte i = 0; i < 10; i++)
      {
        foreach (MemorizedSpellSlot spellSlot in player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(i))
        {
          if (!spellSlot.IsPopulated)
            continue;

          spellList += $"{(int)spellSlot.Spell}";
          if (spellSlot.MetaMagic != MetaMagic.None)
            spellList += $"${(int)spellSlot.MetaMagic}";
          spellList += "_";
        }
      }

      if (spellList.Length > 0)
        spellList = spellList.Remove(spellList.Length - 1);

      SqLiteUtils.InsertQuery("playerGrimoire",
          new List<string[]>() {
            new string[] { "characterId", player.characterId.ToString() },
            new string[] { "grimoireName", grimoireName },
            new string[] { "serializedGrimoire", spellList } },
          new List<string>() { "characterId", "grimoireName" },
          new List<string[]>() { new string[] { "serializedGrimoire" } },
          new List<string>() { "characterId", "grimoireName" });

      player.oid.SendServerMessage($"Votre grimoire {grimoireName.ColorString(ColorConstants.White)} a bien été enregistré !", new Color(32, 255, 32));

      DrawGrimoireWelcomePage();
    }
    private void DrawGrimoireList()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Voici la liste de vos grimoires.",
        "Lequel souhaitez-vous consulter ?"
        };

      var result = SqLiteUtils.SelectQuery("playerGrimoire",
        new List<string>() { { "grimoireName" }, { "serializedGrimoire" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      foreach (var grimoire in result.Results)
      {
        string grimoireName = grimoire.GetString(0);
        string serializedGrimoire = grimoire.GetString(1);

        player.menu.choices.Add((
          grimoireName,
          () => HandleSelectedGrimoire(grimoireName, serializedGrimoire)
        ));
      }

      player.menu.choices.Add(("Retour.", () => DrawGrimoireWelcomePage()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void HandleSelectedGrimoire(string grimoireName, string serializedGrimoire)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        $"Vous avez sélectionné le grimoire {grimoireName.ColorString(new Color(32, 255, 32))}",
        "Que souhaitez-vous faire ?"
        };

      player.menu.choices.Add(("Utiliser celui-ci.", () => LoadGrimoire(grimoireName, serializedGrimoire)));
      player.menu.choices.Add(("Y appliquer mon grimoire actuel.", () => SaveGrimoire(grimoireName)));
      player.menu.choices.Add(("Le supprimer.", () => DeleteGrimoire(grimoireName)));
      player.menu.choices.Add(("Retour.", () => DrawGrimoireList()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void DeleteGrimoire(string grimoireName)
    {
      SqLiteUtils.DeletionQuery("playerGrimoire",
         new Dictionary<string, string>() { { "characterId", player.characterId.ToString() }, { "grimoireName", grimoireName } });

      player.oid.SendServerMessage($"Votre grimoire {grimoireName.ColorString(ColorConstants.White)} a bien été supprimé.", new Color(32, 255, 32));
      
      DrawGrimoireList();
    }
    private void LoadGrimoire(string grimoireName, string serializedGrimoire)
    {
      string[] spellList = serializedGrimoire.Split("_");
      int i = 0;
      byte previousSpellLevel = 0;

      foreach (string spell in spellList)
      {
        string[] splitSpell = spell.Split("$");

        Spell spellId = (Spell)int.Parse(splitSpell[0]);
        byte spellLevel = (byte)Spells2da.spellsTable.GetSpellDataEntry(spellId).level;

        if (previousSpellLevel < spellLevel)
          i = 0;

        previousSpellLevel = spellLevel;

        if(player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlotCountByLevel(spellLevel) < i)
          continue;

        player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spellLevel)[i].Spell = spellId;

        if (splitSpell.Length > 1)
          player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spellLevel)[i].MetaMagic = (MetaMagic)int.Parse(splitSpell[1]);
        else
          player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spellLevel)[i].MetaMagic = MetaMagic.None;

        i++;
      }

      player.oid.SendServerMessage($"Votre grimoire {grimoireName.ColorString(ColorConstants.White)} a bien été chargé.", new Color(32, 255, 32));
      player.menu.Close();
    }
  }
}
