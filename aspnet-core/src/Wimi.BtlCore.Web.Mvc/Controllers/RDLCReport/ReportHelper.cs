using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Controllers.RDLCReport
{
    public class ReportHelper
    {
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {

            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                return new DataTable();
            }

            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable("dt");
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }

            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);

                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }


        public static DataSet ListToDataSet<T>(List<T> list)
        {
            if (list.Count == 0) return new DataSet();
            var properties = list[0].GetType().GetProperties();
            var cols = properties.Select(p => new DataColumn(p.Name));
            var dt = new DataTable();
            dt.Columns.AddRange(cols.ToArray());
            list.ForEach(x => dt.Rows.Add(properties.Select(p => p.GetValue(x)).ToArray()));
            return new DataSet { Tables = { dt } };
        }


       
    }
}
