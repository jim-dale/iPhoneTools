
namespace iPhoneTools
{
    public static class PropertyListReader
    {
        public static object LoadFrom(string path)
        {
            var binaryReader = new BinaryPropertyListReader();

            object result = binaryReader.LoadFrom(path);
            if (result is null)
            {
                var xmlReader = new XmlPropertyListReader();

                result = xmlReader.LoadFrom(path);
            }

            return result;
        }
    }
}
