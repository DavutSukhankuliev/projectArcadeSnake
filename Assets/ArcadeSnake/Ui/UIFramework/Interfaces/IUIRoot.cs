using UnityEngine;

namespace ArcadeSnake
{
    public interface IUIRoot
    {
        Canvas Canvas { get; set; }
        Camera Camera { get; set; }
        Transform Container { get; }
        Transform PoolContainer { get; }
    }
}
