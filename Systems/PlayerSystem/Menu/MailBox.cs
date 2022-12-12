﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MailBox : PlayerWindow
      {
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<bool> inboxEnabled = new("inboxEnabled");
        private readonly NuiBind<bool> outboxEnabled = new("outboxEnabled");
        private readonly NuiBind<bool> suppressEnabled = new("suppressEnabled"); 
        private readonly NuiBind<bool> receiptSelected = new("receiptSelected");

        private readonly NuiBind<string> title = new("title");
        private readonly NuiBind<string> content = new("content");
        private readonly NuiBind<string> senderName = new("senderName");
        private readonly NuiBind<string> receivedDate = new("receivedDate");
        private readonly NuiBind<Color> readColorBinding = new("readColorBinding");

        private readonly Color readColor = new(255, 255, 255, 125); 
        private readonly Color unreadColor = new(255, 255, 255);

        private readonly NuiBind<List<NuiComboEntry>> comboEntries = new("comboEntries");
        private readonly NuiBind<int> selectedEntry = new("selectedEntry");

        IEnumerable<Mail> filteredList;
        Mail lastReadMail;

        public MailBox(Player player) : base(player)
        {
          windowId = "mailBox";

          rootGroup.Layout = layoutColumn;
          layoutColumn.Children = rootChildren;

          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(senderName) { Id = "read", Tooltip = senderName, ForegroundColor = readColorBinding, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { Width = 120 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(title) { Id = "read", Tooltip = title, ForegroundColor = readColorBinding, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(receivedDate) { Id = "read", Tooltip = receivedDate, ForegroundColor = readColorBinding, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { Width = 110 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "delete", Tooltip = "Supprimer" }) { Width = 35 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          LoadMailBoxLayout();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, 600, 500) : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 600, 500);

          window = new NuiWindow(rootGroup, "Boîte aux lettres")
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
            nuiToken.OnNuiEvent += HandleEditorItemEvents;

            selectedEntry.SetBindValue(player.oid, nuiToken.Token, 0);
            LoadMailBoxBinding();
            inboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
            outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
            SearchMails();
            LoadMessages(filteredList);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorItemEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "inbox":

                  LoadMailBoxLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadMailBoxBinding();
                  inboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  SearchMails();
                  LoadMessages(filteredList);

                  break;

                case "outbox":

                  LoadMailBoxLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadMailBoxBinding();
                  inboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  outboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  SearchMails();
                  LoadMessages(filteredList);

                  break;

                case "write":
                  LoadWriteLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  break;

                case "delete":

                  player.mails.Remove(filteredList.ElementAt(nuiEvent.ArrayIndex));

                  LoadMailBoxLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadMailBoxBinding();
                  inboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  SearchMails();
                  LoadMessages(filteredList);

                  break;

                case "copy":

                  NwItem letter = NwItem.Create("skillbookgeneriq", player.oid.LoginCreature.Location);
                  letter.BaseItem = NwBaseItem.FromItemType(BaseItemType.MiscMedium);
                  letter.Appearance.SetSimpleModel(42);
                  letter.Tag = "missive";
                  letter.Name = $"Missive de {lastReadMail.from}";
                  letter.Description = $"De {lastReadMail.from}\n" +
                  $"A {lastReadMail.to}\n" +
                  $"Le {lastReadMail.sentDate:dd/MM/yyyy}\n\n" +
                  $"{lastReadMail.title}\n\n" +
                  $"{lastReadMail.content}";

                  letter.Clone(player.oid.LoginCreature);
                  letter.Destroy();

                  player.bankGold -= 1;

                  break;

                case "send":

                  int targetId = selectedEntry.GetBindValue(player.oid, nuiToken.Token);
                  string targetName = Utils.mailReceiverEntries.FirstOrDefault(m => m.Value == targetId).Label;
                  string mailTitle = title.GetBindValue(player.oid, nuiToken.Token);
                  string mailContent = content.GetBindValue(player.oid, nuiToken.Token);

                  Mail newMail = new Mail(player.oid.LoginCreature.Name, player.characterId, targetName, targetId, mailTitle, mailContent, DateTime.Now, receiptSelected.GetBindValue(player.oid, nuiToken.Token));
                  player.mails.Add(newMail);

                  if (targetId != player.characterId)
                    newMail.SendMailToPlayer(targetId);

                  LoadMailBoxLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadMailBoxBinding();
                  inboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  SearchMails();
                  LoadMessages(filteredList);

                  player.bankGold -= 1;

                  if (receiptSelected.GetBindValue(player.oid, nuiToken.Token))
                    player.bankGold -= 5;

                  Utils.LogMessageToDMs($"MAIL SYSTEM - {player.oid.LoginCreature.Name} vient d'envoyer une lettre à {targetName}\n" +
                    $"Titre : {mailTitle}\n" +
                    $"Contenu : {mailContent}");

                  break;

                case "unreadFilter":

                  LoadMailBoxLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadMailBoxBinding();
                  inboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  SearchMails();
                  LoadMessages(filteredList.Where(m => !m.read));

                  break;

                case "massDelete":

                  foreach (var mail in filteredList.ToList())
                    player.mails.Remove(mail);

                  LoadMailBoxLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadMailBoxBinding();
                  inboxEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  SearchMails();
                  LoadMessages(filteredList);

                  break;
              }

              break;

            case NuiEventType.MouseUp:

              if(nuiEvent.ElementId == "read")
              {
                Mail clickedMail = filteredList.ElementAt(nuiEvent.ArrayIndex);

                if(!clickedMail.read && clickedMail.receipt)
                  clickedMail.SendReceiptToPlayer();

                clickedMail.read = true;

                LoadReadLayout(clickedMail);
                rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
              }

              break;

            case NuiEventType.Watch:

              switch(nuiEvent.ElementId)
              {
                case "search":
                case "selectedEntry":
                  SearchMails(); 
                  break;
              }

              break;
          }
        }
        private void SearchMails()
        {
          string searchValue = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          string selectedSender = null;

          try { selectedSender = Utils.mailReceiverEntries.ElementAt(selectedEntry.GetBindValue(player.oid, nuiToken.Token)).Label; }
          catch(Exception) { }

          if (inboxEnabled.GetBindValue(player.oid, nuiToken.Token))
          {
            filteredList = !string.IsNullOrEmpty(selectedSender) ? player.mails.Where(m => m.to == selectedSender) : player.mails;
            
            if(!string.IsNullOrEmpty(searchValue))
              filteredList = filteredList.Where(m => m.title.ToLower().Contains(searchValue));
          }
          else
          {
            filteredList = !string.IsNullOrEmpty(selectedSender) ? player.mails.Where(m => m.from == selectedSender) : player.mails;

            if (!string.IsNullOrEmpty(searchValue))
              filteredList = player.mails.Where(m => m.title.ToLower().Contains(searchValue));
          }
        }
        private void LoadButtons()
        {
          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Réception") { Id = "inbox", Enabled = inboxEnabled, Height = 35, Width = 100 },
              new NuiButton("Boite d'envoi") { Id = "outbox", Enabled = outboxEnabled, Height = 35, Width = 100 },
              new NuiButton("Ecrire") { Id = "write", Height = 35, Width = 100 },
              new NuiButton("Non lus") { Id = "unreadFilter", Tooltip = "Filtre par missives non lues", Height = 35, Width = 100 },
              new NuiButton("Suppression") { Id = "massDelete", Tooltip = "Supprime toutes les missives actuellement actuellement affichés", Enabled = suppressEnabled, Height = 35, Width = 100 },
              new NuiSpacer()
            }
          });
        }
        private void StopAllWatchBindings()
        {
          selectedEntry.SetBindWatch(player.oid, nuiToken.Token, false);
          search.SetBindWatch(player.oid, nuiToken.Token, false);
          receiptSelected.SetBindWatch(player.oid, nuiToken.Token, false);
        }
        private void LoadMailBoxLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          lastReadMail = null;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = comboEntries, Selected = selectedEntry, Height = 35, Width = 580 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 20, false) { Height = 35, Width = 580 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35, Width = 580 } } });
        }
        private void LoadMailBoxBinding()
        {
          StopAllWatchBindings();

          selectedEntry.SetBindValue(player.oid, nuiToken.Token, 0);
          search.SetBindValue(player.oid, nuiToken.Token, "");
          suppressEnabled.SetBindValue(player.oid, nuiToken.Token, true);
          selectedEntry.SetBindWatch(player.oid, nuiToken.Token, true);
          search.SetBindWatch(player.oid, nuiToken.Token, true);
        }

        private void LoadMessages(IEnumerable<Mail> displayList)
        {
          List<string> nameList = new();
          List<string> dateList = new();
          List<string> titleList = new();
          List<NuiComboEntry> entries = new();
          List<Color> colorList = new();
          int count = 0;

          entries.Add(new NuiComboEntry("Tous", count));

          foreach (var mail in displayList)
          {
            string receiver;
            string sender;

            if (inboxEnabled.GetBindValue(player.oid, nuiToken.Token))
            {
              receiver = mail.from;
              sender = mail.to;
              mail.read = true;
            }
            else
            {
              receiver = mail.to;
              sender = mail.from;
            }

            if (!nameList.Contains(sender))
            {
              count++;
              entries.Add(new NuiComboEntry(sender, count));
            }

            nameList.Add(sender);
            titleList.Add(mail.title);
            dateList.Add(mail.sentDate.ToString("dd/MM/yyyy HH:mm"));
            colorList.Add(mail.read ? readColor : unreadColor);
          }

          senderName.SetBindValues(player.oid, nuiToken.Token, nameList);
          title.SetBindValues(player.oid, nuiToken.Token, titleList);
          receivedDate.SetBindValues(player.oid, nuiToken.Token, dateList);
          comboEntries.SetBindValue(player.oid, nuiToken.Token, entries);
          readColorBinding.SetBindValues(player.oid, nuiToken.Token, colorList);
          listCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
        private void LoadReadLayout(Mail mail)
        {
          StopAllWatchBindings();
          rootChildren.Clear();
          LoadButtons();
          lastReadMail = mail;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiLabel($"De : {mail.from}, A : {mail.to}, Le : {mail.sentDate:dd/MM/yyyy HH:mm}") { Tooltip = mail.from, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle, Height = 35, Width = 560 }, new NuiSpacer() } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiLabel(mail.title) { Tooltip = mail.title, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle, Height = 35, Width = 560 }, new NuiSpacer() } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiText(mail.content) { Height = 300, Width = 560 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Répondre") { Id = "write", Tooltip = "Coût d'envoi : 1 pièce", Enabled = mail.from != player.oid.LoginCreature.Name && !StringUtils.noReplyArray.Contains(mail.from), Height = 35, Width = 80 },
            new NuiSpacer(),
            new NuiButtonImage("ir_empytqs") { Id = "copy", Tooltip = "Retirer une copie physique, coût 1 pièce", Enabled = lastReadMail.fromCharacterId > 0, Height = 35, Width = 35 },
            new NuiSpacer(),
            new NuiButtonImage("ir_ban") { Id = "delete", Tooltip = "Détruire cette missive", Height = 35, Width = 35 },
            new NuiSpacer()
          } });

          suppressEnabled.SetBindValue(player.oid, nuiToken.Token, false);
          inboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
          outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
        }
        private void LoadWriteLayout()
        {
          StopAllWatchBindings();
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = Utils.mailReceiverEntries, Selected = selectedEntry, Height = 35, Width = 580 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Objet", title, 200, false) { Height = 35, Width = 580 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Contenu", content, 3000, true) { Height = 200, Width = 580 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Envoyer") { Id = "send", Height = 35, Width = 60 }, new NuiCheck("Accusé", receiptSelected) { Tooltip = "Demander un accusé de lecture (coût supplémentaire de 5 pièces)" }, new NuiSpacer() } });

          selectedEntry.SetBindValue(player.oid, nuiToken.Token, -1);
          title.SetBindValue(player.oid, nuiToken.Token, "");
          content.SetBindValue(player.oid, nuiToken.Token, "");
          suppressEnabled.SetBindValue(player.oid, nuiToken.Token, false);

          if (lastReadMail != null)
          {
            selectedEntry.SetBindValue(player.oid, nuiToken.Token, lastReadMail.fromCharacterId);
            title.SetBindValue(player.oid, nuiToken.Token, $"Réponse : {lastReadMail.title}");
          }

          inboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
          outboxEnabled.SetBindValue(player.oid, nuiToken.Token, true);
          selectedEntry.SetBindWatch(player.oid, nuiToken.Token, true);
          receiptSelected.SetBindWatch(player.oid, nuiToken.Token, true);
        }
      }
    }
  }
}
