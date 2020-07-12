// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Bot.Builder.Testing.XUnit;

using MusicMasterBot;

namespace MusicMasterBot.Tests.Dialogs.TestData
{
    /// <summary>
    /// A class to generate test cases for <see cref="BookingDialogTests"/>.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Ignoring to make code more readable")]
    public class BookingDialogTestsDataGenerator
    {
        public static IEnumerable<object[]> BookingFlows()
        {
            yield return BuildTestCaseObject(
                "Full flow",
                new SongRequest(),
                new[,]
                {
                    { "hi", "Where would you like to travel to?" },
                    { "Seattle", "Where are you traveling from?" },
                    { "New York", "When would you like to travel?" },
                    { "tomorrow", $"Please confirm, I have you traveling to: Seattle from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No" },
                    { "yes", null },
                },
                new SongRequest
                {
                    Title = "Seattle",
                    Artist = "New York",
                    Album = $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                });

            yield return BuildTestCaseObject(
                "Full flow with 'no' at confirmation",
                new SongRequest(),
                new[,]
                {
                    { "hi", "Where would you like to travel to?" },
                    { "Seattle", "Where are you traveling from?" },
                    { "New York", "When would you like to travel?" },
                    { "tomorrow", $"Please confirm, I have you traveling to: Seattle from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No" },
                    { "no", null },
                },
                null);

            yield return BuildTestCaseObject(
                "Destination given",
                new SongRequest
                {
                    Title = "Bahamas",
                    Artist = null,
                    Album = null,
                },
                new[,]
                {
                    { "hi", "Where are you traveling from?" },
                    { "New York", "When would you like to travel?" },
                    { "tomorrow", $"Please confirm, I have you traveling to: Bahamas from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No" },
                    { "yes", null },
                },
                new SongRequest
                {
                    Title = "Bahamas",
                    Artist = "New York",
                    Album = $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                });

            yield return BuildTestCaseObject(
                "Destination and Origin given",
                new SongRequest
                {
                    Title = "Seattle",
                    Artist = "New York",
                    Album = null,
                },
                new[,]
                {
                    { "hi", "When would you like to travel?" },
                    { "tomorrow", $"Please confirm, I have you traveling to: Seattle from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No" },
                    { "yes", null },
                },
                new SongRequest
                {
                    Title = "Seattle",
                    Artist = "New York",
                    Album = $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                });

            yield return BuildTestCaseObject(
                "All booking details given for today",
                new SongRequest
                {
                    Title = "Seattle",
                    Artist = "Bahamas",
                    Album = $"{DateTime.Now:yyyy-MM-dd}",
                },
                new[,]
                {
                    { "hi", $"Please confirm, I have you traveling to: Seattle from: Bahamas on: {DateTime.Now:yyyy-MM-dd}. Is this correct? (1) Yes or (2) No" },
                    { "yes", null },
                },
                new SongRequest
                {
                    Title = "Seattle",
                    Artist = "Bahamas",
                    Album = $"{DateTime.Now:yyyy-MM-dd}",
                });
        }

        public static IEnumerable<object[]> CancelFlows()
        {
            yield return BuildTestCaseObject(
                "Cancel on origin prompt",
                new SongRequest(),
                new[,]
                {
                    { "hi", "Where would you like to travel to?" },
                    { "cancel", "Cancelling..." },
                },
                null);

            yield return BuildTestCaseObject(
                "Cancel on destination prompt",
                new SongRequest(),
                new[,]
                {
                    { "hi", "Where would you like to travel to?" },
                    { "Seattle", "Where are you traveling from?" },
                    { "cancel", "Cancelling..." },
                },
                null);

            yield return BuildTestCaseObject(
                "Cancel on date prompt",
                new SongRequest(),
                new[,]
                {
                    { "hi", "Where would you like to travel to?" },
                    { "Seattle", "Where are you traveling from?" },
                    { "New York", "When would you like to travel?" },
                    { "cancel", "Cancelling..." },
                },
                null);

            yield return BuildTestCaseObject(
                "Cancel on confirm prompt",
                new SongRequest(),
                new[,]
                {
                    { "hi", "Where would you like to travel to?" },
                    { "Seattle", "Where are you traveling from?" },
                    { "New York", "When would you like to travel?" },
                    { "tomorrow", $"Please confirm, I have you traveling to: Seattle from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No" },
                    { "cancel", "Cancelling..." },
                },
                null);
        }

        /// <summary>
        /// Wraps the test case data into a <see cref="TestDataObject"/>.
        /// </summary>
        private static object[] BuildTestCaseObject(string testCaseName, SongRequest inputBookingInfo, string[,] utterancesAndReplies, SongRequest expectedBookingInfo)
        {
            var testData = new BookingDialogTestCase()
            {
                Name = testCaseName,
                InitialBookingDetails = inputBookingInfo,
                UtterancesAndReplies = utterancesAndReplies,
                ExpectedBookingDetails = expectedBookingInfo,
            };
            return new object[] { new TestDataObject(testData) };
        }
    }
}
