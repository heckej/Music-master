using System;
using System.Collections.Generic;

namespace UserCommandLogic
{
    public class Song
    {
        /// <summary>
        /// The title of the song described by this Song.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The artist of the song described by this Song.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// The album title of the song described by this Song.
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// The year in which the song described by this Song was published.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// A collection of genres describing the song described by this Song.
        /// </summary>
        public ISet<string> Genres { get; set; }

        /// <summary>
        /// The path to a media file that can play the song described by this Song.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The a collection of Songs that are very much alike the song described by this Song.
        /// </summary>
        public ISet<Song> BestMatches { get; set; }

        /// <summary>
        /// A variable describing whether the artist of the song described by this Song is in the database.
        /// </summary>
        [Obsolete("This property has never been used and is likely to be removed in the future.")]
        public bool ArtistInDatabase { get; set; }

        /// <summary>
        /// A variable describing whether the title of the song described by this Song is in the database.
        /// </summary>
        [Obsolete("This property has never been used and is likely to be removed in the future.")]
        public bool TitleInDatabase { get; set; }

        [Obsolete("This function has never been used and is likely to be removed in the future.")]
        public void UpdateByDatabase()
        {
            /// find the song in the database based on title, artist; if not found, note the best matches for artist and title with their best matches for the other parameter respectively
            throw new NotImplementedException();
        }
    }
}
