using UnityEngine;
using Cinemachine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class CamController : MonoBehaviour
    {

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public CinemachineVirtualCamera virtualCamera;
        public float MaxCameraDistance;
        public float MinCameraDistance;
        public float targetDistance;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        public CharacterStatsUI UI;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private CinemachineComponentBase componentBase;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        }

        private void Start()
        {
            
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            

        }



        private void LateUpdate()
        {
            CameraRotation();
            CameraDist();
        }

        private void CameraRotation()
        {
            if(UI.UIActive || UI.GameEnd)
            {
                LockCameraPosition = true;
            }
            else
                LockCameraPosition = false;
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * GameManager.CameraSensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * GameManager.CameraSensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void CameraDist()
        {
            if (_input.Scroll > 0)
            {
                //Debug.Log("scroll up");
                targetDistance -= 0.5f;

                if (componentBase is Cinemachine3rdPersonFollow && targetDistance >= MinCameraDistance)
                {
                    (componentBase as Cinemachine3rdPersonFollow).CameraDistance = targetDistance;
                }
                else
                {
                    targetDistance = MinCameraDistance;
                }
            }
            else if (_input.Scroll < 0)
            {
                //Debug.Log("scroll down");
                targetDistance += 0.5f;

                if (componentBase is Cinemachine3rdPersonFollow && targetDistance <= MaxCameraDistance)
                {
                    (componentBase as Cinemachine3rdPersonFollow).CameraDistance = targetDistance;
                }
                else
                {
                    targetDistance = MaxCameraDistance;
                }
            }
        }
    }
}