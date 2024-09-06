namespace VCard.Communication.Api.EmailSender;

internal sealed class EmailSenderConfiguration
{
    public const string SectionName = "EmailSender";
    public string SenderName { get; set; }
    public string ApiKey { get; set; }
}