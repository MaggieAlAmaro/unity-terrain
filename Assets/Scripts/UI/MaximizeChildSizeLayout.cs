using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaximizeChildSizeLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height
    }


    public FitType fit;
    public int rows;
    public int cellNumber;
    private Vector2 cellSize;


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        float sqrRt = Mathf.Sqrt(transform.childCount);
        cellNumber = Mathf.CeilToInt(sqrRt);

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float _cellSize = 0;
        if (parentWidth <= parentHeight)
            _cellSize = parentWidth / (float)cellNumber;
        else
            _cellSize = parentHeight / (float)cellNumber;

        cellSize.x = _cellSize;
        cellSize.y = _cellSize;

        int colCount = 0;
        int rowCount = 0;


        //TODO: Control column size


        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / cellNumber;
            colCount = i % cellNumber;

            var item = rectChildren[i];

            var xPos = (cellSize.x * colCount);
            var yPos = (cellSize.y * rowCount);


            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }


    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }

}
