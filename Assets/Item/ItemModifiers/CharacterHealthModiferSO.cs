using UnityEngine;

[CreateAssetMenu]
public class CharacterHealthModiferSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        HealthBar.Instance.UpdateHealth(+5);
    }
}
