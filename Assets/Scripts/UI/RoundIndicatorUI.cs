using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundIndicatorUI : MonoBehaviour
{
    public Image RoundImage;

    public void ShowRound(int round, List<int> winList)
    {
        string spriteName = "Rounds/round" + round;
        Texture2D texture = (Texture2D)Resources.Load(spriteName);
        Rect rect = new Rect(0, 0, texture.width, texture.height);

        RoundImage.sprite = Sprite.Create(texture, rect, Vector2.zero);
        RoundImage.preserveAspect = true;
    }
}
