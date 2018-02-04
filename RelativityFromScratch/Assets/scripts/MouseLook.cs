using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    private Vector2 mouseLook;
    private Vector2 smoothV;
    [SerializeField] private float sensitivity;
    [SerializeField] private float smoothing;
    private bool gravityFlipped;
    private float zRotation;

    private PlayerMovement playerMovement;
    private GameObject player;

	// Use this for initialization
	void Start () {
        player = this.transform.parent.gameObject;
        playerMovement = player.GetComponent<PlayerMovement>();
	}
	
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Physics.gravity *= -1;
            gravityFlipped = !gravityFlipped;
            playerMovement.jumpSpeed *= -1;
        }
    }

	// Update is called once per frame
	void FixedUpdate () {

            Vector2 mouseDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouseDirection = Vector2.Scale(mouseDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, mouseDirection.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, mouseDirection.y, 1f / smoothing);

            mouseLook += smoothV;



        if (gravityFlipped)
        {


            if (zRotation > 179)
            {
                zRotation = 180;

            }
            else
            {
               zRotation += 180 * (Time.deltaTime*2);
            }

            mouseLook.y = Mathf.Clamp(mouseLook.y, -70f, 70f);
            player.transform.localRotation = Quaternion.AngleAxis(-mouseLook.x, Vector3.up) * Quaternion.AngleAxis(zRotation, Vector3.forward);
            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        }
        else
        {
            if (player.transform.localRotation.z == 0)
            {
                mouseLook.y = Mathf.Clamp(mouseLook.y, -70f, 70f);
                transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
                player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
            }
            else
            {
                if (zRotation < 0)
                {
                    zRotation = 0;
                }
                else
                {
                    zRotation -= 180 * (Time.fixedDeltaTime*2);
                }


                mouseLook.y = Mathf.Clamp(mouseLook.y, -70f, 70f);
                player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up) * Quaternion.AngleAxis(zRotation, Vector3.forward);
                transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            }

        }
    }
}
