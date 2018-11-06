using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using AnimationHelpers;

public class TestTransformAnimator
{
    private GameObject go;
    private Transform transform;

    [UnitySetUp]
    private IEnumerator SetUp()
    {
        go = new GameObject("Test Transform");
        go.transform.position = Vector3.zero;
        transform = go.transform;
        yield return null;
    }

    [UnityTearDown]
    private IEnumerator TearDown()
    {
        GameObject.Destroy(go);
        go = null;
        transform = null;
        yield return null;
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

    [UnityTest]
    public IEnumerator TestMovePath()
    {
        var path = new[] {Vector3.right, Vector3.right + Vector3.up};
        var durations = new[] {1f, 2f};

        var startTime = Time.time;
        var animator = transform.Animate().MovePath(path, durations).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + durations.Sum(), Time.time, 0.2f);
        Assert.AreEqual(Vector3.right + Vector3.up, go.transform.localPosition);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMovePathSingelDuration()
    {
        var path = new[] { Vector3.right, Vector3.right + Vector3.up };
        var duration = 1;

        var startTime = Time.time;
        var animator = transform.Animate().MovePath(path, duration).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1 * 2, Time.time, 0.2f);
        Assert.AreEqual(Vector3.right + Vector3.up, go.transform.localPosition);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLocalMovePath()
    {
        var path = new[] { Vector3.right, Vector3.right + Vector3.up };
        var durations = new[] { 1f, 2f };

        var startTime = Time.time;
        var animator = transform.Animate().LocalMovePath(path, durations).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + durations.Sum(), Time.time, 0.2f);
        Assert.AreEqual(Vector3.right + Vector3.up, go.transform.localPosition);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLocalMovePathSingelDuration()
    {
        var path = new[] { Vector3.right, Vector3.right + Vector3.up };
        var duration = 1;

        var startTime = Time.time;
        var animator = transform.Animate().LocalMovePath(path, duration).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1 * 2, Time.time, 0.2f);
        Assert.AreEqual(Vector3.right + Vector3.up, go.transform.localPosition);

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
        var animator = transform.Animate().RotateWithSpeed(Quaternion.Euler(1, 0, 0), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Quaternion.Euler(1, 0, 0), go.transform.rotation);

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
        var animator = transform.Animate().LocalRotateWithSpeed(Quaternion.Euler(1, 0, 0), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Quaternion.Euler(1, 0, 0), go.transform.rotation);

        yield return null;
    }
    #endregion

    #region TestScale
    [UnityTest]
    public IEnumerator TestScale()
    {
        var startTime = Time.time;
        var animator = transform.Animate().Scale(new Vector3(2, 2, 2), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(new Vector3(2, 2, 2), go.transform.localScale);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestScaleWithSpeed()
    {
        var startTime = Time.time;
        var animator = transform.Animate().ScaleWithSpeed(new Vector3(2, 1, 1), 1f).Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(new Vector3(2, 1, 1), go.transform.localScale);

        yield return null;
    }
    #endregion

    #region TestSequence

    [UnityTest]
    public IEnumerator TestSimpleSeqence()
    {
        var startTime = Time.time;
        var animator = transform.Animate()
            .Sequence(1f)
                .Move(Vector3.zero, Vector3.right)
                .Rotate(Quaternion.identity, Quaternion.Euler(45, 45, 45))
                .Scale(new Vector3(1, 1, 1), new Vector3(2, 2, 2))
                .Done()
            .Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.1f);
        Assert.AreEqual(Vector3.right, go.transform.position);
        Assert.AreEqual(new Vector3(2, 2, 2), go.transform.localScale);
        Assert.AreEqual(Quaternion.Euler(45, 45, 45), go.transform.rotation);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestManySeqences()
    {
        var startTime = Time.time;
        var animator = transform.Animate()
                .Sequence(1f)
                    .Move(Vector3.zero, Vector3.right)
                    .Rotate(Quaternion.identity, Quaternion.Euler(45, 45, 45))
                    .Scale(new Vector3(1, 1, 1), new Vector3(2, 2, 2))
                .Done()
                .Sequence(1f)
                    .Move(Vector3.zero, Vector3.left)
                .Done()
            .Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 2, Time.time, 0.2f);
        Assert.AreEqual(Vector3.left, go.transform.position);
        Assert.AreEqual(new Vector3(2, 2, 2), go.transform.localScale);
        Assert.AreEqual(Quaternion.Euler(45, 45, 45), go.transform.rotation);

        yield return null;
    }

    #endregion

    #region Other

    [UnityTest]
    public IEnumerator TestDo()
    {
        int counter = 0;
        
        var startTime = Time.time;
        var animator = transform.Animate()
            .Move(Vector3.right, 1f)
            .Do(() => counter++)
            .Do(() => counter++)
            .Execute();
        yield return animator.CurrentCoroutine;

        Assert.AreEqual(startTime + 1, Time.time, 0.2f);
        Assert.AreEqual(2, counter);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestStop()
    {
        var startTime = Time.time;
        var animator = transform.Animate()
            .Move(Vector3.right, 1f)
            .Move(Vector3.left, 2f)
            .Execute();
        yield return new WaitForSeconds(1f);
        animator.Stop();

        Assert.AreEqual(startTime + 1, Time.time, 0.2f);
        Assert.AreEqual(0f, Vector3.Distance(Vector3.right, transform.position), 0.1f);
        Assert.IsFalse(animator.IsRunning);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestWaitForSeconds()
    {
        var startTime = Time.time;

        var animator = transform.Animate()
            .WaitForSeconds(1f)
            .Execute();
        yield return new WaitForSeconds(1f);
        animator.Stop();

        Assert.AreEqual(startTime + 1, Time.time, 0.2f);
        Assert.IsFalse(animator.IsRunning);

        yield return null;
    }
    #endregion
}
