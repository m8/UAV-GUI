using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using DirectShowLib;
using drcom_v1.models;
using System.Drawing.Drawing2D;
using System.IO.Ports;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Reflection;
using OpenTK.Platform;




namespace drcom_v1
{
    public partial class Form1 : Form
    {

        private Capture capture;
        private bool captureStatus;
        int CameraDevice = 0;
        Video_Device[] WebCams;

        bool draw = false;
        int s = 3;
        Color color = Color.Red;

        SerialPort myPort;

        GMarkerGoogle marker;
        GMapOverlay markerOverlay;
        DataTable dt;

        int fileSale;
        double latIn = 39.5379397451763;
        double lngIn = 34.8486328125;

        Image oldImage;

        int rowAngle;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            oldImage = pictureBox2.Image;

            DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            WebCams = new Video_Device[_SystemCamereas.Length];
            for (int i = 0; i < _SystemCamereas.Length; i++)
            {
                WebCams[i] = new Video_Device() { Id = i, Name = _SystemCamereas[i].Name, ClassID = _SystemCamereas[i].ClassID }; //fill web cam array
                comboBox1.Items.Add(WebCams[i].Name);
            }

            foreach (var map in GMapProviders.List)
            {
                comboBox2.Items.Add(map);
            }


            dt = new DataTable();
            dt.Columns.Add(new DataColumn("Tanım", typeof(string)));
            dt.Columns.Add(new DataColumn("Latitude", typeof(double)));
            dt.Columns.Add(new DataColumn("Longitude", typeof(double)));

            dataGridView1.DataSource = dt;


            comboBox1.SelectedIndex = 0;

            //Gmap Controlller
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleSatelliteMap;
            gMapControl1.Position = new PointLatLng(latIn, lngIn);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 23;
            gMapControl1.Zoom = 5;
            gMapControl1.AutoScroll = true;

        }



        private void planeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Plane planeform = new Plane();
            planeform.Show();
        }

        private void settingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Setting setform = new Setting();
            setform.mainForm = this;
            setform.Show();

        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame().ToImage<Bgr, Byte>();
            ımageBox1.Image = ImageFrame;
        }


        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
        }



        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.White);

            // draw the shading background:
            List<Point> shadePoints = new List<Point>();
            shadePoints.Add(new Point(0, pictureBox1.ClientSize.Height));
            shadePoints.Add(new Point(pictureBox1.ClientSize.Width, 0));
            shadePoints.Add(new Point(pictureBox1.ClientSize.Width,
                                      pictureBox1.ClientSize.Height));
            e.Graphics.FillPolygon(Brushes.LightGray, shadePoints.ToArray());

            // scale the drawing larger:
            using (Matrix m = new Matrix())
            {
                m.Scale(4, 4);
                e.Graphics.Transform = m;

                List<Point> polyPoints = new List<Point>();
                polyPoints.Add(new Point(10, 10));
                polyPoints.Add(new Point(12, 35));
                polyPoints.Add(new Point(22, 35));
                polyPoints.Add(new Point(24, 22));

                // use a semi-transparent background brush:
                using (SolidBrush br = new SolidBrush(Color.FromArgb(100, Color.Yellow)))
                {
                    e.Graphics.FillPolygon(br, polyPoints.ToArray());
                }
                e.Graphics.DrawPolygon(Pens.DarkBlue, polyPoints.ToArray());

                foreach (Point p in polyPoints)
                {
                    e.Graphics.FillEllipse(Brushes.Red,
                                           new Rectangle(p.X - 2, p.Y - 2, 4, 4));
                }
            }
        }



        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        public void setSerialPort(SerialPort port)
        {
            serialPort1 = port;

            serialPort1.DataReceived += serialPort1_DataReceived;
        }
        int imageUpdate = 0;
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            string orgReceived = serialPort1.ReadLine();
            string serialReceived = orgReceived.TrimStart('\0').TrimEnd('\r');
            //Console.WriteLine(serialReceived);
            //textBox1.Invoke((MethodInvoker)(() => textBox1.Text = serialReceived));
            //string[] split = serialReceived.Split(new Char[] { '\r', '\n', '\\',' ' });
            int value;
            imageUpdate++;
            if (int.TryParse(serialReceived, out value))
            {
                Console.WriteLine(value);
                if (imageUpdate % 10 == 0)
                {
                    pictureBox2.Image = RotateImage(global::drcom_v1.Properties.Resources.Horizon_GroundSky, value);
                    imageUpdate = 0;
                }

            }
            //if (serialReceived != null)
            //{
            //    try
            //    {
            //        if (!string.IsNullOrWhiteSpace(serialReceived))
            //        {
            //            int rowAngle = Convert.ToInt32(serialReceived);
            //            Console.WriteLine(rowAngle);
            //            //rowAngle = -rowAngle;
            //            //pictureBox2.Image = RotateImage(global::drcom_v1.Properties.Resources.Horizon_GroundSky, rowAngle);
            //        }
            //    }
            //    catch
            //    {

            //    }
            //}
        }
        public static Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(rotationAngle);


            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(img, new Rectangle(0, 0, 450, 720));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

            dt.Rows.Add(textBox1.Text, Convert.ToDouble(textBox2.Text), Convert.ToDouble(textBox3.Text));


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            capture = new Emgu.CV.Capture(comboBox1.SelectedIndex);

            capture.QueryFrame();
            Application.Idle += new EventHandler(ProcessFrame);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string map = comboBox2.Text;

            var provider = GMapProviders.List.Where(x => x.Name == map).FirstOrDefault();
            gMapControl1.MapProvider = provider;

        }

        private void gMapControl1_Click(object sender, EventArgs e)
        {

        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {


        }

        private void button12_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.RemoveAt(fileSale); //Konum silme

        }

        private void gMapControl1_DoubleClick(object sender, EventArgs e)
        {
            //Enlem ve boylam bilgilerinin alınması

        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double lng = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
            double lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;

            //enlem ve boylamın textboxlara yazdırılması
            textBox2.Text = lat.ToString();
            textBox3.Text = lng.ToString();

            //Marker pozisyonun ayarlanması
            //addMarker(lat, lng);


        }

        private void addMarker(double lat, double lng)
        {
            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lng, lat),
              GMarkerGoogleType.green);
            markersOverlay.Markers.Add(marker);
            gMapControl1.Overlays.Add(markersOverlay);
        }


    }
}
