using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private MenuButtonController menuButtonController;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorFunctions animatorFunctions;
    [SerializeField] private int thisIndex;

    void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);

            if (Input.GetAxis("Submit") == 1)
            {
                OnClick();
            }
            // else if (animator.GetBool("pressed"))
            // {
            //     // animator.SetBool("pressed", false);
            //     // animatorFunctions.disableOnce = true;
            // }
        }
        else
        {
            animator.SetBool("selected", false);
        }
    }

    public void OnClick()
    {
        Debug.Log("Clicked!");
        animator.SetTrigger("pressed");
    }
}
