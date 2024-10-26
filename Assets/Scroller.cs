using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    [SerializeField] private float _x, _y;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _image.uvRect = new Rect(_image.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _image.uvRect.size);
    }
}