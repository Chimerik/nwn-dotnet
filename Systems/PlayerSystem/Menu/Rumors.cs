using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Microsoft.Data.Sqlite;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class RumorsWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly NuiGroup groupText = new() { Id = "groupText", Border = true, Width = 520, Height = 200 };
        private readonly NuiRow readRow = new();
        private readonly NuiRow writeRow = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> contentText = new("contentText");
        private readonly NuiBind<string> titleText = new("titleText");
        private readonly NuiBind<string> titles = new("titles");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<bool> visible = new("visible");
        private readonly NuiBind<bool> enableSaveRumor = new("enableSaveRumor");

        private readonly List<Rumor> currentRumors = new();
        private Rumor editedRumor;

        public RumorsWindow(Player player) : base(player)
        {
          windowId = "rumors";
          rootColumn.Children = rootChidren;

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButton(titles) { Id = "read", Tooltip = "Racontez moi cette rumeur" }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("ir_charsheet") { Id = "edit", Tooltip = "Attendez, ce n'est pas tout à fait ce qu'il s'est passé !", Visible = visible, Enabled = visible }) { Width = 35},
            new NuiListTemplateCell(new NuiButtonImage("ir_abort") { Id = "delete", Tooltip = "Honnêtement, oubliez ça", Visible = visible, Enabled = visible }) { Width = 35}
          };

          readRow = new NuiRow() { Width = 500, Height = 190, Children = new List<NuiElement>()
          {
              new NuiText(titleText) { Tooltip = titleText, Width = 80, Height = 190 },
              new NuiText(contentText) { Width = 430, Height = 190 }
          } };

          writeRow = new NuiRow() { Width = 500, Height = 200, Children = new List<NuiElement>()
          {
              new NuiTextEdit("Titre de la rumeur", titleText, 100, false) { Tooltip = titleText, Width = 80, Height = 190 },
              new NuiTextEdit("Contenu de la rumeur", contentText, 4000, true) { Width = 430, Height = 190 }
          } };

          groupText.Layout = readRow;
          rootChidren.Add(groupText);

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ir_partychat") { Id = "readGreaterRumors", Tooltip = "Quelles sont les dernières grandes rumeurs ?", Width = 35, Height = 35 },
            new NuiButtonImage("ir_chat") { Id = "readLesserRumors", Tooltip = "Dites moi tout des derniers potins et on-dits.", Width = 35, Height = 35 },
            new NuiButtonImage("ir_command") { Id = "createRumor", Tooltip = "J'en ai une bien bonne à vous raconter !", Width = 35, Height = 35 },
            new NuiButtonImage("ir_dialog") { Id = "readMyRumors", Tooltip = "Vous vous souvenez de ce que je vous ai raconté ?", Width = 35, Height = 35 },
            new NuiButtonImage("ir_learnscroll") { Id = "saveRumor", Tooltip = "Rappelez-vous bien de ça !", Width = 35, Height = 35, Enabled = enableSaveRumor },
            new NuiSpacer()
          } });

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35, Width = 520 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 600);

          window = new NuiWindow(rootColumn, "Aubergiste")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleRumorEvents;

            titleText.SetBindValue(player.oid, nuiToken.Token, "Aubergiste");
            contentText.SetBindValue(player.oid, nuiToken.Token, "Bonjour, ami ! Prend une place et cette petite chopine.\n" +
              "A moins que tu n'aies quelque goût pour les dernières histoires du cru ?");

            enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, false);
            groupText.SetLayout(player.oid, nuiToken.Token, readRow);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleRumorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "readGreaterRumors": LoadDMRumorsList(); break;
                case "readLesserRumors": LoadPCRumorsList(); break;
                case "createRumor": LoadCreateRumorGUI(); break;
                case "readMyRumors": LoadMyRumorsList(); break;
                case "saveRumor": SaveRumor(); break;
                case "read": LoadRumor(currentRumors[nuiEvent.ArrayIndex]); break;
                case "edit": EditRumor(currentRumors[nuiEvent.ArrayIndex]); break;
                case "delete": DeleteRumor(currentRumors[nuiEvent.ArrayIndex]); break;
              }
              break;
          }
        }
        private void LoadDMRumorsList()
        {
          currentRumors.Clear();
          groupText.SetLayout(player.oid, nuiToken.Token, readRow);

          titleText.SetBindValue(player.oid, nuiToken.Token, "Aubergiste");
          contentText.SetBindValue(player.oid, nuiToken.Token, "Voilà ce qui dit de plus important ces derniers temps.");
          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, false);

          List<string> titleList = new List<string>();
          List<bool> visibleList = new List<bool>();

          foreach (Rumor rumor in Utils.rumors)
          {
            if (!rumor.dmCreated)
              continue;

            titleList.Add(rumor.title);
            currentRumors.Add(rumor);

            if (player.oid.IsDM)
              visibleList.Add(true);
            else
              visibleList.Add(false);
          }

          titles.SetBindValues(player.oid, nuiToken.Token, titleList);
          visible.SetBindValues(player.oid, nuiToken.Token, visibleList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void LoadPCRumorsList()
        {
          currentRumors.Clear();
          groupText.SetLayout(player.oid, nuiToken.Token, readRow);

          titleText.SetBindValue(player.oid, nuiToken.Token, "Aubergiste");
          contentText.SetBindValue(player.oid, nuiToken.Token, "Voici les petits potins du moment.");
          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, false);

          List<string> titleList = new List<string>();
          List<bool> visibleList = new List<bool>();

          foreach (Rumor rumor in Utils.rumors)
          {
            if (rumor.dmCreated)
              continue;

            titleList.Add(rumor.title);
            currentRumors.Add(rumor);

            if (player.oid.IsDM || player.characterId == rumor.characterId)
              visibleList.Add(true);
            else
              visibleList.Add(false);
          }

          titles.SetBindValues(player.oid, nuiToken.Token, titleList);
          visible.SetBindValues(player.oid, nuiToken.Token, visibleList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void LoadMyRumorsList()
        {
          currentRumors.Clear();
          groupText.SetLayout(player.oid, nuiToken.Token, readRow);

          titleText.SetBindValue(player.oid, nuiToken.Token, "Aubergiste");
          contentText.SetBindValue(player.oid, nuiToken.Token, "Voici les rumeurs que vous avez laissé courir.");
          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, false);

          List<string> titleList = new List<string>();
          List<bool> visibleList = new List<bool>();

          foreach (Rumor rumor in Utils.rumors)
          {
            if (rumor.characterId != player.characterId)
              continue;

            titleList.Add(rumor.title);
            currentRumors.Add(rumor);
            visibleList.Add(true);
          }

          titles.SetBindValues(player.oid, nuiToken.Token, titleList);
          visible.SetBindValues(player.oid, nuiToken.Token, visibleList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void LoadCreateRumorGUI()
        {
          groupText.SetLayout(player.oid, nuiToken.Token, writeRow);
          titleText.SetBindValue(player.oid, nuiToken.Token, "");
          contentText.SetBindValue(player.oid, nuiToken.Token, "");
          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, true);
        }
        private void SaveRumor()
        {
          string title = titleText.GetBindValue(player.oid, nuiToken.Token);
          string content = contentText.GetBindValue(player.oid, nuiToken.Token);

          if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
          {
            player.oid.SendServerMessage("Votre rumeur doit avoir un titre et un message afin d'être retenue par les locaux !", ColorConstants.Red);
            return;
          }

          int newRumorId = 0;

          try
          {
            newRumorId = Utils.rumors.MaxBy(r => r.id).id + 1;
          }
          catch (Exception) { }

          if(editedRumor != null)
          {
            editedRumor.title = title;
            editedRumor.content = content;
            editedRumor = null;
          }
          else
            Utils.rumors.Insert(0, new Rumor(newRumorId, title, content, player.oid.IsDM, player.characterId, player.oid.LoginCreature.Name));

          player.oid.SendServerMessage($"Votre rumeur {title.ColorString(ColorConstants.White)} a bien été retenue par les locaux !", ColorConstants.Orange);
          Utils.LogMessageToDMs($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) vient d'enregistrer la rumeur {newRumorId} : {title}\n\n{content}");

          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, false);
          LoadMyRumorsList();
          SaveRumorsToDatabase();
        }
        private static async void SaveRumorsToDatabase()
        {
          Task<string> serializedRumors = Task.Run(() => JsonConvert.SerializeObject(Utils.rumors));
          await serializedRumors;

          SqLiteUtils.UpdateQuery("rumors",
          new List<string[]>() { new string[] { "rumors", serializedRumors.Result } },
          new List<string[]>() { new string[] { "rowid", "1" } });
        }
        private void LoadRumor(Rumor rumor)
        {
          groupText.SetLayout(player.oid, nuiToken.Token, readRow);
          titleText.SetBindValue(player.oid, nuiToken.Token, rumor.title);
          contentText.SetBindValue(player.oid, nuiToken.Token, rumor.content);
          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void EditRumor(Rumor rumor)
        {
          groupText.SetLayout(player.oid, nuiToken.Token, writeRow);
          titleText.SetBindValue(player.oid, nuiToken.Token, rumor.title);
          contentText.SetBindValue(player.oid, nuiToken.Token, rumor.content);
          enableSaveRumor.SetBindValue(player.oid, nuiToken.Token, true);
          editedRumor = rumor;
        }
        private void DeleteRumor(Rumor rumor)
        {
          player.oid.SendServerMessage($"Votre rumeur {rumor.title.ColorString(ColorConstants.White)} a bien été oubliée par les locaux", ColorConstants.Orange);
          Utils.LogMessageToDMs($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) vient de supprimer la rumeur {rumor.id} : {rumor.title}\n\n{rumor.content}");
          Utils.rumors.Remove(rumor);
          LoadMyRumorsList();
          SaveRumorsToDatabase();
        }
      }
    }
  }
}
