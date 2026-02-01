using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJumpST : MonoBehaviour
{
    
    [SerializeField]private int Jump_index = 0;
    public void Jump()
    {
        SceneManager.LoadScene(Jump_index);
    }
}
