using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectsListSO", menuName = "Scriptable Objects/StatusEffectsListSO")]
public class StatusEffectsListSO : ScriptableObject
{
    public StatusEffectSO[] StatusEffects;

    private void OnValidate()
    {
        if (StatusEffects == null)
        {
            StatusEffects = System.Array.Empty<StatusEffectSO>();
            return;
        }

        StatusEffects = System.Array.FindAll(StatusEffects, effect => effect != null);
    }
}
