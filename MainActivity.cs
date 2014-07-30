using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using System.IO;
using System.Xml;
using Android.Gestures;

namespace colourGenerator
{
    [Activity(Label = "colourGenerator", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, View.IOnTouchListener
    {
        public ImageView circleImageView;
        public float _viewX;
        public float _viewY;
        private GestureDetector gestureDetector;
        
        protected override void OnCreate(Bundle bundle)
        {
            
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            circleImageView = (ImageView)FindViewById(Resource.Id.imageView);
            createBitMap("circle", circleImageView);
            circleImageView.SetOnTouchListener(this);
            gestureDetector = new GestureDetector(this, new GestureListener(circleImageView,this));
            
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _viewX = e.GetX();
                    break;
                case MotionEventActions.Move:
                    int left = (int)(e.RawX - _viewX);
                    int right = (int)(left + circleImageView.Width);
                    int width = WindowManager.DefaultDisplay.Width;
                    int height = WindowManager.DefaultDisplay.Height;
                    if (left > width)
                        left = width;
                    circleImageView.Layout(left, circleImageView.Top, right, circleImageView.Bottom);
                    break;
            }
            return gestureDetector.OnTouchEvent(e);
        }

        public XMLColour ReadXMLColour()
        {
            
            XMLColour recivedColour = new XMLColour() {RedColour =0, BlueColour= 0, GreenColour= 0 };
            //StreamReader strm = new StreamReader(@"http://www.colourlovers.com/api/colors/random");
            //XDocument xd = XDocument.Load(strm);
            //var colourList = (from x in xd.Root.Descendants("colour")
            //                  select new XMLColour
            //                  {
            //                      RedColour = int.Parse(x.Element("red").Value),
            //                      GreenColour = int.Parse(x.Element("green").Value),
            //                      BlueColo");ur = int.Parse(x.Element("blue").Value),
            //                  }).ToList<XMLColour>();
            try
            {
                XmlReader xReader = XmlReader.Create(@"http://www.colourlovers.com/api/colors/random");
                xReader.ReadToFollowing("title");
                string title = xReader.ReadElementString("title");
                xReader.ReadToFollowing("red");
                int red = int.Parse(xReader.ReadElementString("red"));
                xReader.ReadToFollowing("green");
                int green = int.Parse(xReader.ReadElementString("green"));
                xReader.ReadToFollowing("blue");
                int blue = int.Parse(xReader.ReadElementString("blue"));

                recivedColour.RedColour = red; recivedColour.GreenColour = green; recivedColour.BlueColour = blue;
                recivedColour.Title = title;
            }
            catch (Exception ex)
            {
                throw ex;
            }
     
            return recivedColour;
         }

        public void createBitMap(string bitmapType, ImageView imageview)
        {
            var paint = new Paint();
            paint.TextSize = 20;
            int width = 100, height = 100;
            Bitmap canvasBitmap = Bitmap.CreateBitmap(width, height,
                                            Bitmap.Config.Argb8888);
            XMLColour xcolour =  ReadXMLColour();
            Canvas imageCanvas = new Canvas(canvasBitmap);
            imageCanvas.DrawText(xcolour.Title,0 , height / 2, paint);

            
            paint.SetARGB(xcolour.RedColour, xcolour.GreenColour, xcolour.BlueColour, 0);
            paint.SetStyle(Paint.Style.FillAndStroke);
            
            ShapeDrawable shape = new ShapeDrawable(new OvalShape());
            shape.Paint.Set(paint);
            shape.SetIntrinsicWidth(width);
            shape.SetIntrinsicHeight(height);

            if(bitmapType.ToLower() == "circle")
            {
                imageview.SetBackgroundDrawable(shape);
            }
            else if (bitmapType.ToLower() == "circlewithtext")
            {
                imageview.SetBackgroundDrawable(new LayerDrawable(new Drawable[]
                {shape, new BitmapDrawable(canvasBitmap)})); 
            }
        }

       
    }
    public class GestureListener : GestureDetector.SimpleOnGestureListener, GestureDetector.IOnGestureListener
    {
        private MainActivity main;
        private ImageView imageView;
        private GestureDetector gesture;
        public GestureListener(ImageView img, MainActivity mainActivity)
        {
            this.main = mainActivity;
            this.imageView = img;
            gesture = new GestureDetector(main, this);
        }
        public override Boolean OnDown(MotionEvent e)
        {
            return true;
        }
        public override Boolean OnSingleTapConfirmed(MotionEvent e)
        {
            main.createBitMap("circle", imageView);
            return true;
        }
        // event when double tap occurs
        public override Boolean OnDoubleTap(MotionEvent e)
        {
            main.createBitMap("circleWithText", imageView);
            return true;
        }
    }
    public class XMLColour
    {
        public int RedColour {get; set;}
        public int GreenColour {get; set;}
        public int BlueColour {get; set;}
        public string Title { get; set; }
    }

    
}

