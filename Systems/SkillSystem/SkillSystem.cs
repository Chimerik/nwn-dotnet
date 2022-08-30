﻿using System;
using System.Collections.Generic;
using NLog;
using Anvil.API;
using NWN.Core;
using System.ComponentModel;
using System.Threading.Tasks;

namespace NWN.Systems
{
  public static class SkillSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public enum Category
    {
      [Description("Corps_&_Esprit")]
      MindBody,
      [Description("Combat")]
      Fight,
      [Description("Artisanat")]
      Craft,
      [Description("Magie")]
      Magic,
      [Description("Historique")]
      StartingTraits,
      [Description("Langage")]
      Language
    }

    public static Dictionary<int, Learnable> learnableDictionary = new Dictionary<int, Learnable>();

    public static void InitializeLearnables()
    {
      learnableDictionary.Add(CustomSkill.ImprovedStrength, new LearnableSkill(CustomSkill.ImprovedStrength, "Force accrue", "Augmente la force d'un point par niveau d'entraînement.", Category.MindBody, "ife_X2GrStr1", 4, 3, Ability.Constitution, Ability.Strength, false, HandleImproveAbility));
      learnableDictionary.Add(CustomSkill.ImprovedDexterity, new LearnableSkill(CustomSkill.ImprovedDexterity, "Dextérité accrue", "Augmente la dextérité d'un point par niveau d'entraînement.", Category.MindBody, "ife_X2GrDex1", 4, 3, Ability.Constitution, Ability.Dexterity, false, HandleImproveAbility));
      learnableDictionary.Add(CustomSkill.ImprovedConstitution, new LearnableSkill(CustomSkill.ImprovedConstitution, "Constitution accrue", "Augmente la constitution d'un point par niveau d'entraînement.", Category.MindBody, "ife_X2GrCon1", 4, 3, Ability.Constitution, Ability.Charisma, false, HandleImproveAbility));
      learnableDictionary.Add(CustomSkill.ImprovedIntelligence, new LearnableSkill(CustomSkill.ImprovedIntelligence, "Intelligence accrue", "Augmente l'intelligence d'un point par niveau d'entraînement.", Category.MindBody, "ife_X2GrInt1", 4, 3, Ability.Constitution, Ability.Intelligence, false, HandleImproveAbility));
      learnableDictionary.Add(CustomSkill.ImprovedWisdom, new LearnableSkill(CustomSkill.ImprovedWisdom, "Sagesse accrue", "Augmente la sagesse d'un point par niveau d'entraînement.", Category.MindBody, "ife_X2GrWis1", 4, 3, Ability.Constitution, Ability.Wisdom, false, HandleImproveAbility));
      learnableDictionary.Add(CustomSkill.ImprovedCharisma, new LearnableSkill(CustomSkill.ImprovedCharisma, "Charisme accrue", "Augmente le charisme d'un point par niveau d'entraînement.", Category.MindBody, "ife_X2GrCha1", 4, 3, Ability.Constitution, Ability.Charisma, false, HandleImproveAbility));

      learnableDictionary.Add(CustomSkill.ImprovedHealth, new LearnableSkill(CustomSkill.ImprovedHealth, "Résilience", "Augmente les points de vie de de 1 + (Robustesse + 3 * modificateur de constitution de base) par niveau.\n\n Ce don est rétroactif.", Category.MindBody, "ife_X2GrCon1", 5, 2, Ability.Constitution, Ability.Charisma, false, HandleImproveHealth));
      learnableDictionary.Add(CustomSkill.Toughness, new LearnableSkill(CustomSkill.Toughness, "Robustesse", "Augmente le multiplicateur d'augmentation des points de vie de un par niveau d'entraînement.\n\n Rétroactif.", Category.MindBody, "ife_tough", 5, 1, Ability.Constitution, Ability.Charisma, false, HandleImproveHealth));

      learnableDictionary.Add(CustomSkill.ImprovedFortitude, new LearnableSkill(CustomSkill.ImprovedFortitude, "Vigueur renforcée", "Augmente le jet de vigueur d'un point par niveau d'entraînement.", Category.MindBody, "ife_X1Blood", 8, 1, Ability.Strength, Ability.Constitution, false, HandleImproveSavingThrow));
      learnableDictionary.Add(CustomSkill.ImprovedReflex, new LearnableSkill(CustomSkill.ImprovedReflex, "Réflexes renforcés", "Augmente le jet de réflexe d'un point par niveau d'entraînement.", Category.MindBody, "ife_X1Snake", 8, 1, Ability.Dexterity, Ability.Constitution, false, HandleImproveSavingThrow));
      learnableDictionary.Add(CustomSkill.ImprovedWill, new LearnableSkill(CustomSkill.ImprovedWill, "Volonté renforcée", "Augmente le jet de volonté d'un point par niveau d'entraînement.", Category.MindBody, "ife_X1Bull", 8, 1, Ability.Wisdom, Ability.Constitution, false, HandleImproveSavingThrow));

      learnableDictionary.Add(CustomSkill.Athletics, new LearnableSkill(CustomSkill.Athletics, "Athlétisme", "Un jet de Force (Athlétisme) couvre les difficultés physiques que vous rencontrez en grimpant, en sautant ou en nageant. Ce qui inclue les activités suivantes :\n\nVous essayez d'escalader une falaise abrupte ou glissante, d'éviter les dangers en escaladant un mur ou de vous accrocher à une surface pendant que quelque chose essaie de vous faire tomber.\nVous essayez de sauter sur une distance inhabituellement longue ou de réaliser une cascade au milieu d'un saut.\nVous avez du mal à nager ou à rester à flot dans des courants dangereux, des vagues agitées par des tempêtes ou des zones d'algues épaisses. Ou une autre créature essaie de vous pousser ou de vous tirer sous l'eau ou d'interférer d'une autre manière avec votre nage.\n\nCette compétence remplace Discipline pour les personnages orientés force.", Category.MindBody, "isk_discipline", 10, 1, Ability.Strength, Ability.Constitution, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Acrobatics, new LearnableSkill(CustomSkill.Acrobatics, "Acrobatie", "Un jet de Dextérité (Acrobatie) couvre toute tentative de garder l'équilibre dans les situations délicates, comme essayer de courir sur une plaque de glace, rester stable sur une corde raide ou rester debout sur le pont d'un navire lors d'une forte houle.\nLe DM peut également demander un jet de Dextérité (Acrobatie) psi vous tenter d'effectuer des cascades acrobatiques, y compris des plongeons tonneaux, sauts périlleux ou des flips.\n\nCette compétence remplace Discipline pour les personnages orientés dextérité et n'accorde aucun point de CA supplémentaire.", Category.MindBody, "ife_X1Tum", 10, 1, Ability.Dexterity, Ability.Constitution, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.OpenLock, new LearnableSkill(CustomSkill.OpenLock, "Crochetage", NwSkill.FromSkillType(Skill.OpenLock).Description.ToString(), Category.MindBody, "isk_olock", 10, 1, Ability.Dexterity, Ability.Intelligence, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Escamotage, new LearnableSkill(CustomSkill.Escamotage, "Escamotage", "Un jet de Dextérité (Escamotage) couvre toute tentative un tour de passe-passe ou de supercherie manuelle, comme déposer quelque chose dans les poches de quelqu'un d'autre ou tenter de dissimuler un objet votre propre personne. Le jet de Dextérité (Escamotage) permet égaelment de déterminer si vous parvenez à délester quelqu'un de son porte-monnaie ou lui faire les poches.\n\nCette compétence remplace Vol à la Tire.", Category.MindBody, "isk_pocket", 10, 1, Ability.Dexterity, Ability.Constitution, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Stealth, new LearnableSkill(CustomSkill.Stealth, "Furtivité", "Un jet de Dextérité (Furtivité) couvre toute tentative de se dissimulation à l'oeil et à l'oreille des ennemis : pour passer furtivement sous le nez des gardes, vous échapper sans vous faire remarquer ou prendre quelqu'un par surprise.\n\nCette compétence remplace à la fois Discrétion et Déplacement silencieux.", Category.MindBody, "isk_hide", 10, 1, Ability.Dexterity, Ability.Constitution, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Concentration, new LearnableSkill(CustomSkill.Concentration, "Concentration", NwSkill.FromSkillType(Skill.Concentration).Description.ToString(), Category.MindBody, "isk_concen", 10, 1, Ability.Wisdom, Ability.Constitution, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Arcana, new LearnableSkill(CustomSkill.Arcana, "Arcane", "Un jet d'Intelligence (Arcane) couvre toute tentative de se rappeler des connaissances sur les sorts, les objets magiques, les symboles ésotériques, les traditions magiques, les plans d'existence et les habitants de ces plans.\n\nCette compétence remplace Connaissance des sorts", Category.MindBody, "isk_spellcraft", 10, 1, Ability.Intelligence, Ability.Wisdom, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.History, new LearnableSkill(CustomSkill.History, "Histoire", "Un jet d'Intelligence (Histoire) couvre toute tentative de se souvenir de traditions, d'événements historiques, de personnages légendaires, d'anciens royaumes, de conflits passés, de guerres récentes et de civilisations perdues.", Category.MindBody, "ife_X1App", 10, 1, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Nature, new LearnableSkill(CustomSkill.Nature, "Nature", "Un jet d'Intelligence (Nature) couvre toute tentative de se souvenir de connaissances sur le terrain, les plantes, les animaux, la météo et les cycles naturels.", Category.MindBody, "ife_X2GrWis1", 10, 1, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Religion, new LearnableSkill(CustomSkill.Religion, "Religion", "Un jet d'Intelligence (Religion) couvre toute tentative de se souvenir des traditions concernant les divinités, les rites, les prières, les hiérarchies religieuses, les symboles sacrés et les pratiques des cultes.", Category.MindBody, "isk_lore", 10, 1, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Medicine, new LearnableSkill(CustomSkill.Medicine, "Médecine", "Un jet d'Intelligence (Médecine) couvre toute tentative de se souvenir de détails sur le fonctionnement du corps humain, sur des remèdes ou des poisons. Cette compétence est également utilisée afin de pratiquer des actes médicaux.", Category.MindBody, "isk_heal", 10, 1, Ability.Intelligence, Ability.Wisdom, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Investigation, new LearnableSkill(CustomSkill.Investigation, "Investigation", "Un jet d'Intelligence (Investigation) couvre toute recherche d'indice et de déductions. Vous pouvez déduire l'emplacement d'un objet caché, discerner à partir de l'apparence d'une blessure quel type d'arme l'a infligée, ou déterminer le point le plus faible dans un tunnel qui pourrait provoquer son effondrement.\n L'examen d'anciens parchemins à la recherche d'un fragment de connaissance caché peut également nécessiter un jet d'Intelligence (Investigation).\n\nCette compétence remplace fouille pour la détection des pièges.", Category.MindBody, "isk_search", 10, 1, Ability.Intelligence, Ability.Wisdom, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.TrapExpertise, new LearnableSkill(CustomSkill.TrapExpertise, "Maîtrise des pièges", "Cette compétence remplace à la fois désamorçage et pose de pièges.", Category.MindBody, "isk_distrap", 10, 1, Ability.Intelligence, Ability.Dexterity, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Dressage, new LearnableSkill(CustomSkill.Dressage, "Dressage", "Un jet de Sagesse (Dressage) couvre toute tentative de calmer un animal domestique, empêcher une monture d'être effrayée ou deviner les intentions d'un animal.", Category.MindBody, "isk_aniemp", 10, 1, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.Insight, new LearnableSkill(CustomSkill.Insight, "Intuition", "Un jet de Sagesse (Intuition) couvre toute tentative de déterminer les véritables intentions d'une créature, par exemple lorsque vous souhaitez dévoiler un mensonge ou prédire le prochain mouvement de quelqu'un.\nIl s'agit principalement de glaner des indices dans le langage corporel, les changements de tons de la voix ou de manières.", Category.MindBody, "isk_listen", 10, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.Perception, new LearnableSkill(CustomSkill.Perception, "Perception", "Un jet de Sagesse (Perception) couvre toute tentative de repérer, d'entendre ou de détecter la présence de quelque chose. Il mesure votre conscience générale de votre environnement et l'acuité de vos sens.\nPar exemple, vous pouvez essayer d'entendre une conversation à travers une porte fermée, d'écouter sous une fenêtre ouverte ou d'entendre des monstres se déplacer furtivement dans la forêt.\nVous pouvez aussi essayer de repérer des choses qui sont obscurcies ou faciles à manquer, qu'il s'agisse d'orcs embusqués sur une route, de voyous cachés dans l'ombre d'une ruelle ou de bougies sous une porte secrète fermée.\n\nCette compétence remplace à la fois Détection et Perception Auditive.", Category.MindBody, "isk_spot", 10, 1, Ability.Wisdom, Ability.Intelligence, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Survival, new LearnableSkill(CustomSkill.Survival, "Survie", "Un jet de Sagesse (Survie) couvre toute tentative de suivre des traces, chasser du gibier sauvage, guider votre groupe à travers des friches gelées, identifier des signes indiquant que des ours-hiboux vivent à proximité, prédire la météo ou d'éviter les sables mouvants et autres dangers naturels.", Category.MindBody, "ife_X1CrTrap", 10, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.Deception, new LearnableSkill(CustomSkill.Deception, "Tromperie", "Un jet de Charisme (Tromperie) couvre toute tentative de cacher la vérité de manière convaincante, que ce soit verbalement ou par vos actions. Cette compétence permet de dissimuler les traces du mensonge dans votre voix et votre langage corporel.\nN'oubliez pas que dans la plupart des cas, en jeu, la crédibilité de vos paroles par rapport à la confiance accordée par vos interlocuteurs prime.\nDe base, vous ne parviendrez pas à faire croire à la garde que cette épée à deux mains pleine de sang n'est rien d'autre que l'appui d'un vieillard pour l'aider à marcher, même sur un très bon jet.", Category.MindBody, "isk_X2bluff", 10, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.Intimidation, new LearnableSkill(CustomSkill.Intimidation, "Intimidation", "Un jet de Charisme (Intimidation) couvre toute tentative d'influencer quelqu'un par des menaces manifestes, des actions hostiles et de la violence physique.\nPar exemple, essayer de soutirer des informations d'un prisonnier, convaincre des voyous de reculer devant une confrontation ou utiliser le bord d'une bouteille cassée pour convaincre un vizir ricanant de reconsidérer une décision.", Category.MindBody, "isk_X2Inti", 10, 1, Ability.Charisma, Ability.Constitution, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Performance, new LearnableSkill(CustomSkill.Performance, "Performance", "Un jet de Charisme (Performance) couvre toute tentative de ravir un public avec de la musique, de la danse, du théâtre, des contes ou toute autre forme de divertissement.", Category.MindBody, "isk_perform", 10, 1, Ability.Charisma, Ability.Dexterity, false, HandleBaseSkill));
      learnableDictionary.Add(CustomSkill.Persuasion, new LearnableSkill(CustomSkill.Persuasion, "Persuasion", "Un jet de Charisme (Persuasion) couvre toute tentative d'influencer quelqu'un ou un groupe de personnes avec du tact, des grâces sociales ou une bonne nature.\nEn règle générale, la persuasion est utilisée lorsque vous agissez de bonne foi, pour favoriser des amitiés, faire des demandes cordiales ou faire preuve d'une étiquette appropriée.\nPar exemple : convaincre un chambellan de laisser votre groupe voir le roi, négocier la paix entre des tribus en guerre ou inspirer une foule de citadins.", Category.MindBody, "isk_persuade", 10, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.Taunt, new LearnableSkill(CustomSkill.Taunt, "Raillerie", NwSkill.FromSkillType(Skill.Taunt).Description.ToString(), Category.MindBody, "isk_taunt", 10, 1, Ability.Charisma, Ability.Intelligence, false, HandleBaseSkill));

      learnableDictionary.Add(CustomSkill.Acolyte, new LearnableSkill(CustomSkill.Acolyte, "Acolyte", "google1JU5_KaJTVhoy4PyGFo5sIBIPUbLWe6tMNENQ2kR2WFY", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Wisdom, Ability.Charisma, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Anthropologist, new LearnableSkill(CustomSkill.Anthropologist, "Anthropologue", "google1KLiNxm_dHLbRh-dveP--LAfcIMHCjHhcX98a7xzZGOI", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Wisdom, Ability.Intelligence, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Archeologist, new LearnableSkill(CustomSkill.Archeologist, "Archéologue", "google1ULJttGDVkgc5vsk9DvzEJqG53Yuh_meh59T7TWkmpVs", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Intelligence, Ability.Wisdom, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.CloisteredScholar, new LearnableSkill(CustomSkill.CloisteredScholar, "Erudit", "google1jPUik90zrJ7XhNVNILd0MhaWOmLqA9XRpQ8MTnNffBA", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Wisdom, Ability.Intelligence, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Sage, new LearnableSkill(CustomSkill.Sage, "Sage", "google1AdvUpfuXxrIdv35Go4poPSFFm_4tlVvzJK5cXmR_QMw", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Wisdom, Ability.Intelligence, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Hermit, new LearnableSkill(CustomSkill.Hermit, "Ermite", "google1jPUik90zrJ7XhNVNILd0MhaWOmLqA9XRpQ8MTnNffBA", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Wisdom, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Wanderer, new LearnableSkill(CustomSkill.Wanderer, "Voyageur", "google1X2s8SwAG8I3AgDuB7Mo-yaWVpk3_AZmaXZtG2pDrdMc", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Constitution, Ability.Wisdom, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Athlete, new LearnableSkill(CustomSkill.Athlete, "Athlète", "google15h9-KjZ0sjS1yvstLjLEf3mumjJ4Xq5E-pbmgfAv9Xw", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Outlander, new LearnableSkill(CustomSkill.Outlander, "Sauvage", "google1qm3URzCigQ_xIz-BPT4kjLdXhtvyfFBI5F9ZEuxvweQ", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Wisdom, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Soldier, new LearnableSkill(CustomSkill.Soldier, "Soldat", "google1QKnLB4iEuX8pNmqPXDfV8SSSeDcsT0_9e5xMNsCLa0c", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Mercenary, new LearnableSkill(CustomSkill.Mercenary, "Mercenaire", "google1vDKqHBxFtjmhn25r0dhVruaMgzfSWxFV7D2grdPtDso", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.FolkHero, new LearnableSkill(CustomSkill.FolkHero, "Héros du peuple", "google1S4BK_DoT2tnV1EjMYvvVTrg-mR0tI-3uRBCyry5twi0", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Sailor, new LearnableSkill(CustomSkill.Sailor, "Marin", "google15sc6ymheE3JJpcg8qR_ATyB5xxj-aZAc5Ei2XFcsHpE", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Dexterity, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Shipwright, new LearnableSkill(CustomSkill.Shipwright, "Charpentier Naval", "google1pA026_rZo7PlCrpwbq3zJz2P_UCcyLxITX3EKabuXP4", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Fisher, new LearnableSkill(CustomSkill.Fisher, "Pêcheur", "google19uXzfsD2RzNYb3ledV2CMHjEXrJCCCb6uYDlpIYmxmw", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Constitution, Ability.Wisdom, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Marine, new LearnableSkill(CustomSkill.Marine, "Officier de la marine", "google1g4Hoj6WS1uAAcvpNyrTGju0Mh81H9aIecK9B---sAgk", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Criminal, new LearnableSkill(CustomSkill.Criminal, "Criminel", "google1l0m9pkIcfVy37ZjPl9wEB7s-PD2OwRt9Tco_KUhT-xI", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Dexterity, Ability.Strength, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Charlatan, new LearnableSkill(CustomSkill.Charlatan, "Charlatan", "google1ps07V3Lbp18RMIwrkYYGGyxC3tk5L8Y97zpJHw0eqO8", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Intelligence, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Smuggler, new LearnableSkill(CustomSkill.Smuggler, "Contrebandier", "google1BRYovMiish9iFnN5Q77cW14vP6bGjO14m32NzGJFiV4", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.StreetUrchin, new LearnableSkill(CustomSkill.StreetUrchin, "Gosse des rues", "google1vtim0ITSkBzl5IlPGjhKuZOTCyBYA_GHLc8M09t1IoM", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Dexterity, Ability.Charisma, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Gambler, new LearnableSkill(CustomSkill.Gambler, "Parieur", "google1HkPJH8uqCCn4k4J8HUh3g52v4TfAEFAYiFJlXYhEvuM", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Dexterity, Ability.Charisma, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Entertainer, new LearnableSkill(CustomSkill.Entertainer, "Saltimbanque", "google1Y87LKyg4DLKdlzUcfFtMj7M1rkSh6yAXx6Cb6Q16Dug", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.CityWatch, new LearnableSkill(CustomSkill.CityWatch, "Agent du guet", "google1JmIBbWSJ6oec820F-4TYeShM43XAKtOtCz2vBf2lPT4", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Investigator, new LearnableSkill(CustomSkill.Investigator, "Détective", "google1wMwqmw3jVGFAnQCDa-ayjqeKUL2QGXPyP7KIc7QoDy8", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Dexterity, Ability.Intelligence, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.KnightOfTheOrder, new LearnableSkill(CustomSkill.KnightOfTheOrder, "Chevalier de l'Ordre", "google1psb8aH-EaKINYKif3XC-MG3mtTFgh-5MYAehcJnCxl4", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Strength, Ability.Charisma, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Noble, new LearnableSkill(CustomSkill.Noble, "Noble", "google1_KAkFnH9Ydt2s0ljOGvwn-7mo_Vk5PrqtqIwJZ48k-Q", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Courtier, new LearnableSkill(CustomSkill.Courtier, "Courtisan", "google1B1C2bcvU9HBb-d2m1lnFhnHWN2a46hp3zWDb9nBZnJU", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Intelligence, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.FailedMerchant, new LearnableSkill(CustomSkill.FailedMerchant, "Marchand ruiné", "google1-2AuXuxSW1PICZWicsGUcb8Sgh_rGTVUYc6eT2zWJO8", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Intelligence, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Taken, new LearnableSkill(CustomSkill.Taken, "Captif", "google16_6ygOZjsfJF7Ngk5VZ9gSK_nlAz7kqy5-sSDlVVPxw", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Constitution, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Heir, new LearnableSkill(CustomSkill.Heir, "Héritier", "google1_D4_FywpDXAJXABkhpuwMAUkg68dsRiU07p-9Q-XSiA", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Magistrate, new LearnableSkill(CustomSkill.Magistrate, "Magistrat", "google16w21xr6HgBE159pLr1Br0mf7T00zsIzonGdXTGekIYs", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Intelligence, Ability.Wisdom, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.AdventurerScion, new LearnableSkill(CustomSkill.AdventurerScion, "Héritier d'un célèbre aventurier", "google1S7UROAImbnZdGf5Q_CkScJ_gfmfRDBcHGhQ9LpqHoAg", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Dexterity, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Refugee, new LearnableSkill(CustomSkill.Refugee, "Réfugié", "google1GCBVKWeDNR20kqOqKwIex8qlCbOltNmUpkblINEShYM", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Dexterity, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Prisoner, new LearnableSkill(CustomSkill.Prisoner, "Prisonnier", "google1Qdyz-fNuGrqI64NYaAP6wmiQd7GhUz0-hqf5F-vrYps", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Constitution, Ability.Charisma, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.HauntedOne, new LearnableSkill(CustomSkill.HauntedOne, "Tourmenté", "google1yrgm7p09M0_-Y4nDkxY7gtT1LaO1Av305zBDBoqe72M", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Constitution, Ability.Charisma, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.Faceless, new LearnableSkill(CustomSkill.Faceless, "Sans-visage", "google1ghCYrBt8e58F5QQB5gvyn294XZO7jHkdvSIYZfYgS9g", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Constitution, false, HandleBackground));
      learnableDictionary.Add(CustomSkill.SecretIdentity, new LearnableSkill(CustomSkill.SecretIdentity, "Identité Secrète", "google1EevCfGvIUXDSx2iEJwPMwN3BDuNGBMR_GSQKZqiIsYQ", Category.StartingTraits, "ife_X2GrWis1", 5, 1, Ability.Charisma, Ability.Dexterity, false, HandleBackground));

      learnableDictionary.Add(CustomSkill.Elfique, new LearnableSkill(CustomSkill.Elfique, "Elfique", "Permet de parler et comprendre l'elfique.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Nain, new LearnableSkill(CustomSkill.Nain, "Nain", "Permet de parler et comprendre le nain.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Orc, new LearnableSkill(CustomSkill.Orc, "Orc", "Permet de parler et comprendre l'orc.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Giant, new LearnableSkill(CustomSkill.Giant, "Giant", "Permet de parler et comprendre le géant.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Gobelin, new LearnableSkill(CustomSkill.Gobelin, "Gobelin", "Permet de parler et comprendre le gobelin.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Halfelin, new LearnableSkill(CustomSkill.Halfelin, "Halfelin", "Permet de parler et comprendre l'hafelin.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Abyssal, new LearnableSkill(CustomSkill.Abyssal, "Abyssal", "Permet de parler et comprendre l'abyssal.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Celestial, new LearnableSkill(CustomSkill.Celestial, "Céleste", "Permet de parler et comprendre le céleste.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Draconique, new LearnableSkill(CustomSkill.Draconique, "Draconique", "Permet de parler et comprendre le draconique.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Profond, new LearnableSkill(CustomSkill.Profond, "Profond", "Permet de parler et comprendre le langage d'Outreterre.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Infernal, new LearnableSkill(CustomSkill.Infernal, "Infernal", "Permet de parler et comprendre l'infernal.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Primordiale, new LearnableSkill(CustomSkill.Primordiale, "Primordiale", "Permet de parler et comprendre le primordial.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Sylvain, new LearnableSkill(CustomSkill.Sylvain, "Sylvain", "Permet de parler et comprendre le sylvain.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Druidique, new LearnableSkill(CustomSkill.Druidique, "Druidique", "Permet de parler et comprendre le druidique.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Voleur, new LearnableSkill(CustomSkill.Voleur, "Voleur", "Permet de parler et comprendre le langage des voleurs.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Gnome, new LearnableSkill(CustomSkill.Gnome, "Gnome", "Permet de parler et comprendre le gnome.", Category.Language, "icon_elf", 1, 763, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.ImprovedAttackBonus, new LearnableSkill(CustomSkill.ImprovedAttackBonus, "Attaque améliorée", "Augmente la pénétration d'armure d'un point par niveau.", Category.Fight, "ife_tough", 12, 2, Ability.Constitution, Ability.Dexterity, false, HandleImproveAttack));
      learnableDictionary.Add(CustomSkill.ImprovedCasterLevel, new LearnableSkill(CustomSkill.ImprovedCasterLevel, "Maîtrise des sorts", "Augmente le niveau de lanceur de sorts d'un point par niveau.", Category.Magic, "ife_tough", 12, 3, Ability.Constitution, Ability.Charisma));

      learnableDictionary.Add(CustomSkill.ImprovedLightArmorProficiency, new LearnableSkill(CustomSkill.ImprovedLightArmorProficiency, "Maîtrise de l'armure légère", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'une armure légère.", Category.Fight, "ife_armor_l", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedMediumArmorProficiency, new LearnableSkill(CustomSkill.ImprovedMediumArmorProficiency, "Maîtrise de l'armure intermédiaire", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'une armure intermédiaire.", Category.Fight, "ife_armor_m", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedHeavyArmorProficiency, new LearnableSkill(CustomSkill.ImprovedHeavyArmorProficiency, "Maîtrise de l'armure lourde", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'une armure lourde.", Category.Fight, "ife_armor_h", 20, 4, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedFullPlateProficiency, new LearnableSkill(CustomSkill.ImprovedFullPlateProficiency, "Maîtrise du harnois", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'un harnois.", Category.Fight, "ife_X2ArSkin", 20, 5, Ability.Constitution, Ability.Strength));

      learnableDictionary.Add(CustomSkill.ImprovedLightShieldProficiency, new LearnableSkill(CustomSkill.ImprovedLightShieldProficiency, "Maîtrise de la rondache", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'une rondache.", Category.Fight, "ife_sh_prof", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedMediumShieldProficiency, new LearnableSkill(CustomSkill.ImprovedMediumShieldProficiency, "Maîtrise de l'écu", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'un écu.", Category.Fight, "ife_X1DivShl", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedHeavyShieldProficiency, new LearnableSkill(CustomSkill.ImprovedHeavyShieldProficiency, "Maîtrise du pavois", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts d'un pavois.", Category.Fight, "ife_x3_pdkhshld", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedDualWieldDefenseProficiency, new LearnableSkill(CustomSkill.ImprovedDualWieldDefenseProficiency, "Défense ambidextre", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de réduction de dégâts de l'arme de votre main non directrice.", Category.Fight, "ife_x3_pdkhshld", 20, 2, Ability.Constitution, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.ImprovedClubProficiency, new LearnableSkill(CustomSkill.ImprovedClubProficiency, "Maîtrise du gourdin", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un gourdin.", Category.Fight, "ife_wepfoc_Clu", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedShortSwordProficiency, new LearnableSkill(CustomSkill.ImprovedShortSwordProficiency, "Maîtrise de l'épée courte", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une épée courte.", Category.Fight, "ife_wepfoc_Ssw", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedLightFlailProficiency, new LearnableSkill(CustomSkill.ImprovedLightFlailProficiency, "Maîtrise du fléeau léger", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un fléau léger.", Category.Fight, "ife_wepfoc_Lfl", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedShortBowProficiency, new LearnableSkill(CustomSkill.ImprovedShortBowProficiency, "Maîtrise de l'arc court", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un arc court.", Category.Fight, "ife_wepsfoc_Sbw", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedLightCrossBowProficiency, new LearnableSkill(CustomSkill.ImprovedLightCrossBowProficiency, "Maîtrise de l'arbalète courte", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une arbalète courte.", Category.Fight, "ife_wepfoc_LXb", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedLightMaceProficiency, new LearnableSkill(CustomSkill.ImprovedLightMaceProficiency, "Maîtrise de la masse légère", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une masse légère.", Category.Fight, "ife_toife_wepfoc_Lmaugh", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedDaggerProficiency, new LearnableSkill(CustomSkill.ImprovedDaggerProficiency, "Maîtrise de l'arbalète courte", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une dague.", Category.Fight, "ife_wepfoc_Dag", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedDartProficiency, new LearnableSkill(CustomSkill.ImprovedDartProficiency, "Maîtrise du dard", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un dard.", Category.Fight, "ife_wepfoc_Dar", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedUnharmedProficiency, new LearnableSkill(CustomSkill.ImprovedUnharmedProficiency, "Maîtrise du combat à mains nues", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts des dégâts à mains nues.", Category.Fight, "ife_impcrit_Una", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedLightHammerProficiency, new LearnableSkill(CustomSkill.ImprovedLightHammerProficiency, "Maîtrise du marteau léger", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un marteau léger.", Category.Fight, "ife_wepfoc_LHa", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedHandAxeProficiency, new LearnableSkill(CustomSkill.ImprovedHandAxeProficiency, "Maîtrise de la hachette", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une hachette.", Category.Fight, "ife_wepfoc_Tax", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedQuarterStaffProficiency, new LearnableSkill(CustomSkill.ImprovedQuarterStaffProficiency, "Maîtrise du bâton", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un bâton", Category.Fight, "ife_wepfoc_Sta", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedMagicStaffProficiency, new LearnableSkill(CustomSkill.ImprovedMagicStaffProficiency, "Maîtrise du bourdon", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un bourdon", Category.Fight, "ife_wepfoc_Sta", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedMorningStarProficiency, new LearnableSkill(CustomSkill.ImprovedMorningStarProficiency, "Maîtrise de l'étoile du matin", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une étoile du matin.", Category.Fight, "ife_wepfoc_Mor", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedShortSpearProficiency, new LearnableSkill(CustomSkill.ImprovedShortSpearProficiency, "Maîtrise de la lance", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une lance", Category.Fight, "ife_wepfoc_Spe", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedSlingProficiency, new LearnableSkill(CustomSkill.ImprovedSlingProficiency, "Maîtrise de la fronde", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une fronde", Category.Fight, "ife_wepfoc_SLi", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedSickleProficiency, new LearnableSkill(CustomSkill.ImprovedSickleProficiency, "Maîtrise de la serpe", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une serpe", Category.Fight, "ife_wepfoc_SLi", 20, 2, Ability.Constitution, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.ImprovedLongSwordProficiency, new LearnableSkill(CustomSkill.ImprovedLongSwordProficiency, "Maîtrise de l'épée longue", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une épée longue.", Category.Fight, "ife_wepfoc_LSw", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedBattleAxeProficiency, new LearnableSkill(CustomSkill.ImprovedBattleAxeProficiency, "Maîtrise de la hache de guerre", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une hache de guerre.", Category.Fight, "ife_wepfoc_Bax", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedWarHammerProficiency, new LearnableSkill(CustomSkill.ImprovedWarHammerProficiency, "Maîtrise du marteau de guerre", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un marteau de guerre.", Category.Fight, "ife_wepfoc_Wha", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedLongBowProficiency, new LearnableSkill(CustomSkill.ImprovedLongBowProficiency, "Maîtrise de l'arc long", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un arc long.", Category.Fight, "ife_wepfoc_Lbw", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedHeavyCrossbowProficiency, new LearnableSkill(CustomSkill.ImprovedHeavyCrossbowProficiency, "Maîtrise de l'arbalète lourde", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une arbalète lourde.", Category.Fight, "ife_wepfoc_Hxb", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedHalberdProficiency, new LearnableSkill(CustomSkill.ImprovedHalberdProficiency, "Maîtrise de la hallebarde", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une hallebarde.", Category.Fight, "ife_wepfoc_Hal", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedGreatSwordProficiency, new LearnableSkill(CustomSkill.ImprovedGreatSwordProficiency, "Maîtrise de l'épée à deux mains", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une épée à deux mains.", Category.Fight, "ife_wepfoc_Gsw", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedGreatAxeProficiency, new LearnableSkill(CustomSkill.ImprovedGreatAxeProficiency, "Maîtrise de la grande hache", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une hache à deux mains.", Category.Fight, "ife_wepfoc_Hax", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedHeavyFlailProficiency, new LearnableSkill(CustomSkill.ImprovedHeavyFlailProficiency, "Maîtrise du fléau lourd", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un fléau lourd.", Category.Fight, "ife_wepfoc_Hfl", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedRapierProficiency, new LearnableSkill(CustomSkill.ImprovedRapierProficiency, "Maîtrise de la rapière", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une rapière.", Category.Fight, "ife_wepfoc_Rap", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedScimitarProficiency, new LearnableSkill(CustomSkill.ImprovedScimitarProficiency, "Maîtrise du cimeterre", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un cimeterre.", Category.Fight, "ife_wepfoc_Sci", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedThrowingAxeProficiency, new LearnableSkill(CustomSkill.ImprovedThrowingAxeProficiency, "Maîtrise de la hache de lancer", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une hache de lancer.", Category.Fight, "ife_wepfoc_Tax", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedTridentProficiency, new LearnableSkill(CustomSkill.ImprovedTridentProficiency, "Maîtrise du trident", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un trident.", Category.Fight, "ife_X2WFTri", 20, 3, Ability.Constitution, Ability.Strength));

      learnableDictionary.Add(CustomSkill.ImprovedBastardSwordProficiency, new LearnableSkill(CustomSkill.ImprovedBastardSwordProficiency, "Maîtrise de l'épée bâtarde", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une épée bâtarde.", Category.Fight, "ife_wepfoc_Bsw", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedTwoBladedSwordProficiency, new LearnableSkill(CustomSkill.ImprovedTwoBladedSwordProficiency, "Maîtrise de l'épée double", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une épée double.", Category.Fight, "ife_wepfoc_2sw", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedDireMaceProficiency, new LearnableSkill(CustomSkill.ImprovedDireMaceProficiency, "Maîtrise de la double masse", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une double masse.", Category.Fight, "ife_wepfoc_Dma", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedDoubleAxeProficiency, new LearnableSkill(CustomSkill.ImprovedDoubleAxeProficiency, "Maîtrise de la double hache", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une double hache.", Category.Fight, "ife_wepfoc_Dax", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedKamaProficiency, new LearnableSkill(CustomSkill.ImprovedKamaProficiency, "Maîtrise du kama", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un kama.", Category.Fight, "ife_wepfoc_Kam", 20, 5, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedKukriProficiency, new LearnableSkill(CustomSkill.ImprovedKukriProficiency, "Maîtrise du kukri", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un kukri.", Category.Fight, "ife_wepfoc_Kuk", 20, 5, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedKatanaProficiency, new LearnableSkill(CustomSkill.ImprovedKatanaProficiency, "Maîtrise du katana", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un katana.", Category.Fight, "ife_wepfoc_Kat", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedScytheProficiency, new LearnableSkill(CustomSkill.ImprovedScytheProficiency, "Maîtrise de la faux", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une faux.", Category.Fight, "ife_wepfoc_Scy", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedDwarvenWarAxeProficiency, new LearnableSkill(CustomSkill.ImprovedDwarvenWarAxeProficiency, "Maîtrise de la hache naine", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'une hache naine.", Category.Fight, "ife_X2WFDWAx", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ImprovedWhipProficiency, new LearnableSkill(CustomSkill.ImprovedWhipProficiency, "Maîtrise du fouet", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un fouet.", Category.Fight, "ife_X2WFWhip", 20, 5, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ImprovedShurikenProficiency, new LearnableSkill(CustomSkill.ImprovedShurikenProficiency, "Maîtrise du shuriken", "Chaque niveau permet de bénéficier de 10 % de la valeur maximale de dégâts d'un shuriken.", Category.Fight, "ife_wepfoc_Shu", 20, 5, Ability.Constitution, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.ClubCriticalScience, new LearnableSkill(CustomSkill.ClubCriticalScience, "Science du gourdin", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un gourdin.", Category.Fight, "ife_impcrit_Clu", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ShortSwordCriticalScience, new LearnableSkill(CustomSkill.ShortSwordCriticalScience, "Science de l'épée courte", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une épée courte.", Category.Fight, "ife_impcrit_Ssw", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.LightFlailCriticalScience, new LearnableSkill(CustomSkill.LightFlailCriticalScience, "Science du fléeau léger", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un fléau léger.", Category.Fight, "ife_impcrit_Lfl", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ShortBowCriticalScience, new LearnableSkill(CustomSkill.ShortBowCriticalScience, "Science de l'arc court", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un arc court.", Category.Fight, "ife_impcrit_SBw", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.LightCrossBowCriticalScience, new LearnableSkill(CustomSkill.LightCrossBowCriticalScience, "Science de l'arbalète légère", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une arbalète légère.", Category.Fight, "ife_impcrit_Lxb", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.LightMaceCriticalScience, new LearnableSkill(CustomSkill.LightMaceCriticalScience, "Science de la masse légère", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une masse légère.", Category.Fight, "ife_impcrit_Lma", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.DaggerCriticalScience, new LearnableSkill(CustomSkill.DaggerCriticalScience, "Science de l'arbalète courte", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une dague.", Category.Fight, "ife_impcrit_Dag", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.DartCriticalScience, new LearnableSkill(CustomSkill.DartCriticalScience, "Science du dard", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un dard.", Category.Fight, "ife_impcrit_Dar", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.UnharmedCriticalScience, new LearnableSkill(CustomSkill.UnharmedCriticalScience, "Science du combat à mains nues", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un dard.", Category.Fight, "ife_tough", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.LightHammerCriticalScience, new LearnableSkill(CustomSkill.LightHammerCriticalScience, "Science du marteau léger", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un marteau léger.", Category.Fight, "ife_impcrit_Lha", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.HandAxeCriticalScience, new LearnableSkill(CustomSkill.HandAxeCriticalScience, "Science de la hachette", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une hachette.", Category.Fight, "ife_impcrit_Hax", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.QuarterStaffCriticalScience, new LearnableSkill(CustomSkill.QuarterStaffCriticalScience, "Science du bâton", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un bâton", Category.Fight, "ife_impcrit_Sta", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.MagicStaffCriticalScience, new LearnableSkill(CustomSkill.MagicStaffCriticalScience, "Science du bourdon", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un bourdon", Category.Fight, "ife_impcrit_Sta", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.MorningStarCriticalScience, new LearnableSkill(CustomSkill.MorningStarCriticalScience, "Science de l'étoile du matin", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une étoile du matin.", Category.Fight, "ife_impcrit_Mor", 20, 2, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ShortSpearCriticalScience, new LearnableSkill(CustomSkill.ShortSpearCriticalScience, "Science de la lance", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une lance", Category.Fight, "ife_impcrit_Spe", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.SlingCriticalScience, new LearnableSkill(CustomSkill.SlingCriticalScience, "Science de la fronde", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une fronde", Category.Fight, "ife_impcrit_Sli", 20, 2, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.SickleCriticalScience, new LearnableSkill(CustomSkill.SickleCriticalScience, "Science de la serpe", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une serpe", Category.Fight, "ife_impcrit_Sic", 20, 2, Ability.Constitution, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.LongSwordCriticalScience, new LearnableSkill(CustomSkill.LongSwordCriticalScience, "Science de l'épée longue", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une épée longue.", Category.Fight, "ife_impcrit_Lsw", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.BattleAxeCriticalScience, new LearnableSkill(CustomSkill.BattleAxeCriticalScience, "Science de la hache de guerre", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une hache de guerre.", Category.Fight, "ife_impcrit_BAx", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.WarHammerCriticalScience, new LearnableSkill(CustomSkill.WarHammerCriticalScience, "Science du marteau de guerre", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un marteau de guerre.", Category.Fight, "ife_impcrit_Wha", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.LongBowCriticalScience, new LearnableSkill(CustomSkill.LongBowCriticalScience, "Science de l'arc long", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un arc long.", Category.Fight, "ife_impcrit_LBw", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.HeavyCrossbowCriticalScience, new LearnableSkill(CustomSkill.HeavyCrossbowCriticalScience, "Science de l'arbalète lourde", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une arbalète lourde.", Category.Fight, "ife_impcrit_Hxb", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.HalberdCriticalScience, new LearnableSkill(CustomSkill.HalberdCriticalScience, "Science de la hallebarde", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une hallebarde.", Category.Fight, "ife_impcrit_Hal", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.GreatSwordCriticalScience, new LearnableSkill(CustomSkill.GreatSwordCriticalScience, "Science de l'épée à deux mains", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une épée à deux mains.", Category.Fight, "ife_impcrit_GSw", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.GreatAxeCriticalScience, new LearnableSkill(CustomSkill.GreatAxeCriticalScience, "Science de la grande hache", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une hache à deux mains.", Category.Fight, "ife_impcrit_Gax", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.HeavyFlailCriticalScience, new LearnableSkill(CustomSkill.HeavyFlailCriticalScience, "Science du fléau lourd", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un fléau lourd.", Category.Fight, "ife_impcrit_HFl", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.RapierCriticalScience, new LearnableSkill(CustomSkill.RapierCriticalScience, "Science de la rapière", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une rapière.", Category.Fight, "ife_impcrit_Rap", 20, 3, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ScimitarCriticalScience, new LearnableSkill(CustomSkill.ScimitarCriticalScience, "Science du cimeterre", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un cimeterre.", Category.Fight, "ife_impcrit_Sci", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ThrowingAxeCriticalScience, new LearnableSkill(CustomSkill.ThrowingAxeCriticalScience, "Science de la hache de lancer", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une hache de lancer.", Category.Fight, "ife_impcrit_Tax", 20, 3, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.TridentCriticalScience, new LearnableSkill(CustomSkill.TridentCriticalScience, "Science du trident", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un trident.", Category.Fight, "ife_X2CrtTri", 20, 3, Ability.Constitution, Ability.Strength));

      learnableDictionary.Add(CustomSkill.BastardSwordCriticalScience, new LearnableSkill(CustomSkill.BastardSwordCriticalScience, "Science de l'épée bâtarde", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une épée bâtarde.", Category.Fight, "ife_Impcrit_Bsw", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.TwoBladedSwordCriticalScience, new LearnableSkill(CustomSkill.TwoBladedSwordCriticalScience, "Science de l'épée double", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une épée double.", Category.Fight, "ife_impcrit_2sw", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.DireMaceCriticalScience, new LearnableSkill(CustomSkill.DireMaceCriticalScience, "Science de la double masse", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une double masse.", Category.Fight, "ife_impcrit_Dma", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.DoubleAxeCriticalScience, new LearnableSkill(CustomSkill.DoubleAxeCriticalScience, "Science de la double hache", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une double hache.", Category.Fight, "ife_impcrit_Dax", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.KamaCriticalScience, new LearnableSkill(CustomSkill.KamaCriticalScience, "Science du kama", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un kama.", Category.Fight, "ife_impcrit_Kam", 20, 5, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.KukriCriticalScience, new LearnableSkill(CustomSkill.KukriCriticalScience, "Science du kukri", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un kukri.", Category.Fight, "ife_impcrit_kuk", 20, 5, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.KatanaCriticalScience, new LearnableSkill(CustomSkill.KatanaCriticalScience, "Science du katana", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un katana.", Category.Fight, "ife_impcrit_Kat", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ScytheCriticalScience, new LearnableSkill(CustomSkill.ScytheCriticalScience, "Science de la faux", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une faux.", Category.Fight, "ife_impcrit_Scy", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.DwarvenWarAxeCriticalScience, new LearnableSkill(CustomSkill.DwarvenWarAxeCriticalScience, "Science de la hache naine", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec une hache naine.", Category.Fight, "ife_X2WFDWAx", 20, 5, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.WhipCriticalScience, new LearnableSkill(CustomSkill.WhipCriticalScience, "Science du fouet", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un fouet.", Category.Fight, "ife_X2CrtWhip", 20, 5, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ShurikenCriticalScience, new LearnableSkill(CustomSkill.ShurikenCriticalScience, "Science du shuriken", "Chaque niveau permet de bénéficier de 1 % de chance de critique supplémentaire avec un shuriken.", Category.Fight, "ife_impcrit_Shu", 20, 5, Ability.Constitution, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.TwoWeaponFighting, new LearnableSkill(CustomSkill.TwoWeaponFighting, "Combat à deux armes", NwFeat.FromFeatType(Feat.TwoWeaponFighting).Description.ToString().ToString(), Category.Fight, "ife_twoweap", 1, 2, Ability.Dexterity, Ability.Constitution, true));
      learnableDictionary.Add(CustomSkill.WeaponFinesse, new LearnableSkill(CustomSkill.WeaponFinesse, "Finesse", NwFeat.FromFeatType(Feat.WeaponFinesse).Description.ToString().ToString(), Category.Fight, "ife_finesse", 1, 2, Ability.Dexterity, Ability.Constitution, true));

      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot0, new LearnableSkill(CustomSkill.ImprovedSpellSlot0, "Emplacement Cercle 0", "Augmente le nombre d'emplacements de sorts de cercle 0 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsC1", 10, 1, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot1, new LearnableSkill(CustomSkill.ImprovedSpellSlot1, "Emplacement Cercle 1", "Augmente le nombre d'emplacements de sorts de cercle 1 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsA1", 10, 2, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot2, new LearnableSkill(CustomSkill.ImprovedSpellSlot2, "Emplacement Cercle 2", "Augmente le nombre d'emplacements de sorts de cercle 2 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsF1", 10, 3, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot3, new LearnableSkill(CustomSkill.ImprovedSpellSlot3, "Emplacement Cercle 3", "Augmente le nombre d'emplacements de sorts de cercle 3 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsE1", 10, 4, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot4, new LearnableSkill(CustomSkill.ImprovedSpellSlot4, "Emplacement Cercle 4", "Augmente le nombre d'emplacements de sorts de cercle 4 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsS1", 10, 5, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot5, new LearnableSkill(CustomSkill.ImprovedSpellSlot5, "Emplacement Cercle 5", "Augmente le nombre d'emplacements de sorts de cercle 5 disponibles d'un par niveau.", Category.Magic, "ife_X2EpSkFSpCr", 10, 6, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot6, new LearnableSkill(CustomSkill.ImprovedSpellSlot6, "Emplacement Cercle 6", "Augmente le nombre d'emplacements de sorts de cercle 6 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicFort", 10, 7, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot7, new LearnableSkill(CustomSkill.ImprovedSpellSlot7, "Emplacement Cercle 7", "Augmente le nombre d'emplacements de sorts de cercle 7 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicRefl", 10, 8, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot8, new LearnableSkill(CustomSkill.ImprovedSpellSlot8, "Emplacement Cercle 8", "Augmente le nombre d'emplacements de sorts de cercle 8 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicProw", 10, 9, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot9, new LearnableSkill(CustomSkill.ImprovedSpellSlot9, "Emplacement Cercle 9", "Augmente le nombre d'emplacements de sorts de cercle 9 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicRepu", 10, 10, Ability.Charisma, Ability.Constitution, false, HandleAddedSpellSlot));

      learnableDictionary.Add(CustomSkill.OreDetection, new LearnableSkill(CustomSkill.OreDetection, "Détection de matéria minérale", "Permet l'utilisation du détecteur de matéria afin de trouver des concentrations de minerais riches en Substance.\nChaque niveau augmente de 1 la distance de révélation du filon.\nChaque niveau augmente de 5 % la chance de révélation du filon.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon et de sa proximité de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.WoodDetection, new LearnableSkill(CustomSkill.WoodDetection, "Détection de matéria arboricole", "Permet l'utilisation du détecteur de matéria afin de trouver des concentrations d'arbres riches en Substance.\nChaque niveau augmente de 1 la distance de révélation du filon.\nChaque niveau augmente de 5 % la chance de révélation du filon.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon et de sa proximité de 5 %", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.PeltDetection, new LearnableSkill(CustomSkill.PeltDetection, "Détection de matéria animale", "Permet l'utilisation du détecteur de matéria afin d'identifier des créatures disposant d'une forte concentration de Substance.\nChaque niveau augmente de 1 la distance de révélation du filon.\nChaque niveau augmente de 5 % la chance de révélation du filon.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon et de sa proximité de 5 %", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.OreDetectionSpeed, new LearnableSkill(CustomSkill.OreDetectionSpeed, "Détection minérale accélérée", "Chaque niveau diminue de 5 % le temps de recherche nécessaire à la détection de matéria minérale.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionSpeed, new LearnableSkill(CustomSkill.WoodDetectionSpeed, "Détection arboricole accélérée", "Chaque niveau diminue de 5 % le temps de recherche nécessaire à la détection de matéria arboricole.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionSpeed, new LearnableSkill(CustomSkill.PeltDetectionSpeed, "Détection animale accélérée", "Chaque niveau diminue de 5 % le temps de recherche nécessaire à la détection de matéria animale.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionAccuracy, new LearnableSkill(CustomSkill.OreDetectionAccuracy, "Détection minérale précise", "Chaque niveau augmente de 5 % la précision d'estimation de proximité de la masse la plus concentrée en matéria minérale.", Category.Craft, "ife_X2EpicRepu", 15, 2, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionAccuracy, new LearnableSkill(CustomSkill.WoodDetectionAccuracy, "Détection arboricole précise", "Chaque niveau augmente de 5 % la précision d'estimation de proximité de la plus grande masse de matéria arboricole.", Category.Craft, "ife_X2EpicRepu", 15, 2, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionAccuracy, new LearnableSkill(CustomSkill.PeltDetectionAccuracy, "Détection animale précise", "Chaque niveau augmente de 5 % la précision d'estimation de proximité de la plus grande masse de matéria animale.", Category.Craft, "ife_X2EpicRepu", 15, 2, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionOrientation, new LearnableSkill(CustomSkill.OreDetectionOrientation, "Détection minérale orientée", "Permet de déterminer la direction vers laquele se trouve la plus grande masse de matéria minérale.\nChaque niveau augmente la précision de 5 %.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionOrientation, new LearnableSkill(CustomSkill.WoodDetectionOrientation, "Détection arboricole orientée", "Permet de déterminer la direction vers laquele se trouve la plus grande masse de matéria arboricole.\nChaque niveau augmente la précision de 5 %.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionOrientation, new LearnableSkill(CustomSkill.PeltDetectionOrientation, "Détection animale orientée", "Permet de déterminer la direction vers laquele se trouve la plus grande masse de matéria animale.\nChaque niveau augmente la précision de 5 %.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionEstimation, new LearnableSkill(CustomSkill.OreDetectionEstimation, "Estimation minérale", "Chaque niveau augmente de 5 % la précision de l'estimation de masse de matéria d'un filon minérale.", Category.Craft, "ife_X2EpicRepu", 15, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionEstimation, new LearnableSkill(CustomSkill.WoodDetectionEstimation, "Estimation arboricole", "Chaque niveau augmente de 5 % la précision de l'estimation de masse de matéria d'un filon arboricole.", Category.Craft, "ife_X2EpicRepu", 15, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionEstimation, new LearnableSkill(CustomSkill.PeltDetectionEstimation, "Estimation animale", "Chaque niveau augmente de 5 % la précision de l'estimation de masse de matéria d'un filon animal.", Category.Craft, "ife_X2EpicRepu", 15, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionAdvanced, new LearnableSkill(CustomSkill.OreDetectionAdvanced, "Détection minérale avancée", "Chaque niveau augmente de 1 la distance maximale de révélation des filons minéraux.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodDetectionAdvanced, new LearnableSkill(CustomSkill.WoodDetectionAdvanced, "Détection arboricole avancée", "Chaque niveau augmente de 1 la distance maximale de révélation des filons arboricoles.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.PeltDetectionAdvanced, new LearnableSkill(CustomSkill.PeltDetectionAdvanced, "Détection animale avancée", "Chaque niveau augmente de 1 la distance maximale de révélation des filons animaux.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.OreDetectionMastery, new LearnableSkill(CustomSkill.OreDetectionMastery, "Détection minérale avancée", "Chaque niveau augmente de 5 % la chance de révélation de filons minéraux.\nLes filons de qualité supérieure sont généralement moins abondants et donc plus difficiles à révéler.", Category.Craft, "ife_X2EpicRepu", 40, 2, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodDetectionMastery, new LearnableSkill(CustomSkill.WoodDetectionMastery, "Détection arboricole avancée", "Chaque niveau augmente de 5 % la chance de révélation de filons arboricoles.\nLes filons de qualité supérieure sont généralement moins abondants et donc plus difficiles à révéler.", Category.Craft, "ife_X2EpicRepu", 40, 2, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.PeltDetectionMastery, new LearnableSkill(CustomSkill.PeltDetectionMastery, "Détection animale avancée", "Chaque niveau augmente de 5 % la chance de révélation de filons animaux.\nLes filons de qualité supérieure sont généralement moins abondants et donc plus difficiles à révéler.", Category.Craft, "ife_X2EpicRepu", 40, 2, Ability.Constitution, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.MineralExtraction, new LearnableSkill(CustomSkill.MineralExtraction, "Extraction minérale", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une veine minérale ainsi que la vitesse d'extraction.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.MineralExtractionSpeed, new LearnableSkill(CustomSkill.MineralExtractionSpeed, "Extraction minérale accélérée", "Chaque niveau augmente de 5 % la vitesse d'extraction d'une veine minérale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.MineralExtractionYield, new LearnableSkill(CustomSkill.MineralExtractionYield, "Extraction minérale améliorée", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une veine minérale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.MineralExtractionCriticalSuccess, new LearnableSkill(CustomSkill.MineralExtractionCriticalSuccess, "Extraction minérale avancée", "Chaque niveau augmente de 1 % la probabilité de réussir d'extraire une matéria de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.MineralExtractionCriticalFailure, new LearnableSkill(CustomSkill.MineralExtractionCriticalFailure, "Extraction minérale sécurisée", "Chaque niveau réduit de 1 % le risque d'extraire une matéria de qualité inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Strength, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.WoodExtraction, new LearnableSkill(CustomSkill.WoodExtraction, "Extraction arboricole", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une veine arboricole ainsi que la vitesse d'extraction.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionSpeed, new LearnableSkill(CustomSkill.WoodExtractionSpeed, "Extraction arboricole accélérée", "Chaque niveau augmente de 5 % la vitesse d'extraction d'une veine arboricole.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionYield, new LearnableSkill(CustomSkill.WoodExtractionYield, "Extraction arboricole améliorée", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une veine arboricole.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionCriticalSuccess, new LearnableSkill(CustomSkill.WoodExtractionCriticalSuccess, "Extraction arboricole avancée", "Chaque niveau augmente de 1 % la probabilité de réussir d'extraire une matéria de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionCriticalFailure, new LearnableSkill(CustomSkill.WoodExtractionCriticalFailure, "Extraction arboricole sécurisée", "Chaque niveau réduit de 1 % le risque d'extraire une matéria de qualité inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Strength, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.PeltExtraction, new LearnableSkill(CustomSkill.PeltExtraction, "Extraction animale", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une veine animale ainsi que la vitesse d'extraction.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionSpeed, new LearnableSkill(CustomSkill.PeltExtractionSpeed, "Extraction animale accélérée", "Chaque niveau augmente de 5 % la vitesse d'extraction d'une veine animale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionYield, new LearnableSkill(CustomSkill.PeltExtractionYield, "Extraction animale améliorée", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une veine animale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionCriticalSuccess, new LearnableSkill(CustomSkill.PeltExtractionCriticalSuccess, "Extraction animale avancée", "Chaque niveau augmente de 1 % la probabilité de réussir d'extraire une matéria de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionCriticalFailure, new LearnableSkill(CustomSkill.PeltExtractionCriticalFailure, "Extraction animale sécurisée", "Chaque niveau réduit de 1 % le risque d'extraire une matéria de qualité inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Dexterity, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.ReprocessingOre, new LearnableSkill(CustomSkill.ReprocessingOre, "Raffinage minéral", "Réduit la quantité de matéria minérale gachée lors du raffinage de 3 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingOreEfficiency, new LearnableSkill(CustomSkill.ReprocessingOreEfficiency, "Raffinage minéral efficace", "Réduit la quantité de matéria minérale gachée lors du raffinage de 2 % par niveau..", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingOreExpertise, new LearnableSkill(CustomSkill.ReprocessingOreExpertise, "Raffinage minéral expert", "Réduit de 12 % par niveau la quantité de matéria minérale gachée liée au niveau de qualité (base 25 % par niveau de qualité).", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingOreLuck, new LearnableSkill(CustomSkill.ReprocessingOreLuck, "Chance du Raffineur minéral", "Donne 1 % de chance par niveau de raffiner une matéria minérale brute en une matéria de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingWood, new LearnableSkill(CustomSkill.ReprocessingWood, "Raffinage arboricole", "Réduit la quantité de matérial arboricole gachée lors du raffinage de 3 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingWoodEfficiency, new LearnableSkill(CustomSkill.ReprocessingWoodEfficiency, "Raffinage arboricole efficace", "Réduit la quantité de matéria arboricole gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingWoodExpertise, new LearnableSkill(CustomSkill.ReprocessingWoodExpertise, "Raffinage arboricole expert", "Réduit de 12 % par niveau la quantité de matéria aboricole gachée liée au niveau de qualité (base 25 % par niveau de qualité).", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingWoodLuck, new LearnableSkill(CustomSkill.ReprocessingWoodLuck, "Chance du Raffineur arboricole", "Donne 1 % de chance par niveau de raffiner une matéria arboricole brute en une matéria de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingPelt, new LearnableSkill(CustomSkill.ReprocessingOre, "Raffinage animal", "Réduit la quantité de matérial animale gachée lors du raffinage de 3 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingPeltEfficiency, new LearnableSkill(CustomSkill.ReprocessingPeltEfficiency, "Raffinage animal efficace", "Réduit la quantité de matéria animale gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingPeltExpertise, new LearnableSkill(CustomSkill.ReprocessingPeltExpertise, "Raffinage animal expert", "Réduit de 12 % par niveau la quantité de matéria animale gachée liée au niveau de qualité (base 25 % par niveau de qualité).", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ReprocessingPeltLuck, new LearnableSkill(CustomSkill.ReprocessingPeltLuck, "Chance du Raffineur animal", "Donne 1 % de chance par niveau de raffiner une matéria animale brute en une matéria de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.ReprocessingGrade1Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade1Expertise, "Raffinage expert qualité 1", "Réduit la quantité de matéria de qualité 1 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade2Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade2Expertise, "Raffinage expert qualité 2", "Réduit la quantité de matéria de qualité 2 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade3Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade3Expertise, "Raffinage expert qualité 3", "Réduit la quantité de matéria de qualité 3 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade4Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade4Expertise, "Raffinage expert qualité 4", "Réduit la quantité de matéria de qualité 4 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade5Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade5Expertise, "Raffinage expert qualité 5", "Réduit la quantité de matéria de qualité 5 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade6Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade6Expertise, "Raffinage expert qualité 6", "Réduit la quantité de matéria de qualité 6 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 6, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade7Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade7Expertise, "Raffinage expert qualité 7", "Réduit la quantité de matéria de qualité 7 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 7, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReprocessingGrade8Expertise, new LearnableSkill(CustomSkill.ReprocessingGrade8Expertise, "Raffinage expert qualité 8", "Réduit la quantité de matéria de qualité 8 gachée lors du raffinage de 2 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 8, Ability.Dexterity, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.MateriaGradeConcentration, new LearnableSkill(CustomSkill.MateriaGradeConcentration, "Concentration de matéria", "Réduit la quantité de matéria nécessaire pour amorcer une concentration de 5 % par niveau.", Category.Craft, "ife_X2EpicRepu", 20, 2, Ability.Strength, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.ConnectionsGates, new LearnableSkill(CustomSkill.ConnectionsGates, "Relations Quartier des Portes", "Diminue les taxes imposées aux Portes de la Cité de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ConnectionsGovernment, new LearnableSkill(CustomSkill.ConnectionsGovernment, "Relations Quartier du Gouvernement", "Diminue les taxes imposées au Quartier du Gouvernement de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ConnectionsPromenade, new LearnableSkill(CustomSkill.ConnectionsPromenade, "Relations Quartier de la Promenade", "Diminue les taxes imposées au Quartier de la Promenade de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ConnectionsTemple, new LearnableSkill(CustomSkill.ConnectionsTemple, "Relations Quartier des Temples", "Diminue les taxes imposées au Quartier des Temples de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.BlueprintCopy, new LearnableSkill(CustomSkill.BlueprintCopy, "Copie de patron", "Permet la copie de patrons originaux. Diminue le temps de copie 5 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.BlueprintEfficiency, new LearnableSkill(CustomSkill.BlueprintEfficiency, "Copie efficace", "Augmente le nombre d'utilisations des patrons que vous copiez de 1 par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.BlueprintEconomy, new LearnableSkill(CustomSkill.BlueprintEconomy, "Artisan économe", "Donne une chance de 5 % par niveau de ne pas consommer d'utilisation lors d'un travail artisanal faisant usage d'une copie de patron.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.BlueprintResearch, new LearnableSkill(CustomSkill.BlueprintResearch, "Recherche en efficacité", "Permet de rechercher une amélioration en efficacité sur des patrons originaux.\nDiminue le temps de recherche de 5 % par niveau.\nCe type de recherche permet de diminuer le temps de fabrication, de réparation de l'objet correspondant.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.BlueprintMetallurgy, new LearnableSkill(CustomSkill.BlueprintMetallurgy, "Recherche en rendement", "Permet de recherche une amélioration de rendement sur des patrons originaux.\nDiminue le temps de recherche de 5 % par niveau.\nCetype de recherche permet de diminuer le coût de fabrication de l'objet correspondant.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.AdvancedCraft, new LearnableSkill(CustomSkill.AdvancedCraft, "Artisanat avancé", "Diminue de 3 % supplémentaires les temps de recherche sur des patrons originaux.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.Blacksmith, new LearnableSkill(CustomSkill.Blacksmith, "Forgeron", "Diminue le temps de fabrication, de réparation et le coût en matéria d'un objet de la forge de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.Woodworker, new LearnableSkill(CustomSkill.Woodworker, "Ebéniste", "Diminue le temps de fabrication, de réparation et le coût en matéria d'un objet de la scierie de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Strength));
      learnableDictionary.Add(CustomSkill.Tanner, new LearnableSkill(CustomSkill.Tanner, "Tanneur", "Diminue le temps de fabrication, de réparation et le coût en matéria d'un objet de la tannerie de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ArtisanExceptionnel, new LearnableSkill(CustomSkill.ArtisanExceptionnel, "Artisan d'exception", "Augmente de 1 % par niveau la chance de parvenir à produire un objet avec un emplacement d'enchantement supplémentaire.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ArtisanApplique, new LearnableSkill(CustomSkill.ArtisanApplique, "Artisan appliqué", "Augmente de 3 % par niveau la chance d'augmenter la durabilité d'un objet lors de sa fabrication.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ArtisanFocus, new LearnableSkill(CustomSkill.ArtisanFocus, "Artisan concentré", "Augmente de 5 % le gain de durabilité obtenu lors de l'activation de la compétence artisan appliqué.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.Renforcement, new LearnableSkill(CustomSkill.Renforcement, "Renforcement", "Permet d'augmenter la durabilité d'un objet de 5 % par renforcement. Cumulable 10 fois.\n\nDiminue le temps de travail nécessaire de 5 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.Recycler, new LearnableSkill(CustomSkill.Recycler, "Recyclage", "Permet de mettre en pièces les objets afin d'extraire une fraction de la matéria brute qu'ils contiennent.\n\n Diminue le temps nécessaire au recyclage et augmente le rendement de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.RecyclerFast, new LearnableSkill(CustomSkill.RecyclerFast, "Recyclage accéléré", "Permet d'effectuer une tâche de recyclage 1 % plus rapidement par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.RecyclerExpert, new LearnableSkill(CustomSkill.RecyclerExpert, "Recyclage expert", "Augmente le rendement du recyclage de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Repair, new LearnableSkill(CustomSkill.Repair, "Réparation", "Permet de réparer les objets.\n\n Diminue le temps et le coût en matéria nécessaires à la réparation.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.RepairFast, new LearnableSkill(CustomSkill.RepairFast, "Réparation accélérée", "Permet d'effectuer une tâche de réparation 1 % plus rapidement par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.RepairExpert, new LearnableSkill(CustomSkill.RepairExpert, "Réparation experte", "Diminue le coût en matéria d'une réparation de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.RepairCareful, new LearnableSkill(CustomSkill.RepairCareful, "Réparation prudente", "Diminue la perte de durabilité maximale à chaque réparation de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.SurchargeArcanique, new LearnableSkill(CustomSkill.SurchargeArcanique, "Surcharge arcanique", "Permet de forcer l'ajout d'emplacements d'enchantements sur un objet au risque de le briser.\n\nAugmente de 1 % par niveau la chance de parvenir à forcer l'ajout d'un emplacement d'enchantement supplémentaire.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Charisma, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.SurchargeControlee, new LearnableSkill(CustomSkill.SurchargeControlee, "Surcharge contrôlée", "Augmente de 5 % par niveau la chance de conserver l'objet intact lors de l'échec d'une tentative de surcharge.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Wisdom, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.Enchanteur, new LearnableSkill(CustomSkill.Enchanteur, "Enchanteur", "Permet l'ajout d'effets magiques permanents aux objets disposant d'emplacements libres.\n\nRéduit de 1 % par niveau le coût et le temps nécessaire pour enchanter un objet.", Category.Craft, "ife_X2EpicRepu", 20, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.EnchanteurExpert, new LearnableSkill(CustomSkill.EnchanteurExpert, "Enchanteur Expert", "Augmente de 2 % par niveau la chance d'incanter un enchantement plus puissant.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.EnchanteurChanceux, new LearnableSkill(CustomSkill.EnchanteurChanceux, "Enchanteur Chanceux", "Augmente de 1 % par niveau la chance de ne pas consommer d'emplacement lors d'un enchantement.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomSkill.CombattantPrecautionneux, new LearnableSkill(CustomSkill.CombattantPrecautionneux, "Combattant Précautionneux", "Diminue de 1 % par niveau le risque d'usure des objets.", Category.Fight, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.ImprovedDodge, new LearnableSkill(CustomSkill.ImprovedDodge, "Esquive améliorée", "Augmente la probabilité d'esquiver une attaque de 2% par niveau.", Category.Fight, "ife_X2EpicRepu", 10, 3, Ability.Dexterity, Ability.Constitution, false, null, new Dictionary<Ability, int>() { { Ability.Dexterity, 22 } }));

      learnableDictionary.Add(CustomSkill.CraftOnHandedMeleeWeapon, new LearnableSkill(CustomSkill.CraftOnHandedMeleeWeapon, "Fourbisseur léger", "Diminue le temps de fabrication, de réparation et le coût en matériaux des armes de mêlée à une main de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftTwoHandedMeleeWeapon, new LearnableSkill(CustomSkill.CraftTwoHandedMeleeWeapon, "Fourbisseur lourd", "Diminue le temps de fabrication, de réparation et le coût en matériaux des armes de mêlée à deux mains de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftRangedWeapon, new LearnableSkill(CustomSkill.CraftRangedWeapon, "Artillier", "Diminue le temps de fabrication, de réparation et le coût en matériaux des armes à distance de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftShield, new LearnableSkill(CustomSkill.CraftShield, "Blindeur", "Diminue le temps de fabrication, de réparation et le coût en matériaux des boucliers 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftArmor, new LearnableSkill(CustomSkill.CraftArmor, "Armurier", "Diminue le temps de fabrication, de réparation et le coût en matériaux des armures de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftClothes, new LearnableSkill(CustomSkill.CraftClothes, "Costumier", "Diminue le temps de fabrication, de réparation et le coût en matériaux des vêtements de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftAmmunitions, new LearnableSkill(CustomSkill.CraftAmmunitions, "Cartouchier", "Diminue le temps de fabrication, de réparation et le coût en matériaux des munitions de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.CraftClothing, new LearnableSkill(CustomSkill.CraftClothing, "Craft Vêtements", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftFullPlate, new LearnableSkill(CustomSkill.CraftFullPlate, "Craft Harnois", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftHalfPlate, new LearnableSkill(CustomSkill.CraftHalfPlate, "Craft Armure de plaques", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftSplintMail, new LearnableSkill(CustomSkill.CraftSplintMail, "Craft Clibanion", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBreastPlate, new LearnableSkill(CustomSkill.CraftBreastPlate, "Craft Cuirasse", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftScaleMail, new LearnableSkill(CustomSkill.CraftScaleMail, "Craft Chemise de mailles", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftStuddedLeather, new LearnableSkill(CustomSkill.CraftStuddedLeather, "Craft Cuir clouté", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLeatherArmor, new LearnableSkill(CustomSkill.CraftLeatherArmor, "Craft Armure de cuir", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftPaddedArmor, new LearnableSkill(CustomSkill.CraftPaddedArmor, "Craft Armure matelassée", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftShortsword, new LearnableSkill(CustomSkill.CraftShortsword, "Craft Epée courte", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLongsword, new LearnableSkill(CustomSkill.CraftLongsword, "Craft Epée longue", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBattleAxe, new LearnableSkill(CustomSkill.CraftBattleAxe, "Craft Hache d'armes", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBastardSword, new LearnableSkill(CustomSkill.CraftBastardSword, "Craft Epée bâtarde", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLightFlail, new LearnableSkill(CustomSkill.CraftLightFlail, "Craft Fléau léger", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftWarHammer, new LearnableSkill(CustomSkill.CraftWarHammer, "Craft Marteau de guerre", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftHeavyCrossbow, new LearnableSkill(CustomSkill.CraftHeavyCrossbow, "Craft Arbalète lourde", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLightCrossbow, new LearnableSkill(CustomSkill.CraftLightCrossbow, "Craft Arbalète légère", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLongBow, new LearnableSkill(CustomSkill.CraftLongBow, "Craft Arc long", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLightMace, new LearnableSkill(CustomSkill.CraftLightMace, "Craft Masse légère", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftHalberd, new LearnableSkill(CustomSkill.CraftHalberd, "Craft Hallebarde", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftShortBow, new LearnableSkill(CustomSkill.CraftShortBow, "Craft Arc court", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftTwoBladedSword, new LearnableSkill(CustomSkill.CraftTwoBladedSword, "Craft Epée double", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftGreatSword, new LearnableSkill(CustomSkill.CraftGreatSword, "Craft Epée à deux mains", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftSmallShield, new LearnableSkill(CustomSkill.CraftSmallShield, "Craft Rondache", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftTorch, new LearnableSkill(CustomSkill.CraftTorch, "Craft Torche", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftHelmet, new LearnableSkill(CustomSkill.CraftHelmet, "Craft Heaume", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftGreatAxe, new LearnableSkill(CustomSkill.CraftGreatAxe, "Craft Grande Hache", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftAmulet, new LearnableSkill(CustomSkill.CraftAmulet, "Craft Amulette", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftArrow, new LearnableSkill(CustomSkill.CraftArrow, "Craft Flêches", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBelt, new LearnableSkill(CustomSkill.CraftBelt, "Craft Ceinture", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftDagger, new LearnableSkill(CustomSkill.CraftDagger, "Craft Dague", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBolt, new LearnableSkill(CustomSkill.CraftBolt, "Craft Carreaux", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBoots, new LearnableSkill(CustomSkill.CraftBoots, "Craft Bottes", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBullets, new LearnableSkill(CustomSkill.CraftBullets, "Craft Billes", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftClub, new LearnableSkill(CustomSkill.CraftClub, "Craft Gourdin", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftDarts, new LearnableSkill(CustomSkill.CraftDarts, "Craft Dards", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftDireMace, new LearnableSkill(CustomSkill.CraftDireMace, "Craft Masse double", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftHeavyFlail, new LearnableSkill(CustomSkill.CraftHeavyFlail, "Craft Fléau lourd", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftGloves, new LearnableSkill(CustomSkill.CraftGloves, "Craft Gants", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLightHammer, new LearnableSkill(CustomSkill.CraftLightHammer, "Craft Marteau léger", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftHandAxe, new LearnableSkill(CustomSkill.CraftHandAxe, "Craft Hachette", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftKama, new LearnableSkill(CustomSkill.CraftKama, "Craft Kama", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftKukri, new LearnableSkill(CustomSkill.CraftKukri, "Craft Kukri", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftMagicRod, new LearnableSkill(CustomSkill.CraftMagicRod, "Craft Bâton magique", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftStaff, new LearnableSkill(CustomSkill.CraftStaff, "Craft Bourdon", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftMagicWand, new LearnableSkill(CustomSkill.CraftMagicWand, "Craft Baguette magique", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftMorningStar, new LearnableSkill(CustomSkill.CraftMorningStar, "Craft Morgenstern", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftPotion, new LearnableSkill(CustomSkill.CraftPotion, "Craft Potion", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftQuarterstaff, new LearnableSkill(CustomSkill.CraftQuarterstaff, "Craft Bâton", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftRapier, new LearnableSkill(CustomSkill.CraftRapier, "Craft Rapière", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftRing, new LearnableSkill(CustomSkill.CraftRing, "Craft Anneau", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftScimitar, new LearnableSkill(CustomSkill.CraftScimitar, "Craft Cimeterre", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftScythe, new LearnableSkill(CustomSkill.CraftScythe, "Craft Faux", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftLargeShield, new LearnableSkill(CustomSkill.CraftLargeShield, "Craft Ecu", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftTowerShield, new LearnableSkill(CustomSkill.CraftTowerShield, "Craft Pavois", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftShortSpear, new LearnableSkill(CustomSkill.CraftShortSpear, "Craft Lance", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftShuriken, new LearnableSkill(CustomSkill.CraftShuriken, "Craft Shuriken", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftSickle, new LearnableSkill(CustomSkill.CraftSickle, "Craft Serpe", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftSling, new LearnableSkill(CustomSkill.CraftSling, "Craft Fronde", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftThrowingAxe, new LearnableSkill(CustomSkill.CraftThrowingAxe, "Craft Hache de lancer", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftBracer, new LearnableSkill(CustomSkill.CraftBracer, "Craft Brassard", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftCloak, new LearnableSkill(CustomSkill.CraftCloak, "Craft Cape", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftTrident, new LearnableSkill(CustomSkill.CraftTrident, "Craft Trident", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftDwarvenWarAxe, new LearnableSkill(CustomSkill.CraftDwarvenWarAxe, "Craft Hache naine", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftWhip, new LearnableSkill(CustomSkill.CraftWhip, "Craft Fouet", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftDoubleAxe, new LearnableSkill(CustomSkill.CraftDoubleAxe, "Craft Hache double", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftKatana, new LearnableSkill(CustomSkill.CraftKatana, "Craft Katana", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CraftSpellScroll, new LearnableSkill(CustomSkill.CraftSpellScroll, "Craft Parchemin", "Diminue le temps de fabrication, de réparation et le coût en matériaux de l'objet concerné de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Intelligence, Ability.Constitution));
    }
    public static Dictionary<Feat, CustomFeat> customFeatsDictionnary = new Dictionary<Feat, CustomFeat>()
    {
      { CustomFeats.StripMiner, new CustomFeat("Minage par Fracturation", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeats.ContractScience, new CustomFeat("Science du contrat", "Permet de créer un contrat supplémentaire par niveau (enchères comprises).", 5) },
      { CustomFeats.Marchand, new CustomFeat("Marchand", "Permet de vendre cinq objets supplémentaires par échoppe.", 20) },
      { CustomFeats.Magnat, new CustomFeat("Magnat", "Permet d'ouvrir une échoppe supplémentaire par niveau.", 5) },
      { CustomFeats.Negociateur, new CustomFeat("Négociateur", "Permet d'enregistrer 3 ordres supplémentaires à l'Hôtel des ventes.", 10) },
      { CustomFeats.BrokerRelations, new CustomFeat("Relations Courtières", "Réduit de 6 % par niveau la taxe de courtage.", 5) },
      { CustomFeats.BrokerAffinity, new CustomFeat("Affinités Courtières", "Réduit de 6 % par niveau la taxe de courtage.", 5) },
      { CustomFeats.Comptabilite, new CustomFeat("Comptabilité", "Réduit de 11 % par niveau la taxe de vente.", 5) },
      { CustomFeats.Sit, new CustomFeat("S'asseoir", "Ce don vous permet de vous asseoir puis d'ajuster l'affichage de votre personnage (mais pas sa position réelle). \n\nIl est possible de choisir une autre emote afin de s'afficher sous une autre posture.", 1) },
      { CustomFeats.AlchemistEfficiency, new CustomFeat("Alchimiste économe", "Permet l'utilisation du mortier et produit 1 * [niveau] de poudre à partir d'un ingrédient d'alchimie.", 5) },
      { CustomFeats.AlchemistCareful, new CustomFeat("Alchimiste prudent", "Permet d'ajouter de l'eau à un mélange alchimique afin d'adoucir le mélange et de retourner vers l'état neutre.", 1) },
      { CustomFeats.AlchemistExpert, new CustomFeat("Alchimiste expert", "Permet d'ajouter un effet supplémentaire à une potion par niveau.", 5) },
      { CustomFeats.Alchemist, new CustomFeat("Alchimiste", "Diminue le temps d'infusion d'une potion de 2 % par niveau.", 10) },
      { CustomFeats.AlchemistAware, new CustomFeat("Alchimiste attentif", "Permet de distinguer les changements de couleurs lors d'un mélange alchimique.\n\n Donne des indications sur la proximité d'un effet.", 5) },
      { CustomFeats.AlchemistAccurate, new CustomFeat("Alchimiste précis", "Permet de distinguer les changements d'odeurs lors d'un mélange alchimique.\n\n Donne des indications sur le type de solution vers lequel tendre pour obtenir l'effet le plus proche.", 5) },
    };

    private static bool HandleImproveHealth(PlayerSystem.Player player, int customSkillId)
    {
      int improvedHealth = 0;
      if (player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth))
        improvedHealth = player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel;

      int toughness = 0;
      if (player.learnableSkills.ContainsKey(CustomSkill.Toughness))
        toughness = player.learnableSkills[CustomSkill.Toughness].currentLevel;

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + toughness) * improvedHealth);

      return true;
    }
    private static bool HandleImproveAbility(PlayerSystem.Player player, int customSkillId)
    {
      //Log.Info($"improve ability triggered : {customSkillId}");
      switch (customSkillId)
      {
        case CustomSkill.ImprovedStrength:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) + 1));
          break;
        case CustomSkill.ImprovedDexterity:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) + 1));
          break;
        case CustomSkill.ImprovedConstitution:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) + 1));
          HandleImproveHealth(player, CustomSkill.ImprovedHealth);
          break;
        case CustomSkill.ImprovedIntelligence:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) + 1));
          break;
        case CustomSkill.ImprovedWisdom:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Wisdom, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) + 1));
          break;
        case CustomSkill.ImprovedCharisma:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) + 1));
          break;
      }

      return true;
    }
    private static bool HandleBackground(PlayerSystem.Player player, int customSkillId)
    {
      switch (customSkillId)
      {
        case CustomSkill.Acolyte:
        case CustomSkill.Anthropologist:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Insight))
            player.learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Insight]));
          player.learnableSkills[CustomSkill.Insight].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Religion))
            player.learnableSkills.Add(CustomSkill.Religion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Religion]));
          player.learnableSkills[CustomSkill.Religion].bonusPoints += 1;
          break;

        case CustomSkill.Archeologist:
          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;
          break;

        case CustomSkill.CloisteredScholar:
          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Arcana))
            player.learnableSkills.Add(CustomSkill.Arcana, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Arcana]));
          player.learnableSkills[CustomSkill.Arcana].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Arcana);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Nature))
            player.learnableSkills.Add(CustomSkill.Nature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Nature]));
          player.learnableSkills[CustomSkill.Nature].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Religion))
            player.learnableSkills.Add(CustomSkill.Religion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Religion]));
          player.learnableSkills[CustomSkill.Religion].bonusPoints += 1;
          break;

        case CustomSkill.Sage:
          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Arcana))
            player.learnableSkills.Add(CustomSkill.Arcana, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Arcana]));
          player.learnableSkills[CustomSkill.Arcana].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Arcana);
          break;

        case CustomSkill.Hermit:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Medicine))
            player.learnableSkills.Add(CustomSkill.Medicine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Medicine]));
          player.learnableSkills[CustomSkill.Medicine].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Religion))
            player.learnableSkills.Add(CustomSkill.Religion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Religion]));
          player.learnableSkills[CustomSkill.Religion].bonusPoints += 1;
          break;

        case CustomSkill.Wanderer:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Persuasion))
            player.learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Persuasion]));
          player.learnableSkills[CustomSkill.Persuasion].bonusPoints += 1;
          break;

        case CustomSkill.Athlete:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Acrobatics))
            player.learnableSkills.Add(CustomSkill.Acrobatics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Acrobatics]));
          player.learnableSkills[CustomSkill.Acrobatics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Acrobatics);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);

          break;

        case CustomSkill.Outlander:
        case CustomSkill.Marine:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);
          break;

        case CustomSkill.Soldier:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Intimidation))
            player.learnableSkills.Add(CustomSkill.Intimidation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Intimidation]));
          player.learnableSkills[CustomSkill.Intimidation].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Intimidation);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);
          break;

        case CustomSkill.Mercenary:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Persuasion))
            player.learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Persuasion]));
          player.learnableSkills[CustomSkill.Persuasion].bonusPoints += 1;
          break;

        case CustomSkill.FolkHero:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Dressage))
            player.learnableSkills.Add(CustomSkill.Dressage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Dressage]));
          player.learnableSkills[CustomSkill.Dressage].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;
          break;

        case CustomSkill.Sailor:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Perception))
            player.learnableSkills.Add(CustomSkill.Perception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Perception]));
          player.learnableSkills[CustomSkill.Perception].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Perception);
          break;

        case CustomSkill.Shipwright:
          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Perception))
            player.learnableSkills.Add(CustomSkill.Perception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Perception]));
          player.learnableSkills[CustomSkill.Perception].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Perception);

          // TODO : Accès gratuit à l'artisanat charpentier + 1 point de compétence bonus, uniquement si l'utilisateur ne connait pas déjà l'artisanat charpentier
          break;

        case CustomSkill.Fisher:
          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;

          // TODO : Accès gratuit à l'artisanat pêcheur + 1 point de compétence bonus, uniquement si l'utilisateur ne connait pas déjà l'artisanat pêcheur
          break;

        case CustomSkill.Criminal:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Stealth))
            player.learnableSkills.Add(CustomSkill.Stealth, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Stealth]));
          player.learnableSkills[CustomSkill.Stealth].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Stealth);
          break;

        case CustomSkill.Charlatan:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Escamotage))
            player.learnableSkills.Add(CustomSkill.Escamotage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Escamotage]));
          player.learnableSkills[CustomSkill.Escamotage].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Escamotage);
          break;

        case CustomSkill.Smuggler:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);
          break;

        case CustomSkill.StreetUrchin:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Escamotage))
            player.learnableSkills.Add(CustomSkill.Escamotage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Escamotage]));
          player.learnableSkills[CustomSkill.Escamotage].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Escamotage);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Stealth))
            player.learnableSkills.Add(CustomSkill.Stealth, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Stealth]));
          player.learnableSkills[CustomSkill.Stealth].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Stealth);
          break;

        case CustomSkill.Gambler:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Insight))
            player.learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Insight]));
          player.learnableSkills[CustomSkill.Insight].bonusPoints += 1;
          break;

        case CustomSkill.Entertainer:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Acrobatics))
            player.learnableSkills.Add(CustomSkill.Acrobatics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Acrobatics]));
          player.learnableSkills[CustomSkill.Acrobatics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Acrobatics);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Performance))
            player.learnableSkills.Add(CustomSkill.Performance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Performance]));
          player.learnableSkills[CustomSkill.Performance].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Performance);
          break;

        case CustomSkill.CityWatch:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Athletics))
            player.learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Athletics]));
          player.learnableSkills[CustomSkill.Athletics].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Athletics);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Insight))
            player.learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Insight]));
          player.learnableSkills[CustomSkill.Insight].bonusPoints += 1;
          break;

        case CustomSkill.Investigator:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Investigation))
            player.learnableSkills.Add(CustomSkill.Investigation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Investigation]));
          player.learnableSkills[CustomSkill.Investigation].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Investigation);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Insight))
            player.learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Insight]));
          player.learnableSkills[CustomSkill.Insight].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Perception))
            player.learnableSkills.Add(CustomSkill.Perception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Perception]));
          player.learnableSkills[CustomSkill.Perception].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Perception);
          break;

        case CustomSkill.KnightOfTheOrder:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Persuasion))
            player.learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Persuasion]));
          player.learnableSkills[CustomSkill.Persuasion].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Religion))
            player.learnableSkills.Add(CustomSkill.Religion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Religion]));
          player.learnableSkills[CustomSkill.Religion].bonusPoints += 1;
          break;

        case CustomSkill.Noble:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Persuasion))
            player.learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Persuasion]));
          player.learnableSkills[CustomSkill.Persuasion].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.History))
            player.learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.History]));
          player.learnableSkills[CustomSkill.History].bonusPoints += 1;
          break;

        case CustomSkill.Courtier:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Persuasion))
            player.learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Persuasion]));
          player.learnableSkills[CustomSkill.Persuasion].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Insight))
            player.learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Insight]));
          player.learnableSkills[CustomSkill.Insight].bonusPoints += 1;
          break;

        case CustomSkill.FailedMerchant:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Persuasion))
            player.learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Persuasion]));
          player.learnableSkills[CustomSkill.Persuasion].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Investigation))
            player.learnableSkills.Add(CustomSkill.Investigation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Investigation]));
          player.learnableSkills[CustomSkill.Investigation].bonusPoints += 1;
          break;

        case CustomSkill.Taken:
        case CustomSkill.Refugee:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Nature))
            player.learnableSkills.Add(CustomSkill.Nature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Nature]));
          player.learnableSkills[CustomSkill.Nature].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;
          break;

        case CustomSkill.Heir:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Arcana))
            player.learnableSkills.Add(CustomSkill.Arcana, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Arcana]));
          player.learnableSkills[CustomSkill.Arcana].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Arcana);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Survival))
            player.learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Survival]));
          player.learnableSkills[CustomSkill.Survival].bonusPoints += 1;
          break;

        case CustomSkill.HauntedOne:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Arcana))
            player.learnableSkills.Add(CustomSkill.Arcana, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Arcana]));
          player.learnableSkills[CustomSkill.Arcana].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Arcana);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Investigation))
            player.learnableSkills.Add(CustomSkill.Investigation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Investigation]));
          player.learnableSkills[CustomSkill.Investigation].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Investigation);
          break;

        case CustomSkill.Magistrate:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Insight))
            player.learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Insight]));
          player.learnableSkills[CustomSkill.Insight].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Intimidation))
            player.learnableSkills.Add(CustomSkill.Intimidation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Intimidation]));
          player.learnableSkills[CustomSkill.Intimidation].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Intimidation);
          break;

        case CustomSkill.Faceless:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Intimidation))
            player.learnableSkills.Add(CustomSkill.Intimidation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Intimidation]));
          player.learnableSkills[CustomSkill.Intimidation].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Intimidation);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Performance))
            player.learnableSkills.Add(CustomSkill.Performance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Performance]));
          player.learnableSkills[CustomSkill.Performance].bonusPoints += 1;
          break;

        case CustomSkill.SecretIdentity:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;

          if (!player.learnableSkills.ContainsKey(CustomSkill.Stealth))
            player.learnableSkills.Add(CustomSkill.Stealth, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Stealth]));
          player.learnableSkills[CustomSkill.Stealth].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Stealth);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Performance))
            player.learnableSkills.Add(CustomSkill.Performance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Performance]));
          player.learnableSkills[CustomSkill.Performance].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Performance);
          break;

        case CustomSkill.AdventurerScion:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Perception))
            player.learnableSkills.Add(CustomSkill.Perception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Perception]));
          player.learnableSkills[CustomSkill.Perception].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Perception);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Performance))
            player.learnableSkills.Add(CustomSkill.Performance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Performance]));
          player.learnableSkills[CustomSkill.Performance].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Performance);
          break;

        case CustomSkill.Prisoner:
          if (!player.learnableSkills.ContainsKey(CustomSkill.Perception))
            player.learnableSkills.Add(CustomSkill.Perception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Perception]));
          player.learnableSkills[CustomSkill.Perception].bonusPoints += 1;

          HandleBaseSkill(player, CustomSkill.Perception);

          if (!player.learnableSkills.ContainsKey(CustomSkill.Deception))
            player.learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Deception]));
          player.learnableSkills[CustomSkill.Deception].bonusPoints += 1;
          break;
      }

      return true;
    }
    private static bool HandleBaseSkill(PlayerSystem.Player player, int customSkillId)
    {
      switch (customSkillId)
      {
        case CustomSkill.Athletics:

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity))
            return true;

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) == player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity)
            && player.learnableSkills.ContainsKey(CustomSkill.Acrobatics) && player.learnableSkills[CustomSkill.Acrobatics].totalPoints > player.learnableSkills[CustomSkill.Athletics].totalPoints)
            return true;

          player.oid.LoginCreature.SetSkillRank(Skill.Discipline, (sbyte)player.learnableSkills[CustomSkill.Athletics].totalPoints);

          break;

        case CustomSkill.Acrobatics:

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength))
            return true;

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) == player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength)
            && player.learnableSkills.ContainsKey(CustomSkill.Athletics) && player.learnableSkills[CustomSkill.Athletics].totalPoints > player.learnableSkills[CustomSkill.Acrobatics].totalPoints)
            return true;

          player.oid.LoginCreature.SetSkillRank(Skill.Discipline, (sbyte)player.learnableSkills[CustomSkill.Acrobatics].totalPoints);

          break;

        case CustomSkill.OpenLock:
          player.oid.LoginCreature.SetSkillRank(Skill.OpenLock, (sbyte)player.learnableSkills[CustomSkill.OpenLock].totalPoints);
          break;

        case CustomSkill.Escamotage:
          player.oid.LoginCreature.SetSkillRank(Skill.PickPocket, (sbyte)player.learnableSkills[CustomSkill.Escamotage].totalPoints);
          break;

        case CustomSkill.Stealth:
          player.oid.LoginCreature.SetSkillRank(Skill.Hide, (sbyte)player.learnableSkills[CustomSkill.Stealth].totalPoints);
          player.oid.LoginCreature.SetSkillRank(Skill.MoveSilently, (sbyte)player.learnableSkills[CustomSkill.Stealth].totalPoints);
          break;

        case CustomSkill.Concentration:
          player.oid.LoginCreature.SetSkillRank(Skill.Concentration, (sbyte)player.learnableSkills[CustomSkill.Concentration].totalPoints);
          break;

        case CustomSkill.Arcana:
          player.oid.LoginCreature.SetSkillRank(Skill.Spellcraft, (sbyte)player.learnableSkills[CustomSkill.Arcana].totalPoints);
          player.oid.LoginCreature.SetSkillRank(Skill.Lore, (sbyte)player.learnableSkills[CustomSkill.Arcana].totalPoints);
          break;

        case CustomSkill.Medicine:
          player.oid.LoginCreature.SetSkillRank(Skill.Heal, (sbyte)player.learnableSkills[CustomSkill.Medicine].totalPoints);
          break;

        case CustomSkill.Investigation:
          player.oid.LoginCreature.SetSkillRank(Skill.Search, (sbyte)player.learnableSkills[CustomSkill.Investigation].totalPoints);
          break;

        case CustomSkill.Perception:
          player.oid.LoginCreature.SetSkillRank(Skill.Spot, (sbyte)player.learnableSkills[CustomSkill.Perception].totalPoints);
          player.oid.LoginCreature.SetSkillRank(Skill.Listen, (sbyte)player.learnableSkills[CustomSkill.Perception].totalPoints);
          break;

        case CustomSkill.Intimidation:
          player.oid.LoginCreature.SetSkillRank(Skill.Intimidate, (sbyte)player.learnableSkills[CustomSkill.Intimidation].totalPoints);
          break;

        case CustomSkill.Performance:
          player.oid.LoginCreature.SetSkillRank(Skill.Perform, (sbyte)player.learnableSkills[CustomSkill.Performance].totalPoints);
          break;

        case CustomSkill.Taunt:
          player.oid.LoginCreature.SetSkillRank(Skill.Taunt, (sbyte)player.learnableSkills[CustomSkill.Taunt].totalPoints);
          break;

        case CustomSkill.TrapExpertise:
          player.oid.LoginCreature.SetSkillRank(Skill.DisableTrap, (sbyte)player.learnableSkills[CustomSkill.TrapExpertise].totalPoints);
          player.oid.LoginCreature.SetSkillRank(Skill.SetTrap, (sbyte)player.learnableSkills[CustomSkill.TrapExpertise].totalPoints);
          break;
      }

      return true;
    }
    private static bool HandleImproveAttack(PlayerSystem.Player player, int customSkill)
    {
      player.oid.LoginCreature.BaseAttackBonus += 1;
      return true;
    }
    private static bool HandleImproveSavingThrow(PlayerSystem.Player player, int customSkillId)
    {
      switch (customSkillId)
      {
        case CustomSkill.ImprovedFortitude:
          player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.Fortitude, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Fortitude) + 1));
          break;
        case CustomSkill.ImprovedReflex:
          player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.Reflex, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Reflex) + 1));
          break;
        case CustomSkill.ImprovedWill:
          player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.Will, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Will) + 1));
          break;
        case CustomSkill.ImprovedSavingThrowAll:
          player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.All, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.All) + 1));
          break;
      }

      return true;
    }
    private static bool HandleAddedSpellSlot(PlayerSystem.Player player, int customSkillId)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      IPSpellLevel spellLevel = IPSpellLevel.SL0;

      switch (customSkillId)
      {
        case CustomSkill.ImprovedSpellSlot1:
          spellLevel = IPSpellLevel.SL1;
          break;
        case CustomSkill.ImprovedSpellSlot2:
          spellLevel = IPSpellLevel.SL2;
          break;
        case CustomSkill.ImprovedSpellSlot3:
          spellLevel = IPSpellLevel.SL3;
          break;
        case CustomSkill.ImprovedSpellSlot4:
          spellLevel = IPSpellLevel.SL4;
          break;
        case CustomSkill.ImprovedSpellSlot5:
          spellLevel = IPSpellLevel.SL5;
          break;
        case CustomSkill.ImprovedSpellSlot6:
          spellLevel = IPSpellLevel.SL6;
          break;
        case CustomSkill.ImprovedSpellSlot7:
          spellLevel = IPSpellLevel.SL7;
          break;
        case CustomSkill.ImprovedSpellSlot8:
          spellLevel = IPSpellLevel.SL8;
          break;
        case CustomSkill.ImprovedSpellSlot9:
          spellLevel = IPSpellLevel.SL9;
          break;
      }

      if (skin == null)
      {
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

        Task waitSkinCreated = NwTask.Run(async () =>
        {
          NwItem pcSkin = await NwItem.Create("peaudejoueur", player.oid.LoginCreature);
          pcSkin.Name = $"Propriétés de {player.oid.LoginCreature.Name}";
          pcSkin.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
          player.oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
        });
      }

      skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, spellLevel), EffectDuration.Permanent);

      return true;
    }

    public static int[] forgeBasicSkillBooks = new int[] { CustomSkill.MineralExtraction, CustomSkill.Blacksmith, CustomSkill.Recycler, CustomSkill.MineralExtractionYield, CustomSkill.ReprocessingOre, CustomSkill.ReprocessingOreEfficiency, CustomSkill.ReprocessingGrade1Expertise, CustomSkill.ReprocessingGrade2Expertise, CustomSkill.MateriaGradeConcentration, CustomSkill.Renforcement, CustomSkill.BlueprintCopy, CustomSkill.BlueprintEfficiency, CustomSkill.BlueprintEconomy, CustomSkill.BlueprintMetallurgy, CustomSkill.BlueprintResearch, CustomSkill.CraftDagger, CustomSkill.CraftLightMace, CustomSkill.CraftMorningStar, CustomSkill.CraftShortSpear, CustomSkill.CraftShortsword, CustomSkill.CraftSickle, CustomSkill.CraftScaleMail };
    public static int[] woodBasicSkillBooks = new int[] { CustomSkill.Renforcement, CustomSkill.Recycler, CustomSkill.BlueprintMetallurgy, CustomSkill.BlueprintResearch, CustomSkill.WoodExtraction, CustomSkill.WoodDetection, CustomSkill.ReprocessingWood, CustomSkill.Woodworker, CustomSkill.CraftSmallShield, CustomSkill.CraftClub, CustomSkill.CraftDarts, CustomSkill.CraftBullets, CustomSkill.CraftHeavyCrossbow, CustomSkill.CraftLightCrossbow, CustomSkill.CraftQuarterstaff, CustomSkill.CraftSling, CustomSkill.CraftArrow, CustomSkill.CraftBolt };
    public static int[] leatherBasicSkillBooks = new int[] { CustomSkill.Renforcement, CustomSkill.Recycler, CustomSkill.PeltExtraction, CustomSkill.ReprocessingPelt, CustomSkill.Tanner, CustomSkill.CraftLeatherArmor, CustomSkill.CraftStuddedLeather, CustomSkill.CraftPaddedArmor, CustomSkill.CraftClothing, CustomSkill.CraftWhip, CustomSkill.CraftBelt, CustomSkill.CraftBoots, CustomSkill.CraftBracer, CustomSkill.CraftCloak, CustomSkill.CraftGloves };
    //public static Feat[] craftSkillBooks = new Feat[] { CustomFeats.Metallurgy, CustomFeats.AdvancedCraft, CustomFeats.Miner, CustomFeats.Geology, CustomFeats.Prospection, CustomFeats.VeldsparReprocessing, CustomFeats.ScorditeReprocessing, CustomFeats.PyroxeresReprocessing, CustomFeats.StripMiner, CustomFeats.Reprocessing, CustomFeats.ReprocessingEfficiency, CustomFeats.Connections, CustomFeats.Forge };
    public static Feat[] alchemyBasicSkillBooks = new Feat[] { CustomFeats.Alchemist, CustomFeats.AlchemistCareful, CustomFeats.AlchemistEfficiency };
    public static Feat[] languageSkillBooks = new Feat[] { CustomFeats.Abyssal, CustomFeats.Céleste, CustomFeats.Gnome, CustomFeats.Draconique, CustomFeats.Druidique, CustomFeats.Nain, CustomFeats.Elfique, CustomFeats.Géant, CustomFeats.Gobelin, CustomFeats.Halfelin, CustomFeats.Infernal, CustomFeats.Orc, CustomFeats.Primordiale, CustomFeats.Sylvain, CustomFeats.Voleur, CustomFeats.Gnome };

    public static Feat[] lowSkillBooks = new Feat[] { CustomFeats.AlchemistExpert, CustomFeats.Renforcement, CustomFeats.ArtisanApplique, CustomFeats.Enchanteur, CustomFeats.Comptabilite, CustomFeats.BrokerRelations, CustomFeats.Negociateur, CustomFeats.Magnat, CustomFeats.Marchand, CustomFeats.Recycler, Feat.Ambidexterity, CustomFeats.Skinning, CustomFeats.Hunting, CustomFeats.ImprovedSpellSlot2, CustomFeats.WoodReprocessing, CustomFeats.Ebeniste, CustomFeats.WoodCutter, CustomFeats.WoodProspection, CustomFeats.CraftOreExtractor, CustomFeats.CraftForgeHammer, CustomFeats.Forge, CustomFeats.Reprocessing, CustomFeats.BlueprintCopy, CustomFeats.Research, CustomFeats.Miner, CustomFeats.Metallurgy, Feat.DeneirsEye, Feat.DirtyFighting, Feat.ResistDisease, Feat.Stealthy, Feat.SkillFocusAnimalEmpathy, Feat.SkillFocusBluff, Feat.SkillFocusConcentration, Feat.SkillFocusDisableTrap, Feat.SkillFocusDiscipline, Feat.SkillFocusHeal, Feat.SkillFocusHide, Feat.SkillFocusIntimidate, Feat.SkillFocusListen, Feat.SkillFocusLore, Feat.SkillFocusMoveSilently, Feat.SkillFocusOpenLock, Feat.SkillFocusParry, Feat.SkillFocusPerform, Feat.SkillFocusPickPocket, Feat.SkillFocusSearch, Feat.SkillFocusSetTrap, Feat.SkillFocusSpellcraft, Feat.SkillFocusSpot, Feat.SkillFocusTaunt, Feat.SkillFocusTumble, Feat.SkillFocusUseMagicDevice, Feat.Mobility, Feat.PointBlankShot, Feat.IronWill, Feat.Alertness, Feat.CombatCasting, Feat.Dodge, Feat.ExtraTurning, Feat.GreatFortitude };
    public static Feat[] mediumSkillBooks = new Feat[] { CustomFeats.AlchemistAccurate, CustomFeats.AlchemistAware, CustomFeats.CombattantPrecautionneux, CustomFeats.EnchanteurExpert, CustomFeats.BrokerAffinity, CustomFeats.BadPeltReprocessing, CustomFeats.CommonPeltReprocessing, CustomFeats.NormalPeltReprocessing, CustomFeats.UncommunPeltReprocessing, CustomFeats.RarePeltReprocessing, CustomFeats.MagicPeltReprocessing, CustomFeats.EpicPeltReprocessing, CustomFeats.LegendaryPeltReprocessing, CustomFeats.ImprovedSpellSlot3, CustomFeats.ImprovedSpellSlot4, CustomFeats.LaurelinReprocessing, CustomFeats.MallornReprocessing, CustomFeats.TelperionReprocessing, CustomFeats.OiolaireReprocessing, CustomFeats.NimlothReprocessing, CustomFeats.QlipothReprocessing, CustomFeats.FerocheneReprocessing, CustomFeats.ValinorReprocessing, CustomFeats.WoodReprocessingEfficiency, CustomFeats.AnimalExpertise, CustomFeats.CraftTorch, CustomFeats.CraftStuddedLeather, CustomFeats.CraftSling, CustomFeats.CraftSmallShield, CustomFeats.CraftSickle, CustomFeats.CraftShortSpear, CustomFeats.CraftRing, CustomFeats.CraftPaddedArmor, CustomFeats.CraftPotion, CustomFeats.CraftQuarterstaff, CustomFeats.CraftMorningStar, CustomFeats.CraftMagicWand, CustomFeats.CraftLightMace, CustomFeats.CraftLightHammer, CustomFeats.CraftLightFlail, CustomFeats.CraftLightCrossbow, CustomFeats.CraftLeatherArmor, CustomFeats.CraftBullets, CustomFeats.CraftCloak, CustomFeats.CraftClothing, CustomFeats.CraftClub, CustomFeats.CraftDagger, CustomFeats.CraftDarts, CustomFeats.CraftGloves, CustomFeats.CraftHeavyCrossbow, CustomFeats.CraftHelmet, CustomFeats.CraftAmulet, CustomFeats.CraftArrow, CustomFeats.CraftBelt, CustomFeats.CraftBolt, CustomFeats.CraftBoots, CustomFeats.CraftBracer, CustomFeats.ReprocessingEfficiency, CustomFeats.StripMiner, CustomFeats.VeldsparReprocessing, CustomFeats.ScorditeReprocessing, CustomFeats.PyroxeresReprocessing, CustomFeats.PlagioclaseReprocessing, CustomFeats.Geology, CustomFeats.Prospection, Feat.TymorasSmile, Feat.LliirasHeart, Feat.RapidReload, Feat.Expertise, Feat.ImprovedInitiative, Feat.DefensiveRoll, Feat.SneakAttack, Feat.FlurryOfBlows, Feat.WeaponSpecializationHeavyCrossbow, Feat.WeaponSpecializationDagger, Feat.WeaponSpecializationDart, Feat.WeaponSpecializationClub, Feat.StillSpell, Feat.RapidShot, Feat.SilenceSpell, Feat.PowerAttack, Feat.Knockdown, Feat.LightningReflexes, Feat.ImprovedUnarmedStrike, Feat.Cleave, Feat.CalledShot, Feat.DeflectArrows, Feat.WeaponSpecializationLightCrossbow, Feat.WeaponSpecializationLightFlail, Feat.WeaponSpecializationLightMace, Feat.Disarm, Feat.EmpowerSpell, Feat.WeaponSpecializationMorningStar, Feat.ExtendSpell, Feat.SpellFocusAbjuration, Feat.SpellFocusConjuration, Feat.SpellFocusDivination, Feat.SpellFocusEnchantment, Feat.WeaponSpecializationSickle, Feat.WeaponSpecializationSling, Feat.WeaponSpecializationSpear, Feat.WeaponSpecializationStaff, Feat.WeaponSpecializationThrowingAxe, Feat.WeaponSpecializationTrident, Feat.WeaponSpecializationUnarmedStrike, Feat.SpellFocusEvocation, Feat.SpellFocusIllusion, Feat.SpellFocusNecromancy, Feat.SpellFocusTransmutation, Feat.SpellPenetration };
    public static Feat[] highSkillBooks = new Feat[] { CustomFeats.ImprovedDodge, CustomFeats.EnchanteurChanceux, CustomFeats.SurchargeControlee, CustomFeats.SurchargeArcanique, CustomFeats.ArtisanExceptionnel, CustomFeats.AdvancedCraft, CustomFeats.CraftWarHammer, CustomFeats.CraftTrident, CustomFeats.CraftThrowingAxe, CustomFeats.CraftStaff, CustomFeats.CraftSplintMail, CustomFeats.CraftSpellScroll, CustomFeats.CraftShortsword, CustomFeats.CraftShortBow, CustomFeats.CraftScimitar, CustomFeats.CraftScaleMail, CustomFeats.CraftRapier, CustomFeats.CraftMagicRod, CustomFeats.CraftLongsword, CustomFeats.CraftLongBow, CustomFeats.CraftLargeShield, CustomFeats.CraftBattleAxe, CustomFeats.OmberReprocessing, CustomFeats.KerniteReprocessing, CustomFeats.GneissReprocessing, CustomFeats.CraftHalberd, CustomFeats.JaspetReprocessing, CustomFeats.CraftHeavyFlail, CustomFeats.CraftHandAxe, CustomFeats.HemorphiteReprocessing, CustomFeats.CraftGreatAxe, CustomFeats.CraftGreatSword, Feat.ArcaneDefenseAbjuration, Feat.ArcaneDefenseConjuration, Feat.ArcaneDefenseDivination, Feat.ArcaneDefenseEnchantment, Feat.ArcaneDefenseEvocation, Feat.ArcaneDefenseIllusion, Feat.ArcaneDefenseNecromancy, Feat.ArcaneDefenseTransmutation, Feat.BlindFight, Feat.SpringAttack, Feat.GreatCleave, Feat.ImprovedExpertise, Feat.SkillMastery, Feat.Opportunist, Feat.Evasion, Feat.WeaponSpecializationDireMace, Feat.WeaponSpecializationDoubleAxe, Feat.WeaponSpecializationDwaxe, Feat.WeaponSpecializationGreatAxe, Feat.WeaponSpecializationGreatSword, Feat.WeaponSpecializationHalberd, Feat.WeaponSpecializationHandAxe, Feat.WeaponSpecializationHeavyFlail, Feat.WeaponSpecializationKama, Feat.WeaponSpecializationKatana, Feat.WeaponSpecializationKukri, Feat.WeaponSpecializationBastardSword, Feat.WeaponSpecializationLightHammer, Feat.WeaponSpecializationLongbow, Feat.WeaponSpecializationLongSword, Feat.WeaponSpecializationRapier, Feat.WeaponSpecializationScimitar, Feat.WeaponSpecializationScythe, Feat.WeaponSpecializationShortbow, Feat.WeaponSpecializationShortSword, Feat.WeaponSpecializationShuriken, Feat.WeaponSpecializationBattleAxe, Feat.QuickenSpell, Feat.MaximizeSpell, Feat.ImprovedTwoWeaponFighting, Feat.ImprovedPowerAttack, Feat.WeaponSpecializationTwoBladedSword, Feat.WeaponSpecializationWarHammer, Feat.WeaponSpecializationWhip, Feat.ImprovedDisarm, Feat.ImprovedKnockdown, Feat.ImprovedParry, Feat.ImprovedCriticalBastardSword, Feat.ImprovedCriticalBattleAxe, Feat.ImprovedCriticalClub, Feat.ImprovedCriticalDagger, Feat.ImprovedCriticalDart, Feat.ImprovedCriticalDireMace, Feat.ImprovedCriticalDoubleAxe, Feat.ImprovedCriticalDwaxe, Feat.ImprovedCriticalGreatAxe, Feat.ImprovedCriticalGreatSword, Feat.ImprovedCriticalHalberd, Feat.ImprovedCriticalHandAxe, Feat.ImprovedCriticalHeavyCrossbow, Feat.ImprovedCriticalHeavyFlail, Feat.ImprovedCriticalKama, Feat.ImprovedCriticalKatana, Feat.ImprovedCriticalKukri, Feat.ImprovedCriticalLightCrossbow, Feat.ImprovedCriticalLightFlail, Feat.ImprovedCriticalLightHammer, Feat.ImprovedCriticalLightMace, Feat.ImprovedCriticalLongbow, Feat.ImprovedCriticalLongSword, Feat.ImprovedCriticalMorningStar, Feat.ImprovedCriticalRapier, Feat.ImprovedCriticalScimitar, Feat.ImprovedCriticalScythe, Feat.ImprovedCriticalShortbow, Feat.ImprovedCriticalShortSword, Feat.ImprovedCriticalShuriken, Feat.ImprovedCriticalSickle, Feat.ImprovedCriticalSling, Feat.ImprovedCriticalSpear, Feat.ImprovedCriticalStaff, Feat.ImprovedCriticalThrowingAxe, Feat.ImprovedCriticalTrident, Feat.ImprovedCriticalTwoBladedSword, Feat.ImprovedCriticalUnarmedStrike, Feat.ImprovedCriticalWarHammer, Feat.ImprovedCriticalWhip };
    public static Feat[] epicSkillBooks = new Feat[] { CustomFeats.CraftWhip, CustomFeats.CraftTwoBladedSword, CustomFeats.CraftTowerShield, CustomFeats.CraftShuriken, CustomFeats.CraftScythe, CustomFeats.CraftKukri, CustomFeats.CraftKatana, CustomFeats.CraftBreastPlate, CustomFeats.CraftDireMace, CustomFeats.CraftDoubleAxe, CustomFeats.CraftDwarvenWarAxe, CustomFeats.CraftFullPlate, CustomFeats.CraftHalfPlate, CustomFeats.CraftBastardSword, CustomFeats.CraftKama, CustomFeats.DarkOchreReprocessing, CustomFeats.CrokiteReprocessing, CustomFeats.BistotReprocessing, Feat.ResistEnergyAcid, Feat.ResistEnergyCold, Feat.ResistEnergyElectrical, Feat.ResistEnergyFire, Feat.ResistEnergySonic, Feat.ZenArchery, Feat.CripplingStrike, Feat.SlipperyMind, Feat.GreaterSpellFocusAbjuration, Feat.GreaterSpellFocusConjuration, Feat.GreaterSpellFocusDivination, Feat.GreaterSpellFocusDiviniation, Feat.GreaterSpellFocusEnchantment, Feat.GreaterSpellFocusEvocation, Feat.GreaterSpellFocusIllusion, Feat.GreaterSpellFocusNecromancy, Feat.GreaterSpellFocusTransmutation, Feat.GreaterSpellPenetration };

    public static int[] shopBasicMagicSkillBooks = new int[] { CustomSkill.Enchanteur,/* CustomSkill.Comptabilite, CustomSkill.BrokerRelations, CustomSkill.Negociateur, CustomSkill.ContractScience, CustomSkill.Marchand, CustomSkill.Magnat*/ };
    public static int GetCustomFeatLevelFromSkillPoints(Feat feat, int currentSkillPoints)
    {
      int multiplier = learnableDictionary[(int)feat].multiplier;
      var result = Math.Log(currentSkillPoints / (250 * multiplier)) / Math.Log(5);

      if (result > 4)
      {
        result = 4;
        result += (currentSkillPoints - (int)(250 * multiplier * Math.Pow(5, 4))) / (int)(250 * multiplier * Math.Pow(5, 4));
      }

      if (result < 0)
        return 0;
      else
        return 1 + (int)result;
    }
  }
}
