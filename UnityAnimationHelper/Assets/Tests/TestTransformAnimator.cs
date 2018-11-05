using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using AnimationHelpers;

public class TestTransformAnimator
{
    private GameObject go;
    private Transform transform;

    [SetUp]
    private void SetUp()
    {
        go = new GameObject("Test Transform");
        go.transform.position = Vector3.zero;
        transform = go.transform;
    }

    #region MoveTests
    [UnityTest]
    public IEnumerator TestMoveWithSpeed()
    {
        var startTime = Time.time;
        var animator = transform.Animate().MoveWithSpeed(Vector3.right, 1).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Vector3.right, go.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMove()
    {
        var startTime = Time.time;
        var animator = transform.Animate().Move(Vector3.right, 1).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Vector3.right, go.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLocalMoveWithSpeed()
    {
        var startTime = Time.time;
        var animator = transform.Animate().LocalMoveWithSpeed(Vector3.right, 1).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Vector3.right, go.transform.localPosition);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLocalMove()
    {
        var startTime = Time.time;
        var animator = transform.Animate().LocalMove(Vector3.right, 1).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Vector3.right, go.transform.localPosition);

        yield return null;
    }
    #endregion

    #region TestRotation
    [UnityTest]
    public IEnumerator TestRotate()
    {
        var startTime = Time.time;
        var animator = transform.Animate().Rotate(Quaternion.Euler(45, 45, 45), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Quaternion.Euler(45, 45, 45), go.transform.rotation);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestRotateWithSpeed()
    {
        var startTime = Time.time;
        var animator = transform.Animate().RotateWithSpeed(Quaternion.Euler(45, 45, 45), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Quaternion.Euler(45, 45, 45), go.transform.rotation);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLocalRotate()
    {
        var startTime = Time.time;
        var animator = transform.Animate().LocalRotate(Quaternion.Euler(45, 45, 45), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Quaternion.Euler(45, 45, 45), go.transform.rotation);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLocalRotateWithSpeed()
    {
        var startTime = Time.time;
        var animator = transform.Animate().LocalRotateWithSpeed(Quaternion.Euler(45, 45, 45), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Quaternion.Euler(45, 45, 45), go.transform.rotation);

        yield return null;
    }
    #endregion


}
