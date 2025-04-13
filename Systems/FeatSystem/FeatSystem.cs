using Anvil.Services;
using Anvil.API.Events;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public partial class FeatSystem
  {
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.Creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      NwCreature caster = onUseFeat.Creature;
      NwFeat feat = onUseFeat.Feat;
      NwGameObject target = onUseFeat.TargetObject;

      LogUtils.LogMessage($"{caster.Name} utilise le don {feat.Name} ({feat.Id})", LogUtils.LogType.Combat);

      switch (feat.Id)
      {
        //case CustomSkill.Determination: SecondWind(caster); return;

        case CustomSkill.MaitreBouclier:
          MaitreBouclier(caster, target);
          onUseFeat.PreventFeatUse = true;
          return;

        case CustomSkill.Sprint: Sprint(caster, onUseFeat); return;
        case CustomSkill.Disengage: Disengage(caster, onUseFeat); return;
        case CustomSkill.Stealth: Stealth(caster, onUseFeat); return;
        case CustomSkill.ShortRest: ShortRest(caster); return;
        case CustomSkill.LongRest: LongRest(caster); return;
        case CustomSkill.Chargeur: Chargeur(caster, target); return;
        case CustomSkill.FighterSurge: ActionSurge(caster); return;
        case CustomSkill.FighterInflexible: Inflexible(caster); return;
        case CustomSkill.MageDeGuerre: MageDeGuerre(caster); return;
        case CustomSkill.FureurOrc: FureurOrc(caster); return;
        case CustomSkill.AgressionOrc: AgressionOrc(caster, target); return;
        case CustomSkill.MeneurExaltant: MeneurExaltant(caster); return;
        case CustomSkill.Musicien: Musicien(caster); return;
        case CustomSkill.Chanceux: Chanceux(caster, target); return;
        case CustomSkill.MainsGuerisseuses: MainsGuerisseuses(caster, target); return;
        case CustomSkill.AilesAngeliques: AilesAngeliques(caster); return;
        case CustomSkill.InspirationHeroique: InspirationHeroique(caster); return;
        case CustomSkill.HighElfCantrip: HighElfCantrip(caster); return;

        case CustomSkill.ConspirateurMaitriseTactique: TacticalMastery(caster, onUseFeat); return;
        case CustomSkill.RoublardViseeStable: ViseeStable(caster); return;
        case CustomSkill.RoublardCoupDeChance: CoupDeChance(caster); return;

        case CustomSkill.ArcaneArcherTirAffaiblissant:
        case CustomSkill.ArcaneArcherTirAgrippant:
        case CustomSkill.ArcaneArcherTirBannissement:
        case CustomSkill.ArcaneArcherTirEnvoutant:
        case CustomSkill.ArcaneArcherTirExplosif:
        case CustomSkill.ArcaneArcherTirOmbres: TirArcanique(caster, feat.Id); return;
        case CustomSkill.ArcaneArcherTirChercheur: CreatureUtils.HandleTirChercheur(caster); return;

        case CustomSkill.WarMasterAttaqueMenacante: AttaqueMenacante(caster); return;
        case CustomSkill.WarMasterAttaquePrecise: AttaquePrecise(caster); return;
        case CustomSkill.WarMasterBalayage: Balayage(caster); return;
        case CustomSkill.WarMasterRenversement: Renversement(caster); return;
        case CustomSkill.WarMasterDesarmement: Desarmement(caster); return;
        case CustomSkill.WarMasterDiversion: Diversion(caster); return;
        case CustomSkill.WarMasterFeinte: Feinte(caster); return;
        case CustomSkill.WarMasterInstruction: Instruction(caster, target); return;
        case CustomSkill.WarMasterJeuDeJambe: JeuDeJambe(caster); return;
        case CustomSkill.WarMasterManoeuvreTactique: ManoeuvreTactique(caster, target); return;
        case CustomSkill.WarMasterParade: Parade(caster); return;
        case CustomSkill.WarMasterProvocation: Provocation(caster); return;
        case CustomSkill.WarMasterRalliement: Ralliement(caster, target); return;
        case CustomSkill.WarMasterRiposte: Riposte(caster); return;
        case CustomSkill.WarMasterConnaisTonEnnemi: ConnaisTonEnnemi(caster, target); return;

        case CustomSkill.EldritchKnightArmeLiee:
        case CustomSkill.EldritchKnightArmeLiee2: ArmeLiee(caster, feat); return;
        case CustomSkill.EldritchKnightArmeLieeInvocation:
        case CustomSkill.EldritchKnightArmeLieeInvocation2: ArmeLieeInovcation(caster, feat); return;

        case CustomSkill.BarbarianRecklessAttack: RecklessAttack(caster); return;
        case CustomSkill.BarbarianRagePersistante: RagePersistante(caster); return;
        case CustomSkill.FrappeBrutale: 
        case CustomSkill.FrappeSiderante: 
        case CustomSkill.FrappeDechirante: FrappeBrutale(caster, feat.Id); return;
        case CustomSkill.BersekerPresenceIntimidante: BersekerPresenceIntimidante(caster); return;
        case CustomSkill.BersekerRestorePresenceIntimidante: BersekerRestorePresenceIntimidante(caster); return;

        case CustomSkill.WildMagicSense: SensDeLaMagie(caster); return;
        case CustomSkill.WildMagicMagieGalvanisanteBienfait: Bienfait(caster, target); return;
        case CustomSkill.WildMagicMagieGalvanisanteRecuperation: Recuperation(caster, target); return;
        case CustomSkill.RepercussionInstable: RepercussionInstable(caster); return;

        case CustomSkill.MonkUnarmoredSpeed: MonkUnarmoredSpeed(caster); return;
        case CustomSkill.MonkBonusAttack: MonkBonusAttack(caster); return;
        case CustomSkill.MonkPatience: MonkPatience(caster); return;
        case CustomSkill.MonkDelugeDeCoups: MonkDelugeDeCoups(caster); return;
        case CustomSkill.MonkSlowFall: MonkSlowFall(caster); return;
        case CustomSkill.MonkStunStrike: MonkStunStrike(caster); return;
        case CustomSkill.MonkFrappesRenforcees: FrappesRenforcees(caster); return;
        case CustomSkill.MonkDiamondSoul: DiamondSoul(caster); return;
        case CustomSkill.MonkDesertion: MonkDesertion(caster); return;
        case CustomSkill.MonkPlenitude: MonkPlenitude(caster); return;

        case CustomSkill.MonkManifestationAme: MonkManifestationAme(caster); return;
        case CustomSkill.MonkManifestationCorps: MonkManifestationCorps(caster); return;
        case CustomSkill.MonkManifestationEsprit: MonkManifestationEsprit(caster); return;
        case CustomSkill.MonkResonanceKi: MonkResonanceKi(caster); return;
        case CustomSkill.MonkExplosionKi: MonkExplosionKi(caster); return;
        case CustomSkill.MonkPaumeVibratoire: MonkPaumeVibratoire(caster); return;

        case CustomSkill.TraqueurLinceulDombre:
        case CustomSkill.MonkLinceulDombre: MonkLinceulDombre(caster, onUseFeat); return;
        //case CustomSkill.ClercLinceulDombre:
        case CustomSkill.MonkFouleeDombre: MonkFouleeDombre(caster, onUseFeat); return;
        case CustomSkill.MonkFrappeDombre: MonkFrappeDombre(caster, onUseFeat); return;
        case CustomSkill.MonkMetabolismeSurnaturel: MetabolismeSurnaturel(caster); return;

        case CustomSkill.MonkLienElementaire: LienElementaire(caster); return;
        case CustomSkill.MonkCrochetsDuSerpentDeFeu: CrochetsDuSerpentDeFeu(caster); return;
        case CustomSkill.MonkFaconnageDeLaRiviere: FaconnageDeLaRiviere(caster); return;
        case CustomSkill.MonkPorteParLeVent: PorteParLeVent(caster); return;

        case CustomSkill.WizardRestaurationArcanique: RestaurationArcanique(caster); return;
        case CustomSkill.AbjurationWardProjetee: ProtectionProjetee(caster, target); return;
        case CustomSkill.DivinationPresage: 
        case CustomSkill.DivinationPresage2: 
        case CustomSkill.DivinationPresageSuperieur: 
          Presage(caster, target, feat); return;
        case CustomSkill.DivinationSeeInvisibility: DivinationSeeInvisible(caster); return;
        case CustomSkill.DivinationDarkVision: DivinationDarkVision(caster); return;
        case CustomSkill.DivinationSeeEthereal: DivinationSeeEthereal(caster); return;

        case CustomSkill.EvocateurSurcharge: SurchargeArcanique(caster); return;

        case CustomSkill.IllusionDouble: IllusionDouble(caster); return;

        case CustomSkill.InvocationMineure: InvocationMineure(caster); return;

        case CustomSkill.NecromancieUndeadControl: NecromancieUndeadControl(caster, target); return;

        case CustomSkill.TransmutationAlchimieMineure: TransmutationAlchimieMineure(caster); return;
        case CustomSkill.TransmutationStone: TransmutationStone(player); return;
        case CustomSkill.TransmutationMaitre: TransmutationMaster(player); return;

        case CustomSkill.BardInspiration: InspirationBardique(caster, target); return;
        case CustomSkill.DefenseVaillante: DefenseVaillante(caster, target); return;
        case CustomSkill.DegatsVaillants: DegatsVaillants(caster, target); return;
        case CustomSkill.SourceDinspiration: SourceDinspiration(caster); return;
        case CustomSkill.BotteDefensive:
        case CustomSkill.BotteDefensiveDeMaitre: BotteDefensive(caster, feat.Id); return;
        case CustomSkill.BotteTranchante:
        case CustomSkill.BotteTranchanteDeMaitre: BotteTranchante(caster, feat.Id); return;

        case CustomSkill.RangerInfatiguable: Infatiguable(caster); return;
        case CustomSkill.Survivant1: Survivant(caster); return;
        case CustomSkill.RegenerationNaturelle: RegenerationNaturelle(caster); return;
        case CustomSkill.ProfondeursFrappeRedoutable: FrappeRedoutable(caster); return;

        case CustomSkill.BelluaireFurieBestiale: FurieBestiale(caster); return;
        case CustomSkill.BelluaireRugissementProvoquant: BelluaireRugissementProvoquant(caster); return;
        case CustomSkill.BelluairePatteMielleuse: PatteMielleuse(caster); return;
        case CustomSkill.BelluaireChargeSanglier: ChargeDuSanglier(caster, target); return;
        case CustomSkill.BelluaireRageSanglier: RageDuSanglier(caster); return;
        case CustomSkill.BelluaireCorbeauAveuglement: CorbeauAveuglement(caster); return;
        case CustomSkill.BelluaireCorbeauMauvaisAugure: CorbeauMauvaisAugure(caster, target); return;
        //case CustomSkill.BelluaireLoupMorsurePlongeante: LoupMorsurePlongeante(caster); return;
        case CustomSkill.BelluaireSpiderWeb: SpiderWeb(caster, target is null ? onUseFeat.TargetPosition : target.Position); return;
        case CustomSkill.BelluaireSpiderCocoon: SpiderCocoon(caster, target); return;

        case CustomSkill.SensDivin: SensDivin(caster); return;
        case CustomSkill.ChatimentDivin: ChatimentDivin(caster); return;
        case CustomSkill.AuraDeProtection: AuraDeProtection(caster); return;
        case CustomSkill.DevotionSaintesRepresailles: SaintesRepresailles(caster, target); return;
        case CustomSkill.DevotionNimbeSacree: NimbeSacree(caster); return;
        case CustomSkill.AnciensGuerisonRayonnante: GuerisonRayonnante(caster); return;
        case CustomSkill.AnciensChampionAntique: ChampionAntique(caster); return;
        case CustomSkill.PaladinVoeuHostile: VoeudHostilite(caster, target); return;
        case CustomSkill.PaladinPuissanceInquisitrice: PuissanceInquisitrice(caster, target); return;
        case CustomSkill.AngeDeLaVengeance: AngeDeLaVengeance(caster); return;

        case CustomSkill.ClercInterventionDivine: InterventionDivine(caster); return;
        case CustomSkill.ClercIncantationPuissante: IncantationPuissante(caster, target); return;

        //case CustomSkill.ClercMartial: ClercMartial(caster); return;
        case CustomSkill.ClercFrappeGuidee: FrappeGuidee(caster, target); return;

        case CustomSkill.ClercIllumination: ClercIllumination(caster, target); return;
        case CustomSkill.ClercHaloDeLumiere: HaloDeLumiere(caster); return;

        case CustomSkill.ClercVisionDuPasse: VisionDuPasse(caster); return;

        case CustomSkill.ClercFureurDestructrice: FureurDestructrice(caster); return;
        case CustomSkill.ClercEnfantDeLaTempete: EnfantDeLaTempete(caster); return;

        case CustomSkill.ClercPreservationDeLaVie: PreservationDeLaVie(caster); return;

        case CustomSkill.ClercBenedictionDuFilou: BenedictionDuFilou(caster, target); return;
        case CustomSkill.TeleportRepliqueDuplicite: TranspositionDuFilou(caster); return;

        case CustomSkill.SorcellerieInnee: SorcellerieInnee(caster); return;
        case CustomSkill.SorcellerieIncarnee: SorcellerieIncarnee(caster); return;
        case CustomSkill.EnsoSourceToSlot: SourceToSlot(caster); return;
        case CustomSkill.EnsoSlotToSource: SlotToSource(caster); return;
        case CustomSkill.EnsoPrudence: 
        case CustomSkill.EnsoAllonge: 
        case CustomSkill.EnsoExtension: 
        case CustomSkill.EnsoAmplification: 
        case CustomSkill.EnsoGemellite: 
        case CustomSkill.EnsoIntensification: 
        case CustomSkill.EnsoAcceleration: 
        case CustomSkill.EnsoGuidage: 
        case CustomSkill.EnsoSubtilite: 
        case CustomSkill.EnsoTransmutation: Metamagie(caster, feat.Id); return;
        case CustomSkill.EnsoDracoWings: AilesDraconiques(caster); return;
        case CustomSkill.EnsoGuideTempete: GuideTempete(caster); return;
        case CustomSkill.EnsoAmeDesVents: AmeDesVents(caster); return;

        case CustomSkill.DruideReveilSauvage: ReveilSauvage(caster); return;
        case CustomSkill.DruideFrappePrimordialeFroid:
        case CustomSkill.DruideFrappePrimordialeFeu: 
        case CustomSkill.DruideFrappePrimordialeElec: 
        case CustomSkill.DruideFrappePrimordialeTonnerre: FrappePrimordiale(caster, feat.Id); return;
        case CustomSkill.MageNature: SlotToFormeSauvage(caster); return;

        case CustomSkill.FormeSauvageOurs: 
        case CustomSkill.FormeSauvageCorbeau: 
        case CustomSkill.FormeSauvageTigre: FormeSauvage(caster, feat.Id); return;
        case CustomSkill.FormeSauvageAir:
        case CustomSkill.FormeSauvageTerre:
        case CustomSkill.FormeSauvageFeu:
        case CustomSkill.FormeSauvageEau: FormeSauvage(caster, feat.Id, 2); return;

        case CustomSkill.DruideEconomieNaturelle: EconomieNaturelle(caster); return;
        case CustomSkill.DruideRecuperationNaturelle: RecuperationNaturelle(caster); return;
        case CustomSkill.DruideSanctuaireNaturel: SanctuaireNaturel(caster); return;
        case CustomSkill.DruideLuneRadieuse: LuneRadieuse(caster); return;
        case CustomSkill.DruideFureurDesFlots: FureurDesFlots(caster); return;

        case CustomSkill.OccultisteFourberieMagique: FourberieMagique(caster); return;
        case CustomSkill.OccultisteContactDoutremonde: ContactDoutremonde(caster); return;
        case CustomSkill.ChatimentOcculte: ChatimentOcculte(caster); return;
        case CustomSkill.DonDuProtecteur: DonDuProtecteur(caster, target); return;
        case CustomSkill.DoubleVue: DoubleVue(caster, target); return;
        case CustomSkill.PacteDuTome: PacteDuTome(caster); return;

        case CustomSkill.LueurDeGuérison: LueurDeGuérison(caster, target); return;
        case CustomSkill.VengeanceCalcinante: VengeanceCalcinante(caster); return;

        case CustomSkill.ResilienceFielleuse: ResilienceFielleuse(caster); return;
        case CustomSkill.TraverseeInfernale: TraverseeInfernale(caster); return;

        case CustomSkill.EspritEveille: EspritEveille(caster, target); return;
        case CustomSkill.SortsPsychiques: SortsPsychiques(caster); return;

        case CustomSkill.ExpertiseCommotion: Commotion(caster); return;
        case CustomSkill.ExpertiseAffaiblissement: Affaiblissement(caster); return;
        case CustomSkill.ExpertiseArretCardiaque: ArretCardiaque(caster); return;
        case CustomSkill.ExpertiseTranspercer: Transpercer(caster); return;
        case CustomSkill.ExpertiseTirPercant: TirPercant(caster); return;
        case CustomSkill.ExpertiseMoulinet: Moulinet(caster); return;
        case CustomSkill.ExpertiseLaceration: Laceration(caster); return;
        case CustomSkill.ExpertiseMutilation: Mutilation(caster); return;
        case CustomSkill.ExpertiseFendre: Fendre(caster); return;
        case CustomSkill.ExpertiseCharge: Charge(caster); return;
        case CustomSkill.ExpertiseFrappeDuPommeau: FrappeDuPommeau(caster); return;
        case CustomSkill.ExpertiseDesarmement: ExpertiseDesarmement(caster); return;
        case CustomSkill.ExpertiseBriseEchine: BriseEchine(caster); return;
        case CustomSkill.ExpertiseDestabiliser: Destabiliser(caster); return;
        case CustomSkill.ExpertiseCoupeJarret: CoupeJarret(caster); return;
        case CustomSkill.ExpertiseEntaille: Entaille(caster); return;
        case CustomSkill.ExpertiseRenforcement: Renforcement(caster); return;
        case CustomSkill.ExpertisePreparation: Preparation(caster); return;
        case CustomSkill.ExpertiseStabilisation: Stabilisation(caster); return;

        case CustomSkill.FighterSecondWind:
          if (CreatureUtils.HandleBonusActionUse(caster)) return;
          else onUseFeat.PreventFeatUse = true; return;
      }

      int featId = feat.Id + 10000;

      /*SkillSystem.Attribut featAttribute = SkillSystem.learnableDictionary[featId].attribut;
      int attributeLevel = player.GetAttributeLevel(featAttribute);
      int bonusAttributeChance = 0;

      NwItem castItem = caster.GetItemInSlot(InventorySlot.RightHand);

      if (castItem is not null)
      {
        if (castItem is not null && ItemUtils.GetItemAttribute(castItem) == featAttribute)
          for (int i = 0; i < castItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            if (castItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Maîtrise)
              bonusAttributeChance += 3;
      }

      castItem = caster.GetItemInSlot(InventorySlot.LeftHand);

      if (castItem is not null)
      {
        if (castItem is not null && ItemUtils.GetItemAttribute(castItem) == featAttribute)
          for (int i = 0; i < castItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            if (castItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Maîtrise)
              bonusAttributeChance += 3;
      }

      if (NwRandom.Roll(Utils.random, 100) < bonusAttributeChance)
        attributeLevel += 1;*/

      if (feat.Spell is not null && SpellUtils.IsBonusActionSpell(caster, feat.Spell.Id, Spells2da.spellTable[feat.Spell.Id], feat))
      {
        if (!CreatureUtils.HandleBonusActionUse(caster))
        {
          onUseFeat.PreventFeatUse = true;
          return;
        }
      }


      switch (featId)
      {
        case CustomSkill.SeverArtery:
          caster.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").Value = featId;
          return;
      }

      switch (feat.FeatType)
      {
        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:

          onUseFeat.PreventFeatUse = true;
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(feat.FeatType));
          break;
      }
    }
  }
}
