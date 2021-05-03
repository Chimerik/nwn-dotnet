using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using static NWN.Systems.Arena.Utils;

namespace NWN.Systems.Arena
{
  public static class Config
  {
    public const string PVE_ARENA_AREA_RESREF = "c2c_arena";
    public const string PVE_ARENA_WAYPOINT_TAG = "waypoint";
    public const string PVE_ARENA_MONSTER_WAYPOINT_TAG = "_ARENA_MONSTER_SPAWN";
    public const string PVE_ENTRY_WAYPOINT_TAG = "arena_entry_waypoint";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_TAG = "PullRopeChain";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_ON_USED_SCRIPT = "arena_chain_ou";
    public const string PVE_ARENA_CREATURE_ON_DEATH_SCRIPT = "arena_ondeath";
    public const string PVE_ARENA_CHALLENGER_VARNAME = "PVE_ARENA_CHALLENGER";
    public const string PVE_ARENA_CREATURE_TAG = "pve_arena_creature";
    public const int roundMax = 8;
    public enum Difficulty
    {
      Level1 = 1,
      Level2,
      Level3,
      Level4,
      Level5,
    }

    public static Dictionary<uint, ArenaMalus> arenaMalusDictionary = new Dictionary<uint, ArenaMalus>()
    {
      { 1, new ArenaMalus("Sorts de soins interdits", 3.01, ApplyNoHealingSpellMalus) },
      { 2, new ArenaMalus("Invocations interdites", 2.99, ApplyNoSummonsMalus) },
      { 3, new ArenaMalus("Magie offensive interdite", 2.89, ApplyNoOffensiveSpellsMalus) },
      { 4, new ArenaMalus("Magie défensive interdite", 3.14, ApplyNoBuffsMalus) },
      { 5, new ArenaMalus("Magie interdite", 3.16, ApplyNoMagicMalus) },
      { 6, new ArenaMalus("Accessoires interdits", 2.9, ApplyNoAccessoriesMalus) },
      { 7, new ArenaMalus("Armure interdite", 2.7, ApplyNoArmorMalus) },
      { 8, new ArenaMalus("Armes interdites", 3.08, ApplyNoWeaponsMalus) },
      { 9, new ArenaMalus("Utilisation d'objets interdite", 3.03, ApplyNoUseableItemMalus) },
      { 10, new ArenaMalus("Ralentissement", 1.9, ApplySlowMalus) },
      { 11, new ArenaMalus("Mini", 2.71, ApplyMiniMalus) },
      { 12, new ArenaMalus("Poison", 2, ApplyPoisonMalus) },
      { 13, new ArenaMalus("Crapaud", 2.72, ApplyFrogMalus) },
      { 14, new ArenaMalus("Temps x5", 2.47, ApplyTimeX5DamageMalus) },
      { 15, new ArenaMalus("1/2 HP", 2.60, ApplyHealthHalvedMalus) },
      { 16, new ArenaMalus("Echec des sorts", 2.52, ApplySpellFailureMalus) },
      { 17, new ArenaMalus("1/2 HP + Echec des sorts", 2.73, ApplyHealthHalvedAndSpellFailureMalus) },
      { 18, new ArenaMalus("Dissipation", 2.56, ApplyDissipationMalus) },
      { 19, new ArenaMalus("Chance", 0, ApplyNoMalus) },
      { 20, new ArenaMalus("Soins", 1, ApplyNoMalusAndHeal) },
    };

    private static async void ApplyNoMagicMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast -= SpellSystem.HandleBeforeSpellCast;
      player.oid.OnSpellCast -= NoMagicMalus;
      player.oid.OnSpellCast += NoMagicMalus;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      player.oid.OnSpellCast -= NoMagicMalus;

      player.oid.SendServerMessage("L'interdiction d'usage de sorts a été levée.", Color.ORANGE);
    }

    private static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de sorts est en vigueur.", Color.RED);
    }

    private static async void ApplyNoHealingSpellMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast -= NoHealingSpellMalus;
      player.oid.OnSpellCast += NoHealingSpellMalus;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast -= NoHealingSpellMalus;
      player.oid.SendServerMessage("L'interdiction d'usage de magie curative a été levée.", Color.ORANGE);
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
        case Spell.Regenerate:
        case Spell.MonstrousRegeneration:
          onSpellCast.PreventSpellCast = true;
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de magie curative est en vigueur.", Color.RED);
          break;
      }
    }

    private static async void ApplyNoSummonsMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast -= NoSummoningSpellMalus;
      player.oid.OnSpellCast += NoSummoningSpellMalus;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      foreach (NwCreature summon in player.oid.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast -= NoSummoningSpellMalus;
      player.oid.SendServerMessage("L'interdiction d'usage d'invocations a été levée.", Color.ORANGE);
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
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de sorts d'invocation est en vigueur.", Color.RED);
          break;
      }
    }
    private static async void ApplyNoOffensiveSpellsMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast -= NoOffensiveSpellMalus;
      player.oid.OnSpellCast += NoOffensiveSpellMalus;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast -= NoOffensiveSpellMalus;
      player.oid.SendServerMessage("L'interdiction d'usage de magie offensive a été levée.", Color.ORANGE);
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
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de magie offensive est en vigueur.", Color.RED);
          break;
      }
    }

    private static async void ApplyNoBuffsMalus(PlayerSystem.Player player)
    {
      player.oid.OnSpellCast -= NoBuffMalus;
      player.oid.OnSpellCast += NoBuffMalus;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      foreach (Effect effect in player.oid.ActiveEffects.Where(e => e.Tag != "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.RemoveEffect(effect);

      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDispelDisjunction));

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast -= NoBuffMalus;
      player.oid.SendServerMessage("L'interdiction d'usage de magie défensive a été levée.", Color.ORANGE);
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
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de magie défensive est en vigueur !", Color.RED);
          break;
      }
    }
    private static async void ApplyNoArmorMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task waitArmorEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Chest) != null, tokenSource.Token);
      Task waitArenaExit = NwTask.WaitUntil(() => player.pveArena.currentRound == 0, tokenSource.Token);

      await NwTask.WhenAny(waitArmorEquip, waitArenaExit);
      tokenSource.Cancel();

      if (waitArenaExit.IsCompletedSuccessfully)
      {
        player.oid.SendServerMessage("L'interdiction de port d'armure a été levée.", Color.ORANGE);
        return;
      }

      await NwTask.Delay(TimeSpan.FromSeconds(0.5));

      Task armorUnequip = NwTask.Run(async () =>
      {
        NwItem armor = player.oid.GetItemInSlot(InventorySlot.Chest);
        await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Chest));
        if (armor != null)
        {
          armor.Clone(player.oid);
          armor.Destroy();
        }
      });

      await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Chest) == null);

      player.oid.SendServerMessage("L'interdiction de port d'armure est en vigueur.", Color.RED);
      ApplyNoArmorMalus(player);
    }
    private static async void ApplyNoWeaponsMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task waitRightHandEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.RightHand) != null, tokenSource.Token);
      Task waitLeftHandEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.LeftHand) != null, tokenSource.Token);
      Task waitArenaExit = NwTask.WaitUntil(() => player.pveArena.currentRound == 0, tokenSource.Token);

      await NwTask.WhenAny(waitRightHandEquip, waitLeftHandEquip, waitArenaExit);
      tokenSource.Cancel();

      if (waitArenaExit.IsCompletedSuccessfully)
      {
        player.oid.SendServerMessage("L'interdiction de port d'arme a été levée.", Color.ORANGE);
        return;
      }

      await NwTask.Delay(TimeSpan.FromSeconds(0.5));

      if (waitRightHandEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.RightHand));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.RightHand) == null);
      }

      if (waitLeftHandEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.LeftHand));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.LeftHand) == null);
      }

      player.oid.SendServerMessage("L'interdiction de port d'armes est en vigueur.", Color.RED);
      ApplyNoWeaponsMalus(player);
    }
    private static async void ApplyNoAccessoriesMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task waitBeltEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Belt) != null, tokenSource.Token);
      Task waitArmsEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Arms) != null, tokenSource.Token);
      Task waitBootsEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Boots) != null, tokenSource.Token);
      Task waitCloakEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Cloak) != null, tokenSource.Token);
      Task waitLeftRingEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.LeftRing) != null, tokenSource.Token);
      Task waitRightRingEquip = NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.RightRing) != null, tokenSource.Token);
      Task waitArenaExit = NwTask.WaitUntil(() => player.pveArena.currentRound == 0, tokenSource.Token);

      await NwTask.WhenAny(waitBeltEquip, waitArmsEquip, waitBootsEquip, waitCloakEquip, waitLeftRingEquip, waitRightRingEquip, waitArenaExit);
      tokenSource.Cancel();

      if (waitArenaExit.IsCompletedSuccessfully)
      {
        player.oid.SendServerMessage("L'interdiction de port d'accessoire a été levée.", Color.ORANGE);
        return;
      }

      await NwTask.Delay(TimeSpan.FromSeconds(0.5));

      if (waitBeltEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Belt));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Belt) == null);
      }

      if (waitArmsEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Arms));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Arms) == null);
      }

      if (waitBootsEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Boots));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Boots) == null);
      }

      if (waitLeftRingEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.LeftRing));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.LeftRing) == null);
      }

      if (waitRightRingEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.RightRing));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.RightRing) == null);
      }

      if (waitCloakEquip.IsCompletedSuccessfully)
      {
        Task unequip = NwTask.Run(async () =>
        {
          await player.oid.ActionUnequipItem(player.oid.GetItemInSlot(InventorySlot.Cloak));
        });

        await NwTask.WaitUntil(() => player.oid.GetItemInSlot(InventorySlot.Cloak) == null);
      }

      player.oid.SendServerMessage("L'interdiction de port d'accessoires est en vigueur.", Color.RED);
      ApplyNoAccessoriesMalus(player);
    }
    private static async void ApplyNoUseableItemMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      PlayerSystem.eventService.Unsubscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(player.oid, ItemSystem.OnItemUseBefore);
      PlayerSystem.eventService.Subscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(player.oid, NoUseableItemMalus)
        .Register<ItemEvents.OnItemUseBefore>();

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      PlayerSystem.eventService.Unsubscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(player.oid, NoUseableItemMalus);
      PlayerSystem.eventService.Subscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(player.oid, ItemSystem.OnItemUseBefore)
        .Register<ItemEvents.OnItemUseBefore>();

      player.oid.SendServerMessage("L'interdiction de l'utilisation d'objets a été levée.", Color.ORANGE);
    }
    private static void NoUseableItemMalus(ItemEvents.OnItemUseBefore onItemUse)
    {
      onItemUse.Skip = true;
      ((NwPlayer)onItemUse.Creature).SendServerMessage("L'interdiction d'utilisation d'objets est en vigueur.", Color.RED);
    }
    private static async void ApplySlowMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));

      Effect slow = Effect.Slow();
      slow.Tag = "_ARENA_MALUS_SLOW";
      slow.SubType = EffectSubType.Supernatural;

      Effect msDecrease = Effect.MovementSpeedDecrease(10);
      slow.Tag = "_ARENA_MALUS_SLOW";
      slow.SubType = EffectSubType.Supernatural;

      player.oid.ApplyEffect(EffectDuration.Permanent, slow);
      player.oid.ApplyEffect(EffectDuration.Permanent, msDecrease);

      player.oid.OnSpellCastAt -= SlowMalusCure;
      player.oid.OnSpellCastAt += SlowMalusCure;

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      foreach (Effect eff in player.oid.ActiveEffects.Where(e => e.Tag == "_ARENA_MALUS_SLOW"))
        player.oid.RemoveEffect(eff);

      player.oid.OnSpellCastAt -= SlowMalusCure;
      player.oid.SendServerMessage("Le handicap de ralentissement a été levé.", Color.ORANGE);
    }
    public static void SlowMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:
        case Spell.Haste:
        case Spell.MassHaste:

          foreach (Effect eff in onSpellCastAt.Creature.ActiveEffects.Where(e => e.Tag == "_ARENA_MALUS_SLOW"))
            onSpellCastAt.Creature.RemoveEffect(eff);

          onSpellCastAt.Creature.OnSpellCastAt -= SlowMalusCure;
          break;
      }
    }
    private static async void ApplyMiniMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (player.oid.GetLocalVariable<float>("_ARENA_MALUS_MINI").HasValue)
        return;

      VisualTransform visualT = player.oid.VisualTransform;
      player.oid.GetLocalVariable<float>("_ARENA_MALUS_MINI").Value = player.oid.VisualTransform.Scale;
      visualT.Scale *= 0.6f;
      player.oid.VisualTransform = visualT;

      player.oid.OnCreatureDamage -= MiniMalus;
      player.oid.OnSpellCastAt -= MiniMalusCure;
      player.oid.OnCreatureDamage += MiniMalus;
      player.oid.OnSpellCastAt += MiniMalusCure;

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      if (player.oid.GetLocalVariable<float>("_ARENA_MALUS_MINI").HasValue)
      {
        visualT.Scale = player.oid.GetLocalVariable<float>("_ARENA_MALUS_MINI").Value;
        player.oid.GetLocalVariable<float>("_ARENA_MALUS_MINI").Delete();
        player.oid.VisualTransform = visualT;
      }
          
      player.oid.OnCreatureDamage -= MiniMalus;
      player.oid.OnSpellCastAt -= MiniMalusCure;
      player.oid.SendServerMessage("Le handicap de miniaturisation a été levé.", Color.ORANGE);
    }
    public static void MiniMalus(OnCreatureDamage onDamage)
    {
      onDamage.DamageData.Base = 1;
    }
    public static void MiniMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:

          if (onSpellCastAt.Creature.GetLocalVariable<float>("_ARENA_MALUS_MINI").HasValue)
          {
            VisualTransform vt = onSpellCastAt.Creature.VisualTransform;
            vt.Scale = onSpellCastAt.Creature.GetLocalVariable<float>("_ARENA_MALUS_MINI").Value;
            onSpellCastAt.Creature.GetLocalVariable<float>("_ARENA_MALUS_MINI").Delete();
            onSpellCastAt.Creature.VisualTransform = vt;
          }

          onSpellCastAt.Creature.OnCreatureDamage -= MiniMalus;
          onSpellCastAt.Creature.OnSpellCastAt -= MiniMalusCure;
          break;
      }
    }
    private static async void ApplyFrogMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (player.oid.GetLocalVariable<float>("_ARENA_MALUS_FROG").HasValue)
        return;

      player.oid.GetLocalVariable<int>("_ARENA_MALUS_FROG").Value = (int)player.oid.CreatureAppearanceType;

      player.oid.CreatureAppearanceType = (AppearanceType)6396;
      player.oid.OnSpellCast -= FrogSpellMalus;
      player.oid.OnCreatureDamage -= FrogMalus;
      player.oid.OnSpellCastAt -= FrogMalusCure;
      player.oid.OnCreatureDamage += FrogMalus;
      player.oid.OnSpellCastAt += FrogMalusCure;
      player.oid.OnSpellCast += FrogSpellMalus;

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      if (player.oid.GetLocalVariable<int>("_ARENA_MALUS_FROG").HasValue)
        player.oid.CreatureAppearanceType = (AppearanceType)player.oid.GetLocalVariable<int>("_ARENA_MALUS_FROG").Value;

      player.oid.OnCreatureDamage -= FrogMalus;
      player.oid.OnSpellCastAt -= FrogMalusCure;
      player.oid.OnSpellCast -= FrogSpellMalus;
      player.oid.SendServerMessage("Le handicap de métamorphose a été levé.", Color.ORANGE);
    }
    public static void FrogMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:

          if (onSpellCastAt.Creature.GetLocalVariable<int>("_ARENA_MALUS_FROG").HasValue)
            onSpellCastAt.Creature.CreatureAppearanceType = (AppearanceType)onSpellCastAt.Creature.GetLocalVariable<int>("_ARENA_MALUS_FROG").Value;

          onSpellCastAt.Creature.OnCreatureDamage -= FrogMalus;
          onSpellCastAt.Creature.OnSpellCastAt -= FrogMalusCure;
          onSpellCastAt.Creature.OnSpellCast -= FrogSpellMalus;
          break;
      }
    }
    public static void FrogMalus(OnCreatureDamage onDamage)
    {
      int damage = onDamage.DamageData.Base / 4;
      if (damage < 1)
        damage = 1;
      onDamage.DamageData.Base = damage;
    }
    public static void FrogSpellMalus(OnSpellCast onSpellCast)
    {
      switch(onSpellCast.Spell)
      {
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:
          return;
      }

      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("La métamorphose vous empêche de faire usage de magie !");
    }
    private static async void ApplyPoisonMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonL));

      if (player.oid.RollSavingThrow(SavingThrow.Fortitude, 15 + ((int)player.pveArena.currentDifficulty * 2), SavingThrowType.Poison) != SavingThrowResult.Failure)
        return;

      Effect poison = Effect.VisualEffect(VfxType.DurGlowGreen);
      poison.SubType = EffectSubType.Supernatural;
      poison.Tag = "_ARENA_MALUS_POISON";
      player.oid.ApplyEffect(EffectDuration.Permanent, poison);
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.Damage(player.oid.MaxHP / 4, DamageType.Acid));

      player.oid.OnHeartbeat -= PoisonMalus;
      player.oid.OnSpellCastAt -= PoisonMalusCure;
      player.oid.OnHeartbeat += PoisonMalus;
      player.oid.OnSpellCastAt += PoisonMalusCure;

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.RemoveEffect(poison);
      player.oid.OnHeartbeat -= PoisonMalus;
      player.oid.OnSpellCastAt -= PoisonMalusCure;
      player.oid.SendServerMessage("Le handicap d'empoisonnement a été levé.", Color.ORANGE);
    }
    public static void PoisonMalus(CreatureEvents.OnHeartbeat onHearbeat)
    {
      int hpLost = onHearbeat.Creature.MaxHP / 32;
      if (hpLost < 1)
        hpLost = 1;

      onHearbeat.Creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(hpLost, DamageType.Acid));
    }
    public static void PoisonMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch(onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.NeutralizePoison:
          Effect poison = onSpellCastAt.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == "_ARENA_MALUS_POISON");
          if(poison != null)
            onSpellCastAt.Creature.RemoveEffect(poison);
          onSpellCastAt.Creature.OnHeartbeat -= PoisonMalus;
          onSpellCastAt.Creature.OnSpellCastAt -= PoisonMalusCure;
          break;
      }
    }
    public static void ApplyTimeX5DamageMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));

      int damage = (DateTime.Now - player.pveArena.dateArenaEntered).Seconds / 10;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Magical));
    }
    private static async void ApplyHealthHalvedMalus(PlayerSystem.Player player)
    {
      if (player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").HasNothing)
        player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").Value = CreaturePlugin.GetMaxHitPointsByLevel(player.oid, 1);

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, CreaturePlugin.GetMaxHitPointsByLevel(player.oid, 1) / 2);

      if (player.oid.HP > player.oid.MaxHP)
        player.oid.HP = player.oid.MaxHP;
      
      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      if (player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").HasNothing)
        return;

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").Value);
      player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").Delete();

      player.oid.SendServerMessage("Le handicap de résilience a été levé.", Color.ORANGE);
    }
    private static async void ApplySpellFailureMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));

      player.oid.OnSpellCast += SpellFailureMalus;

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast -= SpellFailureMalus;
      player.oid.SendServerMessage("Le handicap d'échec des sorts a été levé.", Color.ORANGE);
    }

    private static void SpellFailureMalus(OnSpellCast onSpellCast)
    {
      if (NwRandom.Roll(NWN.Utils.random, 100, 1) < 6)
      {
        onSpellCast.PreventSpellCast = true;
        ((NwPlayer)onSpellCast.Caster).SendServerMessage("Votre sort échoue en raison du handicap d'échec des sorts.", Color.RED);
      }
    }
    private static async void ApplyHealthHalvedAndSpellFailureMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));

      player.oid.OnSpellCast += SpellFailureMalus;

      if (player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").HasNothing)
        player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").Value = CreaturePlugin.GetMaxHitPointsByLevel(player.oid, 1);

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, CreaturePlugin.GetMaxHitPointsByLevel(player.oid, 1) / 2);
      if (player.oid.HP > player.oid.MaxHP)
        player.oid.HP = player.oid.MaxHP;

      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      player.oid.OnSpellCast -= SpellFailureMalus;
      player.oid.SendServerMessage("Le handicap d'échec des sorts a été levé.", Color.ORANGE);

      if (player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").HasNothing)
        return;

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").Value);
      player.oid.GetLocalVariable<int>("_ARENA_MALUS_HEALTH").Delete();

      player.oid.SendServerMessage("Le handicap de résilience a été levé.", Color.ORANGE);
    }
    private static void ApplyDissipationMalus(PlayerSystem.Player player)
    {
      foreach (NwCreature summon in player.oid.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }

      player.oid.ApplyEffect(EffectDuration.Instant, Effect.DispelMagicAll(100));
      player.oid.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfDispelDisjunction));
    }
    private static void ApplyNoMalus(PlayerSystem.Player player)
    {
      player.oid.SendServerMessage("Quelle chance, aucun handicap !", Color.PINK);
    }
    private static void ApplyNoMalusAndHeal(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingG));
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.Heal(player.oid.MaxHP / 2));
      player.oid.SendServerMessage("Quelle chance, aucun handicap et des soins gratuits !", Color.PINK);
    }
    public static RoundCreatures[] GetNormalEncounters(Difficulty difficulty) {
    switch(difficulty)
    {
        default: throw new Exception($"PvE Arena: Invalid normal encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "goblinfooder", "goblinfooder", "gobelinfrondeur", "gobelinfrondeur", "gobelinfourbe" },
            points: 1  
          ),
          new RoundCreatures(
            resrefs: new string[] { "bat_sewer", "bat_sewer", "bat_sewer", "bat_sewer" },
            points: 1
          ),
          new RoundCreatures(
            resrefs: new string[] { "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect"   },
            points: 2
          ),
          new RoundCreatures(
            resrefs: new string[] { "dog_sewer_starve", "dog_sewer_starve", "dog_sewer_starve" },
            points: 5
          ),
          new RoundCreatures(
            resrefs: new string[] { "rat_meca", "rat_meca", "rat_meca", "rat_meca" },
            points: 6
          ),
          new RoundCreatures(
            resrefs: new string[] { "crab_meca", "crab_meca", "pingu_meca", "pingu_meca" },
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
            resrefs: new string[] { "cutter_meca", "cutter_meca" },
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
            resrefs: new string[] { "rat_meca", "rat_meca", "rat_meca", "dog_meca_defect" },
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
