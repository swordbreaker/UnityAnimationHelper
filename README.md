# UnityAnimationHelper
Herlper classes to animate Unity objects form scripts. 

The project provieds extention methods to animate transform, Textmesh pro text and variables.

For the API Documentation go to (https://swordbreaker.github.io/UnityAnimationHelper/api/index.html)[https://swordbreaker.github.io/UnityAnimationHelper/api/index.html]

## Transfrom Animator

You can do a duration movement with the [Move](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TransformAnimator.html#AnimationHelpers_TransformAnimator_Move_Vector3_System_Single_Func_System_Single_System_Single__) or [LocalMove](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TransformAnimator.html#AnimationHelpers_TransformAnimator_LocalMove_Vector3_System_Single_Func_System_Single_System_Single__) method

```c#
var animator = transform.Animate().Move(transform.position + transform.right * 2, 2f).Execute();
```

When you have a [TransformAnimator](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TransformAnimator.html) instance you can access the corutine and stop the animation.

You can wait in a coroutine
```c#
yield return animator.CurrentCoroutine;
```

And you can stop the animation
```c#
animator.Stop();
```


It is also possible to perform a [Sequence](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TransformAnimator.html#AnimationHelpers_TransformAnimator_Sequence_System_Single_) of animation simultaneous but only with a duration animation.
```c#
var duration = 2f; //2 seconds

yield return transform.Animate()
    .Sequence(2f)
        .LocalMove(transform.localPosition, transform.localPosition + Vector3.right * 4)
        .LocalRotate(transform.localRotation, Quaternion.Euler(45, 45, 0))
        .Custom(t => GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.blue, t))
        .Done()
    //you can also wait for seconds or with WaitForCondition() for a contrition to be true.
    .WaitForSeconds(0.2f)
    .Sequence(0.5f)
        .LocalMove(transform.localPosition + Vector3.right * 4,
            transform.localPosition + Vector3.right * 4 - Vector3.up * 4)
        .LocalRotate(transform.localRotation, Quaternion.Euler(180, 90, 0))
        .Custom(t => GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.green, t))
        .Done()
    .Execute().CurrentCoroutine;
```

You can also Loop an animation with [LoopExecute](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TransformAnimator.html#AnimationHelpers_TransformAnimator_LoopExecute) when you no define a count the Animation will loop infinitely.

```c#
transform.Animate()
  .Sequence(2f)
      //to get a smooth looping rotation we cannot use the Rotate method.
      .Custom(t => transform.localRotation = Quaternion.Euler((360 * t), (360 * t), (360 * t))) 
      .Done()
  .LoopExecute();
```

With the [MovePath](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TransformAnimator.html#AnimationHelpers_TransformAnimator_MovePath_IList_Vector3__IList_System_Single__Func_System_Single_System_Single__) method you can let the transform move along a defined path.

```c#
transform.position = new Vector2(0, -1);
transform.Animate().MovePath(
    new List<Vector3>()
    {
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1)
    },
    0.5f).LoopExecute();
  ```
  
## Value Animator
It is possible to do a manual animation for a single variable. The [extension Methods](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.ValueAnimationExtentions.html) supports Vector3, Color and float.

```c#
var count = 0f;
//create a new value Animator
var animator = count.CreateValueDurationAnimation(5, 1000);

//start the animation manually
animator.StartManualAnimation();

//update the text as long the animation is running
while (animator.UpdateAnimation())
{
    _text.text = animator.CurrentValue.ToString("F1");
    yield return new WaitForEndOfFrame();
}

//you can reverse the animation
animator.Reverse();
animator.StartManualAnimation();

//The UpdateAnimation will return true when t reaches 1 therefore the value will never be 0. Use CalculateCurrentValue to prevent this issue.
var isActive = true;
while (isActive)
{
    _text.text = animator.CalculateCurrentValue(out isActive).ToString("F1");
    yield return new WaitForEndOfFrame();
}
```
 
 
## TypeWriter

With the [TypeWriteText](https://swordbreaker.github.io/UnityAnimationHelper/api/AnimationHelpers.TmpProExtention.html) method you can produce a Type Write style animation for Text Mesh Pro text.

```c#
GetComponent<TMP_Text>().TypeWriteText(0.1f, 1f).Execute();
```
