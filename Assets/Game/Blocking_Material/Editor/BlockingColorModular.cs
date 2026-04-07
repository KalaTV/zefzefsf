using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class BlockingColorModular : MonoBehaviour
{
    [SerializeField]
    private Color color = Color.white;

    private MeshRenderer meshRenderer;
    private Material instanceMaterial;

    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        CreateInstanceMaterial();
        ApplyColor();
    }

    void OnValidate()
    {
        CreateInstanceMaterial();
        ApplyColor();
    }

    private void CreateInstanceMaterial()
    {
        if (meshRenderer == null) return;
        
        if (instanceMaterial != null && meshRenderer.sharedMaterial == instanceMaterial)
            return;
        instanceMaterial = new Material(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial = instanceMaterial;
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        ApplyColor();
    }

    public Color GetColor()
    {
        return color;
    }

    private void ApplyColor()
    {
        if (instanceMaterial == null) return;

        if (instanceMaterial.HasProperty("_Color"))
        {
            instanceMaterial.color = color;
        }
        else if (instanceMaterial.HasProperty("_BaseColor"))
        {
            instanceMaterial.SetColor("_BaseColor", color);
        }
    }
}