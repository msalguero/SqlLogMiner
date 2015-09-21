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
                }else if (column.Type == "char")
                {
                    column.Value = BitConverter.ToChar(new byte[2] { rowLogContents[rowPointer], 0}, 0).ToString();
                    rowPointer += 1;
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
            }
            
            short columnCount = BitConverter.ToInt16(rowLogContents, rowPointer);
            rowPointer += 2;

            byte nullBitmap = rowLogContents[rowPointer];
            rowPointer += 1;

            //string hexVariableColumnCount = RevertBytes(rowLogContents.Substring(rowPointer, 4));
            short variableColumnCount = BitConverter.ToInt16(rowLogContents, rowPointer);
            rowPointer += 2;
          
            
            foreach (var column in tableSchema.Columns)
            {
                if (column.Type == "varchar")
                {
                    short variableColumnEnd = BitConverter.ToInt16(rowLogContents, rowPointer);
                    rowPointer += 2;
                    column.Value = System.Text.Encoding.Default.GetString(rowLogContents,rowPointer,variableColumnEnd - rowPointer);
                }
            }
         
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
    }
}
