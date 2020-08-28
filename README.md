# Music-master
Music maestro, it's time for a bot that serves your taste of music - let's hope.

Say something like 
> `>> Play Someone Like Me by Adele` and enjoy! 

> `>> Louder` if you don't hear it well, or 

> `>> Silent!` if you're tired for a moment.

> `>> What about some hard rock?`* 

or 

> `>> Do you know Let it be by The Beatles?`

It's all
> `<< Music Master, at your service!

_*Playing songs by genre has not been implemented (yet)._

## In short
This DJ bot is meant to be run as a server on the computer that should play the requested music (preferably with some speakers and a microphone connected to it).

## License
MIT, Copyright 2020 heckej

## Requirements
* A computer with some music and speakers
* A database filled with basic data about the songs (title, artist, album, year, path to file, genre)
* A LUIS model that recognises the intents that should be understood (e.g. _play a certain song_)

## Dependencies
* To compile and run the chatbot you need the .NET Core SDK.
* To play music (in Linux), you should install [cmus](https://cmus.github.io/) and add music to your library.
* Speech recognition (in Linux) needs `python 3` with the [SpeechRecognition](https://pypi.org/project/SpeechRecognition/) library installed.
* Text to speech (in Linux & Windows with modifications) needs `python 3` with the _Google-Text-to-Speech_ library [gTTS](https://pypi.org/project/gTTS/). For Windows, it is sufficient to have Powershell installed (you still might need to update the path in the code to where Powershell is located).

## Features
### Music Player (Linux only)
To play music the Music Master Bot calls the `cmus` command line music player using the `cmus-remote` commands. No plans for support of a Windows music player.

### Speech Client
The speech client allows a microphone to be used as input device for the chatbot. Communication with the chatbot is established by a custom adapter, more specifically a customised version of the [Bot Builder Community](https://github.com/BotBuilderCommunity/botbuilder-community-dotnet) [Alexa Adapter](https://github.com/BotBuilderCommunity/botbuilder-community-dotnet/tree/develop/libraries/Bot.Builder.Community.Adapters.Alexa) (MIT License).

#### Speech recognition (Linux only, maybe Windows with some modifications)
Speech recognition (in Linux, could work in Windows) is done in Python using the SpeechRecognition library. When the language of the spoken language is set to something different from English, English input will still be understood.
##### Multi-language support: English + Dutch
Requests sent to the custom adapter should contain a `Language` property. If the value is set to `nl` (Dutch), the chatbot will partially 'translate' the request to English and respond by saying the response out loud in Dutch to the user (currently not in the written text, because written and spoken text often contain two different values).
#### Text to speech (Windows & Linux)
The bot can respond by saying some things out loud using text to speech. It therefore uses the _Google-Text-to-Speech_ library in Linux (could work in Windows too) and built in tools in Powershell in Windows. Before sending the text to the TTS service, it is given to `fastText` (MIT License) to determine the language in which it's written. Based on the result, the bot can send varying language arguments to the TTS service, so the text (e.g. the title of a French song) is pronounced as ideally as possible.

## More details
### Python called from .NET Core
In the code of the bot there is a class that allows to execute commands in a separate process, with a function specifically for `python3` scripts, however it should be easy enough to change this to be used in Windows too.

### User command interpretation
To detect intents and entities a custom [LUIS](https://luis.ai) model is used.
