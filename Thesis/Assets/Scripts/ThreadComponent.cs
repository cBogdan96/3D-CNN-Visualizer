using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class ThreadComponent
{
    Thread thread;
    int index;
    Main main;
    FeatureMapCreator featureMapCreator;
    Dictionary<string, Dictionary<string, ushort[][]>> feature;
    StructHelper structHelper;
    byte[] frameInfo = new byte[1];

    public ThreadComponent(int index, Main main, FeatureMapCreator featureMapCreator)
    {
        this.featureMapCreator = featureMapCreator;
        this.index = index;
        this.main = main;
    }

    public void init()
    {
        thread = new Thread(doLogic);
        thread.IsBackground = true;
        thread.Start();
    }
    void doLogic()
    {
        Connector socketConnector = new Connector();
        socketConnector.establishConnection(60000 + index);
        while (true)
        {
            doLogicGetData(socketConnector);
        }
    }
    void doLogicGetData(Connector socketConnector)
    {

        socketConnector.Send(Encoding.ASCII.GetBytes("\n"));
        socketConnector.Receive(ref frameInfo);
        
        if (frameInfo != null) // size of the empty buffer
        {
            Debug.Log("I started deserialization for" + index);
            this.feature = MessagePackSerializer.Deserialize<Dictionary<string, Dictionary<string, ushort[][]>>>(frameInfo);
            Debug.Log("I finished deserialization for" + index);
            structHelper = featureMapCreator.updateFeatures(feature);
            try
            {
                var convPixelsFifthBatch = getConvPixels(structHelper.featureMapList);
                var lastLayersPixels = getLastLayersPixels(); // make sure it is not reused  
                var time = DateTime.Now.ToString("HH:mm:ss");
                main.updateFeatureMapPixels(convPixelsFifthBatch, lastLayersPixels, time);
            }
            catch
            {
            }

        }

    }
    public void stop()
    {
        thread.Interrupt();
    }
    public Dictionary<string, Color[]> getConvPixels(Dictionary<string, KeyValuePair<string, ushort[][]>> features)
    {
        Color color = new Color();
        color.a = 1f;
        Color[] pixels;

        Dictionary<string, Color[]> featureMapsPixels = new Dictionary<string, Color[]>();
        var values = features.Values;
        float normalizedValue;

        var max = -100000;
        var min = 100000;
        int index;

        foreach (KeyValuePair<string, ushort[][]> f in values)
        {
            index = 0;
            int height = f.Value.GetLength(0);
            int width = f.Value.GetLength(0);
            // new color 
            pixels = new Color[height * width];
            for (int k = 0; k < height; k++)
            {
                for (int l = 0; l < width; l++)
                {
                    if (f.Value[k][l] > max)
                    {
                        max = f.Value[k][l];
                    }

                    if (f.Value[k][l] < min)
                    {
                        min = f.Value[k][l];
                    }
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    normalizedValue = ((float)f.Value[i][j] - min) / (max - min);
                    color.r = normalizedValue;
                    color.g = normalizedValue;
                    color.b = normalizedValue;
                    pixels[index] = color;
                    index++;
                }
            }
            if (featureMapsPixels.ContainsKey(f.Key))
            {
                featureMapsPixels[f.Key] = pixels;
            }
            else
            {
                featureMapsPixels.Add(f.Key, pixels);
            }
        }
        return featureMapsPixels;
    }
    public Dictionary<string, Color[]> getLastLayersPixels()
    {
        Color color = new Color();
        color.a = 1f;
        Dictionary<string, Color[]> featureMapListLastLayersPixels = new Dictionary<string, Color[]>();
        float max = -10000f;
        float min = 10000f;
        foreach (KeyValuePair<string, ushort> f in structHelper.featureMapListLastLayers.Values)
        {
            if (f.Value > max)
            {
                max = f.Value;
            }

            if (f.Value < min)
            {
                min = f.Value;
            }
        }
        float normalizedValue;
        foreach (KeyValuePair<string, ushort> f in structHelper.featureMapListLastLayers.Values)
        {
            Color[] pixels = new Color[1];
            normalizedValue = ((float)f.Value - min) / (max - min);
            color.r = normalizedValue;
            color.g = normalizedValue;
            color.b = normalizedValue;
            pixels[0] = color;
            if (featureMapListLastLayersPixels.ContainsKey(f.Key))
            {
                featureMapListLastLayersPixels[f.Key] = pixels;
            }
            else
            {
                featureMapListLastLayersPixels.Add(f.Key, pixels);
            }
        }
        return featureMapListLastLayersPixels;
    }

}
