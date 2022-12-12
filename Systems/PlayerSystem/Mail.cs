using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public class Mail
  {
    public readonly string from;
    public readonly int fromCharacterId;
    public readonly string to;
    public readonly int toCharacterId;
    public readonly string title;
    public readonly string content;
    public readonly DateTime sentDate;
    public readonly DateTime? expirationDate;
    public bool read;
    public bool receipt;

    public Mail(string from, int fromCharacterId, string to, int toCharacterId, string title, string content, DateTime sentDate, bool receipt = false, DateTime? expirationDate = null, bool read = false)
    {
      this.from = from;
      this.fromCharacterId = fromCharacterId;
      this.to = to;
      this.toCharacterId = toCharacterId;
      this.title = title;
      this.content = content;
      this.sentDate = sentDate;
      this.expirationDate = expirationDate;
      this.read = read;
      this.receipt = receipt;
    }
    public Mail()
    {

    }
    public Mail(SerializableMail serializedMail)
    {
      from = serializedMail.from;
      fromCharacterId = serializedMail.fromCharacterId;
      to = serializedMail.to;
      title = serializedMail.title;
      content = serializedMail.content;
      expirationDate = serializedMail.expirationDate;
      sentDate = serializedMail.sentDate;
      read = serializedMail.read;
      receipt = serializedMail.receipt;
    }

    public class SerializableMail
    {
      public int fromCharacterId { get; set; }
      public string from { get; set; }
      public int toCharacterId { get; set; }
      public string to { get; set; }
      public string title { get; set; }
      public string content { get; set; }
      public DateTime sentDate { get; set; }
      public DateTime? expirationDate { get; set; }
      public bool read { get; set; }
      public bool receipt { get; set; }

      public SerializableMail()
      {

      }
      public SerializableMail(Mail mailBase)
      {
        fromCharacterId = mailBase.fromCharacterId;
        from = mailBase.from;
        toCharacterId = mailBase.toCharacterId;
        to = mailBase.to;
        title = mailBase.title;
        content = mailBase.content;
        sentDate = mailBase.sentDate;
        expirationDate = mailBase.expirationDate;
        read = mailBase.read;
        receipt = mailBase.receipt;
      }
    }
    public void SendMailToPlayer(int characterId)
    {
      PlayerSystem.Player targetPlayer = PlayerSystem.Players.FirstOrDefault(p => p.Value.characterId == characterId).Value;

      if (targetPlayer == null || targetPlayer.pcState == PlayerSystem.Player.PcState.Offline)
        SendMailToDataBase(characterId.ToString());
      else
        SendMailNotification(targetPlayer);

    }
    private async void SendMailToDataBase(string characterId)
    {
      var result = await SqLiteUtils.SelectQueryAsync("playerCharacters",
        new List<string>() { { "mails" } },
        new List<string[]>() { new string[] { "ROWID", characterId } });

      if (result != null && result.Count > 0)
      {
        string serializedRequests = result.FirstOrDefault()[0];

        Task loadMails = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedRequests))
            return;

          List<Mail> mailRequest = JsonConvert.DeserializeObject<List<Mail>>(serializedRequests);
          mailRequest.Add(this);

          SqLiteUtils.UpdateQuery("playerCharacters",
            new List<string[]>() { new string[] { "mails", JsonConvert.SerializeObject(mailRequest) } },
            new List<string[]>() { new string[] { "rowid", characterId } });
        });
      }
      else
      {
        List<Mail> mailRequest = new() { this };
        SqLiteUtils.UpdateQuery("playerCharacters",
            new List<string[]>() { new string[] { "mails", JsonConvert.SerializeObject(mailRequest) } },
            new List<string[]>() { new string[] { "rowid", characterId } });
      }
    }
    private void SendMailNotification(PlayerSystem.Player player)
    {
      player.mails.Add(this);

      if (!player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailNotification))
        return;

      if (fromCharacterId < 1)
        player.oid.SendServerMessage("Votre pièce vibre. Vous venez de recevoir une nouvelle missive de la banque", ColorConstants.Orange);
      else
        player.oid.SendServerMessage("Votre pièce vibre. Vous venez de recevoir une nouvelle missive personnelle", ColorConstants.Orange);

      if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1)
        player.bankGold -= player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value * 5;
    }
    public void SendReceiptToPlayer()
    {
      new Mail("Banque Skalsgard", -1, from, fromCharacterId, "Accusé de lecture", 
        $"Très honoré client,\n\n Nous sommes ravis de vous annoncer que votre missive du {sentDate:dd/MM/YYYY} à destination de {to} a bien été reçue et lue par son destinataire.\n\nTitre de la missive : {title}",
        DateTime.Now, false, DateTime.Now.AddMonths(3)).SendMailToPlayer(fromCharacterId);
    }
  }
}
