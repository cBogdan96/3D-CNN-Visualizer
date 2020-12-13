using UnityEngine;
public class PlaneCreator : MonoBehaviour
{

    public Texture2D textureFeature;

    public int height, width, scale;
    public void PlaneObject(FeatureMapData featureMapData, ushort[][] featureMap)
    {
        scale = featureMapData.getCoordScale();
        gameObject.name = featureMapData.getName();
        height = featureMapData.getHeight();
        width = featureMapData.getWidth();
        textureFeature = new Texture2D(featureMapData.getHeight(), featureMapData.getWidth());
        
        Color[] pixels = new Color[featureMapData.getHeight() * featureMapData.getWidth()];
        int index = 0;
        int max = -10000;
        int min = 10000;

        for (int k = 0; k < height; k++)
            {
                for (int l = 0; l < width; l++)
                {
                    if (featureMap[k][l] > max)
                    {
                        max = featureMap[k][l];
                    }

                    if (featureMap[k][l] < min)
                    {
                        min = featureMap[k][l];
                    }
                }
            }
        for (int i = 0; i < featureMap.GetLength(0); i++)
        {
            for (int j = 0; j < featureMap.GetLength(0); j++)
            {
                float normalizedValue = ((float)featureMap[i][j]- min) / (max - min);
                pixels[index] = new Color(normalizedValue, normalizedValue, normalizedValue, 1);
                index++;
            }
        }
        textureFeature.SetPixels(pixels);
        textureFeature.Apply();
        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFeature); /// you will not need this in update 
        gameObject.transform.position = new Vector3(featureMapData.getCoordX(), featureMapData.getCoordY(), featureMapData.getCoordZ());
        gameObject.transform.Rotate(-90, 180, 0, Space.Self);
        gameObject.transform.localScale = new Vector3(width / 10f , 1.0f, height / 10f );
    }

    private void updatePlaneObject(Color[] pixels)
    {
        textureFeature.SetPixels(pixels);
        textureFeature.Apply();
    }
    public void createPlane(FeatureMapData featureMapData, ushort[][] featureMap)
    {
        this.PlaneObject(featureMapData, featureMap);
    }
    public void updatePlane(Color[] pixels)
    {
        this.updatePlaneObject(pixels);
    }
}
