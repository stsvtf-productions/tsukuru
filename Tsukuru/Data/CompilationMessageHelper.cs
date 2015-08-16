﻿using System;

namespace Tsukuru.Data
{
    public class CompilationMessageHelper
    {
        public static CompilationMessage ParseFromString(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }

            if (!IsLineErrorOrWarning(line))
            {
                return new CompilationMessage
                {
                    Message = line.Trim(), 
                    RawLine = line
                };
            }

            // FILENAME(FIRSTLINE -- LASTLINE) : PREFIX NUMBER: TEXT
            // or
            // FILENAME(LASTLINE) : PREFIX NUMBER: TEXT

            int lineInfoStartIdx = line.IndexOf("(", StringComparison.InvariantCultureIgnoreCase);

            if (lineInfoStartIdx == -1)
            {
                return null;
            }

			int lineInfoEndIdx = line.IndexOf(")", lineInfoStartIdx, StringComparison.InvariantCultureIgnoreCase);

            if (lineInfoEndIdx == -1)
            {
                return null;
            }

			int prefixStartIdx = line.IndexOf(" : ", lineInfoEndIdx + 1, StringComparison.InvariantCultureIgnoreCase) + " : ".Length;
			int prefixEndIdx = line.IndexOf(":", prefixStartIdx + 1, StringComparison.InvariantCultureIgnoreCase);

	        var message = new CompilationMessage
	        {
		        RawLine = line,
		        FileName = line.Substring(0, lineInfoStartIdx)
	        };

	        // Try to get line numbers
            string buffer = line.Substring(lineInfoStartIdx + 1, (lineInfoEndIdx - lineInfoStartIdx) - 1);

	        const string multipleLineSeparator = "--";

			if (buffer.Contains(multipleLineSeparator))
            {
				int firstLineIdx = Convert.ToInt32(
					buffer.Substring(
						startIndex: 0,
						length: buffer.IndexOf(multipleLineSeparator, StringComparison.InvariantCultureIgnoreCase) - 1)
							.Trim());

				int lastLineIdx = Convert.ToInt32(
					buffer.Substring(
						startIndex: buffer.IndexOf(multipleLineSeparator, StringComparison.InvariantCultureIgnoreCase) + 1,
						length: buffer.Length - buffer.IndexOf(multipleLineSeparator, StringComparison.InvariantCultureIgnoreCase)));

                message.FirstLine = firstLineIdx;
                message.LastLine = lastLineIdx;
            }
            else
            {
                int lastLineIdx = Convert.ToInt32(buffer);

                message.FirstLine = null;
                message.LastLine = lastLineIdx;
            }

            message.Prefix = line.Substring(prefixStartIdx, (prefixEndIdx - prefixStartIdx));
            message.Message = line.Substring(prefixEndIdx + 1);

            return message;
        }

        public static bool IsLineErrorOrWarning(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            bool error = false;
	        string[] prefixes = 
	        {
				"error",
				"fatal error",
				"warning"
	        };

			foreach (string prefix in prefixes)
            {
                if (line.ToLower().Contains(prefix))
                {
                    error = true;
                }
            }

            return error;
        }

        public static bool IsLineError(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            bool error = false;
			string[] prefixes = 
	        {
				"error",
				"fatal error"
	        };

			foreach (string prefix in prefixes)
            {
                if (line.ToLower().Contains(prefix))
                {
                    error = true;
                }
            }

            return error;
        }

        public static bool IsLineWarning(string line)
        {
	        return !string.IsNullOrWhiteSpace(line) 
				&& line.ToLower().Contains("warning");
        }
    }

}
