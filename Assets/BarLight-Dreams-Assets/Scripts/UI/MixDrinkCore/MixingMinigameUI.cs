using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingMinigameUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private Transform arrowParent;
    [SerializeField] private MixingArrowUI arrowPrefab;

    private MixingSettings settings;

    private List<ArrowType> sequence = new();
    private List<MixingArrowUI> spawnedArrows = new();

    private int currentIndex;

    private Action onSuccess;

    private bool playing;

    private void Update()
    {
        if (!playing) return;

        if (Input.anyKeyDown)
        {
            CheckInput();
        }
    }

    public void StartGame(MixingSettings settings, Action successCallback)
    {
        this.settings = settings;
        onSuccess = successCallback;

        root.SetActive(true);

        GenerateSequence();

        currentIndex = 0;

        playing = true;

        PlayerController.instance.movement.SetCanMove(false);
    }

    private void GenerateSequence()
    {
        sequence.Clear();

        foreach (Transform child in arrowParent)
        {
            Destroy(child.gameObject);
        }

        spawnedArrows.Clear();

        for (int i = 0; i < settings.arrowCount; i++)
        {
            ArrowType randomArrow = (ArrowType)UnityEngine.Random.Range(0, 4);

            sequence.Add(randomArrow);

            MixingArrowUI arrow = Instantiate(arrowPrefab, arrowParent);

            arrow.Setup(randomArrow);

            spawnedArrows.Add(arrow);
        }
    }

    private void CheckInput()
    {
        ArrowType? input = GetInputArrow();

        if (input == null) return;

        ArrowType target = sequence[currentIndex];

        if (input == target)
        {
            spawnedArrows[currentIndex].SetCorrect();

            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                Success();
            }
        }
        else
        {
            Wrong();
        }
    }

    private ArrowType? GetInputArrow()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return ArrowType.Up;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            return ArrowType.Down;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            return ArrowType.Left;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            return ArrowType.Right;

        return null;
    }

    private void Wrong()
    {
        playing = false;

        foreach (MixingArrowUI arrow in spawnedArrows)
        {
            arrow.SetWrong();
        }

        Invoke(nameof(ResetSequence), 1f);
    }

    private void ResetSequence()
    {
        foreach (MixingArrowUI arrow in spawnedArrows)
        {
            arrow.ResetColor();
        }

        currentIndex = 0;

        playing = true;
    }

    private void Success()
    {
        playing = false;

        StartCoroutine(MoveDelay());
    }

    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(.25f);

        root.SetActive(false);

        PlayerController.instance.movement.SetCanMove(true);

        onSuccess?.Invoke();
    }
}