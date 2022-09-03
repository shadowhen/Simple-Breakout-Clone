using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    // For those who don't know, SerializeField is better way
    // to make variable accessible in the editor without having
    // to make the variables public in code.
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private GameObject _playerPrefab;

    // Since Unity 2021.3, the Unity uses Text - TextMeshPro in the UI dropdown.
    // However, you can still use old Text from Unity UI if
    // you go through dropdown for legacy.
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _ballText;
    [SerializeField] private TextMeshProUGUI _levelText;
    
    // It is disabled because we are going to use C# events to update the highscore text
    //[SerializeField] private TextMeshProUGUI _highscoreText;

    // UI panels are techincally game objects, so we could get away having game objects as the type
    [SerializeField] private GameObject _panelMenu;
    [SerializeField] private GameObject _panelPlay;
    [SerializeField] private GameObject _panelLevelCompleted;
    [SerializeField] private GameObject _panelGameOver;
    [SerializeField] private GameObject _panelPause;

    // These levels provide convinent access in the editor to add or remove levels
    [SerializeField] private GameObject[] _levels;

    // This is an example of singleton. Honestly, I would try to avoid singletons if possible.
    // However, the tutorial decide to use singletons, so I just follow through anyway.
    // For more information about singletons, look at Game Programming Patterns book on the topic:
    // https://gameprogrammingpatterns.com/singleton.html
    public static GameManager Instance { get; private set; }
    
    private GameObject _currentBall;
    private GameObject _currentLevel;
    private GameObject _player;
    private bool _isSwitchingState;

    #region Properties

    private int _highscore;
    public int Highscore { get { return _highscore; } }

    // For those who are using Visual Studio, there is a shortcut when writing properties
    // Just type 'propfull' and hit tab twice to get a full property.
    private int _score;
    public int Score
    {
        get { return _score; }
        set 
        { 
            _score = value;

            // Instead of using '_scoreText.text = "Score: " + score',
            // SetText function allows to pass arguments to the string format
            //
            // The ? operator used in this scenario is equivalent to:
            // if (_scoreText != null)
            // {
            //     _scoreText.SetText("Score: {0}", _score);
            // }
            _scoreText?.SetText("SCORE: {0}", _score);
        }
    }

    private int _level;
    public int Level
    {
        get { return _level; }
        set 
        { 
            _level = value;
            _levelText?.SetText("LEVEL: {0}", _level + 1);
        }
    }

    private int _balls;
    public int Balls
    {
        get { return _balls; }
        set 
        {
            _balls = value;
            _ballText?.SetText("BALLS: {0}", _balls);
        }
    }

    private bool _isPaused;
    public bool IsPaused
    {
        get { return _isPaused; }
    }

    #endregion

    #region Managers

    // The sound manager would be added using the Unity Editor; however,
    // the sound manager itself should be accessed by other scripts
    // through the game manager
    [SerializeField] private SoundManager _soundManager;
    public SoundManager SoundManager { get => _soundManager; }

    private SettingsManager _settingsManager;
    public SettingsManager SettingsManager { get => _settingsManager;  }

    #endregion

    // To be honest, this is techincally an implementation of a state machine
    // For more information: https://gameprogrammingpatterns.com/state.html
    public enum State { MENU, INIT, PLAY, LEVEL_COMPLETED, LOAD_LEVEL, GAME_OVER, PAUSE };
    private State _state;

    // Events
    public event Action<State> StateChangedEvent;
    public event Action<int> HighscoreUpdateEvent;

    void Awake()
    {
        // This will ensure that the instance is set before the Start call occurs
        Instance = this;

        // Create the settings manager that can be accessed by other scripts through game manager
        _settingsManager = new SettingsManager();
    }

    void Start()
    {
        // This would ensure that there can only be one GameManager in code.
        //Instance = this;

        // Start with the menu first
        SwitchState(State.MENU);
    }

    public void SwitchState(State newState, float delay = 0)
    {
        // Start switch delay coroutine in case that we need to delay
        StartCoroutine(SwitchDelay(newState, delay));
    }

    private IEnumerator SwitchDelay(State newState, float delay)
    {
        // Since we are in a coroutine, switching state should be true,
        // so other parts of the game can synchronize with the state
        _isSwitchingState = true;
        
        // Wait for certaim amount of time based on the delay given
        yield return new WaitForSeconds(delay);

        // End current state first
        EndState();

        // Go to new state and begin in that state
        _state = newState; 
        BeginState(newState);
        
        // We are done with switching state, so switching state should be false
        _isSwitchingState = false;

        // Invoke the event since we are done with switching state
        StateChangedEvent?.Invoke(newState);
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MENU:
                // Cursor should be visible since the player would interact the UI
                Cursor.visible = true;

                // Disabled since we are not going to use presistent state for highscore
                //_highscoreText.SetText("HIGHSCORE: {0}", PlayerPrefs.GetInt("highscore");

                //_highscoreText.SetText("HIGHSCORE: {0}", _highscore);
                
                // Invoke highscore update event in case if any game object needs the highscore now
                HighscoreUpdateEvent?.Invoke(_highscore);

                // Make the menu visible
                _panelMenu.SetActive(true);
                break;
            case State.INIT:
                // Cursor should not be visible since the player would be controlling the paddle
                Cursor.visible = false;

                // Make the play interface visible for the player
                _panelPlay.SetActive(true);

                Score = 0; // Every game starts off with zero
                Level = 0; // Index starts at 0 for accessing levels
                Balls = 3; // Could be set by variable instead of a constant if we need to change it at any point in the editor

                // Cleanup anything on the screen already
                if (_currentLevel != null)
                    Destroy(_currentLevel);
                if (_currentBall != null)
                    Destroy(_currentBall);
                if (_player != null)
                    Destroy(_player);

                // Create new player every time the game initalizes
                _player = Instantiate(_playerPrefab);

                // After initalization, load a new level by switching to load level state
                SwitchState(State.LOAD_LEVEL);
                break;
            case State.PLAY:
                break;
            case State.LEVEL_COMPLETED:
                // Get rid of the level and the ball so we can load the next level
                Destroy(_currentBall);
                Destroy(_currentLevel);
                
                // Increment the level count since we access the levels using index
                Level++;

                // Show the player that the level is complete
                _panelLevelCompleted.SetActive(true);

                // Load a level with a delay, so the level complete won't disappear immediately
                SwitchState(State.LOAD_LEVEL, 2f);
                break;
            case State.LOAD_LEVEL:
                // Load new level if there any levels to go through.
                // Otherwise, end the game
                if (Level >= _levels.Length)
                {
                    SwitchState(State.GAME_OVER);
                }
                else
                {
                    _currentLevel = Instantiate(_levels[Level]);
                    SwitchState(State.PLAY);
                }
                break;
            case State.GAME_OVER:
                // Disabled since we are not going to store highscore in persistent state
                /*
                if (Score > PlayerPrefs.GetInt("highscore"))
                {
                    PlayerPrefs.SetInt("highscore", Score);
                }
                */

                // Try to record the current score
                RecordHighScore();

                // Make the panel visible to the player
                _panelGameOver.SetActive(true);
                break;
            case State.PAUSE:
                // Make the cursor visible so the player can interact with the interface
                Cursor.visible = true;
                
                // "Pauses" the game since Update call can still happen even if time scale is zero
                _isPaused = true;
                Time.timeScale = 0f;

                // Make the panel visible to the player
                _panelPause.SetActive(true);
                break;
        }
    }

    void Update()
    {
        switch (_state)
        {
            case State.MENU:
                break;
            case State.INIT:
                break;
            case State.PLAY:
                // Should the ball not exist, generate new ball or end the game
                // depending if the player has any balls/lives left
                if (_currentBall == null)
                {
                    if (Balls > 0)
                    {
                        _currentBall = Instantiate(_ballPrefab);
                    }
                    else
                    {
                        SwitchState(State.GAME_OVER);
                    }
                }

                // Should the level be clear and not switching states, load a new level.
                // The reason for checking not switching states is to prevent unnecessary
                // coroutine calls for switching states.
                if (_currentLevel != null && _currentLevel.transform.childCount == 0 && !_isSwitchingState)
                {
                    SwitchState(State.LEVEL_COMPLETED);
                }

                // The player presses the escape key to pause the game
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                }

                break;
            case State.LEVEL_COMPLETED:
                break;
            case State.LOAD_LEVEL:
                break;
            case State.GAME_OVER:
                // The player should press any key to go to the menu
                if (Input.anyKeyDown)
                {
                    SwitchState(State.MENU);
                }
                break;
            case State.PAUSE:
                // The player would press the escape key to unpause the game
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ResumeGame();
                }
                break;
        }
    }

    void EndState()
    {
        switch (_state)
        {
            case State.MENU:
                _panelMenu.SetActive(false);
                break;
            case State.INIT:
                break;
            case State.PLAY:
                break;
            case State.LEVEL_COMPLETED:
                _panelLevelCompleted.SetActive(false);
                break;
            case State.LOAD_LEVEL:
                break;
            case State.GAME_OVER:
                _panelPlay.SetActive(false);
                _panelGameOver.SetActive(false);
                break;
            case State.PAUSE:
                Cursor.visible = false;
                _isPaused = false;
                Time.timeScale = 1f;

                _panelPause.SetActive(false);
                break;
        }
    }

    public void PlayClick()
    {
        SwitchState(State.INIT);
    }

    public void ResumeGame()
    {
        // Don't any code below if the game state is not paused
        if (_state != State.PAUSE)
            return;

        // Go back to play state
        SwitchState(State.PLAY);
    }

    public void PauseGame()
    {
        // Don't pause the game if the player is not playing the game
        if (_state != State.PLAY)
            return;
        SwitchState(State.PAUSE);
    }

    public void RestartGame()
    {
        // Record the highscore first in case the player has a higher score than
        // the current highscore
        RecordHighScore();

        // Switch to the intial game state to load the first level
        SwitchState(State.INIT);
    }

    private void RecordHighScore()
    {
        // Find the best score between the current score and current high scroe
        _highscore = Mathf.Max(_highscore, Score);

        // Invoke event for updating high score
        HighscoreUpdateEvent?.Invoke(_highscore);
    }
}