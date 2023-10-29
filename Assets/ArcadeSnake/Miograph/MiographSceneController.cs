using System;
using DG.Tweening;
using Zenject;

namespace ArcadeSnake
{
    public struct MiographSceneProtocol
    {
        public int ID;
        public int Channel;
        public int LocalID;
        public float Data;
        public Sensitivity Sensitivity;
        public float Delta;
        public float MiographCooldown;

        public MiographSceneProtocol(int id, int channel, int localid, float data, Sensitivity sens, float delta, float miographCooldown)
        {
            ID = id;
            Channel = channel;
            LocalID = localid;
            Data = data;
            Sensitivity = sens;
            Delta = delta;
            MiographCooldown = miographCooldown;
        }
    }

    public class Sensitivity
    {
        public float Global;

        public Sensitivity(float global)
        {
            Global = global;
        }
    }

    public class MiographSceneController
    {
        private int _id;
        private int _channel;
        private int _localID;
        private float _delta;
        private float _miographCooldown;
        
        private IMiographDataSource _miographNetworkController;
        private MiographInputConfig _config;

        public Action<MiographSceneProtocol> SetDataAction { get; set; }
        public Sensitivity Sensitivity { get; set; }

        public void Action()
        {
            _miographNetworkController.GetData(_channel, (d) =>
            {                
                if (Int32.TryParse(d.Data, out var value))
                {
                    var data = value / 65472.0f;

                    SetDataAction?.Invoke(new MiographSceneProtocol(_id, _channel, _localID, data, Sensitivity, _delta, _miographCooldown));                    
                }
                
                DOVirtual.DelayedCall(0.05f, () =>
                {
                    Action();
                });                
            });
        }

        public MiographSceneController(
            int id,
            int localID,
            IMiographDataSource miographNetworkController,
            MiographInputConfig config)
        {            
            _config = config;
            _id = id;
            _channel = _config.Get(_id).Channel;            
            Sensitivity = new Sensitivity(_config.Get(_id).Sensitivity);
            _delta = _config.Get(_id).Delta;
            _miographCooldown = _config.Get(_id).MiographCooldown;
            _localID = localID;
            _miographNetworkController = miographNetworkController;
        }

        public class Factory : PlaceholderFactory<int, int, IMiographDataSource, MiographSceneController>
        {

        }
    }
}