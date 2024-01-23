using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragResizeContent : MonoBehaviour
{

    private float clickPos;
    private float currentPos;
    public RectTransform contentToDrag;
    public RectTransform contentToAdapt;
    private float initLeft;
    private float initRight;
    private void OnMouseDown()
    {

        clickPos = Input.mousePosition.x;
        initLeft = contentToDrag.offsetMin.x;
        initRight = contentToAdapt.offsetMax.x;
    }

    private void OnMouseDrag()
    {
        currentPos = Input.mousePosition.x;
        float dist = currentPos - clickPos;

        contentToDrag.offsetMin = new Vector2(initLeft + dist, contentToDrag.offsetMin.y);
        contentToAdapt.offsetMax = new Vector2(initRight + dist, contentToAdapt.offsetMax.y);

    }

    private void OnMouseExit()
    {
        
    }

    private void OnMouseUp()
    {
        
    }
    private void OnMouseEnter()
    {
        
    }
  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
