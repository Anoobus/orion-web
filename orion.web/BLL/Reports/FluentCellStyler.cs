using System;
using ClosedXML.Excel;
using Orion.Web.Common;

namespace Orion.Web.BLL.Reports
{
    public static class FluentCellStyle
    {
        public static IXLCell AddLeftBorder(this IXLCell cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.AsRange().AddLeftBorder(borderStyle, color);
            return cell;
        }

        public static IXLCell AddRightBorder(this IXLCell cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.AsRange().AddRightBorder(borderStyle, color);
            return cell;
        }

        public static IXLCell AddTopBorder(this IXLCell cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.AsRange().AddRightBorder(borderStyle, color);
            return cell;
        }

        public static IXLCell AddBottomBorder(this IXLCell cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.AsRange().AddBottomBorder(borderStyle, color);
            return cell;
        }

        public static IXLCell SetAlignHorizontal(this IXLCell cell, XLAlignmentHorizontalValues align)
        {
            cell.AsRange().SetAlignHorizontal(align);
            return cell;
        }

        public static IXLCell SetAlignVertical(this IXLCell cell, XLAlignmentVerticalValues align)
        {
            cell.AsRange().SetAlignVertical(align);
            return cell;
        }

        public static IXLCell SetFontStyle(this IXLCell cell, Action<IXLFont> config)
        {
            cell.AsRange().SetFontStyle(config);
            return cell;
        }

        public static IXLCell AssignValue<T>(this IXLCell cell, T val, Func<T, string> customProjection = null, (XLDataType format, string style) dataFormatOverride = default)
        {
            cell.AsRange().AssignValue(val, customProjection, dataFormatOverride);
            return cell;
        }

        public enum CellToThe
        {
            Left,
            Right,
            Below,
            Above
        }

        public static IXLRange MergeWith(this IXLCell cell, CellToThe nighborToMerge)
        {
            if (nighborToMerge == CellToThe.Left)
            {
                return cell.Worksheet.Range(cell, cell.CellLeft()).Merge();
            }
            else if (nighborToMerge == CellToThe.Right)
            {
                return cell.Worksheet.Range(cell, cell.CellRight()).Merge();
            }
            else if (nighborToMerge == CellToThe.Above)
            {
                return cell.Worksheet.Range(cell, cell.CellAbove()).Merge();
            }
            else if (nighborToMerge == CellToThe.Below)
            {
                return cell.Worksheet.Range(cell, cell.CellBelow()).Merge();
            }

            throw new NotImplementedException();
        }

        public static IXLRange AddLeftBorder(this IXLRange cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.Style.Border.LeftBorder = borderStyle;
            if (borderStyle != XLBorderStyleValues.None)
            {
                cell.Style.Border.LeftBorderColor = color ?? XLColor.Black;
            }

            foreach (var c in cell.Cells())
            {
                if (c.WorksheetColumn().ColumnNumber() > 1)
                {
                    c.CellLeft().Style.Border.RightBorder = borderStyle;
                    if (borderStyle != XLBorderStyleValues.None)
                    {
                        c.CellLeft().Style.Border.RightBorderColor = color ?? XLColor.Black;
                    }
                }
            }

            return cell;
        }

        public static IXLRange AddRightBorder(this IXLRange cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.Style.Border.RightBorder = borderStyle;
            if (borderStyle != XLBorderStyleValues.None)
            {
                cell.Style.Border.RightBorderColor = color ?? XLColor.Black;
            }

            foreach (var c in cell.Cells())
            {
                c.CellRight().Style.Border.LeftBorder = borderStyle;
                if (borderStyle != XLBorderStyleValues.None)
                {
                    c.CellRight().Style.Border.LeftBorderColor = color ?? XLColor.Black;
                }
            }

            return cell;
        }

        public static IXLRange AddTopBorder(this IXLRange cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.Style.Border.TopBorder = borderStyle;
            if (borderStyle != XLBorderStyleValues.None)
            {
                cell.Style.Border.TopBorderColor = color ?? XLColor.Black;
            }

            foreach (var c in cell.Cells())
            {
                c.CellAbove().Style.Border.BottomBorder = borderStyle;
                if (borderStyle != XLBorderStyleValues.None)
                {
                    c.CellAbove().Style.Border.BottomBorderColor = color ?? XLColor.Black;
                }
            }

            return cell;
        }

        public static IXLRange AddBottomBorder(this IXLRange cell, XLBorderStyleValues borderStyle = XLBorderStyleValues.Thin, XLColor color = null)
        {
            cell.Style.Border.BottomBorder = borderStyle;
            if (borderStyle != XLBorderStyleValues.None)
            {
                cell.Style.Border.BottomBorderColor = color ?? XLColor.Black;
            }

            foreach (var c in cell.Cells())
            {
                c.CellBelow().Style.Border.TopBorder = borderStyle;
                if (borderStyle != XLBorderStyleValues.None)
                {
                    c.CellBelow().Style.Border.TopBorderColor = color ?? XLColor.Black;
                }
            }

            return cell;
        }

        public static IXLRange SetAlignHorizontal(this IXLRange cell, XLAlignmentHorizontalValues align)
        {
            cell.Style.Alignment.Horizontal = align;
            return cell;
        }

        public static IXLRange SetAlignVertical(this IXLRange cell, XLAlignmentVerticalValues align)
        {
            cell.Style.Alignment.Vertical = align;
            return cell;
        }

        public static IXLRange AssignValue<T>(this IXLRange cell, T val, Func<T, string> customProjection = null, (XLDataType format, string style) dataFormatOverride = default)
        {
            if (customProjection != null)
            {
                cell.Value = customProjection(val);
            }
            else
            {
                cell.Value = val;
                if (dataFormatOverride != default)
                {
                    cell.DataType = dataFormatOverride.format;
                    if (dataFormatOverride.format == XLDataType.Number)
                        cell.Style.NumberFormat.Format = dataFormatOverride.style;

                    if (dataFormatOverride.format == XLDataType.DateTime)
                        cell.Style.DateFormat.Format = dataFormatOverride.style;
                }
                else if (typeof(T) == typeof(DateTime)
                    || typeof(T) == typeof(DateTimeOffset)
                    || typeof(T) == typeof(DateTimeWithZone))
                {
                    cell.DataType = XLDataType.DateTime;
                    cell.Style.DateFormat.Format = "yyyy-MM-dd";
                }
            }

            return cell;
        }

        public static IXLRange SetFontStyle(this IXLRange cell, Action<IXLFont> config)
        {
            config(cell.Style.Font);
            return cell;
        }
    }
}
