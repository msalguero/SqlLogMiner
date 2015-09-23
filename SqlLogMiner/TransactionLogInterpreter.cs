using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlLogMiner.Entities;

namespace SqlLogMiner
{
    public static class TransactionLogInterpreter
    {
        public static void InterpretRowLogContent(byte[] rowLogContents, ref TableSchema tableSchema)
        {
            short statusBitA = BitConverter.ToInt16(new byte[2] { 0, rowLogContents[0] }, 0);
            short statusBitB = BitConverter.ToInt16(new byte[2] { 0, rowLogContents[1] }, 0);
            short columnCountOffset = BitConverter.ToInt16(rowLogContents.SubArray(4, 2), 0);

            int rowPointer = 4;
            foreach (var column in tableSchema.Columns)
            {
                if (column.Type == "int")
                {
                    column.Value = BitConverter.ToInt32(rowLogContents.SubArray(rowPointer,4),0).ToString();
                    rowPointer += 4;
                }
                else if (column.Type.Contains("char("))
                {
                    int charCount = Int32.Parse(column.Type.Split('(')[1].Split(')')[0]);
                    column.Value = ParseToCharArray(rowLogContents.SubArray(rowPointer,charCount));
                    //column.Value = BitConverter.ToChar(new byte[2] { rowLogContents[rowPointer], 0}, 0).ToString();
                    rowPointer += charCount;
                }
                else if (column.Type == "datetime")
                {
                    var baseDate = "1900-01-01 00:00";
                    var date = DateTime.ParseExact(baseDate, "yyyy-MM-dd HH:mm", null);
                    date = date.AddDays(BitConverter.ToUInt32(rowLogContents, rowPointer + 4));
                    date = date.AddSeconds(BitConverter.ToUInt32(rowLogContents, rowPointer)/300);
                    column.Value = date.ToString("yyyy/MM/dd HH:mm:ss");
                    rowPointer += 8;
                }
                else if (column.Type == "smalldatetime")
                {
                    var baseDate = "1900-01-01 00:00";
                    var date = DateTime.ParseExact(baseDate, "yyyy-MM-dd HH:mm", null);
                    int days = BitConverter.ToUInt16(rowLogContents, rowPointer + 2);
                    date = date.AddDays(days);
                    date = date.AddMinutes(BitConverter.ToUInt16(rowLogContents, rowPointer));
                    column.Value = date.ToString("yyyy/MM/dd HH:mm:ss");
                    rowPointer += 4;
                }
                else if (column.Type == "bigint")
                {
                    column.Value = BitConverter.ToInt64(rowLogContents.SubArray(rowPointer, 8), 0).ToString();
                    rowPointer += 8;
                }
                else if (column.Type == "tinyint")
                {
                    column.Value = rowLogContents[rowPointer++].ToString();
                }
                else if (column.Type == "money")
                {
                    column.Value = ((BitConverter.ToInt64(rowLogContents, rowPointer)) / 10000).ToString();
                    rowPointer += 8;
                }
                else if (column.Type.Contains("decimal") || column.Type.Contains(("numeric")))
                {
                    int precision = Int32.Parse(column.Type.Split('(')[1].Split(',')[0]);
                    int byteCount = GetByteLengthForPrecision(precision);
                    column.Value = BitConverter.ToInt64(rowLogContents.SubArray(rowPointer+1, byteCount-1), 0).ToString();
                    rowPointer += byteCount;
                }
                else if (column.Type == "float")
                {
                    column.Value = BitConverter.ToDouble(rowLogContents, rowPointer).ToString();
                    rowPointer += 8;
                }
                else if (column.Type == "real")
                {
                    column.Value = BitConverter.ToSingle(rowLogContents, rowPointer).ToString();
                    rowPointer += 4;
                }
                else if (column.Type == "bit")
                {
                    column.Value = rowLogContents[rowPointer++].ToString();
                }
                else if (column.Type.Contains("binary"))
                {
                    int bitCount = Int32.Parse(column.Type.Split('(')[1].Split(')')[0]);
                    column.Value = ParseToBinary(rowLogContents.SubArray(rowPointer, bitCount));
                }
            }
            
            short columnCount = BitConverter.ToInt16(rowLogContents, rowPointer);
            rowPointer += 2;

            byte nullBitmap = rowLogContents[rowPointer];
            rowPointer += 1;

            //string hexVariableColumnCount = RevertBytes(rowLogContents.Substring(rowPointer, 4));
            short variableColumnCount = BitConverter.ToInt16(rowLogContents, rowPointer);
            rowPointer += 2;
            int variableColumnPointer = rowPointer + (variableColumnCount*2);
            
            foreach (var column in tableSchema.Columns)
            {
                if (column.Type == "varchar")
                {
                    short variableColumnEnd = BitConverter.ToInt16(rowLogContents, rowPointer);
                    rowPointer += 2;
                    column.Value = System.Text.Encoding.Default.GetString(rowLogContents, variableColumnPointer, variableColumnEnd - variableColumnPointer);
                    variableColumnPointer = variableColumnEnd;
                }
            }
         
        }

        public static string RedoScript(TableSchema tableSchema, string operation)
        {
            string script = "";

            if (operation == "LOP_INSERT_ROWS")
                return GenerateInsert(tableSchema);
            else if (operation == "LOP_DELETE_ROWS")
                return GenerateDelete(tableSchema);
            return script;
        }

        private static string GenerateDelete(TableSchema tableSchema)
        {
            string script = "DELETE FROM " + tableSchema.TableName + "WHERE ";

            foreach (var column in tableSchema.Columns)
            {
                script = script + column.ColumnName+" = '"+ column.Value+"'";
                if (tableSchema.Columns.Last() != column)
                    script += " AND ";
            }
            return script;
        }

        private static string GenerateInsert(TableSchema tableSchema)
        {
            string script = "INSERT INTO " + tableSchema.TableName + " values('";

            foreach (var column in tableSchema.Columns)
            {
                script += column.Value;
                if (tableSchema.Columns.Last() == column)
                    script += "')";
                else
                    script += "', '";
            }
            return script;
        }

        public static string UndoScript(TableSchema tableSchema, string operation)
        {
            string script = "";
            if (operation == "LOP_INSERT_ROWS")
                return GenerateDelete(tableSchema);
            else if (operation == "LOP_DELETE_ROWS")
                return GenerateInsert(tableSchema);
            return script;
        }

        private static string ParseToBinary(byte[] array)
        {
            string binaryString = "";
            foreach (var currentByte in array)
            {
                binaryString += Convert.ToString(currentByte, 2);
            }

            return binaryString;
        }

        private static string ParseToCharArray(byte[] array)
        {
            string charArray = "";
            foreach (var currentByte in array)
            {
                charArray += BitConverter.ToChar(new byte[2] { currentByte, 0 }, 0).ToString();
            }

            return charArray;
        }

        private static byte[] RevertBytes(byte[] byteArray)
        {
            byte[] newByteArray = new byte[byteArray.Length];
            for (int i = 0; i < byteArray.Length; i+=2)
            {
                newByteArray[byteArray.Length-i] = byteArray[i];
            }
            return newByteArray;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static int GetByteLengthForPrecision(int precision)
        {
            if (precision <= 9)
            {
                return 5;
            }
            else if (precision <= 19)
            {
                return 9;
            }
            else if (precision <= 28)
            {
                return 13;
            }
            else if(precision <= 38)
            {
                return 17;
            }

            return 0;
        }
    }
}
