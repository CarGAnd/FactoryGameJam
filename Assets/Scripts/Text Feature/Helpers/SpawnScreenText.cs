using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public static class SpawnScreenText
{
    private static Vector2 offSetToScreenEdge = new Vector2(0, 100);
    private const float defaultDisappearTime = 3f;
    private const float defaultAnimationTime = 0.2f;
    private static Color defaultColor = Color.white;
    //private static LeanTweenType defaultLeanTweenType = LeanTweenType.easeInOutSine;
    //private static ScreenTextRotation defaultScreenTextRotation = ScreenTextRotation.None;

    public static GameObject SpawnDebugTextMidScreen(string label, ScreenTextRotation rotation = ScreenTextRotation.None, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f) {
        return SpawnText(label, Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f)), Color.white, TextAlignmentOptions.Center, 2, ScreenTextSize.Medium, rotation, textAnimation, tweenType, time);
    }

    public static GameObject SpawnDebugText(string label, ScreenTextRotation rotation = ScreenTextRotation.None, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f) {
        return SpawnText(label, Camera.main.ViewportToScreenPoint(new Vector2(0.05f, 0.95f)), Color.white, TextAlignmentOptions.Left, 2, ScreenTextSize.Small, rotation, textAnimation, tweenType, time);
    }

    public static GameObject SpawnText (string label, Vector2 position, Color color, TextAlignmentOptions textAlignmentOptions, float disappearAfter = 4f, ScreenTextSize size = ScreenTextSize.Large, ScreenTextRotation rotation = ScreenTextRotation.None, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f)
    {
        GameObject textParent = new GameObject();
        SetUpParent(ref textParent, position, rotation);

        TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
        SetUpTextLabel(ref labelText, label, color, size, textAlignmentOptions);

        AnimateText(ref textParent, textAnimation, tweenType, time);

        GameObject.Destroy(textParent, disappearAfter + time);

        return textParent;
    }

    public static GameObject SpawnTextRandom(string label, bool spawnRandom, Color color, float disappearAfter = 4f, ScreenTextSize size = ScreenTextSize.Large, ScreenTextRotation rotation = ScreenTextRotation.None, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f)
    {
        GameObject textParent = new GameObject();
        SetUpParent(ref textParent, Vector2.zero, rotation);

        TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
        SetUpTextLabel(ref labelText, label, color, size);

        if (spawnRandom)
        {
            Vector2 textSize = (new Vector2(labelText.fontSize * label.Length, labelText.fontSize) / 2) + offSetToScreenEdge;
            textParent.transform.position = GetRandomPosition(textSize);
        }

        AnimateText(ref textParent, textAnimation, tweenType, time);

        GameObject.Destroy(textParent, disappearAfter + time);

        return textParent;
    }

    public static GameObject SpawnText(string label, float disappearAfter = 4f, ScreenTextSize size = ScreenTextSize.Large, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            float time = Random.Range(0.2f, 0.8f);

            GameObject textParent = new GameObject();
            SetUpParent(ref textParent, Vector2.zero, ScreenTextRotation.RandomLarge);

            TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
            SetUpTextLabel(ref labelText, label, defaultColor, size);

            Vector2 textSize = (new Vector2(labelText.fontSize * label.Length, labelText.fontSize) / 2) + offSetToScreenEdge;
            textParent.transform.position = GetRandomPosition(textSize);

            //AnimateText(ref textParent, RandomUtilities.RandomEnumValueExcluding(ScreenTextAnimation.None), defaultLeanTweenType, time);

            GameObject.Destroy(textParent, disappearAfter + time);
        }

        return null;
    }

    public static GameObject SpawnText(string label, Vector2 position, TextAlignmentOptions textAlignmentOptions, ScreenTextSize size = ScreenTextSize.Large, ScreenTextRotation rotation = ScreenTextRotation.None, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f)
    {
        GameObject textParent = new GameObject();
        SetUpParent(ref textParent, position, rotation);

        TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
        SetUpTextLabel(ref labelText, label, defaultColor, size, textAlignmentOptions);

        AnimateText(ref textParent, textAnimation, tweenType, time);

        GameObject.Destroy(textParent, defaultDisappearTime + time);

        return textParent;
    }

    public static GameObject SpawnTextRandom(string label, bool spawnRandom, ScreenTextSize size = ScreenTextSize.Large, ScreenTextRotation rotation = ScreenTextRotation.None, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f)
    {
        GameObject textParent = new GameObject();
        SetUpParent(ref textParent, Vector2.zero, rotation);

        TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
        SetUpTextLabel(ref labelText, label, defaultColor, size);

        if (spawnRandom)
        {
            Vector2 textSize = (new Vector2(labelText.fontSize * label.Length, labelText.fontSize) / 2) + offSetToScreenEdge;
            textParent.transform.position = GetRandomPosition(textSize);
        }

        AnimateText(ref textParent, textAnimation, tweenType, time);

        GameObject.Destroy(textParent, defaultDisappearTime + time);

        return textParent;
    }

    public static GameObject SpawnTextAnimation(string label, Vector2 position, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f, ScreenTextSize size = ScreenTextSize.Large, ScreenTextRotation rotation = ScreenTextRotation.None)
    {
        GameObject textParent = new GameObject();
        SetUpParent(ref textParent, position, rotation);

        TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
        SetUpTextLabel(ref labelText, label, defaultColor, size);

        AnimateText(ref textParent, textAnimation, tweenType, time);

        GameObject.Destroy(textParent, defaultDisappearTime + time);

        return textParent;
    }

    public static GameObject SpawnTextRandom(string label, bool spawnRandom, ScreenTextAnimation textAnimation = ScreenTextAnimation.JumpIn, LeanTweenType tweenType = LeanTweenType.easeInOutSine, float time = 0.2f, ScreenTextSize size = ScreenTextSize.Large, ScreenTextRotation rotation = ScreenTextRotation.None)
    {
        GameObject textParent = new GameObject();
        SetUpParent(ref textParent, Vector2.zero, rotation);

        TextMeshProUGUI labelText = textParent.AddComponent<TextMeshProUGUI>();
        SetUpTextLabel(ref labelText, label, defaultColor, size);

        if (spawnRandom)
        {
            Vector2 textSize = (new Vector2(labelText.fontSize * label.Length, labelText.fontSize) / 2) + offSetToScreenEdge;
            textParent.transform.position = GetRandomPosition(textSize);
        }

        AnimateText(ref textParent, textAnimation, tweenType, time);

        GameObject.Destroy(textParent, defaultDisappearTime + time);

        return textParent;
    }

    private static Vector2 GetRandomPosition (Vector2 size)
    {
        float spawnY = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0 + size.y)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - size.y)).y);
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0 + size.x, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - size.x, 0)).x);

        return new Vector2(spawnX, spawnY);
    }

    private static void SetUpTextLabel (ref TextMeshProUGUI labelText, string label, Color color, ScreenTextSize size, TextAlignmentOptions textAlignmentOptions = TextAlignmentOptions.Center)
    {
        SetUpTextSize(size, ref labelText);
        labelText.text = label;
        labelText.enableWordWrapping = false;
        labelText.color = color;
        labelText.alignment = textAlignmentOptions;
    }

    private static void SetUpParent (ref GameObject gameObject, Vector2 position, ScreenTextRotation rotation)
    {
        gameObject.SetActive(false);
        gameObject.transform.position = position;
        gameObject.transform.rotation = SetUpTextRotation(rotation);

        Canvas parent = GameObject.FindObjectOfType<Canvas>();

        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.localScale = Vector3.one;
        gameObject.name = "Text";
    }

    private static void AnimateText (ref GameObject gameObject, ScreenTextAnimation textAnimation, LeanTweenType tweenType, float time)
    {
        switch (textAnimation)
        {
            case ScreenTextAnimation.None:
                gameObject.SetActive(true);
                return;
            case ScreenTextAnimation.JumpIn:
                JumpInAnimation(ref gameObject, tweenType, time);
                break;
            /*case ScreenTextAnimation.FadeInWiggle:
                FadeInAnimation(ref gameObject, tweenType, time);
                break;*/
            case ScreenTextAnimation.PopIn:
                PopInAnimation(ref gameObject, time);
                break;
        }
    }

    private static void JumpInAnimation (ref GameObject gameObject, LeanTweenType tweenType, float time)
    {
        Vector2 endPosition = gameObject.transform.position;
        gameObject.transform.position = endPosition + new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        gameObject.transform.localScale = Vector3.one * 3;
        gameObject.SetActive(true);

        LeanTween.move(gameObject, endPosition, time).setEaseSpring();
        LeanTween.scale(gameObject, Vector2.one, time).setEase(tweenType);
        //AddWaveEffect(endPosition, time);
    }

    private static void FadeInAnimation(ref GameObject gameObject, LeanTweenType tweenType, float time)
    {
        Vector2 endPosition = gameObject.transform.position;
        gameObject.transform.position = endPosition + new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        gameObject.transform.localScale = Vector3.one * 3;
        gameObject.SetActive(true);

        LeanTween.move(gameObject, endPosition, time).setEaseSpring();
        LeanTween.scale(gameObject, Vector2.one, time).setEase(tweenType);
    }

    private static void PopInAnimation(ref GameObject gameObject, float time)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        LeanTween.scale(gameObject, Vector2.one, time).setEaseSpring();
        LeanTween.moveLocalX(gameObject, 10, 0.5f).setEasePunch().setDelay(time);
    }

    /* private static void AddWaveEffect (Vector2 position, float time)
    {
        ParticleSystem system = GameObject.Instantiate(GameManager.Instance.impactEffect, position, Quaternion.Euler(90, 0, 0));
        var main = system.main;
        main.startDelay = time;
    } */

    private static void SetUpTextSize(ScreenTextSize type, ref TextMeshProUGUI labelText)
    {
        switch (type)
        {
            case ScreenTextSize.Small:
                labelText.fontSize = 60;
                break;
            case ScreenTextSize.Medium:
                labelText.fontSize = 120;
                break;
            case ScreenTextSize.Large:
                labelText.fontSize = 180;
                break;
            case ScreenTextSize.Mega:
                labelText.fontSize = 240;
                break;
            default:
                break;
        }
    }

    private static Quaternion SetUpTextRotation (ScreenTextRotation rotation)
    {
        switch (rotation)
        {
            case ScreenTextRotation.TiltLeftSmall:
                return Quaternion.Euler(0, 0, 5);
            case ScreenTextRotation.TiltRightSmall:
                return Quaternion.Euler(0, 0, -5);
            case ScreenTextRotation.TiltLeftMedium:
                return Quaternion.Euler(0, 0, 10);
            case ScreenTextRotation.TiltRightMedium:
                return Quaternion.Euler(0, 0, -10);
            case ScreenTextRotation.TiltLeftLarge:
                return Quaternion.Euler(0, 0, 15);
            case ScreenTextRotation.TiltRightLarge:
                return Quaternion.Euler(0, 0, -15);
            case ScreenTextRotation.None:
                return Quaternion.Euler(0, 0, 0);
            case ScreenTextRotation.RandomSmall:
                return Quaternion.Euler(0, 0, Random.Range(-5, 5));
            case ScreenTextRotation.RandomMedium:
                return Quaternion.Euler(0, 0, Random.Range(-10, 10));
            case ScreenTextRotation.RandomLarge:
                return Quaternion.Euler(0, 0, Random.Range(-15, 15));
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }

    public enum ScreenTextRotation
    {
        TiltLeftSmall,
        TiltRightSmall,
        TiltLeftMedium,
        TiltRightMedium,
        TiltLeftLarge,
        TiltRightLarge,
        None,
        RandomSmall,
        RandomMedium,
        RandomLarge
    }

    public enum ScreenTextAnimation
    {
        None,
        JumpIn,
        //FadeInWiggle,
        PopIn
    }

    public enum ScreenTextSize
    {
        Small,
        Medium,
        Large,
        Mega
    }
}