using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WiimoteLib;
using WiiTUIO.Provider;
using WpfScreenHelper;

namespace WpfApp7.ViewModels
{
    public class MainWinViewModel
    {
        private const string APP_NAME_FOLDER = "WiimoteGunTester2";

        private WiimoteCollection mWC;
        public WiimoteCollection MWC => mWC;

        private List<Wiimote> wiimoteList = new List<Wiimote>();
        public List<Wiimote> WiimoteList => wiimoteList;

        private enum CalibrationStep : ushort
        {
            None,
            CenterScreen,
            TopLeft,
            BottomRight,
            Done,
        }

        private CalibrationStep currentCalibStep = CalibrationStep.None;

        private int canvasWidth;
        public int CanvasWidth => canvasWidth;
        private int canvasHeight;
        public int CanvasHeight => canvasHeight;

        private int gunImageTop;
        public int GunImageTop => gunImageTop;
        public event EventHandler GunImageTopChanged;

        private int gunImageLeft;
        public int GunImageLeft => gunImageLeft;
        public event EventHandler GunImageLeftChanged;

        private int gunImageBottom;
        public int GunImageBottom => gunImageBottom;
        public event EventHandler GunImageBottomChanged;

        private int gunImageRight;
        public int GunImageRight => gunImageRight;
        public event EventHandler GunImageRightChanged;

        private int gunCenterTop;
        public int GunCenterTop => gunCenterTop;
        public event EventHandler GunCenterTopChanged;

        private int gunCenterLeft;
        public int GunCenterLeft => gunCenterLeft;
        public event EventHandler GunCenterLeftChanged;

        private bool displayTopLeftGunImg;
        public bool DisplayTopLeftGunImg
        {
            get => displayTopLeftGunImg;
            private set
            {
                if (displayTopLeftGunImg == value) return;
                displayTopLeftGunImg = value;
                DisplayTopLeftGunImgChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DisplayTopLeftGunImgChanged;


        private bool displayBottomRightGunImg;
        public bool DisplayBottomRightGunImg
        {
            get => displayBottomRightGunImg;
            private set
            {
                if (displayBottomRightGunImg == value) return;
                displayBottomRightGunImg = value;
                DisplayBottomRightGunImgChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DisplayBottomRightGunImgChanged;


        private bool displayCenterGunImg;
        public bool DisplayCenterGunImg => displayCenterGunImg;

        public event EventHandler DisplayCenterGunImgChanged;

        private WiimoteStateData stateData = new WiimoteStateData();
        public WiimoteStateData StateData => stateData;
        public event EventHandler StateDataChanged;

        private int lightGunPointX;
        public int LightGunPointX => lightGunPointX;
        public event EventHandler LightGunPointXChanged;

        private int lightGunPointY;
        public int LightGunPointY => lightGunPointY;
        public event EventHandler LightGunPointYChanged;

        private bool lightGunPointVisible;
        public bool LightGunPointVisible => lightGunPointVisible;
        public event EventHandler LightGunPointVisibleChanged;

        private PointF topLeftCalibPoint = new PointF();
        private PointF bottomRightCalibPoint = new PointF();
        private PointF centerCalibPoint = new PointF();

        private bool previousBState;
        private bool currentBState;

        private string midpointString = string.Empty;
        public string MidPointString
        {
            get => midpointString;
            private set
            {
                if (midpointString == value) return;
                midpointString = value;
                MidPointStringChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler MidPointStringChanged;

        private string calibPointString = string.Empty;
        public string CalibPointString
        {
            get => calibPointString;
            private set
            {
                if (calibPointString == value) return;
                calibPointString = value;
                CalibPointStringChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler CalibPointStringChanged;

        private string currentStepHelpText = string.Empty;
        public string CurrentStepHelpText
        {
            get => currentStepHelpText;
            private set
            {
                if (currentStepHelpText == value) return;
                currentStepHelpText = value;
                CurrentStepHelpTextChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler CurrentStepHelpTextChanged;

        private string warningHelpText = string.Empty;
        public string WarningHelpText
        {
            get => warningHelpText;
            private set
            {
                if (warningHelpText == value) return;
                warningHelpText = value;
                WarningHelpTextChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler WarningHelpTextChanged;

        private double topLeftXCoorAdj;
        public double TopLeftXCoorAdj
        {
            get => topLeftXCoorAdj;
            set
            {
                //double temp = Convert.ToDouble(value);
                //if (topLeftXCoorAdj == temp) return;
                if (topLeftXCoorAdj == value) return;
                topLeftXCoorAdj = value;
                TopLeftXCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler TopLeftXCoorAdjChanged;

        private double topLeftYCoorAdj;
        public double TopLeftYCoorAdj
        {
            get => topLeftYCoorAdj;
            set
            {
                //double temp = Convert.ToDouble(value);
                //if (topLeftYCoorAdj == temp) return;
                //topLeftYCoorAdj = temp;
                if (topLeftYCoorAdj == value) return;
                topLeftYCoorAdj = value;
                TopLeftYCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler TopLeftYCoorAdjChanged;

        private double bottomRightXCoorAdj;
        public double BottomRightXCoorAdj
        {
            get => bottomRightXCoorAdj;
            set
            {
                //double temp = Convert.ToDouble(value);
                //if (bottomRightXCoorAdj == temp) return;
                if (bottomRightXCoorAdj == value) return;
                bottomRightXCoorAdj = value;
                BottomRightXCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler BottomRightXCoorAdjChanged;

        private double bottomRightYCoorAdj;
        public double BottomRightYCoorAdj
        {
            get => bottomRightYCoorAdj;
            set
            {
                //double temp = Convert.ToDouble(value);
                //if (bottomRightYCoorAdj == temp) return;
                //bottomRightYCoorAdj = temp;
                if (bottomRightYCoorAdj == value) return;
                bottomRightYCoorAdj = value;
                BottomRightYCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler BottomRightYCoorAdjChanged;

        private double centerXCoorAdj;
        public double CenterXCoorAdj
        {
            get => centerXCoorAdj;
            set
            {
                if (centerXCoorAdj == value) return;
                centerXCoorAdj = value;
                CenterXCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler CenterXCoorAdjChanged;

        private double centerYCoorAdj;
        public double CenterYCoorAdj
        {
            get => centerYCoorAdj;
            set
            {
                if (centerYCoorAdj == value) return;
                centerYCoorAdj = value;
                CenterYCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler CenterYCoorAdjChanged;

        private bool displayDoneVis;
        public bool DisplayDoneVis
        {
            get => displayDoneVis;
            set
            {
                if (displayDoneVis == value) return;
                displayDoneVis = value;
                DisplayDoneVisChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DisplayDoneVisChanged;

        private WiiGunMappingConfig gunMappingConfig;

        public event EventHandler WiimoteStatePreprocess;

        private Warper pWarper;
        private PointF[] finalPos = new PointF[4];
        private uint[] see = new uint[4];
        private PointF median;
        private CursorPos lastPos = new CursorPos(0.0f, 0.0f, false);

        private float xDistTop;
        private float xDistBottom;
        private float yDistLeft;
        private float yDistRight;

        float angleTop;
        float angleBottom;
        float angleLeft;
        float angleRight;

        private float[] angleOffset = new float[4];

        double angle;
        float ledsHeight;
        float ledsWidth;

        private float topOffset;
        private float bottomOffset;
        private float leftOffset;
        private float rightOffset;

        private CursorPos currentPoint;
        private PointF outputPoint;
        private float TLled = 0.0f;
        private float TRled = 0.0f;

        private const int GUN_IMG_WIDTH = 72;
        private const int GUN_IMG_HEIGHT = 72;
        private const int IRSENSORS_LEN = 4;

        public int GunImgWidth => GUN_IMG_WIDTH;
        public int GunImgHeight => GUN_IMG_HEIGHT;

        public MainWinViewModel()
        {
            mWC = new WiimoteCollection();
            this.pWarper = new Warper();
            SetCanvasDimensions((int)WpfScreenHelper.SystemInformation.VirtualScreen.Width,
                (int)WpfScreenHelper.SystemInformation.VirtualScreen.Height);

            gunMappingConfig = new WiiGunMappingConfig();
            DetectPreviousMapping();

            TopLeftXCoorAdjChanged += MainWinViewModel_TopLeftXCoorAdjChanged;
            TopLeftYCoorAdjChanged += MainWinViewModel_TopLeftYCoorAdjChanged;
            BottomRightXCoorAdjChanged += MainWinViewModel_BottomRightXCoorAdjChanged;
            BottomRightYCoorAdjChanged += MainWinViewModel_BottomRightYCoorAdjChanged;
            CenterXCoorAdjChanged += MainWinViewModel_CenterXCoorAdjChanged;
            CenterYCoorAdjChanged += MainWinViewModel_CenterYCoorAdjChanged;
        }

        private void MainWinViewModel_BottomRightYCoorAdjChanged(object sender, EventArgs e)
        {
            if (currentCalibStep == CalibrationStep.Done)
            {
                bottomRightCalibPoint.Y = (float)bottomRightYCoorAdj;
                gunMappingConfig.RemoteMapping.MappingPoints.BottomRightY = bottomRightYCoorAdj;
                GenerateCalibPointOutput();
            }
        }

        private void MainWinViewModel_BottomRightXCoorAdjChanged(object sender, EventArgs e)
        {
            if (currentCalibStep == CalibrationStep.Done)
            {
                bottomRightCalibPoint.X = (float)bottomRightXCoorAdj;
                gunMappingConfig.RemoteMapping.MappingPoints.BottomRightX = bottomRightXCoorAdj;
                GenerateCalibPointOutput();
            }
        }

        private void DetectPreviousMapping()
        {
            string testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                APP_NAME_FOLDER, "mappings.json");

            if (!Directory.Exists(Path.GetDirectoryName(testPath)))
            {
                string dirPath = Path.GetDirectoryName(testPath);
                Directory.CreateDirectory(dirPath);
            }

            gunMappingConfig = null;
            if (File.Exists(testPath))
            {
                string json = string.Empty;
                using (StreamReader sr = new StreamReader(testPath))
                {
                    json = sr.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(json))
                {
                    bool fileRead = false;

                    try
                    {
                        WiiGunMappingConfig config = JsonSerializer.Deserialize<WiiGunMappingConfig>(json);
                        gunMappingConfig = config;
                        fileRead = true;
                    }
                    catch (JsonException)
                    {
                        // File parsing failed. Make blank instance of config
                        gunMappingConfig = new WiiGunMappingConfig();
                    }

                    if (fileRead)
                    {
                        currentCalibStep = CalibrationStep.Done;
                        EstablishDoneFromMapping();
                        SetupDone();
                    }
                }
            }
            else
            {
                gunMappingConfig = new WiiGunMappingConfig();
            }
        }

        public void SaveMappingConfig()
        {
            if (currentCalibStep != CalibrationStep.Done || gunMappingConfig == null)
            {
                return;
            }

            string testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                APP_NAME_FOLDER, "mappings.json");

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize(gunMappingConfig, options);
            try
            {
                using (StreamWriter sw = new StreamWriter(testPath, false))
                {
                    sw.Write(json);
                }
            }
            catch (IOException) { }
        }

        private void MainWinViewModel_CenterYCoorAdjChanged(object sender, EventArgs e)
        {
            if (currentCalibStep == CalibrationStep.Done)
            {
                centerCalibPoint.Y = (float)centerYCoorAdj;
                gunMappingConfig.RemoteMapping.MappingPoints.CenterY = centerYCoorAdj;
                GenerateCalibPointOutput();
                FinalizeCalibrationData();
            }
        }

        private void MainWinViewModel_CenterXCoorAdjChanged(object sender, EventArgs e)
        {
            if (currentCalibStep == CalibrationStep.Done)
            {
                centerCalibPoint.X = (float)centerXCoorAdj;
                gunMappingConfig.RemoteMapping.MappingPoints.CenterX = centerXCoorAdj;
                GenerateCalibPointOutput();
                FinalizeCalibrationData();
            }
        }

        private void MainWinViewModel_TopLeftYCoorAdjChanged(object sender, EventArgs e)
        {
            if (currentCalibStep == CalibrationStep.Done)
            {
                topLeftCalibPoint.Y = (float)topLeftYCoorAdj;
                gunMappingConfig.RemoteMapping.MappingPoints.TopLeftY = topLeftYCoorAdj;
                GenerateCalibPointOutput();
            }
        }

        private void MainWinViewModel_TopLeftXCoorAdjChanged(object sender, EventArgs e)
        {
            if (currentCalibStep == CalibrationStep.Done)
            {
                topLeftCalibPoint.X = (float)topLeftXCoorAdj;
                gunMappingConfig.RemoteMapping.MappingPoints.TopLeftX = topLeftXCoorAdj;
                GenerateCalibPointOutput();
            }
        }

        public void FindWiimotes()
        {
            mWC.FindAllWiimotes();

            int index = 1;
            foreach (Wiimote wm in mWC)
            {
                if (wm.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                    wm.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);

                //if (index == 1)
                {
                    wm.WiimoteChanged += Wm_WiimoteChanged;
                }

                wiimoteList.Add(wm);
                index++;
            }
        }

        public void SetCanvasDimensions(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;

            SetupGunImgCoords();
        }

        private void SetupGunImgCoords()
        {
            gunImageTop = (int)(2 - (GUN_IMG_HEIGHT / 2.0));
            gunImageLeft = (int)(2 - (GUN_IMG_WIDTH / 2.0));

            gunImageBottom = (int)(canvasHeight - (GUN_IMG_HEIGHT / 2.0));
            gunImageRight = (int)(canvasWidth - (GUN_IMG_WIDTH / 2.0));

            gunCenterTop = (int)((canvasHeight - GUN_IMG_HEIGHT) / 2.0);
            gunCenterLeft = (int)((canvasWidth - GUN_IMG_WIDTH) / 2.0);
        }

        private void Wm_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            WiimoteState ws = e.WiimoteState;

            WiimoteStatePreprocess?.Invoke(this, EventArgs.Empty);

            //stateData.MidPointX = 1.0 - ws.IRState.Midpoint.X;
            //stateData.MidPointY = ws.IRState.Midpoint.Y;

            //MidPointString = $"{stateData.MidPointX} {stateData.MidPointY}";

            currentPoint = CalculateCurrentPoint(ws);
            outputPoint = GetTransformPosition(currentPoint);

            stateData.MidPointX = currentPoint.medianX;
            stateData.MidPointY = currentPoint.medianY;

            MidPointString = $"{currentPoint.medianX} {currentPoint.medianY}";

            //Trace.WriteLine($"{stateData.MidPointX} {stateData.MidPointY}");

            //ws.IRState.Midpoint.X;
            //ws.IRState.Midpoint.Y;

            currentBState = ws.ButtonState.B;

            if (currentCalibStep != CalibrationStep.None &&
                currentCalibStep != CalibrationStep.Done)
            {
                if (previousBState && !currentBState)
                {
                    bool found = !currentPoint.OutOfReach && currentPoint.fourPointsFound;
                    if (found)
                    {
                        NextCalibrationStep();
                    }
                    else
                    {
                        WarningHelpText = "Failed to find a median point. Please try again";
                    }
                }
            }
            else if (currentCalibStep == CalibrationStep.Done)
            {
                outputPoint = GetTransformPosition(currentPoint);

                double tempX = (outputPoint.X - topLeftCalibPoint.X) /
                    (bottomRightCalibPoint.X - topLeftCalibPoint.X);

                double tempY = (outputPoint.Y - topLeftCalibPoint.Y) /
                    (bottomRightCalibPoint.Y - topLeftCalibPoint.Y);

                lightGunPointX = (int)(canvasWidth * Math.Clamp(tempX, 0.0, 1.0));
                lightGunPointY = (int)(canvasHeight * Math.Clamp(tempY, 0.0, 1.0));
                //lightGunPointX = (int)ws.IRState.Midpoint.X;
                //lightGunPointY = (int)ws.IRState.Midpoint.Y;

                //LightGunPointXChanged?.Invoke(this, EventArgs.Empty);
                //LightGunPointYChanged?.Invoke(this, EventArgs.Empty);
                
            }

            StateDataChanged?.Invoke(this, EventArgs.Empty);
            previousBState = currentBState;
        }

        private PointF GetTransformPosition(CursorPos cursor)
        {
            PointF tempPoint = new PointF();

            pWarper.setSource(finalPos[0].X, finalPos[0].Y,
                finalPos[1].X, finalPos[1].Y, finalPos[2].X, finalPos[2].Y,
                finalPos[3].X, finalPos[3].Y);
            float dsx = 0.0f, dsy = 0.0f;
            pWarper.warp((float)centerXCoorAdj, (float)centerYCoorAdj, ref dsx, ref dsy);
            tempPoint.X = dsx;
            tempPoint.Y = dsy;

            return tempPoint;
        }

        private void FinalizeCalibrationData()
        {
            pWarper.setDestination(TRled, 0.0f, TLled, 0.0f, TLled, 1.0f, TRled, 1.0f);
        }

        private CursorPos CalculateCurrentPoint(WiimoteState wiimoteState)
        {
            CursorPos resultPos = new CursorPos(0.0f, 0.0f, false);
            //PointF tempPoint = new PointF();

            IRState irState = wiimoteState.IRState;
            byte seenFlags = 0;
            double Roll = Math.Atan2(wiimoteState.AccelState.Values.X, wiimoteState.AccelState.Values.Z);

            bool fourPtsFound = irState.IRSensors[0].Found == true &&
                irState.IRSensors[1].Found == true &&
                irState.IRSensors[2].Found == true && irState.IRSensors[3].Found == true;

            if (fourPtsFound)
            {
                median.Y = (irState.IRSensors[0].Position.Y + irState.IRSensors[1].Position.Y + irState.IRSensors[2].Position.Y + irState.IRSensors[3].Position.Y + 0.002f) / 4;
                median.X = (irState.IRSensors[0].Position.X + irState.IRSensors[1].Position.X + irState.IRSensors[2].Position.X + irState.IRSensors[3].Position.X + 0.002f) / 4;
            }

            for (int i = 0; i < 4; i++)
            {
                if (irState.IRSensors[i].Found)
                {
                    double point_angle = Math.Atan2(irState.IRSensors[i].Position.Y - median.Y, irState.IRSensors[i].Position.X - median.X) - Roll;
                    if (point_angle < 0) point_angle += 2 * Math.PI;

                    int index = (int)(point_angle / (Math.PI / 2));

                    finalPos[index] = irState.IRSensors[i].Position;
                    see[index] = (see[index] << 1) | 1;
                    seenFlags |= (byte)(1 << index);
                }
                else
                    see[i] = 0;
            }

            // Perform check early. Make sure at least 3 real IR points were detected.
            if (see.Count(seen => seen == 0) >= 2)
            {
                CursorPos err = lastPos;
                //CursorPos err = new CursorPos(0.0f, 0.0f, true);
                //err.medianX = 0.0f;
                //err.medianY = 0.0f;
                err.OutOfReach = true;
                err.fourPointsFound = false;
                //err.OffScreen = true;

                return err;
            }

            while ((seenFlags & 15) != 0 && (seenFlags & 15) != 15)
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((seenFlags & (1 << i)) == 0)
                    {
                        see[i] = 0;
                        int[] neighbors;
                        switch (i)
                        {
                            case 0:
                                neighbors = new[] { 3, 1 };
                                break;
                            case 1:
                                neighbors = new[] { 2, 0 };
                                break;
                            case 2:
                                neighbors = new[] { 1, 3 };
                                break;
                            case 3:
                                neighbors = new[] { 0, 2 };
                                break;
                            default:
                                neighbors = Array.Empty<int>();
                                break;
                        }

                        foreach (int neighbor in neighbors)
                        {
                            float f = 0;
                            if ((seenFlags & (1 << neighbor)) != 0) // Check if the bit for the neighbor is set
                            {
                                switch (i)
                                {
                                    case 0:
                                        f = angleBottom - angleOffset[neighbor];
                                        break;
                                    case 1:
                                        f = angleBottom + (angleOffset[neighbor] - MathF.PI);
                                        break;
                                    case 2:
                                        f = angleTop + angleOffset[neighbor];
                                        break;
                                    case 3:
                                        f = angleTop - (angleOffset[neighbor] - MathF.PI);
                                        break;
                                }
                            }

                            float distance = 0;
                            switch (i)
                            {
                                case 0:
                                    distance = (neighbor == 3) ? yDistRight : xDistBottom;
                                    break;
                                case 1:
                                    distance = (neighbor == 2) ? yDistLeft : xDistBottom;
                                    break;
                                case 2:
                                    distance = (neighbor == 1) ? yDistLeft : xDistTop;
                                    break;
                                case 3:
                                    distance = (neighbor == 0) ? yDistRight : xDistTop;
                                    break;
                            }

                            finalPos[i].X = finalPos[neighbor].X + distance * MathF.Cos(f);
                            finalPos[i].Y = finalPos[neighbor].Y + distance * -MathF.Sin(f);
                            seenFlags |= (byte)(1 << i);
                            break;
                        }
                    }
                }
                if ((seenFlags & 15) == 15) break;
            }

            if (!fourPtsFound)
            {
                median.Y = (finalPos[0].Y + finalPos[1].Y + finalPos[2].Y + finalPos[3].Y + 0.002f) / 4;
                median.X = (finalPos[0].X + finalPos[1].X + finalPos[2].X + finalPos[3].X + 0.002f) / 4;
            }

            // If 4 LEDS can be seen and loop has run through 5 times update offsets and height
            if (((1 << 5) & see[0] & see[1] & see[2] & see[3]) != 0)
            {
                angleOffset[0] = angleTop - (angleLeft - MathF.PI);
                angleOffset[1] = -(angleTop - angleRight);
                angleOffset[2] = -(angleBottom - angleLeft);
                angleOffset[3] = angleBottom - (angleRight - MathF.PI);
                ledsHeight = (yDistLeft + yDistRight) / 2.0f;
                ledsWidth = (xDistTop + xDistBottom) / 2.0f;
            }

            // If 2 LEDS can be seen and loop has run through 5 times update angle and distances
            if (((1 << 5) & see[2] & see[1]) != 0)
            {
                angleLeft = MathF.Atan2(finalPos[1].Y - finalPos[2].Y, finalPos[2].X - finalPos[1].X);
                yDistLeft = MathF.Hypot((finalPos[2].Y - finalPos[1].Y), (finalPos[2].X - finalPos[1].X));
            }

            if (((1 << 5) & see[0] & see[3]) != 0)
            {
                angleRight = MathF.Atan2(finalPos[0].Y - finalPos[3].Y, finalPos[3].X - finalPos[0].X);
                yDistRight = MathF.Hypot((finalPos[0].Y - finalPos[3].Y), (finalPos[0].X - finalPos[3].X));
            }

            if (((1 << 5) & see[2] & see[3]) != 0)
            {
                angleTop = MathF.Atan2(finalPos[2].Y - finalPos[3].Y, finalPos[3].X - finalPos[2].X);
                xDistTop = MathF.Hypot((finalPos[2].Y - finalPos[3].Y), (finalPos[2].X - finalPos[3].X));
            }

            if (((1 << 5) & see[0] & see[1]) != 0)
            {
                angleBottom = MathF.Atan2(finalPos[1].Y - finalPos[0].Y, finalPos[0].X - finalPos[1].X);
                xDistBottom = MathF.Hypot((finalPos[1].Y - finalPos[0].Y), (finalPos[1].X - finalPos[0].X));
            }

            // Add tilt correction
            //angle = -(MathF.Atan2(finalPos[0].Y - finalPos[1].Y, finalPos[1].X - finalPos[0].X) + MathF.Atan2(finalPos[2].Y - finalPos[3].Y, finalPos[3].X - finalPos[2].X)) / 2;
            //if (angle < 0) angle += MathF.PI * 2;
            angle = Roll;
            //Trace.WriteLine($"ANGLE: {angle} | ROLL {Roll}");

            /*if (see.Count(seen => seen == 0) >= 2 || Double.IsNaN(median.X) ||
                Double.IsNaN(median.Y))
            {
                CursorPos err = lastPos;
                err.OutOfReach = true;
                err.fourPointsFound = false;

                return err;
            }
            */

            resultPos.OutOfReach = false;
            resultPos.fourPointsFound = fourPtsFound;
            resultPos.medianX = median.X;
            resultPos.medianY = median.Y;
            lastPos = resultPos;

            return resultPos;
        }

        public void UpdateLightGunPoint()
        {
            LightGunPointXChanged?.Invoke(this, EventArgs.Empty);
            LightGunPointYChanged?.Invoke(this, EventArgs.Empty);
        }

        public void CheckStartProcess()
        {
            if (currentCalibStep != CalibrationStep.Done)
            {
                StartCalibration();
            }
        }

        public void StartCalibration()
        {
            NextCalibrationStep();
        }

        public void Reset()
        {
            currentCalibStep = CalibrationStep.None;
            SetupReset();
        }

        public void NextCalibrationStep()
        {
            switch(currentCalibStep)
            {
                case CalibrationStep.None:
                    currentCalibStep = CalibrationStep.CenterScreen;
                    SetupCenterScreen();
                    break;
                case CalibrationStep.CenterScreen:
                    double sin = Math.Sin(angle * -1);
                    double cos = Math.Cos(angle * -1);

                    float tempX = currentPoint.medianX - 0.5f;
                    float tempY = currentPoint.medianY - 0.5f;

                    float xnew = (float)(tempX * cos - tempY * sin);
                    float ynew = (float)(tempX * sin + tempY * cos);

                    tempX = xnew + 0.5f;
                    tempY = ynew + 0.5f;
                    centerCalibPoint.X = (float)(tempX);
                    centerCalibPoint.Y = (float)tempY;
                    centerXCoorAdj = centerCalibPoint.X;
                    centerYCoorAdj = centerCalibPoint.Y;

                    TLled = (float)(0.5 - ((ledsWidth / ledsHeight) / 4.0f));
                    TRled = (float)(0.5 + ((ledsWidth / ledsHeight) / 4.0f));

                    currentCalibStep = CalibrationStep.TopLeft;
                    SetupTopLeftScreen();
                    FinalizeCalibrationData();
                    break;
                case CalibrationStep.TopLeft:
                    topLeftCalibPoint.X = (float)(outputPoint.X);
                    topLeftCalibPoint.Y = (float)outputPoint.Y;
                    topLeftXCoorAdj = topLeftCalibPoint.X;
                    topLeftYCoorAdj = topLeftCalibPoint.Y;
                    //topLeftCalibPoint.X = (float)Math.Clamp(stateData.MidPointX - diffX,
                    //    0.0, 1.0);
                    //topLeftCalibPoint.Y = (float)Math.Clamp(stateData.MidPointY - diffY,
                    //    0.0, 1.0);

                    currentCalibStep = CalibrationStep.BottomRight;
                    SetupBottomRightScreen();
                    //currentCalibStep = CalibrationStep.Done;
                    //SetupDone();
                    break;
                case CalibrationStep.BottomRight:
                    bottomRightCalibPoint.X = (float)(outputPoint.X);
                    bottomRightCalibPoint.Y = (float)outputPoint.Y;
                    bottomRightXCoorAdj = bottomRightCalibPoint.X;
                    bottomRightYCoorAdj = bottomRightCalibPoint.Y;
                    currentCalibStep = CalibrationStep.Done;
                    SetupDone();
                    break;
                case CalibrationStep.Done:
                    currentCalibStep = CalibrationStep.None;
                    SetupReset();
                    break;
                default: break;
            }
        }

        private void SetupReset()
        {
            displayTopLeftGunImg = false;
            displayCenterGunImg = false;
            displayBottomRightGunImg = false;
            lightGunPointVisible = false;
            topLeftCalibPoint = new PointF();
            bottomRightCalibPoint = new PointF();
            centerCalibPoint = new PointF();
            TLled = TRled = 0.0f;
            CalibPointString = string.Empty;
            CurrentStepHelpText = string.Empty;
            WarningHelpText = string.Empty;
            DisplayDoneVis = false;

            DisplayTopLeftGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayBottomRightGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayCenterGunImgChanged?.Invoke(this, EventArgs.Empty);
            LightGunPointVisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetupCenterScreen()
        {
            displayTopLeftGunImg = false;
            displayBottomRightGunImg = false;
            displayCenterGunImg = true;
            topLeftCalibPoint = new PointF();
            bottomRightCalibPoint = new PointF();
            centerCalibPoint = new PointF();
            CurrentStepHelpText = "Aim for center point and press B";
            WarningHelpText = string.Empty;
            DisplayDoneVis = false;

            DisplayTopLeftGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayBottomRightGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayCenterGunImgChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetupTopLeftScreen()
        {
            displayTopLeftGunImg = true;
            displayCenterGunImg = false;
            displayBottomRightGunImg = false;
            CurrentStepHelpText = "Aim for top left point and press B";
            WarningHelpText = string.Empty;

            DisplayTopLeftGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayBottomRightGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayCenterGunImgChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetupBottomRightScreen()
        {
            displayTopLeftGunImg = false;
            displayCenterGunImg = false;
            displayBottomRightGunImg = true;
            CurrentStepHelpText = "Aim for top left point and press B";
            WarningHelpText = string.Empty;

            DisplayTopLeftGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayBottomRightGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayCenterGunImgChanged?.Invoke(this, EventArgs.Empty);
        }

        private void EstablishDoneFromMapping()
        {
            topLeftXCoorAdj = gunMappingConfig.RemoteMapping.MappingPoints.TopLeftX;
            topLeftCalibPoint.X = (float)gunMappingConfig.RemoteMapping.MappingPoints.TopLeftX;
            //builder.AppendLine($"{topLeftCalibPoint.X} {topLeftCalibPoint.Y}");
            //builder.AppendLine($"{centerCalibPoint.X} {centerCalibPoint.Y}");

            topLeftYCoorAdj = gunMappingConfig.RemoteMapping.MappingPoints.TopLeftY;
            topLeftCalibPoint.Y = (float)gunMappingConfig.RemoteMapping.MappingPoints.TopLeftY;


            bottomRightXCoorAdj = gunMappingConfig.RemoteMapping.MappingPoints.BottomRightX;
            bottomRightCalibPoint.X = (float)gunMappingConfig.RemoteMapping.MappingPoints.BottomRightX;

            bottomRightYCoorAdj = gunMappingConfig.RemoteMapping.MappingPoints.BottomRightY;
            bottomRightCalibPoint.Y = (float)gunMappingConfig.RemoteMapping.MappingPoints.BottomRightY;


            centerXCoorAdj = gunMappingConfig.RemoteMapping.MappingPoints.CenterX;
            centerCalibPoint.X = (float)gunMappingConfig.RemoteMapping.MappingPoints.CenterX;


            centerYCoorAdj = gunMappingConfig.RemoteMapping.MappingPoints.CenterY;
            centerCalibPoint.Y = (float)gunMappingConfig.RemoteMapping.MappingPoints.CenterY;

            TLled = (float)gunMappingConfig.RemoteMapping.MappingPoints.TLled;
            TRled = (float)gunMappingConfig.RemoteMapping.MappingPoints.TRled;
        }

        private void SetupDone()
        {
            displayTopLeftGunImg = true;
            displayBottomRightGunImg = true;
            displayCenterGunImg = true;
            lightGunPointVisible = true;
            CurrentStepHelpText = string.Empty;
            WarningHelpText = string.Empty;
            DisplayDoneVis = true;

            DisplayTopLeftGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayBottomRightGunImgChanged?.Invoke(this, EventArgs.Empty);
            DisplayCenterGunImgChanged?.Invoke(this, EventArgs.Empty);
            LightGunPointVisibleChanged?.Invoke(this, EventArgs.Empty);
            TopLeftXCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            TopLeftYCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            BottomRightXCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            BottomRightYCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            CenterXCoorAdjChanged?.Invoke(this, EventArgs.Empty);
            CenterYCoorAdjChanged?.Invoke(this, EventArgs.Empty);

            GenerateCalibPointOutput();
            FinalizeCalibrationData();
            SaveCalibPointConfigInfo();
        }

        private void SaveCalibPointConfigInfo()
        {
            //gunMappingConfig.RemoteMaping.Find((t) => t.Serial)
            if (gunMappingConfig != null)
            {
                gunMappingConfig.RemoteMapping.MappingPoints = new MappingPoints()
                {
                    TopLeftX = topLeftXCoorAdj,
                    TopLeftY = topLeftYCoorAdj,
                    BottomRightX = bottomRightXCoorAdj,
                    BottomRightY = bottomRightYCoorAdj,
                    CenterX = centerXCoorAdj,
                    CenterY = centerYCoorAdj,
                };
            }
        }

        private void RefreshCalibPointConfigInfo()
        {
            if (gunMappingConfig != null)
            {
                MappingPoints mapPoints = gunMappingConfig.RemoteMapping.MappingPoints;

                mapPoints.TopLeftX = topLeftXCoorAdj;
                mapPoints.TopLeftY = topLeftYCoorAdj;
                mapPoints.BottomRightX = bottomRightXCoorAdj;
                mapPoints.BottomRightY = bottomRightYCoorAdj;
                mapPoints.CenterX = centerXCoorAdj;
                mapPoints.CenterY = centerYCoorAdj;
            }
        }

        private void GenerateCalibPointOutput()
        {
            StringBuilder builder = new StringBuilder();
            //builder.AppendLine($"OG TL {origTopLeftCalibPoint.X} | {origTopLeftCalibPoint.Y}");
            builder.AppendLine($"\"Top\": {string.Create(CultureInfo.InvariantCulture, $"{topLeftCalibPoint.Y}")},");
            builder.AppendLine($"\"Bottom\": {string.Create(CultureInfo.InvariantCulture, $"{bottomRightCalibPoint.Y}")},");

            builder.AppendLine($"\"Left\": {string.Create(CultureInfo.InvariantCulture, $"{topLeftCalibPoint.X}")},");
            builder.AppendLine($"\"Right\": {string.Create(CultureInfo.InvariantCulture, $"{bottomRightCalibPoint.X}")},");

            builder.AppendLine($"\"CenterX\": {string.Create(CultureInfo.InvariantCulture, $"{centerCalibPoint.X}")},");
            builder.AppendLine($"\"CenterY\": {string.Create(CultureInfo.InvariantCulture, $"{centerCalibPoint.Y}")},");

            builder.AppendLine($"\"TLled\": {string.Create(CultureInfo.InvariantCulture, $"{TLled}")},");
            builder.AppendLine($"\"TRled\": {string.Create(CultureInfo.InvariantCulture, $"{TRled}")},");
            CalibPointString = builder.ToString();
            //CalibPointStringChanged?.Invoke(this, EventArgs.Empty);

            // Debug cmd output
            //Trace.WriteLine($"OG TL {origTopLeftCalibPoint.X} | {origTopLeftCalibPoint.Y}");
            Trace.WriteLine($"Top {string.Create(CultureInfo.InvariantCulture, $"{topLeftCalibPoint.X}")} {string.Create(CultureInfo.InvariantCulture, $"{topLeftCalibPoint.Y}")}");
            Trace.WriteLine($"Bottom {string.Create(CultureInfo.InvariantCulture, $"{bottomRightCalibPoint.Y}")} {string.Create(CultureInfo.InvariantCulture, $"{bottomRightCalibPoint.Y}")}");
            Trace.WriteLine($"Left {string.Create(CultureInfo.InvariantCulture, $"{topLeftCalibPoint.X}")} {string.Create(CultureInfo.InvariantCulture, $"{topLeftCalibPoint.X}")}");
            Trace.WriteLine($"Right {string.Create(CultureInfo.InvariantCulture, $"{bottomRightCalibPoint.X}")} {string.Create(CultureInfo.InvariantCulture, $"{bottomRightCalibPoint.X}")}");
            
            Trace.WriteLine($"CenterX {string.Create(CultureInfo.InvariantCulture, $"{centerCalibPoint.X}")} {string.Create(CultureInfo.InvariantCulture, $"{centerCalibPoint.X}")}");
            Trace.WriteLine($"CenterY {string.Create(CultureInfo.InvariantCulture, $"{centerCalibPoint.Y}")} {string.Create(CultureInfo.InvariantCulture, $"{centerCalibPoint.Y}")}");

            Trace.WriteLine($"TLled {string.Create(CultureInfo.InvariantCulture, $"{TLled}")}");
            Trace.WriteLine($"TRled {string.Create(CultureInfo.InvariantCulture, $"{TRled}")}");
            Trace.WriteLine("");
        }

        public void TearDown()
        {
            //SaveMappingConfig();
            StateDataChanged = null;

            foreach (Wiimote wm in mWC)
            {
                wm.Disconnect();
            }

            mWC.Clear();
            wiimoteList.Clear();
        }
    }

    public class WiimoteStateData
    {
        private double midPointX;
        public double MidPointX
        {
            get => midPointX;
            set => midPointX = value;
        }

        private double midPointY;
        public double MidPointY
        {
            get => midPointY;
            set => midPointY = value;
        }

        private double lightGunX;
        public double LightGunX
        {
            get => lightGunX;
            set => lightGunX = value;
        }

        private double lightGunY;
        public double LightGunY
        {
            get => lightGunY;
            set => lightGunY = value;
        }
    }



    public class WiiGunMappingConfig
    {
        private WiimoteMapping remoteMapping = new WiimoteMapping();
        public WiimoteMapping RemoteMapping
        {
            get => remoteMapping;
            set => remoteMapping = value;
        }
    }

    public class WiimoteMapping
    {
        //private string serial;
        //public string Serial
        //{
        //    get => serial;
        //    set => serial = value;
        //}

        private MappingPoints mappingPoints = new MappingPoints();
        public MappingPoints MappingPoints
        {
            get => mappingPoints;
            set => mappingPoints = value;
        }
    }

    public class MappingPoints
    {
        private double topLeftX;
        public double TopLeftX
        {
            get => topLeftX;
            set => topLeftX = value;
        }

        private double topLeftY;
        public double TopLeftY
        {
            get => topLeftY;
            set => topLeftY = value;
        }


        private double bottomRightX;
        public double BottomRightX
        {
            get => bottomRightX;
            set => bottomRightX = value;
        }

        private double bottomRightY;
        public double BottomRightY
        {
            get => bottomRightY;
            set => bottomRightY = value;
        }


        private double centerX;
        public double CenterX
        {
            get => centerX;
            set => centerX = value;
        }

        private double centerY;
        public double CenterY
        {
            get => centerY;
            set => centerY = value;
        }

        private double tlLed;
        public double TLled
        {
            get => tlLed;
            set => tlLed = value;
        }

        private double trLed;
        public double TRled
        {
            get => trLed;
            set => trLed = value;
        }
    }

    public static class MathF
    {
        public const float PI = (float)Math.PI;
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float Cos(float d) => (float)Math.Cos(d);
        public static float Round(float a) => (float)Math.Round(a);
        public static float Sin(float a) => (float)Math.Sin(a);
        public static float Hypot(float p, float b) => (float)Math.Sqrt(Math.Pow(p, 2) + Math.Pow(b, 2));
        public static float Sqrt(float d) => (float)Math.Sqrt(d);
        public static float Max(float val1, float val2) => (float)Math.Max(val1, val2);
        public static float Min(float val1, float val2) => (float)Math.Min(val1, val2);
    }
}
