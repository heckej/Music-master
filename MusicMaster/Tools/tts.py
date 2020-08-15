from gtts import gTTS
import os
import sys

with file = open("tts.log", "a"):
    file.write("test")

print("hello world")
if __name__ == "__main__":
    print(str(sys.argv))
    tts = gTTS(text=sys.argv[1], lang='en')
    tts.save("speak.mp3")
    os.system("mpg321 speak.mp3")
