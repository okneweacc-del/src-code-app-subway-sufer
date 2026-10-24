using UnityEngine;

namespace SYBO.Common.Automation;

public class AutomationStepExecutor : IAutomationStepExecutor
{
	protected IAutomationStep[] _steps;

	private int _currentStepIndex;

	private IAutomationStep _currentStep;

	private float _startTime;

	public virtual bool RequiresConfirmation => false;

	public virtual string ConfirmationMessage => string.Empty;

	public bool IsDone => _steps.Length == 0 || _steps[_steps.Length - 1].IsDone;

	public float ElapsedDuration => Time.realtimeSinceStartup - _startTime;

	public AutomationStepExecutor(IAutomationStep[] steps)
	{
		_steps = steps;
	}

	public virtual void Initialize()
	{
		for (int i = 0; i < _steps.Length; i++)
		{
			_steps[i].Initialize();
		}
		_currentStepIndex = 0;
		_currentStep = _steps[_currentStepIndex];
	}

	public virtual void StartExecution()
	{
		_startTime = Time.realtimeSinceStartup;
	}

	public virtual void Terminate()
	{
		for (int i = 0; i < _steps.Length; i++)
		{
			_steps[i].Cleanup();
		}
	}

	public virtual void Update(float unscaledDeltaTime)
	{
		if (!IsDone)
		{
			if (_steps[_currentStepIndex].IsDone)
			{
				_currentStepIndex++;
				_currentStep = _steps[_currentStepIndex];
			}
			_currentStep.Update(unscaledDeltaTime);
		}
	}
}
