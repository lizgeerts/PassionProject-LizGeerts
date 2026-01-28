// using UnityEngine;
// using UnityEngine.Rendering.Universal;

// public class MultiverseManager : MonoBehaviour
// {
//     public GameManager gameManager;
//     public GameObject playerCamGO;
//     public GameObject lookUpCamGO;

//     private cinema playerCam;
//     private CinemachineCamera lookUpCam;


//     void Start()
//     {
//         playerCam = playerCamGO.GetComponent<CinemachineCamera>();
//         lookUpCam = lookUpCamGO.GetComponent<CinemachineCamera>();
//     }

//     void FixedUpdate() //for camera and physics
//     {
//         if (gameManager.transitionMultiverse)
//         {
//             SwitchToLookUp();
//             gameManager.transitionMultiverse = false;
//         }
//     }

//     void SwitchToLookUp()
//     {
//         playerCam.Priority.Value = 10;
//         lookUpCam.Priority.Value = 20;
//     }

//     public void SwitchBackToPlayer()
//     {
//         playerCam.Priority.Value = 20;
//         lookUpCam.Priority.Value = 10;
//     }
// }
