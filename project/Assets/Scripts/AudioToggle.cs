using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    private Toggle _toggle;

    [SerializeField] private AudioMixer _output;
    [SerializeField] private string _name;
    [SerializeField] private bool _reverseToggle;

    // Determines whether the toggle is meant for music or sound effects
    [SerializeField] private bool _isMusic;

    private void Start()
    {
        // Get toggle component so we can access whether the toggle is on or not
        _toggle = GetComponent<Toggle>();

        if (_isMusic)
        {
            // Get boolean for the music is on or off
            _toggle.isOn = GameManager.Instance.SettingsManager.MusicOn;
            
            // Reverse the toggle on if the reverse toggle is enabled
            if (_reverseToggle)
                _toggle.isOn = !_toggle.isOn;
        }
        else
        {
            // Get boolean for SFX is on or off
            _toggle.isOn = GameManager.Instance.SettingsManager.SFXOn;

            // Reverse the toggle on if the reverse toggle is enabled
            if (_reverseToggle)
                _toggle.isOn = !_toggle.isOn;
        }
    }

    public void Toggle()
    {
        // Should the reverse toggle be enabled, the toggle on would be filped in this case
        // Otherwise, toggle on behavior would remain normal
        bool toggleOn = _reverseToggle != _toggle.isOn;

        // Unless someone knows how to actually mute channels in code,
        // this solution would be a way to mute a channel.
        //
        // Note: The problem with this solution means that if we decide to
        // have custom volume value and want to "mute" the channel, we
        // need to store the custom volume value and restore it when
        // the channel is "not mute".
        _output.SetFloat(_name, toggleOn ? 0.0f : -80.0f);

        if (_isMusic)
        {
            GameManager.Instance.SettingsManager.MusicOn = toggleOn;
        }
        else
        {
            GameManager.Instance.SettingsManager.SFXOn = toggleOn;
        }
    }
}
