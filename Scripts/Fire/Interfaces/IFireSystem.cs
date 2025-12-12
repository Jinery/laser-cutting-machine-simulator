using UnityEngine.Events;

public interface IFireSystem
{
    event UnityAction<FireSource> onFireStarted;
    event UnityAction<FireSource> onFireExtinguished;
    void ReportFire(FireSource source);
    void ReportExtinguished(FireSource source);
}
