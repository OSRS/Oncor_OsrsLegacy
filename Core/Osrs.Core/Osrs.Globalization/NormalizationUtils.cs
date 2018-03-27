//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
namespace Osrs.Globalization
{
    public enum NormalizationForm
    {
        FormC = 1,
        FormD = 2,
        FormKC = 5,
        FormKD = 6
    }

    public static class NormalizationUtils
    {
        public static string Normalize(this string src)
        {
            return Normalization.Normalize(src, 0);
        }

        public static string Normalize(this string src, NormalizationForm normalizationForm)
        {
            switch (normalizationForm)
            {
                default:
                    return Normalization.Normalize(src, 0);
                case NormalizationForm.FormD:
                    return Normalization.Normalize(src, 1);
                case NormalizationForm.FormKC:
                    return Normalization.Normalize(src, 2);
                case NormalizationForm.FormKD:
                    return Normalization.Normalize(src, 3);
            }
        }

        public static bool IsNormalized(this string src)
        {
            return Normalization.IsNormalized(src, 0);
        }

        public static bool IsNormalized(this string src, NormalizationForm normalizationForm)
        {
            switch (normalizationForm)
            {
                default:
                    return Normalization.IsNormalized(src, 0);
                case NormalizationForm.FormD:
                    return Normalization.IsNormalized(src, 1);
                case NormalizationForm.FormKC:
                    return Normalization.IsNormalized(src, 2);
                case NormalizationForm.FormKD:
                    return Normalization.IsNormalized(src, 3);
            }
        }
    }
}
