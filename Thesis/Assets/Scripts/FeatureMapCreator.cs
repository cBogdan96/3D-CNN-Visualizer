using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FeatureMapCreator
{
    public StructHelper structHelper = new StructHelper();


    //public static List<string> predictions_labels  = new List<string>();

    public FeatureMapCreator()
    {
    }
    public Tuple<int, int> closestDivisors(int length)
    {
        int n = (int)Math.Floor(Math.Sqrt(length));
        Tuple<int, int> res = null;
        for (int i = n; i > 0; i--)
        {
            if (length % i == 0)
            {
                return res = new Tuple<int, int>(i, length / i);
            }
        }
        return res;
    }
    public StructHelper makeFeature(GameObject fetureTemplate, GameObject featurePredictionsTemplate, Dictionary<string, Dictionary<string, ushort[][]>> feature)
    {
        FeatureMapData featureMapData = new FeatureMapData();
        int index;
        int counter;
        GameObject go;
        initStructObjects();
        featureMapData.setCoordX(0);
        featureMapData.setCoordY(0);
        featureMapData.setCoordZ(0);
        int counterLayers = 0;
        int scalingFactor = 40;
        var labels = new List<string>(feature["labels"].Keys);

        foreach (KeyValuePair<string, Dictionary<string, ushort[][]>> layer in feature)
        {

            counter = 0;
            counterLayers = counterLayers + 1;
            featureMapData.setCoordScale(scalingFactor / counterLayers);
            Tuple<int, int>  divisors = closestDivisors(layer.Value.Count);
            featureMapData.setCoordY(0);
            featureMapData.setCoordX(0);
            featureMapData.setCoordZ(featureMapData.getCoordZ() - 200);
            foreach (KeyValuePair<string, ushort[][]> featureMap in layer.Value)
            {
                featureMapData.setHeight(featureMap.Value.GetLength(0));
                featureMapData.setWidth(featureMap.Value.GetLength(0));
                featureMapData.setName(featureMap.Key);

                featureMapData.setCoordY(featureMapData.getCoordY() + featureMapData.getWidth() + featureMapData.getWidth());

                if (featureMapData.getName().Contains("fc") ||featureMapData.getName().Contains("dense")||featureMapData.getName().Contains("pred"))
                {
                    index = 0;
                    divisors = closestDivisors(featureMap.Value[0].Length);
                    var name = featureMapData.getName();
                    var featureMaps = featureMap.Value[0];
                    featureMapData.setCoordX(0);
                    featureMapData.setCoordY(0);
                    for (int i = 0; i < divisors.Item1; i++)
                    {
                        featureMapData.setCoordX(featureMapData.getCoordX() + 60);
                        featureMapData.setCoordY(0);
                        for (int j = 0; j < divisors.Item2; j++)
                        {
                            go = GameObject.Instantiate(featurePredictionsTemplate);
                            go.name = featureMapData.getName();
                            CubeCreator cube = go.AddComponent<CubeCreator>();
                            featureMapData.setCoordY(featureMapData.getCoordY() + 60);

                            if(featureMapData.getName().Contains("pred")){
                                cube.createCube(featureMapData, name + "_" + labels[index], featureMaps[index]); 
                                structHelper.LastLayersPredictions.Add(name + "_" + index.ToString(), cube);
                            }else{
                                cube.createCube(featureMapData, name + "_" + index.ToString(), featureMaps[index]); 
                                structHelper.LastLayersPredictions.Add(name + "_" + index.ToString(), cube);
                            }
                            index++;
                        }
                    }
                }else
                {
                    go = GameObject.Instantiate(fetureTemplate);
                    go.name = featureMapData.getName();
                    PlaneCreator plane = go.GetComponent<PlaneCreator>();
                    plane.createPlane(featureMapData, featureMap.Value);
                    structHelper.convLayers.Add(featureMap.Key, plane);
                }
                counter++;
                if (counter == divisors.Item2)
                {
                    featureMapData.setCoordY(0);
                    featureMapData.setCoordX(featureMapData.getCoordX()+ featureMapData.getHeight() + featureMapData.getHeight());
                    counter = 0;
                }
            }
        }
        var parent = GameObject.Find("BackCamera").GetComponent<Camera>().transform.position = new Vector3(800, 800, featureMapData.getCoordZ()-70);
        return structHelper; 
    }

    public StructHelper updateFeatures(Dictionary<string, Dictionary<string, ushort[][]>> feature)
    {
        initStructDataObjects();
        foreach (KeyValuePair<string, Dictionary<string, ushort[][]>> layer in feature)
        {
            Tuple<int, int>  divisors = closestDivisors(layer.Value.Count());

            foreach (KeyValuePair<string, ushort[][]> featureMap in layer.Value)
            {
                if (featureMap.Key.Contains("fc") || featureMap.Key.Contains("dense") || featureMap.Key.Contains("predictions"))
                {
                    var name = featureMap.Key;
                    int index = 0;
                    var featureMaps = featureMap.Value[0];
                    divisors = closestDivisors(featureMap.Value[0].Length);
                    for (int i = 0; i < divisors.Item1; i++)
                    {
                        for (int j = 0; j < divisors.Item2; j++)
                        {   
                            if(featureMap.Key.Contains("predictions")){
                                structHelper.featureMapListLastLayers[featureMap.Key +"_" +index.ToString()] = new KeyValuePair<string, ushort>(featureMap.Key+"_"+index.ToString(), featureMaps[index]);
                            }else{
                                structHelper.featureMapListLastLayers[featureMap.Key+ "_" + index.ToString()] = new KeyValuePair<string, ushort>(featureMap.Key+ "_" + index.ToString(), featureMaps[index]);
                            }
                            index++;
                        }
                    }
                }else
                {
                    structHelper.featureMapList[featureMap.Key] = featureMap;
                }
            }
        }
        return structHelper;
    }
    public void initStructObjects (){
        structHelper.LastLayersPredictions = new Dictionary<string, CubeCreator>();
        structHelper.convLayers = new Dictionary<string, PlaneCreator>();
    }
    public void initStructDataObjects(){
        structHelper.featureMapList = new Dictionary<string, KeyValuePair<string, ushort[][]>>();
        structHelper.featureMapListLastLayers = new Dictionary<string, KeyValuePair<string, ushort>>();
    }
}
