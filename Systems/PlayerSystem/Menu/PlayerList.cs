using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class PlayerListWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> playerNames = new("playerNames");
        private readonly NuiBind<string> areaNames = new("areaNames");
        private readonly NuiBind<string> bonusRoleplay = new("bonusRoleplay");
        private readonly NuiBind<string> bonusRoleplayDown = new("bonusRoleplayDown");
        private readonly NuiBind<bool> partyInviteEnabled = new("partyInviteEnabled");
        private readonly NuiBind<string> muteIcon = new("muteIcon");
        private readonly NuiBind<string> muteTooltip = new("muteTooltip");
        private readonly NuiBind<string> globalMuteIcon = new("globalMuteIcon");
        private readonly NuiBind<string> globalMuteTooltip = new("globalMuteTooltip");
        private readonly NuiBind<bool> muteEnabled = new("muteEnabled");
        private readonly NuiBind<string> listenIcon = new("listenIcon");
        private readonly NuiBind<string> listenTooltip = new("listenTooltip");
        private readonly NuiBind<string> globalListenIcon = new("globalListenIcon");
        private readonly NuiBind<string> globalListenTooltip = new("globalListenTooltip");
        private readonly NuiBind<string> hostileIcon = new("hostileIcon");
        private readonly NuiBind<string> hostileTooltip = new("hostileTooltip");
        private readonly NuiBind<int> listCount = new("listCount");

        public IEnumerable<NwPlayer> myPlayerList;
        private NwPlayer selectedPlayer;

        public PlayerListWindow(Player player) : base(player)
        {
          windowId = "playerList";
          
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(playerNames) { Tooltip = areaNames, Height = 35, VerticalAlign = NuiVAlign.Middle }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_dialog") { Id = "mp", Tooltip = "Ouvrir un canal privé", Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_invite") { Id = "party", Tooltip = "Inviter à rejoindre le groupe", Enabled = partyInviteEnabled, Height = 35 }) { Width = 35 });
          
          if(!player.oid.IsDM && player.oid.PlayerName != "Chim")
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(muteIcon) { Id = "mute", Tooltip = muteTooltip, Enabled = muteEnabled, Height = 35 }) { Width = 35 });

          if (player.oid.IsDM || player.oid.PlayerName == "Chim" || player.bonusRolePlay > 3)
            rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ief_damincr") { Id = "commend", Tooltip = bonusRoleplay, Height = 35 }) { Width = 35 });

          if (player.oid.IsDM || player.oid.PlayerName == "Chim")
          {
            rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ief_damdecr") { Id = "downgrade", Tooltip = bonusRoleplayDown, Height = 35 }) { Width = 35 });
            rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(listenIcon) { Id = "listen", Tooltip = listenTooltip, Enabled = muteEnabled, Height = 35 }) { Width = 35 });
            rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_more_sp_ab") { Id = "tp", Tooltip = "Se téléporter auprès de ce joueur", Height = 35 }) { Width = 35 });
            rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_boot") { Id = "kick", Tooltip = "Exclure ce joueur du module", Enabled = muteEnabled, Height = 35 }) { Width = 35 });
          }

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(hostileIcon) { Id = "hostile", Tooltip = hostileTooltip, Enabled = muteEnabled, Height = 35 }) { Width = 35 });

          rootRow.Children = rootChildren;
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiSpacer(),
            new NuiButtonImage(globalMuteIcon) { Id = "globalMute", Tooltip = globalMuteTooltip, Height = 35, Width = 35 },
            new NuiSpacer(),
            new NuiButtonImage("ief_arcanefail") { Id = "areaHostile", Tooltip = "Active l'hostilité sur tous les joueurs de la zone qui ne font pas partie de votre groupe", Height = 35, Width = 35 },
            new NuiSpacer(),
            new NuiButtonImage(globalListenIcon) { Id = "globalListen", Tooltip = globalListenTooltip, Enabled = player.oid.IsDM || player.oid.PlayerName == "Chim", Height = 35, Width = 35 },
            new NuiSpacer()
          } });

          CreateWindow();
        }
        public void CreateWindow()
        {

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 400, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 4);

          window = new NuiWindow(rootRow, "Liste des joueurs")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };          

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandlePlayerListEvents;

            if (player.mutedList.Contains(0))
            {
              globalMuteIcon.SetBindValue(player.oid, nuiToken.Token, "ief_darkness");
              globalMuteTooltip.SetBindValue(player.oid, nuiToken.Token, "Réactiver la réception globale de MPs. Ceux que vous avez sélectionné individuellement resteront cependant bloqués.");
            }
            else
            {
              globalMuteIcon.SetBindValue(player.oid, nuiToken.Token, "ief_darkvis");
              globalMuteTooltip.SetBindValue(player.oid, nuiToken.Token, "Bloquer la réception globale de MPs afin de vous concentrer sur votre rp. Vous recevrez cependant toujours les MPs des DMs");
            }

            if (player.listened.Count > 0)
            {
              globalListenIcon.SetBindValue(player.oid, nuiToken.Token, "ief_blind");
              globalListenTooltip.SetBindValue(player.oid, nuiToken.Token, "Désactiver l'écoute de tous les joueurs.");
            }
            else
            {
              globalListenIcon.SetBindValue(player.oid, nuiToken.Token, "ief_concealed");
              globalListenTooltip.SetBindValue(player.oid, nuiToken.Token, "Activer l'écoute de tous les joueurs.");
            }

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            UpdatePlayerList();
          }
        }

        private void HandlePlayerListEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "mp":

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);

                  if (player.windows.ContainsKey(selectedPlayer.PlayerName))
                    ((PrivateMessageWindow)player.windows[selectedPlayer.PlayerName]).CreateWindow();
                  else
                    player.windows.Add(selectedPlayer.PlayerName, new PrivateMessageWindow(player, selectedPlayer));

                  break;

                case "commend":

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);

                  if (!Players.TryGetValue(selectedPlayer.LoginCreature, out Player commendedPlayer))
                    return;

                  if (player.oid.IsDM || player.oid.PlayerName == "Chim")
                  {
                    if (commendedPlayer.bonusRolePlay > 3)
                      return;

                    commendedPlayer.bonusRolePlay += 1;

                    List<string> updatedList = bonusRoleplay.GetBindValues(player.oid, nuiToken.Token);
                    updatedList[nuiEvent.ArrayIndex] = commendedPlayer.bonusRolePlay < 4 ? $"Augmenter le bonus roleplay au niveau {commendedPlayer.bonusRolePlay + 1}" : "Bonus roleplay niveau max (4)";
                    bonusRoleplay.SetBindValues(player.oid, nuiToken.Token, updatedList);

                    selectedPlayer.SendServerMessage($"Votre bonus roleplay est désormais de {commendedPlayer.bonusRolePlay}", new Color(32, 255, 32));

                    SaveBRPToDatabase(commendedPlayer);
                  }
                  else
                  {
                    if (commendedPlayer.bonusRolePlay < 4)
                    {
                      selectedPlayer.SendServerMessage("Vous venez d'obtenir une recommandation pour une augmentation de bonus roleplay !", ColorConstants.Rose);

                      if (commendedPlayer.bonusRolePlay == 1)
                      {
                        commendedPlayer.bonusRolePlay = 2;
                        selectedPlayer.SendServerMessage("Votre bonus roleplay est désormais de 2", new Color(32, 255, 32));
                        SaveBRPToDatabase(commendedPlayer);
                      }
                    }

                    Utils.LogMessageToDMs($"{selectedPlayer.LoginCreature.Name} vient de recommander {selectedPlayer.LoginCreature.Name} pour une augmentation de bonus roleplay.");

                    player.oid.SendServerMessage($"Vous venez de recommander {selectedPlayer.LoginCreature.Name.ColorString(ColorConstants.White)} pour une augmentation de bonus roleplay !", ColorConstants.Rose);
                  }

                  break;

                case "downgrade":

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);

                  if (!Players.TryGetValue(selectedPlayer.LoginCreature, out Player downgradedPlayer))
                    return;

                  if (player.oid.IsDM || player.oid.PlayerName == "Chim")
                  {
                    if (downgradedPlayer.bonusRolePlay < 1)
                      return;

                    downgradedPlayer.bonusRolePlay -= 1;

                    List<string> updatedList = bonusRoleplayDown.GetBindValues(player.oid, nuiToken.Token);
                    updatedList[nuiEvent.ArrayIndex] = downgradedPlayer.bonusRolePlay > 0 ? $"Diminuer le bonus roleplay au niveau {downgradedPlayer.bonusRolePlay - 1}" : "Bonus roleplay niveau min (0)";
                    bonusRoleplay.SetBindValues(player.oid, nuiToken.Token, updatedList);

                    selectedPlayer.SendServerMessage($"Votre bonus roleplay est désormais de {downgradedPlayer.bonusRolePlay}", new Color(32, 255, 32));

                    SaveBRPToDatabase(downgradedPlayer);
                  }

                  break;

                case "mute":

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);

                  if (!Players.TryGetValue(selectedPlayer.LoginCreature, out Player mutedPlayer))
                    return;

                  List<string> updatedMutedIconList = muteIcon.GetBindValues(player.oid, nuiToken.Token);
                  List<string> updatedMutedTooltipList = muteTooltip.GetBindValues(player.oid, nuiToken.Token);

                  if (!player.mutedList.Contains(mutedPlayer.accountId))
                  {
                    player.mutedList.Add(mutedPlayer.accountId);
                    player.oid.SendServerMessage($"Vous bloquez désormais tous les mps de {selectedPlayer.PlayerName.ColorString(ColorConstants.White)}.", ColorConstants.Blue);

                    updatedMutedIconList[nuiEvent.ArrayIndex] = "ief_forcewalk";
                    updatedMutedTooltipList[nuiEvent.ArrayIndex] = "Autoriser les MPs de ce joueur";
                  }
                  else
                  {
                    player.mutedList.Remove(mutedPlayer.accountId);
                    player.oid.SendServerMessage($"Vous ne bloquez plus les mps de {selectedPlayer.PlayerName.ColorString(ColorConstants.White)}", ColorConstants.Blue);

                    updatedMutedIconList[nuiEvent.ArrayIndex] = "ief_haste";
                    updatedMutedTooltipList[nuiEvent.ArrayIndex] = "Bloquer les MPs de ce joueur";
                  }

                  muteIcon.SetBindValues(player.oid, nuiToken.Token, updatedMutedIconList);
                  muteTooltip.SetBindValues(player.oid, nuiToken.Token, updatedMutedTooltipList);
                  SaveMutedPlayersToDatabase(player);

                  break;

                case "globalMute":

                  if (!player.mutedList.Contains(0))
                  {
                    player.mutedList.Add(0);
                    player.oid.SendServerMessage("Vous bloquez désormais la réception globale des mps. Vous recevrez cependant toujours ceux des DMs.", ColorConstants.Blue);
                    globalMuteIcon.SetBindValue(player.oid, nuiToken.Token, "ief_darkness");
                    globalMuteTooltip.SetBindValue(player.oid, nuiToken.Token, "Réactiver la réception globale de MPs. Ceux que vous avez sélectionné individuellement resteront cependant bloqués.");
                  }
                  else
                  {
                    player.mutedList.Remove(0);
                    player.oid.SendServerMessage("Vous réactivez désormais la réception globale des mps. Vous ne recevrez cependant pas ceux que vous bloquez individuellement.", ColorConstants.Blue);
                    globalMuteIcon.SetBindValue(player.oid, nuiToken.Token, "ief_darkness");
                    globalMuteTooltip.SetBindValue(player.oid, nuiToken.Token, "Réactiver la réception globale de MPs. Ceux que vous avez sélectionné individuellement resteront cependant bloqués.");
                  }

                  SaveMutedPlayersToDatabase(player);

                  break;

                case "listen":

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);

                  if (!Players.TryGetValue(selectedPlayer.LoginCreature, out Player listenPlayer))
                    return;

                  List<string> updatedListenIconList = listenIcon.GetBindValues(player.oid, nuiToken.Token);
                  List<string> updatedListenTooltipList = listenTooltip.GetBindValues(player.oid, nuiToken.Token);

                  if (!player.listened.Contains(selectedPlayer))
                  {
                    player.listened.Add(selectedPlayer);
                    player.oid.SendServerMessage($"{selectedPlayer.LoginCreature.Name.ColorString(ColorConstants.White)} est désormais sur écoute.", ColorConstants.Blue);

                    updatedListenIconList[nuiEvent.ArrayIndex] = "ief_blind";
                    updatedListenTooltipList[nuiEvent.ArrayIndex] = "Retirer ce joueur de la liste d'écoute";
                  }
                  else
                  {
                    player.listened.Remove(selectedPlayer);
                    player.oid.SendServerMessage($"{selectedPlayer.LoginCreature.Name.ColorString(ColorConstants.White)} n'est plus sur écoute.", ColorConstants.Blue);

                    updatedListenIconList[nuiEvent.ArrayIndex] = "ief_concealed";
                    updatedListenTooltipList[nuiEvent.ArrayIndex] = "Mettre ce joueur sur écoute";
                  }

                  listenIcon.SetBindValues(player.oid, nuiToken.Token, updatedListenIconList);
                  listenTooltip.SetBindValues(player.oid, nuiToken.Token, updatedListenTooltipList);

                  break;

                case "globalListen":

                  if (player.listened.Count > 0)
                  {
                    player.oid.SendServerMessage("Ecoute globale désactivée.", ColorConstants.Cyan);
                    player.listened.Clear();

                    globalListenIcon.SetBindValue(player.oid, nuiToken.Token, "ief_concealed");
                    globalListenTooltip.SetBindValue(player.oid, nuiToken.Token, "Activer l'écoute de tous les joueurs.");
                  }
                  else
                  {
                    foreach (NwPlayer oPC in NwModule.Instance.Players.Where(p => !p.IsDM))
                      player.listened.Add(oPC);

                    globalListenIcon.SetBindValue(player.oid, nuiToken.Token, "ief_blind");
                    globalListenTooltip.SetBindValue(player.oid, nuiToken.Token, "Désactiver l'écoute de tous les joueurs.");

                    player.oid.SendServerMessage("Ecoute globale activée.", ColorConstants.Cyan);
                  }

                  break;

                case "tp":

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);

                  if (selectedPlayer.ControlledCreature.Location != null)
                    player.oid.ControlledCreature.Location = selectedPlayer.ControlledCreature.Location;
                  else
                    player.oid.SendServerMessage($"{selectedPlayer.ControlledCreature.Name} est actuellement en transition. Veuillez attendre la fin du chargement de zone pour vous téléporter.", ColorConstants.Red);
                  
                  break;

                case "kick":
                  myPlayerList.ElementAt(nuiEvent.ArrayIndex).BootPlayer();
                  break;

                case "hostile": 

                  selectedPlayer = myPlayerList.ElementAt(nuiEvent.ArrayIndex);
                  List<string> updatedHostileIconList = hostileIcon.GetBindValues(player.oid, nuiToken.Token);
                  List<string> updatedHostileTooltipList = hostileTooltip.GetBindValues(player.oid, nuiToken.Token);

                  if (player.oid.ControlledCreature.IsReactionTypeHostile(selectedPlayer.ControlledCreature))
                  {
                    player.oid.SetPCReputation(true, selectedPlayer);
                    updatedHostileIconList[nuiEvent.ArrayIndex] = "ief_acincr";
                    updatedHostileTooltipList[nuiEvent.ArrayIndex] = "Activer l'hostilité à l'égard de ce joueur";
                  }
                  else
                  {
                    player.oid.SetPCReputation(false, selectedPlayer);
                    updatedHostileIconList[nuiEvent.ArrayIndex] = "ief_acdecr";
                    updatedHostileTooltipList[nuiEvent.ArrayIndex] = "Désactiver l'hostilité à l'égard de ce joueur";
                  }

                  hostileIcon.SetBindValues(player.oid, nuiToken.Token, updatedHostileIconList);
                  hostileTooltip.SetBindValues(player.oid, nuiToken.Token, updatedHostileTooltipList);

                  break;

                case "areaHostile":

                  foreach (NwPlayer disliked in NwModule.Instance.Players.Where(p => p.IsConnected && !player.oid.IsDM && p.ControlledCreature.Area == player.oid.ControlledCreature.Area && p != player.oid && !player.oid.PartyMembers.Contains(p)))
                    player.oid.SetPCReputation(false, disliked);

                  break;
              }

              break;
          }
        }
        public void UpdatePlayerList()
        {
          List<string> playerNamesList = new ();
          List<string> areaNamesList = new ();
          List<string> brpList = new();
          List<string> brpDownList = new();
          List<string> muteIconList = new();
          List<string> muteTooltipList = new();
          List<string> listenIconList = new();
          List<string> listenTooltipList = new();
          List<string> hostileIconList = new();
          List<string> hostileTooltipList = new();
          List<bool> partyInviteEnabledList = new ();
          List<bool> muteEnabledList = new();

          myPlayerList = NwModule.Instance.Players.Where(p => p.IsConnected && p != player.oid);

          foreach (NwPlayer playerList in myPlayerList)
          {
            if (!Players.TryGetValue(playerList.LoginCreature, out Player playerObject))
              continue;

            string playerName = playerList.ControlledCreature != playerList.LoginCreature ? $"{playerList.LoginCreature.Name} ({playerList.ControlledCreature.Name})" : playerList.LoginCreature.Name;
            playerNamesList.Add(playerName);

            string areaName = playerName;
            string brpLabel = "Recommander ce joueur";
            string brpDownLabel = "";

            if (player.oid.IsDM || player.oid.PlayerName == "Chim")
            {
              areaName += " - " + playerList.ControlledCreature.Area != null ? playerList.ControlledCreature.Area.Name : "En transition";

              brpLabel = playerObject.bonusRolePlay < 4 ? $"Augmenter le bonus roleplay au niveau {playerObject.bonusRolePlay + 1}" : "Bonus roleplay niveau max (4)";
              brpDownLabel = playerObject.bonusRolePlay > 0 ? $"Diminuer le bonus roleplay au niveau {playerObject.bonusRolePlay - 1}" : "Bonus roleplay niveau min (0)";

              muteEnabledList.Add(true);
            }
            else
              muteEnabledList.Add(false);

            if (player.mutedList.Contains(playerObject.accountId))
            {
              muteIconList.Add("ief_forcewalk");
              muteTooltipList.Add("Autoriser les MPs de ce joueur");
            }
            else
            {
              muteIconList.Add("ief_haste");
              muteTooltipList.Add("Bloquer les MPs de ce joueur");
            }

            if(player.listened.Contains(playerList))
            {
              listenIconList.Add("ief_blind");
              listenTooltipList.Add("Retirer ce joueur de la liste d'écoute");
            }
            else
            {
              listenIconList.Add("ief_concealed");
              listenTooltipList.Add("Mettre ce joueur sur écoute");
            }

            if(player.oid.ControlledCreature.IsReactionTypeHostile(playerList.ControlledCreature))
            {
              hostileIconList.Add("ief_acdecr");
              hostileTooltipList.Add("Désactiver l'hostilité à l'égard de ce joueur");
            }
            else
            {
              hostileIconList.Add("ief_acincr");
              hostileTooltipList.Add("Activer l'hostilité à l'égard de ce joueur");
            }

            brpList.Add(brpLabel);
            brpDownList.Add(brpDownLabel);
            areaNamesList.Add(areaName);
            partyInviteEnabledList.Add(!player.oid.PartyMembers.Contains(playerList));
          }

          playerNames.SetBindValues(player.oid, nuiToken.Token, playerNamesList);
          areaNames.SetBindValues(player.oid, nuiToken.Token, areaNamesList);
          partyInviteEnabled.SetBindValues(player.oid, nuiToken.Token, partyInviteEnabledList);
          bonusRoleplay.SetBindValues(player.oid, nuiToken.Token, brpList);
          bonusRoleplayDown.SetBindValues(player.oid, nuiToken.Token, brpDownList);
          muteIcon.SetBindValues(player.oid, nuiToken.Token, muteIconList);
          muteTooltip.SetBindValues(player.oid, nuiToken.Token, muteTooltipList);
          muteEnabled.SetBindValues(player.oid, nuiToken.Token, muteEnabledList);
          listenIcon.SetBindValues(player.oid, nuiToken.Token, listenIconList);
          listenTooltip.SetBindValues(player.oid, nuiToken.Token, listenTooltipList);
          hostileIcon.SetBindValues(player.oid, nuiToken.Token, hostileIconList);
          hostileTooltip.SetBindValues(player.oid, nuiToken.Token, hostileTooltipList);
          listCount.SetBindValue(player.oid, nuiToken.Token, playerNamesList.Count);
        }
        private static void SaveBRPToDatabase(Player commendedPlayer)
        {
          SqLiteUtils.UpdateQuery("PlayerAccounts",
            new List<string[]>() { new string[] { "bonusRolePlay", commendedPlayer.bonusRolePlay.ToString() } },
            new List<string[]>() { new string[] { "rowid", commendedPlayer.accountId.ToString() } });
        }
        private static async void SaveMutedPlayersToDatabase(Player player)
        {
          Task<string> serializeMuted = Task.Run(() => JsonConvert.SerializeObject(player.mutedList));
          await serializeMuted;

          SqLiteUtils.UpdateQuery("PlayerAccounts",
              new List<string[]>() { new string[] { "mutedPlayers", serializeMuted.Result } },
              new List<string[]>() { new string[] { "rowid", player.accountId.ToString() } });
        }
      }
    }
  }
}
