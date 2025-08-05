using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System.Diagnostics;
using System.IO;

public class SpeechAnalyzer : MonoBehaviour
{
    public TMP_Text feedbackText; // Assign in Inspector
    public TMP_Text scoreText;    // Assign in Inspector

    private DictationRecognizer dictationRecognizer;
    private float speechStartTime;
    private string spokenText = "";
    private string audioFilePath = "speech.wav"; // Placeholder for recorded audio

    void Start()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnSpeechResult;
        dictationRecognizer.DictationComplete += OnSpeechComplete;
    }

    public void StartRecording()
    {
        speechStartTime = Time.time;
        spokenText = "";
        dictationRecognizer.Start();
        feedbackText.text = "Listening...";
    }

    public void StopRecording()
    {
        dictationRecognizer.Stop();
        UnityEngine.Debug.Log("Dictation Stopped. Final Speech: " + spokenText);
    }

    private void OnSpeechResult(string text, ConfidenceLevel confidence)
    {
        spokenText = text; // 🔹 Replace instead of appending
    }

    private void OnSpeechComplete(DictationCompletionCause cause)
    {
        float speechDuration = Time.time - speechStartTime;
        EvaluateSpeech(spokenText, speechDuration);
    }

    private void EvaluateSpeech(string speech, float duration)
    {
        if (string.IsNullOrWhiteSpace(speech))
        {
            feedbackText.text = "No speech detected. Please try again.";
            scoreText.text = "Score: 0.0/10";
            return;
        }

        UnityEngine.Debug.Log("Sending to AI Analysis: " + speech);

        int wordCount = speech.Split(' ').Length;
        float wordsPerMinute = (wordCount / duration) * 60;
        int fillerWords = CountFillerWords(speech);
        int pauses = CountPauses(speech);
        
        // Scoring System (0 to 10 Scale)
        float clarity = Mathf.Clamp((speech.Length / 50f) * 10, 0, 10);
        float confidence = Mathf.Clamp(((wordsPerMinute / 150f) * 10) - (fillerWords * 1.5f), 0, 10);
        float pace = Mathf.Clamp((1 - Mathf.Abs(wordsPerMinute - 135) / 50f) * 10, 0, 10);
        float fluency = Mathf.Clamp((10 - fillerWords * 2 - pauses * 2), 0, 10);
        float finalScore = (clarity + confidence + pace + fluency) / 4;
        
        // AI-based analysis
        GetAIAnalysis(speech, finalScore);
    }

    private int CountFillerWords(string speech)
    {
        string[] fillers = { "um", "uh", "like", "you know", "so", "actually" };
        int count = 0;
        foreach (var filler in fillers)
        {
            count += (speech.ToLower().Split(filler).Length - 1);
        }
        return count;
    }

    private int CountPauses(string speech)
    {
        return speech.Split(new string[] { "...", "—" }, System.StringSplitOptions.None).Length - 1;
    }

    private void GetAIAnalysis(string speech, float baseScore)
{
    string pythonPath = @"C:\Users\kanch\AppData\Local\Programs\Python\Python312\python.exe";
    string scriptPath = @"C:\Users\kanch\analyze_speech.py"; // Ensure this is correct

    if (!File.Exists(scriptPath))
    {
        UnityEngine.Debug.LogError("Python script not found: " + scriptPath);
        feedbackText.text = "AI Analysis Error: Script not found.";
        scoreText.text = "Score: " + baseScore.ToString("F1") + "/10";
        return;
    }

    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = pythonPath,
        Arguments = "\"" + scriptPath + "\" \"" + spokenText + "\"",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };

    Process process = Process.Start(psi);
    string output = process.StandardOutput.ReadToEnd();
    string errorOutput = process.StandardError.ReadToEnd();
    process.WaitForExit();

    if (!string.IsNullOrWhiteSpace(errorOutput))
    {
        UnityEngine.Debug.LogError("Python Error: " + errorOutput);
        feedbackText.text = "AI Analysis Error: " + errorOutput;
        scoreText.text = "Score: " + baseScore.ToString("F1") + "/10";
        return;
    }

    if (string.IsNullOrWhiteSpace(output))
    {
        UnityEngine.Debug.LogError("AI Analysis Error: No output from script.");
        feedbackText.text = "AI Analysis Error: No response from AI.";
        scoreText.text = "Score: " + baseScore.ToString("F1") + "/10";
        return;
    }

    UnityEngine.Debug.Log("AI Raw Output: " + output);

    // Extract AI score & feedback
    string[] lines = output.Split('\n');
    float aiScore = baseScore; // Default to base score
    string feedback = "";

    foreach (var line in lines)
    {
        if (float.TryParse(line.Trim(), out float extractedScore))
        {
            aiScore = extractedScore; // Extract numeric score
        }
        else
        {
            feedback += line + "\n"; // Collect text feedback
        }
    }

    // Average base and AI score
    float finalScore = Mathf.Clamp((baseScore + aiScore) / 2, 0, 10);

    feedbackText.text = feedback.Trim(); // Display AI feedback
    scoreText.text = "Score: " + finalScore.ToString("F1") + "/10";
}

}