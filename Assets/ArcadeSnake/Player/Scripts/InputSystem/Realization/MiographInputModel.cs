using System;
using UnityEngine;

public enum InputButtonType
{
    None,
    Left,
    Right
}

[Serializable]
public class MiographInputModel
{
    [SerializeField] private int id;
    [SerializeField] private int channel;
    [SerializeField] private InputButtonType buttonAfterEdge;
    [SerializeField] private float sensitivity;
    [SerializeField] private float delta;
    [SerializeField] private float miographCooldown;

    public int ID => id;
    public int Channel => channel;
    public InputButtonType ButtonAfterEdge => buttonAfterEdge;
    public float Sensitivity
    {
        get => sensitivity;
        set => sensitivity = value;
    }

    public float Delta => delta;
    public float MiographCooldown => miographCooldown;
}
