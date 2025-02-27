using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PressAnyKeyFader : MonoBehaviour
{
    public TextMeshProUGUI pressText;
    private float fadeSpeed = 1f;
    private bool fadingOut = true;

    void Update()
    {
        float alphaChange = fadeSpeed * Time.deltaTime;
        Color textColor = pressText.color;

        if (fadingOut)
        {
            textColor.a -= alphaChange;
            if (textColor.a <= 0f)
            {
                textColor.a = 0f;
                fadingOut = false;
            }
        }
        else
        {
            textColor.a += alphaChange;
            if (textColor.a >= 1f)
            {
                textColor.a = 1f;
                fadingOut = true;
            }
        }

        pressText.color = textColor;
    }
}
