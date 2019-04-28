using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeNextScene : MonoBehaviour
{
	public static string NextSceneToUse = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextScene()
    {
	    if (NextSceneToUse == "")
	    {
		    SceneManager.LoadScene("FirstLevel");
		}

	    else
	    {
		    SceneManager.LoadScene(NextSceneToUse);
	    }
	}
}
