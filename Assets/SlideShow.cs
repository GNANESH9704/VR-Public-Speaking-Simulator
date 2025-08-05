using UnityEngine;
using UnityEngine.UI;

public class SlideShow : MonoBehaviour
{
    public Sprite[] slides; // Array of slides
    public Image slideDisplay; // UI Image component to display slides
    private int currentIndex = 0; // Track current slide index

    void Start()
    {
        if (slideDisplay == null)
        {
            Debug.LogError("SlideDisplay is not assigned! Assign the Image component in the Inspector.");
            return;
        }

        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("Slide array is empty! Assign slides in the Inspector.");
            return;
        }

        slideDisplay.preserveAspect = true; // Ensure aspect ratio is maintained
        currentIndex = 0;
        UpdateSlide();
    }

    public void NextSlide()
    {
        if (slides == null || slides.Length == 0) return;

        currentIndex = (currentIndex + 1) % slides.Length;
        Debug.Log("Next Slide: " + currentIndex);
        UpdateSlide();
    }

    public void PreviousSlide()
    {
        if (slides == null || slides.Length == 0) return;

        currentIndex = (currentIndex - 1 + slides.Length) % slides.Length;
        Debug.Log("Previous Slide: " + currentIndex);
        UpdateSlide();
    }

    private void UpdateSlide()
    {
        if (slideDisplay != null && slides.Length > 0)
        {
            slideDisplay.sprite = slides[currentIndex];

            // Ensure the image is fully visible
            slideDisplay.color = new Color(1, 1, 1, 1);
        }
    }
}
