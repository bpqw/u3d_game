using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookController : MonoBehaviour
{

    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private PlayerInput playerInput;
    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        HandleRotate();
        HandleFOVChange();
    }

    private void HandleRotate()
    {
        Vector2 cameraRotateVec = playerInput.GetCameraRotateNormalized();

        freeLook.m_XAxis.m_InputAxisValue = cameraRotateVec.x;
        freeLook.m_YAxis.m_InputAxisValue = cameraRotateVec.y;
    }

    private void HandleFOVChange()
    {
        // TODO: You should bind the scroll of your mouse(������) in the Input Actions
        // Set an Action 'ScrollUp', Action Type is Value, Control Type is Axis, Binding 'Mouse->Scroll->Up'
        // Set an Action 'ScrollDown'
        // Get the input value of the scroll, change the Field Of View accordingly
        // 'm_Lens' and 'FieldOfView' may help
        
    }
}
