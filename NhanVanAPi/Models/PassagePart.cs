using System.Runtime.CompilerServices;

namespace NhanVanAPi.Models
{
    public class PassagePart
    {
        // this is not a table 
        public string Type { get; set; } = "text"; // "text" or "blank"
        public string? Content { get; set; }       // for text
        public int? QuestionNumber { get; set; }       // for blank: which question number
    }
}
