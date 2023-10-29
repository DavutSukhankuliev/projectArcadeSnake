using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MiographInputConfig", menuName = "Configs/MiographInputConfig")]
public class MiographInputConfig : ScriptableObject
{
    [SerializeField] private int[] activeInputs;
    [SerializeField] public MiographInputModel[] inputModels;

    public int[] ActiveInputs => activeInputs;

    private Dictionary<int, MiographInputModel> _dict;

    public MiographInputModel Get(int id)
    {
        Init();
        return _dict[id];
    }

    private void Init()
    {
        if (_dict != null)
        {
            return;
        }
        _dict = new Dictionary<int, MiographInputModel>();
        foreach (var model in inputModels)
        {
            _dict.Add(model.ID, model);
        }
    }
}
