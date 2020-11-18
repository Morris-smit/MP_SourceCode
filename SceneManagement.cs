using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Dictionary<string, GameObject> shipList = PersistantDataManager.Instance.GetPlayerHangar().GetHangar();
        for (int i = 0; i < shipList.Count; i++)
        {
            Ship ship = shipList.ElementAt(i).Value.GetComponent<Ship>();

            //ship.resetHealth();
            ship.gameObject.SetActive(false);
        }
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
