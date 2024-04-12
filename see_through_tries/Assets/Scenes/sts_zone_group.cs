using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sts_zone_group : MonoBehaviour
{

    public SeeThroughShaderZone zone3;
    private bool _toggledOn = false;

    // Start is called before the first frame update
    void Start()
    {
        zone3.toggleZoneActivation();

        StartCoroutine(toggleWalls());
    }

    private IEnumerator toggleWalls()
    {
        yield return new WaitForSeconds(2f);

        _toggledOn = !_toggledOn;
        Debug.Log("_toggledOn: " + _toggledOn);

        zone3.toggleZoneActivation();

        StartCoroutine(toggleWalls());
    }
}
