using OptiTrack;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ItsyBitsOptitrackCapture
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly int FLUSH_AFTER_THIS_MANY_LINES = 20;

        // optitrack fastiness * 36 bins of size 10° * 10 samples for 10° 
        //per bin:
        public static readonly int SAMPLE_THRESHOLD = 1;

        public String Participant { get; set; } = "0";

        private OptiTrackConnector connector;

        private StreamWriter log;
        private readonly NumberFormatInfo ENnumberFormat = new NumberFormatInfo();

        public ObservableCollection<Shape> Shapes { get; set; } = new ObservableCollection<Shape>();
        public Shape CurrentShape { get; set; }

        private readonly bool TEST_MODE = false;


        public MainWindow()
        {
            Angles = new ObservableCollection<KeyValuePair<int, int>>();
            InitAngles();


            InitializeComponent();
            DataContext = this;

            ENnumberFormat.NumberDecimalSeparator = ".";

            LogDebug("Let's rotate!");

            LogDebug("Start logging");



            if ((StartLog()
                && StartOptitrack()
                && StartServer()) || TEST_MODE)
            {
                //StartStudyButton.IsEnabled = true;
                //    CancelConditionButton.IsEnabled = true;
                RecordButton.IsEnabled = true;
            }


            LogDebug("Make sure that you executed the following command to start a reverse tunnel to the Android device: adb reverse tcp:9999 tcp:9999");

            AddShapeInAllSizes("triangle", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/triangle.png");
            AddShapeInAllSizes("star", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/star.png");
            AddShapeInAllSizes("heart", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/heart.png");
            AddShapeInAllSizes("circle", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/circle.png");
            AddShapeInAllSizes("cross", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/cross.png");
            AddShapeInAllSizes("square", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/square.png");
            AddShapeInAllSizes("parallel", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/parallel.png");
            AddShapeInAllSizes("arrow", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/arrow.png");
            AddShapeInAllSizes("moon", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/moon.png");
            AddShapeInAllSizes("hexa", "pack://application:,,,/ItsyBitsOptitrackCapture;component/shapes/hexa.png");

            Shapes.Shuffle("#teamdarmstadt".ToAwesomeSeed());
            CurrentShape = Shapes.First();

            if (TEST_MODE)
            {
                GridRight.Visibility = Visibility.Visible;
                LogDebug("We are in test mode!");
            }

        }

        private void AddShapeInAllSizes(String name, String imagePath)
        {
            Shapes.Add(new Shape()
            {
                Name = name,
                Size = "tiny",
                Image = new BitmapImage(new Uri(imagePath))
            });

            Shapes.Add(new Shape()
            {
                Name = name,
                Size = "small",
                Image = new BitmapImage(new Uri(imagePath))
            });

            Shapes.Add(new Shape()
            {
                Name = name,
                Size = "medium",
                Image = new BitmapImage(new Uri(imagePath))
            });

            Shapes.Add(new Shape()
            {
                Name = name,
                Size = "big",
                Image = new BitmapImage(new Uri(imagePath))
            });
        }

        private void InitAngles()
        {
            for (int i = 0; i < 360; i += 1)
            {
                Angles.Add(new KeyValuePair<int, int>(i, 0));
                var angle = i;
                var binIndex = Math.Round(angle / 10.0, 0) * 10;
                Console.WriteLine("bin:" + binIndex);
            }
        }


        private SimpleTcpServer server;

        private bool StartServer()
        {
            //we need to run  adb reverse tcp:9999 tcp:9999
            server = new SimpleTcpServer().Start(9999);

            server.ClientConnected += (sender, client) =>
            {
                String msg = "client connected from " + client.Client.RemoteEndPoint;
                LogDebug(msg);
                Console.WriteLine(msg);
            };
            server.ClientDisconnected += (sender, client) =>
            {
                String msg = "client disconnected from " + client.Client.RemoteEndPoint;
                LogDebug(msg);
                Console.WriteLine(msg);
                clientOnline = false;
            };

            server.DataReceived += (sender, msg) =>
            {
                String info = "Message from client " + msg.TcpClient.Client.RemoteEndPoint + ": " + msg.MessageString;
                //d(info);
                Console.WriteLine(info);

                //if (msg.MessageString.Contains("StartRecording"))
                //{
                //    clientOnline = true;
                //    d("Client online");
                //}

                //if (msg.MessageString.Contains("ShapePlaced"))
                //{
                //    Application.Current.Dispatcher.Invoke(new Action(() => { shapePlaced(); }));
                //    d("Shape placed");
                //}
                //else if (msg.MessageString.Contains("ShapeRemoved"))
                //{
                //    Application.Current.Dispatcher.Invoke(new Action(() => { shapeRemoved(); }));
                //    d("Shape removed");
                //}
                //msg.ReplyLine("Thanks");
            };

            //server.Delimiter = ASCIIEncoding.UTF8.GetBytes("\n")[0];
            //server.DelimiterDataReceived += (sender, msg) => 
            //{
            //    Console.WriteLine("Delim Message from client" + msg.TcpClient.Client.RemoteEndPoint + ": " + msg.MessageString);


            //};
            return server.IsStarted;
        }

        private void StopOptitrack()
        {
            connector.StopTracking();
            optitrackStatus.Content = "Stopped";
            optiButton.Content = "Start";

        }

        private bool StartOptitrack()
        {
            connector = new OptiTrackConnector();
            if (!connector.Init(@"optitrack_setup.ttp"))
            {
                LogDebug("Failed to init optitrack. Missing ttp file or server down?");
                return false;
            }

            connector.StartTracking(HandleTrackingResult);
            optitrackStatus.Content = "Started";
            optiButton.Content = "Stop";

            return true;
        }


        private void Opti_Click(object sender, RoutedEventArgs e)
        {
            if ((optiButton.Content as String) == "Start")
            {
                StartOptitrack();
            }
            else
            {
                StopOptitrack();
            }
        }

        private double GetTime()
        {
            return (DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0))).Subtract(new TimeSpan(2, 0, 0)).TotalMilliseconds;
        }


        private bool StartLog()
        {
            String logFile = "ItsyBitsOptitrackCapture-log-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";
            try
            {
                log = new StreamWriter(@logFile, append: true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            //file: Index;Participant;NanoTime;RoundedTime;Shape;Size;OptiAngle;VisibleMarkers;RawX;RawY;RawZ;RawYaw;RawPitch;RawRoll;CalibratedX;CalibratedY;CalibratedZ;MergeTime;AndroidTime;Data
            String header = "Participant;NanoTime;RoundedTime;Shape;Size;OptiAngle;VisibleMarkers;RawX;RawY;RawZ;RawYaw;RawPitch;RawRoll;CalibratedX;CalibratedY;CalibratedZ";
            LogLine(header);
            log.Flush();

            logButton.Content = "Stop";
            logStatus.Content = "Logging to " + logFile;
            return true;
        }

        private void StopLog()
        {
            if (log != null)
            {
                log.Close();
            }

            logButton.Content = "Start";
            logStatus.Content = "Stopped";
        }

        private int flushCount = 0;
        private void LogLine(String line)
        {
            if (log != null)
            {
                log.WriteLine(line);
                flushCount++;

                if (flushCount >= FLUSH_AFTER_THIS_MANY_LINES)
                {
                    flushCount = 0;
                    log.Flush();
                }

            }
        }

      
        public ObservableCollection<KeyValuePair<int, int>> Angles { get; set; }

        private bool shapeIsPlaced = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private int logIt = 0;

        private void HandleTrackingResult(Frame frame)
        {
            if (frame.Trackables.Length >= 1)
            {
                int tangibleId = 1;

                //d("Recieved optitrack frame");

                Trackable trackable = frame.Trackables[0];
                if (waitForObjectRemovalFirst && frame.Markers.Length == 0)
                {
                    waitForObjectRemovalFirst = false;
                    directlyAfterConditionRemoval = true;
                }

                if (frame.Markers.Length > 5)
                {
                    LogDebug("More than 5 markers in view! Check for foreign objects");
                    return;
                }

                if (frame.Markers.Length < 3)
                {
                    LogDebug("Less than 3 markers in view! Try to change hand position");
                    return;
                }

                if (CurrentShape == null)
                {
                    LogError("Error shape not defined, LOG NOT CORRECT!!!!!!!!");
                    return;
                }

                if (!clientOnline)
                {
                    LogDebug("Android client not online. Did you start it?");
                    return;
                }

                if (trackable.IsTracked && trackable.Id == tangibleId)
                {
                    if (calibrate)
                    {
                        calibratedX = trackable.X * 1000;
                        calibratedY = trackable.Y * 1000;
                        calibratedZ = trackable.Z * 1000;

                        calibrate = false;
                        LogDebug("calibrated to x=" + calibratedX + ", y=" + calibratedY + ", z=" + calibratedZ);
                    }
                    else if (
                      (CheckPositionAndAngle(trackable) && !waitForObjectRemovalFirst)
                      || TEST_MODE
                  )
                    {
                        double rotation = trackable.Yaw;
                        //double degree = toDeg(rotation) + 180;
                        double angle = 360 - (rotation + 180);

                        //String line = getTime(round: false).ToString(ENnumberFormat) + "," + getTime().ToString(ENnumberFormat) + "," + rotation.ToString(ENnumberFormat) + "," + degree.ToString(ENnumberFormat) + ", roll=" + toDeg(trackable.Roll) + ", pitch=" + toDeg(trackable.Pitch);

                        double time = GetTime();

                        String line =
                            Participant
                            + ";" + time.ToString(ENnumberFormat)
                            + ";" + Math.Round(time, 0).ToString(ENnumberFormat)
                            + ";" + CurrentShape.Name
                            + ";" + CurrentShape.Size
                            + ";" + angle.ToString(ENnumberFormat)
                            + ";" + frame.Markers.Length
                            + ";" + trackable.X.ToString(ENnumberFormat)
                            + ";" + trackable.Y.ToString(ENnumberFormat)
                            + ";" + trackable.Z.ToString(ENnumberFormat)
                            + ";" + trackable.Yaw.ToString(ENnumberFormat)
                            + ";" + trackable.Pitch.ToString(ENnumberFormat)
                            + ";" + trackable.Roll.ToString(ENnumberFormat)
                            + ";" + calibratedX.ToString(ENnumberFormat)
                            + ";" + calibratedY.ToString(ENnumberFormat)
                            + ";" + calibratedZ.ToString(ENnumberFormat)
                            ;


                        LogLine(line);


                        //int binIndex = BinTo10(angle);
                        int binIndex = (int)Math.Round(angle, 0);
                        CountUpAngleAndRotateArrow(binIndex);

                        CheckIfConditionFinished();

                        logIt++;
                        if (logIt > 100)
                        {
                            LogDebug("Object rotation:" + angle + " (binIndex: " + binIndex + ", rawYaw:" + trackable.Yaw + ")");
                            Console.WriteLine("Object rotation:" + angle + " (binIndex: " + binIndex + ", rawYaw:" + trackable.Yaw + ")");
                            logIt = 0;
                        }
                    }
                }
                else
                {
                    //d("trackable not there or shape not placed?");
                }



            }


        }

        private bool CheckPositionAndAngle(Trackable trackable)
        {
            double pitchAbs = Math.Abs(trackable.Pitch);
            double rollAbs = Math.Abs(trackable.Roll);


            double x = trackable.X * 1000;
            double y = trackable.Y * 1000; //height
            double z = trackable.Z * 1000;

            double xCalib = x - calibratedX;
            double yCalib = y - calibratedY;
            double zCalib = z - calibratedZ;

            calibCount++;
            if (calibCount > 30)
            {
                //d("x = " + xCalib + ", y=" + yCalib + ", z=" + zCalib + ", pitchAbs=" + pitchAbs + ", rollAbs=" + rollAbs);
                calibCount = 0;
            }

            //breite: 70mm
            //höhe: 110mm
            double offset = 10; //spielraum
            double angleOffset = 15;
            double heightOffset = 10;

            if (pitchAbs <= angleOffset
                && rollAbs <= angleOffset
                && yCalib.Range(-heightOffset, heightOffset)
                && xCalib.Range(-5, 110 + offset)
                && zCalib.Range(-5, 70 + offset)
                )
            {
                ShapePlaced();
                return true;
            }

            LogDebug("removed? x = " + xCalib + ", y=" + yCalib + ", z=" + zCalib + ", pitchAbs=" + pitchAbs + ", rollAbs=" + rollAbs);
            ShapeRemoved();
            return false;
        }

        private void CheckIfConditionFinished()
        {
            if (Application.Current != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {

                        foreach (KeyValuePair<int, int> pair in Angles)
                        {
                            if (pair.Value < SAMPLE_THRESHOLD)
                            {
                                //Still something left to do
                                return;
                            }
                        }

                        //Change condition
                        RecordingFinished();
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private bool waitForObjectRemovalFirst = false;
        private void RecordingFinished()
        {
            if (Participant == "0")
            {
                Participant = "1";
            }
            else
            {
                Participant = "0";
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Participant"));

            ShapeImageBorder.BorderBrush = Brushes.DarkRed;

            if (CurrentShape == Shapes.Last())
            {
                RotateText("Experiment finished, thank you!");
                GridMiddle.Visibility = Visibility.Collapsed;
                return;
            }

            waitForObjectRemovalFirst = true;
            Angles.Clear();
            InitAngles();

            GridMiddle.Visibility = Visibility.Collapsed;
            RotateText("Condition finished (" + Shapes.IndexOf(CurrentShape) + "/" + Shapes.Count + ")! Please remove the current shape from the tracking space and select the next shape");
            directlyAfterConditionRemoval = true;
            GridRight.Visibility = Visibility.Hidden;
        }

       
        private void LogError(String text)
        {
            LogDebug(text);
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    EmergencyBorder.BorderBrush = Brushes.DarkRed;
                    EmergencyBorder.BorderThickness = new Thickness(5);

                }));
            }

        }
        private void LogDebug(String text)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    debug.AppendText(text + Environment.NewLine);
                    debug.ScrollToEnd();
                }));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((logButton.Content as String) == "Start")
            {
                StartLog();

            }
            else
            {
                StopLog();

            }
        }

        private void ShapePlaced()
        {
            if (shapeIsPlaced)
            {
                return;
            }
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    shapeIsPlaced = true;
                    ShapeImageBorder.BorderBrush = Brushes.Green;

                    Text.Visibility = Visibility.Hidden;
                    Text.Content = "Thanks! Now please move and rotate the shape on the screen so that all rotations turn from red to green:";
                    Text.Visibility = Visibility.Visible;
                    ((Storyboard)FindResource("fadeIn")).Begin(Text);


                    GridRight.Visibility = Visibility.Visible;
                    ((Storyboard)FindResource("fadeIn")).Begin(GridRight);
                }));
            }
        }

        private void ShapeRemoved()
        {
            if (!shapeIsPlaced)
            {
                return;
            }
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    shapeIsPlaced = false;
                    ShapeImageBorder.BorderBrush = Brushes.DarkRed;

                    Text.Visibility = Visibility.Hidden;
                    if (directlyAfterConditionRemoval)
                    {
                        Text.Content = "Thanks! Please grab \"" + CurrentShape.Name + "\" with size \"" + CurrentShape.Size + "\" and put it on the smartphone:";
                        directlyAfterConditionRemoval = false;
                    }
                    else
                    {
                        Text.Content = "Wait! Do not remove the shape yet";
                    }
                    Text.Visibility = Visibility.Visible;
                    ((Storyboard)FindResource("fadeIn")).Begin(Text);
                }));
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            //if (Text.Visibility == Visibility.Visible)
            //{
            //    Text.Visibility = Visibility.Hidden;
            //} else
            //{
            //    ((Storyboard)FindResource("fadeIn")).Begin(Text);
            //    Text.Visibility = Visibility.Visible;
            //}

            ShapeImageBorder.BorderBrush = Brushes.DarkRed;
            //rotateShape("Please place the following shape on the smartphone:", new BitmapImage(new Uri("pack://application:,,,/ItsyBitsOptitrackCapture;component/dummy.png")));


            RotateShape("Please place the following shape \"" + CurrentShape.Name + "\" with size \"" + CurrentShape.Size + "\" on the smartphone:", CurrentShape.Image, CurrentShape.Size);
            EmergencyBorder.BorderBrush = Brushes.LightGray;
            EmergencyBorder.BorderThickness = new Thickness(2);

            GridRight.Visibility = Visibility.Hidden;
            GridMiddle.Visibility = Visibility.Collapsed;
        }

        private void RotateText(String text)
        {
            Animate.Visibility = Visibility.Hidden;
            Text.Content = text;
            Animate.Visibility = Visibility.Visible;
            ((Storyboard)FindResource("fadeIn")).Begin(Animate);
        }

        private void RotateShape(String text, BitmapImage shapeImage, String scaleName)
        {
            double scale = 1;
            if (scaleName == "tiny")
            {
                scale = 0.3;
            }

            if (scaleName == "small")
            {
                scale = 0.6;
            }

            if (scaleName == "medium")
            {
                scale = 0.8;
            }

            if (scaleName == "big")
            {
                scale = 1;
            }

            Animate.Visibility = Visibility.Hidden;
            Text.Content = text;
            ShapeImage.Source = shapeImage;
            ShapeImage.RenderTransform = new ScaleTransform(scale, scale);
            Animate.Visibility = Visibility.Visible;
            ((Storyboard)FindResource("fadeIn")).Begin(Animate);
        }

        private void CancelConditionButtonClick(object sender, RoutedEventArgs e)
        {

            if (TEST_MODE)
            {
                //for (int i = 0; i < 1; i += 2)
                //{
                //    for (int x = 0; x < 2; x++)
                //    {
                //        CountUpAngleAndRotateArrow(i);
                //    }
                //}

                int i = int.Parse(Participant);
                CountUpAngleAndRotateArrow(i);
                //for (int i = 10; i < 120; i += 1)
                //{
                //    for (int x = 0; x < 75; x++)
                //    {
                //        CountUpAngleAndRotateArrow(i);
                //    }
                //}

                //CountUpAngleAndRotateArrow(angle++);

            }
            else
            {
                RecordingFinished();
            }
        }

        private double calibratedX, calibratedY, calibratedZ;

        public int CurrentAngle { get; set; } = 0 - 90;
        private void CountUpAngleAndRotateArrow(int angle)
        {
            for (int i = 0; i < Angles.Count; i++)
            {
                KeyValuePair<int, int> pair = Angles[i];
                if (pair.Key == angle)
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Angles[i] = new KeyValuePair<int, int>(Angles[i].Key, Angles[i].Value + 1);
                        }));
                    }
                    break;
                }
            }

            CurrentAngle = angle - 90;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentAngle"));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            StopOptitrack();
            StopLog();
        }


        private bool calibrate = false;
        private int calibCount;
        private bool clientOnline = true;


        private bool directlyAfterConditionRemoval = false;

        private void CalibrateButton_Click(object sender, RoutedEventArgs e)
        {
            calibrate = true;
        }
    }
}
