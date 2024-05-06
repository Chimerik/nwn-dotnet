using Anvil.Services;
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

        case CustomSkill.EnlargeDuergar:
          onUseFeat.Creature.GetObjectVariable<LocalVariableInt>("_ENLARGE_DUERGAR").Value = 1;
          return;

        case CustomSkill.HellishRebuke:
          HellishRebuke(onUseFeat.Creature, onUseFeat.TargetObject);
          onUseFeat.PreventFeatUse = true;
          return;

        case CustomSkill.MaitreBouclier:
          MaitreBouclier(onUseFeat.Creature, onUseFeat.TargetObject);
          onUseFeat.PreventFeatUse = true;
          return;

        case CustomSkill.Sprint: Sprint(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.Disengage: Disengage(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.Stealth: Stealth(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.Chargeur: Chargeur(onUseFeat.Creature, onUseFeat.TargetObject); return;
        //case CustomSkill.Dodge: Dodge(onUseFeat.Creature, player); return;
        case CustomSkill.FighterSecondWind: SecondWind(onUseFeat.Creature); return;
        case CustomSkill.FighterSurge: ActionSurge(onUseFeat.Creature); return;
        case CustomSkill.CogneurLourd: CogneurLourd(onUseFeat.Creature); return;
        case CustomSkill.TireurDelite: TireurDelite(onUseFeat.Creature); return;
        case CustomSkill.MageDeGuerre: MageDeGuerre(onUseFeat.Creature); return;
        case CustomSkill.FureurOrc: FureurOrc(onUseFeat.Creature); return;
        case CustomSkill.AgressionOrc: AgressionOrc(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.MeneurExaltant: MeneurExaltant(onUseFeat.Creature); return;
        case CustomSkill.Chanceux: Chanceux(onUseFeat.Creature); return;
        case CustomSkill.ConspirateurMaitriseTactique: TacticalMastery(onUseFeat.Creature, onUseFeat); return;

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
        case CustomSkill.WarMasterObservation: Observation(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.EldritchKnightArmeLiee:
        case CustomSkill.EldritchKnightArmeLiee2: ArmeLiee(onUseFeat.Creature, onUseFeat.Feat); return;
        case CustomSkill.EldritchKnightArmeLieeInvocation:
        case CustomSkill.EldritchKnightArmeLieeInvocation2: ArmeLieeInovcation(onUseFeat.Creature, onUseFeat.Feat); return;

        case CustomSkill.BarbarianRecklessAttack: RecklessAttack(onUseFeat.Creature); return;
        case (int)Feat.BarbarianRage: BarbarianRage(onUseFeat.Creature); return;
        case CustomSkill.BersekerFrenziedStrike: FrappeFrenetique(onUseFeat.Creature); return;

        case CustomSkill.TotemFerociteIndomptable: FerociteIndomptable(onUseFeat.Creature); return;
        case CustomSkill.TotemAspectTigre: AspectTigre(onUseFeat.Creature); return;
        case CustomSkill.TotemLienElan: LienElan(onUseFeat.Creature); return;

        case CustomSkill.WildMagicSense: SensDeLaMagie(onUseFeat.Creature); return;
        case CustomSkill.WildMagicTeleportation: Teleportation(onUseFeat.Creature); return;
        case CustomSkill.WildMagicMagieGalvanisanteBienfait: Bienfait(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.WildMagicMagieGalvanisanteRecuperation: Recuperation(onUseFeat.Creature, onUseFeat.TargetObject); return;

        case CustomSkill.MonkBonusAttack: MonkBonusAttack(onUseFeat.Creature); return;
        case CustomSkill.MonkPatience: MonkPatience(onUseFeat.Creature); return;
        case CustomSkill.MonkDelugeDeCoups: MonkDelugeDeCoups(onUseFeat.Creature); return;
        case CustomSkill.MonkSlowFall: MonkSlowFall(onUseFeat.Creature); return;
        case CustomSkill.MonkStunStrike: MonkStunStrike(onUseFeat.Creature); return;
        case CustomSkill.MonkDiamondSoul: DiamondSoul(onUseFeat.Creature); return;
        case CustomSkill.MonkDesertion: MonkDesertion(onUseFeat.Creature); return;
        case CustomSkill.MonkPlenitude: MonkPlenitude(onUseFeat.Creature); return;
        case CustomSkill.MonkManifestationAme: MonkManifestationAme(onUseFeat.Creature); return;
        case CustomSkill.MonkManifestationCorps: MonkManifestationCorps(onUseFeat.Creature); return;
        case CustomSkill.MonkManifestationEsprit: MonkManifestationEsprit(onUseFeat.Creature); return;
        case CustomSkill.MonkResonanceKi: MonkResonanceKi(onUseFeat.Creature); return;
        case CustomSkill.MonkExplosionKi: MonkExplosionKi(onUseFeat.Creature); return;
        case CustomSkill.MonkPaumeVibratoire: MonkPaumeVibratoire(onUseFeat.Creature); return;
        case CustomSkill.MonkTenebres: MonkTenebres(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkDarkVision: MonkDarkVision(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkPassageSansTrace: MonkPassageSansTrace(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkSilence: MonkSilence(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkLinceulDombre: MonkLinceulDombre(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkFouleeDombre: MonkFouleeDombre(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkFrappeDombre: MonkFrappeDombre(onUseFeat.Creature, onUseFeat); return;
        case CustomSkill.MonkHarmony: MonkHarmony(onUseFeat.Creature); return;

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
        case CustomSkill.ChantDuRepos: ChantDuRepos(onUseFeat.Creature); return;
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
