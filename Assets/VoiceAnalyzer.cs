using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoiceAnalyzer : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isRecording = false;
    public TMP_Text feedbackText; // Assign in Inspector
    public Button startButton;
    public Button stopButton;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Assign button events
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
    }

    void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            isRecording = true;
            audioSource.clip = Microphone.Start(null, true, 10, 44100);
            audioSource.loop = false;
            audioSource.Play();

            feedbackText.text = "Recording... Speak now!";
            Debug.Log("Recording started.");
        }
        else
        {
            feedbackText.text = "No microphone detected!";
            Debug.LogError("No microphone detected!");
        }
    }

    void StopRecording()
    {
        if (!isRecording) 
        {
            Debug.Log("StopRecording called, but recording was not active.");
            return;
        }

        isRecording = false;
        Microphone.End(null);
        audioSource.Stop();

        // Analyze loudness
        float loudness = AnalyzeAudio();
        string confidenceFeedback = loudness < 0.1f ? "Try to speak louder for confidence!" : "Great confidence!";
        
        // Display feedback
        feedbackText.text = "Recording stopped.\n" + confidenceFeedback;
        Debug.Log("Recording stopped. Feedback: " + confidenceFeedback);
    }

    float AnalyzeAudio()
    {
        if (audioSource.clip == null)
        {
            Debug.LogError("No audio clip found for analysis!");
            return 0;
        }

        float[] samples = new float[audioSource.clip.samples];
        audioSource.clip.GetData(samples, 0);

        float sum = 0;
        foreach (var sample in samples)
        {
            sum += Mathf.Abs(sample);
        }
        return sum / samples.Length; // Average loudness
    }
}
