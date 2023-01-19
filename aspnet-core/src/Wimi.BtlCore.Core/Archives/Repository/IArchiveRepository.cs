using Abp.Dependency;
using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Archives.Repository
{
    public interface IArchiveRepository : ITransientDependency
    {
        void InsertDataToArchiveTable(ArchiveEntry archiveEntry, List<string> archiveDatas, List<string> columns);

        void DeleteDataFromTargetTable(ArchiveEntry archiveEntry, List<string> archiveDatas);

        List<string> GetArchiveDatas(string startTime, ArchiveEntry archiveEntry);

        bool CheckArchiveTableIsExisted(ArchiveEntry archiveEntry);

        void CreateArchiveTable(ArchiveEntry archiveEntry);

        List<string> GetArchiveTableColumns(ArchiveEntry archiveEntry);

        Tuple<List<long>, List<long>> GetArchiveDatasTraceCatalogs(string startTime, ArchiveEntry archiveEntry);

    }
}
