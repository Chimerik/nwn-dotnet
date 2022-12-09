using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public Mail(string from, int fromCharacterId, string to, int toCharacterId, string title, string content, DateTime sentDate, DateTime? expirationDate = null, bool read = false)
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
    }
    public Mail()
    {

    }
    public async void SendMailToPlayer(string characterId)
    {
      var result = await SqLiteUtils.SelectQueryAsync("mails",
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
      }
    }
  }
}
