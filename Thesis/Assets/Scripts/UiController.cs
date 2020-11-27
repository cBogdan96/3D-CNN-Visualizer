using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;
    public GameObject cam4;
    public Text text;

    // How quickly the camera moves

    public float panSpeed = 20f;

    // How quickly the camera zooms

    public float zoomSpeed = 50f;

    // The minimum distance of the mouse cursor from the screen edge required to pan the camera

    public float borderWidth = 10f;

    // Boolean to control if moving the mouse within the borderWidth distance will pan the camera

    public bool edgeScrolling = true;

    //Private Variables

    // Minimum distance from the camera to the camera target

    private float zoomMin = 11.0f;

    // Maximum distance from the camera to the camera target

    private float zoomMax = 49.0f;

    void Update()
    {

        //text.transform.position = cam1.transform.position + cam1.transform.forward * 3;
        switchCamera();
        Movement();
        Zoom();
    }

    void Movement()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        if (Input.GetKey("s") || edgeScrolling == true && Input.mousePosition.y >= Screen.height - borderWidth)
        {
            pos += forward * panSpeed * 3 * Time.deltaTime;
        }

        if (Input.GetKey("w") || edgeScrolling == true && Input.mousePosition.y <= borderWidth)
        {
            pos -= forward * panSpeed * 3 * Time.deltaTime;
        }

        if (Input.GetKey("a") || edgeScrolling == true && Input.mousePosition.x >= Screen.width - borderWidth)
        {
            pos += right * panSpeed * 3 * Time.deltaTime;
        }

        if (Input.GetKey("d") || edgeScrolling == true && Input.mousePosition.x <= borderWidth)
        {
            pos -= right * panSpeed * 3 * Time.deltaTime;
        }

        if (Input.GetKey("q"))
        {
            pos += Vector3.up * panSpeed * 3 * Time.deltaTime;
        }
        if (Input.GetKey("e"))
        {
            pos += -Vector3.up * panSpeed * 3 * Time.deltaTime;
        }
        transform.position = pos;
    }
    void Zoom()
    {
        Vector3 camPos = transform.position;

        float distance = Vector3.Distance(transform.position, transform.position);

        // When we scroll our mouse wheel up, zoom in if the camera is not within the minimum distance (set by our zoomMin variable)

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && distance > zoomMin)

        {
            camPos += transform.forward * zoomSpeed * 2 * Time.deltaTime;
        }
        // When we scroll our mouse wheel down, zoom out if the camera is not outside of the maximum distance (set by our zoomMax variable)

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && distance < zoomMax)
        {
            camPos -= transform.forward * zoomSpeed * 2 * Time.deltaTime;

        }
        // Set the camera's position to the position of the temporary variable
        transform.position = camPos;
    }



    private void switchCamera()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            cam1.tag = "MainCamera";
            cam2.tag = "Untagged";
            cam3.tag = "Untagged";
            cam4.tag = "Untagged";

            cam1.SetActive(true);
            cam2.SetActive(false);
            cam3.SetActive(false);
            cam4.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            cam1.tag = "Untagged";
            cam2.tag = "MainCamera";
            cam3.tag = "Untagged";
            cam4.tag = "Untagged";

            cam1.SetActive(false);
            cam2.SetActive(true);
            cam3.SetActive(false);
            cam4.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {

            cam1.tag = "Untagged";
            cam2.tag = "Untagged";
            cam3.tag = "MainCamera";
            cam4.tag = "Untagged";

            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(true);
            cam4.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //GameObject.Find("BackCamera").GetComponent<Camera>().enabled = true;
            //GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;
            cam1.tag = "Untagged";
            cam2.tag = "Untagged";
            cam3.tag = "Untagged";
            cam4.tag = "MainCamera";

            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(false);
            cam4.SetActive(true);
        }
    }




}
