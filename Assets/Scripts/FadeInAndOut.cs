using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class FadeInAndOut : MonoBehaviour {

    // Group to hold multiple panels
    public CanvasGroup[] fadeCanvasGroups;
    public float fadinspeed;
    public float waitTime;

    // The panel that needs to be worked on
    private int currentPanel;

    // Singleton
    public static FadeInAndOut instance;

    // Use this for initialization
    void Start() {
        if (instance == null)
        {
            instance = this;
            Debug.Log("Singleton is made FadeInAndOut");
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // loads next scene or start Ienumrator for the next panel
    public void NextPanel()
    {
        if (currentPanel >= fadeCanvasGroups.Length)
        {
            return;
        }
        StartCoroutine(FadeIn(fadeCanvasGroups[currentPanel]));
    }

    public void Reset()
    {
        currentPanel = 0;
    }
	
	IEnumerator FadeIn(CanvasGroup Panel)
    {
        Panel.alpha = 0;
        while ( Panel.alpha != 1)
        {
            Panel.alpha += fadinspeed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        while (Panel.alpha != 0)
        {
            Panel.alpha -= fadinspeed * Time.deltaTime;
            yield return null;
        }
        currentPanel++;
        NextPanel();
    }
}
