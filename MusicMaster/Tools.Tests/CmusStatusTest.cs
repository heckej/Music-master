using System;
using Xunit;

namespace Tools.Tests.Players.Status
{
    public class CmusStatusTest
    {
        [Fact]
        public void TestStatusParsing()
        {
            var statusString = System.IO.File.ReadAllText(@"D:\webdev\Music-master\MusicMaster\Tools.Tests\cmus_status.txt");
            var statusDictionary = CmusStatus.ParseCmusStatus(statusString);

            Assert.Equal("playing", statusDictionary["status"]);
            Assert.Equal("/home/jvh/Muziek/Alicia Keys/Girl On Fire/Karaoke Girl On Fire (Main Version) - Alicia Keys -.mp3", statusDictionary["file"]);
            Assert.Equal("241", statusDictionary["duration"]);
            Assert.Equal("183", statusDictionary["position"]);
            Assert.Equal("Alicia Keys", statusDictionary["artist"]);
            Assert.Equal("Girl On Fire", statusDictionary["album"]);
            Assert.Equal("Girl on fire karaoke", statusDictionary["title"]);
            Assert.Equal("2012", statusDictionary["date"]);
            Assert.Equal("Pop", statusDictionary["genre"]);
            Assert.Equal("album", statusDictionary["aaa_mode"]);
            Assert.Equal("true", statusDictionary["continue"]);
            Assert.Equal("true", statusDictionary["play_library"]);
            Assert.Equal("true", statusDictionary["play_sorted"]);
            Assert.Equal("false", statusDictionary["repeat"]);
            Assert.Equal("false", statusDictionary["repeat_current"]);
            Assert.Equal("true", statusDictionary["shuffle"]);
            Assert.Equal("70", statusDictionary["vol_left"]);
            Assert.Equal("70", statusDictionary["vol_right"]);
        }
    }
}
