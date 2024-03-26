using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTestManager : MonoBehaviour
{
    //[SerializeField] private float viewNumber;
    [SerializeField] private ViewManager cameraFunctionality;
    [SerializeField] private List<GameObject> terrainObjects;
    private int activeTerrainIndex;
    void Start()
    {
        activeTerrainIndex = 0;
        for (int i = 1; i < terrainObjects.Count; i++)
        {
            terrainObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageInput();
    }



    void SetActiveTerrainByIndex(int terrainIndex)
    {
        //terrainObjects[terrainIndex].GetComponent<TerrainManager>().Height;

        if (terrainObjects[terrainIndex].activeSelf != true)
            terrainObjects[terrainIndex].SetActive(true);

        for (int i = 0; i < terrainObjects.Count; i++)
        {
            if (i == terrainIndex)
                continue;
            terrainObjects[i].SetActive(false);
        }
    }
    void ManageInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cameraFunctionality.SetRotationAngle(Mathf.PI *2 /3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cameraFunctionality.SetRotationAngle(Mathf.PI * 2 *2/ 3);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cameraFunctionality.SetRotationAngle(Mathf.PI);

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraFunctionality.ToggleRotation();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            activeTerrainIndex = Mathf.Clamp(activeTerrainIndex + 1, 0, terrainObjects.Count-1);
            SetActiveTerrainByIndex(activeTerrainIndex);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            activeTerrainIndex = Mathf.Clamp(activeTerrainIndex - 1, 0, terrainObjects.Count-1);
            SetActiveTerrainByIndex(activeTerrainIndex);
        }
    }
}
