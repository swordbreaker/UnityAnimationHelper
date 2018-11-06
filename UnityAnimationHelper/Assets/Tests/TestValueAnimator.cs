using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AnimationHelpers;

public class TestValueAnimator
{
    [UnityTest]
    public IEnumerator TestVectorDuration()
    {
        var startTime = Time.time;
        var v = Vector3.zero;
        var animation = v.CreateValueDurationAnimation(1, Vector3.right);

        bool isRunning = true;
        animation.StartManualAnimation();
        while (isRunning)
        {
            v = animation.CalculateCurrentValue(out isRunning);
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual(Vector3.right, v);
        Assert.AreEqual(startTime + 1f, Time.time, 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestVectorSpeed()
    {
        var startTime = Time.time;
        var v = Vector3.zero;
        var animation = v.CreateValueSpeedAnimation(1, Vector3.right);

        bool isRunning = true;
        animation.StartManualAnimation();
        while (isRunning)
        {
            v = animation.CalculateCurrentValue(out isRunning);
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual(Vector3.right, v);
        Assert.AreEqual(startTime + 1f, Time.time, 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestColorDuration()
    {
        var startTime = Time.time;
        var c = new Color(0,0,0);
        var endColor = new Color(1, 0, 0);

        var animation = c.CreateValueDurationAnimation(1, endColor);

        bool isRunning = true;
        animation.StartManualAnimation();
        while (isRunning)
        {
            c = animation.CalculateCurrentValue(out isRunning);
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual(endColor, c);
        Assert.AreEqual(startTime + 1f, Time.time, 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestColorSpeed()
    {
        var startTime = Time.time;
        var c = new Color(0, 0, 0);
        var endColor = new Color(1, 0, 0);

        var animation = c.CreateValueSpeedAnimation(1, endColor);

        bool isRunning = true;
        animation.StartManualAnimation();
        while (isRunning)
        {
            c = animation.CalculateCurrentValue(out isRunning);
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual(endColor, c);
        Assert.AreEqual(startTime + 1f, Time.time, 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestFloatDuration()
    {
        var startTime = Time.time;
        var f = 0f;
        var endFloat = 1f;

        var animation = f.CreateValueDurationAnimation(1, endFloat);

        bool isRunning = true;
        animation.StartManualAnimation();
        while (isRunning)
        {
            f = animation.CalculateCurrentValue(out isRunning);
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual(endFloat, f);
        Assert.AreEqual(startTime + 1f, Time.time, 0.1f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestFloatSpeed()
    {
        var startTime = Time.time;
        var f = 0f;
        var endFloat = 1f;

        var animation = f.CreateValueSpeedAnimation(1, endFloat);

        bool isRunning = true;
        animation.StartManualAnimation();
        while (isRunning)
        {
            f = animation.CalculateCurrentValue(out isRunning);
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual(endFloat, f);
        Assert.AreEqual(startTime + 1f, Time.time, 0.1f);

        yield return null;
    }
}
