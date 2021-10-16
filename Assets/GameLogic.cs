using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private Text _enemyCountText;
    [SerializeField] private Image _enemyProgressBar;

    [SerializeField] private Button _warriorButton;
    [SerializeField] private Text _warriorCountText;
    [SerializeField] private Image _warriorProgressBar;
    
    [SerializeField] private Button _peasantButton;
    [SerializeField] private Text _peasantCountText;
    [SerializeField] private Image _peasantProgressBar;
    
    [SerializeField] private Text _wheetCountText;
    [SerializeField] private Image _farmProgressBar;
    
    [SerializeField] private float _waveInterval;
    [SerializeField] private float _farmInterval;
    [SerializeField] private float _peasantCooldown;
    [SerializeField] private float _warriorCooldown;


    [SerializeField] private int _startWheetValue;
    [SerializeField] private int _peasantCost;
    [SerializeField] private int _warriorCost;
    [SerializeField] private int _peasantValue;

    [SerializeField] private int _waveIncome;
    [SerializeField] private int _peasonWinCount;
    [SerializeField] private int _wheetWinCount;

    [SerializeField] private int _startWaveNumber;
        
    [SerializeField] private AudioSource _buttonPressedSound;
    [SerializeField] private AudioSource _buttonPressedErrorSound;
    [SerializeField] private AudioSource _farmSound;
    [SerializeField] private AudioSource _peasantSound;
    [SerializeField] private AudioSource _warriorSound;
    [SerializeField] private AudioSource _fightSound;

    [SerializeField] private Image _pauseButton;
    [SerializeField] private Image _muteButton;
    
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private Text _endHeaderText;
    [SerializeField] private Text _statisticsText;

    private Color _fullColor = new Color(255, 255, 255, 255);
    private Color _transparentColor = new Color(255, 255, 255, 100);
    
    private float _previousWaveTime;
    private float _previousFarmTime;
    
    private float _warriorTimer;
    private float _peasantTimer;

    private int _warriorsCount;
    private int _peasantCount;
    private int _enemyCount;
    private int _wheetCount;
    
    private int _waveCount;

    private bool _isMuted = false;
    private bool _isGameEnded;
    
    void Start()
    {
        _enemyCount = 0;
        _enemyCountText.text = _enemyCount.ToString();
        _previousWaveTime = 0f;
        
        _warriorsCount = 0;
        _warriorCountText.text = _warriorsCount.ToString();
        _warriorTimer = -1;
        _warriorProgressBar.fillAmount = 0;

        _peasantCount = 0;
        _peasantCountText.text = _peasantCount.ToString();
        _peasantTimer = -1;
        _peasantProgressBar.fillAmount = 0;

        _wheetCount = _startWheetValue;
        _wheetCountText.text = _wheetCount.ToString();
        
        _previousFarmTime = 0f;

        _waveCount = 0;
        _isGameEnded = false;
    }

    void Update()
    {
        if (_isGameEnded)
        {
            return;
        }
        
        float waveTimePassed = Time.time - _previousWaveTime;
        _enemyProgressBar.fillAmount = waveTimePassed / _waveInterval;
        if (waveTimePassed >= _waveInterval)
        {
            SpawnWave();
            _previousWaveTime = Time.time;
        }
        
        float farmTimePassed = Time.time - _previousFarmTime;
        _farmProgressBar.fillAmount = farmTimePassed / _farmInterval;
        if (farmTimePassed >= _farmInterval)
        {
            FarmWheet();
            _previousFarmTime = Time.time;
        }

        if (_warriorTimer > 0)
        {
            _warriorProgressBar.fillAmount = 1 - _warriorTimer / _warriorCooldown;
            _warriorTimer -= Time.deltaTime;

            if (_warriorTimer <= 0)
            {
                AddWarrior();
            }
        }
        
        if (_peasantTimer > 0)
        {
            _peasantProgressBar.fillAmount = 1 - _peasantTimer / _peasantCooldown;
            _peasantTimer -= Time.deltaTime;

            if (_peasantTimer <= 0)
            {
                AddPeasant();
            }
        }
    }

    public void OnAddWarriorPressed()
    {
        if (_wheetCount >= _warriorCost)
        {
            _buttonPressedSound.Play();
            ChangeWheetValue(-_warriorCost);
            _warriorTimer = _warriorCooldown;
            _warriorButton.interactable = false;
        }
        else
        {
            _buttonPressedErrorSound.Play();
        }
    }
    
    public void OnAddPeasantPressed()
    {
        if (_wheetCount >= _peasantCost)
        {
            _buttonPressedSound.Play();
            ChangeWheetValue(-_peasantCost);
            _peasantTimer = _peasantCooldown;
            _peasantButton.interactable = false;
        }
        else
        {
            _buttonPressedErrorSound.Play();
        }
    }

    public void OnPausePressed()
    {
        if (Time.timeScale == 0)
        {
            _pauseButton.color = _transparentColor;
            Time.timeScale = 1;
        }
        else
        {
            _pauseButton.color = _fullColor;
            Time.timeScale = 0;
        }
    }
    
    public void OnMutePressed()
    {
        if (_isMuted)
        {
            _buttonPressedSound.mute = false;
            _buttonPressedErrorSound.mute = false;
            _farmSound.mute = false;
            _peasantSound.mute = false;
            _warriorSound.mute = false;
            _fightSound.mute = false;
            
            _muteButton.color = _transparentColor;
            _isMuted = false;
        }
        else
        {
            _buttonPressedSound.mute = true;
            _buttonPressedErrorSound.mute = true;
            _farmSound.mute = true;
            _peasantSound.mute = true;
            _warriorSound.mute = true;
            _fightSound.mute = true;
            
            _muteButton.color = _fullColor;
            _isMuted = true;
        }
    }
    
    private void AddWarrior()
    {
        _warriorsCount += 1;
        _warriorCountText.text = _warriorsCount.ToString();
        _warriorTimer = -1;
        _warriorProgressBar.fillAmount = 0;
        _warriorButton.interactable = true;
        _warriorSound.Play();
    }
    
    private void AddPeasant()
    {
        _peasantCount += 1;
        _peasantCountText.text = _peasantCount.ToString();
        _peasantTimer = -1;
        _peasantProgressBar.fillAmount = 0;
        _peasantButton.interactable = true;
        _peasantSound.Play();
        if (_peasantCount >= _peasonWinCount)
        {
            OnWin();
        }
    }
    private void SpawnWave()
    {
        _waveCount++;
        if (_waveCount < _startWaveNumber)
        {
            return;
        }

        if (_enemyCount > 0)
        {
            _fightSound.Play();
        }
        
        if (_warriorsCount >= _enemyCount)
        {
            _warriorsCount -= _enemyCount;
            _enemyCount += _waveIncome;
            _enemyCountText.text = _enemyCount.ToString();
            _warriorCountText.text = _warriorsCount.ToString();
        }
        else
        {
            OnLoose();
        }
    }

    private void FarmWheet()
    {
        _farmSound.Play();
        ChangeWheetValue(_peasantCount * _peasantValue);
        if (_wheetCount >= _wheetWinCount)
        {
            OnWin();
        }
    }

    private void OnLoose()
    {
        ShowEndGamePanel("You loose!");
    }
    
    private void OnWin()
    {
        ShowEndGamePanel("You win!");
    }
    
    private void ShowEndGamePanel(string headerText)
    {
        OnPausePressed();
        _endHeaderText.text = headerText;
        _statisticsText.text = 
            $@"Waves passed: {_waveCount - _startWaveNumber}
Peasants: {_peasantCount}
Warriors: {_warriorsCount}
Wheet: {_wheetCount}";
        _endGamePanel.SetActive(true);
        _isGameEnded = true;
    }

    private void ChangeWheetValue(int delta)
    {
        _wheetCount += delta;
        _wheetCountText.text = _wheetCount.ToString();
    }
}
