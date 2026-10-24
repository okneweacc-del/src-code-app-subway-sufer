using System;
using WordHunt;

namespace Missions;

public class DailyChallengesInRowAward : Award
{
	public DailyChallengesInRowAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
		: base(awardData, savedProgress, handler)
	{
		SyncOffset();
	}

	private int GetDailyChallengesInRow()
	{
		bool todayCompleted;
		return WordHuntManager.GetWordHuntDaysInARow(out todayCompleted);
	}

	protected override int GetProgressForGoal(AwardGoal goal)
	{
		return GetDailyChallengesInRow() - savedProgress.offset;
	}

	public override bool IsGoalAccomplished(AwardGoal goal)
	{
		if (goal is AwardSimpleGoal awardSimpleGoal && GetProgressForGoal(goal) >= awardSimpleGoal.goal)
		{
			return true;
		}
		return false;
	}

	protected override void ActivateCurrentAwardTier()
	{
		WordHuntManager.OnDailyWordInRowChanged += OnDailyWordInRowChanged;
		CheckAndUpdateProgress();
	}

	protected override void DeactivateCurrentAwardTier()
	{
		WordHuntManager.OnDailyWordInRowChanged -= OnDailyWordInRowChanged;
	}

	protected override void RecordNewOffsets()
	{
	}

	private void SyncOffset()
	{
		int dailyChallengesInRow = GetDailyChallengesInRow();
		if (savedProgress.offset == -1 || savedProgress.offset > dailyChallengesInRow)
		{
			savedProgress.offset = dailyChallengesInRow;
		}
		if (savedProgress.offset < 0)
		{
			savedProgress.offset = 0;
		}
	}

	private void OnDailyWordInRowChanged()
	{
		CheckAndUpdateProgress();
	}
}
