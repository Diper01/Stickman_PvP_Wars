using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSquare : MonoBehaviour {

    [SerializeField] MoveDirection moveDirection;
    [SerializeField] float speed;

    private Vector2 direction;
    private RectTransform rectTransform;
    private float upperBorder = 800f;
    private float bottomBorder = -800f;
    private float rightBorder = 1200f;
    private float leftBorder = -1200f;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        switch (moveDirection)
        {
            case MoveDirection.UP:
                direction = Vector2.up;
                break;
            case MoveDirection.DOWN:
                direction = Vector2.down;
                break;
            case MoveDirection.RIGHT:
                direction = Vector2.right;
                break;
            case MoveDirection.LEFT:
                direction = Vector2.left;
                break;           
        }
    }

    private void FixedUpdate()
    {
        rectTransform.localPosition = (Vector2)rectTransform.localPosition + direction * speed;
        switch (moveDirection)
        {
            case MoveDirection.UP:
                if (rectTransform.localPosition.y > upperBorder)                              
                    rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, bottomBorder);                
                break;
            case MoveDirection.DOWN:
                if (rectTransform.localPosition.y < bottomBorder)
                    rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, upperBorder);
                break;
            case MoveDirection.RIGHT:
                if (rectTransform.localPosition.x > rightBorder)
                    rectTransform.localPosition = new Vector2(leftBorder, rectTransform.localPosition.y);
                break;
            case MoveDirection.LEFT:
                if (rectTransform.localPosition.x < leftBorder)
                    rectTransform.localPosition = new Vector2(rightBorder, rectTransform.localPosition.y);
                break;
        }
    }

}


public enum MoveDirection {
    UP,
    DOWN,
    RIGHT,
    LEFT
}