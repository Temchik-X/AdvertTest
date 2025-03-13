using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace AdvertTest.Services
{
    public class AdvertisingService
    {
        private readonly Dictionary<string, List<string>> _platformsByLocation = new();

        // Загрузка данных из файла
        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден.");
            }

            var lines = File.ReadAllLines(filePath);
            _platformsByLocation.Clear();

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                {
                    continue; // Пропускаем некорректные строки
                }

                var platformName = parts[0].Trim();
                var locations = parts[1].Split(',').Select(loc => loc.Trim()).ToList();

                foreach (var location in locations)
                {
                    if (!_platformsByLocation.ContainsKey(location))
                    {
                        _platformsByLocation[location] = new List<string>();
                    }
                    _platformsByLocation[location].Add(platformName);
                }
            }
        }

        // Поиск рекламных площадок для заданной локации
        public List<string> FindPlatformsForLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return new List<string>();
            }

            var result = new HashSet<string>(); // Используем HashSet для уникальности

            foreach (var key in _platformsByLocation.Keys)
            {
                if (location.StartsWith(key)) // Проверяем вложенность локации
                {
                    result.UnionWith(_platformsByLocation[key]);
                }
            }

            return result.ToList();
        }
    }
}
