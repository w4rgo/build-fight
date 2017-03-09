using Assets.Scripts.CustomObjects.VoxelEngine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Zenject;

namespace Assets.Scripts.Character
{
    [RequireComponent(typeof(PlatformerCharacter))]
    public class CharacterUserControl : MonoBehaviour
    {
        [SerializeField] private GameObject mouseBlockSelectorPrefab;
        private GameObject mouseBlockSelector;
        public bool IsControllable { get; set; }


        private PlatformerCharacter m_Character;
        private bool m_Jump;
        private IWorldModifier worldModifier;

        [Inject]
        public void Init(IWorldModifier worldModifier)
        {
            this.worldModifier = worldModifier;
        }

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter>();
        }

        private void Start()
        {
            if (!IsControllable)
            {
                return;
            }
            mouseBlockSelector = GameObject.Instantiate(mouseBlockSelectorPrefab);
        }

        private void Update()
        {
            if (!IsControllable)
            {
                return;
            }

            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (CrossPlatformInputManager.GetMouseButton(0))
            {
                worldModifier.CreateBlock(mouseBlockSelector.transform.position);
            }

            if (CrossPlatformInputManager.GetMouseButton(2))
            {
                worldModifier.DestroyBlock(mouseBlockSelector.transform.position);
            }
        }


        private void FixedUpdate()
        {
            if (!IsControllable)
            {
                return;
            }
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }
    }
}