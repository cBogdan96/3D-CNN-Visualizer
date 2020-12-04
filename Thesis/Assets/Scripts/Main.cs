using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TupleModelType = System.Tuple<System.Collections.Generic.Dictionary<string, UnityEngine.Color[]>, System.Collections.Generic.Dictionary<string, UnityEngine.Color[]>, System.String>;

public class Main : MonoBehaviour
{
    public GameObject featuresTemplate;
    public GameObject featuresPredictionsTemplate;
    public GameObject layerTextData;
    public GameObject exitText;
    public GameObject controlsText;



    private float frameRate = 1f; 
    private float recordTime = 0f; 
    private float framesTimer = 0f;
    private float recordTimer = 0f;
    private int numThreads = 1; 
    private string layersToSee;

    FeatureMapCreator featureMapCreator;
    Connector socketConnector;
    public StructHelper featureMapsObjects = new StructHelper();
    public volatile Dictionary<string, Dictionary<string, ushort[][]>> feature;

    public TupleModelType featureMap;
    public BlockingCollection<TupleModelType> queue = new BlockingCollection<TupleModelType>(360);
    public GameObject backgroundLoadingScreen;
    public SceneLoader sceneLoader;

    public string model;
    public string modelPath;
    public string videoPath;
    public int visualisationMode;
    public int inputImageSizeValue;

    void Start()
    {

        //TODO: check camera to change on button click
        //TODO: add loading screen
        //FeatureMapCreator.predictions_labels = loadLabelsImageNet();
        featureMapCreator = new FeatureMapCreator();
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        socketConnector = new Connector();
        socketConnector.establishConnection(60000); // 60000

        model = getModelNameAndSize(sceneLoader.modelPicker.ToString()); // get model type
        visualisationMode = sceneLoader.mode;
        inputImageSizeValue = sceneLoader.inputImageSizeValue;
        
        if (model.Equals("17"))
        {
            modelPath = sceneLoader.pathModel;
        }
        else {
            modelPath = "None";
        }

        if (inputImageSizeValue != 0)
        {
            model +=" " + inputImageSizeValue.ToString();
        }

        if (visualisationMode == 1)
        {
            videoPath = sceneLoader.pathVideo;
        }
        else
        {
            videoPath = "None";
        }

        recordTime = sceneLoader.loadingTime;  /// get loading time 
        frameRate = 1 /sceneLoader.framesPerSecond; // get framerate update
        numThreads = sceneLoader.threads;  // get number of threads

        HashSet<string>.Enumerator layers = sceneLoader.layersToGet.GetEnumerator();   /// get layers to see
        while (layers.MoveNext()) 
        {
            layersToSee = layersToSee + " " + layers.Current;
        }
        byte[] start_server = Encoding.ASCII.GetBytes("start " + numThreads + " "+ model + " "+ modelPath +" " + visualisationMode + " " + videoPath + " " + sceneLoader.framesPerSecond + " "+ layersToSee + "\n"); 

        var z = socketConnector.SendAndReceive(start_server);
        Debug.Log("I started deserialization");
        feature = MessagePackSerializer.Deserialize<Dictionary<string, Dictionary<string, ushort[][]>>>(z);
        Debug.Log("I finished deserialization");
        featureMapsObjects = featureMapCreator.makeFeature(featuresTemplate,featuresPredictionsTemplate, feature);

        for (int x = 1; x <= numThreads; x++)
        {
            new ThreadComponent(x, this, featureMapCreator).init();
            Thread.Sleep(1000);
        }

    }

    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if (hitInfo.transform.gameObject.name.Contains("fmap")) {

                    layerTextData.GetComponent<Text>().text = hitInfo.transform.gameObject.name;
                }
            }
            else
            {

            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //byte[] stop_server = Encoding.ASCII.GetBytes("stop " + numThreads + "\n"); /// start server 
            //var z = socketConnector.SendAndReceive(stop_server);
            Application.Quit();
            //Debug.Log("Am iesit");
        }
        recordTimer += Time.deltaTime;

        if (recordTimer < recordTime)
        {
            return;
        }
        if (backgroundLoadingScreen.activeSelf == true) {
            backgroundLoadingScreen.SetActive(false);
        }
        if (exitText.activeSelf == false)
        {
            exitText.SetActive(true);
        }
        if (controlsText.activeSelf == false)
        {
            controlsText.SetActive(true);
        }
        if (layerTextData.activeSelf == false)
        {
            layerTextData.SetActive(true);
        }
        framesTimer += Time.deltaTime;
        if (framesTimer < frameRate)
        {
            return;
        }

        framesTimer = 0f;

        bool featureExists  = this.queue.TryTake( out this.featureMap);
        //Debug.Log("iau element iar lungimea ramasa este" + this.featureMap);
        if (!featureExists) {
            return;
        }


        if (featureMap.Item1.Count != 0)
        {
            lock (featureMap.Item1)
            {
                Debug.Log("pixels update");
                foreach (KeyValuePair<string, Color[]> f in featureMap.Item1)
                {
                    featureMapsObjects.convLayers[f.Key].GetComponent<PlaneCreator>().updatePlane(f.Value);
                }
                this.featureMap.Item1.Clear();
            }
        }
        lock (featureMap.Item2)
        {
            var index = 0;
            var indexLast = 0;
            var maxTest = -3.0;
            var minTest = 3.0;
            if (featureMap.Item2.Count != 0)
            {
                foreach (KeyValuePair<string, Color[]> f in featureMap.Item2)
                {
                    if (f.Key.Contains("predictions"))
                    {
                        if (f.Value[0].b > maxTest)
                        {
                            maxTest = f.Value[0].b;
                        }
                        if (f.Value[0].b < minTest)
                        {
                            minTest = f.Value[0].b;
                        }
                        featureMapsObjects.LastLayersPredictions[f.Key].GetComponent<CubeCreator>().updateCube(f.Value);
                        index++;
                    }
                    else
                    {
                        featureMapsObjects.LastLayersPredictions[f.Key].GetComponent<CubeCreator>().updateCube(f.Value);
                        indexLast++;
                    }
                }
                this.featureMap.Item2.Clear();
            }
        }
    }

    public void updateFeatureMapPixels(Dictionary<string, Color[]> featureMapPixels, Dictionary<string, Color[]> featureMapPixelsLastLayers, String time)
    {
        TupleModelType tuple = new TupleModelType(featureMapPixels, featureMapPixelsLastLayers, time);
        this.queue.Add(tuple);
        this.queue.OrderByDescending(key => key.Item3);
    }
    
    public string getModelNameAndSize(string modelName)
    {

        switch (modelName)
        {
            case ("VGG16"):
                return "0 224";
            case ("VGG19"):
                return "1 224";
            case ("Xception"):
                return "2 299";
            case ("DenseNet121"):
                return "3 224";
            case ("DenseNet169"):
                return "4 224";
            case ("DenseNet201"):
                return "5 224";
            case ("MobileNetV2"):
                return "6 224";
            case ("InceptionV3"):
                return "7 299";
            case ("InceptionResNetV2"):
                return "8 299";
            case ("ResNet50"):
                return "9 224";
            case ("ResNet50V2"):
                return "10 224";
            case ("ResNet101"):
                return "11 224";
            case ("ResNet101V2"):
                return "12 224";
            case ("ResNet152"):
                return "13 224";
            case ("ResNet152V2"):
                return "14 224";
            case ("NASNetLarge"):
                return "15 331";
            case ("NASNetMobile"):
                return "16 224";
            case ("17"):
                return "17";
            default: return "0 224";
        }
    }
}
