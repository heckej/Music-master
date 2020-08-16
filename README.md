# Music-master
A bot that serves your taste of music (hopefully).

Say something like 'Play Someone Like Me by Adele' and enjoy! 'Louder' if you don't hear it well, or 'Silent!' if you're tired for a moment. 'What about some hard rock?' or 'Do you know Let it be by The Beatles?'.

## License
MIT, Copyright 2020 heckej

## Dependencies
* To play music (in Linux), you should install [cmus](https://cmus.github.io/) and add music to your library.
* Speech recognition (in Linux) needs `python 3` with the [SpeechRecognition](https://pypi.org/project/SpeechRecognition/) library installed.
* Text to speech (in Linux & Windows with modifications) needs `python 3` with the _Google-Text-to-Speech_ library [gTTS](https://pypi.org/project/gTTS/). For Windows, it is sufficient to have Powershell installed (you still might need to update the path in the code to where Powershell is located).

## Music Player (Linux only)
To play music the Music Master Bot calls the `cmus` command line music player using the `cmus-remote` commands. No plans for support of a Windows music player.

## Speech Client
The speech client allows a microphone to be used as input device for the chatbot. Communication with the chatbot is established by a custom adapter, more specifically a customised version of the [Bot Builder Community](https://github.com/BotBuilderCommunity/botbuilder-community-dotnet) [Alexa Adapter](https://github.com/BotBuilderCommunity/botbuilder-community-dotnet/tree/develop/libraries/Bot.Builder.Community.Adapters.Alexa) (MIT License).

### Speech recognition (Linux only, maybe Windows with some modifications)
Speech recognition (in Linux, could work in Windows) is done in Python using the SpeechRecognition library.

## Text to speech (Windows & Linux)
The bot can respond by saying some things out loud using text to speech. It therefore uses the _Google-Text-to-Speech_ library in Linux (could work in Windows too) and built in tools in Powershell in Windows.

## Python called from .NET Core
In the code of the bot there is a class that allows to execute commands in a separate process, with a function specifically for `python3` scripts, however it should be easy enough to change this to be used in Windows too.

## User command interpretation
To detect intents and entities a custom [LUIS](https://luis.ai) model is used.
