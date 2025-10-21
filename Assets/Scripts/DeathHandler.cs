using StarterAssets;
using UnityEngine;

public class DeathHandler : MonoBehaviour {
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] FirstPersonController playerController;

    Weapon weapon;
    void Start() {
        gameOverCanvas.enabled = false;
        weapon = FindFirstObjectByType<Weapon>();
    }

    public void HandleDeath() {
        gameOverCanvas.enabled = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerController.enabled = false;
        weapon.canShoot = false;
        FindFirstObjectByType<WeaponSwitcher>().enabled = false;
    }


}
