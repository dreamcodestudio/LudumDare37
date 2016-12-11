using IndieYP.Delegates;
using IndieYP.Utils;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    //Gameplay
    public static event Shared.VoidDelegate OnCellExploded;
    public static event Shared.VoidDelegate OnFieldUpdated;
    public static event Shared.VoidDelegate OnCompletedStage;

    //Inputs

    public void CallCellExploded()
    {
        if (OnCellExploded != null)
            OnCellExploded();
    }

    public void CallFieldUpdated()
    {
        if (OnFieldUpdated != null)
            OnFieldUpdated();
    }

    public void CallCompletedStage()
    {
        if (OnCompletedStage != null)
            OnCompletedStage();
    }

}
