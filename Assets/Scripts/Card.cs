using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    private int value;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetGameMode() == GameManager.GameMode.ChooseCard && Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 origin = new Vector2(worldMousePos.x, worldMousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                // Debug.Log(this.value + " clicked");
                GameManager.OnCardChosen(this.gameObject);
            }
        }
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public int GetValue()
    {
        return this.value;
    }
}
