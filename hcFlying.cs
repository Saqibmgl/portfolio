using UnityEngine;
using UnityEngine.Events;
using Invector.vCharacterController;
using System.Collections;
namespace Invector.vCharacterController.vActions
{
    

    [vClassHeader(" Flying Action")]
    public class hcFlying : vMonoBehaviour
    {
        #region Flying Variables

        [vEditorToolbar("Settings")]

        public string enterFlyAnimation = "EnterFly";
        public string exitFlyAnimation = "LandOnGround";
        public float enterFlyAnimationCrossfade = 0.15f;

        [Tooltip("Distance from Ground  to start flying")]
        public float distanceFromGroundToStartFlying = 1f;
        [Tooltip("Distance from Ground  to end flying")]
        public float distanceFromGroundToStopFlying = 0.5f;

        [Tooltip("Speed to fly forward")]
        public float flyForwardSpeed = 15f;

        [Tooltip("Speed to fly sprint forward")]
        public float flySprintForwardSpeed = 20f;
        [Tooltip("Speed to rotate the character")]
        public float flyRotationSpeed = 4f;
        [Tooltip("Smooth value for the character movement")]
        public float flyMovementSmooth = 2f;
        [Tooltip("Smooth value for the character animation transition")]
        public float flyAnimationSmooth = 1f;
        [Tooltip("Smooth value for the character movement up and down")]
        public float flyUpDownSmooth = 0.5f;

        [vHelpBox("! Assign a curve here, otherwise the character won't move up or down !")]
        public AnimationCurve updownSmoothCurve;

        [Tooltip("Speed to fly up")]
        public float flyUpSpeed = 10f;
        [Tooltip("Speed to fly down")]
        public float flyDownSpeed = -10f;
        [Tooltip("Increase the radius of the capsule collider to avoid enter walls")]
        public float colliderRadius = .5f;
        [Tooltip("Increase the radius of the capsule collider to avoid enter walls")]
        public float colliderHeight = .5f;
      

        [Header("Health/Stamina Consuption")]
        [Tooltip("Leave with 0 if you don't want to use stamina consuption")]
        public float stamina = 15f;
        [Tooltip("How much health will drain after all the stamina were consumed")]
        public int healthConsumption = 1;
        public GameObject flyBtn;
        public GameObject jmpBtn;
        public GameObject ExitflyBtn;
        public GameObject joystickBtn;
        public GameObject attackBtns;
        public GameObject flyDownBtn;
        public GameObject flyUPBtn;
        public GameObject flyRunBtn;
        public GameObject flyFireBtn;

        public GameObject[] particles;

        [vEditorToolbar("Inputs")]
        [Tooltip("Input to make the character start flying")]
        public GenericInput startFlyButton = new GenericInput("F", "A", "A");
        [Tooltip("Input to make the character stop flying")]
        public GenericInput exitFlyButton = new GenericInput("C", "A", "A");
        [Tooltip("Input to make the character go up")]
        public GenericInput flyUpInput = new GenericInput("UpArrow", "X", "X");
        [Tooltip("Input to make the character go down")]
        public GenericInput flyDownInput = new GenericInput("DownArrow", "Y", "Y");

        [Tooltip("Input to make the character go down")]
        public GenericInput flySprintInput = new GenericInput("LeftShift", "Y", "Y");

        [vEditorToolbar("Events")]
        public UnityEvent OnEnterFly;
        public UnityEvent OnExitFly;
        public UnityEvent OnFlying;

        [vEditorToolbar("Debug"), Tooltip("Debug Mode will show the current behaviour at the console window")]
        public bool debugMode;

        [vReadOnly(false), SerializeField]
        public bool isFlyingState;

        protected bool hasStartedFlying;
        protected bool isFlyingSprintState;
        protected vShooterMeleeInput tpInput;
        protected float flyUpInterpolate;
        protected float flyDownInterpolate;

        protected float timer;
        protected float originalMoveSpeed;
        protected float originalRotationSpeed;
        protected float originalMovementSmooth;
        protected float originalAnimationSmooth;

        [Space(10)]
        public bool tut;
        #endregion

        protected void Start()
        {

            tpInput = GetComponentInParent<vShooterMeleeInput>();
            if (tpInput)
            {
                tpInput.onFixedUpdate -= UpdateFlyingBehavior;
                tpInput.onFixedUpdate += UpdateFlyingBehavior;
            }
        }

        IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed)
        {
            float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
            float t = 0;
            while (t <= 1.0f)
            {
                t += step; 
                objectToMove.position = Vector3.Lerp(a, b, t); 
                yield return new WaitForFixedUpdate();         
            }
            objectToMove.position = b;

          
        }

       
        bool CheckIfCanFly()
        {
           
            return tpInput.cc.groundDistance > distanceFromGroundToStartFlying && !tpInput.cc.isGrounded && !isFlyingState;

           

        }

        protected virtual void UpdateFlyingBehavior()
        {
            if (CheckIfCanFly() && !isFlyingState && PlayerPrefs.GetInt("Jump") == 1)
            {
                flyBtn.SetActive(true);
                jmpBtn.SetActive(false);
               

            }
                
            else if(PlayerPrefs.GetInt("Jump") == 1)
            {
                flyBtn.SetActive(false);
                jmpBtn.SetActive(true);
               
            }
               


            if (startFlyButton.GetButtonDown() && CheckIfCanFly() )
            {

                tpInput.cc.animator.CrossFadeInFixedTime(enterFlyAnimation, enterFlyAnimationCrossfade);
                EnterFlyState();
                StartCoroutine(MoveFromTo(transform, transform.position, transform.position + transform.up * 6f, 5));
                jmpBtn.SetActive(false);
                if (PlayerPrefs.GetInt("Jump") == 1)
                {
                    ExitflyBtn.SetActive(true);
                }


            }
           

            if (isFlyingState && exitFlyButton.GetButtonDown())
            {
                ExitFlyState();
               
    }

            OnFlyState();

        }

       

        protected virtual void OnFlyState()
        {

            if (tpInput.cc.isDead)
            {
                ExitFlyState();
                return;
            }
            if  (!hasStartedFlying)  return;

            if (isFlyingState)
            {
                jmpBtn.SetActive(false);
                //joystickBtn.SetActive(false);
                attackBtns.SetActive(false);

                if (PlayerPrefs.GetInt("Buttotns") == 1)
                {
                    
                    flyUPBtn.SetActive(true);
                    flyDownBtn.SetActive(true);
                    //flyRunBtn.SetActive(true);
                    //flyFireBtn.SetActive(true);
                }
                

                isFlyingSprintState = flySprintInput.GetButton();

                FlyUpOrDownInput();                                      // input to swin up or down


               
                if (tpInput.IsAiming)
                     tpInput.AimInput();                                      // update the input
               
                
                if(tpInput.cc.isStrafing)
                    tpInput.cc.SetAnimatorMoveSpeed(tpInput.cc.strafeSpeed);
                else
                 tpInput.cc.SetAnimatorMoveSpeed(tpInput.cc.freeSpeed);  // update the animator input magnitude



                tpInput.MoveInput();
               // StaminaConsumption();
                tpInput.cc.colliderRadius = colliderRadius;
                tpInput.cc.colliderHeight = colliderHeight;

                OnFlying.Invoke();

            }
            else
            {
                tpInput.cc.ResetCapsule();
            }


            if (tpInput.cc.groundDistance < distanceFromGroundToStopFlying)
            {
                tpInput.cc.animator.CrossFadeInFixedTime(exitFlyAnimation, enterFlyAnimationCrossfade);
                ExitFlyState();

            }

        }

        protected virtual void StaminaConsumption()
        {
            if (tpInput.cc.currentStamina <= 0)
            {
                tpInput.cc.ChangeHealth(-healthConsumption);
            }
            else
            {
                tpInput.cc.ReduceStamina(stamina, true);            // call the ReduceStamina method from the player
                tpInput.cc.currentStaminaRecoveryDelay = 0.25f;     // delay to start recovery stamina           
            }
        }
        bool up;
        bool down;

        protected void FixedUpdate()
        {
            if (up)
            {
                tpInput.cc._rigidbody.AddForce(transform.up * flyUpSpeed * flyUpInterpolate);
            }else if (down)
            {
                tpInput.cc._rigidbody.AddForce(transform.up * flyDownSpeed * flyDownInterpolate);
            }
           

        }

        protected virtual void EnterFlyState()
        {
            if(PlayerPrefs.GetInt("Jump") == 1)
            {
                ExitflyBtn.SetActive(true);
            }
           

            jmpBtn.SetActive(false);
            for(int i=0; i < particles.Length; i++)
            {
                particles[i].SetActive(true);
            }
            if (debugMode)
            {
                Debug.Log("Player enter  Flying State");
            }
            hasStartedFlying = true;
            isFlyingState = true;

            //Store the original Speed
            originalMoveSpeed = tpInput.cc.moveSpeed;
            originalRotationSpeed = tpInput.cc.freeSpeed.rotationSpeed;
            originalAnimationSmooth = tpInput.cc.freeSpeed.animationSmooth;
            originalMovementSmooth = tpInput.cc.freeSpeed.movementSmooth;

            OnEnterFly.Invoke();
            tpInput.SetLockBasicInput(true);
            tpInput.cc.disableCheckGround = true;
            tpInput.cc.lockSetMoveSpeed = true;
            tpInput.cc.moveSpeed = flyForwardSpeed;
            tpInput.cc.freeSpeed.rotationSpeed = flyRotationSpeed;
            tpInput.cc.freeSpeed.animationSmooth = flyAnimationSmooth;
            tpInput.cc.freeSpeed.movementSmooth = flyMovementSmooth;
            ResetPlayerValues();
           
            tpInput.cc._rigidbody.useGravity = false;
            tpInput.cc._rigidbody.drag = 10f;
            tpInput.cc._capsuleCollider.isTrigger = false;

            tpInput.cc.isGrounded = false;
            tpInput.cc.animator.SetBool(vAnimatorParameters.IsGrounded, false);
            tpInput.cc.animator.SetBool("isFlying", true);
        }

        protected virtual void ExitFlyState()
        {
            ExitflyBtn.SetActive(false);

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(false);
            }
            if (PlayerPrefs.GetInt("Jump") == 1)
            {
                jmpBtn.SetActive(true);
              
            }
            joystickBtn.SetActive(true);
            attackBtns.SetActive(true);
            flyUPBtn.SetActive(false);
            flyDownBtn.SetActive(false);
            flyRunBtn.SetActive(false);
            flyFireBtn.SetActive(false);

            if (debugMode)
            {
                Debug.Log("Player exit  Flying State");
            }
            isFlyingState = false;
            hasStartedFlying = false;
            tpInput.SetLockAllInput(false);
            tpInput.cc.disableCheckGround = false;
            tpInput.cc.lockSetMoveSpeed = false;
            tpInput.cc.moveSpeed = originalMoveSpeed;
            tpInput.cc.freeSpeed.rotationSpeed = originalRotationSpeed;
            tpInput.cc.freeSpeed.animationSmooth = originalAnimationSmooth;
            tpInput.cc.freeSpeed.movementSmooth = originalMovementSmooth;
            tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 0);
            tpInput.cc.ResetCapsule();
            tpInput.cc._rigidbody.useGravity = true;
            tpInput.cc._rigidbody.drag = 0f;

            OnExitFly.Invoke();
            tpInput.ResetCustomIKAdjustState();
            tpInput.cc.animator.SetBool("isFlying", false);
        }

        protected virtual void FlyUpOrDownInput()
        {
            if (tpInput.cc.customAction)
            {
                return;
            }

            if (tpInput.IsAiming)
            {
                tpInput.SetCustomIKAdjustState("FlyAiming");
            }
            else
            {
                tpInput.SetCustomIKAdjustState("FlyStandingAiming");
            }


            if (flyUpInput.GetButton())  //Conditions to Move UP         
            {
                if (debugMode)
                {
                    Debug.Log("Player Flying UP");
                }
                up = true;
                ///Set Velocity do Move UP
                flyDownInterpolate = 0f;
                flyUpInterpolate += Time.deltaTime * flyUpDownSmooth;
                flyUpInterpolate = Mathf.Clamp(flyUpInterpolate, 0f, 1f);

                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = Mathf.Lerp(vel.y, flyUpSpeed, updownSmoothCurve.Evaluate(flyUpInterpolate));
                tpInput.cc._rigidbody.velocity = vel;

                ///Set input Y to force character movement;
                tpInput.cc.input.y = 1f;
                ///Change Action State to fly Down
                tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 4);
            }
            else if (flyDownInput.GetButton())//Conditions to Move Down        
            {
                if (debugMode)
                {
                    Debug.Log("Player Flying Down");
                }

                down = true;

                flyUpInterpolate = 0f;
                flyDownInterpolate += Time.deltaTime * flyUpDownSmooth;
                flyDownInterpolate = Mathf.Clamp(flyDownInterpolate, 0f, 1f);

                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = Mathf.Lerp(vel.y, flyDownSpeed, updownSmoothCurve.Evaluate(flyDownInterpolate));
                tpInput.cc._rigidbody.velocity = vel;


                ///Set input Y to force charter movement;
                tpInput.cc.input.y = -1f;
                ///Change Action State to fly Down
                tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 3);
            }
            else
            {
                down = false;
                up = false;

                ///Reset Input Y and Character Vel Y;
                tpInput.cc.input.y = 0f;
                flyDownInterpolate = 0f;
                flyUpInterpolate = 0f;
                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = Mathf.Lerp(vel.y, 0f, flyUpDownSmooth * Time.deltaTime);
                tpInput.cc._rigidbody.velocity = vel;

                if (isFlyingSprintState)
                {
                    if (debugMode)
                    {
                        Debug.Log("Player Flying Sprint1");
                    }
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 2);
                    tpInput.cc.moveSpeed = flySprintForwardSpeed;
                   
                }
                else
                {
                    tpInput.cc.moveSpeed = flyForwardSpeed;
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 1);

                    
                       
                }
            }
        }



        protected virtual void ResetPlayerValues()
        {
            tpInput.cc.isJumping = false;
            tpInput.cc.isSprinting = false;
            tpInput.cc.isCrouching = false;
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 1);
            tpInput.cc.isGrounded = true;                                         // ground the character so that we can run the root motion without any issues
            tpInput.cc.animator.SetBool(vAnimatorParameters.IsGrounded, true);    // also ground the character on the animator so that he won't float after finishes the climb animation
            tpInput.cc.verticalVelocity = 0f;
        }



    }
}