/**
Fonte: https://github.com/mono/opentk/blob/master/Source/Examples/OpenGL/1.x/Textures.cs
 */
using System;
using System.Drawing;
using CG_Biblioteca;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace textura
{
  class Render : GameWindow
  {
    //FIXME: precisei instalar $ brew install mono-libgdiplus
    int texture;

    Esfera sol = new Esfera("Sol");
     Esfera terra = new Esfera("Terra");
     Esfera lua = new Esfera("Lua");

     CameraPerspective camera = new CameraPerspective();

     Vector3 eye, target, up;

     bool seguirTerra = false;
     bool stop = false;

     float speed = 1;

    public void OrigemCamera()
    {
      
      camera.Eye = new Vector3(6, 6, 6);
      camera.At =  new Vector3(0, 0, 0);
    }

    public Render(int width, int height) : base(width, height) { }

    protected override void OnLoad(EventArgs e)
    { 
      CameraReprojetaTela();
      base.OnLoad(e);
      GL.ClearColor(Color.Black);
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.CullFace);
      
      GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

      sol.CarregaTextura("sun.jpg");
      terra.CarregaTextura("earth.bmp");
      lua.CarregaTextura("moon.png");

      terra.TranslacaoXYZ(6,0,1);
      lua.TranslacaoXYZ(2,0,1);

      sol.EscalaXYZBBox(0.5);
      terra.EscalaXYZBBox(0.8);
      lua.EscalaXYZBBox(0.7);

      sol.FilhoAdicionar(terra);
      terra.FilhoAdicionar(lua);
      
      OrigemCamera();
    }

    protected override void OnUnload(EventArgs e)
    {
      sol.RemoveTextura();
      terra.RemoveTextura();
      lua.RemoveTextura();
    }

    protected override void OnResize(EventArgs e)
    {
      CameraReprojetaTela();
      base.OnResize(e);
      GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.Fovy, camera.Aspect, camera.Near, camera.Far);
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref projection);
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    { 
      if(!stop)
      {
        Animacao();
      }
      
      if(seguirTerra){
      CameraTerra();
    }
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      Matrix4 modelview = Matrix4.LookAt(camera.Eye,camera.At, camera.Up);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadMatrix(ref modelview);

      SRU3D();
      sol.Desenhar();

      SwapBuffers();
    }

    public void AtualizaProjecao(){
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.Fovy, camera.Aspect, camera.Near, camera.Far);
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref projection);
    }
    public void CameraReprojetaTela(){
      camera.Fovy = (float)Math.PI / 4;
      camera.Aspect = Width / (float)Height;
      camera.Near = 1.0f;
      camera.Far = 50.0f;
      AtualizaProjecao();
    }

    public void Animacao()
    {
      terra.RotacaoY(0.3);   // em torno do sol
      terra.RotacaoYBBox(15); // rotacao da terra
      lua.RotacaoY(-13);
    }


    public void projecaoTerra(){
      camera.Near = 3f;
      camera.Far = 10;
      camera.Fovy = camera.Fovy/2;
      AtualizaProjecao();
    }

    public void CameraTerra(){
      Ponto4D centro = terra.GetBBox().obterCentro;
      Transformacao4D t = terra.GetMatriz();
      centro = t.MultiplicarPonto(centro);
      Vector3 v = new Vector3((float)centro.X, 0, (float)centro.Z);
      camera.Eye = v;
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        this.Exit();
      if(e.Key == Key.T){
        seguirTerra = true;
        projecaoTerra();
        CameraTerra();
      }
      if(e.Key == Key.N){
        seguirTerra = false;
        CameraReprojetaTela();
        OrigemCamera();
      }
      if(e.Key == Key.P){
        stop = true;
      }
      if(e.Key == Key.C){
        stop = false;
      }


      // else
      //   if (e.Key == Key.F)
      //   GL.CullFace(CullFaceMode.Front);
      // if (e.Key == Key.B)
      //   GL.CullFace(CullFaceMode.Back);
      // if (e.Key == Key.A)
      // //FIXME: aqui deveria aplicar a textura no lado de fora e dentro, mas não aparece nada
      //   GL.CullFace(CullFaceMode.FrontAndBack);
    }


    private void SRU3D()
    {
      GL.LineWidth(3);
      GL.Begin(PrimitiveType.Lines);
      GL.Color3(Color.Red);
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      GL.Color3(Color.Green);
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      GL.Color3(Color.Blue);
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();
    }

  }

  class Program
  {
    static void Main(string[] args)
    {
      Render window = new Render(600, 600);
      window.Run(1.0 / 60.0);
    }
  }

}