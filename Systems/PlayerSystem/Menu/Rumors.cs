using System.Collections.Generic;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class RumorsWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiRow introTextRow;
        private readonly NuiRow readGreaterRumorsRow;
        private readonly NuiRow readLesserRumorsRow;
        private readonly NuiRow createRumorsRow;
        private readonly NuiRow deleteRumorsRow;
        private readonly NuiRow returnRow;
        private readonly NuiList listRow;
        private readonly NuiBind<string> npcText = new("npcText");
        private readonly List<int> rumortIds = new();
        private readonly NuiBind<string> titles = new("titles");
        private readonly NuiBind<string> contents = new("contents");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<bool> visible = new("visible");
        private string newRumorTitle { get; set; }
        private int selectedRumorId { get; set; }

        public RumorsWindow(Player player, NwCreature innkeeper) : base(player)
        {
          windowId = "rumors";

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiText(titles)),
            new NuiListTemplateCell(new NuiText(contents)),
            new NuiListTemplateCell(new NuiButton("Modifier") { Id = "modify", Visible = visible, Enabled = visible }),
            new NuiListTemplateCell(new NuiButton("Supprimer") { Id = "delete", Visible = visible, Enabled = visible })
          };

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          introTextRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiImage(innkeeper.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 100 },
              new NuiText(npcText) { Width = 450, Height = 250 }
            }
          };

          readGreaterRumorsRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Quelles sont les dernières grandes rumeurs ?") { Id = "readGreaterRumors", Width = 510 } } };
          readLesserRumorsRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Dites moi tout des derniers potins et on-dits.") { Id = "readLesserRumors", Width = 510 } } };
          createRumorsRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("J'en ai une bien bonne à vous raconter !") { Id = "createRumor", Width = 510 } } };
          deleteRumorsRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Vous vous souvenez de ce que je vous ai raconté ?") { Id = "readMyRumors", Width = 510 } } };
          returnRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Retour.") { Id = "retour", Width = 510 } } };

          listRow = new NuiList(rowTemplate, listCount) { RowHeight = 75 };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();
          rootChidren.Add(introTextRow);
          rootChidren.Add(readGreaterRumorsRow);
          rootChidren.Add(readLesserRumorsRow);
          rootChidren.Add(createRumorsRow);
          rootChidren.Add(deleteRumorsRow);

          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 450);

          window = new NuiWindow(rootGroup, "Ernesto Arna")
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

            npcText.SetBindValue(player.oid, nuiToken.Token, "Bonjour, ami ! Prend une place et cette petite chopine.\n" +
              "A moins que tu n'aies quelque goût pour les dernières histoires du cru ?");

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
                case "readGreaterRumors":
                  LoadDMRumorsList();
                  break;

                case "readLesserRumors":
                  LoadPCRumorsList();
                  break;

                case "createRumor":
                  GetRumorTitle();
                  break;

                case "readMyRumors":
                  LoadMyRumorsList();
                  break;

                case "return":
                  CloseWindow();
                  CreateWindow();
                  break;

                case "modify":
                  selectedRumorId = nuiEvent.ArrayIndex;
                  GetNewRumorTitle();
                  break;

                case "delete":

                  SqLiteUtils.DeletionQuery("rumors",
                  new Dictionary<string, string>() { { "rowid", rumortIds[nuiEvent.ArrayIndex].ToString() } });

                  player.oid.SendServerMessage($"La rumeur {titles.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex].ColorString(ColorConstants.White)} a bien été supprimée", ColorConstants.Pink);

                  CloseWindow();
                  CreateWindow();
                  break;
              }
              break;
          }
        }
        private void LoadDMRumorsList()
        {
          rootChidren.Clear();
          rumortIds.Clear();
          rootChidren.Add(introTextRow);
          rootChidren.Add(listRow);
          rootChidren.Add(returnRow);

          npcText.SetBindValue(player.oid, nuiToken.Token, "Voilà ce qui dit de plus important ces derniers temps.");

          var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT title, content, rowid from rumors r " +
          "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID " +
          "where pa.rank in ('admin', 'staff')");

          List<string> titleList = new List<string>();
          List<string> contentList = new List<string>();

          foreach (var result in query.Results)
          {
            titleList.Add(result.GetString(0));
            contentList.Add(result.GetString(1));
            rumortIds.Add(result.GetInt(2));
          }

          if (player.oid.IsDM)
            visible.SetBindValue(player.oid, nuiToken.Token, true);
          else
            visible.SetBindValue(player.oid, nuiToken.Token, false);

          titles.SetBindValues(player.oid, nuiToken.Token, titleList);
          contents.SetBindValues(player.oid, nuiToken.Token, contentList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void LoadPCRumorsList()
        {
          rootChidren.Clear();
          rumortIds.Clear();
          rootChidren.Add(introTextRow);
          rootChidren.Add(listRow);
          rootChidren.Add(returnRow);

          npcText.SetBindValue(player.oid, nuiToken.Token, "Voici les petits potins du moment.");

          var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT title, content, rowid from rumors r " +
          "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID " +
          "where pa.rank not in ('admin', 'staff')");

          List<string> titleList = new List<string>();
          List<string> contentList = new List<string>();

          foreach (var result in query.Results)
          {
            titleList.Add(result.GetString(0));
            contentList.Add(result.GetString(1));
            rumortIds.Add(result.GetInt(2));
          }

          if (player.oid.IsDM)
            visible.SetBindValue(player.oid, nuiToken.Token, true);
          else
            visible.SetBindValue(player.oid, nuiToken.Token, false);

          titles.SetBindValues(player.oid, nuiToken.Token, titleList);
          contents.SetBindValues(player.oid, nuiToken.Token, contentList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void GetRumorTitle()
        {
          if (!player.windows.ContainsKey("playerInput")) player.windows.Add("playerInput", new PlayerInputWindow(player, "Retirer combien d'unités ?", SetRumorTitle));
          else ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Retirer combien d'unités ?", SetRumorTitle);
        }
        private bool SetRumorTitle(string inputValue)
        {
          if (string.IsNullOrEmpty(inputValue))
          {
            player.oid.SendServerMessage("Un titre est nécessaire pour créer votre rumeur.", ColorConstants.Red);
            return true;
          }

          newRumorTitle = inputValue;

          if (player.windows.ContainsKey("playerInput"))
            ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Quel sera le contenu de votre rumeur ?", SetRumorContent);
          else
            player.windows.Add("playerInput", new PlayerInputWindow(player, "Quel sera le contenu de votre rumeur  ?", SetRumorContent));

          return true;
        }
        private bool SetRumorContent(string inputValue)
        {
          if (string.IsNullOrEmpty(inputValue))
          {
            player.oid.SendServerMessage("Le contenu de votre rumeur ne peut pas être vide.", ColorConstants.Red);
            return true;
          }

          Task waitForTransaction = NwTask.Run(async () =>
          {
            bool queryResult = await SqLiteUtils.InsertQueryAsync("rumors",
              new List<string[]>() {
            new string[] { "accountId", player.accountId.ToString() },
            new string[] { "title", newRumorTitle },
            new string[] { "content", inputValue } },
              new List<string>() { "accountId", "title" },
              new List<string[]>() { new string[] { "content" } });

            player.HandleAsyncQueryFeedback(queryResult, $"Héhé {newRumorTitle.ColorString(ColorConstants.White)}, c'est pas tombé dans l'oreille d'un sourd !", "Erreur technique - Votre rumeur n'as pas été enregistrée.");

            CloseWindow();
            CreateWindow();

            if (!player.oid.IsDM)
              await Bot.staffGeneralChannel.SendMessageAsync($"{Bot.discordServer.EveryoneRole.Mention} Création de la rumeur {newRumorTitle} par {player.oid.LoginCreature.Name} à valider.");
          });

          return true;
        }
        private async void LoadMyRumorsList()
        {
          rootChidren.Clear();
          rumortIds.Clear();
          rootChidren.Add(introTextRow);
          rootChidren.Add(listRow);
          rootChidren.Add(returnRow);

          npcText.SetBindValue(player.oid, nuiToken.Token, "Voici les rumeurs que vous avez laissé courir.");

          var query = await SqLiteUtils.SelectQueryAsync("rumors",
            new List<string>() { { "title" }, { "content" }, { "rowid" } },
            new List<string[]>() { new string[] { "accountId", player.accountId.ToString() } });

          List<string> titleList = new List<string>();
          List<string> contentList = new List<string>();

          if (query != null)
            foreach (var result in query)
            {
              titleList.Add(result[0]);
              contentList.Add(result[1]);
              rumortIds.Add(int.Parse(result[2]));
            }

          visible.SetBindValue(player.oid, nuiToken.Token, true);

          titles.SetBindValues(player.oid, nuiToken.Token, titleList);
          contents.SetBindValues(player.oid, nuiToken.Token, contentList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void GetNewRumorTitle()
        {
          if (!player.windows.ContainsKey("playerInput")) player.windows.Add("playerInput", new PlayerInputWindow(player, "Retirer combien d'unités ?", UpdateRumorTitle, titles.GetBindValues(player.oid, nuiToken.Token)[selectedRumorId]));
          else ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Retirer combien d'unités ?", UpdateRumorTitle, titles.GetBindValues(player.oid, nuiToken.Token)[selectedRumorId]);
        }
        private bool UpdateRumorTitle(string inputValue)
        {
          if (string.IsNullOrEmpty(inputValue))
          {
            player.oid.SendServerMessage("Un titre est nécessaire pour créer votre rumeur.", ColorConstants.Red);
            return true;
          }

          newRumorTitle = inputValue;

          if (!player.windows.ContainsKey("playerInput")) player.windows.Add("playerInput", new PlayerInputWindow(player, "Retirer combien d'unités ?", UpdateRumorContent, titles.GetBindValues(player.oid, nuiToken.Token)[selectedRumorId]));
          else ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Retirer combien d'unités ?", UpdateRumorContent, titles.GetBindValues(player.oid, nuiToken.Token)[selectedRumorId]);

          return true;
        }
        private bool UpdateRumorContent(string inputValue)
        {
          if (string.IsNullOrEmpty(inputValue))
          {
            player.oid.SendServerMessage("Le contenu de votre rumeur ne peut pas être vide.", ColorConstants.Red);
            return true;
          }

          SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "title", newRumorTitle }, new string[] { "content", inputValue } },
          new List<string[]>() { new string[] { "rowid", rumortIds[selectedRumorId].ToString() } });

          player.oid.SendServerMessage($"La rumeur {newRumorTitle.ColorString(ColorConstants.White)} a bien été modifiée");

          CloseWindow();
          CreateWindow();

          Task waitForTransaction = NwTask.Run(async () =>
          {
            if (!player.oid.IsDM)
              await Bot.staffGeneralChannel.SendMessageAsync($"{Bot.discordServer.EveryoneRole.Mention} Modification de la rumeur {newRumorTitle} par {player.oid.LoginCreature.Name} à valider.");
          });

          return true;
        }
      }
    }
  }
}
