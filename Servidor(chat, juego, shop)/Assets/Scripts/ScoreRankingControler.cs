using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRankingControler : MonoBehaviour
{
    public GameObject scrollView;
    private bool isActive = false;

    private void Awake()
    {
        scrollView.SetActive(false);
    }

    public void ToggleScrollView()
    {
        isActive = !isActive;
        scrollView.SetActive(isActive);
    }
}
