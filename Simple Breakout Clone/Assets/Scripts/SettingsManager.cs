using System;
using UnityEngine;

public class SettingsManager
{
	// Ids for the PlayerPrefs
	private static string _musicOnPrefsId = "MusicOn";
	private static string _sfxOnPrefsId = "SFXOn";

	public SettingsManager()
	{
		// Load settings from the player preferences
		_musicOn = PlayerPrefs.GetInt(_musicOnPrefsId, 1) != 0;
		_sfxOn = PlayerPrefs.GetInt(_sfxOnPrefsId, 1) != 0;
	}

	private bool _musicOn = true;

	public bool MusicOn
	{
		get { return _musicOn; }
		set 
		{
            // Set the value and invoke event for any observers for listening
            _musicOn = value;
			MusicToggleEvent?.Invoke(value);
			
			// Updates the player preferences for music toggle
			// Note: I don't know how to set booleans in the PlayerPrefs, but using
			// integers should be fine for now
			PlayerPrefs.SetInt(_musicOnPrefsId, value ? 1 : 0);
		}
	}


	private bool _sfxOn = true;

	public bool SFXOn
	{
		get { return _sfxOn; }
		set 
		{ 
			// Set the value and invoke event for any observers for listening
			_sfxOn = value;
			SFXToggleEvent?.Invoke(value);

            // Updates the player preferences for SFX toggle
            PlayerPrefs.SetInt(_sfxOnPrefsId, value ? 1 : 0);
        }
	}

	// Events that potiential observers could be watching for
	public static Action<bool> MusicToggleEvent;
	public static Action<bool> SFXToggleEvent;
}
