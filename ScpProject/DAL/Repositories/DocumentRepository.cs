using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using d = Models.Documents;
using Dapper;

namespace DAL.Repositories
{
    public interface IDocumentRepository
    {
        Task<int> InsertDocument(d.Document targetDocument);
    }

    public class DocumentRepository : IDocumentRepository
    {
        private string ConnectionString;
        public DocumentRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<d.Document> GetDocument(int documentId)
        {
            var get = @"SELECT [Title]
                                       ,[Contents]
                                       ,[CreatedUserId]
                                       ,[OrganizationId] 
                        FROM documents WHERE id = @documentId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.QueryFirstAsync<d.Document>(get, new {documentId = documentId });
            }
        }
        public async Task<List<d.Agreement>> GetDocumentAgreements(int documentId)
        {
            var get = @"SELECT ([Description]
                                       ,[IsDeleted]
                                       ,[CreatedUserId]
                                       ,[DocumentId]
                                       ,[OrganizationId])
                        FROM Agreements WHERE id = @documentId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return (await sqlConn.QueryAsync<d.Agreement>(get, new { documentId = documentId })).ToList();
            }

        }
        public async Task<int> InsertDocument(d.Document targetDocument)
        {
            var insert = @"
                            INSERT INTO [dbo].[Documents]
                                       ([Title]
                                       ,[Contents]
                                       ,[CreatedUserId]
                                       ,[OrganizationId])
                                 VALUES
                                       (@title
                                       ,@contents
                                       ,@createdUserId
                                       ,@organization); select SCOPE_IDENTITY();
                            ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(insert, new { title = targetDocument.Title, contents = targetDocument.Contents, createdUserId = targetDocument.CreatedUserId, organization = targetDocument.OrganizationId });
            }
        }
        public async Task<int> InsertArgeement(d.Agreement targetArgreement)
        {

            var insert = @"
                            INSERT INTO [dbo].[Agreements]
                                       ([Description]
                                       ,[IsDeleted]
                                       ,[CreatedUserId]
                                       ,[DocumentId]
                                       ,[OrganizationId])
                                 VALUES
                                       (@description
                                       ,false
                                       ,@createdUserId
                                       ,@documentId
                                       ,@organizationId); select SCOPE_IDENTITY();
                            ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(insert, new { description = targetArgreement.Description, createdUserId = targetArgreement.CreatedUserId, documentId = targetArgreement.DocumentId, organizationId = targetArgreement.OrganizationId });
            }
        }
        public async Task AssignDocuments(int documentId, int assigneer, List<int> athleteIds)
        {
            var updateSql = @"UPDATE documents SET isLocked = 1 WHERE id = @documentId";
            var assigneSql = @"INSERT INTO AssignedDocuments (documentId,athleteId,assignedDate,assignedByUser) VALUES (@documentId,@athleteId,getdate(),@assigner)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(updateSql, new { documentId });
                for (int i = 0; i < athleteIds.Count(); i++)
                {
                    await sqlConn.ExecuteAsync(assigneSql, new { documentId = documentId, athleteId = athleteIds[i], assigner = assigneer });
                }
            }
        }
    }
}
