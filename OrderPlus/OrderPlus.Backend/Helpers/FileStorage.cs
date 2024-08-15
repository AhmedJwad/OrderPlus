
namespace OrderPlus.Backend.Helpers
{
    public class FileStorage : IFileStorage
    {
        public async Task  RemoveFileAsync(string path, string nombreContenedor)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveFileAsync(byte[] content, string extention, string containerName)
        {
            MemoryStream stream = new MemoryStream(content);
            string guid = $"{Guid.NewGuid()}{extention}";
            
            try
            {
                stream.Position = 0;
               
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{containerName}",guid );
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch
            {
                return string.Empty;
            }

            return  $"images/{containerName}/{guid}";
        }
    }
}
