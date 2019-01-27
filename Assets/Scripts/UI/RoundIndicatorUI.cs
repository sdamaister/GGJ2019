using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundIndicatorUI : MonoBehaviour
{
    public Image RoundImage;
    public RectTransform DotContainer;
    public Sprite DotSprite;
    public float DotSize = 20.0f;

    private Color[] playerColors =
    {
        new Color(0.5f, 0.5f, 0.5f),
        new Color(1.0f, 0.0f, 0.0f),
        new Color(0.03933785f, 0.490566f, 0.1836982f),
        new Color(0.07075471f, 0.2146379f, 1.0f),
        new Color(0.9693313f, 1.0f, 0.0f)
    };

    public void ShowRound(int round, List<int> winList)
    {
        string spriteName = "Rounds/round" + round;
        Texture2D texture = (Texture2D)Resources.Load(spriteName);
        Rect rect = new Rect(0, 0, texture.width, texture.height);

        RoundImage.sprite = Sprite.Create(texture, rect, Vector2.zero);
        RoundImage.preserveAspect = true;

        RectTransform localRectTransform = GetComponent<RectTransform>();

        RoundImage.rectTransform.sizeDelta = new Vector2(localRectTransform.rect.width, localRectTransform.rect.height * 0.5f);
        RoundImage.rectTransform.anchoredPosition = new Vector2(0.0f, -RoundImage.rectTransform.rect.height * 0.3f);

        GeneratePlayerList(winList);
    }

    private void GeneratePlayerList(List<int> winList)
    {
        foreach (Transform child in DotContainer.transform)
        {
            Destroy(child.gameObject);
        }

        float totalWidth = winList.Count * DotSize;

        DotContainer.sizeDelta = new Vector2(totalWidth, DotSize);

        DotContainer.anchoredPosition = new Vector2(0.0f, -((RoundImage.rectTransform.rect.height / RoundImage.sprite.texture.height) * 100.0f) + RoundImage.rectTransform.anchoredPosition.y);

        float currentPostion = DotSize / 2;
        for (int i = 0; i < winList.Count; i++)
        {
            GameObject dotObject = new GameObject();
            dotObject.name = "Winner #" + i + " Player #" + winList[i];

            Image image = dotObject.AddComponent<Image>();
            image.sprite = DotSprite;
            image.preserveAspect = true;
            image.color = playerColors[winList[i] + 1];

            RectTransform rectTransform = image.rectTransform;

            rectTransform.SetParent(DotContainer.transform, false);

            rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(currentPostion, 0.0f);
            rectTransform.sizeDelta = new Vector2(DotSize, DotSize);

            currentPostion += DotSize;
        }
    }
}
