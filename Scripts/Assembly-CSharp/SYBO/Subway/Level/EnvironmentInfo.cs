using System;

namespace SYBO.Subway.Level;

[Serializable]
public class EnvironmentInfo
{
	public readonly EnvironmentKind Kind;

	public readonly bool HasStartEnd;

	public readonly bool HasSides;

	public readonly bool UseSameVariationPerSequence;

	public readonly bool UseSameVariationForSides;

	public readonly int MaxMidPieceCount;

	public readonly int MidPieceLengthInBlocks;

	public readonly int StartEndPieceLengthInBlocks;

	public static int DefaultMaxMidPieceCount = 100;

	public static int DefaultMidPieceLengthInBlocks = 2;

	public static int DefaultMaxMidLengthInBlocks = DefaultMidPieceLengthInBlocks * DefaultMaxMidPieceCount;

	public static readonly EnvironmentInfo None = new EnvironmentInfo(EnvironmentKind.None, hasStartEnd: false, hasSides: false, useSameVariationPerSequence: false, useSameVariationForSides: false, 0, 0, 0);

	public static readonly EnvironmentInfo Empty = new EnvironmentInfo(EnvironmentKind.Empty, hasStartEnd: false, hasSides: false, useSameVariationPerSequence: false, useSameVariationForSides: false, 0, 0, 0);

	public static readonly EnvironmentInfo FillersLow = new EnvironmentInfo(EnvironmentKind.FillersLow, hasStartEnd: false, hasSides: true, useSameVariationPerSequence: false, useSameVariationForSides: false, DefaultMaxMidPieceCount, 2, 0);

	public static readonly EnvironmentInfo FillersMedium = new EnvironmentInfo(EnvironmentKind.FillersMedium, hasStartEnd: false, hasSides: true, useSameVariationPerSequence: false, useSameVariationForSides: false, DefaultMaxMidPieceCount, 2, 0);

	public static readonly EnvironmentInfo FillersHigh = new EnvironmentInfo(EnvironmentKind.FillersHigh, hasStartEnd: false, hasSides: true, useSameVariationPerSequence: false, useSameVariationForSides: false, DefaultMaxMidPieceCount, 2, 0);

	public static readonly EnvironmentInfo Gates = new EnvironmentInfo(EnvironmentKind.Gates, hasStartEnd: false, hasSides: false, useSameVariationPerSequence: false, useSameVariationForSides: false, 1, 4, 0);

	public static readonly EnvironmentInfo Tube = new EnvironmentInfo(EnvironmentKind.Tube, hasStartEnd: false, hasSides: false, useSameVariationPerSequence: true, useSameVariationForSides: false, DefaultMaxMidPieceCount, 2, 0);

	public static readonly EnvironmentInfo Pillars = new EnvironmentInfo(EnvironmentKind.Pillars, hasStartEnd: true, hasSides: false, useSameVariationPerSequence: true, useSameVariationForSides: false, DefaultMaxMidPieceCount, 2, 1);

	public static readonly EnvironmentInfo Station = new EnvironmentInfo(EnvironmentKind.Station, hasStartEnd: true, hasSides: false, useSameVariationPerSequence: true, useSameVariationForSides: false, DefaultMaxMidPieceCount, 2, 1);

	public static readonly EnvironmentInfo Epic = new EnvironmentInfo(EnvironmentKind.Epic, hasStartEnd: true, hasSides: false, useSameVariationPerSequence: true, useSameVariationForSides: false, DefaultMaxMidPieceCount, 4, 4);

	public static readonly EnvironmentInfo[] All = new EnvironmentInfo[9] { FillersLow, FillersMedium, FillersHigh, Gates, Tube, Pillars, Station, Epic, Empty };

	public int MidPieceLength => MidPieceLengthInBlocks * 90;

	public int StartEndPieceLength => StartEndPieceLengthInBlocks * 90;

	public int MinLengthInBlocks => (!HasStartEnd) ? MidPieceLengthInBlocks : (2 * StartEndPieceLengthInBlocks + MidPieceLengthInBlocks);

	public int LengthMin => MinLengthInBlocks * 90;

	public int MidLengthMax => MaxMidPieceCount * MidPieceLengthInBlocks;

	public int MaxLengthInBlocks => (!HasStartEnd) ? MidLengthMax : (2 * StartEndPieceLengthInBlocks + MidLengthMax);

	public int MaxLength => MaxLengthInBlocks * 90;

	public EnvironmentInfo(EnvironmentKind envKind, bool hasStartEnd, bool hasSides, bool useSameVariationPerSequence, bool useSameVariationForSides, int maxMidPieceCount, int lengthMidPieceInBlocks, int lengthStartEndPieceInBlocks)
	{
		Kind = envKind;
		HasStartEnd = hasStartEnd;
		HasSides = hasSides;
		UseSameVariationPerSequence = useSameVariationPerSequence;
		UseSameVariationForSides = useSameVariationForSides;
		if (DefaultMaxMidPieceCount < maxMidPieceCount)
		{
			SDebug.LogError($"[EnvironmentInfo] Kind {envKind} : Max mid piece count {maxMidPieceCount} larger than allowed {DefaultMaxMidPieceCount}");
		}
		MaxMidPieceCount = maxMidPieceCount;
		MidPieceLengthInBlocks = lengthMidPieceInBlocks;
		StartEndPieceLengthInBlocks = lengthStartEndPieceInBlocks;
	}

	public int GetSequenceLengthInBlocks(EnvironmentSequence sequence)
	{
		if (HasStartEnd && (sequence == EnvironmentSequence.Start || sequence == EnvironmentSequence.End))
		{
			return StartEndPieceLengthInBlocks;
		}
		return MidPieceLengthInBlocks;
	}

	public static EnvironmentInfo GetByKind(EnvironmentKind envKind)
	{
		EnvironmentInfo result = None;
		bool flag = false;
		EnvironmentInfo[] all = All;
		foreach (EnvironmentInfo environmentInfo in all)
		{
			if (environmentInfo.Kind == envKind)
			{
				result = environmentInfo;
				flag = true;
			}
		}
		if (!flag)
		{
			SDebug.LogError("Could not find " + typeof(EnvironmentInfo).Name + " for EnvironmentKind " + envKind);
		}
		return result;
	}
}
