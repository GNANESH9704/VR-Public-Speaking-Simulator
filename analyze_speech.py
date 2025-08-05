import whisper
import spacy

# Load the AI models
nlp = spacy.load("en_core_web_sm")
model = whisper.load_model("small")

# Function to analyze speech
def analyze_speech(audio_file):
    result = model.transcribe(audio_file)  # Convert speech to text
    text = result["text"]

    # List of common filler words
    filler_words = ["um", "uh", "like", "you know", "so", "actually"]
    doc = nlp(text)

    # Count how many filler words are used
    fillers_count = sum(1 for token in doc if token.text.lower() in filler_words)

    return f"Filler words: {fillers_count}, Speech: {text}"

print(analyze_speech("speech.mp3"))  # Test with an audio file
