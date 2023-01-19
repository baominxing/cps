using System;

namespace Wimi.BtlCore.BTLMongoDB
{
    public class MongoTableAttribute:Attribute
    {
        public string TableName { get; set; }

        /// <summary>
        /// 是否按月划分collection
        /// </summary>
        public bool IsMonthlyBasis { get; set; }

        public MongoTableAttribute(string tableName, bool isMonthlyBasis = false)
        {
            this.TableName = tableName;
            this.IsMonthlyBasis = isMonthlyBasis;
        }

        public string GetTableName()
        {
            return !IsMonthlyBasis ? this.TableName : $"{this.TableName}{DateTime.Now:yyyyMM}";
        }
    }
}