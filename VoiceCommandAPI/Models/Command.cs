namespace VoiceCommandAPI.Models
{
    public class Command
    {
        public int? Id { get; set; }
        public string CommandText { get; set; } = string.Empty;
        public DateTime? RecognizedAt { get; set; }
    }
}