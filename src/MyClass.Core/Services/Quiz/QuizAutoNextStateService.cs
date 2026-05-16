using System.Collections.Concurrent;

namespace MyClass.Core.Services;

public sealed class QuizAutoNextStateService : IQuizAutoNextStateService
{
    private readonly ConcurrentDictionary<int, bool> _autoNextStates = new();

    public bool IsAutoNextEnabled(int classId)
    {
        return _autoNextStates.TryGetValue(classId, out var isEnabled) && isEnabled;
    }

    public void SetAutoNextEnabled(int classId, bool isEnabled)
    {
        if (!isEnabled)
        {
            ClearAutoNext(classId);
            return;
        }

        _autoNextStates[classId] = true;
    }

    public void ClearAutoNext(int classId)
    {
        _autoNextStates.TryRemove(classId, out _);
    }
}
