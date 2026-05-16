namespace MyClass.Core.Services;

public interface IQuizAutoNextStateService
{
    bool IsAutoNextEnabled(int classId);

    void SetAutoNextEnabled(int classId, bool isEnabled);

    void ClearAutoNext(int classId);
}
