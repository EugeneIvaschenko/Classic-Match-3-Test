using DG.Tweening;
using UnityEngine;

public class Match3Animation {
    private static float swapDuration = 0.4f;
    private static float failSwapDuration = 0.2f;
    private static float desrtoyingDuration = 0.6f;
    private static float fallingDurationForUnit = 0.2f;

    public static void SwapTiles(Transform tile1, Transform tile2, TweenCallback callback = null) {
        tile1.DOMove(tile2.position, swapDuration);
        tile2.DOMove(tile1.position, swapDuration).OnComplete(callback);
    }
    public static void FailSwapping(Transform tile1, Transform tile2, TweenCallback callback = null) {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(tile1.DOMove(tile2.position, failSwapDuration));
        sequence.Join(tile2.DOMove(tile1.position, failSwapDuration));
        sequence.Append(tile1.DOMove(tile1.position, failSwapDuration));
        sequence.Join(tile2.DOMove(tile2.position, failSwapDuration));
        sequence.AppendCallback(callback);
    }

    public static void DestroyTiles(Transform[] transforms, TweenCallback callback = null) {
        Sequence sequence = DOTween.Sequence();
        foreach (var transform in transforms) {
            sequence.Join(transform.DOScale(0, desrtoyingDuration));
        }
        sequence.AppendCallback(callback);
    }

    public static void FallTiles(Tile[] oldTiles, Transform[][] newTransforms, TweenCallback callback = null) {
        Sequence oldSeq = DOTween.Sequence();
        foreach (var tile in oldTiles) {
            Vector3 targetPosition = new(tile.x, tile.y);
            oldSeq.Join(tile.transform.DOLocalMove(targetPosition, fallingDurationForUnit).SetEase(Ease.Linear));
        }

        Sequence newSeqMove = DOTween.Sequence();
        Sequence newSeqScale = DOTween.Sequence();
        foreach (var colOfNew in newTransforms) {
            if (colOfNew.Length == 0)
                continue;
            float pointOfRising = colOfNew[^1].position.y + 1;

            for (int i = 0; i < colOfNew.Length; i++) {
                Transform transform = colOfNew[i].transform;

                float startPosY = pointOfRising + i;
                float targetY = transform.position.y;
                transform.position = new Vector3(transform.position.x, startPosY, transform.position.z);
                newSeqMove.Join(transform.DOMoveY(targetY, Mathf.Abs(startPosY - targetY) * fallingDurationForUnit).SetEase(Ease.Linear));

                Vector3 targetScale = transform.localScale;
                transform.localScale = Vector3.zero;
                newSeqScale.Insert(fallingDurationForUnit * i, transform.DOScale(targetScale, fallingDurationForUnit / 2));
            }
        }

        newSeqMove.AppendCallback(callback);
    }
}