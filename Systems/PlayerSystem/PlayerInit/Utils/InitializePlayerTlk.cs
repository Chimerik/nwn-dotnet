﻿using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private async void InitializePlayerTlk()
      {
        await NwTask.NextFrame();

        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DivinationPresage))
        {
          NwFeat.FromFeatId(CustomSkill.DivinationPresage).Name.SetPlayerOverride(oid, $"Présage : {oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage1Variable).Value}");
          NwFeat.FromFeatId(CustomSkill.DivinationPresage2).Name.SetPlayerOverride(oid, $"Présage : {oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage2Variable).Value}");

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DivinationPresageSuperieur))
            NwFeat.FromFeatId(CustomSkill.DivinationPresageSuperieur).Name.SetPlayerOverride(oid, $"Présage : {oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.Presage3Variable).Value}");
        }

        if (learnableSkills.ContainsKey(CustomSkill.FighterArcaneArcher))
        {
          new StrRef(8).SetPlayerOverride(oid, "Archer-Mage");
          oid.SetTextureOverride("fighter", "arcanearcher");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterChampion))
        {
          new StrRef(8).SetPlayerOverride(oid, "Champion");
          oid.SetTextureOverride("fighter", "champion");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterWarMaster))
        {
          new StrRef(8).SetPlayerOverride(oid, "Maître de Guerre");
          oid.SetTextureOverride("fighter", "warmaster");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterEldritchKnight))
        {
          new StrRef(8).SetPlayerOverride(oid, "Guerrier Occulte");
          oid.SetTextureOverride("fighter", "eldritchknight");
        }

        if (learnableSkills.ContainsKey(CustomSkill.BarbarianBerseker))
        {
          new StrRef(5213).SetPlayerOverride(oid, "Berseker");
          oid.SetTextureOverride("barbarian", "berseker");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianTotem))
        {
          new StrRef(5213).SetPlayerOverride(oid, "Voie du Totem");
          oid.SetTextureOverride("barbarian", "totem");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianWildMagic))
        {
          new StrRef(5213).SetPlayerOverride(oid, "Voie de la Magie Sauvage");
          oid.SetTextureOverride("barbarian", "wildmagic");
        }
        
        if (learnableSkills.ContainsKey(CustomSkill.RogueThief))
        {
          new StrRef(16).SetPlayerOverride(oid, "Voleur");
          oid.SetTextureOverride("rogue", "thief");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueAssassin))
        {
          new StrRef(16).SetPlayerOverride(oid, "Assassin");
          oid.SetTextureOverride("rogue", "assassin");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueArcaneTrickster))
        {
          new StrRef(16).SetPlayerOverride(oid, "Escroc Arcanique");
          oid.SetTextureOverride("rogue", "arcane_trickster");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueConspirateur))
        {
          new StrRef(16).SetPlayerOverride(oid, "Conspirateur");
          oid.SetTextureOverride("rogue", "conspirateur");
        }

        if (learnableSkills.ContainsKey(CustomSkill.MonkPaume))
        {
          new StrRef(10).SetPlayerOverride(oid, "Voie de la Paume");
          oid.SetTextureOverride("monk", "monk_paume");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.MonkOmbre))
        {
          new StrRef(10).SetPlayerOverride(oid, "Voie de l'Ombre");
          oid.SetTextureOverride("monk", "monk_shadow");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.MonkElements))
        {
          new StrRef(10).SetPlayerOverride(oid, "Voie des Quatre Eléments");
          oid.SetTextureOverride("monk", "monk_elements");
        }

        if (learnableSkills.ContainsKey(CustomSkill.WizardAbjuration))
        {
          new StrRef(20).SetPlayerOverride(oid, "Abjurateur");
          oid.SetTextureOverride("wizard", "abjuration");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardDivination))
        {
          new StrRef(20).SetPlayerOverride(oid, "Devin");
          oid.SetTextureOverride("wizard", "divination");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEnchantement))
        {
          new StrRef(20).SetPlayerOverride(oid, "Enchanteur");
          oid.SetTextureOverride("wizard", "enchantement");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Evocateur");
          oid.SetTextureOverride("wizard", "evocation");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Illusionniste");
          oid.SetTextureOverride("wizard", "illusion");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Invocateur");
          oid.SetTextureOverride("wizard", "invocation");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardNecromancie))
        {
          new StrRef(20).SetPlayerOverride(oid, "Nécromancien");
          oid.SetTextureOverride("wizard", "necromancie");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardTransmutation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Transmutateur");
          oid.SetTextureOverride("wizard", "transmutation");
        }

        if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDuSavoir))
        {
          new StrRef(2).SetPlayerOverride(oid, "Collège du Savoir");
          oid.SetTextureOverride("bard", "college_lore");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDeLaVaillance))
        {
          new StrRef(2).SetPlayerOverride(oid, "Collège de la Vaillance");
          oid.SetTextureOverride("bard", "vaillance");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDeLaVaillance))
        {
          new StrRef(2).SetPlayerOverride(oid, "Collège de l'Escrime");
          oid.SetTextureOverride("bard", "escrime");
        }

        if (learnableSkills.ContainsKey(CustomSkill.RangerChasseur))
        {
          new StrRef(14).SetPlayerOverride(oid, "Conclave des Chasseurs");
          oid.SetTextureOverride("ranger", "chasseur");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RangerBelluaire))
        {
          new StrRef(14).SetPlayerOverride(oid, "Conclave des Belluaires");
          oid.SetTextureOverride("ranger", "conclave_betes");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RangerProfondeurs))
        {
          new StrRef(14).SetPlayerOverride(oid, "Conclave des Profondeurs");
          oid.SetTextureOverride("ranger", "profondeurs");
        }

        var auraDeProtection = NwFeat.FromFeatId(CustomSkill.AuraDeProtection);

        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeCourage))
        {
          var auraDeCourage = NwFeat.FromFeatId(CustomSkill.AuraDeCourage);
          var tlk = NwFeat.FromFeatId(CustomSkill.AuraDeProtection).Name;
          tlk.SetPlayerOverride(oid, "Aura de Courage");

          oid.SetTextureOverride(auraDeProtection.IconResRef, auraDeCourage.IconResRef);

          tlk = auraDeProtection.Description;
          tlk.SetPlayerOverride(oid, $"{tlk.ToString()}\n\n{auraDeCourage.Description.ToString()}");
        }

        if (learnableSkills.ContainsKey(CustomSkill.PaladinSermentDevotion))
        {
          new StrRef(12).SetPlayerOverride(oid, "Serment de Dévotion");
          oid.SetTextureOverride("paladin", "devotion");

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinAuraDeDevotion))
          {
            var auraDeDevotion = NwFeat.FromFeatId(CustomSkill.PaladinAuraDeDevotion);
            var tlk = auraDeProtection.Name;
            tlk.SetPlayerOverride(oid, "Aura de Dévotion");

            oid.SetTextureOverride(auraDeProtection.IconResRef, auraDeDevotion.IconResRef);

            tlk = auraDeProtection.Description;
            tlk.SetPlayerOverride(oid, $"{tlk.ToString()}\n\n{auraDeDevotion.Description.ToString()}");
          }
        }
        else if(learnableSkills.ContainsKey(CustomSkill.PaladinSermentDesAnciens))
        {
          new StrRef(12).SetPlayerOverride(oid, "Serment des Anciens");
          oid.SetTextureOverride("paladin", "anciens");

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinAuraDeGarde))
          {
            var auraDeGarde = NwFeat.FromFeatId(CustomSkill.PaladinAuraDeGarde);
            var tlk = auraDeProtection.Name;
            tlk.SetPlayerOverride(oid, "Aura de Garde");

            oid.SetTextureOverride(auraDeProtection.IconResRef, auraDeGarde.IconResRef);

            tlk = auraDeProtection.Description;
            tlk.SetPlayerOverride(oid, $"{tlk.ToString()}\n\n{auraDeGarde.Description.ToString()}");
          }
        }
        else if (learnableSkills.ContainsKey(CustomSkill.PaladinSermentVengeance))
        {
          new StrRef(12).SetPlayerOverride(oid, "Serment de Vengeance");
          oid.SetTextureOverride("paladin", "vengeance"); 
        }

        if (learnableSkills.ContainsKey(CustomSkill.ClercDuperie))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Duperie");
          oid.SetTextureOverride("clerc", "duperie");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercGuerre))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Guerre");
          oid.SetTextureOverride("clerc", "guerre");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercLumiere))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Lumière");
          oid.SetTextureOverride("clerc", "light_domain");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercNature))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Nature");
          oid.SetTextureOverride("clerc", "nature_domain");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercSavoir))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Nature");
          oid.SetTextureOverride("clerc", "domaine_savoir");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercTempete))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Tempête");
          oid.SetTextureOverride("clerc", "domaine_tempete");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercVie))
        {
          new StrRef(12).SetPlayerOverride(oid, "Domaine de la Vie");
          oid.SetTextureOverride("clerc", "domaine_vie");
        }
        
        if (learnableSkills.ContainsKey(CustomSkill.EnsorceleurLigneeDraconique))
        {
          new StrRef(9).SetPlayerOverride(oid, "Lignée Draconique");
          oid.SetTextureOverride("ensorceleur", "enso_draconique");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.EnsorceleurTempete))
        {
          new StrRef(9).SetPlayerOverride(oid, "Sorcellerie de la Tempête");
          oid.SetTextureOverride("ensorceleur", "enso_tempete");
        }
        
        if (learnableSkills.ContainsKey(CustomSkill.DruideCercleTellurique))
        {
          new StrRef(6).SetPlayerOverride(oid, "Cercle Tellurique");
          oid.SetTextureOverride("druide", "druide_terre");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.DruideCercleSelenite))
        {
          new StrRef(6).SetPlayerOverride(oid, "Cercle Sélénite");
          oid.SetTextureOverride("druide", "druide_lune");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.DruideCerclePelagique))
        {
          new StrRef(6).SetPlayerOverride(oid, "Cercle Pélagique");
          oid.SetTextureOverride("druide", "druide_mer");
        }

        if (learnableSkills.ContainsKey(CustomSkill.OccultisteArchifee))
        {
          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(oid, "Mécène Archifée");
          oid.SetTextureOverride("occultiste", "warlock_archfey");
        }
        else if(learnableSkills.ContainsKey(CustomSkill.OccultisteCeleste))
        {
          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(oid, "Mécène Céleste");
          oid.SetTextureOverride("occultiste", "warlock_celeste");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.OccultisteFielon))
        {
          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(oid, "Mécène Fiélon");
          oid.SetTextureOverride("occultiste", "warlock_fielon");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.OccultisteGrandAncien))
        {
          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(oid, "Mécène Grand Ancien");
          oid.SetTextureOverride("occultiste", "warlock_ancien");
        }
      }
    }
  }
}
