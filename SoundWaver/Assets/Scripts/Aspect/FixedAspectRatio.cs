using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedAspectRatio
{
    static float _width;
    static float _height;
    public static float Aspect { get { return _height / _width; } }

    public static void Execute (Camera camera, float width, float height)
    {
        _width = width;
        _height = height;
        float ratio = Aspect / (Screen.height * 1.0f / Screen.width);
        float offset = (1.0f - ratio) / 2f;
        camera.rect = new Rect (offset, 0f, ratio, 1f);
    }
}