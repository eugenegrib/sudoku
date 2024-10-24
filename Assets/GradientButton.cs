using UnityEngine;
using UnityEngine.UI;

public class GradientButtonWithMask : MonoBehaviour
{
    [SerializeField] public Color color1 = Color.red;
    [SerializeField] public Color color2 = Color.blue;

    private void Start()
    {
        // Создаем градиент
        var gradient = new Gradient();
        var colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(color1, 0.0f);
        colorKeys[1] = new GradientColorKey(color2, 1.0f);
        var alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);
        gradient.SetKeys(colorKeys, alphaKeys);

        // Получаем компонент Image
        Image buttonImage = GetComponent<Image>();

        // Устанавливаем градиент
        buttonImage.material = new Material(Shader.Find("Custom/GradientButtonShader"));
        buttonImage.material.SetColor("_Color1", color1);
        buttonImage.material.SetColor("_Color2", color2);
    }
}