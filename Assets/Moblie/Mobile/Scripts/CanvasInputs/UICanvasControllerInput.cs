using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public FirstPersonController starterAssetsInputs;
        public SonicScream sonicScream;
        public TerrainScanner terrainScanner;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
           starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualScreamInput(bool virtualScreamState)
        {
            sonicScream.ScreamInput(virtualScreamState);
        }
        public void VirtualSonarInput(Vector2 virtualSonarDirection)
        {
                       terrainScanner.SonarInput(virtualSonarDirection);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            //starterAssetsInputs.SprintInput(virtualSprintState);
        }
        
    }

}
