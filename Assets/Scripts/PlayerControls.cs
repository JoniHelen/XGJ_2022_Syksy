using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] LayerMask raycastMask;
    [SerializeField] float PieceSpeed = 10f;

    private bool MouseDown = false;
    private bool GrabbedPuzzlePiece = false;

    private PuzzlePiece puzzlePiece = null;

    void Update()
    {
        if (puzzlePiece != null)
            if (puzzlePiece.IsInPlace) puzzlePiece = null;

        if (MouseDown) {
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit[] hitInfo = Physics.RaycastAll(mouseRay, 20f, raycastMask);
            
            foreach (RaycastHit hit in hitInfo) {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("PuzzlePiece") && puzzlePiece == null && !GrabbedPuzzlePiece) {
                    puzzlePiece = hitObject.GetComponent<PuzzlePiece>();
                    if (puzzlePiece.IsInPlace) puzzlePiece = null;
                    GrabbedPuzzlePiece = true;
                }

                if (hitObject.CompareTag("RayCatcher") && puzzlePiece != null)
                    puzzlePiece.transform.position = Vector3.Lerp(puzzlePiece.transform.position, hit.point, Time.deltaTime * PieceSpeed);
            }
        }
        else {
            puzzlePiece = null;
            GrabbedPuzzlePiece = false;
        }
    }

    public void PlayerMouseClick(InputAction.CallbackContext ctx) {
        if (ctx.started) MouseDown = true;
        if (ctx.canceled) MouseDown = false;
    }
}
