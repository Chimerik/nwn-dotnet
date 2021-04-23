using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using static NWN.Systems.Arena.Utils;

namespace NWN.Systems.Arena
{
  public static class Config
  {
    public const string PVE_ARENA_AREA_RESREF = "c2c_arena";
    public const string PVE_ARENA_WAYPOINT_TAG = "waypoint";
    public const string PVE_ENTRY_WAYPOINT_TAG = "arena_entry_waypoint";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_TAG = "PullRopeChain";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_ON_USED_SCRIPT = "arena_chain_ou";
    public const string PVE_ARENA_CREATURE_ON_DEATH_SCRIPT = "arena_ondeath";
    public const string PVE_ARENA_CHALLENGER_VARNAME = "PVE_ARENA_CHALLENGER";
    public const string PVE_ARENA_CREATURE_TAG = "pve_arena_creature";
    public const int roundMax = 8;
    public enum Difficulty
    {
      Level1,
      Level2,
      Level3,
      Level4,
      Level5,
    }

    public static Dictionary<uint, ArenaMalus> arenaMalusDictionary = new Dictionary<uint, ArenaMalus>()
    {
      { 1, new ArenaMalus("Sorts de soins interdits", ApplyNoHealingSpellMalus) },
      { 2, new ArenaMalus("Invocations interdites", ApplyNoSummonsMalus) },
      { 3, new ArenaMalus("Magie offensive interdite", ApplyNoOffensiveSpellsMalus) },
      { 4, new ArenaMalus("Magie défensive interdite", ApplyNoBuffsMalus) },
      { 5, new ArenaMalus("Magie interdite", ApplyNoMagicMalus) },
      { 6, new ArenaMalus("Accessoires interdits", ApplyNoAccessoriesMalus) },
      { 7, new ArenaMalus("Armure interdite", ApplyNoArmorMalus) },
      { 8, new ArenaMalus("Armes interdites", ApplyNoWeaponsMalus) },
      { 9, new ArenaMalus("Utilisation d'objets interdite", ApplyNoSummonsMalus) },
      { 10, new ArenaMalus("Ralentissement", ApplyNoSummonsMalus) },
      { 11, new ArenaMalus("Mini", ApplyNoSummonsMalus) },
      { 12, new ArenaMalus("Poison", ApplyNoSummonsMalus) },
      { 13, new ArenaMalus("Crapaud", ApplyNoSummonsMalus) },
      { 14, new ArenaMalus("Temps x5", ApplyNoSummonsMalus) },
      { 15, new ArenaMalus("1/2 HP", ApplyNoSummonsMalus) },
      { 16, new ArenaMalus("Echec des sorts", ApplyNoSummonsMalus) },
      { 17, new ArenaMalus("1/2 HP + Echec des sorts", ApplyNoSummonsMalus) },
      { 18, new ArenaMalus("Dissipation", ApplyNoSummonsMalus) },
      { 19, new ArenaMalus("Chance", ApplyNoSummonsMalus) },
      { 20, new ArenaMalus("Soins", ApplyNoSummonsMalus) },
    };

    private static void ApplyNoMagicMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast += NoMagicMalus;

      Task malusSelected = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.pveArena.currentRound == 1);
        player.oid.OnSpellCast -= NoMagicMalus;
      });
    }

    private static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre malus actuel vous empêche de faire usage de sorts !", Color.RED);
    }

    private static void ApplyNoHealingSpellMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast += NoHealingSpellMalus;

      Task malusSelected = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.pveArena.currentRound == 1);
        player.oid.OnSpellCast -= NoHealingSpellMalus;
      });
    }

    private static void NoHealingSpellMalus(OnSpellCast onSpellCast)
    {
      switch(onSpellCast.Spell)
      {
        case Spell.Heal:
        case Spell.HealingCircle:
        case Spell.MassHeal:
        case Spell.CureCriticalWounds:
        case Spell.CureLightWounds:
        case Spell.CureMinorWounds:
        case Spell.CureModerateWounds:
        case Spell.CureSeriousWounds:
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveBlindnessAndDeafness:
        case Spell.RemoveCurse:
        case Spell.RemoveDisease:
        case Spell.RemoveFear:
        case Spell.RemoveParalysis:
        case Spell.NaturesBalance:
        case Spell.NeutralizePoison:
          onSpellCast.PreventSpellCast = true;
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre malus actuel vous empêche de faire usage de sorts de soins !", Color.RED);
          break;
      }
    }

    private static void ApplyNoSummonsMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast += NoSummoningSpellMalus;

      foreach(NwCreature summon in player.oid.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }

      Task malusSelected = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.pveArena.currentRound == 1);
        player.oid.OnSpellCast -= NoSummoningSpellMalus;
      });
    }
    private static void NoSummoningSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.AnimateDead:
        case Spell.AbilityPmAnimateDead:
        case Spell.SummonCreatureI:
        case Spell.SummonCreatureIi:
        case Spell.SummonCreatureIii:
        case Spell.SummonCreatureIv:
        case Spell.SummonCreatureIx:
        case Spell.SummonCreatureV:
        case Spell.SummonCreatureVi:
        case Spell.SummonCreatureVii:
        case Spell.SummonCreatureViii:
        case Spell.SummonShadow:
        case Spell.CreateUndead:
        case Spell.CreateGreaterUndead:
        case Spell.ElementalSwarm:
        case Spell.AbilityPmSummonGreaterUndead:
        case Spell.AbilityPmSummonUndead:
        case Spell.AbilitySummonAnimalCompanion:
        case Spell.AbilitySummonCelestial:
        case Spell.AbilitySummonFamiliar:
        case Spell.AbilitySummonMephit:
        case Spell.AbilitySummonSlaad:
        case Spell.AbilitySummonTanarri:
        case Spell.ElementalSummoningItem:
        case Spell.GreaterShadowConjurationSummonShadow:
        case Spell.PaladinSummonMount:
        case Spell.ShadesSummonShadow:
        case Spell.ShadowConjurationSummonShadow:
        case Spell.BlackBladeOfDisaster:
        case Spell.Blackstaff:
        case Spell.Gate:
        case Spell.PlanarAlly:
        case Spell.PlanarBinding:
        case Spell.GreaterPlanarBinding:
        case Spell.MordenkainensSword:
        case Spell.ShelgarnsPersistentBlade:
          onSpellCast.PreventSpellCast = true;
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre malus actuel vous empêche de faire usage de sorts d'invocation !", Color.RED);
          break;
      }
    }
    private static void ApplyNoOffensiveSpellsMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast += NoOffensiveSpellMalus;

      Task malusSelected = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.pveArena.currentRound == 1);
        player.oid.OnSpellCast -= NoOffensiveSpellMalus;
      });
    }
    private static void NoOffensiveSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.AcidSplash:
        case Spell.BallLightning:
        case Spell.BigbysClenchedFist:
        case Spell.BigbysCrushingHand:
        case Spell.BigbysForcefulHand:
        case Spell.BigbysGraspingHand:
        case Spell.BladeBarrier:
        case Spell.Bombardment:
        case Spell.BurningHands:
        case Spell.CallLightning:
        case Spell.ChainLightning:
        case Spell.CircleOfDeath:
        case Spell.CircleOfDoom:
        case Spell.Cloudkill:
        case Spell.Combust:
        case Spell.ConeOfCold:
        case Spell.Crumble:
        case Spell.DelayedBlastFireball:
        case Spell.Destruction:
        case Spell.Drown:
        case Spell.Earthquake:
        case Spell.ElectricJolt:
        case Spell.EpicHellball:
        case Spell.EpicRuin:
        case Spell.EnergyDrain:
        case Spell.Enervation:
        case Spell.EvardsBlackTentacles:
        case Spell.FingerOfDeath:
        case Spell.Fireball:
        case Spell.Firebrand:
        case Spell.FireStorm:
        case Spell.FlameArrow:
        case Spell.FlameLash:
        case Spell.FlameStrike:
        case Spell.FleshToStone:
        case Spell.GedleesElectricLoop:
        case Spell.GreaterShadowConjurationAcidArrow:
        case Spell.HammerOfTheGods:
        case Spell.Harm:
        case Spell.HorizikaulsBoom:
        case Spell.HorridWilting:
        case Spell.IceDagger:
        case Spell.IceStorm:
        case Spell.Implosion:
        case Spell.IncendiaryCloud:
        case Spell.Inferno:
        case Spell.InflictCriticalWounds:
        case Spell.InflictLightWounds:
        case Spell.InflictMinorWounds:
        case Spell.InflictModerateWounds:
        case Spell.InflictSeriousWounds:
        case Spell.IsaacsGreaterMissileStorm:
        case Spell.IsaacsLesserMissileStorm:
        case Spell.LightningBolt:
        case Spell.MagicMissile:
        case Spell.MelfsAcidArrow:
        case Spell.MestilsAcidBreath:
        case Spell.MeteorSwarm:
        case Spell.NegativeEnergyBurst:
        case Spell.NegativeEnergyRay:
        case Spell.PhantasmalKiller:
        case Spell.Poison:
        case Spell.PowerWordKill:
        case Spell.Quillfire:
        case Spell.RayOfFrost:
        case Spell.SearingLight:
        case Spell.ShadesConeOfCold:
        case Spell.ShadesFireball:
        case Spell.ShadesWallOfFire:
        case Spell.ShadowConjurationMagicMissile:
        case Spell.SlayLiving:
        case Spell.SoundBurst:
        case Spell.SpikeGrowth:
        case Spell.StormOfVengeance:
        case Spell.Sunbeam:
        case Spell.Sunburst:
        case Spell.UndeathsEternalFoe:
        case Spell.UndeathToDeath:
        case Spell.VampiricTouch:
        case Spell.WailOfTheBanshee:
        case Spell.WallOfFire:
        case Spell.Weird:
        case Spell.WordOfFaith:
          onSpellCast.PreventSpellCast = true;
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre malus actuel vous empêche de faire usage de sorts offensifs !", Color.RED);
          break;
      }
    }

    private static void ApplyNoBuffsMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast += NoBuffMalus;

      foreach (API.Effect effect in player.oid.ActiveEffects.Where(e => e.Tag != "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.RemoveEffect(effect);

      player.oid.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.ImpDispelDisjunction));

      Task malusSelected = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.pveArena.currentRound == 1);
        player.oid.OnSpellCast -= NoBuffMalus;
      });
    }
    private static void NoBuffMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.Aid:
        case Spell.Amplify:
        case Spell.Auraofglory:
        case Spell.AuraOfVitality:
        case Spell.Awaken:
        case Spell.Barkskin:
        case Spell.Battletide:
        case Spell.BladeThirst:
        case Spell.Bless:
        case Spell.BlessWeapon:
        case Spell.BloodFrenzy:
        case Spell.BullsStrength:
        case Spell.Camoflage:
        case Spell.CatsGrace:
        case Spell.Charger:
        case Spell.ClairaudienceAndClairvoyance:
        case Spell.Clarity:
        case Spell.CloakOfChaos:
        case Spell.ContinualFlame:
        case Spell.Darkfire:
        case Spell.Darkvision:
        case Spell.DeathArmor:
        case Spell.DeathWard:
        case Spell.Displacement:
        case Spell.DivineFavor:
        case Spell.DivineMight:
        case Spell.DivinePower:
        case Spell.DivineShield:
        case Spell.EagleSpledor:
        case Spell.ElementalShield:
        case Spell.Endurance:
        case Spell.EndureElements:
        case Spell.EnergyBuffer:
        case Spell.EntropicShield:
        case Spell.EpicMageArmor:
        case Spell.Etherealness:
        case Spell.EtherealVisage:
        case Spell.ExpeditiousRetreat:
        case Spell.FlameWeapon:
        case Spell.FoxsCunning:
        case Spell.FreedomOfMovement:
        case Spell.GhostlyVisage:
        case Spell.GlobeOfInvulnerability:
        case Spell.GlyphOfWarding:
        case Spell.GreaterBullsStrength:
        case Spell.GreaterCatsGrace:
        case Spell.GreaterEagleSplendor:
        case Spell.GreaterEndurance:
        case Spell.GreaterFoxsCunning:
        case Spell.GreaterMagicFang:
        case Spell.GreaterMagicWeapon:
        case Spell.GreaterOwlsWisdom:
        case Spell.GreaterShadowConjurationMinorGlobe:
        case Spell.GreaterShadowConjurationMirrorImage:
        case Spell.GreaterSpellMantle:
        case Spell.GreaterStoneskin:
        case Spell.Haste:
        case Spell.HolyAura:
        case Spell.HolySword:
        case Spell.ImprovedInvisibility:
        case Spell.Invisibility:
        case Spell.InvisibilitySphere:
        case Spell.Ironguts:
        case Spell.KeenEdge:
        case Spell.LesserMindBlank:
        case Spell.LesserSpellMantle:
        case Spell.Light:
        case Spell.MageArmor:
        case Spell.MagicCircleAgainstChaos:
        case Spell.MagicCircleAgainstEvil:
        case Spell.MagicCircleAgainstGood:
        case Spell.MagicCircleAgainstLaw:
        case Spell.MagicFang:
        case Spell.MagicVestment:
        case Spell.MagicWeapon:
        case Spell.MassCamoflage:
        case Spell.MassHaste:
        case Spell.MestilsAcidSheath:
        case Spell.MindBlank:
        case Spell.MinorGlobeOfInvulnerability:
        case Spell.MonstrousRegeneration:
        case Spell.NegativeEnergyProtection:
        case Spell.OneWithTheLand:
        case Spell.OwlsInsight:
        case Spell.OwlsWisdom:
        case Spell.PolymorphSelf:
        case Spell.Prayer:
        case Spell.Premonition:
        case Spell.ProtectionFromChaos:
        case Spell.ProtectionFromElements:
        case Spell.ProtectionFromEvil:
        case Spell.ProtectionFromGood:
        case Spell.ProtectionFromLaw:
        case Spell.ProtectionFromSpells:
        case Spell.Regenerate:
        case Spell.Resistance:
        case Spell.ResistElements:
        case Spell.Sanctuary:
        case Spell.SeeInvisibility:
        case Spell.ShadesStoneskin:
        case Spell.ShadowConjurationInivsibility:
        case Spell.ShadowConjurationMageArmor:
        case Spell.ShadowShield:
        case Spell.Shapechange:
        case Spell.Shield:
        case Spell.ShieldOfFaith:
        case Spell.ShieldOfLaw:
        case Spell.Silence:
        case Spell.SpellMantle:
        case Spell.SpellResistance:
        case Spell.StoneBones:
        case Spell.StoneToFlesh:
        case Spell.TensersTransformation:
        case Spell.TimeStop:
        case Spell.TrueSeeing:
        case Spell.TrueStrike:
        case Spell.TymorasSmile:
        case Spell.Virtue:
        case Spell.WarCry:
        case Spell.WoundingWhispers:
          onSpellCast.PreventSpellCast = true;
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre malus actuel vous empêche de faire usage de sorts défensifs !", Color.RED);
          break;
      }
    }
    private static async void ApplyNoArmorMalus(PlayerSystem.Player player)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task waitArmorEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Chest) != null, tokenSource.Token);
      Task waitArenaExit = NwTask.WaitUntil(() => player.pveArena.currentRound == 1, tokenSource.Token);

      await NwTask.WhenAny(waitArmorEquip, waitArenaExit);
      tokenSource.Cancel();

      if (waitArenaExit.IsCompletedSuccessfully)
      {
        player.oid.SendServerMessage("L'interdiction de port d'armure a été levée.", Color.ORANGE);
        return;
      }

      await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Chest));

      ApplyNoArmorMalus(player);
    }
    private static async void ApplyNoWeaponsMalus(PlayerSystem.Player player)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task waitRightHandEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.RightHand) != null, tokenSource.Token);
      Task waitLeftHandEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.LeftHand) != null, tokenSource.Token);
      Task waitArenaExit = NwTask.WaitUntil(() => player.pveArena.currentRound == 1, tokenSource.Token);

      await NwTask.WhenAny(waitRightHandEquip, waitLeftHandEquip, waitArenaExit);
      tokenSource.Cancel();

      if (waitArenaExit.IsCompletedSuccessfully)
      {
        player.oid.SendServerMessage("L'interdiction de port d'arme a été levée.", Color.ORANGE);
        return;
      }

      if (waitRightHandEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.RightHand));

      if(waitLeftHandEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.LeftHand));

      ApplyNoWeaponsMalus(player);
    }
    private static async void ApplyNoAccessoriesMalus(PlayerSystem.Player player)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task waitNeckEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Neck) != null, tokenSource.Token);
      Task waitBeltEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Belt) != null, tokenSource.Token);
      Task waitArmsEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Arms) != null, tokenSource.Token);
      Task waitBootsEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Boots) != null, tokenSource.Token);
      Task waitCloakEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Cloak) != null, tokenSource.Token);
      Task waitLeftRingEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.LeftRing) != null, tokenSource.Token);
      Task waitRightRingEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.RightRing) != null, tokenSource.Token);
      Task waitArenaExit = NwTask.WaitUntil(() => player.pveArena.currentRound == 1, tokenSource.Token);

      await NwTask.WhenAny(waitNeckEquip, waitBeltEquip, waitArmsEquip, waitBootsEquip, waitCloakEquip, waitLeftRingEquip, waitRightRingEquip, waitArenaExit);
      tokenSource.Cancel();

      if (waitArenaExit.IsCompletedSuccessfully)
      {
        player.oid.SendServerMessage("L'interdiction de port d'accessoire a été levée.", Color.ORANGE);
        return;
      }

      if (waitNeckEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Neck));

      if (waitBeltEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Belt));

      if (waitArmsEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Arms));

      if (waitBootsEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Boots));

      if (waitLeftRingEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.LeftRing));

      if (waitRightRingEquip.IsCompletedSuccessfully)
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.RightRing));

      ApplyNoAccessoriesMalus(player);
    }
    public static RoundCreatures[] GetNormalEncounters(Difficulty difficulty) {
      switch(difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid normal encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "gobelineclaireur", "gobelineclaireur", "gobelineclaireur", "gobelineclaireur" },
            points: 1  
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_bat", "nw_bat", "nw_bat" },
            points: 1
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_rat001", "nw_rat001" },
            points: 2
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_cougar" },
            points: 5
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_jaguar" },
            points: 6
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_dog", "nw_dog" },
            points: 4
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }

    public static RoundCreatures[] GetEliteEncounters(Difficulty difficulty)
    {
      switch (difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid elite encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "nw_orca" },
            points: 10
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_gnoll001" },
            points: 10
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_boar" },
            points: 10
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }

    public static RoundCreatures[] GetBossEncounters(Difficulty difficulty)
    {
      switch (difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid boss encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "nw_bearbrwn" },
            points: 20
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_fire" },
            points: 20
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_spiddire" },
            points: 20
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }
  }
}
