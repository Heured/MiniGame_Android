using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highscore : MonoBehaviour
{
    public GameObject starExplosionPrefab;

    void starEmitterActionAtPosition()
    {
        GameObject starExplode = Instantiate(starExplosionPrefab, this.transform.root);
        starExplode.transform.position = new Vector3(0, 3, (float)StickHeroGameSceneZposition.emitterZposition);

    }
}
