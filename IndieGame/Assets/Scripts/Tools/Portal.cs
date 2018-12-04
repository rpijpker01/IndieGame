using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private bool GoToHubPortal = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameController.questProgress >= 1)
        {
            RaycastHit raycastHit = new RaycastHit();
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit);
            if (raycastHit.transform.gameObject == this.gameObject)
            {
                if (GoToHubPortal)
                {
                    GameController.GoToHub();
                }
                else
                {
                    GameController.GoToLevel();
                }
            }
        }
    }
}
