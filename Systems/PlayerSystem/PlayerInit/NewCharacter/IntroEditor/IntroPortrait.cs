using System;
using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroPortraitWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> portraits1 = new("portraits1");
        private readonly NuiBind<string> portraits2 = new("portraits2");
        private readonly NuiBind<string> portraits3 = new("portraits3");
        private readonly NuiBind<string> portraits4 = new("portraits4");
        private readonly NuiBind<string> portraits5 = new("portraits5");
        private readonly NuiBind<string> portraits6 = new("portraits6");
        private readonly NuiBind<string> portraits7 = new("portraits7");

        private readonly NuiBind<int> listCount = new("listCount");
        private string portraitResRef;

        public IntroPortraitWindow(Player player) : base(player)
        {
          windowId = "introPortrait";
          rootColumn.Children = rootChildren;

          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits1) { Id = "portraitSelect1", Tooltip = portraits1 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits2) { Id = "portraitSelect2", Tooltip = portraits2 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits3) { Id = "portraitSelect3", Tooltip = portraits3 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits4) { Id = "portraitSelect4", Tooltip = portraits4 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits5) { Id = "portraitSelect5", Tooltip = portraits5 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits6) { Id = "portraitSelect6", Tooltip = portraits6 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits7) { Id = "portraitSelect7", Tooltip = portraits7 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()) { VariableSize = true });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 70, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 70, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue },
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = 70 , Encouraged = player.oid.LoginCreature.GetObjectVariable < PersistentVariableInt >("_IN_CHARACTER_CREATION_STATS").HasValue},
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 128 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.9f);

          window = new NuiWindow(rootColumn, "Votre reflet - Choisissez votre portrait")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleIntroMirrorEvents;

            UpdatePortraitList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleIntroMirrorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "welcome":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new IntroMirrorWindow(player));
                  else ((IntroMirrorWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();

                  return;

                case "beauty":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyColorsModifier")) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, player.oid.LoginCreature));
                  else ((BodyColorWindow)player.windows["bodyColorsModifier"]).CreateWindow(player.oid.LoginCreature);

                  break;

                case "class":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introClassSelector")) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)player.windows["introClassSelector"]).CreateWindow();

                  break;

                case "race":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introRaceSelector")) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)player.windows["introRaceSelector"]).CreateWindow();

                  break;

                case "histo":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introHistorySelector")) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)player.windows["introHistorySelector"]).CreateWindow();

                  break;

                case "stats":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introAbilities")) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)player.windows["introAbilities"]).CreateWindow();

                  break;

                case "portraitSelect1":

                  portraitResRef = portraits1.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];
                  
                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }

                  break;

                case "portraitSelect2":
                  
                    portraitResRef = portraits2.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }
                  break;

                case "portraitSelect3":

                  portraitResRef = portraits1.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }
                  
                  break;

                case "portraitSelect4":

                  portraitResRef = portraits4.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    ModuleSystem.Log.Info(player.oid.LoginCreature.PortraitResRef);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }
                  
                  break;

                case "portraitSelect5":

                  portraitResRef = portraits5.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    ModuleSystem.Log.Info(player.oid.LoginCreature.PortraitResRef);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }

                  break;

                case "portraitSelect6":

                  portraitResRef = portraits6.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }

                  break;

                case "portraitSelect7":

                  portraitResRef = portraits7.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!string.IsNullOrEmpty(portraitResRef))
                  {
                    player.oid.LoginCreature.PortraitResRef = portraitResRef.Remove(portraitResRef.Length - 1);
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Delete();
                  }

                  break;
              }

              break;
          }
        }
        private void UpdatePortraitList()
        {
          List<string>[] portraitList = new List<string>[] { new(), new(), new(), new(), new(), new(), new() };
          List<string> portraitTable = new();

          if (Portraits2da.playerCustomPortraits.ContainsKey(player.oid.PlayerName))
            portraitTable.AddRange(Portraits2da.playerCustomPortraits[player.oid.PlayerName]);

          int baseRaceId = GetBaseRaceIdFromCustomRace(player.oid.LoginCreature.Race.Id);

          portraitTable.AddRange(Portraits2da.portraitFilteredEntries[baseRaceId, (int)player.oid.LoginCreature.Gender]);

          if(baseRaceId == CustomRace.HalfElf)
          {
            portraitTable.AddRange(Portraits2da.portraitFilteredEntries[CustomRace.Human, (int)player.oid.LoginCreature.Gender]);
            portraitTable.AddRange(Portraits2da.portraitFilteredEntries[CustomRace.Elf, (int)player.oid.LoginCreature.Gender]);
          }

          if (portraitTable != null)
            for (int i = 0; i < portraitTable.Count; i += 7)
              for (int j = 0; j < 7; j++)
                try { portraitList[j].Add(portraitTable[i + j]); }
                catch (Exception) { portraitList[j].Add(""); }

          portraits1.SetBindValues(player.oid, nuiToken.Token, portraitList[0]);
          portraits2.SetBindValues(player.oid, nuiToken.Token, portraitList[1]);
          portraits3.SetBindValues(player.oid, nuiToken.Token, portraitList[2]);
          portraits4.SetBindValues(player.oid, nuiToken.Token, portraitList[3]);
          portraits5.SetBindValues(player.oid, nuiToken.Token, portraitList[4]);
          portraits6.SetBindValues(player.oid, nuiToken.Token, portraitList[5]);
          portraits7.SetBindValues(player.oid, nuiToken.Token, portraitList[6]);
          listCount.SetBindValue(player.oid, nuiToken.Token, portraitList[0].Count);
        }
        private int GetBaseRaceIdFromCustomRace(int customRace)
        {
          return customRace switch
          {
            CustomRace.DeepGnome or CustomRace.RockGnome or CustomRace.ForestGnome => CustomRace.Gnome,
            CustomRace.Drow or CustomRace.HighElf or CustomRace.WoodElf => CustomRace.Elf,
            CustomRace.DrowHalfElf or CustomRace.HighHalfElf or CustomRace.WoodHalfElf => CustomRace.HalfElf,
            CustomRace.GoldDwarf or CustomRace.ShieldDwarf or CustomRace.Duergar => CustomRace.Dwarf,
            CustomRace.LightfootHalfling or CustomRace.StrongheartHalfling => CustomRace.Halfling,
            CustomRace.HalfOrc => CustomRace.HalfOrc,
            _ => CustomRace.Human,
          };
        }
      }
    }
  }
}
