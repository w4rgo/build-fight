using UnityEngine;

namespace Assets.Scripts.Character
{
    public class BlockCreatorSelector : MonoBehaviour
    {
        private void Update()
        {

            Vector3 pos = Input.mousePosition;
            pos.z = transform.position.z - Camera.main.transform.position.z;
            var mousePos = Camera.main.ScreenToWorldPoint(pos);
            transform.position =  new Vector3(Mathf.Round(mousePos.x)+0.5f,
                Mathf.Round(mousePos.y)+0.5f,
                Mathf.Round(mousePos.z)+0.5f);

        }
    }
}