using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public enum ResourceExchangeState
      {
        CreatingProposal,
        AwaitingProposal,
        AwaitingConfirmation
      }
      public class ResourceExchangeWindow : PlayerWindow
      {
        private readonly NuiRow rootRow = new NuiRow();
        private readonly List<NuiElement> rootChildren = new List<NuiElement>();
        private readonly NuiColumn ownerColumn = new NuiColumn() { Width = 400 };
        private readonly List<NuiElement> ownerChildren = new List<NuiElement>();
        private readonly NuiColumn targetColumn = new NuiColumn() { Width = 400 };
        private readonly List<NuiElement> targetChildren = new List<NuiElement>();
        private readonly List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>();
        private readonly List<NuiListTemplateCell> targetRowTemplate = new List<NuiListTemplateCell>();

        private readonly NuiBind<string> myGold = new NuiBind<string>("myGold");
        private readonly NuiBind<string> myResourceNames = new NuiBind<string>("myResourceNames");
        private readonly NuiBind<int> myListCount = new NuiBind<int>("myListCount");
        private readonly NuiBind<string> myResourceIcon = new NuiBind<string>("myResourceIcon");
        private readonly NuiBind<string> myAvailableQuantity = new NuiBind<string>("myAvailableQuantity");
        private readonly NuiBind<string> myQuantity = new NuiBind<string>("myQuantity");

        private readonly NuiBind<string> targetGold = new NuiBind<string>("targetGold");
        private readonly NuiBind<string> targetResourceNames = new NuiBind<string>("targetResourceNames");
        private readonly NuiBind<int> targetListCount = new NuiBind<int>("targetListCount");
        private readonly NuiBind<string> targetResourceIcon = new NuiBind<string>("targetResourceIcon");

        private readonly NuiBind<bool> proposalEnabled = new NuiBind<bool>("proposalEnabled");
        private readonly NuiBind<bool> confirmEnabled = new NuiBind<bool>("confirmEnabled");
        private readonly NuiBind<bool> cancelEnabled = new NuiBind<bool>("cancelEnabled");

        private readonly NuiBind<string> targetState = new NuiBind<string>("targetState");

        public ResourceExchangeState exchangeState { get; set; }
        List<CraftResource> myResourceList;
        List<CraftResource> targetProposal = new List<CraftResource>();
        Player targetPlayer;
        ResourceExchangeWindow targetWindow;

        public ResourceExchangeWindow(Player player, NwGameObject creatureTarget) : base(player)
        {
          windowId = "resourceExchange";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(myResourceIcon) { Tooltip = myResourceNames, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(myResourceNames) { Tooltip = myAvailableQuantity, VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", myQuantity, 10, false)) { Width = 100 });

          targetRowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(targetResourceIcon) { Tooltip = targetResourceNames, Height = 35 }) { Width = 35 });
          targetRowTemplate.Add(new NuiListTemplateCell(new NuiLabel(targetResourceNames) { Tooltip = targetResourceNames, VerticalAlign = NuiVAlign.Middle }));

          rootRow.Children = rootChildren;
          ownerColumn.Children = ownerChildren;
          targetColumn.Children = targetChildren;

          rootChildren.Add(ownerColumn);
          rootChildren.Add(targetColumn);

          CreateOwnerWindow(creatureTarget);
        }
        public ResourceExchangeWindow(Player player, Player playerTarget) : base(player)
        {
          windowId = "resourceExchange";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(myResourceIcon) { Tooltip = myResourceNames, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(myResourceNames) { Tooltip = myAvailableQuantity, VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", myQuantity, 60, false) { Enabled = proposalEnabled }) { Width = 100 });

          targetRowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(targetResourceIcon) { Tooltip = targetResourceNames, Height = 35 }) { Width = 35 });
          targetRowTemplate.Add(new NuiListTemplateCell(new NuiLabel(targetResourceNames) { Tooltip = targetResourceNames, VerticalAlign = NuiVAlign.Middle }));

          rootRow.Children = rootChildren;
          ownerColumn.Children = ownerChildren;
          targetColumn.Children = targetChildren;

          rootChildren.Add(ownerColumn);
          rootChildren.Add(targetColumn);

          CreateWindow(playerTarget);
        }
        private void CreateWindow(Player target)
        {
          this.targetPlayer = target;
          exchangeState = ResourceExchangeState.CreatingProposal;

          ownerChildren.Clear();
          targetChildren.Clear();
          targetProposal.Clear();

          ownerChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel("Votre proposition") { Height = 35, HorizontalAlign = NuiHAlign.Center } } });

          ownerChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel($"Or") { Tooltip = $"Disponible en banque : {player.bankGold}", Width = 30, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("", myGold, 60, false) { Enabled = proposalEnabled, Tooltip = $"Disponible en banque : {player.bankGold}", Width = 300 }
            }
          });

          ownerChildren.Add(new NuiRow() { Height = 350, Children = new List<NuiElement>() { new NuiList(rowTemplate, myListCount) { RowHeight = 35 } } });

          ownerChildren.Add(new NuiRow()
          {
            Height = 35,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Proposer") { Id = "send", Tooltip = "Affiche votre proposition dans la fenêtre du destinataire.", Enabled = proposalEnabled, Width = 80 },
              new NuiButton("Finaliser") { Id = "confirm", Tooltip = "Finalise la transaction après confirmation des deux parties impliquées.", Enabled = confirmEnabled, Width = 80 },
              new NuiButton("Annuler") { Id = "cancel", Tooltip = "Annule la validation et permet de modifier à nouveau la proposition.", Enabled = cancelEnabled, Width = 80 },
              new NuiSpacer()
            }
          });

          targetChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Proposition de {targetPlayer.oid.LoginCreature.Name}") { Height = 35, HorizontalAlign = NuiHAlign.Center } } });

          targetChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel($"Or : ") { Width = 30, VerticalAlign = NuiVAlign.Middle },
              new NuiLabel(targetGold) { Width = 300, VerticalAlign = NuiVAlign.Middle }
            }
          });

          targetChildren.Add(new NuiRow() { Height = 350, Children = new List<NuiElement>() { new NuiList(targetRowTemplate, targetListCount) { RowHeight = 35 } } });
          targetChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiLabel(targetState) { HorizontalAlign = NuiHAlign.Center } } });

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 850);

          window = new NuiWindow(rootRow, $"Proposition d'échange de ressources entre {player.oid.LoginCreature.Name} et {target.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleResourceExchangeEvents;
          player.oid.OnNuiEvent += HandleResourceExchangeEvents;
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          player.oid.OnServerSendArea += OnAreaChangeCloseWindow;
          player.oid.OnClientDisconnect -= HandleResourceExchangeDisconnection;
          player.oid.OnClientDisconnect += HandleResourceExchangeDisconnection;

          token = player.oid.CreateNuiWindow(window, windowId);

          myGold.SetBindValue(player.oid, token, "0");
          targetGold.SetBindValue(player.oid, token, "0");
          targetState.SetBindValue(player.oid, token, "Proposition en cours de modification");

          proposalEnabled.SetBindValue(player.oid, token, true);
          confirmEnabled.SetBindValue(player.oid, token, false);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          LoadResourceList();
        }
        public async void CreateOwnerWindow(NwGameObject target)
        {
          if (!Players.TryGetValue(target, out Player targetPlayer))
          {
            player.oid.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} n'est pas un joueur et ne peut donc pas négocier de transaction.", ColorConstants.Red);
            return;
          }

          if (player.oid.ControlledCreature.DistanceSquared(target) > 25)
          {
            player.oid.SendServerMessage($"Vous êtes trop éloigné de {target.Name.ColorString(ColorConstants.White)} pour négocier une transaction.", ColorConstants.Red);
            return;
          }

          if (targetPlayer.openedWindows.ContainsKey(windowId))
          {
            player.oid.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est déjà en train de négocier une transaction.", ColorConstants.Red);
            return;
          }

          CreateWindow(targetPlayer);

          if (targetPlayer.windows.ContainsKey(windowId))
            ((ResourceExchangeWindow)targetPlayer.windows[windowId]).CreateWindow(player);
          else
            targetPlayer.windows.Add(windowId, new ResourceExchangeWindow(targetPlayer, player));

          await NwTask.Delay(TimeSpan.FromSeconds(0.2));

          targetWindow = (ResourceExchangeWindow)targetPlayer.windows[windowId];
          targetWindow.targetWindow = this;
        }

        private void HandleResourceExchangeEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              // OnClose => Fermer la fenêtre du destinataire si celle-ci est toujours ouverte
              player.oid.OnClientDisconnect -= HandleResourceExchangeDisconnection;
              targetWindow.CloseWindow();
              targetPlayer.oid.SendServerMessage($"Transaction annulée par {player.oid.LoginCreature.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);

              break;
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "send":

                  exchangeState = ResourceExchangeState.AwaitingProposal;
                  proposalEnabled.SetBindValue(player.oid, token, false);
                  cancelEnabled.SetBindValue(player.oid, token, true);

                  targetWindow.targetState.SetBindValue(targetPlayer.oid, targetWindow.token, "En cours de relecture");

                  if (!int.TryParse(myGold.GetBindValue(player.oid, token), out int myInputGold))
                    targetWindow.targetGold.SetBindValue(targetPlayer.oid, targetWindow.token, "0");
                  else if (myInputGold > player.bankGold)
                    targetWindow.targetGold.SetBindValue(targetPlayer.oid, targetWindow.token, player.bankGold.ToString());
                  else
                    targetWindow.targetGold.SetBindValue(targetPlayer.oid, targetWindow.token, myInputGold.ToString());

                  List<CraftResource> myProposal = new List<CraftResource>();
                  List<string> proposedQuantityList = myQuantity.GetBindValues(player.oid, token);

                  for (int i = 0; i < proposedQuantityList.Count; i++)
                  {
                    if (int.TryParse(proposedQuantityList[i], out int proposedQuantity) && proposedQuantity > 0)
                    {
                      if (proposedQuantity > myResourceList[i].quantity)
                        proposedQuantity = myResourceList[i].quantity;
                        
                      myProposal.Add(new CraftResource(myResourceList[i], proposedQuantity));
                    }
                  }
  
                  targetWindow.LoadTargetResourceList(myProposal);

                  if (targetWindow.exchangeState == ResourceExchangeState.AwaitingProposal)
                  {
                    confirmEnabled.SetBindValue(player.oid, token, true);
                    targetWindow.confirmEnabled.SetBindValue(targetPlayer.oid, targetWindow.token, true);
                  }

                  break;

                case "confirm":

                  exchangeState = ResourceExchangeState.AwaitingConfirmation;

                  if (targetWindow.exchangeState == ResourceExchangeState.AwaitingConfirmation)
                  {
                    if (!int.TryParse(targetWindow.targetGold.GetBindValue(targetPlayer.oid, targetWindow.token), out int myGoldInput) || myGoldInput > player.bankGold)
                    {
                      CloseWindow();
                      targetWindow.CloseWindow();
                      targetPlayer.oid.SendServerMessage($"Transaction annulée. {player.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose plus d'assez d'or en banque.", ColorConstants.Red);
                      player.oid.SendServerMessage($"Transaction annulée. Vous ne disposez plus d'assez d'or en banque.", ColorConstants.Red);
                      return;
                    }

                    if (!int.TryParse(targetGold.GetBindValue(player.oid, token), out int targetGoldInput) || targetGoldInput > targetPlayer.bankGold)
                    {
                      CloseWindow();
                      targetWindow.CloseWindow();
                      player.oid.SendServerMessage($"Transaction annulée. {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose plus d'assez d'or en banque.", ColorConstants.Red);
                      targetPlayer.oid.SendServerMessage($"Transaction annulée. Vous ne disposez plus d'assez d'or en banque.", ColorConstants.Red);
                      return;
                    }

                    foreach (CraftResource resource in targetProposal)
                      if (resource.quantity > targetPlayer.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade).quantity)
                      {
                        CloseWindow();
                        targetWindow.CloseWindow();
                        player.oid.SendServerMessage($"Transaction annulée. {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose plus d'assez de {resource.name.ColorString(ColorConstants.White)}", ColorConstants.Red);
                        targetPlayer.oid.SendServerMessage($"Transaction annulée. Vous ne disposez plus d'assez de {resource.name.ColorString(ColorConstants.White)}", ColorConstants.Red);
                        return;
                      }

                    foreach (CraftResource resource in targetWindow.targetProposal)
                      if (resource.quantity > player.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade).quantity)
                      {
                        CloseWindow();
                        targetWindow.CloseWindow();
                        player.oid.SendServerMessage($"Transaction annulée. Vous ne disposez plus d'assez de {resource.name.ColorString(ColorConstants.White)}", ColorConstants.Red);
                        targetPlayer.oid.SendServerMessage($"Transaction annulée. {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose plus d'assez de {resource.name.ColorString(ColorConstants.White)}", ColorConstants.Red);
                        return;
                      }
                    
                    player.bankGold += targetGoldInput;
                    player.bankGold -= myGoldInput;
                    targetPlayer.bankGold += myGoldInput;
                    targetPlayer.bankGold -= targetGoldInput;

                    foreach (CraftResource resource in targetProposal)
                    {
                      CraftResource myResource = player.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade);

                      if (myResource != null)
                        myResource.quantity += resource.quantity;
                      else
                        player.craftResourceStock.Add(new CraftResource(resource, resource.quantity));

                      targetPlayer.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade).quantity -= resource.quantity;
                    }

                    foreach (CraftResource resource in targetWindow.targetProposal)
                    {
                      CraftResource targetResource = targetPlayer.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade);

                      if (targetResource != null)
                        targetResource.quantity += resource.quantity;
                      else
                        targetPlayer.craftResourceStock.Add(new CraftResource(resource, resource.quantity));

                      player.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade).quantity -= resource.quantity;
                    }

                    player.oid.SendServerMessage($"Transaction avec {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} terminée avec succès !", new Color(32, 255, 32));
                    targetPlayer.oid.SendServerMessage($"Transaction avec {player.oid.LoginCreature.Name.ColorString(ColorConstants.White)} terminée avec succès !", new Color(32, 255, 32));

                    CloseWindow();
                    targetWindow.CloseWindow();

                    player.oid.ExportCharacter();
                    targetPlayer.oid.ExportCharacter();
                  }
                  else
                  {
                    confirmEnabled.SetBindValue(player.oid, token, false);
                    targetWindow.targetState.SetBindValue(targetPlayer.oid, targetWindow.token, "En attente de votre finalisation");
                  }

                 break;

                case "cancel":

                  exchangeState = ResourceExchangeState.CreatingProposal;
                  proposalEnabled.SetBindValue(player.oid, token, true);
                  confirmEnabled.SetBindValue(player.oid, token, false);
                  cancelEnabled.SetBindValue(player.oid, token, false);

                  targetWindow.targetState.SetBindValue(targetPlayer.oid, targetWindow.token, "Proposition en cours de modification");

                  if (targetWindow.exchangeState > ResourceExchangeState.CreatingProposal)
                  {
                    targetWindow.exchangeState = ResourceExchangeState.AwaitingProposal;
                    targetWindow.confirmEnabled.SetBindValue(targetPlayer.oid, targetWindow.token, false);
                  }

                  break;
              }

              break;
          }
        }
        private void LoadResourceList()
        {
          List<string> resourceNameList = new List<string>();
          List<string> resourceIconList = new List<string>();
          List<string> availableQuantityList = new List<string>();
          List<string> resourceQuantityList = new List<string>();
          myResourceList = player.craftResourceStock.Where(r => r.quantity > 0).OrderBy(r => r.type).ThenBy(r => r.grade).ToList();

          foreach (CraftResource resource in myResourceList)
          {
            resourceNameList.Add($"{resource.name} (x{resource.quantity})");
            resourceIconList.Add(resource.iconString);
            availableQuantityList.Add($"{resource.quantity} unité(s) en stock");
            resourceQuantityList.Add("0");
          }

          myResourceNames.SetBindValues(player.oid, token, resourceNameList);
          myResourceIcon.SetBindValues(player.oid, token, resourceIconList);
          myAvailableQuantity.SetBindValues(player.oid, token, availableQuantityList);
          myQuantity.SetBindValues(player.oid, token, resourceQuantityList);
          myListCount.SetBindValue(player.oid, token, myResourceList.Count());
        }
        private void LoadTargetResourceList(List<CraftResource> targetResourceList)
        {
          List<string> resourceNameList = new List<string>();
          List<string> resourceIconList = new List<string>();
          List<string> resourceQuantityList = new List<string>();

          foreach (CraftResource resource in targetResourceList)
          {
            resourceNameList.Add($"{resource.name} (x{resource.quantity})");
            resourceIconList.Add(resource.iconString);
          }

          targetResourceNames.SetBindValues(player.oid, token, resourceNameList);
          targetResourceIcon.SetBindValues(player.oid, token, resourceIconList);
          targetListCount.SetBindValue(player.oid, token, targetResourceList.Count());

          targetProposal = targetResourceList;
        }
        private void HandleResourceExchangeDisconnection(OnClientDisconnect onPCDisconnect)
        {
          CloseWindow();
          targetPlayer.oid.SendServerMessage($"Transaction annulée par {player.oid.LoginCreature.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        }
      }
    }
  }
}
