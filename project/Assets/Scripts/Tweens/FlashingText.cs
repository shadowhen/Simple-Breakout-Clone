using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FlashingText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Sequence _flashSequence;

    private void Start()
    {
        // Get the TextMeshPro's text component from the game object
        _text = GetComponent<TextMeshProUGUI>();
        
        // Create a DOTween sequence for the flashing
        _flashSequence = DOTween.Sequence();

        // We want to fade out the text first
        _flashSequence.Append(_text.DOFade(0, 1f));

        // We want to fade in the text to get the flash effect
        _flashSequence.Append(_text.DOFade(1f, 1f));

        // Set loop to infinity so the text can flash on and off
        _flashSequence.SetLoops(-1);
        
        // Prevent the tween from being killed at the end, so we can reuse this
        _flashSequence.SetAutoKill(false);

        // Tell the tween to play
        _flashSequence.Play();
    }

    // When the text becomes invisible, pause the flash tween
    private void OnDisable()
    {
        _flashSequence.Pause();
    }

    // When the text becomes visible, restart the tween
    private void OnEnable()
    {
        _flashSequence.Restart();        
    }

    // When the text is destroyed, kill the sequence to prevent any memory leaks
    private void OnDestroy()
    {
        _flashSequence.Kill();
    }
}
