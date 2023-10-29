using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    /**
     * Local variables
     */
    private GameObject lockedOnIndicator = null;

    /** 
     * Move this code to a different module, or at least not have the player be the placeholder for all these 
     * properties. They don't belong here, but for now it's okay
     */
    public GameObject uiTargetIndicator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SelectLockedOnTarget(GameObject gameObject)
    {
        if (lockedOnTarget != null)
        {
            DeselectLockedOnTarget();
        }

        lockedOnTarget = gameObject;

        lockedOnIndicator = Instantiate(uiTargetIndicator, gameObject.transform, false);
    }
    private void DeselectLockedOnTarget()
    {
        lockedOnTarget = null;
        if (lockedOnIndicator != null)
        {
            Destroy(lockedOnIndicator);
            lockedOnIndicator = null;
        }
    }
}
