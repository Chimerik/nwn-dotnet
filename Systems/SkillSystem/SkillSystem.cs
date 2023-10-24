﻿using System;
using System.Collections.Generic;
using NLog;
using Anvil.API;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public enum Category // Maîtrises + Fighter + chaque classe
    {
      [Description("Corps_&_Esprit")]
      MindBody,
      [Description("Combat")]
      Fight,
      [Description("Artisanat")]
      Craft,
      [Description("Magie")]
      Magic,
      [Description("Inscription")]
      Inscription,
      [Description("Historique")]
      StartingTraits,
      [Description("Langage")]
      Language,
      [Description("Classe")]
      Class,
      [Description("Race")]
      Race,
      [Description("Compétence")]
      Skill
    }

    public static readonly Dictionary<int, Learnable> learnableDictionary = new();

    public static void InitializeLearnables()
    {
      // PROFICIENCIES

      learnableDictionary.Add(CustomSkill.AcrobaticsProficiency, new LearnableSkill(CustomSkill.AcrobaticsProficiency, "Acrobatie - Maîtrise", "Un jet de Dextérité (Acrobatie) couvre toute tentative de garder l'équilibre dans les situations délicates, comme essayer de courir sur une plaque de glace, rester stable sur une corde raide ou rester debout sur le pont d'un navire lors d'une forte houle.\n\nLe DM peut également demander un jet de Dextérité (Acrobatie) si vous tenter d'effectuer des cascades acrobatiques, y compris des plongeons tonneaux, sauts périlleux ou des flips.\n\nNB : Cette compétence n'accorde aucun point de CA supplémentaire, contrairement au jeu de base.", Category.Skill, "ife_SeverA", 1, 750, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.AcrobaticsExpertise, new LearnableSkill(CustomSkill.AcrobaticsExpertise, "Acrobatie - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_SeverA", 1, 1500, Ability.Dexterity, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.AnimalHandlingProficiency, new LearnableSkill(CustomSkill.AnimalHandlingProficiency, "Dressage - Maîtrise", "Un jet de Sagesse (Dressage) couvre toute tentative de calmer un animal domestique, empêcher une monture d'être effrayée ou deviner les intentions d'un animal.", Category.Skill, "ife_SeverA", 1, 750, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.AnimalHandlingExpertise, new LearnableSkill(CustomSkill.AnimalHandlingExpertise, "Dressage - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_SeverA", 1, 1500, Ability.Wisdom, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.ArcanaProficiency, new LearnableSkill(CustomSkill.ArcanaProficiency, "Arcane - Maîtrise", "Un jet d'Intelligence (Arcane) couvre toute tentative de se rappeler des connaissances sur les sorts, les objets magiques, les symboles ésotériques, les traditions magiques, les plans d'existence et les habitants de ces plans.", Category.Skill, "isk_spellcraft", 1, 750, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ArcanaExpertise, new LearnableSkill(CustomSkill.ArcanaExpertise, "Arcane - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_spellcraft", 1, 1500, Ability.Intelligence, Ability.Wisdom));
      
      learnableDictionary.Add(CustomSkill.AthleticsProficiency, new LearnableSkill(CustomSkill.AthleticsProficiency, "Athlétisme - Maîtrise", "Un jet de Force (Athlétisme) couvre les difficultés physiques que vous rencontrez en grimpant, en sautant ou en nageant. Ce qui inclue les activités suivantes :\n- Vous essayez d'escalader une falaise abrupte ou glissante, d'éviter les dangers en escaladant un mur ou de vous accrocher à une surface pendant que quelque chose essaie de vous faire tomber\n- Vous essayez de sauter sur une distance inhabituellement longue ou de réaliser une cascade au milieu d'un saut\n-- Vous avez du mal à nager ou à rester à flot dans des courants dangereux, des vagues agitées par des tempêtes ou des zones d'algues épaisses. Ou une autre créature essaie de vous pousser ou de vous tirer sous l'eau ou d'interférer d'une autre manière avec votre nage", Category.Skill, "ife_SeverA", 1, 750, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.AthleticsExpertise, new LearnableSkill(CustomSkill.AthleticsExpertise, "Athlétisme - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_SeverA", 1, 1500, Ability.Strength, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.DeceptionProficiency, new LearnableSkill(CustomSkill.DeceptionProficiency, "Tromperie - Maîtrise", "Un jet de Charisme (Tromperie) couvre toute tentative de cacher la vérité de manière convaincante, que ce soit verbalement ou par vos actions. Cette compétence permet de dissimuler les traces du mensonge dans votre voix et votre langage corporel.\nN'oubliez pas que dans la plupart des cas, en jeu, la crédibilité de vos paroles par rapport à la confiance accordée par vos interlocuteurs prime.\nDe base, vous ne parviendrez pas à faire croire à la garde que cette épée à deux mains pleine de sang n'est rien d'autre que l'appui d'un vieillard pour l'aider à marcher, même sur un très bon jet.", Category.Skill, "isk_X2bluff", 1, 750, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.DeceptionExpertise, new LearnableSkill(CustomSkill.DeceptionExpertise, "Tromperie - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_X2bluff", 1, 1500, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomSkill.HistoryProficiency, new LearnableSkill(CustomSkill.HistoryProficiency, "Histoire - Maîtrise", "Un jet d'Intelligence (Histoire) couvre toute tentative de se souvenir de traditions, d'événements historiques, de personnages légendaires, d'anciens royaumes, de conflits passés, de guerres récentes et de civilisations perdues.", Category.Skill, "ife_X1App", 1, 750, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.HistoryExpertise, new LearnableSkill(CustomSkill.HistoryExpertise, "Histoire - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_X1App", 1, 1500, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.InsightProficiency, new LearnableSkill(CustomSkill.InsightProficiency, "Intuition - Maîtrise", "Un jet de Sagesse (Intuition) couvre toute tentative de déterminer les véritables intentions d'une créature, par exemple lorsque vous souhaitez dévoiler un mensonge ou prédire le prochain mouvement de quelqu'un.\nIl s'agit principalement de glaner des indices dans le langage corporel, les changements de tons de la voix ou de manières.", Category.Skill, "isk_listen", 1, 750, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.InsightExpertise, new LearnableSkill(CustomSkill.InsightExpertise, "Intuition - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_listen", 1, 1500, Ability.Wisdom, Ability.Intelligence));

      learnableDictionary.Add(CustomSkill.IntimidationProficiency, new LearnableSkill(CustomSkill.IntimidationProficiency, "Intimidation - Maîtrise", "Un jet de Charisme (Intimidation) couvre toute tentative d'influencer quelqu'un par des menaces manifestes, des actions hostiles et de la violence physique.\nPar exemple, essayer de soutirer des informations d'un prisonnier, convaincre des voyous de reculer devant une confrontation ou utiliser un tesson de bouteille brisée pour convaincre un vizir ricanant de reconsidérer une décision.", Category.Skill, "isk_X2Inti", 5, 2, Ability.Charisma, Ability.Strength));
      learnableDictionary.Add(CustomSkill.IntimidationExpertise, new LearnableSkill(CustomSkill.IntimidationExpertise, "Intimidation - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_X2Inti", 5, 3, Ability.Charisma, Ability.Strength));

      learnableDictionary.Add(CustomSkill.InvestigationProficiency, new LearnableSkill(CustomSkill.InvestigationProficiency, "Investigation - Maîtrise", "Un jet d'Intelligence (Investigation) couvre toute recherche d'indice et de déductions. Vous pouvez déduire l'emplacement d'un objet caché, discerner à partir de l'apparence d'une blessure quel type d'arme l'a infligée, ou déterminer le point le plus faible dans un tunnel qui pourrait provoquer son effondrement.\n L'examen d'anciens parchemins à la recherche d'un fragment de connaissance caché peut également nécessiter un jet d'Intelligence (Investigation).\n\nCette compétence remplace fouille pour la détection des pièges.", Category.Skill, "isk_search", 1, 750, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.InvestigationExpertise, new LearnableSkill(CustomSkill.InvestigationExpertise, "Investigation - Expertise", "Un jet d'Intelligence (Investigation) couvre toute recherche d'indice et de déductions. Vous pouvez déduire l'emplacement d'un objet caché, discerner à partir de l'apparence d'une blessure quel type d'arme l'a infligée, ou déterminer le point le plus faible dans un tunnel qui pourrait provoquer son effondrement.\n L'examen d'anciens parchemins à la recherche d'un fragment de connaissance caché peut également nécessiter un jet d'Intelligence (Investigation).\n\nCette compétence remplace fouille pour la détection des pièges.", Category.MindBody, "isk_search", 1, 1500, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.MedicineProficiency, new LearnableSkill(CustomSkill.MedicineProficiency, "Médecine - Maîtrise", "Un jet d'Intelligence (Médecine) couvre toute tentative de se souvenir de détails sur le fonctionnement du corps humain, sur des remèdes ou des poisons. Cette compétence est également utilisée afin de pratiquer des actes médicaux.", Category.Skill, "isk_heal", 1, 750, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.MedicineExpertise, new LearnableSkill(CustomSkill.MedicineExpertise, "Médecine - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_heal", 1, 1500, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.NatureProficiency, new LearnableSkill(CustomSkill.NatureProficiency, "Nature - Maîtrise", "Un jet d'Intelligence (Nature) couvre toute tentative de se souvenir de connaissances sur le terrain, les plantes, les animaux, la météo et les cycles naturels.", Category.Skill, "ife_X2GrWis1", 1, 750, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.NatureExpertise, new LearnableSkill(CustomSkill.NatureExpertise, "Nature - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_X2GrWis1", 1, 1500, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.PerceptionProficiency, new LearnableSkill(CustomSkill.PerceptionProficiency, "Perception - Maîtrise", "Un jet de Sagesse (Perception) couvre toute tentative de repérer, d'entendre ou de détecter la présence de quelque chose. Il mesure votre conscience générale de votre environnement et l'acuité de vos sens.\nPar exemple, vous pouvez essayer d'entendre une conversation à travers une porte fermée, d'écouter sous une fenêtre ouverte ou d'entendre des monstres se déplacer furtivement dans la forêt.\nVous pouvez aussi essayer de repérer des choses qui sont obscurcies ou faciles à manquer, qu'il s'agisse d'orcs embusqués sur une route, de voyous cachés dans l'ombre d'une ruelle ou de bougies sous une porte secrète fermée.", Category.Skill, "isk_spot", 1, 750, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.PerceptionExpertise, new LearnableSkill(CustomSkill.PerceptionExpertise, "Perception - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_spot", 1, 1500, Ability.Wisdom, Ability.Intelligence));
      
      learnableDictionary.Add(CustomSkill.PerformanceProficiency, new LearnableSkill(CustomSkill.PerformanceProficiency, "Performance - Maîtrise", "Un jet de Charisme (Performance) couvre toute tentative de ravir un public avec de la musique, de la danse, du théâtre, des contes ou toute autre forme de divertissement.\n\nChaque rang augmente la durée et l'efficacité des capacités liées à l'encouragement des alliés.", Category.Skill, "ife_SeverA", 1, 750, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.PerformanceExpertise, new LearnableSkill(CustomSkill.PerformanceExpertise, "Performance - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_SeverA", 1, 1500, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomSkill.PersuasionProficiency, new LearnableSkill(CustomSkill.PersuasionProficiency, "Persuasion - Maîtrise", "Un jet de Charisme (Persuasion) couvre toute tentative d'influencer quelqu'un ou un groupe de personnes avec du tact, des grâces sociales ou une bonne nature.\nEn règle générale, la persuasion est utilisée lorsque vous agissez de bonne foi, pour favoriser des amitiés, faire des demandes cordiales ou faire preuve d'une étiquette appropriée.\nPar exemple : convaincre un chambellan de laisser votre groupe voir le roi, négocier la paix entre des tribus en guerre ou inspirer une foule de citadins.", Category.Skill, "isk_persuade", 1, 750, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.PersuasionExpertise, new LearnableSkill(CustomSkill.PersuasionExpertise, "Persuasion - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_persuade", 1, 1500, Ability.Charisma, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.ReligionProficiency, new LearnableSkill(CustomSkill.ReligionProficiency, "Religion - Maîtrise", "Un jet d'Intelligence (Religion) couvre toute tentative de se souvenir des traditions concernant les divinités, les rites, les prières, les hiérarchies religieuses, les symboles sacrés et les pratiques des cultes.", Category.Skill, "isk_lore", 1, 750, Ability.Intelligence, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ReligionExpertise, new LearnableSkill(CustomSkill.ReligionExpertise, "Religion - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_lore", 1, 1500, Ability.Intelligence, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.SleightOfHandProficiency, new LearnableSkill(CustomSkill.SleightOfHandProficiency, "Escamotage - Maîtrise", "Un jet de Dextérité (Escamotage) couvre toute tentative un tour de passe-passe ou de supercherie manuelle, comme déposer quelque chose dans les poches de quelqu'un d'autre ou tenter de dissimuler un objet votre propre personne.\n\nUn jet de Dextérité (Escamotage) permet également de déterminer si vous parvenez à délester quelqu'un de son porte-monnaie ou lui faire les poches.\n\nCette compétence remplace Vol à la Tire.", Category.Skill, "isk_pocket", 1, 750, Ability.Dexterity, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.SleightOfHandExpertise, new LearnableSkill(CustomSkill.SleightOfHandExpertise, "Escamotage - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "isk_pocket", 1, 1500, Ability.Dexterity, Ability.Intelligence));

      learnableDictionary.Add(CustomSkill.StealthProficiency, new LearnableSkill(CustomSkill.StealthProficiency, "Furtivité - Maîtrise", "Un jet de Dextérité (Arts des ombres) couvre toute tentative de se dissimulation à l'œil et à l'oreille des ennemis : pour passer furtivement sous le nez des gardes, vous échapper sans vous faire remarquer ou prendre quelqu'un par surprise.\n\nCette compétence remplace à la fois Discrétion et Déplacement silencieux.\nContrairement au jeu de base, cet attribut ne vous permet pas de disparaître aux yeux des autres.\n\nChaque rang augmente la durée et l'efficacité des capacités liées à la dissimulation et à la furtivité.", Category.Skill, "ife_SeverA", 1, 750, Ability.Intelligence, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.StealthExpertise, new LearnableSkill(CustomSkill.StealthExpertise, "Furtivité - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_SeverA", 1, 1500, Ability.Intelligence, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.SurvivalProficiency, new LearnableSkill(CustomSkill.SurvivalProficiency, "Survie - Maîtrise", "Un jet de Sagesse (Survie) couvre toute tentative de suivre des traces, chasser du gibier sauvage, guider votre groupe à travers des friches gelées, identifier des signes indiquant que des ours-hiboux vivent à proximité, prédire la météo ou d'éviter les sables mouvants et autres dangers naturels.", Category.Skill, "ife_SeverA", 1, 750, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.SurvivalExpertise, new LearnableSkill(CustomSkill.SurvivalExpertise, "Survie - Expertise", "L'expertise permet de doubler votre bonus de maîtrise sur les jets effectués avec cette compétence", Category.MindBody, "ife_SeverA", 1, 1500, Ability.Wisdom, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.StrengthSavesProficiency, new LearnableSkill(CustomSkill.StrengthSavesProficiency, "JDS Force - Maîtrise", "Permet d'ajouter votre bonus de maîtrise à vos jets de sauvegarde utilisant la force.", Category.MindBody, "ife_SeverA", 1, 5000, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.DexteritySavesProficiency, new LearnableSkill(CustomSkill.DexteritySavesProficiency, "JDS Dextérité - Maîtrise", "Permet d'ajouter votre bonus de maîtrise à vos jets de sauvegarde utilisant la dextérité.", Category.MindBody, "ife_SeverA", 1, 5000, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ConstitutionSavesProficiency, new LearnableSkill(CustomSkill.ConstitutionSavesProficiency, "JDS Constitution - Maîtrise", "Permet d'ajouter votre bonus de maîtrise à vos jets de sauvegarde utilisant la constitution.", Category.MindBody, "ife_SeverA", 1, 5000, Ability.Constitution, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.IntelligenceSavesProficiency, new LearnableSkill(CustomSkill.IntelligenceSavesProficiency, "JDS Intelligence - Maîtrise", "Permet d'ajouter votre bonus de maîtrise à vos jets de sauvegarde utilisant l'intelligence.", Category.MindBody, "ife_SeverA", 1, 5000, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WisdomSavesProficiency, new LearnableSkill(CustomSkill.WisdomSavesProficiency, "JDS Sagesse - Maîtrise", "Permet d'ajouter votre bonus de maîtrise à vos jets de sauvegarde utilisant la sagesse.", Category.MindBody, "ife_SeverA", 1, 5000, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CharismaSavesProficiency, new LearnableSkill(CustomSkill.CharismaSavesProficiency, "JDS Charisma - Maîtrise", "Permet d'ajouter votre bonus de maîtrise à vos jets de sauvegarde utilisant le charisme.", Category.MindBody, "ife_SeverA", 1, 5000, Ability.Charisma, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.LightArmorProficiency, new LearnableSkill(CustomSkill.LightArmorProficiency, "Armure légère - Maîtrise", "Utiliser un bouclier en combat sans en avoir la maîtrise vous expose à un désavantage sur :\n-Tous les jets de forces et de dextérité\n- Tous les jets de sauvegarde\n- Tous les jets d'attaques utilisant la force ou la dextérité\n- L'impossibilité d'incanter des sorts", Category.Fight, "ife_armor_l", 1, 250, Ability.Dexterity, Ability.Constitution, HandleLightArmorProficiency));
      learnableDictionary.Add(CustomSkill.MediumArmorProficiency, new LearnableSkill(CustomSkill.MediumArmorProficiency, "Armure intermédiaire - Maîtrise", "Utiliser un bouclier en combat sans en avoir la maîtrise vous expose à un désavantage sur :\n-Tous les jets de forces et de dextérité\n- Tous les jets de sauvegarde\n- Tous les jets d'attaques utilisant la force ou la dextérité\n- L'impossibilité d'incanter des sorts", Category.Fight, "ife_armor_m", 1, 500, Ability.Constitution, Ability.Dexterity, HandleMediumArmorProficiency));
      learnableDictionary.Add(CustomSkill.HeavyArmorProficiency, new LearnableSkill(CustomSkill.HeavyArmorProficiency, "Armure lourde - Maîtrise", "Utiliser une armure lourde en combat sans en avoir la maîtrise vous expose à un désavantage sur :\n-Tous les jets de forces et de dextérité\n- Tous les jets de sauvegarde\n- Tous les jets d'attaques utilisant la force ou la dextérité\n- L'impossibilité d'incanter des sorts", Category.Fight, "ife_armor_h", 1, 750, Ability.Constitution, Ability.Strength, HandleHeavyArmorProficiency));
      learnableDictionary.Add(CustomSkill.ShieldProficiency, new LearnableSkill(CustomSkill.ShieldProficiency, "Bouclier - Maîtrise", "Utiliser un bouclier en combat sans en avoir la maîtrise vous expose à un désavantage sur :\n-Tous les jets de forces et de dextérité\n- Tous les jets de sauvegarde\n- Tous les jets d'attaques utilisant la force ou la dextérité\n- L'impossibilité d'incanter des sorts", Category.Fight, "ife_sh_prof", 1, 250, Ability.Constitution, Ability.Strength, HandleShieldProficiency));

      learnableDictionary.Add(CustomSkill.SimpleWeaponProficiency, new LearnableSkill(CustomSkill.SimpleWeaponProficiency, "Arme simple - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec les armes simples, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 250, Ability.Constitution, Ability.Dexterity, HandleSimpleWeaponProficiency));
      learnableDictionary.Add(CustomSkill.MartialWeaponProficiency, new LearnableSkill(CustomSkill.MartialWeaponProficiency, "Arme martiale - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec les armes martiales, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 500, Ability.Constitution, Ability.Strength, HandleMartialProficiency));
      learnableDictionary.Add(CustomSkill.ExoticWeaponProficiency, new LearnableSkill(CustomSkill.ExoticWeaponProficiency, "Arme exotique - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec les armes exotiques, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength, HandleExoticProficiency));

      learnableDictionary.Add(CustomSkill.SpearProficiency, new LearnableSkill(CustomSkill.SpearProficiency, "Lance - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.LongSwordProficiency, new LearnableSkill(CustomSkill.LongSwordProficiency, "Epée longue - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise aux attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.ShortSwordProficiency, new LearnableSkill(CustomSkill.ShortSwordProficiency, "Epée courte - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.LongBowProficiency, new LearnableSkill(CustomSkill.LongBowProficiency, "Arc long - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.ShortBowProficiency, new LearnableSkill(CustomSkill.ShortBowProficiency, "Arc court - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.RapierProficiency, new LearnableSkill(CustomSkill.RapierProficiency, "Rapière - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.LightHammerProficiency, new LearnableSkill(CustomSkill.LightHammerProficiency, "Marteau léger - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.WarHammerProficiency, new LearnableSkill(CustomSkill.WarHammerProficiency, "Marteau de guerre - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.HandAxeProficiency, new LearnableSkill(CustomSkill.HandAxeProficiency, "Hachette - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.WarAxeProficiency, new LearnableSkill(CustomSkill.WarAxeProficiency, "Hache de bataille - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));
      learnableDictionary.Add(CustomSkill.DwarvenAxeProficiency, new LearnableSkill(CustomSkill.DwarvenAxeProficiency, "Hache naine - Maîtrise", "Vous permet d'ajouter votre bonus de maîtrise à vos attaques avec ce type d'arme, ainsi que d'utiliser leurs attaques spéciales.", Category.Fight, "ife_armor_l", 1, 750, Ability.Constitution, Ability.Strength));

      learnableDictionary.Add(CustomSkill.UncannyDodge, new LearnableSkill(CustomSkill.UncannyDodge, "Esquive Instinctive", "Permet au personnage de réagir instinctivement, même face à un adversaire qu'il ne voit pas.\n\nSi le personnage réussi un jet de survie contre la tromperie d'un adversaire qui l'attaque de dos avec une arme de mêlée, alors le critique automatique sera annulé.", Category.Fight, "ife_uncdodge", 5, 3, Ability.Dexterity, Ability.Wisdom));

      // ORIGINS

      learnableDictionary.Add(CustomSkill.Acolyte, new LearnableSkill(CustomSkill.Acolyte, "Acolyte", "", Category.StartingTraits, "Acolyte", 1, 1, Ability.Wisdom, Ability.Charisma, HandleAcolyteBackground, "1JU5_KaJTVhoy4PyGFo5sIBIPUbLWe6tMNENQ2kR2WFY"));
      learnableDictionary.Add(CustomSkill.Anthropologist, new LearnableSkill(CustomSkill.Anthropologist, "Anthropologue", "", Category.StartingTraits, "Anthropologue", 1, 1, Ability.Wisdom, Ability.Intelligence, HandleAnthropologistBackground, "1KLiNxm_dHLbRh-dveP--LAfcIMHCjHhcX98a7xzZGOI"));
      learnableDictionary.Add(CustomSkill.Archeologist, new LearnableSkill(CustomSkill.Archeologist, "Archéologue", "", Category.StartingTraits, "archeologist", 1, 1, Ability.Intelligence, Ability.Wisdom, HandleArcheologistBackground, "1ULJttGDVkgc5vsk9DvzEJqG53Yuh_meh59T7TWkmpVs"));
      learnableDictionary.Add(CustomSkill.CloisteredScholar, new LearnableSkill(CustomSkill.CloisteredScholar, "Erudit", "", Category.StartingTraits, "Erudit", 1, 1, Ability.Wisdom, Ability.Intelligence, HandleScholarBackground, "1JHepuecYSMwwqwlxMitBMWEOvm5l1mgRaQiKI7In2G0"));
      learnableDictionary.Add(CustomSkill.Sage, new LearnableSkill(CustomSkill.Sage, "Sage", "", Category.StartingTraits, "Sage", 1, 1, Ability.Wisdom, Ability.Intelligence, HandleSageBackground, "1AdvUpfuXxrIdv35Go4poPSFFm_4tlVvzJK5cXmR_QMw"));
      learnableDictionary.Add(CustomSkill.Hermit, new LearnableSkill(CustomSkill.Hermit, "Ermite", "", Category.StartingTraits, "Ermite", 1, 1, Ability.Wisdom, Ability.Constitution, HandleHermitBackground, "1jPUik90zrJ7XhNVNILd0MhaWOmLqA9XRpQ8MTnNffBA"));
      learnableDictionary.Add(CustomSkill.Wanderer, new LearnableSkill(CustomSkill.Wanderer, "Voyageur", "", Category.StartingTraits, "Voyageur", 1, 1, Ability.Constitution, Ability.Wisdom, HandleWandererBackground, "1X2s8SwAG8I3AgDuB7Mo-yaWVpk3_AZmaXZtG2pDrdMc")); 
      learnableDictionary.Add(CustomSkill.Athlete, new LearnableSkill(CustomSkill.Athlete, "Athlète", "", Category.StartingTraits, "athlete", 1, 1, Ability.Strength, Ability.Constitution, HandleAthleteBackground, "15h9-KjZ0sjS1yvstLjLEf3mumjJ4Xq5E-pbmgfAv9Xw"));
      learnableDictionary.Add(CustomSkill.Outlander, new LearnableSkill(CustomSkill.Outlander, "Sauvage", "", Category.StartingTraits, "Sauvage", 1, 1, Ability.Strength, Ability.Wisdom, HandleOutlanderBackground, "1qm3URzCigQ_xIz-BPT4kjLdXhtvyfFBI5F9ZEuxvweQ"));
      learnableDictionary.Add(CustomSkill.Soldier, new LearnableSkill(CustomSkill.Soldier, "Soldat", "", Category.StartingTraits, "Soldat", 1, 1, Ability.Strength, Ability.Constitution, HandleSoldierBackground, "1QKnLB4iEuX8pNmqPXDfV8SSSeDcsT0_9e5xMNsCLa0c"));
      learnableDictionary.Add(CustomSkill.Mercenary, new LearnableSkill(CustomSkill.Mercenary, "Mercenaire", "", Category.StartingTraits, "Mercenaire", 1, 1, Ability.Strength, Ability.Dexterity, HandleMercenaryBackground, "1vDKqHBxFtjmhn25r0dhVruaMgzfSWxFV7D2grdPtDso"));
      learnableDictionary.Add(CustomSkill.FolkHero, new LearnableSkill(CustomSkill.FolkHero, "Héros du peuple", "", Category.StartingTraits, "folk_hero", 1, 1, Ability.Strength, Ability.Dexterity, HandleFolkHeroBackground, "1S4BK_DoT2tnV1EjMYvvVTrg-mR0tI-3uRBCyry5twi0"));
      learnableDictionary.Add(CustomSkill.Sailor, new LearnableSkill(CustomSkill.Sailor, "Marin", "", Category.StartingTraits, "Marin", 1, 1, Ability.Dexterity, Ability.Constitution, HandleSailorBackground, "15sc6ymheE3JJpcg8qR_ATyB5xxj-aZAc5Ei2XFcsHpE"));
      learnableDictionary.Add(CustomSkill.Shipwright, new LearnableSkill(CustomSkill.Shipwright, "Charpentier Naval", "", Category.StartingTraits, "carpenter", 1, 1, Ability.Strength, Ability.Dexterity, HandleShipwrightBackground, "1pA026_rZo7PlCrpwbq3zJz2P_UCcyLxITX3EKabuXP4"));
      learnableDictionary.Add(CustomSkill.Fisher, new LearnableSkill(CustomSkill.Fisher, "Pêcheur", "", Category.StartingTraits, "fisher", 1, 1, Ability.Constitution, Ability.Wisdom, HandleFisherBackground, "19uXzfsD2RzNYb3ledV2CMHjEXrJCCCb6uYDlpIYmxmw"));
      learnableDictionary.Add(CustomSkill.Marine, new LearnableSkill(CustomSkill.Marine, "Officier de la marine", "", Category.StartingTraits, "officer", 1, 1, Ability.Strength, Ability.Constitution, HandleMarineBackground, "1g4Hoj6WS1uAAcvpNyrTGju0Mh81H9aIecK9B---sAgk"));
      learnableDictionary.Add(CustomSkill.Criminal, new LearnableSkill(CustomSkill.Criminal, "Criminel", "", Category.StartingTraits, "Criminel", 1, 1, Ability.Dexterity, Ability.Strength, HandleCriminalBackground, "1l0m9pkIcfVy37ZjPl9wEB7s-PD2OwRt9Tco_KUhT-xI"));
      learnableDictionary.Add(CustomSkill.Charlatan, new LearnableSkill(CustomSkill.Charlatan, "Charlatan", "", Category.StartingTraits, "Charlatan", 1, 1, Ability.Charisma, Ability.Intelligence, HandleCharlatanBackground, "1ps07V3Lbp18RMIwrkYYGGyxC3tk5L8Y97zpJHw0eqO8"));
      learnableDictionary.Add(CustomSkill.Smuggler, new LearnableSkill(CustomSkill.Smuggler, "Contrebandier", "", Category.StartingTraits, "contrebandier", 1, 1, Ability.Charisma, Ability.Dexterity, HandleSmugglerBackground, "1BRYovMiish9iFnN5Q77cW14vP6bGjO14m32NzGJFiV4"));
      learnableDictionary.Add(CustomSkill.StreetUrchin, new LearnableSkill(CustomSkill.StreetUrchin, "Gosse des rues", "", Category.StartingTraits, "urchin", 1, 1, Ability.Dexterity, Ability.Charisma, HandleUrchinBackground, "1vtim0ITSkBzl5IlPGjhKuZOTCyBYA_GHLc8M09t1IoM"));
      learnableDictionary.Add(CustomSkill.Gambler, new LearnableSkill(CustomSkill.Gambler, "Parieur", "", Category.StartingTraits, "parieur", 1, 1, Ability.Dexterity, Ability.Charisma, HandleGamblerBackground, "1HkPJH8uqCCn4k4J8HUh3g52v4TfAEFAYiFJlXYhEvuM"));
      learnableDictionary.Add(CustomSkill.Entertainer, new LearnableSkill(CustomSkill.Entertainer, "Saltimbanque", "", Category.StartingTraits, "Saltimbanque", 1, 1, Ability.Charisma, Ability.Dexterity, HandleEntertainerBackground, "1Y87LKyg4DLKdlzUcfFtMj7M1rkSh6yAXx6Cb6Q16Dug"));
      learnableDictionary.Add(CustomSkill.CityWatch, new LearnableSkill(CustomSkill.CityWatch, "Agent du guet", "", Category.StartingTraits, "guet", 1, 1, Ability.Strength, Ability.Constitution, HandleCityWatchBackground, "1JmIBbWSJ6oec820F-4TYeShM43XAKtOtCz2vBf2lPT4"));
      learnableDictionary.Add(CustomSkill.Investigator, new LearnableSkill(CustomSkill.Investigator, "Détective", "", Category.StartingTraits, "detective", 1, 1, Ability.Dexterity, Ability.Intelligence, HandleInvestigatorBackground, "1wMwqmw3jVGFAnQCDa-ayjqeKUL2QGXPyP7KIc7QoDy8"));
      learnableDictionary.Add(CustomSkill.KnightOfTheOrder, new LearnableSkill(CustomSkill.KnightOfTheOrder, "Chevalier de l'Ordre", "", Category.StartingTraits, "chevalier", 1, 1, Ability.Strength, Ability.Charisma, HandleKnightBackground, "1psb8aH-EaKINYKif3XC-MG3mtTFgh-5MYAehcJnCxl4"));
      learnableDictionary.Add(CustomSkill.Noble, new LearnableSkill(CustomSkill.Noble, "Noble", "", Category.StartingTraits, "Noble", 1, 1, Ability.Charisma, Ability.Constitution, HandleNobleBackground, "1_KAkFnH9Ydt2s0ljOGvwn-7mo_Vk5PrqtqIwJZ48k-Q"));
      learnableDictionary.Add(CustomSkill.Courtier, new LearnableSkill(CustomSkill.Courtier, "Courtisan", "", Category.StartingTraits, "Courtisan", 1, 1, Ability.Charisma, Ability.Intelligence, HandleCourtierBackground, "1B1C2bcvU9HBb-d2m1lnFhnHWN2a46hp3zWDb9nBZnJU"));
      learnableDictionary.Add(CustomSkill.FailedMerchant, new LearnableSkill(CustomSkill.FailedMerchant, "Marchand ruiné", "", Category.StartingTraits, "ruined_merchant", 1, 1, Ability.Intelligence, Ability.Constitution, HandleMerchantBackground, "1-2AuXuxSW1PICZWicsGUcb8Sgh_rGTVUYc6eT2zWJO8"));
      learnableDictionary.Add(CustomSkill.Taken, new LearnableSkill(CustomSkill.Taken, "Captif", "", Category.StartingTraits, "Captif", 1, 1, Ability.Constitution, Ability.Dexterity, HandleTakenBackground, "16_6ygOZjsfJF7Ngk5VZ9gSK_nlAz7kqy5-sSDlVVPxw"));
      learnableDictionary.Add(CustomSkill.Heir, new LearnableSkill(CustomSkill.Heir, "Héritier", "", Category.StartingTraits, "heir", 1, 1, Ability.Charisma, Ability.Constitution, HandleScionBackground, "1_D4_FywpDXAJXABkhpuwMAUkg68dsRiU07p-9Q-XSiA"));
      learnableDictionary.Add(CustomSkill.Magistrate, new LearnableSkill(CustomSkill.Magistrate, "Magistrat", "", Category.StartingTraits, "Magristrat", 1, 1, Ability.Intelligence, Ability.Wisdom, HandleMagistrateBackground, "16w21xr6HgBE159pLr1Br0mf7T00zsIzonGdXTGekIYs"));
      learnableDictionary.Add(CustomSkill.AdventurerScion, new LearnableSkill(CustomSkill.AdventurerScion, "Héritier d'un célèbre aventurier", "", Category.StartingTraits, "scion", 1, 1, Ability.Charisma, Ability.Dexterity, HandleScionBackground, "1S7UROAImbnZdGf5Q_CkScJ_gfmfRDBcHGhQ9LpqHoAg"));
      learnableDictionary.Add(CustomSkill.Refugee, new LearnableSkill(CustomSkill.Refugee, "Réfugié", "", Category.StartingTraits, "refugee", 1, 1, Ability.Dexterity, Ability.Constitution, HandleRefugeeBackground, "1GCBVKWeDNR20kqOqKwIex8qlCbOltNmUpkblINEShYM"));
      learnableDictionary.Add(CustomSkill.Prisoner, new LearnableSkill(CustomSkill.Prisoner, "Prisonnier", "", Category.StartingTraits, "prisoner", 1, 1, Ability.Constitution, Ability.Charisma, HandlePrisonerBackground, "1Qdyz-fNuGrqI64NYaAP6wmiQd7GhUz0-hqf5F-vrYps"));
      learnableDictionary.Add(CustomSkill.HauntedOne, new LearnableSkill(CustomSkill.HauntedOne, "Tourmenté", "", Category.StartingTraits, "tormented", 1, 1, Ability.Constitution, Ability.Charisma, HandleHauntedBackground, "1yrgm7p09M0_-Y4nDkxY7gtT1LaO1Av305zBDBoqe72M"));
      learnableDictionary.Add(CustomSkill.Faceless, new LearnableSkill(CustomSkill.Faceless, "Sans-visage", "", Category.StartingTraits, "faceless", 1, 1, Ability.Charisma, Ability.Constitution, HandleFacelessBackground, "1ghCYrBt8e58F5QQB5gvyn294XZO7jHkdvSIYZfYgS9g"));
      learnableDictionary.Add(CustomSkill.SecretIdentity, new LearnableSkill(CustomSkill.SecretIdentity, "Identité Secrète", "", Category.StartingTraits, "secret_identity", 1, 1, Ability.Charisma, Ability.Dexterity, HandleSecretBackground, "1EevCfGvIUXDSx2iEJwPMwN3BDuNGBMR_GSQKZqiIsYQ"));

      // LANGUAGES

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

      //learnableDictionary.Add(CustomSkill.ImprovedAttackBonus, new LearnableSkill(CustomSkill.ImprovedAttackBonus, "Attaque améliorée", "Augmente la pénétration d'armure d'un point par niveau.", Category.Fight, "ife_tough", 12, 2, Ability.Constitution, Ability.Dexterity, false, HandleImproveAttack));
      //learnableDictionary.Add(CustomSkill.ImprovedCasterLevel, new LearnableSkill(CustomSkill.ImprovedCasterLevel, "Maîtrise des sorts", "Augmente le niveau de lanceur de sorts d'un point par niveau.", Category.Magic, "ife_tough", 12, 3, Ability.Constitution, Ability.Charisma));

      // RACES
      // HUMAN

      learnableDictionary.Add(CustomSkill.HumanVersatility, new LearnableSkill(CustomSkill.HumanVersatility, "Versatile", "Les humains sont caractérisé par leur rapidité à s'adapter à leur environnement. Ils bénéficient des avantages suivants:\n- Maîtrise d'une compétence supplémentaire\n- Capables de porter 25% de poids supplémentaire\n- Vitesse d'apprentissage doublée pour la première languée étudiée\n- Maîtrise des lances, des armures légères et des boucliers", Category.Race, "race_human", 1, 1, Ability.Constitution, Ability.Strength));

      // HIGHELF

      learnableDictionary.Add(CustomSkill.HighElfLanguage, new LearnableSkill(CustomSkill.HighElfLanguage, "Supérieurement cultivé", "Les haut-elfes apprennent leur première langue deux fois plus rapidement.", Category.Race, "race_highelf", 1, 1, Ability.Intelligence, Ability.Wisdom));

      // CLASSES
      // FIGHTER

      learnableDictionary.Add(CustomSkill.Fighter, new LearnableSkill(CustomSkill.Fighter, "Guerrier", "", Category.Class, "fighter", 12, 1, Ability.Strength, Ability.Dexterity, Fighter.LevelUp, "14508VWlYNEYcZoXhO4vUo81s4AHbUOgJ6Rjh9nt86Ys"));
      learnableDictionary.Add(CustomSkill.FighterSecondWind, new LearnableSkill(CustomSkill.FighterSecondWind, "Second Souffle", "Action bonus\nVous soigne de 1d10 + votre niveau de guerrier\nRécupération : repos court", Category.Fight, "ief_SecondWind", 1, 1, Ability.Constitution, Ability.Strength, LearnActivableFeat));
      learnableDictionary.Add(CustomSkill.FighterSurge, new LearnableSkill(CustomSkill.FighterSurge, "Fougue", "Action gratuite\nVous bénéficiez d'une attaque supplémentaire.\nDurée et cooldown : 10 rounds\nRécupération : repos court", Category.Fight, "ief_SecondWind", 1, 1, Ability.Constitution, Ability.Strength, LearnActivableFeat));
      //learnableDictionary.Add(CustomSkill.FighterCombatStyles, new LearnableSkill(CustomSkill.FighterCombatStyles, "Choix - Style de combat", "Choississez un style de combat parmi Archerie, Défense, Duel, Arme à deux mains, Protection, Combat à deux armes.", Category.Fight, "ife_armor_l", 1, 10, Ability.Strength, Ability.Dexterity, Fighter.FighterLevelUp));

      // SPELLS
      // CANTRIPS

      learnableDictionary.Add(CustomSkill.RayOfFrost, new LearnableSkill(CustomSkill.RayOfFrost, "Rayon de Givre", "", Category.Magic, NwSpell.FromSpellType(Spell.RayOfFrost).Description.ToString(), 1, 1, Ability.Intelligence, Ability.Charisma, LearnActivableFeat, "14508VWlYNEYcZoXhO4vUo81s4AHbUOgJ6Rjh9nt86Ys"));

      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot0, new LearnableSkill(CustomSkill.ImprovedSpellSlot0, "Emplacement Cercle 0", "Augmente le nombre d'emplacements de sorts de cercle 0 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsC1", 10, 1, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot1, new LearnableSkill(CustomSkill.ImprovedSpellSlot1, "Emplacement Cercle 1", "Augmente le nombre d'emplacements de sorts de cercle 1 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsA1", 10, 2, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot2, new LearnableSkill(CustomSkill.ImprovedSpellSlot2, "Emplacement Cercle 2", "Augmente le nombre d'emplacements de sorts de cercle 2 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsF1", 10, 3, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot3, new LearnableSkill(CustomSkill.ImprovedSpellSlot3, "Emplacement Cercle 3", "Augmente le nombre d'emplacements de sorts de cercle 3 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsE1", 10, 4, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot4, new LearnableSkill(CustomSkill.ImprovedSpellSlot4, "Emplacement Cercle 4", "Augmente le nombre d'emplacements de sorts de cercle 4 disponibles d'un par niveau.", Category.Magic, "ife_X2EnrRsS1", 10, 5, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot5, new LearnableSkill(CustomSkill.ImprovedSpellSlot5, "Emplacement Cercle 5", "Augmente le nombre d'emplacements de sorts de cercle 5 disponibles d'un par niveau.", Category.Magic, "ife_X2EpSkFSpCr", 10, 6, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot6, new LearnableSkill(CustomSkill.ImprovedSpellSlot6, "Emplacement Cercle 6", "Augmente le nombre d'emplacements de sorts de cercle 6 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicFort", 10, 7, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot7, new LearnableSkill(CustomSkill.ImprovedSpellSlot7, "Emplacement Cercle 7", "Augmente le nombre d'emplacements de sorts de cercle 7 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicRefl", 10, 8, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot8, new LearnableSkill(CustomSkill.ImprovedSpellSlot8, "Emplacement Cercle 8", "Augmente le nombre d'emplacements de sorts de cercle 8 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicProw", 10, 9, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));
      learnableDictionary.Add(CustomSkill.ImprovedSpellSlot9, new LearnableSkill(CustomSkill.ImprovedSpellSlot9, "Emplacement Cercle 9", "Augmente le nombre d'emplacements de sorts de cercle 9 disponibles d'un par niveau.", Category.Magic, "ife_X2EpicRepu", 10, 10, Ability.Charisma, Ability.Constitution, HandleAddedSpellSlot));

      learnableDictionary.Add(CustomSkill.MateriaScanning, new LearnableSkill(CustomSkill.MateriaScanning, "Détection de matéria", "Permet l'utilisation des inscriptions de détection de matéria afin de trouver des dépôts de minerais riches en Substance.\nEn mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration et de 1 % celle de découvrir une qualité supérieure.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Intelligence));

      learnableDictionary.Add(CustomSkill.OreDetection, new LearnableSkill(CustomSkill.OreDetection, "Détection de matéria minérale", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt minéral de faible concentration et de 1 % celle de découvrir une qualité supérieure.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.WoodDetection, new LearnableSkill(CustomSkill.WoodDetection, "Détection de matéria végétale", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt végétal de faible concentration et de 1 % celle de découvrir une qualité supérieure.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.PeltDetection, new LearnableSkill(CustomSkill.PeltDetection, "Détection de matéria animale", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt animal de faible concentration et de 1 % celle de découvrir une qualité supérieure.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.OreDetectionSpeed, new LearnableSkill(CustomSkill.OreDetectionSpeed, "Détection minérale rapide", "Chaque niveau diminue de 5 % le temps de recherche nécessaire à la détection de matéria minérale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionSpeed, new LearnableSkill(CustomSkill.WoodDetectionSpeed, "Détection végétale rapide", "Chaque niveau diminue de 5 % le temps de recherche nécessaire à la détection de matéria végétale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionSpeed, new LearnableSkill(CustomSkill.PeltDetectionSpeed, "Détection animale rapide", "Chaque niveau diminue de 5 % le temps de recherche nécessaire à la détection de matéria animale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionSafe, new LearnableSkill(CustomSkill.OreDetectionSafe, "Détection minérale prudente", "Chaque niveau diminue de 2 % le risque d'épuisement d'une inscription lors de la détection de matéria minérale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionSafe, new LearnableSkill(CustomSkill.WoodDetectionSafe, "Détection végétale prudente", "Chaque niveau diminue de 2 % le risque d'épuisement d'une inscription lors de la détection de matéria végétale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionSafe, new LearnableSkill(CustomSkill.PeltDetectionSafe, "Détection animale prudente", "Chaque niveau diminue de 2 % le risque d'épuisement d'une inscription lors de la détection de matéria animale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionRange, new LearnableSkill(CustomSkill.OreDetectionRange, "Détection minérale élargie", "Chaque niveau augmente le rayon de détection passif de dépôts minéraux de 1.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionRange, new LearnableSkill(CustomSkill.WoodDetectionRange, "Détection végétale élargie", "Chaque niveau augmente le rayon de détection passif de dépôts végétaux de 1.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionRange, new LearnableSkill(CustomSkill.PeltDetectionRange, "Détection animale élargie", "Chaque niveau augmente le rayon de détection passif de dépôts animaux de 1.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionEstimation, new LearnableSkill(CustomSkill.OreDetectionEstimation, "Détection minérale précise", "Chaque niveau augmente de 5 % la précision de l'estimation de concentration de matéria d'un dépôt minérale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionEstimation, new LearnableSkill(CustomSkill.WoodDetectionEstimation, "Détection végétale précise", "Chaque niveau augmente de 5 % la précision de l'estimation de concentration de matéria d'un dépôt arboricole.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionEstimation, new LearnableSkill(CustomSkill.PeltDetectionEstimation, "Détection animale précise", "Chaque niveau augmente de 5 % la précision de l'estimation de concentration de matéria d'un dépôt animal.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionAccuracy, new LearnableSkill(CustomSkill.OreDetectionAccuracy, "Détection minérale sensible", "En mode actif, chaque niveau augmente de 1 % la chance de détection d'un dépôt minéral de faible concentration de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodDetectionAccuracy, new LearnableSkill(CustomSkill.WoodDetectionAccuracy, "Détection végétale sensible", "En mode actif, chaque niveau augmente de 1 % la chance de détection d'un dépôt végétal de faible concentration de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltDetectionAccuracy, new LearnableSkill(CustomSkill.PeltDetectionAccuracy, "Détection animale sensible", "En mode actif, chaque niveau augmente de 1 % la chance de détection d'un dépôt animal de faible concentration de qualité supérieure.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreDetectionAdvanced, new LearnableSkill(CustomSkill.OreDetectionAdvanced, "Détection minérale avancée", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodDetectionAdvanced, new LearnableSkill(CustomSkill.WoodDetectionAdvanced, "Détection végétale avancée", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.PeltDetectionAdvanced, new LearnableSkill(CustomSkill.PeltDetectionAdvanced, "Détection animale avancée", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.OreDetectionMastery, new LearnableSkill(CustomSkill.OreDetectionMastery, "Détection minérale maitrisée", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodDetectionMastery, new LearnableSkill(CustomSkill.WoodDetectionMastery, "Détection végétale maitrisée", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Constitution, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.PeltDetectionMastery, new LearnableSkill(CustomSkill.PeltDetectionMastery, "Détection animale maitrisée", "En mode actif, chaque niveau augmente de 5 % la chance de révélation d'un dépôt de faible concentration.\nChaque niveau augmente la précision de l'estimation de quantité de matéria du filon de 5 %.\nChaque niveau diminue de 5 % le temps de recherche nécessaire.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Constitution, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.MateriaExtraction, new LearnableSkill(CustomSkill.MateriaExtraction, "Extraction de matéria", "Permet l'utilisation des inscriptions d'extraction de matéria afin d'obtenir de la matéria à partir de dépôts naturels.\n\nChaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Strength, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.OreExtraction, new LearnableSkill(CustomSkill.OreExtraction, "Extraction minérale", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'un dépôt minéral ainsi que la vitesse d'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreExtractionSpeed, new LearnableSkill(CustomSkill.OreExtractionSpeed, "Extraction minérale accélérée", "Chaque niveau augmente de 5 % la vitesse d'extraction d'un dépôt minéral.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreExtractionYield, new LearnableSkill(CustomSkill.OreExtractionYield, "Extraction minérale améliorée", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'un dépôt minéral.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreExtractionSafe, new LearnableSkill(CustomSkill.OreExtractionSafe, "Extraction minérale prudente", "Chaque niveau réduit de 2 % le risque d'épuisement d'une inscription lors d'une extraction minérale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.OreExtractionDurable, new LearnableSkill(CustomSkill.OreExtractionDurable, "Extraction minérale durable", "Chaque niveau augmente de 1 % la chance de conserver un dépôt minéral intact même lorsque l'extraction provoque son épuisement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.OreExtractionAdvanced, new LearnableSkill(CustomSkill.OreExtractionAdvanced, "Extraction minérale avancée", "Chaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.OreExtractionMastery, new LearnableSkill(CustomSkill.OreExtractionMastery, "Extraction minérale maîtrise", "Chaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Strength, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.WoodExtraction, new LearnableSkill(CustomSkill.WoodExtraction, "Extraction végétale", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'une dépôt végétal ainsi que la vitesse d'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionSpeed, new LearnableSkill(CustomSkill.WoodExtractionSpeed, "Extraction végétale accélérée", "Chaque niveau augmente de 5 % la vitesse d'extraction d'un dépôt végétal.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionYield, new LearnableSkill(CustomSkill.WoodExtractionYield, "Extraction végétale améliorée", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'un dépôt végétal.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Strength, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionSafe, new LearnableSkill(CustomSkill.WoodExtractionSafe, "Extraction végétale prudente", "Chaque niveau réduit de 2 % le risque d'épuisement d'une inscription lors d'une extraction végétale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.WoodExtractionDurable, new LearnableSkill(CustomSkill.WoodExtractionDurable, "Extraction végétale durable", "Chaque niveau augmente de 1 % la chance de conserver un dépôt végétal intact même lorsque l'extraction provoque son épuisement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.WoodExtractionAdvanced, new LearnableSkill(CustomSkill.WoodExtractionAdvanced, "Extraction végétale avancée", "Chaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Strength, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.WoodExtractionMastery, new LearnableSkill(CustomSkill.WoodExtractionMastery, "Extraction végétale maîtrise", "Chaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Strength, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.PeltExtraction, new LearnableSkill(CustomSkill.PeltExtraction, "Extraction animale", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'un dépôt animal ainsi que la vitesse d'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.\"", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionSpeed, new LearnableSkill(CustomSkill.PeltExtractionSpeed, "Extraction animale accélérée", "Chaque niveau augmente de 5 % la vitesse d'extraction d'un dépôt animal.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionYield, new LearnableSkill(CustomSkill.PeltExtractionYield, "Extraction animale améliorée", "Chaque niveau augmente de 5 % la quantité de matéria extraite d'un dépôt animal.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionSafe, new LearnableSkill(CustomSkill.PeltExtractionSafe, "Extraction animale prudente", "Chaque niveau réduit de 2 % le risque d'épuisement d'une inscription lors d'une extraction animale.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.PeltExtractionDurable, new LearnableSkill(CustomSkill.PeltExtractionDurable, "Extraction animale durable", "Chaque niveau réduit de 1 % le risque de destruction du dépôt lorsque l'extraction animale le vide complètement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Dexterity));
      learnableDictionary.Add(CustomSkill.PeltExtractionAdvanced, new LearnableSkill(CustomSkill.PeltExtractionAdvanced, "Extraction animale avancée", "Chaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.PeltExtractionMastery, new LearnableSkill(CustomSkill.PeltExtractionMastery, "Extraction animale maîtrise", "Chaque niveau diminue de 5 % le temps nécessaire à une extraction.\nChaque niveau augmente de 5 % le rendement de l'extraction.\nChaque niveau augmente de 1 % la chance d'obtenir une matéria de concentration supérieure et diminue de 1 % le risque d'en obtenir une de concentration inférieure.", Category.Craft, "ife_X2EpicRepu", 5, 5, Ability.Dexterity, Ability.Constitution));

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
      learnableDictionary.Add(CustomSkill.ArtisanExceptionnel, new LearnableSkill(CustomSkill.ArtisanExceptionnel, "Artisan d'exception", "Augmente de 1 % par niveau la chance de parvenir à produire un objet avec un emplacement d'inscription supplémentaire.", Category.Craft, "ife_X2EpicRepu", 10, 3, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.ArtisanApplique, new LearnableSkill(CustomSkill.ArtisanApplique, "Artisan appliqué", "Augmente de 3 % par niveau la chance d'augmenter la durabilité d'un objet lors de sa fabrication.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ArtisanFocus, new LearnableSkill(CustomSkill.ArtisanFocus, "Artisan concentré", "Augmente de 5 % le gain de durabilité obtenu lors de l'activation de la compétence artisan appliqué.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.ArtisanPrudent, new LearnableSkill(CustomSkill.ArtisanPrudent, "Artisan Prudent", "Chaque niveau diminue de 2 % le risque d'épuisement d'une inscription lors de la manipulation de matéria raffinée.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Dexterity));

      learnableDictionary.Add(CustomSkill.Renforcement, new LearnableSkill(CustomSkill.Renforcement, "Renforcement", "Permet d'augmenter la durabilité d'un objet de 5 % par renforcement. Cumulable 10 fois.\n\nDiminue le temps de travail nécessaire de 5 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Intelligence, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.Recycler, new LearnableSkill(CustomSkill.Recycler, "Recyclage", "Permet de mettre en pièces les objets afin d'extraire une fraction de la matéria brute qu'ils contiennent.\n\n Diminue le temps nécessaire au recyclage et augmente le rendement de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.RecyclerFast, new LearnableSkill(CustomSkill.RecyclerFast, "Recyclage accéléré", "Permet d'effectuer une tâche de recyclage 1 % plus rapidement par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.RecyclerExpert, new LearnableSkill(CustomSkill.RecyclerExpert, "Recyclage expert", "Augmente le rendement du recyclage de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.Repair, new LearnableSkill(CustomSkill.Repair, "Réparation", "Permet de réparer les objets.\n\n Diminue le temps et le coût en matéria nécessaires à la réparation.", Category.Craft, "ife_X2EpicRepu", 10, 1, Ability.Dexterity, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.RepairFast, new LearnableSkill(CustomSkill.RepairFast, "Réparation accélérée", "Permet d'effectuer une tâche de réparation 1 % plus rapidement par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Intelligence));
      learnableDictionary.Add(CustomSkill.RepairExpert, new LearnableSkill(CustomSkill.RepairExpert, "Réparation experte", "Diminue le coût en matéria d'une réparation de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Wisdom));
      learnableDictionary.Add(CustomSkill.RepairCareful, new LearnableSkill(CustomSkill.RepairCareful, "Réparation prudente", "Diminue la perte de durabilité maximale à chaque réparation de 1 % par niveau.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Dexterity, Ability.Wisdom));

      learnableDictionary.Add(CustomSkill.CalligraphieSurcharge, new LearnableSkill(CustomSkill.CalligraphieSurcharge, "Calligraphie Surchargée", "Permet de forcer l'ajout d'emplacements d'inscriptions sur un objet au risque de le briser.\n\nAugmente de 2 % par niveau la chance de parvenir à forcer l'ajout d'un emplacement d'enchantement supplémentaire.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Constitution));
      learnableDictionary.Add(CustomSkill.CalligraphieSurchargeControlee, new LearnableSkill(CustomSkill.CalligraphieSurchargeControlee, "Calligraphie surcharge contrôlée", "Augmente de 10 % par niveau la chance de conserver l'objet intact lors de l'échec d'une tentative de surcharge.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Wisdom, Ability.Constitution));

      learnableDictionary.Add(CustomSkill.CalligrapheArmurier, new LearnableSkill(CustomSkill.CalligrapheArmurier, "Calligraphe Armurier", "Permet de réaliser des inscriptions ouvragées sur des pièces d'armure et de leur donner vie sous forme d'effets magiques permanents.\n\nRéduit de 2 % par niveau le coût et le temps nécessaire pour réaliser une inscription.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheArmurierMaitre, new LearnableSkill(CustomSkill.CalligrapheArmurierMaitre, "Calligraphe Armurier Maître", "Réduit de 2 % par niveau le temps et le coût de réalisation d'une inscription sur une pièce d'armure.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheArmurierScience, new LearnableSkill(CustomSkill.CalligrapheArmurierScience, "Science de la calligraphie d'armure", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription d'une inscription sur une pièce d'armure.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheArmurierExpert, new LearnableSkill(CustomSkill.CalligrapheArmurierExpert, "Expertise de la calligraphie d'armure", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription d'une inscription sur une pièce d'armure.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Wisdom, Ability.Charisma));

      learnableDictionary.Add(CustomSkill.CalligrapheBlindeur, new LearnableSkill(CustomSkill.CalligrapheBlindeur, "Calligraphe Blindeur", "Permet de réaliser des inscriptions ouvragées sur des boucliers et de leur donner vie sous forme d'effets magiques permanents.\n\nRéduit de 2 % par niveau le coût et le temps nécessaire pour réaliser une inscription.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheBlindeurMaitre, new LearnableSkill(CustomSkill.CalligrapheBlindeurMaitre, "Calligraphe Blindeur Maître", "Réduit de 2 % par niveau le temps et le coût de réalisation d'une inscription sur un bouclier.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheBlindeurScience, new LearnableSkill(CustomSkill.CalligrapheBlindeurScience, "Science de la calligraphie de bouclier", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription sur un bouclier.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheBlindeurExpert, new LearnableSkill(CustomSkill.CalligrapheBlindeurExpert, "Expertise de la calligraphie de bouclier", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription sur un bouclier.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Wisdom, Ability.Charisma));

      learnableDictionary.Add(CustomSkill.CalligrapheCiseleur, new LearnableSkill(CustomSkill.CalligrapheCiseleur, "Calligraphe Ciseleur", "Permet de réaliser des inscriptions ouvragées sur un ornement et de leur donner vie sous forme d'effets magiques permanents.\n\nRéduit de 2 % par niveau le coût et le temps nécessaire pour réaliser une inscription.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Intelligence, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheCiseleurMaitre, new LearnableSkill(CustomSkill.CalligrapheCiseleurMaitre, "Calligraphe Ciseleur Maître", "Réduit de 2 % par niveau le temps et le coût de réalisation d'une inscription sur un ornement.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheCiseleurScience, new LearnableSkill(CustomSkill.CalligrapheCiseleurScience, "Science de la calligraphie des ornements", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription sur un ornement.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Intelligence, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheCiseleurExpert, new LearnableSkill(CustomSkill.CalligrapheCiseleurExpert, "Expertise de la calligraphie des ornements", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription sur un ornement.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Intelligence, Ability.Charisma));

      learnableDictionary.Add(CustomSkill.CalligrapheFourbisseur, new LearnableSkill(CustomSkill.CalligrapheFourbisseur, "Calligraphe Coutelier", "Permet de réaliser des inscriptions ouvragées sur une arme et de leur donner vie sous forme d'effets magiques permanents.\n\nRéduit de 2 % par niveau le coût et le temps nécessaire pour réaliser une inscription.", Category.Craft, "ife_X2EpicRepu", 5, 1, Ability.Intelligence, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheFourbisseurMaitre, new LearnableSkill(CustomSkill.CalligrapheFourbisseurMaitre, "Calligraphe Coutelier Maître", "Réduit de 2 % par niveau le temps et le coût de réalisation d'une inscription sur une arme.", Category.Craft, "ife_X2EpicRepu", 5, 2, Ability.Wisdom, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheFourbisseurScience, new LearnableSkill(CustomSkill.CalligrapheFourbisseurScience, "Science de la calligraphie des armes", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription sur une arme.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 3, Ability.Intelligence, Ability.Charisma));
      learnableDictionary.Add(CustomSkill.CalligrapheFourbisseurExpert, new LearnableSkill(CustomSkill.CalligrapheFourbisseurExpert, "Expertise de la calligraphie des armes", "Réduit de 3 % par niveau le temps et le coût de réalisation d'une inscription sur une arme.\n\nAugmente de 1% la chance de ne pas consommer d'emplacement.", Category.Craft, "ife_X2EpicRepu", 5, 4, Ability.Intelligence, Ability.Charisma));

      learnableDictionary.Add(CustomSkill.CombattantPrecautionneux, new LearnableSkill(CustomSkill.CombattantPrecautionneux, "Combattant Précautionneux", "Diminue de 1 % par niveau le risque d'usure des objets.", Category.Fight, "ife_X2EpicRepu", 10, 2, Ability.Dexterity, Ability.Intelligence));

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

      // COMBAT SKILLS
      learnableDictionary.Add(CustomSkill.SeverArtery, new LearnableSkill(CustomSkill.SeverArtery, "Tranche-artère", "Adrénaline : 4\nAttaque à l'épée.\nInflige 5...21...25 secondes de saignement.", Category.Fight, "ife_SeverA", 5, 1, Ability.Strength, Ability.Dexterity));

      // INSCRIPTIONS
      learnableDictionary.Add(CustomInscription.Cuirassé, new LearnableSkill(CustomInscription.Cuirassé, "Cuirassé", "Inscription de pièce d'armure\n\n+1 Armure et consomme un emplacement libre d'inscription.", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Absorption, new LearnableSkill(CustomInscription.Absorption, "Absorption", "Inscription de pièce d'armure\n\n-1 dégâts physiques aux coups portés sur la pièce d'armure où se trouve cette inscription.\nConsomme un emplacement libre d'inscription\nSe cumule jusqu'à -3 maximum.", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Prismatique, new LearnableSkill(CustomInscription.Prismatique, "Prismatique", "Inscription de pièce d'armure\n\n+1 Armure si maîtrise de l'air > 14\n+1 Armure si maîtrise de la terre > 14\n+1 Armure si maîtrise du feu > 14\n+1 Armure si maîtrise de l'eau > 14\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Artisan, new LearnableSkill(CustomInscription.Artisan, "Artisan", "Inscription de pièce d'armure\n\n+1 Armure par sceau équipé\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeDragon, new LearnableSkill(CustomInscription.GardeDragon, "Garde-Dragon", "Inscription de pièce d'armure\n\n+3 Armure contre les dragons\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeExtérieur, new LearnableSkill(CustomInscription.GardeExtérieur, "Garde-Extérieur", "Inscription de pièce d'armure\n\n+3 Armure contre les extérieurs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeAberration, new LearnableSkill(CustomInscription.GardeAberration, "Garde-Aberration", "Inscription de pièce d'armure\n\n+3 Armure contre les aberrations\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeElementaire, new LearnableSkill(CustomInscription.GardeElementaire, "Garde-Elémentaire", "Inscription de pièce d'armure\n\n+3 Armure contre les créatures élémentaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Persécuteur, new LearnableSkill(CustomInscription.Persécuteur, "Persécuteur", "Inscription de pièce d'armure\n\n+1 Armure\n+6 aux dégâts divins reçus sur cette pièce d'armure\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Inflexible, new LearnableSkill(CustomInscription.Inflexible, "Inflexible", "Inscription de pièce d'armure\n\n+1 Armure contre les dégâts physiques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Redoutable, new LearnableSkill(CustomInscription.Redoutable, "Redoutable", "Inscription de pièce d'armure\n\n+1 Armure contre les dégâts élémentaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Marchevent, new LearnableSkill(CustomInscription.Marchevent, "Marche-Vent", "Inscription de pièce d'armure\n\n+1 Armure si affecté par 3 sorts positifs\n+2 Armure si affecté par 4 sorts positifs\n+3 Armure si affecté par 5 sorts positifs\n+4 Armure si affecté par 6 sorts positifs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeGeant, new LearnableSkill(CustomInscription.GardeGeant, "Garde-Géant", "Inscription de pièce d'armure\n\n+3 Armure contre les géants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeMagie, new LearnableSkill(CustomInscription.GardeMagie, "Garde-Magie", "Inscription de pièce d'armure\n\n+3 Armure contre les créatures magiques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeBon, new LearnableSkill(CustomInscription.GardeBon, "Garde-Bien", "Inscription de pièce d'armure\n\n+2 Armure contre les créatures alignées au Bien\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeChaos, new LearnableSkill(CustomInscription.GardeChaos, "Garde-Chaos", "Inscription de pièce d'armure\n\n+2 Armure contre les créatures alignées au Chaos\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeMal, new LearnableSkill(CustomInscription.GardeMal, "Garde-Mal", "Inscription de pièce d'armure\n\n+2 Armure contre les créatures alignées au Mal\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeNeutre, new LearnableSkill(CustomInscription.GardeNeutre, "Garde-Neutre", "Inscription de pièce d'armure\n\n+2 Armure contre les créatures alignées de façon Neutre\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeLoi, new LearnableSkill(CustomInscription.GardeLoi, "Garde-Loi", "Inscription de pièce d'armure\n\n+2 Armure contre les créatures alignées à la Loi\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Hivernal, new LearnableSkill(CustomInscription.Hivernal, "Hivernal", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts Polaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Ignifugé, new LearnableSkill(CustomInscription.Ignifugé, "Ignifugé", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts de Feu\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Paratonnerre, new LearnableSkill(CustomInscription.Paratonnerre, "Paratonerre", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts de Foudre\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Tectonique, new LearnableSkill(CustomInscription.Tectonique, "Tectonique", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts Terrestres\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Infiltrateur, new LearnableSkill(CustomInscription.Infiltrateur, "Infiltrateur", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts Perçants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Saboteur, new LearnableSkill(CustomInscription.Saboteur, "Saboteur", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts Tranchants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.AvantGarde, new LearnableSkill(CustomInscription.AvantGarde, "Avant-Garde", "Inscription de pièce d'armure\n\n+2 Armure contre les dégâts Contondants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeHalfelin, new LearnableSkill(CustomInscription.GardeHalfelin, "Garde-Halfelin", "Inscription de pièce d'armure\n\n+3 Armure contre les Halfelins\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeHumain, new LearnableSkill(CustomInscription.GardeHumain, "Garde-Humain", "Inscription de pièce d'armure\n\n+3 Armure contre les Humains\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeDemiElfe, new LearnableSkill(CustomInscription.GardeDemiElfe, "Garde-Demi-Elfe", "Inscription de pièce d'armure\n\n+3 Armure contre les Demi-Elfes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeDemiOrc, new LearnableSkill(CustomInscription.GardeDemiOrc, "Garde-Demi-Orc", "Inscription de pièce d'armure\n\n+3 Armure contre les Demi-Orcs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeElfe, new LearnableSkill(CustomInscription.GardeElfe, "Garde-Elfe", "Inscription de pièce d'armure\n\n+3 Armure contre les Elfes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeGnome, new LearnableSkill(CustomInscription.GardeGnome, "Garde-Gnome", "Inscription de pièce d'armure\n\n+3 Armure contre les Gnomes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeNain, new LearnableSkill(CustomInscription.GardeNain, "Garde-Gnome", "Inscription de pièce d'armure\n\n+3 Armure contre les Nains\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Agitateur, new LearnableSkill(CustomInscription.Agitateur, "Agitateur", "Inscription de pièce d'armure\n\n+1 Armure en attaquant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Sentinelle, new LearnableSkill(CustomInscription.Sentinelle, "Sentinelle", "Inscription de pièce d'armure\n\n+1 Armure avec une pose de combat\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Belluaire, new LearnableSkill(CustomInscription.Belluaire, "Belluaire", "Inscription de pièce d'armure\n\n+1 Armure tant que votre compagnon animal est vivant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Eclaireur, new LearnableSkill(CustomInscription.Eclaireur, "Eclaireur", "Inscription de pièce d'armure\n\n+1 Armure tant que vous êtes sous l'effet d'une préparation\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Disciple, new LearnableSkill(CustomInscription.Disciple, "Disciple", "Inscription de pièce d'armure\n\n+2 Armure tant que vous êtes affecté par une condition\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Virtuose, new LearnableSkill(CustomInscription.Virtuose, "Virtuose", "Inscription de pièce d'armure\n\n+2 Armure tant que incantez un sort\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Fossoyeur, new LearnableSkill(CustomInscription.Fossoyeur, "Fossoyeur", "Inscription de pièce d'armure\n\n+1 Armure tant que vous êtes à moins de 80 % de votre santé max\n+2 Armure tant que vous êtes à moins de 60 % de votre santé max\n+3 Armure tant que vous êtes à moins de 40 % de votre santé max\n+4 Armure tant que vous êtes à moins de 20 % de votre santé max\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Prodige, new LearnableSkill(CustomInscription.Prodige, "Prodige", "Inscription de pièce d'armure\n\n+1 Armure tant que vous rechargez 2 capacités\n+2 Armure tant que vous rechargez 4 capacités\n+3 Armure tant que vous rechargez 6 capacités\n+4 Armure tant que vous rechargez 8 capacités\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Destructeur, new LearnableSkill(CustomInscription.Destructeur, "Destructeur", "Inscription de pièce d'armure\n\n+3 Armure tant que vous êtes affecté par un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Bénédiction, new LearnableSkill(CustomInscription.Bénédiction, "Bénédiction", "Inscription de pièce d'armure\n\n+1 Armure tant que vous êtes affecté par un sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Centurion, new LearnableSkill(CustomInscription.Centurion, "Bénédiction", "Inscription de pièce d'armure\n\n+1 Armure tant que vous êtes affecté par un cri, un écho ou un chant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Oublié, new LearnableSkill(CustomInscription.Oublié, "Oublié", "Inscription de pièce d'armure\n\n+1 Armure tant que vous n'êtes affecté par aucun sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeNonVie, new LearnableSkill(CustomInscription.GardeNonVie, "Garde-Non-Vie", "Inscription de pièce d'armure\n\n+3 Armure contre les mort-vivants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeArtifice, new LearnableSkill(CustomInscription.GardeArtifice, "Garde-Artifice", "Inscription de pièce d'armure\n\n+3 Armure contre les créatures artificielles\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeOrc, new LearnableSkill(CustomInscription.GardeOrc, "Garde-Orc", "Inscription de pièce d'armure\n\n+3 Armure contre les orcs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Lieutenant, new LearnableSkill(CustomInscription.Lieutenant, "Lieutenant", "Inscription de pièce d'armure\n\n-2% de durée des maléfices\n-4 Armure\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.MaîtreBlème, new LearnableSkill(CustomInscription.MaîtreBlème, "Maître-Blème", "Inscription de pièce d'armure\n\n+1 Armure tant que vous contrôlez 1 mort-vivant\n+2 Armure tant que vous contrôlez 3 mort-vivants\n+3 Armure tant que vous contrôlez 5 mort-vivants\n+4 Armure tant que vous contrôlez 8 mort-vivants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Marionnettiste, new LearnableSkill(CustomInscription.Marionnettiste, "Marionnettiste", "Inscription de pièce d'armure\n\n+1 Armure tant que vous contrôlez 1 invocation\n+2 Armure tant que vous contrôlez 3 invocation\n+3 Armure tant que vous contrôlez 5 invocations\n+4 Armure tant que vous contrôlez 8 invocations\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Chaman, new LearnableSkill(CustomInscription.Chaman, "Chaman", "Inscription de pièce d'armure\n\n+1 Armure tant que vous contrôlez 1 esprit\n+2 Armure tant que vous contrôlez 3 esprits\n+3 Armure tant que vous contrôlez 5 esprits\n+4 Armure tant que vous contrôlez 8 esprits\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeMonstre, new LearnableSkill(CustomInscription.GardeMonstre, "Garde-Monstre", "Inscription de pièce d'armure\n\n+3 Armure contre les Monstres Primitifs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeHumanoïde, new LearnableSkill(CustomInscription.GardeHumanoïde, "Garde-Humanoïde", "Inscription de pièce d'armure\n\n+3 Armure contre les créatures Humanoïdes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeMétamorphe, new LearnableSkill(CustomInscription.GardeMétamorphe, "Garde-Métamorphe", "Inscription de pièce d'armure\n\n+3 Armure contre les Métamorphes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeGoblinoïde, new LearnableSkill(CustomInscription.GardeGoblinoïde, "Garde-Goblinoïde", "Inscription de pièce d'armure\n\n+3 Armure contre les créatures Gobelinoïdes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeAnimal, new LearnableSkill(CustomInscription.GardeAnimal, "Garde-Animal", "Inscription de pièce d'armure\n\n+3 Armure contre les animaux\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeReptilien, new LearnableSkill(CustomInscription.GardeReptilien, "Garde-Reptilien", "Inscription de pièce d'armure\n\n+3 Armure contre les créatures Reptiliennes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.GardeVermine, new LearnableSkill(CustomInscription.GardeVermine, "Garde-Vermine", "Inscription de pièce d'armure\n\n+3 Armure contre les vermines\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Pourfendeur, new LearnableSkill(CustomInscription.Pourfendeur, "Pourfendeur", "Inscription d'arme\n\n+1% Dégâts\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Défense, new LearnableSkill(CustomInscription.Défense, "Défense", "Inscription d'arme\n\n+1 Armure\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Courage, new LearnableSkill(CustomInscription.Courage, "Courage", "Inscription d'arme\n\n+4 Santé\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Fulgurance, new LearnableSkill(CustomInscription.Fulgurance, "Fulgurance", "Inscription d'arme\n\n1% de chance de diminuer de moitié le temps d'incantation de vos sorts\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Extension, new LearnableSkill(CustomInscription.Extension, "Extension", "Inscription d'arme\n\n6% de durées supplémentaires pour les sorts positifs que vous lancez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Vampirisme, new LearnableSkill(CustomInscription.Vampirisme, "Vampirisme", "Inscription d'arme\n\n+1 de drain de vie\n-1 de régénération de santé\nCumulable jusqu'à +3 max\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Zèle, new LearnableSkill(CustomInscription.Zèle, "Zèle", "Inscription d'arme\n\n+1 de gain d'énergie au touché\n-1 de régénération de santé\nNon cumulable\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurDragon, new LearnableSkill(CustomInscription.PourfendeurDragon, "Pourfendeur de Dragon", "Inscription d'arme\n\n+3% de dégâts contre les Dragons\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurExtérieur, new LearnableSkill(CustomInscription.PourfendeurExtérieur, "Pourfendeur d'Extérieur", "Inscription d'arme\n\n+3% de dégâts contre les Extérieurs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurAberration, new LearnableSkill(CustomInscription.PourfendeurAberration, "Pourfendeur d'Aberration", "Inscription d'arme\n\n+3% de dégâts contre les Aberrations\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurElementaire, new LearnableSkill(CustomInscription.PourfendeurElementaire, "Pourfendeur Elémentaire", "Inscription d'arme\n\n+3% de dégâts contre les créature élémentaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.ForceEtHonneur, new LearnableSkill(CustomInscription.ForceEtHonneur, "Force & Honneur", "Inscription d'arme\n\n+2% de dégâts tant que votre santé est au-dessus de 50%\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MaîtreDeSonDestin, new LearnableSkill(CustomInscription.MaîtreDeSonDestin, "Maître de son Destin", "Inscription d'arme\n\n+2% de dégâts tant vous êtes sous l'effet d'un sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.DanseAvecLaMort, new LearnableSkill(CustomInscription.DanseAvecLaMort, "Danse avec la Mort", "Inscription d'arme\n\n+2% de dégâts tant vous êtes sous l'effet d'une pose de combat\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Sadisme, new LearnableSkill(CustomInscription.Sadisme, "Sadisme", "Inscription d'arme\n\n+3% de dégâts contre les cibles affectées par un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Masochisme, new LearnableSkill(CustomInscription.Masochisme, "Masochisme", "Inscription d'arme\n\n+2% de dégâts\n-1 Armure tant que vous attaquez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.QueDuMuscle, new LearnableSkill(CustomInscription.QueDuMuscle, "Que du muscle", "Inscription d'arme\n\n+2% de dégâts\n-1 Energie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Refuge, new LearnableSkill(CustomInscription.Refuge, "Refuge", "Inscription d'arme\n\n+1 Armure contre les dégâts physiques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Protecteur, new LearnableSkill(CustomInscription.Protecteur, "Protecteur", "Inscription d'arme\n\n+1 Armure contre les dégâts élémentaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Dévotion, new LearnableSkill(CustomInscription.Dévotion, "Dévotion", "Inscription d'arme\n\n+6 Santé tant que vous êtes sous l'effet d'un sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Endurance, new LearnableSkill(CustomInscription.Endurance, "Endurance", "Inscription d'arme\n\n+6 Santé tant que vous êtes sous l'effet d'une pose de combat\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Valeur, new LearnableSkill(CustomInscription.Valeur, "Valeur", "Inscription d'arme\n\n+8 Santé tant que vous êtes affecté par un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Sérénité, new LearnableSkill(CustomInscription.Sérénité, "Sérénité", "Inscription d'arme\n\n1% de chance de diviser par deux le temps de recharge de vos capacités\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.ToutAuTalent, new LearnableSkill(CustomInscription.ToutAuTalent, "Tout au Talent", "Inscription d'arme\n\n2% de chance de diviser par deux le temps d'incantation des sorts correspondant à l'attribut de votre arme\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Vision, new LearnableSkill(CustomInscription.Vision, "Vision", "Inscription d'arme\n\n+1 Energie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurGéant, new LearnableSkill(CustomInscription.PourfendeurGéant, "Pourfendeur de Géant", "Inscription d'arme\n\n+3% de dégâts contre les Géants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurMagie, new LearnableSkill(CustomInscription.PourfendeurMagie, "Pourfendeur Magique", "Inscription d'arme\n\n+3% de dégâts contre les créatures Magiques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurBien, new LearnableSkill(CustomInscription.PourfendeurBien, "Pourfendeur du Bien", "Inscription d'arme\n\n+2% de dégâts contre les créatures alignées au Bien\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurChaos, new LearnableSkill(CustomInscription.PourfendeurChaos, "Pourfendeur du Chaos", "Inscription d'arme\n\n+2% de dégâts contre les créatures alignées au Chaos\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurMal, new LearnableSkill(CustomInscription.PourfendeurMal, "Pourfendeur du Mal", "Inscription d'arme\n\n+2% de dégâts contre les créatures alignées au Mal\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurNeutralité, new LearnableSkill(CustomInscription.PourfendeurNeutralité, "Pourfendeur de la Neutralité", "Inscription d'arme\n\n+2% de dégâts contre les créatures alignées de façon Neutre\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurLoi, new LearnableSkill(CustomInscription.PourfendeurLoi, "Pourfendeur de la Loi", "Inscription d'arme\n\n+2% de dégâts contre les créatures alignées à la Loi\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.VengeanceSeraMienne, new LearnableSkill(CustomInscription.VengeanceSeraMienne, "Vengeance sera mienne", "Inscription d'arme\n\n+3% de dégâts tant que votre santé est en-dessous de 50%\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.AccueillezLaFaucheuse, new LearnableSkill(CustomInscription.AccueillezLaFaucheuse, "Accueillez la Faucheuse", "Inscription d'arme\n\n+3% tant que vous êtes affecté par un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Givroclaste, new LearnableSkill(CustomInscription.Givroclaste, "Givroclaste", "Inscription d'arme\n\n+2% de dégâts Polaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Pyroclaste, new LearnableSkill(CustomInscription.Pyroclaste, "Pyroclaste", "Inscription d'arme\n\n+2% de dégâts de Feu\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Electroclaste, new LearnableSkill(CustomInscription.Electroclaste, "Electroclaste", "Inscription d'arme\n\n+2% de dégâts de Foudre\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Séismoclaste, new LearnableSkill(CustomInscription.Séismoclaste, "Séismoclaste", "Inscription d'arme\n\n+2% de dégâts Terrestres\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Aiguillon, new LearnableSkill(CustomInscription.Aiguillon, "Aiguillon", "Inscription d'arme\n\n+1% de dégâts de Perçant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Rasoir, new LearnableSkill(CustomInscription.Rasoir, "Rasoir", "Inscription d'arme\n\n+1% de dégâts de Tranchant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Fracasseur, new LearnableSkill(CustomInscription.Fracasseur, "Fracasseur", "Inscription d'arme\n\n+1% de dégâts de Contondant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurHalfelin, new LearnableSkill(CustomInscription.PourfendeurHalfelin, "Pourfendeur d'Halfelin", "Inscription d'arme\n\n+3% de dégâts contre les Hafelins\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurHumain, new LearnableSkill(CustomInscription.PourfendeurHumain, "Pourfendeur d'Humain", "Inscription d'arme\n\n+3% de dégâts contre les Humains\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurDemiElfe, new LearnableSkill(CustomInscription.PourfendeurDemiElfe, "Pourfendeur de Demi-Elfe", "Inscription d'arme\n\n+3% de dégâts contre les Demi-Elfes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurDemiOrc, new LearnableSkill(CustomInscription.PourfendeurDemiOrc, "Pourfendeur de Demi-Orc", "Inscription d'arme\n\n+3% de dégâts contre les Demi-Orcs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurElfe, new LearnableSkill(CustomInscription.PourfendeurElfe, "Pourfendeur d'Elfe", "Inscription d'arme\n\n+3% de dégâts contre les Elfes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurGnome, new LearnableSkill(CustomInscription.PourfendeurGnome, "Pourfendeur de Gnome", "Inscription d'arme\n\n+3% de dégâts contre les Gnomes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurNain, new LearnableSkill(CustomInscription.PourfendeurNain, "Pourfendeur de Nain", "Inscription d'arme\n\n+3% de dégâts contre les Nains\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.LaSécuritéAvantTout, new LearnableSkill(CustomInscription.LaSécuritéAvantTout, "La sécurité avant tout !", "Inscription d'arme\n\n+1 d'énergie tant que votre santé est au-dessus de 50%\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.AyezFoi, new LearnableSkill(CustomInscription.AyezFoi, "Ayez foi", "Inscription d'arme\n\n+1 d'énergie tant que vous êtes sous l'effet d'un sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.AucunRecours, new LearnableSkill(CustomInscription.AucunRecours, "Aucun recours !", "Inscription d'arme\n\n+2 d'énergie tant que votre santé est en-dessous de 50%\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MienneEstLaPeine, new LearnableSkill(CustomInscription.MienneEstLaPeine, "Mienne est la peine", "Inscription d'arme\n\n+2 d'énergie tant que vous êtes sous l'effet d'un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.CarpeDiem, new LearnableSkill(CustomInscription.CarpeDiem, "Carpe Diem", "Inscription d'arme\n\n+15 d'énergie\n-1 de récupération d'énergie\nNon cumulable\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Sismique, new LearnableSkill(CustomInscription.Sismique, "Sismique", "Inscription d'arme\n\nVotre arme effectue désormais des dégâts Terrestres\nConsomme un emplacement libre d'inscription\nAppliquer cette inscription sur une arme disposant déjà d'une modification de dégâts remplacera la précédente", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Incendiaire, new LearnableSkill(CustomInscription.Incendiaire, "Incendiaire", "Inscription d'arme\n\nVotre arme effectue désormais des dégâts de Feu\nConsomme un emplacement libre d'inscription\nAppliquer cette inscription sur une arme disposant déjà d'une modification de dégâts remplacera la précédente", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Polaire, new LearnableSkill(CustomInscription.Polaire, "Polaire", "Inscription d'arme\n\nVotre arme effectue désormais des dégâts Polaires\nConsomme un emplacement libre d'inscription\nAppliquer cette inscription sur une arme disposant déjà d'une modification de dégâts remplacera la précédente", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Electrocution, new LearnableSkill(CustomInscription.Electrocution, "Electrocution", "Inscription d'arme\n\nVotre arme effectue désormais des dégâts de Foudre\nConsomme un emplacement libre d'inscription\nAppliquer cette inscription sur une arme disposant déjà d'une modification de dégâts remplacera la précédente", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Fureur, new LearnableSkill(CustomInscription.Fureur, "Fureur", "Inscription d'arme\n\n1% de chance de doubler l'adrénaline lors de vos attaques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Pénétration, new LearnableSkill(CustomInscription.Pénétration, "Pénétration", "Inscription d'arme\n\n3% de chance d'obtenir +20% de pénétration d'armure\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurNonVie, new LearnableSkill(CustomInscription.PourfendeurNonVie, "Pourfendeur de la Non-Vie", "Inscription d'arme\n\n+3% de dégâts contre les mort-vivants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurOrc, new LearnableSkill(CustomInscription.PourfendeurOrc, "Pourfendeur d'Orc", "Inscription d'arme\n\n+3% de dégâts contre les Orcs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Maîtrise, new LearnableSkill(CustomInscription.Maîtrise, "Maîtrise", "Inscription d'arme\n\n+3% de chance de donner +1 à la compétence de maîtrise de l'objet lors de l'utilisation d'un sort ou d'une compétence\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Adepte, new LearnableSkill(CustomInscription.Adepte, "Adepte", "Inscription d'arme\n\n+3% de chance de diviser par deux le temps d'incantation des sorts liés à l'attribut de votre arme\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Barbelé, new LearnableSkill(CustomInscription.Barbelé, "Barbelé", "Inscription d'arme\n\n+4% de durée des effets de saignements que vous infligez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Atrocité, new LearnableSkill(CustomInscription.Atrocité, "Atrocité", "Inscription d'arme\n\n+4% de durée des effets de blessures profondes que vous infligez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Handicapant, new LearnableSkill(CustomInscription.Handicapant, "Handicapant", "Inscription d'arme\n\n+4% de durée des effets d'infirmités que vous infligez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Pesanteur, new LearnableSkill(CustomInscription.Pesanteur, "Pesanteur", "Inscription d'arme\n\n+4% de durée des effets des faiblesses que vous infligez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Venimeuse, new LearnableSkill(CustomInscription.Venimeuse, "Venimeuse", "Inscription d'arme\n\n+4% de durée des effets d'empoisonnement que vous infligez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Mutisme, new LearnableSkill(CustomInscription.Mutisme, "Mutisme", "Inscription d'arme\n\n+4% de durée des effets d'étourdissement que vous infligez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurMonstres, new LearnableSkill(CustomInscription.PourfendeurMonstres, "Pourfendeur de Monstres", "Inscription d'arme\n\n+3% de dégâts contre les Monstres Primitifs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurHumanoïdes, new LearnableSkill(CustomInscription.PourfendeurHumanoïdes, "Pourfendeur d'Humanoïdes", "Inscription d'arme\n\n+3% de dégâts contre les créatures Humanoïdes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurMétamorphes, new LearnableSkill(CustomInscription.PourfendeurMétamorphes, "Pourfendeur de Métamorphes", "Inscription d'arme\n\n+3% de dégâts contre les Métamorphes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurGobelins, new LearnableSkill(CustomInscription.PourfendeurGobelins, "Pourfendeur de Gobelins", "Inscription d'arme\n\n+3% de dégâts contre les créatures Gobelinoïdes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurAnimal, new LearnableSkill(CustomInscription.PourfendeurAnimal, "Pourfendeur Animal", "Inscription d'arme\n\n+3% de dégâts contre les animaux\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurReptilien, new LearnableSkill(CustomInscription.PourfendeurReptilien, "Pourfendeur Reptilien", "Inscription d'arme\n\n+3% de dégâts contre les créatures Reptiliennes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PourfendeurVermine, new LearnableSkill(CustomInscription.PourfendeurVermine, "Pourfendeur de Vermine", "Inscription d'arme\n\n+3% de dégâts contre les Vermines\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Blindé, new LearnableSkill(CustomInscription.Blindé, "Blindé", "Inscription de bouclier\n\n+1 d'armure\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseDragon, new LearnableSkill(CustomInscription.RepousseDragon, "Repousse Dragon", "Inscription de bouclier\n\n+2 d'armure contre les Dragons\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseExtérieur, new LearnableSkill(CustomInscription.RepousseExtérieur, "Repousse Extérieur", "Inscription de bouclier\n\n+2 d'armure contre les Extérieurs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseAberration, new LearnableSkill(CustomInscription.RepousseAberration, "Repousse Aberration", "Inscription de bouclier\n\n+2 d'armure contre les Aberrations\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseElementaire, new LearnableSkill(CustomInscription.RepousseElementaire, "Repousse Elémentaire", "Inscription de bouclier\n\n+2 d'armure contre les créatures élémentaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.LongueVieAuRoi, new LearnableSkill(CustomInscription.LongueVieAuRoi, "Longue vie au Roi !", "Inscription de bouclier\n\n+1 d'armure tant que votre santé est au-dessus de 50%\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.LaFoiEstMonBouclier, new LearnableSkill(CustomInscription.LaFoiEstMonBouclier, "La Foi est mon bouclier", "Inscription de bouclier\n\n+1 d'armure tant que vous êtes sous l'effet d'un sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.LaSurvieDuMieuxEquipé, new LearnableSkill(CustomInscription.LaSurvieDuMieuxEquipé, "La survie du mieux équipé", "Inscription de bouclier\n\n+1 d'armure contre les dégâts physiques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.ParéEnTouteSaison, new LearnableSkill(CustomInscription.ParéEnTouteSaison, "Paré en toute saison", "Inscription de bouclier\n\n+1 d'armure contre les dégâts élémentaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseGéant, new LearnableSkill(CustomInscription.RepousseGéant, "Repousse Géant", "Inscription de bouclier\n\n+2 d'armure contre les géants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseMagie, new LearnableSkill(CustomInscription.RepousseMagie, "Repousse Magie", "Inscription de bouclier\n\n+2 d'armure contre les créatures magiques\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseBon, new LearnableSkill(CustomInscription.RepousseBon, "Repousse Bon", "Inscription de bouclier\n\n+2 d'armure contre les créatures alignées au Bien\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseChaos, new LearnableSkill(CustomInscription.RepousseChaos, "Repousse Chaos", "Inscription de bouclier\n\n+2 d'armure contre les créatures alignées au Chaos\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseMal, new LearnableSkill(CustomInscription.RepousseMal, "Repousse Mal", "Inscription de bouclier\n\n+2 d'armure contre les créatures alignées au Mal\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseNeutre, new LearnableSkill(CustomInscription.RepousseNeutre, "Repousse Neutre", "Inscription de bouclier\n\n+2 d'armure contre les créatures alignées de façon Neutre\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseLoi, new LearnableSkill(CustomInscription.RepousseLoi, "Repousse Loi", "Inscription de bouclier\n\n+2 d'armure contre les créatures alignées à la Loi\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.ContreVentsEtMarées, new LearnableSkill(CustomInscription.ContreVentsEtMarées, "Contre vents et marées", "Inscription de bouclier\n\n+2 d'armure contre les dégâts Perçants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.lEnigmeDelAcier, new LearnableSkill(CustomInscription.lEnigmeDelAcier, "L'énigme de l'acier", "Inscription de bouclier\n\n+2 d'armure contre les dégâts Tranchants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.PasLeVisage, new LearnableSkill(CustomInscription.PasLeVisage, "Pas le visage !", "Inscription de bouclier\n\n+2 d'armure contre les dégâts Contondants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.PortéParLeVent, new LearnableSkill(CustomInscription.PortéParLeVent, "Porté par le vent", "Inscription de bouclier\n\n+2 d'armure contre les dégâts Polaires\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.CommeUnRoc, new LearnableSkill(CustomInscription.CommeUnRoc, "Comme un roc", "Inscription de bouclier\n\n+2 d'armure contre les dégâts Terrestres\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Illumination, new LearnableSkill(CustomInscription.Illumination, "Illumination", "Inscription de bouclier\n\n+2 d'armure contre les dégâts de Feu\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.ChevaucheLaTempête, new LearnableSkill(CustomInscription.ChevaucheLaTempête, "Chevauche la tempête", "Inscription de bouclier\n\n+2 d'armure contre les dégâts de Foudre\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseHalfelin, new LearnableSkill(CustomInscription.RepousseHalfelin, "Repousse Halfelin", "Inscription de bouclier\n\n+2 d'armure contre les Halfelins\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseHumain, new LearnableSkill(CustomInscription.RepousseHumain, "Repousse Humain", "Inscription de bouclier\n\n+2 d'armure contre les Humains\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseDemiElfe, new LearnableSkill(CustomInscription.RepousseDemiElfe, "Repousse Demi-Elfe", "Inscription de bouclier\n\n+2 d'armure contre les Demi-Elfes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseDemiOrc, new LearnableSkill(CustomInscription.RepousseDemiOrc, "Repousse Demi-Orc", "Inscription de bouclier\n\n+2 d'armure contre les Demi-Orcs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseElfe, new LearnableSkill(CustomInscription.RepousseElfe, "Repousse Elfe", "Inscription de bouclier\n\n+2 d'Armure contre les Elfes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseGnome, new LearnableSkill(CustomInscription.RepousseGnome, "Repousse Gnome", "Inscription de bouclier\n\n+2 d'Armure contre les Gnomes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseNain, new LearnableSkill(CustomInscription.RepousseNain, "Repousse Nain", "Inscription de bouclier\n\n+2 d'Armure contre les Nains\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.LaRaisonDuPlusFort, new LearnableSkill(CustomInscription.LaRaisonDuPlusFort, "La raison du plus fort", "Inscription de bouclier\n\n+1 d'armure tant que vous attaquez\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.SavoirNestQueLaMoitiéDuChemin, new LearnableSkill(CustomInscription.SavoirNestQueLaMoitiéDuChemin, "Savoir n'est que la moitié du chemin", "Inscription de bouclier\n\n+1 d'armure tant que vous lancez un sort\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.CeNestQuuneEgratignure, new LearnableSkill(CustomInscription.CeNestQuuneEgratignure, "Ce n'est qu'une égratignure", "Inscription de bouclier\n\n+2 d'armure tant que votre santé est en-dessous de 50 %\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.NeTremblezPas, new LearnableSkill(CustomInscription.NeTremblezPas, "Ne tremblez pas", "Inscription de bouclier\n\n+2 d'Armure tant que vous êtes sous l'effet d'un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Vigueur, new LearnableSkill(CustomInscription.Vigueur, "Vigueur", "Inscription de bouclier\n\n+4 Santé\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseNonVie, new LearnableSkill(CustomInscription.RepousseNonVie, "Repousse Non-Vie", "Inscription de bouclier\n\n+2 d'Armure contre les mort-vivants\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseOrc, new LearnableSkill(CustomInscription.RepousseOrc, "Repousse Orc", "Inscription de bouclier\n\n+2 d'Armure contre les créatures artificielles\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Piété, new LearnableSkill(CustomInscription.Piété, "Piété", "Inscription de bouclier\n\n+6 de Santé tant que vous êtes sous l'effet d'un sort positif\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Ténacité, new LearnableSkill(CustomInscription.Ténacité, "Ténacité", "Inscription de bouclier\n\n+6 de Santé tant que vous êtes sous l'effet d'une pose de combat\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.Détermination, new LearnableSkill(CustomInscription.Détermination, "Détermination", "Inscription de bouclier\n\n+8 de Santé tant que vous êtes sous l'effet d'un maléfice\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.HeureuxLesSimplesdEsprits, new LearnableSkill(CustomInscription.HeureuxLesSimplesdEsprits, "Heureux les simples d'esprits", "Inscription de bouclier\n\n+1 d'Armure\n-1 d'Energie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.LaVieNestQueDouleur, new LearnableSkill(CustomInscription.LaVieNestQueDouleur, "La vie n'est que douleur", "Inscription de bouclier\n\n+1 d'Armure\n-3 de Santé\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseMonstre, new LearnableSkill(CustomInscription.RepousseMonstre, "Repousse Monstre", "Inscription de bouclier\n\n+2 d'Armure contre les Monstres Primitifs\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseHumanoïde, new LearnableSkill(CustomInscription.RepousseHumanoïde, "Repousse Humanoïde", "Inscription de bouclier\n\n+2 d'Armure contre les créatures Humanoïdes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseGobelinoïde, new LearnableSkill(CustomInscription.RepousseGobelinoïde, "Repousse Gobelin", "Inscription de bouclier\n\n+2 d'Armure contre les créatures Gobelinoïdes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseAnimal, new LearnableSkill(CustomInscription.RepousseAnimal, "Repousse Animal", "Inscription de bouclier\n\n+2 d'Armure contre les Animaux\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseMétamorphe, new LearnableSkill(CustomInscription.RepousseMétamorphe, "Repousse Métamorphe", "Inscription de bouclier\n\n+2 d'Armure contre les Métamorphes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseReptilien, new LearnableSkill(CustomInscription.RepousseReptilien, "Repousse Reptilien", "Inscription de bouclier\n\n+2 d'Armure contre les créatures Reptiliennes\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.RepousseVermine, new LearnableSkill(CustomInscription.RepousseVermine, "Repousse Vermine", "Inscription de bouclier\n\n+2 d'Armure contre les Vermines\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Wisdom));
      learnableDictionary.Add(CustomInscription.OnApprendDeSesErreurs, new LearnableSkill(CustomInscription.OnApprendDeSesErreurs, "On apprend de ses erreurs", "Inscription de bijoux\n\n+1 d'Intelligence\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PatienceEtLongueurDeTemps, new LearnableSkill(CustomInscription.PatienceEtLongueurDeTemps, "Patience et longueur de temps", "Inscription de bijoux\n\n+1 de Sagesse\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Evocateur, new LearnableSkill(CustomInscription.Evocateur, "Evocateur", "Inscription de bijoux\n\n+1 emplacement de sort\nEmplacements de sorts maximum 12\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.AuxDependsDeCeluiQuilEcoute, new LearnableSkill(CustomInscription.AuxDependsDeCeluiQuilEcoute, "Aux dépends de celui qui l'écoute", "Inscription de bijoux\n\n+1 de Charisme\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.PoingDeFer, new LearnableSkill(CustomInscription.PoingDeFer, "Poing de fer", "Inscription de bijoux\n\n+1 secondes de durée des renversements que vous infligez\nDurée maximale : 3 secondes\nCette inscription n'a donc pas d'effet sur les capacités dont la durée de renversement est déjà de 3 secondes ou plus\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Ensanglanté, new LearnableSkill(CustomInscription.Ensanglanté, "Ensanglanté", "Inscription de bijoux\n\n-2% de temps d'incantation des sorts exploitant des cadavres\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Invocateur, new LearnableSkill(CustomInscription.Invocateur, "Invocateur", "Inscription de bijoux\n\n-2% de temps d'incantation des sorts d'invocation\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Belliciste, new LearnableSkill(CustomInscription.Belliciste, "Belliciste", "Inscription de bijoux\n\n+1 emplacement de capacité de combat\nMaximum : 12\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.LaMeilleureDesRaisons, new LearnableSkill(CustomInscription.LaMeilleureDesRaisons, "La meilleur des raisons", "Inscription de bijoux\n\n+1 Force\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Opportuniste, new LearnableSkill(CustomInscription.Opportuniste, "Opportuniste", "Inscription de bijoux\n\n+1 Dextérité\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Clarté, new LearnableSkill(CustomInscription.Clarté, "Clarté", "Inscription de bijoux\n\n-5% de durée des effets d'aveuglement vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Pureté, new LearnableSkill(CustomInscription.Pureté, "Pureté", "Inscription de bijoux\n\n-5% de durée des effets de maladie vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Récupération, new LearnableSkill(CustomInscription.Récupération, "Récupération", "Inscription de bijoux\n\n-5% de durée des effets d'étourdissement vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Cicatrisant, new LearnableSkill(CustomInscription.Cicatrisant, "Cicatrisant", "Inscription de bijoux\n\n-5% de durée des effets de blessures profondes vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Ardeur, new LearnableSkill(CustomInscription.Ardeur, "Ardeur", "Inscription de bijoux\n\n-5% de durée des effets de faiblesse vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Mithridate, new LearnableSkill(CustomInscription.Mithridate, "Mithridate", "Inscription de bijoux\n\n-5% de durée des effets d'empoisonnement vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Coagulant, new LearnableSkill(CustomInscription.Coagulant, "Coagulant", "Inscription de bijoux\n\n-5% de durée des effets de saignement vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Hardiesse, new LearnableSkill(CustomInscription.Hardiesse, "Hardiesse", "Inscription de bijoux\n\n-5% de durée des effets d'infirmité vous affectant\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Survivant, new LearnableSkill(CustomInscription.Survivant, "Survivant", "Inscription de bijoux\n\n+1 Santé\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Rayonnant, new LearnableSkill(CustomInscription.Rayonnant, "Rayonnant", "Inscription de bijoux\n\n+1 Energie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.Résilence, new LearnableSkill(CustomInscription.Résilence, "Résilence", "Inscription de bijoux\n\n+1 Constitution\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaDetectionDurabilityMinor, new LearnableSkill(CustomInscription.MateriaDetectionDurabilityMinor, "Matéria détection durable mineure", "Inscription d'objet\n\n+2% de durabilité des inscriptions de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionDurability, new LearnableSkill(CustomInscription.MateriaDetectionDurability, "Matéria détection durable", "Inscription d'objet\n\n+4% de durabilité des inscriptions de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionDurabilityMajor, new LearnableSkill(CustomInscription.MateriaDetectionDurabilityMajor, "Matéria détection durable majeure", "Inscription d'objet\n\n+6% de durabilité des inscriptions de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionDurabilitySupreme, new LearnableSkill(CustomInscription.MateriaDetectionDurabilitySupreme, "Matéria détection durable suprême", "Inscription d'objet\n\n+8% de durabilité des inscriptions de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaExtractionDurabilityMinor, new LearnableSkill(CustomInscription.MateriaExtractionDurabilityMinor, "Matéria extraction durable mineure", "Inscription d'objet\n\n+2% de durabilité des inscriptions d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionDurability, new LearnableSkill(CustomInscription.MateriaExtractionDurability, "Matéria extraction durable", "Inscription d'objet\n\n+4% de durabilité des inscriptions d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionDurabilityMajor, new LearnableSkill(CustomInscription.MateriaExtractionDurabilityMajor, "Matéria extraction durable majeure", "Inscription d'objet\n\n+6% de durabilité des inscriptions d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionDurabilitySupreme, new LearnableSkill(CustomInscription.MateriaExtractionDurabilitySupreme, "Matéria extraction durable suprême", "Inscription d'objet\n\n+8% de durabilité des inscriptions d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaProductionDurabilityMinor, new LearnableSkill(CustomInscription.MateriaProductionDurabilityMinor, "Matéria production durable mineure", "Inscription d'objet\n\n+2% de durabilité des inscriptions de production utilisant de la matéria\nLa présence d'au moins une inscription de production est nécessaire pour réaliser une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionDurability, new LearnableSkill(CustomInscription.MateriaProductionDurability, "Matéria production durable", "Inscription d'objet\n\n+4% de durabilité des inscriptions de production utilisant de la matéria\nLa présence d'au moins une inscription de production est nécessaire pour réaliser une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionDurabilityMajor, new LearnableSkill(CustomInscription.MateriaProductionDurabilityMajor, "Matéria production durable majeure", "Inscription d'objet\n\n+6% de durabilité des inscriptions de production utilisant de la matéria\nLa présence d'au moins une inscription de production est nécessaire pour réaliser une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionDurabilitySupreme, new LearnableSkill(CustomInscription.MateriaProductionDurabilitySupreme, "Matéria production durable suprême", "Inscription d'objet\n\n+8% de durabilité des inscriptions de production utilisant de la matéria\nLa présence d'au moins une inscription de production est nécessaire pour réaliser une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaInscriptionDurabilityMinor, new LearnableSkill(CustomInscription.MateriaInscriptionDurabilityMinor, "Matéria inscription durable mineure", "Inscription d'objet\n\n+2% de durabilité des inscriptions de calligraphies\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionDurability, new LearnableSkill(CustomInscription.MateriaInscriptionDurability, "Matéria inscription durable", "Inscription d'objet\n\n+4% de durabilité des inscriptions de calligraphies\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionDurabilityMajor, new LearnableSkill(CustomInscription.MateriaInscriptionDurabilityMajor, "Matéria inscription durable majeure", "Inscription d'objet\n\n+6% de durabilité des inscriptions de calligraphies\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionDurabilitySupreme, new LearnableSkill(CustomInscription.MateriaInscriptionDurabilitySupreme, "Matéria inscription durable suprême", "Inscription d'objet\n\n+8% de durabilité des inscriptions de calligraphies\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaDetectionAccuracyMinor, new LearnableSkill(CustomInscription.MateriaDetectionAccuracyMinor, "Matéria détection précise mineure", "Inscription d'objet\n\n+2% de précision des estimations de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 1, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionAccuracy, new LearnableSkill(CustomInscription.MateriaDetectionAccuracy, "Matéria détection précise", "Inscription d'objet\n\n+4% de précision des estimations de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionAccuracyMajor, new LearnableSkill(CustomInscription.MateriaDetectionAccuracyMajor, "Matéria détection précise majeure", "Inscription d'objet\n\n+6% de précision des estimations de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionAccuracySupreme, new LearnableSkill(CustomInscription.MateriaDetectionAccuracySupreme, "Matéria détection précise suprême", "Inscription d'objet\n\n+8% de précision des estimations de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaExtractionSpeedMinor, new LearnableSkill(CustomInscription.MateriaExtractionSpeedMinor, "Matéria extraction rapide mineure", "Inscription d'objet\n\n+2% de vitesse d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionSpeed, new LearnableSkill(CustomInscription.MateriaExtractionSpeed, "Matéria extraction rapide", "Inscription d'objet\n\n+4% de vitesse d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionSpeedMajor, new LearnableSkill(CustomInscription.MateriaExtractionSpeedMajor, "Matéria extraction rapide majeure", "Inscription d'objet\n\n+6% de vitesse d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionSpeedSupreme, new LearnableSkill(CustomInscription.MateriaExtractionSpeedSupreme, "Matéria extraction rapide suprême", "Inscription d'objet\n\n+8% de vitesse d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaExtractionYieldMinor, new LearnableSkill(CustomInscription.MateriaExtractionYieldMinor, "Matéria extraction efficace mineure", "Inscription d'objet\n\n+2% de rendement d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionYield, new LearnableSkill(CustomInscription.MateriaExtractionYield, "Matéria extraction efficace", "Inscription d'objet\n\n+4% de rendement d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionYieldMajor, new LearnableSkill(CustomInscription.MateriaExtractionYieldMajor, "Matéria extraction efficace majeure", "Inscription d'objet\n\n+6% de rendement d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionYieldSupreme, new LearnableSkill(CustomInscription.MateriaExtractionYieldSupreme, "Matéria extraction efficace suprême", "Inscription d'objet\n\n+8% de rendement d'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaExtractionQualityMinor, new LearnableSkill(CustomInscription.MateriaExtractionQualityMinor, "Matéria extraction concentrée mineure", "Inscription d'objet\n\n+2% de chance d'obtenir une concentration de matéria de qualité supérieure lors de l'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionQuality, new LearnableSkill(CustomInscription.MateriaExtractionQuality, "Matéria extraction concentrée", "Inscription d'objet\n\n+4% de chance d'obtenir une concentration de matéria de qualité supérieure lors de l'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionQualityMajor, new LearnableSkill(CustomInscription.MateriaExtractionQualityMajor, "Matéria extraction concentrée majeure", "Inscription d'objet\n\n+6% de chance d'obtenir une concentration de matéria de qualité supérieure lors de l'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaExtractionQualitySupreme, new LearnableSkill(CustomInscription.MateriaExtractionQualitySupreme, "Matéria extraction concentrée suprême", "Inscription d'objet\n\n+8% de chance d'obtenir une concentration de matéria de qualité supérieure lors de l'extraction de matéria\nLa présence d'au moins une inscription d'extraction est nécessaire pour effectuer une extraction de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaDetectionSpeedMinor, new LearnableSkill(CustomInscription.MateriaDetectionSpeedMinor, "Matéria détection rapide mineure", "Inscription d'objet\n\n+2% de vitesse de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionSpeed, new LearnableSkill(CustomInscription.MateriaDetectionSpeed, "Matéria détection rapide", "Inscription d'objet\n\n+4% de vitesse de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionSpeedMajor, new LearnableSkill(CustomInscription.MateriaDetectionSpeedMajor, "Matéria détection rapide majeure", "Inscription d'objet\n\n+6% de vitesse de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionSpeedSupreme, new LearnableSkill(CustomInscription.MateriaDetectionSpeedSupreme, "Matéria détection rapide suprême", "Inscription d'objet\n\n+8% de vitesse de détection de matéria\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaDetectionReliabilityMinor, new LearnableSkill(CustomInscription.MateriaDetectionReliabilityMinor, "Matéria détection fiable mineure", "Inscription d'objet\n\n+2% de chance de révéler un peu lors de la détection de matéria en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionReliability, new LearnableSkill(CustomInscription.MateriaDetectionReliability, "Matéria détection fiable", "Inscription d'objet\n\n+4% de chance de révéler un peu lors de la détection de matéria en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionReliabilityMajor, new LearnableSkill(CustomInscription.MateriaDetectionReliabilityMajor, "Matéria détection fiable majeure", "Inscription d'objet\n\n+6% de chance de révéler un peu lors de la détection de matéria en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionReliabilitySupreme, new LearnableSkill(CustomInscription.MateriaDetectionReliabilitySupreme, "Matéria détection fiable suprême", "Inscription d'objet\n\n+8% de chance de révéler un peu lors de la détection de matéria en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaDetectionQualityMinor, new LearnableSkill(CustomInscription.MateriaDetectionQualityMinor, "Matéria détection concentrée mineure", "Inscription d'objet\n\n+2% de chance de révéler un dépôt de concentration de matéria supérieure en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionQuality, new LearnableSkill(CustomInscription.MateriaDetectionQuality, "Matéria détection concentrée", "Inscription d'objet\n\n+4% de chance de révéler un dépôt de concentration de matéria supérieure en mode actifa\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionQualityMajor, new LearnableSkill(CustomInscription.MateriaDetectionQualityMajor, "Matéria détection concentrée majeure", "Inscription d'objet\n\n+6% de chance de révéler un dépôt de concentration de matéria supérieure en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaDetectionQualitySupreme, new LearnableSkill(CustomInscription.MateriaDetectionQualitySupreme, "Matéria détection concentrée suprême", "Inscription d'objet\n\n+8% de chance de révéler un dépôt de concentration de matéria supérieure en mode actif\nLa présence d'au moins une inscription de détection est nécessaire pour effectuer une détection de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaProductionSpeedMinor, new LearnableSkill(CustomInscription.MateriaProductionSpeedMinor, "Matéria production rapide mineure", "Inscription d'objet\n\n+2% de vitesse de production artisanale faisant usage de matéria\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionSpeed, new LearnableSkill(CustomInscription.MateriaProductionSpeed, "Matéria production rapide", "Inscription d'objet\n\n+4% de vitesse de production artisanale faisant usage de matéria\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionSpeedMajor, new LearnableSkill(CustomInscription.MateriaProductionSpeedMajor, "Matéria production rapide majeure", "Inscription d'objet\n\n+6% de vitesse de production artisanale faisant usage de matéria\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionSpeedSupreme, new LearnableSkill(CustomInscription.MateriaProductionSpeedSupreme, "Matéria production rapide suprême", "Inscription d'objet\n\n+8% de vitesse de production artisanale faisant usage de matéria\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaProductionYieldMinor, new LearnableSkill(CustomInscription.MateriaProductionYieldMinor, "Matéria production efficace mineure", "Inscription d'objet\n\n-2% de quantité de matéria nécessaire pour la production artisanale\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionYield, new LearnableSkill(CustomInscription.MateriaProductionYield, "Matéria production efficace", "Inscription d'objet\n\n-4% de quantité de matéria nécessaire pour la production artisanale\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionYieldMajor, new LearnableSkill(CustomInscription.MateriaProductionYieldMajor, "Matéria production efficace majeure", "Inscription d'objet\n\n-6% de quantité de matéria nécessaire pour la production artisanale\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionYieldSupreme, new LearnableSkill(CustomInscription.MateriaProductionYieldSupreme, "Matéria production efficace suprême", "Inscription d'objet\n\n-8% de quantité de matéria nécessaire pour la production artisanale\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaProductionQualityMinor, new LearnableSkill(CustomInscription.MateriaProductionQualityMinor, "Matéria production qualité mineure", "Inscription d'objet\n\n+1% de chance de réaliser un objet artisanal disposant d'un emplacement d'inscription supplémentaire\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionQuality, new LearnableSkill(CustomInscription.MateriaProductionQuality, "Matéria production qualité mineure", "Inscription d'objet\n\n+2% de chance de réaliser un objet artisanal disposant d'un emplacement d'inscription supplémentaire\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionQualityMajor, new LearnableSkill(CustomInscription.MateriaProductionQualityMajor, "Matéria production qualité mineure", "Inscription d'objet\n\n+3% de chance de réaliser un objet artisanal disposant d'un emplacement d'inscription supplémentaire\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaProductionQualitySupreme, new LearnableSkill(CustomInscription.MateriaProductionQualitySupreme, "Matéria production qualité mineure", "Inscription d'objet\n\n+4% de chance de réaliser un objet artisanal disposant d'un emplacement d'inscription supplémentaire\nLa présence d'au moins une inscription de production artisanale est nécessaire pour effectuer une production artisanale à partir de matéria\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaInscriptionSpeedMinor, new LearnableSkill(CustomInscription.MateriaInscriptionSpeedMinor, "Calligraphie rapide mineure", "Inscription d'objet\n\n+2% de vitesse d'inscription lors de la calligraphie\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionSpeed, new LearnableSkill(CustomInscription.MateriaInscriptionSpeed, "Calligraphie rapide", "Inscription d'objet\n\n+4% de vitesse d'inscription lors de la calligraphiea\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionSpeedMajor, new LearnableSkill(CustomInscription.MateriaInscriptionSpeedMajor, "Calligraphie rapide majeure", "Inscription d'objet\n\n+6% de vitesse d'inscription lors de la calligraphie\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionSpeedSupreme, new LearnableSkill(CustomInscription.MateriaInscriptionSpeedSupreme, "Calligraphie rapide suprême", "Inscription d'objet\n\n+8% de vitesse d'inscription lors de la calligraphie\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      learnableDictionary.Add(CustomInscription.MateriaInscriptionYieldMinor, new LearnableSkill(CustomInscription.MateriaInscriptionYieldMinor, "Calligraphie efficace mineure", "Inscription d'objet\n\n-2% de quantité d'influx nécessaire lors de la calligraphie\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 2, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionYield, new LearnableSkill(CustomInscription.MateriaInscriptionYield, "Calligraphie efficace", "Inscription d'objet\n\n-4% de quantité d'influx nécessaire lors de la calligraphiea\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 3, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionYieldMajor, new LearnableSkill(CustomInscription.MateriaInscriptionYieldMajor, "Calligraphie efficace majeure", "Inscription d'objet\n\n-6% de quantité d'influx nécessaire lors de la calligraphie\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 4, Ability.Charisma, Ability.Intelligence));
      learnableDictionary.Add(CustomInscription.MateriaInscriptionYieldSupreme, new LearnableSkill(CustomInscription.MateriaInscriptionYieldSupreme, "Calligraphie efficace suprême", "Inscription d'objet\n\n-8% de quantité d'influx nécessaire lors de la calligraphie\nLa présence d'au moins une inscription de calligraphie est nécessaire pour réaliser une calligraphie\nConsomme un emplacement libre d'inscription", Category.Inscription, "ife_X2EpicRepu", 5, 5, Ability.Charisma, Ability.Intelligence));

      RefreshLearnableDescriptions();
    }
    /*private static bool HandleImproveHealth(PlayerSystem.Player player, int customSkillId)
    {
      player.SetMaxHP();

      return true;
    }*/
    private static bool HandleLightArmorProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.ArmorProficiencyLight));
      return true;
    }
    private static bool HandleMediumArmorProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.ArmorProficiencyMedium));
      return true;
    }
    private static bool HandleHeavyArmorProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.ArmorProficiencyHeavy));
      return true;
    }
    private static bool HandleShieldProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.ShieldProficiency));
      return true;
    }
    private static bool HandleSimpleWeaponProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.WeaponProficiencySimple));
      return true;
    }
    private static bool HandleMartialProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.WeaponProficiencyMartial));
      return true;
    }
    private static bool HandleExoticProficiency(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.WeaponProficiencyExotic));
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
          //HandleImproveHealth(player, CustomSkill.ImprovedHealth);
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
    private static bool HandleAcolyteBackground(PlayerSystem.Player player, int customSkillId)
    {
      if(player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
        player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      if(player.learnableSkills.TryAdd(CustomSkill.ReligionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ReligionProficiency])))
      player.learnableSkills[CustomSkill.ReligionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.ReligionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleAnthropologistBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.HistoryProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HistoryProficiency])))
      player.learnableSkills[CustomSkill.HistoryProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.HistoryProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleArcheologistBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.IntimidationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IntimidationProficiency])))
      player.learnableSkills[CustomSkill.IntimidationProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.IntimidationProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleScholarBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.HistoryProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HistoryProficiency])))
      player.learnableSkills[CustomSkill.HistoryProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.HistoryProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.NatureProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NatureProficiency])))
      player.learnableSkills[CustomSkill.NatureProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.NatureProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleSageBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.HistoryProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HistoryProficiency])))
      player.learnableSkills[CustomSkill.HistoryProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.HistoryProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.ArcanaProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ArcanaProficiency])))
      player.learnableSkills[CustomSkill.ArcanaProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.ArcanaProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleHermitBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.MedicineProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MedicineProficiency])))
      player.learnableSkills[CustomSkill.MedicineProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.MedicineProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.ReligionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ReligionProficiency])))
      player.learnableSkills[CustomSkill.ReligionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.ReligionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleWandererBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency])))
      player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PersuasionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleAthleteBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AcrobaticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AcrobaticsProficiency])))
      player.learnableSkills[CustomSkill.AcrobaticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AcrobaticsProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleOutlanderBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleSoldierBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.IntimidationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IntimidationProficiency])))
      player.learnableSkills[CustomSkill.IntimidationProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.IntimidationProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleMercenaryBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency])))
      player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PersuasionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleFolkHeroBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AnimalHandlingProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnimalHandlingProficiency])))
      player.learnableSkills[CustomSkill.AnimalHandlingProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AnimalHandlingProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleSailorBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency])))
      player.learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleShipwrightBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.HistoryProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HistoryProficiency])))
      player.learnableSkills[CustomSkill.HistoryProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.HistoryProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency])))
      player.learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleFisherBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.MedicineProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MedicineProficiency])))
      player.learnableSkills[CustomSkill.MedicineProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.MedicineProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleMarineBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency])))
      player.learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleCriminalBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.StealthProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.StealthProficiency])))
      player.learnableSkills[CustomSkill.StealthProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.StealthProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleCharlatanBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SleightOfHandProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SleightOfHandProficiency])))
      player.learnableSkills[CustomSkill.SleightOfHandProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SleightOfHandProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleSmugglerBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleUrchinBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.SleightOfHandProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SleightOfHandProficiency])))
      player.learnableSkills[CustomSkill.SleightOfHandProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SleightOfHandProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.StealthProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.StealthProficiency])))
      player.learnableSkills[CustomSkill.StealthProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.StealthProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleGamblerBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleEntertainerBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.PerformanceProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerformanceProficiency])))
      player.learnableSkills[CustomSkill.PerformanceProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PerformanceProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.AcrobaticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AcrobaticsProficiency])))
      player.learnableSkills[CustomSkill.AcrobaticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AcrobaticsProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleCityWatchBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.AthleticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AthleticsProficiency])))
      player.learnableSkills[CustomSkill.AthleticsProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.AthleticsProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleInvestigatorBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.InvestigationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvestigationProficiency])))
      player.learnableSkills[CustomSkill.InvestigationProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InvestigationProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleKnightBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency])))
      player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PersuasionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.ReligionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ReligionProficiency])))
      player.learnableSkills[CustomSkill.ReligionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.ReligionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleNobleBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency])))
      player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PersuasionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.HistoryProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HistoryProficiency])))
      player.learnableSkills[CustomSkill.HistoryProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.HistoryProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleCourtierBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency])))
      player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PersuasionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.NatureProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NatureProficiency])))
      player.learnableSkills[CustomSkill.NatureProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.NatureProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleMerchantBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency])))
      player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PersuasionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.InvestigationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvestigationProficiency])))
      player.learnableSkills[CustomSkill.InvestigationProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InvestigationProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleTakenBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.NatureProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NatureProficiency])))
      player.learnableSkills[CustomSkill.NatureProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.NatureProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleScionBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.ArcanaProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ArcanaProficiency])))
      player.learnableSkills[CustomSkill.ArcanaProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.ArcanaProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleMagistrateBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.IntimidationExpertise, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IntimidationExpertise])))
      player.learnableSkills[CustomSkill.IntimidationExpertise].LevelUp(player);

      player.learnableSkills[CustomSkill.IntimidationProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleRefugeeBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.InsightProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InsightProficiency])))
      player.learnableSkills[CustomSkill.InsightProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InsightProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.SurvivalProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SurvivalProficiency])))
      player.learnableSkills[CustomSkill.SurvivalProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.SurvivalProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandlePrisonerBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.PerceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerceptionProficiency])))
      player.learnableSkills[CustomSkill.PerceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PerceptionProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleHauntedBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.ArcanaProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ArcanaProficiency])))
      player.learnableSkills[CustomSkill.ArcanaProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.ArcanaProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.InvestigationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvestigationProficiency])))
      player.learnableSkills[CustomSkill.InvestigationProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.InvestigationProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleFacelessBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.IntimidationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IntimidationProficiency])))
      player.learnableSkills[CustomSkill.IntimidationProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.IntimidationProficiency].source.Add(Category.StartingTraits);

      return true;
    }
    private static bool HandleSecretBackground(PlayerSystem.Player player, int customSkillId)
    {
      if (player.learnableSkills.TryAdd(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency])))
      player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.DeceptionProficiency].source.Add(Category.StartingTraits);

      if (player.learnableSkills.TryAdd(CustomSkill.PerformanceProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PerformanceProficiency])))
      player.learnableSkills[CustomSkill.PerformanceProficiency].LevelUp(player);

      player.learnableSkills[CustomSkill.PerformanceProficiency].source.Add(Category.StartingTraits);

      return true;
    }

    private static bool HandleWarriorCombatStyle(PlayerSystem.Player player, int customSkillId)
    {
      // TODO : ouvrir une fenêtre pour proposer un choix parmi les styles de combat en excluant les styles déjà connus par le personnage

      return true;
    }
    public static bool LearnActivableFeat(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(customSkillId)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(customSkillId));

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
          //pcSkin.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
          player.oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
        });
      }

      skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, spellLevel), EffectDuration.Permanent);

      return true;
    }

    public static readonly int[] forgeBasicSkillBooks = new int[] { CustomSkill.MateriaScanning, CustomSkill.OreDetection,  CustomSkill.OreExtraction,CustomSkill.MateriaExtraction, CustomSkill.Blacksmith, CustomSkill.Recycler, CustomSkill.OreExtractionYield, CustomSkill.ReprocessingOre, CustomSkill.ReprocessingOreEfficiency, CustomSkill.ReprocessingGrade1Expertise, CustomSkill.ReprocessingGrade2Expertise, CustomSkill.MateriaGradeConcentration, CustomSkill.Renforcement, CustomSkill.BlueprintCopy, CustomSkill.BlueprintEfficiency, CustomSkill.BlueprintEconomy, CustomSkill.BlueprintMetallurgy, CustomSkill.BlueprintResearch, CustomSkill.CraftDagger, CustomSkill.CraftLightMace, CustomSkill.CraftMorningStar, CustomSkill.CraftShortSpear, CustomSkill.CraftShortsword, CustomSkill.CraftSickle, CustomSkill.CraftScaleMail, CustomSkill.OreDetection };
    public static readonly int[] woodBasicSkillBooks = new int[] { CustomSkill.MateriaScanning, CustomSkill.WoodDetection, CustomSkill.WoodExtraction, CustomSkill.MateriaExtraction, CustomSkill.Renforcement, CustomSkill.Recycler, CustomSkill.BlueprintMetallurgy, CustomSkill.BlueprintResearch, CustomSkill.WoodExtraction, CustomSkill.WoodDetection, CustomSkill.ReprocessingWood, CustomSkill.Woodworker, CustomSkill.CraftSmallShield, CustomSkill.CraftClub, CustomSkill.CraftDarts, CustomSkill.CraftBullets, CustomSkill.CraftHeavyCrossbow, CustomSkill.CraftLightCrossbow, CustomSkill.CraftQuarterstaff, CustomSkill.CraftSling, CustomSkill.CraftArrow, CustomSkill.CraftBolt, CustomSkill.WoodDetection };
    public static readonly int[] leatherBasicSkillBooks = new int[] { CustomSkill.MateriaScanning, CustomSkill.PeltDetection, CustomSkill.PeltExtraction, CustomSkill.MateriaExtraction, CustomSkill.Renforcement, CustomSkill.Recycler, CustomSkill.PeltExtraction, CustomSkill.ReprocessingPelt, CustomSkill.Tanner, CustomSkill.CraftLeatherArmor, CustomSkill.CraftStuddedLeather, CustomSkill.CraftPaddedArmor, CustomSkill.CraftClothing, CustomSkill.CraftWhip, CustomSkill.CraftBelt, CustomSkill.CraftBoots, CustomSkill.CraftBracer, CustomSkill.CraftCloak, CustomSkill.CraftGloves, CustomSkill.PeltDetection };
    //public static Feat[] craftSkillBooks = new Feat[] { CustomFeats.Metallurgy, CustomFeats.AdvancedCraft, CustomFeats.Miner, CustomFeats.Geology, CustomFeats.Prospection, CustomFeats.VeldsparReprocessing, CustomFeats.ScorditeReprocessing, CustomFeats.PyroxeresReprocessing, CustomFeats.StripMiner, CustomFeats.Reprocessing, CustomFeats.ReprocessingEfficiency, CustomFeats.Connections, CustomFeats.Forge };
    /*public static readonly Feat[] alchemyBasicSkillBooks = new Feat[] { CustomFeats.Alchemist, CustomFeats.AlchemistCareful, CustomFeats.AlchemistEfficiency };
    public static readonly Feat[] languageSkillBooks = new Feat[] { CustomFeats.Abyssal, CustomFeats.Céleste, CustomFeats.Gnome, CustomFeats.Draconique, CustomFeats.Druidique, CustomFeats.Nain, CustomFeats.Elfique, CustomFeats.Géant, CustomFeats.Gobelin, CustomFeats.Halfelin, CustomFeats.Infernal, CustomFeats.Orc, CustomFeats.Primordiale, CustomFeats.Sylvain, CustomFeats.Voleur, CustomFeats.Gnome };

    public static readonly Feat[] lowSkillBooks = new Feat[] { CustomFeats.AlchemistExpert, CustomFeats.Renforcement, CustomFeats.ArtisanApplique, CustomFeats.Enchanteur, CustomFeats.Comptabilite, CustomFeats.BrokerRelations, CustomFeats.Negociateur, CustomFeats.Magnat, CustomFeats.Marchand, CustomFeats.Recycler, Feat.Ambidexterity, CustomFeats.Skinning, CustomFeats.Hunting, CustomFeats.ImprovedSpellSlot2, CustomFeats.WoodReprocessing, CustomFeats.Ebeniste, CustomFeats.WoodCutter, CustomFeats.WoodProspection, CustomFeats.CraftOreExtractor, CustomFeats.CraftForgeHammer, CustomFeats.Forge, CustomFeats.Reprocessing, CustomFeats.BlueprintCopy, CustomFeats.Research, CustomFeats.Miner, CustomFeats.Metallurgy, Feat.DeneirsEye, Feat.DirtyFighting, Feat.ResistDisease, Feat.Stealthy, Feat.SkillFocusAnimalEmpathy, Feat.SkillFocusBluff, Feat.SkillFocusConcentration, Feat.SkillFocusDisableTrap, Feat.SkillFocusDiscipline, Feat.SkillFocusHeal, Feat.SkillFocusHide, Feat.SkillFocusIntimidate, Feat.SkillFocusListen, Feat.SkillFocusLore, Feat.SkillFocusMoveSilently, Feat.SkillFocusOpenLock, Feat.SkillFocusParry, Feat.SkillFocusPerform, Feat.SkillFocusPickPocket, Feat.SkillFocusSearch, Feat.SkillFocusSetTrap, Feat.SkillFocusSpellcraft, Feat.SkillFocusSpot, Feat.SkillFocusTaunt, Feat.SkillFocusTumble, Feat.SkillFocusUseMagicDevice, Feat.Mobility, Feat.PointBlankShot, Feat.IronWill, Feat.Alertness, Feat.CombatCasting, Feat.Dodge, Feat.ExtraTurning, Feat.GreatFortitude };
    public static readonly Feat[] mediumSkillBooks = new Feat[] { CustomFeats.AlchemistAccurate, CustomFeats.AlchemistAware, CustomFeats.CombattantPrecautionneux, CustomFeats.EnchanteurExpert, CustomFeats.BrokerAffinity, CustomFeats.BadPeltReprocessing, CustomFeats.CommonPeltReprocessing, CustomFeats.NormalPeltReprocessing, CustomFeats.UncommunPeltReprocessing, CustomFeats.RarePeltReprocessing, CustomFeats.MagicPeltReprocessing, CustomFeats.EpicPeltReprocessing, CustomFeats.LegendaryPeltReprocessing, CustomFeats.ImprovedSpellSlot3, CustomFeats.ImprovedSpellSlot4, CustomFeats.LaurelinReprocessing, CustomFeats.MallornReprocessing, CustomFeats.TelperionReprocessing, CustomFeats.OiolaireReprocessing, CustomFeats.NimlothReprocessing, CustomFeats.QlipothReprocessing, CustomFeats.FerocheneReprocessing, CustomFeats.ValinorReprocessing, CustomFeats.WoodReprocessingEfficiency, CustomFeats.AnimalExpertise, CustomFeats.CraftTorch, CustomFeats.CraftStuddedLeather, CustomFeats.CraftSling, CustomFeats.CraftSmallShield, CustomFeats.CraftSickle, CustomFeats.CraftShortSpear, CustomFeats.CraftRing, CustomFeats.CraftPaddedArmor, CustomFeats.CraftPotion, CustomFeats.CraftQuarterstaff, CustomFeats.CraftMorningStar, CustomFeats.CraftMagicWand, CustomFeats.CraftLightMace, CustomFeats.CraftLightHammer, CustomFeats.CraftLightFlail, CustomFeats.CraftLightCrossbow, CustomFeats.CraftLeatherArmor, CustomFeats.CraftBullets, CustomFeats.CraftCloak, CustomFeats.CraftClothing, CustomFeats.CraftClub, CustomFeats.CraftDagger, CustomFeats.CraftDarts, CustomFeats.CraftGloves, CustomFeats.CraftHeavyCrossbow, CustomFeats.CraftHelmet, CustomFeats.CraftAmulet, CustomFeats.CraftArrow, CustomFeats.CraftBelt, CustomFeats.CraftBolt, CustomFeats.CraftBoots, CustomFeats.CraftBracer, CustomFeats.ReprocessingEfficiency, CustomFeats.StripMiner, CustomFeats.VeldsparReprocessing, CustomFeats.ScorditeReprocessing, CustomFeats.PyroxeresReprocessing, CustomFeats.PlagioclaseReprocessing, CustomFeats.Geology, CustomFeats.Prospection, Feat.TymorasSmile, Feat.LliirasHeart, Feat.RapidReload, Feat.Expertise, Feat.ImprovedInitiative, Feat.DefensiveRoll, Feat.SneakAttack, Feat.FlurryOfBlows, Feat.WeaponSpecializationHeavyCrossbow, Feat.WeaponSpecializationDagger, Feat.WeaponSpecializationDart, Feat.WeaponSpecializationClub, Feat.StillSpell, Feat.RapidShot, Feat.SilenceSpell, Feat.PowerAttack, Feat.Knockdown, Feat.LightningReflexes, Feat.ImprovedUnarmedStrike, Feat.Cleave, Feat.CalledShot, Feat.DeflectArrows, Feat.WeaponSpecializationLightCrossbow, Feat.WeaponSpecializationLightFlail, Feat.WeaponSpecializationLightMace, Feat.Disarm, Feat.EmpowerSpell, Feat.WeaponSpecializationMorningStar, Feat.ExtendSpell, Feat.SpellFocusAbjuration, Feat.SpellFocusConjuration, Feat.SpellFocusDivination, Feat.SpellFocusEnchantment, Feat.WeaponSpecializationSickle, Feat.WeaponSpecializationSling, Feat.WeaponSpecializationSpear, Feat.WeaponSpecializationStaff, Feat.WeaponSpecializationThrowingAxe, Feat.WeaponSpecializationTrident, Feat.WeaponSpecializationUnarmedStrike, Feat.SpellFocusEvocation, Feat.SpellFocusIllusion, Feat.SpellFocusNecromancy, Feat.SpellFocusTransmutation, Feat.SpellPenetration };
    public static readonly Feat[] highSkillBooks = new Feat[] { CustomFeats.ImprovedDodge, CustomFeats.EnchanteurChanceux, CustomFeats.SurchargeControlee, CustomFeats.SurchargeArcanique, CustomFeats.ArtisanExceptionnel, CustomFeats.AdvancedCraft, CustomFeats.CraftWarHammer, CustomFeats.CraftTrident, CustomFeats.CraftThrowingAxe, CustomFeats.CraftStaff, CustomFeats.CraftSplintMail, CustomFeats.CraftSpellScroll, CustomFeats.CraftShortsword, CustomFeats.CraftShortBow, CustomFeats.CraftScimitar, CustomFeats.CraftScaleMail, CustomFeats.CraftRapier, CustomFeats.CraftMagicRod, CustomFeats.CraftLongsword, CustomFeats.CraftLongBow, CustomFeats.CraftLargeShield, CustomFeats.CraftBattleAxe, CustomFeats.OmberReprocessing, CustomFeats.KerniteReprocessing, CustomFeats.GneissReprocessing, CustomFeats.CraftHalberd, CustomFeats.JaspetReprocessing, CustomFeats.CraftHeavyFlail, CustomFeats.CraftHandAxe, CustomFeats.HemorphiteReprocessing, CustomFeats.CraftGreatAxe, CustomFeats.CraftGreatSword, Feat.ArcaneDefenseAbjuration, Feat.ArcaneDefenseConjuration, Feat.ArcaneDefenseDivination, Feat.ArcaneDefenseEnchantment, Feat.ArcaneDefenseEvocation, Feat.ArcaneDefenseIllusion, Feat.ArcaneDefenseNecromancy, Feat.ArcaneDefenseTransmutation, Feat.BlindFight, Feat.SpringAttack, Feat.GreatCleave, Feat.ImprovedExpertise, Feat.SkillMastery, Feat.Opportunist, Feat.Evasion, Feat.WeaponSpecializationDireMace, Feat.WeaponSpecializationDoubleAxe, Feat.WeaponSpecializationDwaxe, Feat.WeaponSpecializationGreatAxe, Feat.WeaponSpecializationGreatSword, Feat.WeaponSpecializationHalberd, Feat.WeaponSpecializationHandAxe, Feat.WeaponSpecializationHeavyFlail, Feat.WeaponSpecializationKama, Feat.WeaponSpecializationKatana, Feat.WeaponSpecializationKukri, Feat.WeaponSpecializationBastardSword, Feat.WeaponSpecializationLightHammer, Feat.WeaponSpecializationLongbow, Feat.WeaponSpecializationLongSword, Feat.WeaponSpecializationRapier, Feat.WeaponSpecializationScimitar, Feat.WeaponSpecializationScythe, Feat.WeaponSpecializationShortbow, Feat.WeaponSpecializationShortSword, Feat.WeaponSpecializationShuriken, Feat.WeaponSpecializationBattleAxe, Feat.QuickenSpell, Feat.MaximizeSpell, Feat.ImprovedTwoWeaponFighting, Feat.ImprovedPowerAttack, Feat.WeaponSpecializationTwoBladedSword, Feat.WeaponSpecializationWarHammer, Feat.WeaponSpecializationWhip, Feat.ImprovedDisarm, Feat.ImprovedKnockdown, Feat.ImprovedParry, Feat.ImprovedCriticalBastardSword, Feat.ImprovedCriticalBattleAxe, Feat.ImprovedCriticalClub, Feat.ImprovedCriticalDagger, Feat.ImprovedCriticalDart, Feat.ImprovedCriticalDireMace, Feat.ImprovedCriticalDoubleAxe, Feat.ImprovedCriticalDwaxe, Feat.ImprovedCriticalGreatAxe, Feat.ImprovedCriticalGreatSword, Feat.ImprovedCriticalHalberd, Feat.ImprovedCriticalHandAxe, Feat.ImprovedCriticalHeavyCrossbow, Feat.ImprovedCriticalHeavyFlail, Feat.ImprovedCriticalKama, Feat.ImprovedCriticalKatana, Feat.ImprovedCriticalKukri, Feat.ImprovedCriticalLightCrossbow, Feat.ImprovedCriticalLightFlail, Feat.ImprovedCriticalLightHammer, Feat.ImprovedCriticalLightMace, Feat.ImprovedCriticalLongbow, Feat.ImprovedCriticalLongSword, Feat.ImprovedCriticalMorningStar, Feat.ImprovedCriticalRapier, Feat.ImprovedCriticalScimitar, Feat.ImprovedCriticalScythe, Feat.ImprovedCriticalShortbow, Feat.ImprovedCriticalShortSword, Feat.ImprovedCriticalShuriken, Feat.ImprovedCriticalSickle, Feat.ImprovedCriticalSling, Feat.ImprovedCriticalSpear, Feat.ImprovedCriticalStaff, Feat.ImprovedCriticalThrowingAxe, Feat.ImprovedCriticalTrident, Feat.ImprovedCriticalTwoBladedSword, Feat.ImprovedCriticalUnarmedStrike, Feat.ImprovedCriticalWarHammer, Feat.ImprovedCriticalWhip };
    public static readonly Feat[] epicSkillBooks = new Feat[] { CustomFeats.CraftWhip, CustomFeats.CraftTwoBladedSword, CustomFeats.CraftTowerShield, CustomFeats.CraftShuriken, CustomFeats.CraftScythe, CustomFeats.CraftKukri, CustomFeats.CraftKatana, CustomFeats.CraftBreastPlate, CustomFeats.CraftDireMace, CustomFeats.CraftDoubleAxe, CustomFeats.CraftDwarvenWarAxe, CustomFeats.CraftFullPlate, CustomFeats.CraftHalfPlate, CustomFeats.CraftBastardSword, CustomFeats.CraftKama, CustomFeats.DarkOchreReprocessing, CustomFeats.CrokiteReprocessing, CustomFeats.BistotReprocessing, Feat.ResistEnergyAcid, Feat.ResistEnergyCold, Feat.ResistEnergyElectrical, Feat.ResistEnergyFire, Feat.ResistEnergySonic, Feat.ZenArchery, Feat.CripplingStrike, Feat.SlipperyMind, Feat.GreaterSpellFocusAbjuration, Feat.GreaterSpellFocusConjuration, Feat.GreaterSpellFocusDivination, Feat.GreaterSpellFocusDiviniation, Feat.GreaterSpellFocusEnchantment, Feat.GreaterSpellFocusEvocation, Feat.GreaterSpellFocusIllusion, Feat.GreaterSpellFocusNecromancy, Feat.GreaterSpellFocusTransmutation, Feat.GreaterSpellPenetration };
    */
    public static readonly int[] shopBasicMagicSkillBooks = new int[] { CustomSkill.CalligrapheArmurier, CustomSkill.CalligrapheBlindeur, CustomSkill.CalligrapheCiseleur, CustomSkill.CalligrapheFourbisseur,/* CustomSkill.Comptabilite, CustomSkill.BrokerRelations, CustomSkill.Negociateur, CustomSkill.ContractScience, CustomSkill.Marchand, CustomSkill.Magnat*/ };
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
