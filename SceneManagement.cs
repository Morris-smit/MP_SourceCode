using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToScene(string scene)
    {
        DontDestroyOnLoad(PersistantDataManager.Instance.GetPlayerHangar());
        List<GameObject> shipList = PersistantDataManager.Instance.GetPlayerHangar().GetFleetList();
        for (int i = 0; i < shipList.Count; i++)
        {
            shipList[i].SetActive(false);
        }
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
