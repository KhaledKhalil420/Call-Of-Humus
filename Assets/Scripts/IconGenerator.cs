using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

public class IconGenerator : MonoBehaviour
{
    public Weapon[] allInGameItems;
    public Camera renderCamera;
    public string saveTo;

    [Header("Background remover")]
    public Color colorToRemove;
    public float tolerance = 0.1f;

    private GameObject itemInstance;

    public void LoadAllItems()
    {
        // Load all items
        allInGameItems = Resources.LoadAll<Weapon>("Items");
    }

    public void Generate()
    {
        // Instantiate the first item model (for demonstration purposes)
        if (itemInstance != null) DestroyImmediate(itemInstance);
        if (allInGameItems.Length > 0)
        {
            Weapon item = allInGameItems[0];
            itemInstance = Instantiate(item.model, Vector3.zero, Quaternion.identity);
            itemInstance.tag = "Finish";
        }
    }

    public async void Save()
    {
        // Set camera settings
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = new Color(colorToRemove.r, colorToRemove.g, colorToRemove.b, 0); // Transparent background

        // Create a Render Texture with ARGB32 format for transparency
        RenderTexture renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
        renderTexture.useMipMap = false;
        renderTexture.autoGenerateMips = false;
        renderCamera.targetTexture = renderTexture;

        // Render the camera's view to the Render Texture
        renderCamera.Render();

        // Create a new texture and read the Render Texture into it
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        ApplyTransparencyMask(texture);

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();
        string fileName = Path.Combine(saveTo, allInGameItems[0].itemName + "_Icon.png");

        await Task.Run(() => File.WriteAllBytes(fileName, bytes));

        // Reset
        RenderTexture.active = null;
        renderCamera.targetTexture = null;
        DestroyImmediate(renderTexture);

        await Task.Run(() => allInGameItems[0].sprite = Sprite.Create(texture, new Rect(), new Vector2(0, 0), 100));
    }

    public void RemoveCurrentObject()
    {
        DestroyImmediate(GameObject.FindWithTag("Finish"));
        itemInstance = null;
    }

    public void SetPositionAsTool()
    {
        itemInstance.transform.position = new Vector3(-0.3f, -0.9f, 0);
        itemInstance.transform.eulerAngles = new Vector3(15, 90, 0);
    }

    public void SetPositionAsItem()
    {
        itemInstance.transform.position = Vector3.zero;
        itemInstance.transform.eulerAngles = new Vector3(55, 0, -35);
    }

    private void ApplyTransparencyMask(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {   
            // Check if the pixel color is close to the color
            if (IsColorClose(pixels[i], colorToRemove))
            {
                // Set alpha to 0
                pixels[i].a = 0; 
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    private bool IsColorClose(Color color1, Color color2)
    {
        // Compare all colors to the remove color
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }
}

#region Editor

#if UNITY_EDITOR
[CustomEditor(typeof(IconGenerator))]
public class IconGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IconGenerator generator = (IconGenerator)target;
        if (GUILayout.Button("Generate Icon"))
        {
            generator.Generate();
        }

        if (GUILayout.Button("Save Icon"))
        {
            generator.Save();
        }

        if (GUILayout.Button("Load all Items"))
        {
            generator.LoadAllItems();
        }

        if (GUILayout.Button("Remove current object"))
        {
            generator.RemoveCurrentObject();
        }

        if (GUILayout.Button("Set position as tool"))
        {
            generator.SetPositionAsTool();
        }

        if (GUILayout.Button("Set position as item"))
        {
            generator.SetPositionAsItem();
        }
    }
}

#endif
#endregion
