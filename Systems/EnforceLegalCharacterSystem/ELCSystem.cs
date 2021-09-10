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
    public ELCSystem(EnforceLegalCharacterService elcService)
    {
      this.elcService = elcService;
      elcService.EnforceDefaultEventScripts = true;
      elcService.EnforceEmptyDialog = true;
      elcService.OnValidationFailure += onELCValidationFailure;
      elcService.OnValidationSuccess += onELCValidationSuccess;
    }
    private void onELCValidationSuccess(OnELCValidationSuccess onELCSuccess)
    {
      int characterId = onELCSuccess.Player.ControlledCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value;

      if (characterId > 0)
      {
        var result = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "location" } },
        new List<string[]>() { new string[] { "rowid", characterId.ToString() } });

        if (result.Result != null)
        {
          Location spawnLoc = SqLiteUtils.DeserializeLocation(result.Result.GetString(0));

          if (spawnLoc.Area.Tag.StartsWith("entrepotpersonnel"))
            AreaSystem.CreatePersonnalStorageArea(onELCSuccess.Player.ControlledCreature, characterId);

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
      }
      else
      {
        if (onELCFailure.Type == ValidationFailureType.Character && onELCFailure.SubType == ValidationFailureSubType.ClassSpellcasterInvalidPrimaryStat && oPC.GetAbilityScore(Ability.Intelligence, true) < 11)
          onELCFailure.IgnoreFailure = true;
        else
        {
          string failureMessage = $"ELC VALIDATION FAILURE - Player {onELCFailure.Player.PlayerName} - Character {oPC.Name} - type : {onELCFailure.Type} - SubType : {onELCFailure.SubType}";
          Log.Info(failureMessage);
          Utils.LogMessageToDMs(failureMessage);
        }
      }
    }
  }
}
