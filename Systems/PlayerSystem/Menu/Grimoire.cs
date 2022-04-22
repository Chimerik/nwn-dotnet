using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class GrimoiresWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> grimoireName = new ("grimoireName");
        private readonly NuiBind<bool> saveGrimoireEnabled = new ("saveGrimoireEnabled");
        private readonly List<string> grimoireNamesList = new List<string>();

        public GrimoiresWindow(Player player) : base(player)
        {
          windowId = "grimoires";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Nom d'un nouveau grimoire", grimoireName, 35, false) { Width = 300, Height = 35, Tooltip = "Afin d'enregistrer un nouveau grimoire, un nom doit être renseigné." },
              new NuiButton("Enregistrer") { Id = "newGrimoire", Width = 80, Height = 35, Enabled = saveGrimoireEnabled, Tooltip = "Enregistre un nouveau grimoire avec vos sorts tels qu'actuellement configurés." }
            }
          });

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell> 
          { 
            new NuiListTemplateCell(new NuiButton(buttonText) { Id = "load", Height = 35 }) { Width = 300 },
            new NuiListTemplateCell(new NuiButton("Supprimer") { Id = "delete", Height = 35 }) { Width = 60 },
          };
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          window = new NuiWindow(rootGroup, "Grimoires - Sélection")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleGrimoiresEvents;
          player.oid.OnNuiEvent += HandleGrimoiresEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          foreach (Grimoire grimoire in player.grimoires)
            grimoireNamesList.Add(grimoire.name);

          buttonText.SetBindValues(player.oid, token, grimoireNamesList);
          listCount.SetBindValue(player.oid, token, grimoireNamesList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          grimoireName.SetBindValue(player.oid, token, "");
          grimoireName.SetBindWatch(player.oid, token, true);

          saveGrimoireEnabled.SetBindValue(player.oid, token, false);

          player.openedWindows[windowId] = token;
        }
        private void HandleGrimoiresEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "newGrimoire":

                  SaveGrimoire(grimoireName.GetBindValue(player.oid, token));
                  grimoireName.SetBindValue(player.oid, token, "");
                  player.oid.SendServerMessage("Nouveau grimoire sauvegardé.", ColorConstants.Orange);

                  break;

                case "delete":

                  DeleteGrimoire(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Grimoire supprimé.", ColorConstants.Orange);

                  break;

                case "load":

                  LoadGrimoire(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Grimoire chargé.", ColorConstants.Orange);

                  break;

              }
              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "grimoireName":

                  if (grimoireName.GetBindValue(player.oid, token).Length > 0)
                    saveGrimoireEnabled.SetBindValue(player.oid, token, true);
                  else
                    saveGrimoireEnabled.SetBindValue(player.oid, token, false);

                  break;

              }

              break;
          }
        }
        private void SaveGrimoire(string grimoireName)
        {
          List<int> spellList = new List<int>();
          List<int> metamagicList = new List<int>();

          for (byte i = 0; i < 10; i++)
          {
            foreach (MemorizedSpellSlot spellSlot in player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(i))
            {
              if (!spellSlot.IsPopulated)
                continue;

              spellList.Add((int)spellSlot.Spell.SpellType);
              metamagicList.Add((int)spellSlot.MetaMagic);
            }
          }

          player.grimoires.Add(new Grimoire(grimoireName, spellList, metamagicList));

          grimoireNamesList.Add(grimoireName);
          buttonText.SetBindValues(player.oid, token, grimoireNamesList);
          listCount.SetBindValue(player.oid, token, grimoireNamesList.Count);
        }

        private void DeleteGrimoire(int index)
        {
          player.grimoires.RemoveAt(index);
          grimoireNamesList.RemoveAt(index);
          buttonText.SetBindValues(player.oid, token, grimoireNamesList);
          listCount.SetBindValue(player.oid, token, grimoireNamesList.Count);
        }

        private void LoadGrimoire(int index)
        {
          Grimoire grimoire = player.grimoires[index];
          byte previousSpellLevel = 0;
          int j = 0;

          for (int i = 0; i < grimoire.spellList.Count; i++)
          {
            NwSpell spell = NwSpell.FromSpellId(grimoire.spellList[i]);
            byte spellLevel = spell.InnateSpellLevel;

            if (previousSpellLevel < spellLevel)
              j = 0;

            previousSpellLevel = spellLevel;

            if (player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlotCountByLevel(spellLevel) < j)
              continue;

            player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spellLevel)[j].Spell = spell;
            player.oid.ControlledCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spellLevel)[j].MetaMagic = (MetaMagic)grimoire.metamagicList[i];
          }
        }
      }
    }
  }
}
