using System;
using ArcadeSnake;

public interface IMiographDataSource
{
    public void GetData(int channel, Action<MiographData> onResponse);
}
