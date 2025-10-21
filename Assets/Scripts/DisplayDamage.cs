using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayDamage : MonoBehaviour
{

    [SerializeField] Canvas damageIndicatorCanvas;
    [SerializeField] List<GameObject> bloodSplatterImages;
    [SerializeField] float impactTime = 0.3f;
    void Start()
    {
        foreach (var image in bloodSplatterImages) {
            image.SetActive(false);
        }
    }

    public void ShowDamageImpact() {
        StartCoroutine(ShowSplatter());
    }

    IEnumerator ShowSplatter() {
        int randomIndex =  Random.Range(0, bloodSplatterImages.Count);
        GameObject selectedImage = bloodSplatterImages[randomIndex];
        
        selectedImage.SetActive(true);

        yield return new WaitForSeconds(impactTime);

        selectedImage.SetActive(false);
    }
}
