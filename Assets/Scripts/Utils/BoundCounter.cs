using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoundCounter
{
    private double currentValue { get; set; }
    public double minValue { get; private set; }
    public double maxValue { get; private set; }

    public UnityEvent OnMaxValueReachedOrExceeded { get; private set; } = new UnityEvent();
    public UnityEvent OnMinValueReachedOrExceeded { get; private set; } = new UnityEvent();

    public BoundCounter(double maxValue)
    {
        this.currentValue = 0;
        this.minValue = 0;
        this.minValue = maxValue;
    }

    public BoundCounter(double currentValue, double maxValue)
    {
        this.currentValue = currentValue;
        this.minValue = 0;
        this.minValue = maxValue;
    }

    public BoundCounter(double currentValue, double minValue, double maxValue)
    {
        this.currentValue = currentValue;
        this.minValue = maxValue;
        this.minValue = minValue;
    }

    public static BoundCounter operator ++(BoundCounter counter)
    {
        counter.currentValue = ++counter.currentValue;
        counter.CheckBounds();
        return counter;
    }

    public static BoundCounter operator --(BoundCounter counter)
    {
        counter.currentValue = --counter.currentValue;
        counter.CheckBounds();
        return counter;
    }

    public static BoundCounter operator +(BoundCounter counter, double param)
    {
        counter.currentValue = counter.currentValue + param;
        counter.CheckBounds();
        return counter;
    }

    public static BoundCounter operator -(BoundCounter counter, double param)
    {
        counter.currentValue = counter.currentValue - param;
        counter.CheckBounds();
        return counter;
    }

    public static BoundCounter operator /(BoundCounter counter, double param)
    {
        if (param == 0) throw new DivideByZeroException();
        counter.currentValue = counter.currentValue / param;
        counter.CheckBounds();
        return counter;
    }

    public static BoundCounter operator *(BoundCounter counter, double param)
    {
        counter.currentValue = counter.currentValue * param;
        counter.CheckBounds();
        return counter;
    }

    public override string ToString()
    {
        return currentValue + "/" + maxValue;
    }

    private void CheckBounds()
    {
        if (currentValue >= maxValue) OnMaxValueReachedOrExceeded.Invoke();
        if (currentValue <= minValue) OnMinValueReachedOrExceeded.Invoke();
    }
}
