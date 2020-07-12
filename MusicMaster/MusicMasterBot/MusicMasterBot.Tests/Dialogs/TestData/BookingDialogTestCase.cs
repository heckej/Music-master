// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MusicMasterBot;

namespace MusicMasterBot.Tests.Dialogs.TestData
{
    /// <summary>
    /// A class to store test case data for <see cref="BookingDialogTests"/>.
    /// </summary>
    public class BookingDialogTestCase
    {
        public string Name { get; set; }

        public SongRequest InitialBookingDetails { get; set; }

        public string[,] UtterancesAndReplies { get; set; }

        public SongRequest ExpectedBookingDetails { get; set; }
    }
}
