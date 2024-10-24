using UnityEngine;
using UnityEngine.UI;

public class GradientImage : MonoBehaviour
{
    [Header("Gradient Colors")]
    [SerializeField] private Color color1 = Color.red; // Первый цвет
    [SerializeField] private Color color2 = Color.blue; // Второй цвет

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();

        // Создаем новый материал с шейдером градиента
        Material gradientMaterial = new Material(Shader.Find("Custom/GradientShader"));
        
        // Устанавливаем цвета
        gradientMaterial.SetColor("_Color1", color1);
        gradientMaterial.SetColor("_Color2", color2);

        // Применяем материал к Image
        image.material = gradientMaterial;
    }
}