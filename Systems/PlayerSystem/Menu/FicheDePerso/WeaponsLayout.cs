using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiBind<string> shield = new("shield");
        private readonly NuiBind<bool> shieldVisibility = new("shieldVisibility");
        private readonly NuiBind<string> towerShield = new("towerShield");
        private readonly NuiBind<bool> towerShieldVisibility = new("towerShieldVisibility");
        private readonly NuiBind<string> lightArmor = new("llightArmor");
        private readonly NuiBind<bool> lightArmorVisibility = new("lightArmorVisibility");
        private readonly NuiBind<string> mediumArmor = new("mediumArmor");
        private readonly NuiBind<bool> mediumArmorVisibility = new("mediumArmorVisibility");
        private readonly NuiBind<string> heavyArmor = new("heavyArmor");
        private readonly NuiBind<bool> heavyArmorVisibility = new("heavyArmorVisibility");
        private readonly NuiBind<string> club = new("club");
        private readonly NuiBind<bool> clubVisibility = new("clubVisibility");
        private readonly NuiBind<string> dagger = new("deception");
        private readonly NuiBind<bool> daggerVisibility = new("daggerVisibility");
        private readonly NuiBind<string> spear = new("spear");
        private readonly NuiBind<bool> spearVisibility = new("spearVisibility");
        private readonly NuiBind<string> shortSword = new("shortSword");
        private readonly NuiBind<bool> shortSwordVisibility = new("shortSwordVisibility");
        private readonly NuiBind<string> longsword = new("longsword");
        private readonly NuiBind<bool> longswordVisibility = new("longswordVisibility");
        private readonly NuiBind<string> shortbow = new("shortbow");
        private readonly NuiBind<bool> shortbowVisibility = new("shortbowVisibility");
        private readonly NuiBind<string> longBow = new("longBow");
        private readonly NuiBind<bool> longBowVisibility = new("longBowVisibility");
        private readonly NuiBind<string> rapier = new("rapier");
        private readonly NuiBind<bool> rapierVisibility = new("rapierVisibility");
        private readonly NuiBind<string> lightHammer = new("lightHammer");
        private readonly NuiBind<bool> lightHammerVisibility = new("lightHammerVisibility");
        private readonly NuiBind<string> handAxe = new("handAxe");
        private readonly NuiBind<bool> handAxeVisibility = new("handAxeVisibility");
        private readonly NuiBind<string> battleAxe = new("battleAxe");
        private readonly NuiBind<bool> battleAxeVisibility = new("battleAxeVisibility");
        private readonly NuiBind<string> dwarvenAxe = new("dwarvenAxe");
        private readonly NuiBind<bool> dwarvenAxeVisibility = new("dwarvenAxeVisibility");
        private readonly NuiBind<string> shuriken = new("shuriken");
        private readonly NuiBind<bool> shurikenVisibility = new("shurikenVisibility");
        private readonly NuiBind<string> doubleBlade = new("doubleBlade");
        private readonly NuiBind<bool> doubleBladeVisibility = new("doubleBladeVisibility");
        private readonly NuiBind<string> lightMace = new("lightMace");
        private readonly NuiBind<bool> lightMaceVisibility = new("lightMaceVisibility");
        private readonly NuiBind<string> quarterStaff = new("quarterStaff");
        private readonly NuiBind<bool> quarterStaffVisibility = new("quarterStaffVisibility");
        private readonly NuiBind<string> sickle = new("sickle");
        private readonly NuiBind<bool> sickleVisibility = new("sickleVisibility");
        private readonly NuiBind<string> lightCrossbow = new("lightCrossbow");
        private readonly NuiBind<bool> lightCrossbowVisibility = new("lightCrossbowVisibility");
        private readonly NuiBind<string> dart = new("dart");
        private readonly NuiBind<bool> dartVisibility = new("dartVisibility");
        private readonly NuiBind<string> lightFlail = new("lightFlail");
        private readonly NuiBind<bool> lightFlailVisibility = new("lightFlailVisibility");
        private readonly NuiBind<string> morningstar = new("morningstar");
        private readonly NuiBind<bool> morningstarVisibility = new("morningstarVisibility");
        private readonly NuiBind<string> sling = new("sling");
        private readonly NuiBind<bool> slingVisibility = new("slingVisibility");
        private readonly NuiBind<string> greatAxe = new("greatAxe");
        private readonly NuiBind<bool> greatAxeVisibility = new("greatAxeVisibility");
        private readonly NuiBind<string> greatSword = new("greatSword");
        private readonly NuiBind<bool> greatSwordVisibility = new("greatSwordVisibility");
        private readonly NuiBind<string> scimitar = new("scimitar");
        private readonly NuiBind<bool> scimitarVisibility = new("scimitarVisibility");
        private readonly NuiBind<string> halberd = new("halberd");
        private readonly NuiBind<bool> halberdVisibility = new("halberdVisibility");
        private readonly NuiBind<string> heavyFlail = new("heavyFlail");
        private readonly NuiBind<bool> heavyFlailVisibility = new("heavyFlailVisibility");
        private readonly NuiBind<string> throwingAxe = new("throwingAxe");
        private readonly NuiBind<bool> throwingAxeVisibility = new("throwingAxeVisibility");
        private readonly NuiBind<string> warHammer = new("warHammer");
        private readonly NuiBind<bool> warHammerVisibility = new("warHammerVisibility");
        private readonly NuiBind<string> heavyCrossbow = new("heavyCrossbow");
        private readonly NuiBind<bool> heavyCrossbowVisibility = new("heavyCrossbowVisibility");
        private readonly NuiBind<string> bastardSword = new("bastardSword");
        private readonly NuiBind<bool> bastardSwordVisibility = new("bastardSwordVisibility");
        private readonly NuiBind<string> scythe = new("scythe");
        private readonly NuiBind<bool> scytheVisibility = new("scytheVisibility");
        private readonly NuiBind<string> direMace = new("direMace");
        private readonly NuiBind<bool> direMaceVisibility = new("direMaceVisibility");
        private readonly NuiBind<string> doubleAxe = new("doubleAxe");
        private readonly NuiBind<bool> doubleAxeVisibility = new("doubleAxeVisibility");
        private readonly NuiBind<string> kama = new("kama");
        private readonly NuiBind<bool> kamaVisibility = new("kamaVisibility");
        private readonly NuiBind<string> katana = new("katana");
        private readonly NuiBind<bool> katanaVisibility = new("katanaVisibility");
        private readonly NuiBind<string> kukri = new("kukri");
        private readonly NuiBind<bool> kukriVisibility = new("kukriVisibility");
        private readonly NuiBind<string> whip = new("whip");
        private readonly NuiBind<bool> whipVisibility = new("whipVisibility");

        private void LoadWeaponsLayout()
        {
          LoadTopMenuLayout();

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
            new NuiLabel("Maîtrises d'armures") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiButtonImage("ife_sh_prof") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Bouclier") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiImage(shield) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = shieldVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiButtonImage("ife_armor_l") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Armure Légère") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiImage(lightArmor) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = lightArmorVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_armor_m") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Armure Intermédiaire") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(mediumArmor) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = mediumArmorVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_armor_h") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Armure Lourde") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(heavyArmor) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = heavyArmorVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("is_ShieldMaster") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Pavois") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(towerShield) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = towerShieldVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Armes Simples") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Clu") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Gourdin") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(club) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = clubVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Dag") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Dague") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(dagger) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = daggerVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Hax") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Hachette") { Height = windowWidth / 12,VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(handAxe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = handAxeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_LHa") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Marteau Léger") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(lightHammer) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = lightHammerVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Lma") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Masse Légère") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(lightMace) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = lightMaceVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Sta") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Bâton") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(quarterStaff) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = quarterStaffVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Sic") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Serpe") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(sickle) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = sickleVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_LXb") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Arbalète Légère") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(lightCrossbow) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = lightCrossbowVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Dar") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Dard") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(dart) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = dartVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Lfl") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Fléau Léger") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(lightFlail) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = lightFlailVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Mor") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Morgenstern") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(morningstar) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = morningstarVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_SLi") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Fronde") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(sling) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = slingVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Spe") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Lance") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(spear) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = spearVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Sbw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Arc Court") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(shortbow) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = shortbowVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Armes Martiales") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Bax") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Hache de Guerre") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(battleAxe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = battleAxeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });
          
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Gax") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Hache d'armes") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(greatAxe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = greatAxeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Tax") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Hache de Jet") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(throwingAxe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = throwingAxeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Ssw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Epée Courte") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(shortSword) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = shortSwordVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_LSw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Epée Longue") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(longsword) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = longswordVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Gsw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Espadon") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(greatSword) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = greatSwordVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Sci") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Cimeterre") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(scimitar) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = scimitarVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Rap") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Rapière") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(rapier) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = rapierVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Hfl") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Fléau Lourd") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(heavyFlail) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = heavyFlailVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Wha") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Marteau de Guerre") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(warHammer) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = warHammerVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });


          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Hal") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Hallebarde") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(halberd) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = halberdVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_X2WFWhip") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Fouet") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(whip) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = whipVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Hxb") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Arbalète Lourde") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(heavyCrossbow) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = heavyCrossbowVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Lbw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Arc Long") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(longBow) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = longBowVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Shu") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Arbalète de Poing") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(shuriken) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = shurikenVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Armes Exotiques") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Bsw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Epée Bâtarde") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(bastardSword) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = bastardSwordVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Kat") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Katana") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(katana) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = katanaVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_2sw") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Double Lame") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(doubleBlade) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = doubleBladeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Dax") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Double Hache") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(doubleAxe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = doubleAxeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Dma") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Double Masse") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(direMace) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = direMaceVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Scy") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Faux") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(scythe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = scytheVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_X2WFDWAx") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Hache Naine") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(dwarvenAxe) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = dwarvenAxeVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Kam") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Kama") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(kama) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = kamaVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_wepfoc_Kuk") { Width = windowWidth / 12, Height = windowWidth / 12 },
            new NuiSpacer() { Width = 25 },
            new NuiLabel("Kukri") { Height = windowWidth / 12, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left },
            new NuiSpacer(),
            new NuiImage(kukri) { Width = windowWidth / 8, Height = windowWidth / 12, Visible = kukriVisibility, HorizontalAlign = NuiHAlign.Right, VerticalAlign = NuiVAlign.Middle },
          } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
