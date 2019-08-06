using System.ComponentModel;

namespace QbQuestionsAPI.Domain.Models
{
    public enum ETournamentLevel : byte
    {
        [Description("Middle School")]
        MiddleSchool = 1,

        [Description("High School")]
        HighSchool = 2,

        [Description("Collegiate")]
        Collegiate = 3,

        [Description("Trash")]
        Trash = 4
    }
}