using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public bool gridEnabled = false;
    public GameObject optionsPanel;
    public WormholeScript wormhole;
    public Texture[] skyboxes;
    public int skybox1 = 0;
    public int skybox2 = 1;
    public GameObject optionsLabel;

    // Start is called before the first frame update
    void Start()
    {
        wormhole.SkyboxTexture1 = skyboxes[skybox1];
        wormhole.SkyboxTexture2 = skyboxes[skybox2];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (gridEnabled)
            {
                gridEnabled = false;
            }
            else
            {
                gridEnabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.Mouse3))
        {
            Cursor.visible = false;
        }
        /*
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (optionsLabel.activeSelf)
            {
                optionsLabel.SetActive(false);
            }
            else
            {
                optionsLabel.SetActive(true);
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (optionsPanel.activeSelf)
            {
                optionsPanel.SetActive(false);
            }
            else
            {
                optionsPanel.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            skybox1++;
            if (skybox1 >= skyboxes.Length)
            {
                skybox1 = skyboxes.Length - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            skybox1--;
            if (skybox1 < 0)
            {
                skybox1 = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            skybox2++;
            if (skybox2 >= skyboxes.Length)
            {
                skybox2 = skyboxes.Length - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            skybox2--;
            if (skybox2 < 0)
            {
                skybox2 = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            wormhole.SkyboxTexture1 = skyboxes[skybox1];
            wormhole.SkyboxTexture2 = skyboxes[skybox2];
        }
        wormhole.RayTracingShader.SetBool("gridEnabled", gridEnabled);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.I))
        {
            wormhole.Length += 0.01f;
        }
        if (Input.GetKey(KeyCode.K) && wormhole.Length > 0.02f)
        {
            wormhole.Length -= 0.01f;
        }
    }
}
