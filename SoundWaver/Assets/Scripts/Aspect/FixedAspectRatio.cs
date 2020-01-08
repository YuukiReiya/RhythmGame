using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedAspectRatio
{
    static float _width = 1;
    static float _height = 1;
    public static float Aspect { get { return _height / _width; } }
    private static float DeviceAspect { get { return (float)Screen.height / (float)Screen.width; } }


    public static void Setup(float width, float height)
    {
        _width = width;
        _height = height;
    }
    public static void FitToWidth2D(Camera camera)
    {
        float width = (float)Screen.width;
        camera.orthographicSize = _width / width;
    }

    public static void FitToHeight2D(Camera camera)
    {
        float height = (float)Screen.height;
        camera.orthographicSize = _height / height;
    }

    public static void FitToWidth3D(Camera camera)
    {
        var rect = camera.rect;
        float width = (float)Screen.width;
        float height = (float)Screen.height;
        float hRatio = (width / height) / (_width / _height);
        if (1.0f > hRatio)
        {
            rect.x = 0;
            rect.y = (1.0f - hRatio) / 2.0f;
            rect.width = 1.0f;
            rect.height = hRatio;
        }
        else
        {
            float wRatio = 1.0f / hRatio;
            rect.x = (1.0f - wRatio) / 2.0f;
            rect.y = 0.0f;
            rect.width = wRatio;
            rect.height = 1.0f;
        }
        camera.rect = rect;
    }

    public static void FitToHeight3D(Camera camera)
    {
        var rect = camera.rect;
        float width = (float)Screen.width;
        float height = (float)Screen.height;
        float wRatio = (height / width) / (_height / _width);
        if (1.0f > wRatio)
        {
            rect.x = 0;
            rect.y = (1.0f - wRatio) / 2.0f;
            rect.width = 1.0f;
            rect.height = wRatio;
        }
        else
        {
            float hRatio = 1.0f / wRatio;
            rect.x = (1.0f - hRatio) / 2.0f;
            rect.y = 0.0f;
            rect.width = hRatio;
            rect.height = 1.0f;
        }
        camera.rect = rect;
    }
}