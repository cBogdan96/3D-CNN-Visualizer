using System.Collections.Generic;

public class StructHelper 
{

    public Dictionary<string, KeyValuePair<string, ushort[][]>> featureMapList ;
    public Dictionary<string, KeyValuePair<string, ushort>> featureMapListLastLayers;

    public Dictionary<string, PlaneCreator> convLayers;
    public Dictionary<string, CubeCreator> LastLayersPredictions;
}
