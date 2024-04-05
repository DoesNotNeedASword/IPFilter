using System.Text;
using IPFilter;

public class LogReaderTests
{
    [Fact]
    public async Task ReadLinesAsync_WithRealFile_ReadsCorrectly()
    {
        // Путь к тестовому файлу
        var testFilePath = "..\\..\\..\\logfile1.log";

        // Создаем тестовый файл
        File.WriteAllLines(testFilePath, new[] {
            "192.168.1.100 2024-01-01T08:15:30+00:00",
            "192.168.1.101 2024-01-02T08:15:30+00:00"
        });

        var logReader = new LogReader(); // Используем LogReader, который читает из файла

        var lines = new List<string>();
        await foreach (var line in logReader.ReadLinesAsync(testFilePath))
        {
            lines.Add(line);
        }

        // Проверяем результаты
        Assert.Equal(2, lines.Count); // убедимся, что прочитаны обе строки
        Assert.Equal("192.168.1.100 2024-01-01T08:15:30+00:00", lines[0]);
        Assert.Equal("192.168.1.101 2024-01-02T08:15:30+00:00", lines[1]);

        // Удаляем тестовый файл после выполнения теста
        File.Delete(testFilePath);
    }
}