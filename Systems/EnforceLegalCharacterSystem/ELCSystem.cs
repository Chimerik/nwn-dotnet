using System.Collections.Generic;

using NLog;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ELCSystem))]
  public class ELCSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public readonly EnforceLegalCharacterService elcService;
    public readonly AreaSystem areaSystem;
    public ELCSystem(EnforceLegalCharacterService elcService, AreaSystem areaSystem)
    {
      this.elcService = elcService;
      elcService.EnforceDefaultEventScripts = true;
      elcService.EnforceEmptyDialog = true;
      elcService.OnValidationFailure += onELCValidationFailure;
      elcService.OnValidationSuccess += onELCValidationSuccess;
      this.areaSystem = areaSystem;
    }
    private void onELCValidationSuccess(OnELCValidationSuccess onELCSuccess)
    {
      int characterId = onELCSuccess.Player.ControlledCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value;

      if (characterId > 0)
      {
        var query = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "location" } },
        new List<string[]>() { new string[] { "rowid", characterId.ToString() } });

        foreach(var result in query)
        {
          Location spawnLoc = SqLiteUtils.DeserializeLocation(result[0]);

          if (spawnLoc.Area is null)
          {
            if(onELCSuccess.Player.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").HasValue)
              onELCSuccess.Player.SpawnLocation = PlayerSystem.CreateIntroScene(onELCSuccess.Player, areaSystem);

            return;
          }
          else
            onELCSuccess.Player.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").Delete();


          onELCSuccess.Player.SpawnLocation = spawnLoc;
        }
      }
    }
    private void onELCValidationFailure(OnELCValidationFailure onELCFailure)
    {
      NwCreature oPC = onELCFailure.Player.ControlledCreature;

      if (oPC.GetObjectVariable<PersistentVariableInt>("characterId").HasValue)
      {
        onELCFailure.IgnoreFailure = true;
        LogUtils.LogMessage($"ELC VALIDATION IGNORED - Player {onELCFailure.Player.PlayerName} - Character {oPC.Name} - type : {onELCFailure.Type} - SubType : {onELCFailure.SubType}", LogUtils.LogType.PlayerConnections);
      }
      else
      {
        if ((onELCFailure.Type == ValidationFailureType.Character && onELCFailure.SubType == ValidationFailureSubType.ClassSpellcasterInvalidPrimaryStat && oPC.GetAbilityScore(Ability.Intelligence, true) < 11)
          || onELCFailure.Type == ValidationFailureType.Spell)
          onELCFailure.IgnoreFailure = true;
        else
          LogUtils.LogMessage($"ELC VALIDATION FAILURE - Player {onELCFailure.Player.PlayerName} - Character {oPC.Name} - type : {onELCFailure.Type} - SubType : {onELCFailure.SubType}", LogUtils.LogType.PlayerConnections);
      }
    }
  }
}
