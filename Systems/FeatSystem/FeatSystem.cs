﻿using Anvil.Services;
using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public partial class FeatSystem
  {
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.Creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      LogUtils.LogMessage($"{onUseFeat.Creature.Name} utilise le don {onUseFeat.Feat.Name} ({onUseFeat.Feat.Id})", LogUtils.LogType.Combat);

      switch (onUseFeat.Feat.Id)
      {
        //case CustomSkill.Determination: SecondWind(onUseFeat.Creature); return;

        case CustomSkill.MaitreBouclier:
          MaitreBouclier(onUseFeat.Creature, onUseFeat.TargetObject);
          onUseFeat.PreventFeatUse = true;
          return;

        case CustomSkill.Sprint: Sprint(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.Disengage: Disengage(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.Stealth: Stealth(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.Chargeur: Chargeur(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.FighterSurge: ActionSurge(onUseFeat.Creature); return;
        case CustomSkill.MageDeGuerre: MageDeGuerre(onUseFeat.Creature); return;
        case CustomSkill.FureurOrc: FureurOrc(onUseFeat.Creature); return;
        case CustomSkill.AgressionOrc: AgressionOrc(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.MeneurExaltant: MeneurExaltant(onUseFeat.Creature); return;
        case CustomSkill.Chanceux: Chanceux(onUseFeat.Creature); return;
        case CustomSkill.MainsGuerisseuses: MainsGuerisseuses(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.AilesAngeliques: AilesAngeliques(onUseFeat.Creature); return;

        case CustomSkill.ConspirateurMaitriseTactique: TacticalMastery(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.RoublardViseeStable: ViseeStable(onUseFeat.Creature); return;
        case CustomSkill.RoublardCoupDeChance: CoupDeChance(onUseFeat.Creature); return;

        case CustomSkill.ArcaneArcherTirAffaiblissant:
        case CustomSkill.ArcaneArcherTirAgrippant:
        case CustomSkill.ArcaneArcherTirBannissement:
        case CustomSkill.ArcaneArcherTirEnvoutant:
        case CustomSkill.ArcaneArcherTirExplosif:
        case CustomSkill.ArcaneArcherTirOmbres: TirArcanique(onUseFeat.Creature, onUseFeat.Feat.Id); return;
        case CustomSkill.ArcaneArcherTirChercheur: CreatureUtils.HandleTirChercheur(onUseFeat.Creature); return;

        case CustomSkill.WarMasterAttaqueMenacante: AttaqueMenacante(onUseFeat.Creature); return;
        case CustomSkill.WarMasterAttaquePrecise: AttaquePrecise(onUseFeat.Creature); return;
        case CustomSkill.WarMasterBalayage: Balayage(onUseFeat.Creature); return;
        case CustomSkill.WarMasterRenversement: Renversement(onUseFeat.Creature); return;
        case CustomSkill.WarMasterDesarmement: Desarmement(onUseFeat.Creature); return;
        case CustomSkill.WarMasterDiversion: Diversion(onUseFeat.Creature); return;
        case CustomSkill.WarMasterFeinte: Feinte(onUseFeat.Creature); return;
        case CustomSkill.WarMasterInstruction: Instruction(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.WarMasterJeuDeJambe: JeuDeJambe(onUseFeat.Creature); return;
        case CustomSkill.WarMasterManoeuvreTactique: ManoeuvreTactique(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.WarMasterParade: Parade(onUseFeat.Creature); return;
        case CustomSkill.WarMasterProvocation: Provocation(onUseFeat.Creature); return;
        case CustomSkill.WarMasterRalliement: Ralliement(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.WarMasterRiposte: Riposte(onUseFeat.Creature); return;
        case CustomSkill.WarMasterConnaisTonEnnemi: ConnaisTonEnnemi(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.EldritchKnightArmeLiee:
        case CustomSkill.EldritchKnightArmeLiee2: ArmeLiee(onUseFeat.Creature, onUseFeat.Feat); return;
        case CustomSkill.EldritchKnightArmeLieeInvocation:
        case CustomSkill.EldritchKnightArmeLieeInvocation2: ArmeLieeInovcation(onUseFeat.Creature, onUseFeat.Feat); return;

        case CustomSkill.BarbarianRecklessAttack: RecklessAttack(onUseFeat.Creature); return;
        case CustomSkill.BarbarianRagePersistante: RagePersistante(onUseFeat.Creature); return;
        case CustomSkill.FrappeBrutale: 
        case CustomSkill.FrappeSiderante: 
        case CustomSkill.FrappeDechirante: FrappeBrutale(onUseFeat.Creature, onUseFeat.Feat.Id); return;
        case CustomSkill.BersekerPresenceIntimidante: BersekerPresenceIntimidante(onUseFeat.Creature); return;

        case CustomSkill.WildMagicSense: SensDeLaMagie(onUseFeat.Creature); return;
        case CustomSkill.WildMagicMagieGalvanisanteBienfait: Bienfait(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.WildMagicMagieGalvanisanteRecuperation: Recuperation(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.MonkUnarmoredSpeed: MonkUnarmoredSpeed(onUseFeat.Creature); return;
        case CustomSkill.MonkBonusAttack: MonkBonusAttack(onUseFeat.Creature); return;
        case CustomSkill.MonkPatience: MonkPatience(onUseFeat.Creature); return;
        case CustomSkill.MonkDelugeDeCoups: MonkDelugeDeCoups(onUseFeat.Creature); return;
        case CustomSkill.MonkSlowFall: MonkSlowFall(onUseFeat.Creature); return;
        case CustomSkill.MonkStunStrike: MonkStunStrike(onUseFeat.Creature); return;
        case CustomSkill.MonkFrappesRenforcees: FrappesRenforcees(onUseFeat.Creature); return;
        case CustomSkill.MonkDiamondSoul: DiamondSoul(onUseFeat.Creature); return;
        case CustomSkill.MonkDesertion: MonkDesertion(onUseFeat.Creature); return;
        case CustomSkill.MonkPlenitude: MonkPlenitude(onUseFeat.Creature); return;

        case CustomSkill.MonkManifestationAme: MonkManifestationAme(onUseFeat.Creature); return;
        case CustomSkill.MonkManifestationCorps: MonkManifestationCorps(onUseFeat.Creature); return;
        case CustomSkill.MonkManifestationEsprit: MonkManifestationEsprit(onUseFeat.Creature); return;
        case CustomSkill.MonkResonanceKi: MonkResonanceKi(onUseFeat.Creature); return;
        case CustomSkill.MonkExplosionKi: MonkExplosionKi(onUseFeat.Creature); return;
        case CustomSkill.MonkPaumeVibratoire: MonkPaumeVibratoire(onUseFeat.Creature); return;

        case CustomSkill.TraqueurLinceulDombre:
        case CustomSkill.MonkLinceulDombre: MonkLinceulDombre(onUseFeat.Creature, onUseFeat); return;
        //case CustomSkill.ClercLinceulDombre:
        case CustomSkill.MonkFouleeDombre: MonkFouleeDombre(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkFrappeDombre: MonkFrappeDombre(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkMetabolismeSurnaturel: MetabolismeSurnaturel(onUseFeat.Creature); return;

        case CustomSkill.MonkLienElementaire: LienElementaire(onUseFeat.Creature); return;
        case CustomSkill.MonkCrochetsDuSerpentDeFeu: CrochetsDuSerpentDeFeu(onUseFeat.Creature); return;
        case CustomSkill.MonkFaconnageDeLaRiviere: FaconnageDeLaRiviere(onUseFeat.Creature); return;
        case CustomSkill.MonkPorteParLeVent: PorteParLeVent(onUseFeat.Creature); return;

        case CustomSkill.WizardRestaurationArcanique: RestaurationArcanique(onUseFeat.Creature); return;
        case CustomSkill.AbjurationWardProjetee: ProtectionProjetee(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.DivinationPresage: 
        case CustomSkill.DivinationPresage2: 
        case CustomSkill.DivinationPresageSuperieur: 
          Presage(onUseFeat.Creature, onUseFeat.TargetObject, onUseFeat.Feat); return;
        case CustomSkill.DivinationSeeInvisibility: DivinationSeeInvisible(onUseFeat.Creature); return;
        case CustomSkill.DivinationDarkVision: DivinationDarkVision(onUseFeat.Creature); return;
        case CustomSkill.DivinationSeeEthereal: DivinationSeeEthereal(onUseFeat.Creature); return;

        case CustomSkill.EvocateurSurcharge: SurchargeArcanique(onUseFeat.Creature); return;

        case CustomSkill.IllusionDouble: IllusionDouble(onUseFeat.Creature); return;

        case CustomSkill.InvocationMineure: InvocationMineure(onUseFeat.Creature); return;

        case CustomSkill.NecromancieUndeadControl: NecromancieUndeadControl(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.TransmutationAlchimieMineure: TransmutationAlchimieMineure(onUseFeat.Creature); return;
        case CustomSkill.TransmutationStone: TransmutationStone(player); return;
        case CustomSkill.TransmutationMaitre: TransmutationMaster(player); return;

        case CustomSkill.BardInspiration: InspirationBardique(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.DefenseVaillante: DefenseVaillante(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.DegatsVaillants: DegatsVaillants(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.SourceDinspiration: SourceDinspiration(onUseFeat.Creature); return;
        case CustomSkill.BotteDefensive:
        case CustomSkill.BotteDefensiveDeMaitre: BotteDefensive(onUseFeat.Creature, onUseFeat.Feat.Id); return;
        case CustomSkill.BotteTranchante:
        case CustomSkill.BotteTranchanteDeMaitre: BotteTranchante(onUseFeat.Creature, onUseFeat.Feat.Id); return;

        case CustomSkill.RangerInfatiguable: Infatiguable(onUseFeat.Creature); return;
        case CustomSkill.ProfondeursFrappeRedoutable: FrappeRedoutable(onUseFeat.Creature); return;

        case CustomSkill.BelluaireFurieBestiale: FurieBestiale(onUseFeat.Creature); return;
        case CustomSkill.BelluaireRugissementProvoquant: BelluaireRugissementProvoquant(onUseFeat.Creature); return;
        case CustomSkill.BelluairePatteMielleuse: PatteMielleuse(onUseFeat.Creature); return;
        case CustomSkill.BelluaireChargeSanglier: ChargeDuSanglier(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.BelluaireRageSanglier: RageDuSanglier(onUseFeat.Creature); return;
        case CustomSkill.BelluaireCorbeauAveuglement: CorbeauAveuglement(onUseFeat.Creature); return;
        case CustomSkill.BelluaireCorbeauMauvaisAugure: CorbeauMauvaisAugure(onUseFeat.Creature, onUseFeat.TargetObject); return;
        //case CustomSkill.BelluaireLoupMorsurePlongeante: LoupMorsurePlongeante(onUseFeat.Creature); return;
        case CustomSkill.BelluaireSpiderWeb: SpiderWeb(onUseFeat.Creature, onUseFeat.TargetObject is null ? onUseFeat.TargetPosition : onUseFeat.TargetObject.Position); return;
        case CustomSkill.BelluaireSpiderCocoon: SpiderCocoon(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.SensDivin: SensDivin(onUseFeat.Creature); return;
        case CustomSkill.ChatimentDivin: ChatimentDivin(onUseFeat.Creature); return;
        case CustomSkill.AuraDeProtection: AuraDeProtection(onUseFeat.Creature); return;
        case CustomSkill.DevotionSaintesRepresailles: SaintesRepresailles(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.DevotionNimbeSacree: NimbeSacree(onUseFeat.Creature); return;
        case CustomSkill.AnciensGuerisonRayonnante: GuerisonRayonnante(onUseFeat.Creature); return;
        case CustomSkill.AnciensChampionAntique: ChampionAntique(onUseFeat.Creature); return;
        case CustomSkill.PaladinVoeuHostile: VoeudHostilite(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.PaladinPuissanceInquisitrice: PuissanceInquisitrice(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.AngeDeLaVengeance: AngeDeLaVengeance(onUseFeat.Creature); return;

        case CustomSkill.ClercInterventionDivine: InterventionDivine(onUseFeat.Creature); return;
        case CustomSkill.ClercIncantationPuissante: IncantationPuissante(onUseFeat.Creature, onUseFeat.TargetObject); return;

        //case CustomSkill.ClercMartial: ClercMartial(onUseFeat.Creature); return;
        case CustomSkill.ClercFrappeGuidee: FrappeGuidee(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.ClercIllumination: ClercIllumination(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.ClercHaloDeLumiere: HaloDeLumiere(onUseFeat.Creature); return;

        case CustomSkill.ClercVisionDuPasse: VisionDuPasse(onUseFeat.Creature); return;

        case CustomSkill.ClercFureurDestructrice: FureurDestructrice(onUseFeat.Creature); return;
        case CustomSkill.ClercEnfantDeLaTempete: EnfantDeLaTempete(onUseFeat.Creature); return;

        case CustomSkill.ClercPreservationDeLaVie: PreservationDeLaVie(onUseFeat.Creature); return;

        case CustomSkill.ClercBenedictionDuFilou: BenedictionDuFilou(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.TeleportRepliqueDuplicite: TranspositionDuFilou(onUseFeat.Creature); return;

        case CustomSkill.SorcellerieInnee: SorcellerieInnee(onUseFeat.Creature); return;
        case CustomSkill.SorcellerieIncarnee: SorcellerieIncarnee(onUseFeat.Creature); return;
        case CustomSkill.EnsoSourceToSlot: SourceToSlot(onUseFeat.Creature); return;
        case CustomSkill.EnsoSlotToSource: SlotToSource(onUseFeat.Creature); return;
        case CustomSkill.EnsoPrudence: 
        case CustomSkill.EnsoAllonge: 
        case CustomSkill.EnsoExtension: 
        case CustomSkill.EnsoAmplification: 
        case CustomSkill.EnsoGemellite: 
        case CustomSkill.EnsoIntensification: 
        case CustomSkill.EnsoAcceleration: 
        case CustomSkill.EnsoGuidage: 
        case CustomSkill.EnsoSubtilite: 
        case CustomSkill.EnsoTransmutation: Metamagie(onUseFeat.Creature, onUseFeat.Feat.Id); return;
        case CustomSkill.EnsoDracoWings: AilesDraconiques(onUseFeat.Creature); return;
        case CustomSkill.EnsoGuideTempete: GuideTempete(onUseFeat.Creature); return;
        case CustomSkill.EnsoAmeDesVents: AmeDesVents(onUseFeat.Creature); return;

        case CustomSkill.DruideReveilSauvage: ReveilSauvage(onUseFeat.Creature); return;
        case CustomSkill.DruideFrappePrimordialeFroid:
        case CustomSkill.DruideFrappePrimordialeFeu: 
        case CustomSkill.DruideFrappePrimordialeElec: 
        case CustomSkill.DruideFrappePrimordialeTonnerre: FrappePrimordiale(onUseFeat.Creature, onUseFeat.Feat.Id); return;
        case CustomSkill.MageNature: SlotToFormeSauvage(onUseFeat.Creature); return;

        case CustomSkill.FormeSauvageOurs: 
        case CustomSkill.FormeSauvageCorbeau: 
        case CustomSkill.FormeSauvageTigre: FormeSauvage(onUseFeat.Creature, onUseFeat.Feat.Id); return;
        case CustomSkill.FormeSauvageAir:
        case CustomSkill.FormeSauvageTerre:
        case CustomSkill.FormeSauvageFeu:
        case CustomSkill.FormeSauvageEau: FormeSauvage(onUseFeat.Creature, onUseFeat.Feat.Id, 2); return;

        case CustomSkill.DruideEconomieNaturelle: EconomieNaturelle(onUseFeat.Creature); return;
        case CustomSkill.DruideRecuperationNaturelle: RecuperationNaturelle(onUseFeat.Creature); return;
        case CustomSkill.DruideSanctuaireNaturel: SanctuaireNaturel(onUseFeat.Creature); return;
        case CustomSkill.DruideLuneRadieuse: LuneRadieuse(onUseFeat.Creature); return;
        case CustomSkill.DruideFureurDesFlots: FureurDesFlots(onUseFeat.Creature); return;

        case CustomSkill.OccultisteFourberieMagique: FourberieMagique(onUseFeat.Creature); return;
        case CustomSkill.OccultisteContactDoutremonde: ContactDoutremonde(onUseFeat.Creature); return;
        case CustomSkill.PacteDeLaLame: PacteDeLaLame(onUseFeat.Creature); return;
        case CustomSkill.PacteDeLaLameInvoquer: PacteDeLaLameInvoquer(onUseFeat.Creature); return;
        case CustomSkill.ChatimentOcculte: ChatimentOcculte(onUseFeat.Creature); return;
        case CustomSkill.DonDuProtecteur: DonDuProtecteur(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.DoubleVue: DoubleVue(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.PacteDuTome: PacteDuTome(onUseFeat.Creature); return;

        case CustomSkill.LueurDeGuérison: LueurDeGuérison(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.VengeanceCalcinante: VengeanceCalcinante(onUseFeat.Creature); return;

        case CustomSkill.ResilienceFielleuse: ResilienceFielleuse(onUseFeat.Creature); return;
        case CustomSkill.TraverseeInfernale: TraverseeInfernale(onUseFeat.Creature); return;

        case CustomSkill.EspritEveille: EspritEveille(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.SortsPsychiques: SortsPsychiques(onUseFeat.Creature); return;

        case CustomSkill.ExpertiseCommotion: Commotion(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseAffaiblissement: Affaiblissement(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseArretCardiaque: ArretCardiaque(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseTranspercer: Transpercer(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseTirPercant: TirPercant(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseMoulinet: Moulinet(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseLaceration: Laceration(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseMutilation: Mutilation(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseFendre: Fendre(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseCharge: Charge(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseFrappeDuPommeau: FrappeDuPommeau(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseDesarmement: ExpertiseDesarmement(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseBriseEchine: BriseEchine(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseDestabiliser: Destabiliser(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseCoupeJarret: CoupeJarret(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseEntaille: Entaille(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseRenforcement: Renforcement(onUseFeat.Creature); return;
        case CustomSkill.ExpertisePreparation: Preparation(onUseFeat.Creature); return;
        case CustomSkill.ExpertiseStabilisation: Stabilisation(onUseFeat.Creature); return;

        case CustomSkill.FighterSecondWind:
          if (CreatureUtils.HandleBonusActionUse(onUseFeat.Creature)) return;
          else onUseFeat.PreventFeatUse = true; return;
      }

      int featId = onUseFeat.Feat.Id + 10000;

      /*SkillSystem.Attribut featAttribute = SkillSystem.learnableDictionary[featId].attribut;
      int attributeLevel = player.GetAttributeLevel(featAttribute);
      int bonusAttributeChance = 0;

      NwItem castItem = onUseFeat.Creature.GetItemInSlot(InventorySlot.RightHand);

      if (castItem is not null)
      {
        if (castItem is not null && ItemUtils.GetItemAttribute(castItem) == featAttribute)
          for (int i = 0; i < castItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            if (castItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Maîtrise)
              bonusAttributeChance += 3;
      }

      castItem = onUseFeat.Creature.GetItemInSlot(InventorySlot.LeftHand);

      if (castItem is not null)
      {
        if (castItem is not null && ItemUtils.GetItemAttribute(castItem) == featAttribute)
          for (int i = 0; i < castItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            if (castItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Maîtrise)
              bonusAttributeChance += 3;
      }

      if (NwRandom.Roll(Utils.random, 100) < bonusAttributeChance)
        attributeLevel += 1;*/

      switch (featId)
      {
        case CustomSkill.SeverArtery:
          onUseFeat.Creature.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").Value = featId;
          return;
      }

      switch (onUseFeat.Feat.FeatType)
      {
        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:

          onUseFeat.PreventFeatUse = true;
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat.FeatType));
          break;
      }
    }
  }
}
