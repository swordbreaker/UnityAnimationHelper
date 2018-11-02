using System.Collections;
using System.Collections.Generic;
using AnimationHelpers;
using UnityEngine;

public class TransformAnimationExamples : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(1f);

        //you can do a duration movement with the Move or LocalMove method
        yield return transform.Animate().Move(transform.position + transform.right * 2, 2f).Execute().CurrentCoroutine;

        //when you want to move with speed use the MoveWithSpeed method
        yield return transform.Animate().MoveWithSpeed(transform.position + transform.right * 4, 2f).Execute()
            .CurrentCoroutine;

        //you can also create a Animation sequence which will do different animation simultaneous this only works with a duration
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
        
        //You can also Loop an animation when you no define a count the Animation will loop infinitely
        transform.Animate()
            .Sequence(2f)
                //to get a smooth looping rotation we cannot use the Rotate method.
                .Custom(t => transform.localRotation = Quaternion.Euler((360 * t), (360 * t), (360 * t))) 
                .Done()
            .LoopExecute();

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
    }
}