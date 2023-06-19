using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager : MonoBehaviour
{
    TeleportPlayer teleportPlayer;
    public SimpleFittLaw simpleFittLaw;
    public PhraseChecker phraseChecker;
    public TranscriptionChecker transcriptionChecker;
    public FittsLawCircleSubtraction MathTaskStress;
    public StroopRoomController stroopRoomTask;


    public void StartTask(string taskName)
    {
        // Disable all the task scripts
        simpleFittLaw.enabled = false;
        phraseChecker.enabled = false;
        transcriptionChecker.enabled = false;
        MathTaskStress.enabled = false;
        stroopRoomTask.enabled = false;

        // Enable the corresponding task script based on the taskName
        switch (taskName)
        {
            case "SimpleFittLaw":
                simpleFittLaw.enabled = true;
                teleportPlayer.TeleportToSpawnPoint(1); // Teleport to Fitt task spawn point
                break;
            case "PhraseChecker":
                phraseChecker.enabled = true;
                teleportPlayer.TeleportToSpawnPoint(2); // Teleport to Transcription task spawn point
                break;
            case "TranscriptionChecker":
                transcriptionChecker.enabled = true;
                teleportPlayer.TeleportToSpawnPoint(5); // Teleport to Transcription task spawn point
                break;
            case "MathTaskStress":
                MathTaskStress.enabled = true;
                teleportPlayer.TeleportToSpawnPoint(4); // Teleport to Math task spawn point
                break;
            case "StroopRoom":
                stroopRoomTask.enabled = true;
                teleportPlayer.TeleportToSpawnPoint(3); // Teleport to Stroop task spawn point
                break;
            case "InterviewTask":
                teleportPlayer.TeleportToSpawnPoint(0); // Teleport to Interview task spawn point
                break;
            default:
                Debug.LogError("Invalid task name: " + taskName);
                break;
        }
    }

}
