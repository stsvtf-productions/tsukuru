﻿using System.Collections.Generic;
using Chiaki;

namespace Tsukuru.Translator.Data
{
    internal static class FormatArgumentFactory
    {
        public static List<IFormatArgument> CreateFromString(string input)
        {
            var split = input.Split(',');
            var result = new List<IFormatArgument>();

            foreach (var part in split)
            {
                var cleaned = part
                    .TrimStart("{")
                    .TrimEnd("}")
                    .Split(':');

                if (cleaned.Length != 2)
                {
                    return null;
                }

                int index = int.Parse(cleaned[0]);
                char type = char.Parse(cleaned[1]);

                IFormatArgument argument = null;

                switch (type)
                {
                    case 'i':
                        argument = new IntegerFormatArgument();
                        break;

                    case 's':
                        argument = new StringFormatArgument();
                        break;
                }

                if (argument == null)
                {
                    return null;
                }

                result.Insert(index, argument);
            }

            return result;
        }
    }
}