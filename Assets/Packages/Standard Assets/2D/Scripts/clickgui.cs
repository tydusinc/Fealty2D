using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clickgui : MonoBehaviour {
    public Canvas canvy;
    public Text text;
    public Transform player;
    public Transform This;
    // Use this for initialization
    void Start () {
        canvy.enabled = false;
       
	}
	
	// Update is called once per frame
	void Update () {

        if (player.position.x > This.position.x + 5 || player.position.x < This.position.x - 5)

        {
            canvy.enabled = false;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Pressed left click, casting ray.");
                CastRay();

            }
        }
    }
    void CastRay() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.gameObject.name == "merchgui")
            {
                canvy.enabled = true;
                text.text = "Hello! What would you like to buy from my shop sir ?";

            }
        }
    }

   
}
