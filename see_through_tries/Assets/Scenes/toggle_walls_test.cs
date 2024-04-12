using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggle_walls_test : MonoBehaviour
{
    [SerializeField]
    private float time;
    [SerializeField]
    private GroupShaderReplacement _groupShaderReplacement;
    private bool _toggledOn = false;

    public TransitionController seeThroughShaderController;

    void Start()
    {
        seeThroughShaderController = new TransitionController(this.transform);

        StartCoroutine(toggleWalls());
    }

    private IEnumerator toggleWalls()
    {
        yield return new WaitForSeconds(time);

        _toggledOn = !_toggledOn;
        Debug.Log("_toggledOn: " + _toggledOn);
        if (_toggledOn)
        {
            activateSTSEffect();
        }
        else
        {
            dectivateSTSEffect();
        }

        StartCoroutine(toggleWalls());
    }

    private void activateSTSEffect()
    {
        seeThroughShaderController.notifyOnTriggerEnter(this.transform);

    }

    private void dectivateSTSEffect()
    {
        seeThroughShaderController.notifyOnTriggerExit(this.transform);

    }
}
