using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TcmDeployer.Misc
{
	public static class Logging
	{
		private static Regex mRegEx;

		private static Regex Formatter
		{
			get
			{
				if (mRegEx == null)
					mRegEx = new Regex("\r\n|\r|\n", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

				return mRegEx;
			}
		}

		/// <summary>
		/// Output a log trace for the given <see cref="T:System.Exception" />
		/// </summary>
		/// <param name="ex"><see cref="T:System.Exception" /></param>
		/// <returns><see cref="T:System.Exception" /> logtrace</returns>
		public static String TraceException(Exception ex)
		{
			StringBuilder sbMessage = new StringBuilder();
			int depth = 1;

			if (ex != null)
			{
				if (!String.IsNullOrEmpty(ex.Source))
					sbMessage.AppendFormat("{0} ({1})\n", ex.GetType().FullName, ex.Source);

				if (!String.IsNullOrEmpty(ex.Message))
					sbMessage.AppendLine(ex.Message);

				if (!String.IsNullOrEmpty(ex.StackTrace))
					sbMessage.AppendLine(ex.StackTrace);

				while (ex.InnerException != null)
				{
					String indent = new String('\t', depth);

					ex = ex.InnerException;

					if (!String.IsNullOrEmpty(ex.Source))
					{
						sbMessage.Append(indent);
						sbMessage.AppendFormat("{0} ({1})\n", ex.GetType().FullName, ex.Source);
					}

					if (!String.IsNullOrEmpty(ex.Message))
					{
						sbMessage.Append(indent);
						sbMessage.AppendLine(ex.Message);
					}

					if (!String.IsNullOrEmpty(ex.StackTrace))
					{
						sbMessage.Append(indent);
						sbMessage.AppendLine(Formatter.Replace(ex.StackTrace, "\r" + indent));
					}

					depth++;
				}
			}

			return sbMessage.ToString();
		}
	}
}
