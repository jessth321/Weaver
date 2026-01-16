using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class TempTextFade : MonoBehaviour
{
    public TextMeshProUGUI textTMP;
    public float fadeTime;
    public float fadeDist;
    public AnimationCurve fadeCurve;
    float timer = 0;

    private void Awake()
    {
        //Fade();
    }

    public async void Fade()
    {
        Vector3 originalPos = transform.position;

        while (timer < fadeTime)
        {
            transform.position = originalPos + transform.up * fadeCurve.Evaluate(timer / fadeTime);

            timer += Time.deltaTime;

            await Task.Yield();
        }

        Destroy(gameObject);
    }
}
