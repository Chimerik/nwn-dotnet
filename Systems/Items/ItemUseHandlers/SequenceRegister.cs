using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  class SequenceRegister
  {
    NwCreature oCaster;
    NwItem oRegister;
    NwGameObject oTarget;
    PlayerSystem.Player player;
    public SequenceRegister(NwCreature oCaster, NwItem oRegister, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oCaster.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      this.oCaster = oCaster;
      this.oRegister = oRegister;
      this.oTarget = oTarget;
      this.player = player;

      player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;

      if (oRegister.GetLocalVariable<int>("_REGISTERED_SEQUENCE").HasNothing || oTarget == null)
      {
        DrawSequenceRegisterMainPage();

        Task waitMenuClose = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          player.oid.LoginCreature.GetLocalVariable<int>("_CURRENT_MENU_CLOSED").Delete();
          await NwTask.WaitUntil(() => player.oid.LoginCreature.GetLocalVariable<int>("_CURRENT_MENU_CLOSED").HasValue);
          player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
        });
      }
      else if(oTarget != null)
      {
        HandleCastSequence();
      }
    }
    private void DrawSequenceRegisterMainPage()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Bienvenue dans votre outil d'enregistrement de séquence de sorts",
        "Celui-ci fonctionne comme un raccourci unique pour l'utilisation répétée de sorts.",
        "Que souhaitez-vous faire ?"
      };

      player.menu.choices.Add(("Enregistrer une nouvelle séquence.", () => HandleNewSequenceSelected()));
      player.menu.choices.Add(("Modifier le nom de la séquence.", () => HandleGetSequenceName()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private async void HandleGetSequenceName()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez prononcer le nouveau nom à l'oral."
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        oRegister.Name = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"Votre séquence est désormais nommée {oRegister.Name.ColorString(new Color(32, 255, 32))}.");
        DrawSequenceRegisterMainPage();
      }
    }
    private void DrawSequenceList()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        "Pour enregistrer votre séquence de sorts, lancez simplement vos sorts les uns à la suite de autres.",
      };

      if (oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").HasValue)
      {
        player.menu.titleLines.Add("Liste des sorts enregistrés :");

        string[] spellList = oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").Value.Split("_");

        foreach (string spellId in spellList)
        {
          string spellName = Spells2da.spellsTable.GetSpellDataEntry((Spell)int.Parse(spellId)).name;
          player.menu.titleLines.Add(spellName);
        }
      }

      player.menu.choices.Add(("Valider l'enregistrement de la séquence.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void HandleNewSequenceSelected()
    {
      player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
      player.oid.LoginCreature.OnSpellAction += RegisterSpellSequence;
      oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").Delete();

      DrawSequenceList();
    }
    private void RegisterSpellSequence(OnSpellAction onSpellAction)
    {
      onSpellAction.PreventSpellCast = true;

      if (oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").HasNothing)
        oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").Value = ((int)onSpellAction.Spell).ToString();
      else
        oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").Value += $"_{((int)onSpellAction.Spell)}";

      DrawSequenceList();
    }
    private async void HandleCastSequence()
    {
      Task waitMovement = NwTask.Run(async () =>
      {
        float posX = oCaster.Position.X;
        float posY = oCaster.Position.Y;
        await NwTask.WaitUntil(() => oCaster.Position.X != posX || oCaster.Position.Y != posY);
        oCaster.GetLocalVariable<int>("_SEQUENCE_CANCELLED").Value = 1;
      });

      string[] spellList = oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").Value.Split("_");
      PlayerSystem.Log.Info($"list : {oRegister.GetLocalVariable<string>("_REGISTERED_SEQUENCE").Value}");
      foreach (string spellId in spellList)
      {
        if (oCaster == null || oTarget == null || oCaster.GetLocalVariable<int>("_SEQUENCE_CANCELLED").HasValue)
        {
          oCaster.GetLocalVariable<int>("_SEQUENCE_CANCELLED").Delete();
          return;
        }

        await oCaster.ActionCastSpellAt((Spell)int.Parse(spellId), oTarget);
        await NwTask.Delay(TimeSpan.FromSeconds(3));
      }
    }
  }
}
