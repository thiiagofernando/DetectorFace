using DetectorDeFace.Models;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DetectorDeFace
{

    public partial class Form1 : Form
    {
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.6d, 0.6d);
        HaarCascade faceDetect;
        Image<Bgr, Byte> Frame;
        Capture camera;
        Image<Gray, byte> result;
        Image<Gray, byte> TrainedFace = null;
        Image<Gray, byte> grayFace = null;
        List<Image<Gray, byte>> tranningImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> Users = new List<string>();
        int Count;
        string name = null;

        public Form1()
        {
            InitializeComponent();
            faceDetect = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                using (var db = new BdDataContext())
                {
                    //Busca do Banco sql Server
                    var Labelsinf = db.faces.ToList();
                    string FaceLoad;
                    foreach (var item in Labelsinf)
                    {
                        FaceLoad = "face" + item.Id + ".bmp";
                        tranningImages.Add(new Image<Gray, byte>(Application.StartupPath + $"/Faces/{FaceLoad}"));
                        labels.Add(Labelsinf.Where(d => d.Id == item.Id).First().NomeFace);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Opss: " + ex.Message);
            }
        }

        private void BtnCapturar_Click(object sender, EventArgs e)
        {
            try
            {
                camera = new Capture();
                camera.QueryFrame();
                Application.Idle += new EventHandler(FrameProcedure);
                btnCapturar.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Camera já esta ativada");
            }
        }
        private void BnSavarFace_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new BdDataContext())
                {
                    labels.Clear();
                    tranningImages.Clear();
                    Count = Count + 1;
                    grayFace = camera.QueryGrayFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);
                    MCvAvgComp[][] Detectfaces = grayFace.DetectHaarCascade(faceDetect, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
                    foreach (MCvAvgComp f in Detectfaces[0])
                    {
                        TrainedFace = Frame.Copy(f.rect).Convert<Gray, byte>();
                        break;
                    }
                    TrainedFace = result.Resize(100, 100, INTER.CV_INTER_CUBIC);
                    tranningImages.Add(TrainedFace);
                    labels.Add(txtName.Text);

                    int lista = tranningImages.Count();
                    for (int i=0; i < lista; i++)
                    {
                        //salva no o Banco sql Server
                        Face face = new Face
                        {
                            NomeFace = txtName.Text,
                            DataCadastro = DateTime.Now
                        };
                        db.faces.Add(face);
                        db.SaveChanges();
                        tranningImages.ToArray()[0].Save(Application.StartupPath + "/Faces/" + "face" + face.Id + ".bmp");
                        MessageBox.Show(face.NomeFace +" Add com Sucesso!!");
                        txtName.Clear();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FrameProcedure(object sender, EventArgs e)
        {
            try
            {
                Users.Add("");
                Frame = camera.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);
                grayFace = Frame.Convert<Gray, Byte>();
                MCvAvgComp[][] facedetectNow = grayFace.DetectHaarCascade(faceDetect, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
                foreach (MCvAvgComp item in facedetectNow[0])
                {
                    result = Frame.Copy(item.rect).Convert<Gray, Byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    Frame.Draw(item.rect, new Bgr(Color.Green), 3);
                    if (tranningImages.ToArray().Length != 0)
                    {
                        MCvTermCriteria termCriteria = new MCvTermCriteria(Count, 0.001);
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(tranningImages.ToArray(), labels.ToArray(), 1500, ref termCriteria);
                        name = recognizer.Recognize(result);
                        Frame.Draw(name, ref font, new Point(item.rect.X - 2, item.rect.Y - 2), new Bgr(Color.Red));
                    }

                    Users.Add("");
                }
                CameraBox.Image = Frame;
                //string names = "";
                Users.Clear();
            }
            catch (System.AccessViolationException)
            {
                MessageBox.Show("Camera já esta ativada");
            }
            catch (Exception)
            {
                MessageBox.Show("Camera já esta ativada");
            }
        }
    }
}
