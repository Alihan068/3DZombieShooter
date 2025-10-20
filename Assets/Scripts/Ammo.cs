using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] int ammoAmount = 10;

    public int GetCurrentAmmo() {
        return ammoAmount;
    }

    public void ReduceAmmoAmount() {
        ammoAmount--;
    }
}
