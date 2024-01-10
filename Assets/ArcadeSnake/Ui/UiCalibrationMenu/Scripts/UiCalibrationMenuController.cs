using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class UiCalibrationMenuController : ITickable
    {
        private IUIService _uiService;
        private TickableManager _tickableManager;
        private readonly AudioPlayerController _audioPlayerController;
        private UiCalibrationMenu _calibrationMenu;
        private MiographInputHandler _inputHandler;
        private MiographInputConfig _miographInputConfig;

        private int _counter;
        private float _maxOfAttempt;

        private int _markCounter;

        private float _leftHandData;
        private float _l1;
        private float _l2;
        private float _l3;
        private float _lAverage;
        
        private float _rightHandData;
        private float _r1;
        private float _r2;
        private float _r3;
        private float _rAverage;


        private bool _calibrationStep;
        private bool _doneStep;
        
        private float _timerStep = 3f;
        private float _doneTimer = 1f;

        public UiCalibrationMenuController(
            IUIService uiService, 
            MiographInputHandler inputHandler,
            MiographInputConfig miographInputConfig,
            TickableManager tickableManager,
            AudioPlayerController audioPlayerController
        )
        {
            _uiService = uiService;
            _inputHandler = inputHandler;
            _miographInputConfig = miographInputConfig;
            _tickableManager = tickableManager;
            _audioPlayerController = audioPlayerController;

            _tickableManager.Add(this);
        }
        private void LeftHandler(object sender, float handData)
        {
            _leftHandData = handData;
            LeftHandPower(_leftHandData);
        }
        private void RightHandler(object sender, float handData)
        {
            _rightHandData = handData;
            RightHandPower(_rightHandData);
        }
        
        public void InitCalibrationMenu()
        {
            _calibrationMenu = _uiService.Get<UiCalibrationMenu>();
            
            _calibrationMenu.OnStartCalibrationClickEvent += OnStartCalibrationClickHandler;
            _calibrationMenu.OnMainMenuClickEvent += OnMainMenuClickHandler;
            _calibrationMenu.OnYesClickEvent += OnYesClickHandler;
            _calibrationMenu.OnNoClickEvent += OnNoClickHandler;
            _calibrationMenu.TextInstruction.text = $"You can calibrate miograph for you own";
            
            _inputHandler.RightHandEvent += RightHandler;
            _inputHandler.LeftHandEvent += LeftHandler;
        }

        private void OnStartCalibrationClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");
            _calibrationMenu.WindowHide(_calibrationMenu.WindowStartCalibration);
            StartCalibrationNew();
        }

        private void StartCalibrationNew()
        {
            switch (_counter)
            {
                case 0:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Strain your left hand";
                    _calibrationStep = true;
                    break;
                
                case 1:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Done!";
                    _doneStep = true;
                    break;
                
                case 2:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Strain your left hand 2nd time";
                    _calibrationStep = true;
                    break;
                
                case 3:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Done!";
                    _doneStep = true;
                    break;
                
                case 4:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Strain your left hand 3rd time";
                    _calibrationStep = true;
                   break;
                
                case 5:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Done!";
                    _doneStep = true;
                    break;
                
                 case 6:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Strain your right hand";
                    _calibrationStep = true;
                   break;
                 
                case 7:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Done!";
                    _doneStep = true;
                    break;
                
                case 8:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Strain your right hand 2nd time";
                    _calibrationStep = true;
                    break;
                
                case 9:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Done!";
                    _doneStep = true;
                    break;
                
                case 10:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Strain your right hand 3rd time";
                    _calibrationStep = true;
                    break;
                
                case 11:
                    _calibrationMenu.TextInstruction.text =
                        $"Calibration processing{Environment.NewLine}Done!";
                    _counter = 0;

                    SetupNewCalibration();
                    break;
                
                default:
                    _calibrationMenu.TextInstruction.text = _calibrationMenu.TextInstruction.text;
                    break;
            }
            
        }
        public void Tick()
        {
           
            StepTimer();

          
            DoneTimer();
        }

        private void OnMainMenuClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");
            _audioPlayerController.SetVolumeMainMenuMusic(0);
            
            _uiService.Hide<UiCalibrationMenu>();
            _uiService.Show<UiMainMenu>();
            
            _inputHandler.RightHandEvent -= RightHandler;
            _inputHandler.LeftHandEvent -= LeftHandler;
        }
        public void OnYesClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");
            
            UnsetMark();
            
            _calibrationMenu.WindowHide(_calibrationMenu.WindowSaveNewCalibration);
            _calibrationMenu.WindowShow(_calibrationMenu.WindowStartCalibration);

            _lAverage = (_l1 + _l2 + _l3) / 3;
            _rAverage = (_r1 + _r2 + _r3) / 3;
           
            _miographInputConfig.inputModels[0].Sensitivity = _lAverage;
            _miographInputConfig.inputModels[1].Sensitivity = _rAverage;

            _calibrationMenu.TextInstruction.text = $"Calibration completed";
        }
        private void OnNoClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");

            UnsetMark();
            
            _calibrationMenu.WindowHide(_calibrationMenu.WindowSaveNewCalibration);
            _calibrationMenu.WindowShow(_calibrationMenu.WindowStartCalibration);
            _calibrationMenu.TextInstruction.text = $"Calibration canceled{Environment.NewLine}Start new calibration";
        }
       
        public void LeftHandPower(float value)
        {
            _leftHandData = (float)Math.Round(value, 2); 
            _calibrationMenu.TextHandLeft.text = $"{_leftHandData}";
            _calibrationMenu.ImageHandLeft.fillAmount = _leftHandData;
        }
        public void RightHandPower(float value)
        {
            _rightHandData = (float)Math.Round(value, 2);
            _calibrationMenu.TextHandRight.text = $"{_rightHandData}";
            _calibrationMenu.ImageHandRight.fillAmount = _rightHandData;
        }

        private void SetupNewCalibration()
        {
            _calibrationMenu.WindowShow(_calibrationMenu.WindowSaveNewCalibration);
            _calibrationMenu.TextInstruction.text = $"Apply a new calibration?";
        }
        
        public void SetMark(int i)
        {
            _calibrationMenu.Marks[i].isOn = true;
        }

        public void UnsetMark()
        {
            for (int i = 0; i < _markCounter; i++)
            {
                _calibrationMenu.Marks[i].isOn = false;
            }
            _markCounter = 0;
        }
        
         private void DoneTimer() 
        {
            if (_doneStep) 
            {
                _doneTimer -= Time.deltaTime;
                if (_doneTimer <= 0) {
                    _counter++;

                    _doneTimer = 1f;
                    _doneStep = false;
                    _maxOfAttempt = 0;
                    StartCalibrationNew();
                }
            }
        }

        private void StepTimer() 
        {
            if (_calibrationStep) {
                if (_counter == 0) {
                    if (_leftHandData >= _maxOfAttempt) {
                        _maxOfAttempt = _leftHandData;
                        _l1 = _maxOfAttempt;
                    }
                }
                if (_counter == 2) {
                    if (_leftHandData >= _maxOfAttempt) {
                        _maxOfAttempt = _leftHandData;
                        _l2 = _maxOfAttempt;
                    }
                }
                if (_counter == 4) {
                    if (_leftHandData >= _maxOfAttempt) {
                        _maxOfAttempt = _leftHandData;
                        _l3 = _maxOfAttempt;
                    }
                }
                if (_counter == 6) {
                    if (_rightHandData >= _maxOfAttempt) {
                        _maxOfAttempt = _rightHandData;
                        _r1 = _maxOfAttempt;
                    }
                }
                if (_counter == 8) {
                    if (_rightHandData >= _maxOfAttempt) {
                        _maxOfAttempt = _rightHandData;
                        _r2 = _maxOfAttempt;
                    }
                }
                if (_counter == 10) {
                    if (_rightHandData >= _maxOfAttempt) {
                        _maxOfAttempt = _rightHandData;
                        _r3 = _maxOfAttempt;
                    }
                }
                _timerStep -= Time.deltaTime;
                if (_timerStep <= 0) {
                    SetMark(_markCounter);
                    _audioPlayerController.PlaySFX("OnCalibrationDone");
                    _markCounter++;

                    _timerStep = 3f;
                    _counter++;
                    _calibrationStep = false;
                    StartCalibrationNew();
                }
            }
        }

    }
}