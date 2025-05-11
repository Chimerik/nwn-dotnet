using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiBind<string> skillTitle = new("skillTitle");
        private readonly NuiBind<string> skillDescription = new("skillDescription");
        private readonly NuiBind<string> athletism = new("athletism");
        private readonly NuiBind<string> athletismProficiency = new("athletismProficiency");
        private readonly NuiBind<bool> athletismProficiencyVisibility = new("athletismProficiencyVisibility");
        private readonly NuiBind<string> acrobatie = new("acrobatie");
        private readonly NuiBind<string> acrobatieProficiency = new("acrobatieProficiency");
        private readonly NuiBind<bool> acrobatieProficiencyVisibility = new("acrobatieProficiencyVisibility");
        private readonly NuiBind<string> arcanes = new("arcanes");
        private readonly NuiBind<string> arcanesProficiency = new("arcanesProficiency");
        private readonly NuiBind<bool> arcanesProficiencyVisibility = new("arcanesProficiencyVisibility");
        private readonly NuiBind<string> dressage = new("dressage");
        private readonly NuiBind<string> dressageProficiency = new("dressageProficiency");
        private readonly NuiBind<bool> dressageProficiencyVisibility = new("dressageProficiencyVisibility");
        private readonly NuiBind<string> deception = new("deception");
        private readonly NuiBind<string> deceptionProficiency = new("deceptionProficiency");
        private readonly NuiBind<bool> deceptionProficiencyVisibility = new("deceptionProficiencyVisibility");
        private readonly NuiBind<string> history = new("history");
        private readonly NuiBind<string> historyProficiency = new("historyProficiency");
        private readonly NuiBind<bool> historyProficiencyVisibility = new("historyProficiencyVisibility");
        private readonly NuiBind<string> insight = new("insight");
        private readonly NuiBind<string> insightProficiency = new("insightProficiency");
        private readonly NuiBind<bool> insightProficiencyVisibility = new("insightProficiencyVisibility");
        private readonly NuiBind<string> intimidation = new("intimidation");
        private readonly NuiBind<string> intimidationProficiency = new("intimidationProficiency");
        private readonly NuiBind<bool> intimidationProficiencyVisibility = new("intimidationProficiencyVisibility");
        private readonly NuiBind<string> investigation = new("investigation");
        private readonly NuiBind<string> investigationProficiency = new("investigationProficiency");
        private readonly NuiBind<bool> investigationProficiencyVisibility = new("investigationProficiencyVisibility");
        private readonly NuiBind<string> medecine = new("medecine");
        private readonly NuiBind<string> medecineProficiency = new("medecineProficiency");
        private readonly NuiBind<bool> medecineProficiencyVisibility = new("medecineProficiencyVisibility");
        private readonly NuiBind<string> nature = new("nature");
        private readonly NuiBind<string> natureProficiency = new("natureProficiency");
        private readonly NuiBind<bool> natureProficiencyVisibility = new("natureProficiencyVisibility");
        private readonly NuiBind<string> perception = new("perception");
        private readonly NuiBind<string> perceptionProficiency = new("perceptionProficiency");
        private readonly NuiBind<bool> perceptionProficiencyVisibility = new("perceptionProficiencyVisibility");
        private readonly NuiBind<string> performance = new("performance");
        private readonly NuiBind<string> performanceProficiency = new("performanceProficiency");
        private readonly NuiBind<bool> performanceProficiencyVisibility = new("performanceProficiencyVisibility");
        private readonly NuiBind<string> persuasion = new("persuasion");
        private readonly NuiBind<string> persuasionProficiency = new("persuasionProficiency");
        private readonly NuiBind<bool> persuasionProficiencyVisibility = new("persuasionProficiencyVisibility");
        private readonly NuiBind<string> religion = new("religion");
        private readonly NuiBind<string> religionProficiency = new("religionProficiency");
        private readonly NuiBind<bool> religionProficiencyVisibility = new("religionProficiencyVisibility");
        private readonly NuiBind<string> sleightOfHand = new("sleightOfHand");
        private readonly NuiBind<string> sleightOfHandProficiency = new("sleightOfHandProficiency");
        private readonly NuiBind<bool> sleightOfHandProficiencyVisibility = new("sleightOfHandProficiencyVisibility");
        private readonly NuiBind<string> stealth = new("stealth");
        private readonly NuiBind<string> stealthProficiency = new("stealthProficiency");
        private readonly NuiBind<bool> stealthProficiencyVisibility = new("stealthProficiencyVisibility");
        private readonly NuiBind<string> survival = new("survival");
        private readonly NuiBind<string> survivalProficiency = new("survivalProficiency");
        private readonly NuiBind<bool> survivalProficiencyVisibility = new("survivalProficiencyVisibility");

        private void LoadSkillsLayout()
        {
          LoadTopMenuLayout();

          /*rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel(skillTitle) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 6, Children = new List<NuiElement>()
          { new NuiText(skillDescription) } });*/

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Origine") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(classGroup);

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("proficiency") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 20, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiLabel(proficiency) { Width = windowWidth / 2, Height = windowWidth / 20, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel(str) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 12, Width = windowWidth / 1.0f,*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Athletics") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Athlétisme") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(athletismProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = athletismProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(athletism) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel(dex) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Acrobatics") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Acrobatie") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(acrobatieProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = acrobatieProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(acrobatie) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Sleight") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Escamotage") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(sleightOfHandProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = sleightOfHandProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(sleightOfHand) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Stealth") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Furtivité") { Height = windowWidth / 12,VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(stealthProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = stealthProficiencyVisibility, Margin = 25, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(stealth) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel(intel) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Arcana") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Arcane") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(arcanesProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = arcanesProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(arcanes) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_History") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Histoire") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(historyProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = historyProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(history) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Investigation") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Investigation") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(investigationProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = investigationProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(investigation) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Nature") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Nature") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(natureProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = natureProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(nature) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Religion") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Religion") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(religionProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = religionProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(religion) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel(wis) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Animal") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Dressage") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(dressageProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = dressageProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(dressage) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Insight") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Intuition") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(insightProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = insightProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(insight) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Medicine") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Médecine") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(medecineProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = medecineProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(medecine) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Perception") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Perception") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(perceptionProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = perceptionProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(perception) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Survival") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Survie") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(survivalProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = survivalProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(survival) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel(cha) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Deception") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Tromperie") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(deceptionProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = deceptionProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(deception) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });
          
          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Intimidation") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Intimidation") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(intimidationProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = intimidationProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(intimidation) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Performance") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Performance") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(performanceProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = performanceProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(performance) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, /*Height = windowWidth / 9, Width = windowWidth / 1.1f*/ Children = new List<NuiElement>()
          {
            new NuiButtonImage("isk_Persuasion") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Persuasion") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(persuasionProficiency) { Width = windowWidth / 12, Height = windowWidth / 12, Visible = persuasionProficiencyVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer() { Width = 25 },
            new NuiLabel(persuasion) { Width = windowWidth / 8, Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Right }
          } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
