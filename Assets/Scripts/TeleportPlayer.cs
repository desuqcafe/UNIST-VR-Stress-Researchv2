using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public GameObject[] spawnPoints;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TeleportToSpawnPoint(0);         // Interview      -- Stress
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            TeleportToSpawnPoint(1);         // Fitt     -- Base
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            TeleportToSpawnPoint(2);         // Typing     -- Base
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            TeleportToSpawnPoint(3);         // Stroop      -- Neutral
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            TeleportToSpawnPoint(4);         // Math       -- Stress
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            TeleportToSpawnPoint(5);         // Transcription     -- Neutral
        }
    }

    public void TeleportToSpawnPoint(int index) //
    {
        if (spawnPoints.Length > index)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.SetPositionAndRotation(spawnPoints[index].transform.position, player.rotation);
        }
    }

}
