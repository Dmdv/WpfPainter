using System;
using Common.Contracts;

// ReSharper disable CodeCleanup
// ReSharper disable InconsistentNaming
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Global

namespace Common.Extensions
{
	public static class GuidExtensions
	{
		public static bool IsEqualAndNotEmpty(this Guid? left, Guid? right)
		{
			return left.HasValue && right.HasValue && left != Guid.Empty && left == right;
		}

		public static Guid ToGuid(this string str)
		{
			Guid guid;

			Guard.CheckTrue(
				Guid.TryParse(str, out guid),
				() => new InvalidCastException("Can not convert '{0}' to Guid.".FormatString(str)));

			return guid;
		}
	}
}