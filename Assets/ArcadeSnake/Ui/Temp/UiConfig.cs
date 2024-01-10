using System.Collections.Generic;
using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "UiConfig", menuName = "Config/UiConfig", order = 0)]
    public class UiConfig : ScriptableObject
    {
        [SerializeField] private int[] activeMachines;
        [SerializeField] private UiModel[] models;

        public int[] ActiveMachines => activeMachines;

        private Dictionary<int, UiModel> _dict = new Dictionary<int, UiModel>();

        public UiModel Get(int id)
        {
            Init();
            return _dict[id];
        }

        private void Init()
        {
            if (_dict.Count > 0)
            {
                return;
            }
            foreach (var model in models)
            {
                _dict.Add(model.ID, model);
            }
        }
    }
}