using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnakeGame
{//aama Khali Image le chhe
    public static class Images
    {
        public readonly static ImageSource empty = LoadImage("Empty.png");
        public readonly static ImageSource body = LoadImage("Body.png");
        public readonly static ImageSource head = LoadImage("Head.png");
        public readonly static ImageSource food = LoadImage("Food.png");
        public readonly static ImageSource deadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource deadHead = LoadImage("DeadHead.png");
        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}",UriKind.Relative));
        }
    }
}
