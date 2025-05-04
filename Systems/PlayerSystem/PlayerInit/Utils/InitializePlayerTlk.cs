using Anvil.API;

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
          var playerClass = NwClass.FromClassType(ClassType.Fighter);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Archer-Mage"))
            tlkOverrides[playerClass.Name] = "Archer-Mage";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "arcanearcher"))
            iconOverrides[playerClass.IconResRef] = "arcanearcher";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterChampion))
        {
          var playerClass = NwClass.FromClassType(ClassType.Fighter);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Champion"))
            tlkOverrides[playerClass.Name] = "Champion";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "champion"))
            iconOverrides[playerClass.IconResRef] = "champion";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterWarMaster))
        {
          var playerClass = NwClass.FromClassType(ClassType.Fighter);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Maître de Guerre"))
            tlkOverrides[playerClass.Name] = "Maître de Guerre";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "warmaster"))
            iconOverrides[playerClass.IconResRef] = "warmaster";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterEldritchKnight))
        {
          var playerClass = NwClass.FromClassType(ClassType.Fighter);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Guerrier Occulte"))
            tlkOverrides[playerClass.Name] = "Guerrier Occulte";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "eldritchknight"))
            iconOverrides[playerClass.IconResRef] = "eldritchknight";
        }

        if (learnableSkills.ContainsKey(CustomSkill.BarbarianBerseker))
        {
          var playerClass = NwClass.FromClassType(ClassType.Barbarian);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Berseker"))
            tlkOverrides[playerClass.Name] = "Berseker";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "berseker"))
            iconOverrides[playerClass.IconResRef] = "berseker";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianTotem))
        {
          var playerClass = NwClass.FromClassType(ClassType.Barbarian);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Voie du Totem"))
            tlkOverrides[playerClass.Name] = "Voie du Totem";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "totem"))
            iconOverrides[playerClass.IconResRef] = "totem";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianWildMagic))
        {
          var playerClass = NwClass.FromClassType(ClassType.Barbarian);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Voie de la Magie Sauvage"))
            tlkOverrides[playerClass.Name] = "Voie de la Magie Sauvage";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "wildmagic"))
            iconOverrides[playerClass.IconResRef] = "wildmagic";
        }

        if (learnableSkills.ContainsKey(CustomSkill.RogueThief))
        {
          var playerClass = NwClass.FromClassType(ClassType.Rogue);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Voleur"))
            tlkOverrides[playerClass.Name] = "Voleur";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "thief"))
            iconOverrides[playerClass.IconResRef] = "thief";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueAssassin))
        {
          var playerClass = NwClass.FromClassType(ClassType.Rogue);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Assassin"))
            tlkOverrides[playerClass.Name] = "Assassin";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "assassin"))
            iconOverrides[playerClass.IconResRef] = "assassin";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueArcaneTrickster))
        {
          var playerClass = NwClass.FromClassType(ClassType.Rogue);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Escroc Arcanique"))
            tlkOverrides[playerClass.Name] = "Escroc Arcanique";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "arcane_trickster"))
            iconOverrides[playerClass.IconResRef] = "arcane_trickster";
        }

        if (learnableSkills.ContainsKey(CustomSkill.MonkPaume))
        {
          var playerClass = NwClass.FromClassType(ClassType.Monk);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Voie de la Paume"))
            tlkOverrides[playerClass.Name] = "Voie de la Paume";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "monk_paume"))
            iconOverrides[playerClass.IconResRef] = "monk_paume";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.MonkOmbre))
        {
          var playerClass = NwClass.FromClassType(ClassType.Monk);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Voie de l'Ombre"))
            tlkOverrides[playerClass.Name] = "Voie de l'Ombre";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "monk_shadow"))
            iconOverrides[playerClass.IconResRef] = "monk_shadow";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.MonkElements))
        {
          var playerClass = NwClass.FromClassType(ClassType.Monk);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Voie des Eléments"))
            tlkOverrides[playerClass.Name] = "Voie des Eléments";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "monk_elements"))
            iconOverrides[playerClass.IconResRef] = "monk_elements";
        }

        if (learnableSkills.ContainsKey(CustomSkill.WizardAbjuration))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Abjurateur"))
            tlkOverrides[playerClass.Name] = "Abjurateur";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "abjuration"))
            iconOverrides[playerClass.IconResRef] = "abjuration";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardDivination))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Devin"))
            tlkOverrides[playerClass.Name] = "Devin";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "divination"))
            iconOverrides[playerClass.IconResRef] = "divination";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEnchantement))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Enchanteur"))
            tlkOverrides[playerClass.Name] = "Enchanteur";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "enchantement"))
            iconOverrides[playerClass.IconResRef] = "enchantement";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Evocateur"))
            tlkOverrides[playerClass.Name] = "Evocateur";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "evocation"))
            iconOverrides[playerClass.IconResRef] = "evocation";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Illusionniste"))
            tlkOverrides[playerClass.Name] = "Illusionniste";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "illusion"))
            iconOverrides[playerClass.IconResRef] = "illusion";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Invocateur"))
            tlkOverrides[playerClass.Name] = "Invocateur";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "invocation"))
            iconOverrides[playerClass.IconResRef] = "invocation";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardNecromancie))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Nécromancien"))
            tlkOverrides[playerClass.Name] = "Nécromancien";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "necromancie"))
            iconOverrides[playerClass.IconResRef] = "necromancie";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardTransmutation))
        {
          var playerClass = NwClass.FromClassType(ClassType.Wizard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Transmutateur"))
            tlkOverrides[playerClass.Name] = "Transmutateur";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "transmutation"))
            iconOverrides[playerClass.IconResRef] = "transmutation";
        }

        if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDuSavoir))
        {
          var playerClass = NwClass.FromClassType(ClassType.Bard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Collège du Savoir"))
            tlkOverrides[playerClass.Name] = "Collège du Savoir";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "college_lore"))
            iconOverrides[playerClass.IconResRef] = "college_lore";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDeLaVaillance))
        {
          var playerClass = NwClass.FromClassType(ClassType.Bard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Collège de la Vaillance"))
            tlkOverrides[playerClass.Name] = "Collège de la Vaillance";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "vaillance"))
            iconOverrides[playerClass.IconResRef] = "vaillance";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDeLaVaillance))
        {
          var playerClass = NwClass.FromClassType(ClassType.Bard);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Collège de l'Escrime"))
            tlkOverrides[playerClass.Name] = "Collège de l'Escrime";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "escrime"))
            iconOverrides[playerClass.IconResRef] = "escrime";
        }

        if (learnableSkills.ContainsKey(CustomSkill.RangerChasseur))
        {
          var playerClass = NwClass.FromClassType(ClassType.Ranger);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Conclave des Chasseurs"))
            tlkOverrides[playerClass.Name] = "Conclave des Chasseurs";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "chasseur"))
            iconOverrides[playerClass.IconResRef] = "chasseur";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RangerBelluaire))
        {
          var playerClass = NwClass.FromClassType(ClassType.Ranger);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Conclave des Belluaires"))
            tlkOverrides[playerClass.Name] = "Conclave des Belluaires";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "conclave_betes"))
            iconOverrides[playerClass.IconResRef] = "conclave_betes";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RangerProfondeurs))
        {
          var playerClass = NwClass.FromClassType(ClassType.Ranger);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Conclave des Profondeurs"))
            tlkOverrides[playerClass.Name] = "Conclave des Profondeurs";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "profondeurs"))
            iconOverrides[playerClass.IconResRef] = "profondeurs";
        }

        var auraDeProtection = NwFeat.FromFeatId(CustomSkill.AuraDeProtection);

        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeCourage))
        {
          var auraDeCourage = NwFeat.FromFeatId(CustomSkill.AuraDeCourage);
          var tlk = NwFeat.FromFeatId(CustomSkill.AuraDeProtection).Name;

          if (!tlkOverrides.TryAdd(tlk, "Aura de Courage"))
            tlkOverrides[tlk] = "Aura de Courage";
          if (!iconOverrides.TryAdd(auraDeProtection.IconResRef, auraDeCourage.IconResRef))
            iconOverrides[auraDeProtection.IconResRef] = auraDeCourage.IconResRef;

          tlk = auraDeProtection.Description;

          if (!tlkOverrides.TryAdd(tlk, $"{tlk.ToString()}\n\n{auraDeCourage.Description.ToString()}"))
            tlkOverrides[tlk] = $"{tlk.ToString()}\n\n{auraDeCourage.Description.ToString()}";
        }

        if (learnableSkills.ContainsKey(CustomSkill.PaladinSermentDevotion))
        {
          var playerClass = NwClass.FromClassType(ClassType.Paladin);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Serment de Dévotion"))
            tlkOverrides[playerClass.Name] = "Serment de Dévotion";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "devotion"))
            iconOverrides[playerClass.IconResRef] = "devotion";

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinAuraDeDevotion))
          {
            var auraDeDevotion = NwFeat.FromFeatId(CustomSkill.PaladinAuraDeDevotion);
            var tlk = auraDeProtection.Name;

            if (!tlkOverrides.TryAdd(tlk, "Aura de Dévotion"))
              tlkOverrides[tlk] = "Aura de Dévotion";
            if (!iconOverrides.TryAdd(auraDeProtection.IconResRef, auraDeDevotion.IconResRef))
              iconOverrides[auraDeProtection.IconResRef] = auraDeDevotion.IconResRef;

            tlk = auraDeProtection.Description;

            if (!tlkOverrides.TryAdd(tlk, $"{tlk.ToString()}\n\n{auraDeDevotion.Description.ToString()}"))
              tlkOverrides[tlk] = $"{tlk.ToString()}\n\n{auraDeDevotion.Description.ToString()}";
          }
        }
        else if(learnableSkills.ContainsKey(CustomSkill.PaladinSermentDesAnciens))
        {
          var playerClass = NwClass.FromClassType(ClassType.Paladin);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Serment des Anciens"))
            tlkOverrides[playerClass.Name] = "Serment des Anciens";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "anciens"))
            iconOverrides[playerClass.IconResRef] = "anciens";

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinAuraDeGarde))
          {
            var auraDeGarde = NwFeat.FromFeatId(CustomSkill.PaladinAuraDeGarde);
            var tlk = auraDeProtection.Name;

            if (!tlkOverrides.TryAdd(tlk, "Aura de Garde"))
              tlkOverrides[tlk] = "Aura de Garde";
            if (!iconOverrides.TryAdd(auraDeProtection.IconResRef, auraDeGarde.IconResRef))
              iconOverrides[auraDeProtection.IconResRef] = auraDeGarde.IconResRef;

            tlk = auraDeProtection.Description;

            if (!tlkOverrides.TryAdd(tlk, $"{tlk.ToString()}\n\n{auraDeGarde.Description.ToString()}"))
              tlkOverrides[tlk] = $"{tlk.ToString()}\n\n{auraDeGarde.Description.ToString()}";
          }
        }
        else if (learnableSkills.ContainsKey(CustomSkill.PaladinSermentVengeance))
        {
          var playerClass = NwClass.FromClassType(ClassType.Paladin);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Serment de Vengeance"))
            tlkOverrides[playerClass.Name] = "Serment des Vengeance";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "vengeance"))
            iconOverrides[playerClass.IconResRef] = "vengeance";
        }

        if (learnableSkills.ContainsKey(CustomSkill.ClercDuperie))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Duperie"))
            tlkOverrides[playerClass.Name] = "Domaine de la Duperie";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "duperie"))
            iconOverrides[playerClass.IconResRef] = "duperie";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercGuerre))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Guerre"))
            tlkOverrides[playerClass.Name] = "Domaine de la Guerre";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "guerre"))
            iconOverrides[playerClass.IconResRef] = "guerre";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercLumiere))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Lumière"))
            tlkOverrides[playerClass.Name] = "Domaine de la Lumière";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "light_domain"))
            iconOverrides[playerClass.IconResRef] = "light_domain";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercNature))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Nature"))
            tlkOverrides[playerClass.Name] = "Domaine de la Nature";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "nature_domain"))
            iconOverrides[playerClass.IconResRef] = "nature_domain";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercSavoir))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Nature"))
            tlkOverrides[playerClass.Name] = "Domaine de la Nature";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "domaine_savoir"))
            iconOverrides[playerClass.IconResRef] = "domaine_savoir";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercTempete))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Tempête"))
            tlkOverrides[playerClass.Name] = "Domaine de la Tempête";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "domaine_tempete"))
            iconOverrides[playerClass.IconResRef] = "domaine_tempete";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.ClercVie))
        {
          var playerClass = NwClass.FromClassType(ClassType.Cleric);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Domaine de la Vie"))
            tlkOverrides[playerClass.Name] = "Domaine de la Vie";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "domaine_vie"))
            iconOverrides[playerClass.IconResRef] = "domaine_vie";
        }
        
        if (learnableSkills.ContainsKey(CustomSkill.EnsorceleurLigneeDraconique))
        {
          var playerClass = NwClass.FromClassType(ClassType.Sorcerer);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Lignée Draconique"))
            tlkOverrides[playerClass.Name] = "Lignée Draconique";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "enso_draconique"))
            iconOverrides[playerClass.IconResRef] = "enso_draconique";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.EnsorceleurTempete))
        {
          var playerClass = NwClass.FromClassType(ClassType.Sorcerer);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Sorcellerie de la Tempête"))
            tlkOverrides[playerClass.Name] = "Sorcellerie de la Tempête";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "enso_tempete"))
            iconOverrides[playerClass.IconResRef] = "enso_tempete";
        }
        
        if (learnableSkills.ContainsKey(CustomSkill.DruideCercleTellurique))
        {
          var playerClass = NwClass.FromClassType(ClassType.Druid);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Cercle Tellurique"))
            tlkOverrides[playerClass.Name] = "Cercle Tellurique";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "druide_terre"))
            iconOverrides[playerClass.IconResRef] = "druide_terre";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.DruideCercleSelenite))
        {
          var playerClass = NwClass.FromClassType(ClassType.Druid);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Cercle Sélénite"))
            tlkOverrides[playerClass.Name] = "Cercle Sélénite";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "druide_lune"))
            iconOverrides[playerClass.IconResRef] = "druide_lune";

          new StrRef(6).SetPlayerOverride(oid, "Cercle Sélénite");
          oid.SetTextureOverride("druide", "druide_lune");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.DruideCerclePelagique))
        {
          var playerClass = NwClass.FromClassType(ClassType.Druid);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Cercle Pélagique"))
            tlkOverrides[playerClass.Name] = "Cercle Pélagique";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "druide_mer"))
            iconOverrides[playerClass.IconResRef] = "druide_mer";
        }

        if (learnableSkills.ContainsKey(CustomSkill.OccultisteArchifee))
        {
          var playerClass = NwClass.FromClassId(CustomClass.Occultiste);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Mécène Archifée"))
            tlkOverrides[playerClass.Name] = "Mécène Archifée";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "warlock_archfey"))
            iconOverrides[playerClass.IconResRef] = "warlock_archfey";
        }
        else if(learnableSkills.ContainsKey(CustomSkill.OccultisteCeleste))
        {
          var playerClass = NwClass.FromClassId(CustomClass.Occultiste);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Mécène Céleste"))
            tlkOverrides[playerClass.Name] = "Mécène Céleste";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "warlock_celeste"))
            iconOverrides[playerClass.IconResRef] = "warlock_celeste";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.OccultisteFielon))
        {
          var playerClass = NwClass.FromClassId(CustomClass.Occultiste);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Mécène Fiélon"))
            tlkOverrides[playerClass.Name] = "Mécène Fiélon";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "warlock_fielon"))
            iconOverrides[playerClass.IconResRef] = "warlock_fielon";
        }
        else if (learnableSkills.ContainsKey(CustomSkill.OccultisteGrandAncien))
        {
          var playerClass = NwClass.FromClassId(CustomClass.Occultiste);

          if (!tlkOverrides.TryAdd(playerClass.Name, "Mécène Grand Ancien"))
            tlkOverrides[playerClass.Name] = "Mécène Grand Ancien";
          if (!iconOverrides.TryAdd(playerClass.IconResRef, "warlock_ancien"))
            iconOverrides[playerClass.IconResRef] = "warlock_ancien";
        }

        foreach(var tlk in tlkOverrides)
          tlk.Key.SetPlayerOverride(oid, tlk.Value);

        foreach (var icon in iconOverrides)
          oid.SetTextureOverride(icon.Key, icon.Value);
      }
    }
  }
}
