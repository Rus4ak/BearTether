// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("76SQMCbOiOlAX3KX1LBJRrSt5JKNZx4JjF7YdpmoMvdSSYmIIklqhLQ3OTYGtDc8NLQ3Nzb+u6SHahwrRysbpB30UeL44iIivlo/tKbM+5d+ez7sHlQ6KybcUtmJ+IwI8gFWAbykM3lwuNdIGLd4JA2R1EMuASrbMpfESnbghSe88P5kxR517/nhdUkGtDcUBjswPxywfrDBOzc3NzM2NWJBcUxki+0WEvJVMWzf1IJY6s7YCInaTup9J4TjOw4EWAh5fBZJ5uxGNLb1P+KpUTBlMIdABgrLcmHg2JBt37kaHVCvKym5gF4p1yQsbF8Jus6BB3eYmpbTt5xt0WIj0E/4Ck6g3h2Zjnb8VMpftVZvxTONvWVmQoMsToGbYbJnIzQ1NzY3");
        private static int[] order = new int[] { 8,3,3,4,6,6,13,8,13,13,11,13,12,13,14 };
        private static int key = 54;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
