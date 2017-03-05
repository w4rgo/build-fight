using Assets.Scripts.CustomObjects.VoxelEngine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Character
{
    [RequireComponent(typeof (PlatformerCharacter))]
    public class CharacterUserControl : MonoBehaviour
    {
        private PlatformerCharacter m_Character;
        [SerializeField]
        private RaycastBlockCreator blockCreator;
        private bool m_Jump;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (CrossPlatformInputManager.GetMouseButton(0))
            {
                blockCreator.CreateBlock();
            }

            if (CrossPlatformInputManager.GetMouseButton(2))
            {
                blockCreator.DestructBlock();
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
