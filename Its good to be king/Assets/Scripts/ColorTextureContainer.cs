using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTextureContainer : MonoBehaviour
{
    public Texture2D colorCodedTexture;

    public Texture2D GetColorCodedTexture()
    {
        return colorCodedTexture;
    }
}
