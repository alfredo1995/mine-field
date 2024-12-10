using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiConditionEvent : MonoBehaviour
{
    public bool firstCondition, secondCondition;
    [Tooltip("If true, is a AND condition, otherwise is a OR condition")]public bool andCondition;
    [Tooltip("If true, the output is inverted. ex: a true output becomes false")]public bool invertResult;
    private bool currentResult;

    public UnityEvent OnConditionTrue, OnConditionFalse;
    public UnityEvent<bool> OnOutputChanged;

    public void SetFirstCondition(bool value)
    {
        firstCondition = value;
        CheckResult();
    }

    public void SetSecondCondition(bool value)
    {
        secondCondition = value;
        CheckResult();
    }

    public void CheckResult()
    {
        bool result = firstCondition ^ andCondition || secondCondition ^ andCondition;
        result ^= invertResult;
        //Debug.Log($"first:{firstCondition} + second:{secondCondition} = {result}");
        if (result != currentResult)
        {
            currentResult = result;
            if(result) OnConditionTrue?.Invoke();
            else OnConditionFalse?.Invoke();
            Debug.Log($"state changed: {result}");
            OnOutputChanged?.Invoke(result);
        }
    }
}
