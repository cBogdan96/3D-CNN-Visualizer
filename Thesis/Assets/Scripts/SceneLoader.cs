using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject toggler;
    public Models modelPicker = new Models();
    public float loadingTime = 10f;
    public float framesPerSecond = 3f;
    public int threads = 2;
    public HashSet<string> layersToGet ;
    public int mode = 1; // 1 for video 2 for webcam  
    public string pathModel;
    public string pathVideo;
    public int inputImageSizeValue = 0;

    public GameObject pathToModel;
    public GameObject theadsSelector;
    public GameObject frameSelector;

    public GameObject recordTimeText;
    public GameObject recordTimeTextCounter;
    public GameObject recordTimeSlider;

    public GameObject pathToVideo;
    public GameObject videoPathText;
    public GameObject inputImageSize;



    public enum Models 
    {
        VGG16 ,
        VGG19 ,
        Xception ,
        DenseNet121,
        DenseNet169,
        DenseNet201,
        MobileNet,
        MobileNetV2, 
        InceptionV3,
        InceptionResNetV2,
        ResNet50,  
        ResNet50V2,  
        ResNet101, 
        ResNet101V2,
        ResNet152,
        ResNet152V2,
        NASNetLarge,
        NASNetMobile
    }

    public void Start()
    {
        //Application.targetFrameRate = 30;
        layersToGet = new HashSet<string>();
    }

    public void LoadScene() {

        pathModel = pathToModel.GetComponent<InputField>().text;
        pathVideo = pathToVideo.GetComponent<InputField>().text;
        if ((int)modelPicker == 18)
        {
            inputImageSizeValue = int.Parse(inputImageSize.GetComponent<InputField>().text.ToString());
        }
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("ExecutionScene");

    }

    public void QuitApplication() {
        Application.Quit();
        //Debug.Log("Am iesit");
    }
    public void setLoadingTime(float time) 
    {
        recordTimeTextCounter.GetComponent<Text>().text = time.ToString() + " s";
        loadingTime = time;
    }

    public void setModel(int model) {
        modelPicker = (Models) model;
        //Debug.Log(modelPicker);
        if ((int)modelPicker == 18)
        {
            pathToModel.SetActive(true);
            inputImageSize.SetActive(true);

        }
        else {
            pathToModel.SetActive(false);
            inputImageSize.SetActive(false);

        }

        dropdowOptions(modelPicker.ToString());
    }

    private void dropdowOptions(String modelPicker)
    {
        switch (modelPicker)
        {
            case ("VGG_16"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("VGG_19"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("Xception"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("DenseNet121"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("DenseNet169"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("DenseNet201"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("MobileNet"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6","9","15","20" });
                break;
            case ("MobileNetV2"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6", "9", "15", "20" });
                break;
            case ("InceptionV3"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("InceptionResNetV2"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("ResNet50"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("ResNet50V2"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("ResNet101"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("ResNet101V2"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("ResNet152"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("ResNet152V2"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("NASNetLarge"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("NASNetMobile"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            case ("18"):
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
            default:
                PopulateDropdown(theadsSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                PopulateDropdown(frameSelector.GetComponent<Dropdown>(), new List<string> { "1", "2", "3", "4", "5", "6" });
                break;
        }

    }

    public void setFps(int value)
    {
        //Debug.Log(int.Parse(frameSelector.GetComponent<Dropdown>().options[value].text));
        framesPerSecond = int.Parse(frameSelector.GetComponent<Dropdown>().options[value].text);
    }

    public void setThreads(int value)
    {
        //Debug.Log(int.Parse(theadsSelector.GetComponent<Dropdown>().options[value].text));
        threads = int.Parse(theadsSelector.GetComponent<Dropdown>().options[value].text);
    }

    public void setVisualizationType(int value)
    {
        mode = value;
        if (mode == 1)
        {
            pathToVideo.SetActive(true);
        }
        else
        {
            pathToVideo.SetActive(false);
        }
    }

    public void setLayersShown(string value) {
        
            if (!layersToGet.Contains(value) && GameObject.Find(value).GetComponent<Toggle>().enabled == true)
            {
                layersToGet.Add(value);
            }
            else 
            {
                layersToGet.Remove(value);    
            }
        //Debug.Log(layersToGet.Count);
    
    }

    void PopulateDropdown(Dropdown list, List<String> elementsList)
    {
        list.ClearOptions();
        list.AddOptions(elementsList);
    }
}
