using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    //This script handles and plays audio cues like footsteps, jump and land audio clips based on character movement speed and events; 
    public class AudioControl : MonoBehaviour
    {

        //References to components;
        public Controller controller;
        Animator animator;
        Mover mover;
        Transform tr;
        public AudioSource audioSource;

        //Checks for speed of sounds to play
        private bool isWalking = true;
        private bool isSprinting = false;
        private bool isSliding = false;

        //Checks if sliding clip has already played while sliding
        private bool playSlidingClip = false;

        //Whether footsteps will be based on the currently playing animation or calculated based on walked distance (see further below);
        public bool useAnimationBasedFootsteps = false;

        //Velocity threshold for landing sound effect;
        //Sound effect will only be played if downward velocity exceeds this threshold;
        public float landVelocityThreshold = 5f;

        //Footsteps will be played every time the traveled distance reaches this value (if 'useAnimationBasedFootsteps' is set to 'false');
        private float walkingFootstepDistance = 3f;
        private float sprintingFootstepDistance = 4.5f;
        private float footstepDistance = 3f;
        float currentFootstepDistance = 0f;

        private float currentFootStepValue = 0f;

        //Volume of all audio clips;
        [Range(0f, 1f)]
        public float audioClipVolume = 0.05f;

        //Range of random volume deviation used for footsteps;
        //Footstep audio clips will be played at different volumes for a more "natural sounding" result;
        public float relativeRandomizedVolumeRange = 0.2f;

        //Audio clips;
        public AudioClip[] footStepNormalClips;
        public AudioClip[] footStepSnowClips;
        public AudioClip[] footStepSewerClips;
        public AudioClip[] footStepGrassClips;
        public AudioClip[] footStepWoodClips;
        public AudioClip[] footStepStoneClips;
        public AudioClip slidingClip;
        public AudioClip jumpClip;
        public AudioClip landClip;

        ModifiedWalkerController slideChecker;

        //Audio clips for triggers (play once and done)
        public AudioClip trigger1;

        //Surface
        string currentSurface = "normal";

        //Setup;
        void Start()
        {
            //Get component references;
            //controller = GetComponent<Controller>();
            animator = GetComponentInChildren<Animator>();
            mover = GetComponent<Mover>();
            tr = transform;

            slideChecker = GameObject.Find("Player").GetComponent<ModifiedWalkerController>();

            //Connecting events to controller events;
            controller.OnLand += OnLand;
            controller.OnJump += OnJump;

            if (!animator)
                useAnimationBasedFootsteps = false;
        }

        //Update;
        void Update()
        {

            //Get controller velocity;
            Vector3 _velocity = controller.GetVelocity();

            //Calculate horizontal velocity;
            Vector3 _horizontalVelocity = VectorMath.RemoveDotVector(_velocity, tr.up);

            FootStepUpdate(_horizontalVelocity.magnitude);

            if (slideChecker.isSlipping)
            {
                isWalking = false;
                isSprinting = false;
                isSliding = true;
            }



            else if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.C))
            {
                isWalking = false;
                isSliding = false;
                isSprinting = true;
                playSlidingClip = true;
            }

            else
            {
                isSliding = false;
                isSprinting = false;
                isWalking = true;
                playSlidingClip = true;
            }

            if (isWalking)
            {
                footstepDistance = walkingFootstepDistance;
            }

            else if (isSprinting)
            {
                footstepDistance = sprintingFootstepDistance;
            }
        }

        void FootStepUpdate(float _movementSpeed)
        {
            float _speedThreshold = 0.05f;

            if (useAnimationBasedFootsteps)
            {
                //Get current foot step value from animator;
                float _newFootStepValue = animator.GetFloat("FootStep");

                //Play a foot step audio clip whenever the foot step value changes its sign;
                if ((currentFootStepValue <= 0f && _newFootStepValue > 0f) || (currentFootStepValue >= 0f && _newFootStepValue < 0f))
                {
                    //Only play footstep sound if mover is grounded and movement speed is above the threshold;
                    if (mover.IsGrounded() && _movementSpeed > _speedThreshold)
                        PlayFootstepSound(_movementSpeed);
                }
                currentFootStepValue = _newFootStepValue;
            }
            else
            {
                currentFootstepDistance += Time.deltaTime * _movementSpeed;

                //Play a oneshot sliding sound when the player is sliding 
                if (isSliding && playSlidingClip)
                {
                    playSlidingClip = false;
                    audioSource.PlayOneShot(slidingClip, audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
                }

                //Play foot step audio clip if a certain distance has been traveled and if the player is not sliding;
                else if (!isSliding && currentFootstepDistance > footstepDistance)
                {
                    //Only play footstep sound if mover is grounded and movement speed is above the threshold;
                    if (mover.IsGrounded() && _movementSpeed > _speedThreshold)
                    {
                        PlayFootstepSound(_movementSpeed);
                    }
                    currentFootstepDistance = 0f;
                }
            }
        }

        public void ChangeSoundSurface(string surface)
        {
            currentSurface = surface;

            //adjust sound speeds based on surface
            if (surface == "normal")
            {
                walkingFootstepDistance = 3f;
                sprintingFootstepDistance = 4.5f;
            }

            if (surface == "snow")
            {
                walkingFootstepDistance = 4f;
                sprintingFootstepDistance = 6.5f;

            }

            if (surface == "sewer")
            {
                walkingFootstepDistance = 4.5f;
                sprintingFootstepDistance = 6.5f;
            }

            if (surface == "grass")
            {
                walkingFootstepDistance = 3f;
                sprintingFootstepDistance = 5f;
            }

            if (surface == "wood")
            {
                walkingFootstepDistance = 3f;
                sprintingFootstepDistance = 4.5f;
            }

        }

        void PlayFootstepSound(float _movementSpeed)
        {
            // Add conditional logic in here to play specific footstep sounds based on the surface
            if (currentSurface == "normal")
            {
                int _footStepClipIndex = Random.Range(0, footStepNormalClips.Length);
                audioSource.PlayOneShot(footStepNormalClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
            else if (currentSurface == "snow")
            {
                int _footStepClipIndex = Random.Range(0, footStepSnowClips.Length);
                audioSource.PlayOneShot(footStepSnowClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
            else if (currentSurface == "sewer")
            {
                int _footStepClipIndex = Random.Range(0, footStepSewerClips.Length);
                audioSource.PlayOneShot(footStepSewerClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
            else if (currentSurface == "grass")
            {
                int _footStepClipIndex = Random.Range(0, footStepGrassClips.Length);
                audioSource.PlayOneShot(footStepGrassClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
            else if (currentSurface == "wood")
            {
                int _footStepClipIndex = Random.Range(0, footStepWoodClips.Length);
                audioSource.PlayOneShot(footStepWoodClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
            else if (currentSurface == "stone")
            {
                int _footStepClipIndex = Random.Range(0, footStepStoneClips.Length);
                audioSource.PlayOneShot(footStepStoneClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
            else //just in case, to make simpler just reuse default if ground is weird
            {
                int _footStepClipIndex = Random.Range(0, footStepNormalClips.Length);
                audioSource.PlayOneShot(footStepNormalClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
            }
        }

        /*void PlayTriggerSound()
        {
			audioSource.PlayOneShot(trigger1, audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
		}*/

        void OnLand(Vector3 _v)
        {
            //Only trigger sound if downward velocity exceeds threshold;
            if (VectorMath.GetDotProduct(_v, tr.up) > -landVelocityThreshold)
                return;

            //Play land audio clip;
            audioSource.PlayOneShot(landClip, audioClipVolume);
        }

        void OnJump(Vector3 _v)
        {
            //Play jump audio clip;
            audioSource.PlayOneShot(jumpClip, audioClipVolume);
        }
    }
}

