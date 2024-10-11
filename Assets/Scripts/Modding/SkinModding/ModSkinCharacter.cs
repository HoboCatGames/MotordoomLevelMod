using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModSkinCharacter : ModSkin
{
    [Header("Skin")]
    public Material material;
    public Texture texture;
    public Mesh[] meshes;
    public Mesh additionalMesh;
}
