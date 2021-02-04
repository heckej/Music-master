using MusicMasterBot.CognitiveModels;
using UserCommandLogic;

namespace MusicMasterBot
{
    public class Question
    {
        public string Text { get; set; }
        public string Artist { get; private set; }
        public string Title { get; private set; }
        public string Album { get; private set; }
        public UserQuestion.Intent Intent { get; set; }

        /// <summary>
        /// Updates properties according to values that are in the database.
        /// </summary>
        /// <example>If the question text is 'Who sings Someone Like You?', 
        /// then if the song Someone Like You is in the database, the Title 
        /// property will be set to 'Someone Like You'.</example>
        public void ResolveKnownProperties(SongChooser songChooser)
        {
            Artist = songChooser.GetKnownArtistFromSentence(Text);
            Title = songChooser.GetKnownSongTitleFromSentence(Text);
        }
    }
}
