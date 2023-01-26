using iTextSharp.text;
using System;

namespace Utility.PDF.PdfAttributes
{
    public class PdfFieldAttribute : Attribute
    {
        public const int RED = 0;

        public const int GREEN = 1;

        public const int BLUE = 2;

        public const int BLACK = 3;

        public const int GRAY = 4;

        public string FieldName { get; set; }

        public BaseColor FontColor { get; set; }

        public PdfFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public PdfFieldAttribute(string fieldName, int color)
        {
            FieldName = fieldName;
            switch (color)
            {
                case RED:
                    FontColor = BaseColor.RED; break;
                case GREEN:
                    FontColor = BaseColor.GREEN; break;
                case BLACK:
                    FontColor = BaseColor.BLACK; break;
                case BLUE:
                    FontColor = BaseColor.BLUE; break;
            }
        }
    }
}
