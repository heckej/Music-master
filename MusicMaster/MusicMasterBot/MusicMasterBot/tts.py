from gtts import gTTS
import os
import sys

if __name__ == "__main__":
    print(str(sys.argv))
    text = ' '.join(sys.argv[1:])
    if text != '':
        tts = gTTS(text=text, lang='en')
        tts.save("speak.mp3")
        os.system("mpg321 speak.mp3")
