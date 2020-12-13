using UnityEngine;


public class CubeCreator : MonoBehaviour
{
    public Texture2D textureFeature;
    public void createCube(FeatureMapData featureMapData, string name, float pixel)
    {
        this.createCubeObject(featureMapData, name , pixel);
        Resources.UnloadUnusedAssets();
    }
    public void updateCube(Color[] pixel)
    {
        this.updateCubeObject(pixel);
        Resources.UnloadUnusedAssets();
    }
    public void createCubeObject(FeatureMapData featureMapData, string name, float pixel)
    {
        gameObject.name = name;
        textureFeature = new Texture2D(1, 1);

        Color[] pixels = new Color[1];
        pixels[0] = new Color(pixel, pixel, pixel, 1);
        textureFeature.SetPixels(pixels);
        textureFeature.Apply();

        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFeature);
        gameObject.transform.position = new Vector3(featureMapData.getCoordX(), featureMapData.getCoordY(), featureMapData.getCoordZ());
        gameObject.transform.Rotate(0, 0, 0, Space.Self);
        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * 30;
    }

    private void updateCubeObject(Color[] pixels)
    {
       textureFeature.SetPixels(pixels);
       textureFeature.Apply();
    }
}
