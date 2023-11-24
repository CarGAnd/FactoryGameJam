using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class CubeVisualsController : MonoBehaviour
{
    public MMF_Player mMF_PlayerHover;
    public MMF_Player mMF_PlayerAfterHover;
    public MMF_Player mMF_PlayerPopUp;

    public bool isClicked = false;
    public bool isHovering = false;

    void Update() {
        if (Input.GetMouseButton(0)) {
            PopUp();
        }
    }

    private void PopUp()
    {
        if (!isHovering)
            return;

        isClicked = true;

        if (mMF_PlayerHover.IsPlaying)
            mMF_PlayerHover.StopFeedbacks();

        
        mMF_PlayerPopUp.PlayFeedbacks();
    }

    void OnMouseOver()
    {
        isHovering = true;

        if (isClicked)
            return;

        if (mMF_PlayerHover.IsPlaying)
            return;

        mMF_PlayerHover.PlayFeedbacks();
    }

    void OnMouseExit()
    {
        isHovering = false;

        if (isClicked)
            return;

        mMF_PlayerHover.StopFeedbacks();
        mMF_PlayerAfterHover.PlayFeedbacks();  
    }
}
