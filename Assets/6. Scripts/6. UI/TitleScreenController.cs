using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{
    public RectTransform titleScreenUI;
    public RectTransform mainMenuUI;
    private bool keyPressed = false;
    private float transitionSpeed = 1500f;

    void Update()
    {
        if (!keyPressed && Input.anyKeyDown)
        {
            keyPressed = true;
            StartCoroutine(SlideToMainMenu());
        }
    }

    private IEnumerator SlideToMainMenu()
    {
        Vector3 targetPosition = new Vector3(-1920, 0, 0); // Move title screen left
        while (titleScreenUI.anchoredPosition.x > -1920)
        {
            titleScreenUI.anchoredPosition = Vector3.MoveTowards(titleScreenUI.anchoredPosition, targetPosition, transitionSpeed * Time.deltaTime);
            mainMenuUI.anchoredPosition = Vector3.MoveTowards(mainMenuUI.anchoredPosition, Vector3.zero, transitionSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
