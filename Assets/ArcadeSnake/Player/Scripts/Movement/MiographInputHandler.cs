using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


namespace ArcadeSnake {
    public class MiographInputHandler 
    {
        public EventHandler<float>[] GetValueEvent { get; set; }

        public EventHandler<float> RightHandEvent;
        public EventHandler<float> LeftHandEvent;
        private MiographInputConfig _inputConfig;
        
        private MiographSceneController.Factory _controllerFactory;
        private MiographNetworkController _networkController;
        private InputState _input;

        private InputButtonType[] _inputButtonTypes;
        private MiographSceneController[] _sceneControllers;

        private float[] _velocities;
        private float[] _values;

        private float _miographTime;

        private InputButtonType[] _inputButtonsAfter;
        public float[] Values => _values;
        

        public MiographInputHandler(
            MiographInputConfig inputConfig,
            MiographSceneController.Factory controllerFactory,
            MiographNetworkController miographNetworkController,
            InputState input
        )
        {
            _inputConfig = inputConfig;
            _controllerFactory = controllerFactory;
            _input = input;
            _networkController = miographNetworkController;
            
            _inputButtonTypes = new InputButtonType[_inputConfig.ActiveInputs.Length];
            _sceneControllers = new MiographSceneController[_inputConfig.ActiveInputs.Length];
        }

        public void CreateInput(IMiographDataSource miographDataSource)
        {
            var length = _inputConfig.ActiveInputs.Length;
            GetValueEvent = new EventHandler<float>[length];

            _velocities = new float[length];
            _values = new float[length];

            _inputButtonsAfter = new InputButtonType[length];
            
            for (var i = 0; i < length; i++)
            {
                _sceneControllers[i] = _controllerFactory.Create(_inputConfig.ActiveInputs[i], i, miographDataSource);
                _sceneControllers[i].SetDataAction += DataHandler;
                CheckConnection(_sceneControllers[i]);            
            
                _sceneControllers[i].Action();
            }
        }

        private void CheckConnection(MiographSceneController miographSceneController)
        {
            if(_networkController.IsConnected)
            {
                miographSceneController.Action();            
            }
            else
            {            
                DOVirtual.DelayedCall(0.1f, () => { CheckConnection(miographSceneController); });
            }
        }       
     
        private void DataHandler(MiographSceneProtocol protocol)
        {
            _miographTime += Time.deltaTime;
            
            _values[protocol.LocalID] = Mathf.SmoothDamp(_values[protocol.LocalID], protocol.Data,
                ref _velocities[protocol.LocalID], 0.1f);
            GetValueEvent[protocol.LocalID]?.Invoke(this, _values[protocol.LocalID]);

            if (protocol.ID == 0)
            {
                LeftHandEvent?.Invoke(this, protocol.Data);
            }
            else
            {
                RightHandEvent?.Invoke(this, protocol.Data);
            }

            var m = _inputConfig.Get(protocol.ID);

            if (m.Channel != protocol.Channel)
            {
                return;
            }

            if (protocol.Data > protocol.Sensitivity.Global && _miographTime >= protocol.MiographCooldown)
            {
                switch (m.ButtonAfterEdge)
                {
                    case InputButtonType.None:
                        _inputButtonTypes[protocol.LocalID] = InputButtonType.None;
                        break;
                    case InputButtonType.Left:
                        _input.IsLeftPressed = true;
                        _inputButtonTypes[protocol.LocalID] = InputButtonType.Left;
                        break;
                    case InputButtonType.Right:
                        _input.IsRightPressed = true;
                        _inputButtonTypes[protocol.LocalID] = InputButtonType.Right;
                        break;
                    default:
                        break;
                }
                _miographTime = 0;
            }
        }
    }
}
