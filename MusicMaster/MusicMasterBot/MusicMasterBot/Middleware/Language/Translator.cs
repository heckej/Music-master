using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserCommandLogic;

namespace MusicMasterBot.Middleware.Language
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class Translator : IMiddleware
    {
        private readonly ISongChooser _songChooser;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceMiddleware"/> class.
        /// </summary>
        public Translator(UserState userState, ISongChooser songChooser)
        {
            _songChooser = songChooser;
        }

        /// <summary>
        /// Processes an incoming activity.
        /// </summary>
        /// <param name="turnContext">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // User says something.
                var text = turnContext.Activity.AsMessageActivity().Text;
                if (turnContext.Activity.AsMessageActivity().Locale == "nl")
                {
                    turnContext.Activity.AsMessageActivity().Text = TranslateFromDutch(text);
                }
            }

            turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
            {
                // Read messages sent to the user

                foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                {

                    if (turnContext.Activity.AsMessageActivity().Locale == "nl")
                    {
                        var text = currentActivity.AsMessageActivity().Speak;
                        currentActivity.AsMessageActivity().Speak = TranslateToDutch(text);
                    }

                }

                return await nextSend();
            });

            turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
            {
                // Read messages sent to the user
                if (activity.Type == ActivityTypes.Message)
                {

                }

                return await nextUpdate();
            });

            await next(cancellationToken).ConfigureAwait(false);
        }

        private static IDictionary<string, string> _dutchToEnglish = InitDutchToEnglish();

        private static IDictionary<string, string> InitDutchToEnglish()
        {
            return new Dictionary<string, string>()
            {
                {"artiest", "artist"},
                {"titel", "title"},
                {"wat dacht je van wat muziek", "what about some music"},
                {"wat muziek", "some music"},
                {"iets van muziek", "some music"},
                {"nummer", "song"},
                {"liedje", "song"},
                {"lied", "song"},
                {"muziek", "music"},
                {"speel", "play"},
                {"voor mij", "me"},
                {"voor me", "me"},
                {"eens", ""},
                {"iets van", "something by"},
                {"luider", "louder"},
                {"omhoog", "up"},
                {"hoger", "up"},
                {"stilte", "silence"},
                {"stil", "silent"},
                {"laat", "let"},
                {"laat me iets horen", "let me hear something"},
                {"van", "by"},
                {"de", "the"},
                {"het", "it"},
                {"wat", "what"},
                {"wie", "who"},
                {"dit", "this"},
                {"een", "a" },
                {"deze", "this" },
                {"pauze", "pause" },
                {"afspelen", "play" },
                {"spelen", "play" },
                {"hervatten", "resume" },
                {"volgende", "next" },
                {"volgend", "next" },
                {"vorige", "previous" },
                {"vorig", "previous" },
                {"voorgaand", "previous" },
                {"ken", "know" },
                {"niet", "not"},
                {"mijn", "my"},
                {"nooit", "never" },
                {"gehoord", "heard" },
                {"heb", "have" },
                {"iets", "anything" },
                {"doet me niet denken aan", "doesn't reming me of" },
                {"jammer genoeg", "unfortunately" },
                {"ik", "I" },
                {"één", "any" },
                {"genaamd", "called" },
                {"gemaakt", "made" },
                {"wanneer", "when" },
                {"jaar", "year" },
                {"welk", "which" }
            };
        }

        private string TranslateFromDutch(string text)
        {
            text = text.ToLower();
            foreach ((string dutchWord, string englishWord) in _dutchToEnglish)
            {
                if (text.Contains(dutchWord))
                    text = text.Replace(dutchWord, englishWord);
            }
            return text;
        }

        private static IDictionary<string, string> _englishToDutch = InitEnglishToDutch();

        private static IDictionary<string, string> InitEnglishToDutch()
        {
            return new Dictionary<string, string>()
            {
                {"This song is", "Dit nummer is" },
                {"This song was made by", "Dit nummer werd gemaakt door" },
                {"The current artist is", "De artiest van dit nummer is" },
                {"I don't know when this song has been made.", "Ik weet niet wanneer dit nummer gemaakt is." },
                {"I honestly wouldn't know.", "Ik zou het eerlijk gezegd niet weten." },
                {"To be honest, I have no idea.", "Om eerlijk te zijn: ik heb geen idee." },
                {"That's a big secret to me.", "Dat is een groot geheim voor mij." },
                {"I'd be really smart if I knew that.", "Ik zou pas echt slim zijn, moest ik dat weten." },
                {"I'm sorry. Apparently I don't know that much.", "Sorry, blijkbaar weet ik toch niet zoveel." },
                {"This is a song from", "Dit is een nummer uit" },
                {"I will now play", "Ik zal nu" },
                { "The next song is", "Het volgende nummer is"},
                {"The following song is", "Dit nummer is"},
                {", by", " door" },
                {"by", "spelen van"},
                {"I didn't understand that.", "Dat heb ik niet begrepen." },
                {"What do you mean?", "Wat bedoel je?" },
                {"Sorry, I didn't get that.", "Sorry, dat snap ik niet." },
                {"Sorry, I don't know what you mean.", "Sorry, ik weet niet wat je bedoelt." },
                {"Could you please repeat that?", "Kun je dat alsjeblieft herhalen?" },
                {"Could you rephrase that?", "Kun je dat anders zeggen?" },
                {"Maybe try to ask it differently?", "Vraag het anders eens op een andere manier." },
                {"I don't know what you're talking about.", "Ik heb geen flauw benul waarover je het hebt." },
                {"Who is the artist?", "Wie is de artiest?" },
                {"What is the name of the artist?", "Wie is de artiest?" },
                {"Which artist made the song?", "Welke artiest heeft het nummer gemaakt?" },
                {"Which artist do you want to hear?", "Welke artiest wil je horen?" },
                { "Who sings the song?", "Wie zingt het nummer?" },
                {"Which singer or band do you want to hear?", "Welke zanger of groep wil je horen?" },
                {"What is the title of the song?", "Wat is de titel van het nummer?" },
                {"Which song do you mean?", "Welk nummer bedoel je?" },
                {"What is the song called?", "Hoe heet het nummer?" },
                {"Which song do you want to hear?", "Welk nummer wil je horen?" },
                {"What is it called?", "Hoe heet het?"},
                {"What is the title?", "Wat is de titel?" },
                {"I thought you said", "Ik dacht dat je dit zei:" },
                {"I understood", "Ik verstond" },
                {"I thought I heard", "Ik heb dit gehoord:" },
                {"It sounded like", "Het klonk als" }
            };
        }

        private string TranslateToDutch(string text)
        {
            if (text is null)
                return text;
            var artist = _songChooser.GetKnownArtistFromSentence(text);
            if (artist != null && text.Contains(artist))
                text = text.Replace(artist, "{a}");
            var title = _songChooser.GetKnownSongTitleFromSentence(text);
            if (title != null && text.Contains(title))
                text = text.Replace(title, "{t}");
            /*Console.WriteLine("Text after artist/title replacement: " + text);*/

            var anyReplacements = false;
            foreach (var (englishPartOfSentence, dutchPartOfSentence) in _englishToDutch)
            {
                /*Console.WriteLine("\"" + text + "\" bevat \"" + englishPartOfSentence + "\": " + text.Contains(englishPartOfSentence));*/
                if (text.Contains(englishPartOfSentence))
                {
                    text = text.Replace(englishPartOfSentence, dutchPartOfSentence);
                    anyReplacements = true;
                    /*Console.WriteLine("Text after sentence replacement: " + text);*/
                }
            }

            if (!anyReplacements)
            {
                text = text.ToLower();
                foreach ((string dutchWord, string englishWord) in _dutchToEnglish)
                {
                    if (text.Contains(englishWord) && englishWord.Length > 0)
                        text = text.Replace(englishWord, dutchWord);
                }
            }
            text = text.Replace("{a}", artist);
            text = text.Replace("{t}", title);
            return text;
        }
    }
}
