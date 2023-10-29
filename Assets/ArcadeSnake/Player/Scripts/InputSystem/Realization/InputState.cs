using System.Collections.Generic;

public class InputState
{
    private Dictionary<InputButtonType, float> _dict = new Dictionary<InputButtonType, float>();
    
    public InputState()
    {        
        _dict.Add(InputButtonType.Left, 0f);
        _dict.Add(InputButtonType.None, 0f);
        _dict.Add(InputButtonType.Right, 0f);
    }

    public void PressButton(InputButtonType buttonType, float value)
    {
        _dict[buttonType] = value;
    }

    public float GetValue(InputButtonType buttonType)
    {
        return _dict[buttonType];
    }
    public bool IsLeftPressed
    {
        get;
        set;
    }

    public bool IsRightPressed
    {
        get;
        set;
    }
}
