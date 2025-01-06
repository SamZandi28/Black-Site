using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{ 
   private void OnTriggerEnter(Collider other)
       {
        Debug.Log("working?");
           SceneManager.LoadScene("TeleportationWorld");
       }
}
