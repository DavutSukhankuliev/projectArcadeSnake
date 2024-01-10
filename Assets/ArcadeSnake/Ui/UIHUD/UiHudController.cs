using System;
using UnityEngine;

namespace ArcadeSnake
{
    public class UiHudController
    {
        private readonly AudioPlayerController _audioPlayerController;
        private readonly SnakeMovement _snakeMovement;
        
        private IUIService _uiService;
        private UiHud _uiHud;

        private int _score;
        private int _highScore;

        public UiHudController(
            IUIService uiService,
            AudioPlayerController audioPlayerController,
            SnakeMovement snakeMovement
        )
        {
            _uiService = uiService;
            _audioPlayerController = audioPlayerController;
            _snakeMovement = snakeMovement;

            if (PlayerPrefs.HasKey("SaveScore"))
            {
                _highScore = PlayerPrefs.GetInt("SaveScore");
            }
        }

        public void InitHud()
        {
            _uiHud = _uiService.Get<UiHud>();
            _uiHud.OnPauseClickEvent += OnClickPauseHandler;
            
            _uiHud.TextScore.text = $"{_uiHud.ScoreName} {_score}";
            _uiHud.TextHighscore.text = $"{_uiHud.HighscoreName} {_highScore}";
            _snakeMovement.OnUpdateScoreEvent  += OnUpdateScoreHandler;
        }

        private void OnClickPauseHandler(object sender, EventArgs e)
        {
            //Time.timeScale = 0;
            _snakeMovement.GameIsStart = false;
            
            _audioPlayerController.PauseGamePlayingMusic();
            _audioPlayerController.UnpauseGamePlayMusic();
            _audioPlayerController.SetVolumeGamePlayingMusicToHalf();
            
            _uiService.Show<UiPauseMenu>();
        }

        private void OnUpdateScoreHandler(object sender, int score)
        {
            if (score == 0)
            {
                _score = score;
            }
            else
            {
                _score += score;
            }
            _uiHud.TextScore.text = $"{_uiHud.ScoreName} {_score}";
            
            UpdateHighscore();
        }

        private void UpdateHighscore()
        {
            if (_score > _highScore)
            {
                _highScore = _score;
                PlayerPrefs.SetInt("SaveScore", _highScore);
                _uiHud.TextHighscore.text = $"{_uiHud.HighscoreName} {_highScore}";
            }
        }
    }
}