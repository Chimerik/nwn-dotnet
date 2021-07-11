using NLog;

using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ELCSystem))]
  public class ELCSystem
  {
    /*public static readonly Logger Log = LogManager.GetCurrentClassLogger();
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
      Log.Info($"{onELCSuccess.Player.PlayerName} ELC check OK");

      int characterId = ObjectPlugin.GetInt(onELCSuccess.Player.ControlledCreature, "characterId");

      if (characterId > 0)
      {
        var result = SqLiteUtils.SelectQuery("playerCharacters",
        new List<string>() { { "areaTag" }, { "position" }, { "facing" } },
        new List<string[]>() { new string[] { "rowid", characterId.ToString() } });

        if (result.Result != null)
        {
          string tag = result.Result.GetString(0);

          if (tag.StartsWith("entrepotpersonnel"))
            AreaSystem.CreatePersonnalStorageArea(onELCSuccess.Player.ControlledCreature, characterId);

          onELCSuccess.Player.SpawnLocation = Utils.GetLocationFromDatabase(tag, result.Result.GetString(1), result.Result.GetFloat(2));
        }
      }
    }
    private void onELCValidationFailure(OnELCValidationFailure onELCFailure)
    {
      /*onELCFailure.on
      int characterId = ObjectPlugin.GetInt(callInfo.ObjectSelf, "characterId");
      if (characterId > 0)
      {
        ElcPlugin.SkipValidationFailure();
      }
      else
      {
        int validationFailureType = ElcPlugin.GetValidationFailureType();
        int validationFailureSubType = ElcPlugin.GetValidationFailureSubType();

        if (callInfo.ObjectSelf is NwCreature oPC)
        {

          if (validationFailureType == ElcPlugin.NWNX_ELC_VALIDATION_FAILURE_TYPE_CHARACTER && validationFailureSubType == 15 && oPC.GetAbilityScore(API.Constants.Ability.Intelligence, true) < 11)
            ElcPlugin.SkipValidationFailure();
          else
            Utils.LogMessageToDMs($"ELC VALIDATION FAILURE - Player {oPC.ControllingPlayer.PlayerName} - Character {oPC.Name} - type : {validationFailureType} - SubType : {validationFailureSubType}");
        }
      }
    }*/
  }
}
