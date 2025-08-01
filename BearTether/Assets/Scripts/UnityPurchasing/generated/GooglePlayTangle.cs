// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ZI734GW3MZ9wQdseu6BgYcugg22LqJiljWIE//sbvNiFNj1rsQMnMdt+LaOfCWzOVRkXjSz3nAYQCJygl5LXBfe908LPNbswYBFl4Rvov+iuwvJN9B24CxELy8tXs9ZdTyUSfq/dXxzWC0C42YzZbqnv4yKbiAkxeYQ2UPP0uUbCwFBpt8A+zcWFtuBJN/RwZ58VvSO2XL+GLNpkVIyPq+FgM6cDlM5tCtLn7bHhkJX/oA8FVU3akJlRPqHxXpHN5Hg9qsfowzLvXd7979LZ1vVZl1ko0t7e3trf3F3e0N/vXd7V3V3e3t8XUk1ug/XCUydo7p5xc386XnWEOIvKOaYR46cGTXnZzydhAKm2m349WaCvXUQNe2rFp2hyiFuOyt3c3t/e");
        private static int[] order = new int[] { 3,5,13,6,4,11,9,10,10,13,13,11,12,13,14 };
        private static int key = 223;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
