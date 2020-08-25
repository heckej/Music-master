using System.Linq;

namespace MusicMasterBot.CognitiveModels
{
    // Extends the partial UserQuestion class with methods and properties that simplify accessing entities in the luis results
    public partial class UserQuestion
    {
        public string Negative
            => Entities.Negative?.FirstOrDefault();

        public string Positive
            => Entities.Positve?.FirstOrDefault();

        public string SongDescription
            => Entities.SongDescription?.FirstOrDefault()?.FirstOrDefault();

        public string[] SongInformation
            => Entities.SongInformation?.FirstOrDefault();
    }
}
