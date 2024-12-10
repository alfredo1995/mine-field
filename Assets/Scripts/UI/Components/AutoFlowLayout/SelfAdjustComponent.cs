using UnityEngine;
using static OpaGames.Blackcopter.UI.AutoFlowLayout.AutoFlowLayout;

public class SelfAdjustComponent : MonoBehaviour
{
    [SerializeField] private OrientationAdjustment orientations;

    private void Awake()
    {
        OnLandscape += () => orientations.SetOrientation();
        OnPortrait += () => orientations.SetOrientation(true);
    }
}
