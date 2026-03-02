using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LightSabre : MonoBehaviour
{
    public XRNode controllerNode = XRNode.LeftHand; // Or LeftHand
    private bool isActive = false; // Flag to check if the light saber is active
    public GameObject laser;
    private Vector3 fullSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool lastGripState = false;

    private AudioSource source;
    public AudioClip sabreMoving; // Assign this in the inspector with your sound effect
    public AudioClip sabreOn; // Assign this in the inspector with your sound effect
    public AudioClip sabreHum; // Assign this in the inspector with your sound effect

    InputDevice device;
    private List<InputDevice> inputDevices = new List<InputDevice>();
    bool gripPressed = false;
    
    public UnityEngine.InputSystem.InputActionAsset inputActions;
    private UnityEngine.InputSystem.InputAction leftTrigger;
    private UnityEngine.InputSystem.InputAction leftGrip;

    private Vector3 laserSize;

    public GameObject Reticle;
    public Camera MainCamera;
    
    public GameObject ShootPoint;
    private Ray ray;
    private RaycastHit hit;

    public GameObject ControllerObject;
    private ActionBasedController controller = null;
    public float strongVibrate = 0.75f;
    public float weakVibrate = 0.25f;
    
    public int meleeDamage = 10;
    public int shootDamage = 3;


    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1.0f; // Set spatial blend to 3D sound
        source.volume = 0.5f; // Set volume to a reasonable level
        // laser = transform.Find("SingleLine-LightSaber").gameObject;
        laserSize = laser.GetComponent<VolumetricLines.VolumetricLineBehavior>().StartPos;
        fullSize = laserSize;
        laser.GetComponent<VolumetricLines.VolumetricLineBehavior>().StartPos = new Vector3(0,0,0);
        //Keep laser pulled in until the player presses the button
        // device = InputDevices.GetDeviceAtXRNode(controllerNode);
        // print(device.name);
        leftTrigger = inputActions.FindActionMap("XRI LeftHand Interaction").FindAction("Activate Value");
        leftGrip = inputActions.FindActionMap("XRI LeftHand Interaction").FindAction("Select");
        controller = ControllerObject.GetComponent<ActionBasedController>();
        print(controller.name);
    }

    // Update is called once per frame
    void Update()
    {
        laserSize = laser.GetComponent<VolumetricLines.VolumetricLineBehavior>().StartPos;
        inputDevices.Clear();

        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Left , inputDevices);
        
        // foreach (var device in inputDevices)
        // {
        //     Debug.Log(string.Format("Device name '{0}' has characteristics '{1}'", device.name, device.characteristics.ToString()));
            
        //     if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out gripPressed) && gripPressed)
        //     {
        //         Debug.Log("Grip button is pressed.");
        //     }
        //     else
        //     {
        //         gripPressed = false;
        //     }
        // }

        if (leftGrip.ReadValue<float>() > 0)
        {
            gripPressed = true;
        }
        else
        {
            gripPressed = false;
        }
        
        if (leftTrigger.triggered)
        {
            VibrateWeak(.2f);
            if (Physics.Raycast(ShootPoint.transform.position,
                                transform.TransformDirection(Vector3.forward),
                                out hit, 100))
            {
                Transform objectHit = hit.transform;

                // ✅ Destroy moving targets
                TargetMover targetMover = objectHit.GetComponentInParent<TargetMover>();
                if (targetMover != null)
                {
                    Debug.Log("TargetMover found on: " + targetMover.name);
                    targetMover.TakeHit();
                }
                // ✅ Existing enemy damage code...
                if (objectHit.CompareTag("Enemy") || objectHit.CompareTag("Melee") || objectHit.CompareTag("Boss"))
                {
                    if (objectHit.TryGetComponent(out Enemy_Health enemyHealth))
                        enemyHealth.TakeDamage(shootDamage);
                    else if (objectHit.TryGetComponent(out EnemyBossAI bossHealth))
                        bossHealth.TakeDamage(shootDamage);
                    else if (objectHit.TryGetComponent(out EnemyMeleeAI meleeHealth))
                        meleeHealth.TakeDamage(shootDamage);
                }
            }
        }


        // GetInput(gripPressed);
        LaserControl(gripPressed);

        // device.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);

        // Detect "button down" event (implement your button state logic)
        // Laser control logic
        // ...

        // Get controller velocity
        Vector3 velocity;
        if (device.TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity))
        {
            if (isActive = true && velocity.magnitude > 6f && sabreMoving != null)
            {
                source.PlayOneShot(sabreMoving);
            }
            else if(source.isPlaying == false)
            {
                source.PlayOneShot(sabreHum);
            }
        }

        ray = new Ray(ShootPoint.transform.position, transform.forward);
        if (Physics.Raycast(ShootPoint.transform.position, transform.TransformDirection(Vector3.forward), out hit, 100, LayerMask.GetMask("Default")))
        { 
            Reticle.transform.position = hit.point;
            Reticle.transform.LookAt(Camera.main.transform.position);
            // float distance = Vector3.Distance(hit.point, transform.position);
            // Reticle.transform.localScale = new Vector3(1-distance/10,1-distance/10,1);
        }
        // else
        // { 
        //     Reticle.transform.position = Camera.main.WorldToScreenPoint(ray.GetPoint(10));
        //     Reticle.transform.localScale = new Vector3(1,1,1);
        // }
        // Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 100);
        
        

    }

    private void GetInput(bool gripPressed)
    {
        if (gripPressed && !lastGripState)
        {
            isActive = !isActive;
            laser.SetActive(isActive);
        }
    }

    private void LaserControl(bool gripPressed)
    {
        // lastGripState = gripPressed;

        // Animate laser scaling
        if (gripPressed && laserSize.y < fullSize.y)
        {
            Debug.Log("laser growing");
            laser.SetActive(true);
            laser.GetComponent<VolumetricLines.VolumetricLineBehavior>().StartPos += new Vector3 (0f,4f,0f);
        }
        else if (gripPressed == false && laserSize.y > 0f)
        {
            Debug.Log("laser shrinking");
            laser.GetComponent<VolumetricLines.VolumetricLineBehavior>().StartPos += new Vector3 (0f,-4f,0f);
        }
        else if (gripPressed == false)
        {
            Debug.Log("laser disabled");
            laser.SetActive(false);
        }
    }

    private void Vibrate(float amplitude, float duration)
    {
        // OpenVR currently only supports amplitude
        controller.SendHapticImpulse(amplitude, duration);
    }

    private void VibrateWeak(float duration)
    {
        controller.SendHapticImpulse(weakVibrate, duration);
    }

    private void VibrateStrong(float duration)
    {
        controller.SendHapticImpulse(strongVibrate, duration);
    }


}
