using System;
using UnityEngine;

public class GameOverlayUI : MonoBehaviour
{
    enum OverlayStates
    {
        Hidden,
        Showing,
        Shown,
        Disappearing
    }

    public float FadeDuration = 1.5f;
    public float ShowTime = 1.5f;

    public event Action OnAnimationFinished;

    [SerializeField] private RoundIndicatorUI roundIndicator;
    private CanvasGroup canvasGroup;

    private OverlayStates state = OverlayStates.Hidden;
    private float remainingFade = 0.0f;

    public void ShowRound(int round, int player)
    {
        BeginFade(roundIndicator.gameObject);
        roundIndicator.ShowRound(round, player);
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }

    void BeginFade(GameObject shownObject)
    {
        foreach (var child in transform)
        {
            ((Transform)child).gameObject.SetActive(false);
        }

        shownObject.SetActive(true);
        canvasGroup.alpha = 0.0f;

        remainingFade = FadeDuration;
        state = OverlayStates.Showing;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case OverlayStates.Showing:
            {
                remainingFade -= Time.deltaTime;

                canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, remainingFade / FadeDuration);

                if (remainingFade <= 0.0f)
                {
                    remainingFade = ShowTime;
                    state = OverlayStates.Shown;
                }

                break;
            }
            case OverlayStates.Shown:
            {
                remainingFade -= Time.deltaTime;
                if (remainingFade <= 0.0f)
                {
                    remainingFade = FadeDuration;
                    state = OverlayStates.Disappearing;
                }

                break;
            }
            case OverlayStates.Disappearing:
            {
                remainingFade -= Time.deltaTime;

                canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, remainingFade / FadeDuration);

                if (remainingFade <= 0.0f)
                {
                    state = OverlayStates.Hidden;

                    if (OnAnimationFinished != null)
                    {
                        OnAnimationFinished();
                    }
                }

                break;
            }
        }
    }
}
