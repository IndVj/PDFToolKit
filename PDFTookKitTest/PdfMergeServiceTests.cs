using PDFToolKit.Service;


namespace PDFToolKitTest
{
    public class PdfMergeServiceTests
    {
      public readonly string basePath;

        public PdfMergeServiceTests()
        {
            basePath = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        }


        [Fact]
        public void MergeFiles_ReturnsTrue_WhenValidFilesProvided()
        {
            // Arrange
            var service = new PdfMergeService();
            var file1 = Path.Combine(basePath, "England_World_Cup_2022_Squad.pdf");
            var file2 = Path.Combine(basePath, "France_World_Cup_2022_Squad.pdf");
            var output = Path.Combine(basePath, "merged.pdf");


            // Ensure output doesn't exist
            if (File.Exists(output)) File.Delete(output);

            // Act
            var result = service.MergeFiles(new[] { file1, file2 }, output);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(output));
        }
    }

}

