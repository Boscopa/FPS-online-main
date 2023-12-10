using UnityEngine;
using Photon.Pun;

namespace AstronautPlayer
{
    public class AstronautPlayer : MonoBehaviourPunCallbacks
    {
        private Animator anim;
        private CharacterController controller;

        public float speed = 600.0f;
        public float turnSpeed = 400.0f;
        private Vector3 moveDirection = Vector3.zero;
        public float gravity = 20.0f;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();

            if (!photonView.IsMine)
            {
                // Disable the script if it's not controlling this instance
                enabled = false;
            }
        }

        void Update()
        {
            if (!photonView.IsMine)
                return;

            if (Input.GetKey("w"))
            {
                anim.SetInteger("AnimationPar", 1);
            }
            else
            {
                anim.SetInteger("AnimationPar", 0);
            }

            if (controller.isGrounded)
            {
                moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
            }

            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
            controller.Move(moveDirection * Time.deltaTime);
            moveDirection.y -= gravity * Time.deltaTime;

            // Send the position and rotation to other players
            photonView.RPC("SyncMovement", RpcTarget.Others, transform.position, transform.rotation);
        }

        [PunRPC]
        void SyncMovement(Vector3 newPosition, Quaternion newRotation)
        {
            // Update position and rotation for remote players
            transform.position = newPosition;
            transform.rotation = newRotation;
        }
    }
}
