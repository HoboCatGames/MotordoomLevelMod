using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ModSkinBike : ModSkin
{
    [Header("Skin")]
    public Material material;
    public Texture texture;
    public Texture glowTexture;
    public Mesh[] meshes;
    public Mesh additionalMesh;
}
